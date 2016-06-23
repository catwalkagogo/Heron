using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using CatWalk.Windows;

namespace CatWalk.Heron.Windows {
	using Win32 = CatWalk.Win32;
	using Win = System.Windows;
	public static class WindowUtility {
		public static IEnumerable<MainWindow> MainWindows {
			get {
				return Win::Application.Current.Windows.OfType<MainWindow>();
			}
		}

		private static MainWindow _LatestMainWindow;
		public static MainWindow LatestActiveWindow {
			get {
				return _LatestMainWindow ?? MainWindows.FirstOrDefault(win => win.IsActive) ?? MainWindows.FirstOrDefault();
			}
			internal set {
				_LatestMainWindow = value;
			}
		}

		public static void ArrangeMainWindows(ArrangeMode mode) {
			Arranger arranger = null;
			switch (mode) {
				case ArrangeMode.Cascade:
					arranger = new CascadeArranger();
					break;
				case ArrangeMode.TileHorizontal:
					arranger = new TileHorizontalArranger();
					break;
				case ArrangeMode.TileVertical:
					arranger = new TileVerticalArranger();
					break;
				case ArrangeMode.StackHorizontal:
					arranger = new StackHorizontalArranger();
					break;
				case ArrangeMode.StackVertical:
					arranger = new StackVerticalArranger();
					break;
			}

			foreach (var screen in Win32::Screen.GetMonitors()) {
				var windows = MainWindows
					.Where(w => w.WindowState != WindowState.Minimized)
					.OrderWindowByZOrder()
					.Where(w => {
						var rect = new Int32Rect((int)w.Left, (int)w.Top, (int)w.Width, (int)w.Height);
						var inter = rect.Intersect(screen.ScreenArea);
						var area = inter.Area;
						return area >= rect.Area - area;
					}).ToArray();

				var rects = arranger.Arrange(new Size(screen.WorkingArea.Width, screen.WorkingArea.Height), windows.Length);

				for (var i = 0; i < rects.Length; i++) {
					var win = windows[i];
					var rect = rects[i];
					win.WindowState = WindowState.Normal;
					win.Left = rect.Left;
					win.Top = rect.Top;
					win.Width = rect.Width;
					win.Height = rect.Height;
				}
			}
		}

		public static IEnumerable<T> OrderWindowByZOrder<T>(this IEnumerable<T> windows) where T : Window {
			var byHandle = windows.ToDictionary(win => {
				var source = ((HwndSource)PresentationSource.FromVisual(win));
				if (source == null) {
					return IntPtr.Zero;
				} else {
					return source.Handle;
				}
			});
			return Win32::WindowUtils.OrderByZOrder(byHandle.Select(pair => pair.Key)).Select(hwnd => byHandle[hwnd]);
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

		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static WindowState GetRestoreWindowState(DependencyObject obj) {
			return (WindowState)obj.GetValue(RestoreWindowStateProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static void SetRestoreWindowState(DependencyObject obj, WindowState value) {
			obj.SetValue(RestoreWindowStateProperty, value);
		}

		// Using a DependencyProperty as the backing store for RestoreWindowState.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty RestoreWindowStateProperty =
			DependencyProperty.RegisterAttached(
				"RestoreWindowState",
				typeof(WindowState),
				typeof(WindowUtility),
				new FrameworkPropertyMetadata(
					WindowState.Normal,
					(s, e) => {
						var window = (Window)s;
						window.StateChanged -= Window_StateChanged;
						window.StateChanged += Window_StateChanged;
						Window_StateChanged(window, EventArgs.Empty);
					}
				) {
					BindsTwoWayByDefault = true,
				}
			);

		private static void Window_StateChanged(object sender, EventArgs e) {
			var window = (Window)sender;
			if (window.WindowState != WindowState.Minimized) {
				SetRestoreWindowState(window, window.WindowState);
			}
		}
	}
}