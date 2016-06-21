using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.IOSystem;
using CatWalk.Heron.ViewModel.IOSystem;
using CatWalk.IO;
using CatWalk.IOSystem;
using CatWalk.IOSystem.FileSystem;

namespace CatWalk.Heron.IOSystem.FileSystem {
	public class FileSystemProvider : SystemProvider {
		public FileSystemProvider() {
		}

		public override IEnumerable<ISystemEntry> GetRootEntries(ISystemEntry parent) {
			return Seq.Make(new FileSystemDriveDirectory(parent, "Drives"));
		}

		public override object GetViewModel(object parent, SystemEntryViewModel entry, object previous) {
			return null;
		}

		public override bool TryParsePath(ISystemEntry root, string path, out ISystemEntry entry) {
			var filePath = new FilePath(path);
			entry = null;
			if (!filePath.IsValid) {
				return false;
			}

			// カレントディレクトリをベースに絶対パス変換
			if (filePath.IsRelative) {
				filePath = new FilePath(Environment.CurrentDirectory).Resolve(filePath);
			}

			if (!filePath.IsValid) {
				return false;
			}

			if (filePath.Fragments.Count == 0) {
				entry = new FileSystemDriveDirectory(root, "Drives");
				return true;
			} else {
				var drives = new FileSystemDriveDirectory(root, "Drives");
				var drive = new FileSystemDrive(drives, filePath.Fragments[0].ToUpper(), filePath.Fragments[0].ToUpper()[0]);

				IFileSystemEntry parent = drive;
				IFileSystemEntry subEntry = drive;
				foreach(var subPath in filePath.FragmentPaths.Skip(1)) {
					subEntry = new FileSystemEntry(parent, subPath.FileName, subPath.FullPath);
					parent = subEntry;
				}
				entry = subEntry;

				return true;
			}

		}
	}
}
