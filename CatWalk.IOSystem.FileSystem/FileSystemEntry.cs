/*
	$Id: FileSystemEntry.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Principal;
using System.Threading;
using CatWalk;
using CatWalk.IO;

namespace CatWalk.IOSystem.FileSystem {
	using IO = System.IO;
	public abstract class FileSystemEntry : FileSystemEntryBase, IFileSystemEntry{

		public FileSystemEntry(ISystemEntry parent, string name, string path) : base(parent, name){
			this.FileSystemPath = new FilePath(path, FilePathKind.Absolute, FilePathFormats.Windows);
		}


		public abstract IFilePathFormat FilePathFormat { get; }


		#region Directory

		public FilePath ConcatFileSystemPath(string name) {
			this.ThrowIfNotDirectory();
			return this.FileSystemPath.Resolve(name);
		}

		#endregion

		#region Properties

		public FilePath FileSystemPath { get; private set; }

		#endregion;

		#region FileInformation

		public abstract IFileInformation FileInformation { get; }

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
