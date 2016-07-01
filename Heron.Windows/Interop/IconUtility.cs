using CatWalk.Win32;
using CatWalk.Win32.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CatWalk.Heron.Windows.Interop {
	public static class IconUtility {
		public const int FileIconIndex = 0;
		public const int FolderIconIndex = 3;

		public static ImageSource GetShellIcon(int index, IconSize size) {
			IntPtr largeIcon = IntPtr.Zero;
			IntPtr smallIcon = IntPtr.Zero;
			try {
				Shell32.ExtractIconEx("Shell32.dll", index, out largeIcon, out smallIcon, 1);
				var imageSource = Imaging.CreateBitmapSourceFromHIcon(
					size == IconSize.Large ? largeIcon : smallIcon,
					Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions());

				return imageSource;
			} finally {
				User32.DestroyIcon(largeIcon);
				User32.DestroyIcon(smallIcon);
			}

		}
	}

}
