using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace CatWalk.Heron.View {
	using Win32 = CatWalk.Win32;
	public static class WindowUtility {
		public static IEnumerable<T> OrderWindowByZOrder<T>(this IEnumerable<T> windows) where T : Window {
			var byHandle = windows.ToDictionary(win => {
				var source = ((HwndSource)PresentationSource.FromVisual(win));
				if(source == null) {
					return IntPtr.Zero;
				} else {
					return source.Handle;
				}
			});
			return Win32::WindowUtility.OrderByZOrder(byHandle.Select(pair => pair.Key)).Select(hwnd => byHandle[hwnd]);
		}

		public static void SetForeground(this Window window) {
			Win32::User32.SetForegroundWindow(((HwndSource)PresentationSource.FromVisual(window)).Handle);
		}

		public static void SetTopZOrder(this Window window) {
			Win32::User32.SetWindowPos(
				((HwndSource)PresentationSource.FromVisual(window)).Handle,
				Win32::User32.HWND_TOP,
				0,
				0,
				0,
				0,
				Win32::SetWindowPosOptions.NoActivate |
				Win32::SetWindowPosOptions.NoMove |
				Win32::SetWindowPosOptions.NoSize);
		}

		public static Win32::ScreenInfo GetCurrentScreen(this Window win) {
			return Win32::Screen.GetCurrentMonitor(
				new CatWalk.Int32Rect(
					(int)(Double.IsNaN(win.RestoreBounds.Left) ? 0 : win.RestoreBounds.Left),
					(int)(Double.IsNaN(win.RestoreBounds.Top) ? 0 : win.RestoreBounds.Top),
					(int)(Double.IsNaN(win.RestoreBounds.Width) || win.RestoreBounds.Width < 0 ? 640 : win.RestoreBounds.Width),
					(int)(Double.IsNaN(win.RestoreBounds.Height) || win.RestoreBounds.Height < 0 ? 480 : win.RestoreBounds.Height)));
		}
	}
}