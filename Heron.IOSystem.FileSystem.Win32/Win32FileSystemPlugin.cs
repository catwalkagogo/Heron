using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.Configuration;
using Reactive.Bindings.Extensions;
using Reactive.Bindings;
using CatWalk.Win32.Shell;
using CatWalk.IOSystem;
using CatWalk.IOSystem.FileSystem;
using CatWalk.IOSystem.FileSystem.Win32;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CatWalk.Heron.ViewModel.IOSystem;
using CatWalk.Heron.Windows;

namespace CatWalk.Heron.FileSystem.Win32 {
	using Drawing = System.Drawing;

	public class Win32FileSystemPlugin : Plugin{
		private Win32FileSystemProvider _Provider;
		private Win32FileSystemEntryOperator _Operator;
		private Dictionary<ImageListSize, ImageList> _ImageLists = new Dictionary<ImageListSize, ImageList>();

		public override int Priority {
			get {
				return PriorityNormal;
			}
		}

		protected override void OnLoaded(PluginEventArgs e) {
			this._Provider = new Win32FileSystemProvider();
			this._Operator = Win32FileSystemEntryOperator.Default;
			var app = e.Application;
			app.RegisterSystemProvider(this._Provider);
			app.RegisterEntryOperator(this._Operator);

			Factories.GetEntryImageFactory(app).Register(
				(entry, size) => entry.Entry is IWin32FileSystemEntry,
				(entry, size) => this.GetEntryIcon(entry.Entry, new Size<int>((int)size.Width, (int)size.Height), entry.CancellationToken));

			base.OnLoaded(e);
		}

		protected override void OnUnloaded(PluginEventArgs e) {
			var app = e.Application;
			app.UnregisterSystemProvider(this._Provider);
			app.UnregisterEntryOperator(this._Operator);

			this._ImageLists.Values.ForEach(list => list.Dispose());
			this._ImageLists.Clear();
		}

		public override string DisplayName {
			get {
				return "FileSystem IOSystem";
			}
		}

		public ImageSource GetEntryIcon(ISystemEntry entry, Size<int> size, CancellationToken token) {
			entry.ThrowIfNull("entry");
			var ife = entry as IFileSystemEntry;
			if (ife != null) {
				var bmp = new WriteableBitmap(size.Width, size.Height, 96, 96, PixelFormats.Pbgra32, null);
				Task.Run(new Action(delegate {
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
								if (!token.IsCancellationRequested) {
									bmp.WritePixels(
										new System.Windows.Int32Rect(((int)bmp.Width - bitmap.Width) / 2, ((int)bmp.Height - bitmap.Height) / 2, bitmap.Width, bitmap.Height),
										bitmapData.Scan0,
										bitmapData.Stride * bitmapData.Height,
										bitmapData.Stride);
								}
							} finally {
								bitmap.UnlockBits(bitmapData);
								bitmap.Dispose();
							}
						}));
					} catch {
						if (bitmapData != null) {
							bitmap.UnlockBits(bitmapData);
						}
						if (bitmap != null) {
							bitmap.Dispose();
						}
					}
				}), token);
				return bmp;
			} else {
				return null;
			}
		}

		private ImageList GetImageList(Size<int> size) {
			lock (this._ImageLists) {
				var ilsize = GetImageListSize(size);
				ImageList list;
				if (this._ImageLists.TryGetValue(ilsize, out list)) {
					return list;
				} else {
					list = new ImageList(ilsize);
					this._ImageLists.Add(ilsize, list);
					return list;
				}
			}
		}

		private static ImageListSize GetImageListSize(Size<int> size) {
			if (size.Width <= 16 && size.Height <= 16) {
				return ImageListSize.Small;
			} else if (size.Width <= 32 && size.Height <= 32) {
				return ImageListSize.Large;
			} else if (size.Width <= 48 && size.Height <= 48) {
				return ImageListSize.ExtraLarge;
			} else {
				return ImageListSize.Jumbo;
			}
		}
	}
}