using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CatWalk;
using CatWalk.IOSystem;
using CatWalk.IOSystem.FileSystem;
using CatWalk.IOSystem.FileSystem.Win32;
using CatWalk.Win32.Shell;
using CatWalk.IO;
using CatWalk.Heron.IOSystem;
using CatWalk.Heron.ViewModel.IOSystem;
using CatWalk.Collections;

namespace CatWalk.Heron.FileSystem.Win32 {
	
	public class Win32FileSystemProvider : ISystemProvider{
		private static readonly ColumnDefinition _ExtensionColumn = new ExtensionColumn();
		private static readonly ColumnDefinition _BaseNameColumn = new BaseNameColumn();

		public string DisplayName {
			get {
				return this.Name;
			}
		}

		public string Name {
			get {
				return "Win32FileSystemProvider";
			}
		}


		#region Column

		public bool CanGetColumnDefinitions(ISystemEntry entry) {
			return entry is Win32FileSystemDrive || entry is Win32FileSystemEntry || entry is Win32FileSystemDriveDirectory;
		}

		public IEnumerable<IColumnDefinition> GetColumnDefinitions(ISystemEntry entry) {
			entry.ThrowIfNull("entry");
			IEnumerable<ColumnDefinition> columns = new ColumnDefinition[]{
			};
			var fsentry = entry as Win32FileSystemEntry;
			if (fsentry != null) {
				var source = new FileInfoSource(fsentry);
				columns = columns.Concat(new ColumnDefinition[]{
					new CreationTimeColumn(source),
					new LastWriteTimeColumn(source),
					new LastAccessTimeColumn(source),
					new AttributesColumn(source),
					_ExtensionColumn,
					_BaseNameColumn,
					new OwnerColumn(new ResetLazyColumnValueSource<NTAccount>(() => fsentry.Owner)),
					new AccessControlColumn(new ResetLazyColumnValueSource<FileSecurity>(() => fsentry.AccessControl)),
				});
				if (entry.IsDirectory) {
				} else {
					columns = columns.Concat(new ColumnDefinition[]{
						new FileSizeColumn(source),
					});
				}
			}

			var drive = entry as Win32FileSystemDrive;
			if (drive != null) {
				var source = new DriveInfoSource(drive);
				columns = columns.Concat(new ColumnDefinition[]{
					new VolumeLabelColumn(source),
					new AvailableFreeSpaceColumn(source),
					new TotalFreeSpaceColumn(source),
					new UsedSpaceForAvailableFreeSpaceColumn(source),
					new UsedSpaceForTotalFreeSpaceColumn(source),
					new DriveFormatColumn(source),
				});
			}

			return columns;
		}

		#endregion

		public bool CanGetViewModel(object parent, SystemEntryViewModel entry, object previous) {
			return false;
		}

		#region Grouping

		public bool CanGetGroupings(ISystemEntry entry) {
			return entry is Win32FileSystemDrive || entry is Win32FileSystemEntry || entry is Win32FileSystemDriveDirectory;
		}

		private static readonly IGroupDefinition _FileSizeGroup = new FileSizeEntryGroupDefinition();
		private static readonly IGroupDefinition _CreationTimeGroup = new MonthlyGroupDefinition<CreationTimeColumn>();
		private static readonly IGroupDefinition _LastWriteTimeGroup = new MonthlyGroupDefinition<LastWriteTimeColumn>();
		private static readonly IGroupDefinition _LastAccessTimeGroup = new MonthlyGroupDefinition<LastAccessTimeColumn>();

		public IEnumerable<IGroupDefinition> GetGroupings(ISystemEntry entry) {
			if(entry is IFileSystemEntry) {
				return new IGroupDefinition[]{
					_FileSizeGroup,
					_CreationTimeGroup,
					_LastWriteTimeGroup,
					_LastAccessTimeGroup,
				};
			} else {
				return new IGroupDefinition[0];
			}
		}

		#endregion

		public bool CanGetOrderDefinitions(SystemEntryViewModel entry) {
			return false;
		}

		public IEnumerable<OrderDefinition> GetOrderDefinitions(SystemEntryViewModel entry) {
			throw new NotImplementedException();
		}


