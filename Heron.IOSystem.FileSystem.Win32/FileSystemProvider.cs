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
	using Drawing = System.Drawing;
	public class FileSystemProvider : SystemProvider, IDisposable{
		private static readonly ColumnDefinition _ExtensionColumn = new ExtensionColumn();
		private static readonly ColumnDefinition _BaseNameColumn = new BaseNameColumn();
		private Dictionary<ImageListSize, ImageList> _ImageLists = new Dictionary<ImageListSize,ImageList>();

		#region GetViewModel

		public override object GetViewModel(object parent, SystemEntryViewModel entry, object previous) {
			if(entry.Entry is IFileSystemEntry) {
				var vm = previous as FileSystemViewModel;
				if(vm == null) {
					vm = new FileSystemViewModel();
				}
				return vm;
			} else {
				return null;
			}
		}

		#endregion

		#region GetAdditionalColumnProviders

		protected override IEnumerable<ColumnDefinition> GetAdditionalColumnProviders(ISystemEntry entry) {
			entry.ThrowIfNull("entry");
			IEnumerable<ColumnDefinition> columns = new ColumnDefinition[]{
			};
			var fsentry = entry as Win32FileSystemEntry;
			if(fsentry != null) {
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
				if(entry.IsDirectory) {
				} else {
					columns = columns.Concat(new ColumnDefinition[]{
						new FileSizeColumn(source),
					});
				}
			}

			var drive = entry as FileSystemDrive;
			if(drive != null) {
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

		#region ParsePath

		public override ParsePathResult ParsePath(ISystemEntry root, string path) {
			root.ThrowIfNull("root");
			path.ThrowIfNull("path");
			var filePath = new FilePath(path, FilePathKind.Absolute, FilePathFormats.Windows);
			if(filePath.IsValid && filePath.PathKind == FilePathKind.Absolute) {
				var drives = new FileSystemDriveDirectory(root, "Drives");
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

		public override IEnumerable<ISystemEntry> GetRootEntries(ISystemEntry parent) {
			parent.ThrowIfNull("parent");
			return Seq.Make(new FileSystemDriveDirectory(parent, "Drives"));
		}

		#endregion

		#region GetEntryIcon

		public override object GetEntryIcon(ISystemEntry entry, Size<int> size, CancellationToken token) {
			entry.ThrowIfNull("entry");
			var ife = entry as IFileSystemEntry;
			if(ife != null) {
				var bmp = new WriteableBitmap(size.Width, size.Height, 96, 96, PixelFormats.Pbgra32, null);
				Task.Factory.StartNew(new Action(delegate {
					var list = this.GetImageList(size);
					int overlay;
					var index = list.GetIconIndexWithOverlay(ife.FileSystemPath.FullPath, out overlay);
					Drawing::Bitmap bitmap = null;
					Drawing::Imaging.BitmapData bitmapData = null;
					try {
						bitmap = list.Draw(index, overlay, ImageListDrawOptions.PreserveAlpha);
						bitmapData = bitmap.LockBits(new Drawing::Rectangle(0, 0, bitmap.Width, bitmap.Height), Drawing::Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);
						bmp.Dispatcher.BeginInvoke(new Action(delegate {
							try {
								bmp.WritePixels(
									new System.Windows.Int32Rect(((int)bmp.Width - bitmap.Width) / 2, ((int)bmp.Height - bitmap.Height) / 2, bitmap.Width, bitmap.Height),
									bitmapData.Scan0,
									bitmapData.Stride * bitmapData.Height,
									bitmapData.Stride);
							} finally {
								bitmap.UnlockBits(bitmapData);
								bitmap.Dispose();
							}
						}));
					} catch {
						if(bitmapData != null) {
							bitmap.UnlockBits(bitmapData);
						}
						if(bitmap != null) {
							bitmap.Dispose();
						}
					}
				}), token);
				return bmp;
			} else {
				return base.GetEntryIcon(entry, size, token);
			}
		}

		private ImageList GetImageList(Size<int> size) {
			lock(this._ImageLists) {
				var ilsize = GetImageListSize(size);
				ImageList list;
				if(this._ImageLists.TryGetValue(ilsize, out list)) {
					return list;
				} else {
					list = new ImageList(ilsize);
					this._ImageLists.Add(ilsize, list);
					return list;
				}
			}
		}

		private static ImageListSize GetImageListSize(Size<int> size) {
			if(size.Width <= 16 && size.Height <= 16) {
				return ImageListSize.Small;
			} else if(size.Width <= 32 && size.Height <= 32) {
				return ImageListSize.Large;
			} else if(size.Width <= 48 && size.Height <= 48) {
				return ImageListSize.ExtraLarge;
			} else {
				return ImageListSize.Jumbo;
			}
		}


		#endregion

		#region IDisposable

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private bool _Disposed = false;

		protected virtual void Dispose(bool disposing) {
			if(!this._Disposed) {
				foreach(var list in this._ImageLists.Values) {
					list.Dispose();
				}
				this._Disposed = true;
			}
		}

		~FileSystemProvider() {
			this.Dispose(false);
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
				return ((FileInformation)value).Attributes;
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
			public DriveInfoSource(FileSystemDrive drive)
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

		#region Grouping
		/*
		private static readonly EntryGroupDescription _FileSizeGroup = new FileSizeEntryGroupDescription();
		private static readonly EntryGroupDescription _CreationTimeGroup = new MonthlyGroupDescription<CreationTimeColumn>();
		private static readonly EntryGroupDescription _LastWriteTimeGroup = new MonthlyGroupDescription<LastWriteTimeColumn>();
		private static readonly EntryGroupDescription _LastAccessTimeGroup = new MonthlyGroupDescription<LastAccessTimeColumn>();

		protected override IEnumerable<EntryGroupDescription> GetAdditionalGroupings(ISystemEntry entry) {
			if(entry is IFileSystemEntry) {
				return new EntryGroupDescription[]{
					_FileSizeGroup,
					_CreationTimeGroup,
					_LastWriteTimeGroup,
					_LastAccessTimeGroup,
				};
			} else {
				return base.GetAdditionalGroupings(entry);
			}
		}
		*/
		#endregion

		#region FileSizeGroup
		/*
		private class FileSizeEntryGroupDescription : EntryGroupDescription {
			private static readonly DelegateEntryGroup<int>[] _Candidates;
			private static readonly string COLUMN = typeof(FileSizeColumn).FullName;

			static FileSizeEntryGroupDescription() {
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

			protected override IEntryGroup GroupNameFromItem(SystemEntryViewModel entry, int level, System.Globalization.CultureInfo culture) {
				return _Candidates.FirstOrDefault(grp => grp.Filter(entry));
			}
		}
		*/
		#endregion

	}
}
