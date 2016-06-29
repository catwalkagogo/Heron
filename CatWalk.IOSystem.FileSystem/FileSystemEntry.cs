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
using CatWalk;
using CatWalk.IO;

namespace CatWalk.IOSystem.FileSystem {
	using IO = System.IO;
	public class FileSystemEntry : FileSystemEntryBase, IFileSystemEntry{
		private Lazy<bool> _IsDirectory;

		public FileSystemEntry(ISystemEntry parent, string name, string path) : base(parent, name){
			this.Initialize(parent, name, path, () => IO::Directory.Exists(this.FileSystemPath.FullPath));
		}

		internal FileSystemEntry(ISystemEntry parent, string name, string path, bool isDirectory) : base(parent, name){
			this.Initialize(parent, name, path, () => isDirectory);
		}

		private void Initialize(ISystemEntry parent, string name, string path, Func<bool> isDirectory){
			this.FileSystemPath = new FilePath(path, FilePathKind.Absolute, FilePathFormats.Windows);
			this._DisplayName = new Lazy<string>(() => this.FileSystemPath.FileName);
			this._IsDirectory = new Lazy<bool>(isDirectory);
		}

		public override bool IsDirectory {
			get {
				return this._IsDirectory.Value;
			}
		}

		public override bool IsExists() {
			if(this.IsDirectory) {
				return Directory.Exists(this.FileSystemPath.FullPath);
			} else {
				return File.Exists(this.FileSystemPath.FullPath);
			}
		}

		public override bool IsExists(CancellationToken token) {
			return this.IsExists();
		}

		public override bool IsExists(CancellationToken token, IProgress<double> progress) {
			return base.IsExists();
		}


		#region Directory

		public override bool Contains(string name, CancellationToken token, IProgress<double> progress) {
			this.ThrowIfNotDirectory();
			var path = this.ConcatFileSystemPath(name);
			return Directory.Exists(path.FullPath) || File.Exists(path.FullPath);
		}

		public FilePath ConcatFileSystemPath(string name) {
			this.ThrowIfNotDirectory();
			return this.FileSystemPath.Resolve(name);
		}

		public override IEnumerable<ISystemEntry> GetChildren(CancellationToken token, IProgress<double> progress) {
			this.ThrowIfNotDirectory();
			return Seq.Make(
				Directory.EnumerateDirectories(this.FileSystemPath.FullPath)
					.Select(file => new FileSystemEntry(this, IO::Path.GetFileName(file), file, false) as ISystemEntry),
				Directory.EnumerateFiles(this.FileSystemPath.FullPath)
					.Select(file => new FileSystemEntry(this, IO::Path.GetFileName(file), file, false) as ISystemEntry))
				.WithCancellation(token)
				.Aggregate((a, b) => a.Concat(b));
		}

		public override ISystemEntry GetChild(string name, CancellationToken token, IProgress<double> progress) {
			this.ThrowIfNotDirectory();
			var path = this.ConcatFileSystemPath(name);
			return new FileSystemEntry(this, name, path.FullPath, true);
		}

		#endregion

		#region Properties

		private Lazy<string> _DisplayName;
		public override string DisplayName {
			get {
				return this._DisplayName.Value;
			}
		}

		public FilePath FileSystemPath { get; private set; }

		#endregion;

		#region FileInformation

		public IFileInformation FileInformation {
			get {
				return new FileInformation(this.FileSystemPath.FullPath);
			}

		}

		public long Size {
			get {
				var info = this.FileInformation;
				return info.Length;
			}
		}

		public int LinkCount {
			get {
				var info = this.FileInformation;
				return info.LinkCount;
			}
		}

		public FileAttributes FileAttibutes{
			get{
				return this.FileInformation.Attributes;
			}
		}

		public DateTime CreationTime{
			get{
				return this.FileInformation.CreationTime;
			}
		}

		public DateTime LastWriteTime{
			get{
				return this.FileInformation.LastWriteTime;
			}
		}

		public DateTime LastAccessTime{
			get{
				return this.FileInformation.LastAccessTime;
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
		
		public string FileName{
			get{
				return this.FileSystemPath.FileName;
			}
		}

		public string FileNameWithoutExtension{
			get{
				return this.FileSystemPath.FileNameWithoutExtension;
			}
		}

		public string BaseName{
			get{
				return this.FileName.Split('.').First();
			}
		}

		public string DirectoryName{
			get{
				return this.FileSystemPath.Resolve("..").FullPath;
			}
		}

		public string Extension{
			get{
				return this.FileSystemPath.Extension;
			}
		}

		public IEnumerable<string> Extensions{
			get{
				return this.FileName.Split('.').Skip(1);
			}
		}

		#endregion

		#region Equals

		public bool Equals(IFileSystemEntry entry) {
			return this.FileSystemPath.Equals(entry.FileSystemPath);
		}

		public override bool Equals(object obj) {
			var entry = obj as IFileSystemEntry;
			if(entry != null) {
				return this.Equals(entry);
			} else {
				return base.Equals(obj);
			}
		}

		public override int GetHashCode() {
			return this.FileSystemPath.GetHashCode();
		}

		#endregion
	}
}