		public object GetViewModel(object parent, SystemEntryViewModel entry, object previous) {
			throw new NotImplementedException();
		}


		#region ParsePath

		public ParsePathResult ParsePath(ISystemEntry root, string path) {
			root.ThrowIfNull("root");
			path.ThrowIfNull("path");
			var filePath = new FilePath(path, FilePathKind.Absolute, FilePathFormats.Windows);
			if(filePath.IsValid && filePath.PathKind == FilePathKind.Absolute) {
				var drives = new Win32FileSystemDriveDirectory(root, "Drives");
				var drive = drives.GetChild(filePath.Fragments[0]);
				ISystemEntry entry;
				entry = drive;
				foreach(var name in filePath.Fragments.Skip(1)) {
					entry = entry.GetChild(name);
				}
				return new ParsePathResult(true, entry, FilePathFormats.Windows.EndsWithDirectorySeparator(path));
			} else {
				return new ParsePathResult(false, null, false);
			}
		}

		#endregion

		#region GetRootEntries

		public IEnumerable<ISystemEntry> GetRootEntries(ISystemEntry parent) {
			parent.ThrowIfNull("parent");
			return Seq.Make(new Win32FileSystemDriveDirectory(parent, "Drives"));
		}

		#endregion

		#region Entry Columns
		public class OwnerColumn : CacheColumnDefinition<NTAccount, NTAccount> {
			public OwnerColumn(IColumnValueSource<NTAccount> source) : base(source) {
			}

			protected override NTAccount SelectValue(NTAccount value) {
				return value;
			}
		}

		public class AccessControlColumn : CacheColumnDefinition<FileSecurity, FileSecurity> {
			public AccessControlColumn(IColumnValueSource<FileSecurity> source) : base(source) {
			}

			protected override FileSecurity SelectValue(FileSecurity value) {
				return value;
			}
		}

		public abstract class FileInfoColumn<T> : CacheColumnDefinition<IFileInformation, T> {
			public FileInfoColumn(FileInfoSource source) : base(source) {
			}
		}

		public class FileInfoSource : ResetLazyColumnValueSource<IFileInformation> {
			public FileInfoSource(Win32FileSystemEntry entry)
				: base(() => entry.FileInformation) {
			}
		}

		public class CreationTimeColumn : FileInfoColumn<DateTime> {
			public CreationTimeColumn(FileInfoSource source) : base(source) {
			}

			protected override DateTime SelectValue(IFileInformation value) {
				return value.CreationTime;
			}
		}

		public class LastWriteTimeColumn : FileInfoColumn<DateTime> {
			public LastWriteTimeColumn(FileInfoSource source) : base(source) {
			}

			protected override DateTime SelectValue(IFileInformation value) {
				return value.LastWriteTime;
			}
		}

		public class LastAccessTimeColumn : FileInfoColumn<DateTime> {
			public LastAccessTimeColumn(FileInfoSource source) : base(source) {
			}

			protected override DateTime SelectValue(IFileInformation value) {
				return value.LastAccessTime;
			}
		}

		public class AttributesColumn : FileInfoColumn<FileAttributes> {
			public AttributesColumn(FileInfoSource source) : base(source) {
			}

			protected override FileAttributes SelectValue(IFileInformation value) {
				return ((Win32FileInformation)value).Attributes;
			}
		}

		public class FileSizeColumn : FileInfoColumn<long> {
			public FileSizeColumn(FileInfoSource source) : base(source) {
			}

			protected override long SelectValue(IFileInformation value) {
				return value.Length;
			}
		}

		#endregion

		#region File Columns

		public class ExtensionColumn : ColumnDefinition<string> {
			protected override object GetValueImpl(ISystemEntry entry, bool noCache, CancellationToken token) {
				return ((Win32FileSystemEntry)entry).Extension;
			}
		}

		public class BaseNameColumn : ColumnDefinition<string> {
			protected override object GetValueImpl(ISystemEntry entry, bool noCache, CancellationToken token) {
				return ((Win32FileSystemEntry)entry).BaseName;
			}
		}

		#endregion

		#region Drive Columns

		public class DriveInfoSource : ResetLazyColumnValueSource<DriveInfo> {
			public DriveInfoSource(Win32FileSystemDrive drive)
				: base(() => drive.DriveInfo) {

			}
		}

