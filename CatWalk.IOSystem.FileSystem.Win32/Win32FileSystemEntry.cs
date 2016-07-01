/*
	$Id: FileSystemEntry.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using CatWalk.IO;

namespace CatWalk.IOSystem.FileSystem.Win32 {
	using IO = System.IO;
	public class Win32FileSystemEntry : FileSystemEntry, IWin32FileSystemEntry{
		private Lazy<bool> _IsDirectory;

		public Win32FileSystemEntry(ISystemEntry parent, string name, string path) : base(parent, name, path){
			this.Initialize(() => IO::Directory.Exists(this.FileSystemPath.FullPath));
		}

		internal Win32FileSystemEntry(ISystemEntry parent, string name, string path, bool isDirectory) : base(parent, name, path){
			this.Initialize(() => isDirectory);
		}

		private void Initialize(Func<bool> isDirectory){
			this._IsDirectory = new Lazy<bool>(isDirectory);
		}

		public override bool IsDirectory {
			get {
				return this._IsDirectory.Value;
			}
		}

		public override bool IsExists(CancellationToken token, IProgress<double> progress) {
			if(this.IsDirectory) {
				return Directory.Exists(this.FileSystemPath.FullPath);
			} else {
				return File.Exists(this.FileSystemPath.FullPath);
			}
		}

		public override IFilePathFormat FilePathFormat {
			get {
				return FilePathFormats.Windows;
			}
		}

		public override StringComparison StringComparison {
			get {
				return StringComparison.OrdinalIgnoreCase;
			}
		}


		#region Directory

		public override bool Contains(string name, CancellationToken token, IProgress<double> progress) {
			this.ThrowIfNotDirectory();
			var path = this.ConcatFileSystemPath(name);
			return Directory.Exists(path.FullPath) || File.Exists(path.FullPath);
		}

		public override IEnumerable<ISystemEntry> GetChildren(CancellationToken token, IProgress<double> progress) {
			this.ThrowIfNotDirectory();
			return Seq.Make(
				Directory.EnumerateDirectories(this.FileSystemPath.FullPath)
					.Select(file => new Win32FileSystemEntry(this, IO::Path.GetFileName(file), file, false) as ISystemEntry),
				Directory.EnumerateFiles(this.FileSystemPath.FullPath)
					.Select(file => new Win32FileSystemEntry(this, IO::Path.GetFileName(file), file, false) as ISystemEntry))
				.WithCancellation(token)
				.Aggregate((a, b) => a.Concat(b));
		}

		public override ISystemEntry GetChild(string name, CancellationToken token, IProgress<double> progress) {
			this.ThrowIfNotDirectory();
			var path = this.ConcatFileSystemPath(name);
			return new Win32FileSystemEntry(this, name, path.FullPath, true);
		}

		#endregion

		#region FileInformation

		public override IFileInformation FileInformation {
			get {
				return new FileInformation(this.FileSystemPath.FullPath);
			}

		}

		public FileAttributes FileAttibutes{
			get{
				return ((FileInformation)this.FileInformation).Attributes;
			}
		}
		
		public FileSecurity AccessControl{
			get{
				return File.GetAccessControl(this.FileSystemPath.FullPath);
			}
		}

		public NTAccount Owner{
			get{
				return this.AccessControl.GetOwner(typeof(NTAccount)) as NTAccount;
			}
		}

		public IEnumerable<FileSystemAccessRule> AccessRules{
			get{
				var current = WindowsIdentity.GetCurrent();
				var fs = this.AccessControl;
				var rules = fs.GetAccessRules(true, true, typeof(SecurityIdentifier));
				foreach(FileSystemAccessRule rule in rules){
					if(rule.IdentityReference == current.User){
						yield return rule;
					}
					foreach(IdentityReference group in current.Groups){
						if(rule.IdentityReference == group){
							yield return rule;
							break;
						}
					}
				}
			}
		}

		#endregion
	}
}
