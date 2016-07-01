/*
	$Id: FileSystemDrive.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using CatWalk.IO;

namespace CatWalk.IOSystem.FileSystem.Win32 {
	using IO = System.IO;
	public class FileSystemDrive : FileSystemEntryBase, IWin32FileSystemEntry{
		public char DriveLetter{get; private set;}

		public FileSystemDrive(ISystemEntry parent, char driveLetter) : base(parent, ValidateDriveLetter(driveLetter).ToString()){
			this.DriveLetter = ValidateDriveLetter(driveLetter);
			this.FileSystemPath = new FilePath(this.DriveLetter + ":" + IO::Path.DirectorySeparatorChar, FilePathFormats.Windows);
		}

		/// <summary>
		/// ドライブ文字を検証し、正規化する
		/// </summary>
		/// <param name="driveLetter"></param>
		/// <returns></returns>
		private static char ValidateDriveLetter(char driveLetter){
			driveLetter = driveLetter.ToString().ToUpper()[0];
			if(driveLetter < 'A' || 'Z' < driveLetter){
				throw new ArgumentException("driveLetter");
			}
			return driveLetter;
		}

		public override StringComparison StringComparison {
			get {
				return StringComparison.OrdinalIgnoreCase;
			}
		}

		#region Properties

		public override bool IsDirectory {
			get {
				return true;
			}
		}

		public DriveInfo DriveInfo {
			get {
				return new DriveInfo(this.FileSystemPath.FullPath);
			}
		}

		public override bool IsExists(CancellationToken token, IProgress<double> progress) {
			var info = this.DriveInfo;
			return info.IsReady;
		}

		public bool IsReady{
			get{
				return this.IsExists();
			}
		}

		public DriveType DriveType{
			get{
				var info = this.DriveInfo;
				return (info.IsReady) ? info.DriveType : System.IO.DriveType.Unknown;
			}
		}

		public string DriveFormat{
			get{
				var info = this.DriveInfo;
				return (info.IsReady) ? info.DriveFormat : "";
			}
		}

		public long AvailableFreeSpace{
			get{
				var info = this.DriveInfo;
				return (info.IsReady) ? info.AvailableFreeSpace : 0;
			}
		}

		public long TotalSize{
			get{
				var info = this.DriveInfo;
				return (info.IsReady) ? info.TotalSize : 0;
			}
		}

		public long TotalFreeSpace{
			get{
				var info = this.DriveInfo;
				return (info.IsReady) ? info.TotalFreeSpace : 0;
			}
		}

		#endregion

		#region IFileSystemDirectory Members

		public string ConcatFileSystemPath(string name) {
			return this.FileSystemPath.Resolve(name).FullPath;
		}

		public FilePath FileSystemPath {get; private set;}

		#endregion

		#region ISystemDirectory Members

		public override IEnumerable<ISystemEntry> GetChildren(CancellationToken token, IProgress<double> progress) {
			return
				Seq.Make(
					Directory.EnumerateDirectories(this.FileSystemPath.FullPath)
						.Select(file => new Win32FileSystemEntry(this, IO::Path.GetFileName(file), file, true) as ISystemEntry),
					Directory.EnumerateFiles(this.FileSystemPath.FullPath)
						.Select(file => new Win32FileSystemEntry(this, IO::Path.GetFileName(file), file, false) as ISystemEntry))
				.WithCancellation(token)
				.Aggregate((a,b) => a.Concat(b));
		}

		public override ISystemEntry GetChild(string name, CancellationToken token, IProgress<double> progress) {
			var path = this.ConcatFileSystemPath(name);
			return new Win32FileSystemEntry(this, name, path, true);
		}

		public override bool Contains(string name, CancellationToken token, IProgress<double> progress) {
			var path = this.ConcatFileSystemPath(name);
			return Directory.Exists(path) || File.Exists(path);
		}

		#endregion

		#region Equals

		public bool Equals(IFileSystemEntry entry){
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