		public class AvailableFreeSpaceColumn : CacheColumnDefinition<DriveInfo, long> {
			public AvailableFreeSpaceColumn(DriveInfoSource source) : base(source) {
			}

			protected override long SelectValue(DriveInfo value) {
				return value.AvailableFreeSpace;
			}
		}

		public class TotalFreeSpaceColumn : CacheColumnDefinition<DriveInfo, long> {
			public TotalFreeSpaceColumn(DriveInfoSource source) : base(source) {
			}

			protected override long SelectValue(DriveInfo value) {
				return value.TotalFreeSpace;
			}
		}

		public class TotalSizeColumn : CacheColumnDefinition<DriveInfo, long> {
			public TotalSizeColumn(DriveInfoSource source) : base(source) {
			}

			protected override long SelectValue(DriveInfo value) {
				return value.TotalSize;
			}
		}

		public class UsedSpaceForAvailableFreeSpaceColumn : CacheColumnDefinition<DriveInfo, long> {
			public UsedSpaceForAvailableFreeSpaceColumn(DriveInfoSource source) : base(source) {
			}

			protected override long SelectValue(DriveInfo value) {
				return value.TotalSize - value.AvailableFreeSpace;
			}
		}

		public class UsedSpaceForTotalFreeSpaceColumn : CacheColumnDefinition<DriveInfo, long> {
			public UsedSpaceForTotalFreeSpaceColumn(DriveInfoSource source) : base(source) {
			}

			protected override long SelectValue(DriveInfo value) {
				return value.TotalSize - value.TotalFreeSpace;
			}
		}

		public class VolumeLabelColumn : CacheColumnDefinition<DriveInfo, string> {
			public VolumeLabelColumn(DriveInfoSource source) : base(source) {
			}

			protected override string SelectValue(DriveInfo value) {
				return value.VolumeLabel;
			}
		}

		public class DriveFormatColumn : CacheColumnDefinition<DriveInfo, string> {
			public DriveFormatColumn(DriveInfoSource source) : base(source) {
			}

			protected override string SelectValue(DriveInfo value) {
				return value.DriveFormat;
			}
		}

		public class DriveTypeColumn : CacheColumnDefinition<DriveInfo, DriveType> {
			public DriveTypeColumn(DriveInfoSource source)
				: base(source) {
			}

			protected override DriveType SelectValue(DriveInfo value) {
				return value.DriveType;
			}
		}
		#endregion

		#region FileSizeGroup
		
		private class FileSizeEntryGroupDefinition : IGroupDefinition {
			private static readonly DelegateEntryGroup<int>[] _Candidates;
			private static readonly Type COLUMN = typeof(FileSizeColumn);

			static FileSizeEntryGroupDefinition() {
				const long K = 1024;
				const long M = K * K;
				const long G = M * K;
				_Candidates = new DelegateEntryGroup<int>[]{
					new DelegateEntryGroup<int>(0, "0 bytes", entry => (long)entry.Columns[COLUMN].Value == 0),
					new DelegateEntryGroup<int>(1, "1 - 100KB", entry => {
						var v = (long)entry.Columns[COLUMN].Value;
						return 1 <= v && v <= K * 100;
					}),
					new DelegateEntryGroup<int>(2, "100KB - 1MB", entry => {
						var v = (long)entry.Columns[COLUMN].Value;
						return K * 100 < v && v <= M;
					}),
					new DelegateEntryGroup<int>(3, "1MB - 100MB", entry => {
						var v = (long)entry.Columns[COLUMN].Value;
						return M < v && v <= M * 100;
					}),
					new DelegateEntryGroup<int>(4, "100MB - 1GB", entry => {
						var v = (long)entry.Columns[COLUMN].Value;
						return 100 * M < v && v <= G;
					}),
					new DelegateEntryGroup<int>(5, "1GB -", entry => {
						var v = (long)entry.Columns[COLUMN].Value;
						return G < v;
					}),
				};
			}

			public IGroup GetGroupName(SystemEntryViewModel entry) {
				return _Candidates.FirstOrDefault(grp => grp.Filter(entry));
			}
		}
		
		#endregion

	}
}
