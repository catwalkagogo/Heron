using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using CatWalk.Windows;

namespace CatWalk.Heron.Windows {
	using Win32 = CatWalk.Win32;
	using Win = System.Windows;
	using VM = ViewModel;
	public static class WindowUtility {
		public static IEnumerable<MainWindow> MainWindows {
			get {
				return Win::Application.Current.Windows.OfType<MainWindow>();
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
						var rect = new Rect<int>((int)w.Left, (int)w.Top, (int)w.Width, (int)w.Height);
						var inter = rect.Intersect(screen.ScreenArea);
						var area = inter.Area;
						return area >= rect.Area - area;
					}).ToArray();

				var rects = arranger.Arrange(new Win.Size(screen.WorkingArea.Width, screen.WorkingArea.Height), windows.Length);

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
				new Rect<int>(
					(int)(Double.IsNaN(win.RestoreBounds.Left) ? 0 : win.RestoreBounds.Left),
					(int)(Double.IsNaN(win.RestoreBounds.Top) ? 0 : win.RestoreBounds.Top),
					(int)(Double.IsNaN(win.RestoreBounds.Width) || win.RestoreBounds.Width < 0 ? 640 : win.RestoreBounds.Width),
					(int)(Double.IsNaN(win.RestoreBounds.Height) || win.RestoreBounds.Height < 0 ? 480 : win.RestoreBounds.Height)));
		}

		#region RestoreWindowState

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
						Window_StateChanged(window, EventArgs.Empty);
					},
					(s, value) => {
						var window = (Window)s;
						window.StateChanged -= Window_StateChanged;
						window.StateChanged += Window_StateChanged;

						return value;
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

		#endregion

		#region RestoreBounds

		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static Rect GetRestoreBounds(DependencyObject obj) {
			return (Rect)obj.GetValue(RestoreBoundsProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static void SetRestoreBounds(DependencyObject obj, Rect value) {
			obj.SetValue(RestoreBoundsProperty, value);
		}

		// Using a DependencyProperty as the backing store for RestoreBounds.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty RestoreBoundsProperty =
			DependencyProperty.RegisterAttached(
				"RestoreBounds",
				typeof(Rect),
				typeof(WindowUtility),
				new FrameworkPropertyMetadata(
					Rect.Empty,
					(s, e) => {
						var window = (Window)s;

						window.SizeChanged -= Window_SizeChanged;
						window.LocationChanged -= Window_LocationChanged;

						var bounds = (Rect)e.NewValue;
						var state = window.WindowState;
						window.WindowState = WindowState.Normal;
						window.Top = bounds.Top;
						window.Left = bounds.Left;
						window.Width = bounds.Width;
						window.Height = bounds.Height;
						window.WindowState = state;

						window.SizeChanged += Window_SizeChanged;
						window.LocationChanged += Window_LocationChanged;
					},
					(s, v) => {
						var window = (Window)s;

						window.SizeChanged -= Window_SizeChanged;
						window.LocationChanged -= Window_LocationChanged;
						window.SizeChanged += Window_SizeChanged;
						window.LocationChanged += Window_LocationChanged;

						return v;
					}) {
						BindsTwoWayByDefault = true,
					});

		private static void Window_LocationChanged(object sender, EventArgs e) {
			var window = (Window)sender;
			SetRestoreBounds(window, window.RestoreBounds);
		}

		private static void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
			var window = (Window)sender;
			SetRestoreBounds(window, window.RestoreBounds);
		}

		#endregion

		#region DialogResult

		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static bool? GetDialogResult(DependencyObject obj) {
			return (bool?)obj.GetValue(DialogResultProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static void SetDialogResult(DependencyObject obj, bool? value) {
			obj.SetValue(DialogResultProperty, value);
		}

		// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DialogResultProperty =
			DependencyProperty.RegisterAttached("DialogResult", typeof(bool?), typeof(WindowUtility), new FrameworkPropertyMetadata(null,
				(s, e) => {
					var window = (Window)s;
					if (window.DialogResult != (bool?)e.NewValue) {
						var sub = GetDialogResultSubscriber(window);
						if (sub != null) {
							sub.Dispose();
						}

						window.DialogResult = (bool?)e.NewValue;

						var d = SubscriveDialogResultChange(window, (bool?)e.NewValue);
						SetDialogResultSubscriber(window, d);
					}
				},
				(s, v) => {
					var window = (Window)s;
					var sub = GetDialogResultSubscriber(window);
					if (sub != null) {
						sub.Dispose();
					}

					var d = SubscriveDialogResultChange(window, (bool?)v);
					SetDialogResultSubscriber(window, d);

					return v;
				}) {
					BindsTwoWayByDefault = true
				});

		private static IDisposable SubscriveDialogResultChange(Window window, bool? now) {
			return window.ObserveEveryValueChanged(_ => _.DialogResult)
				.Where(_ => _ != now)
				.Subscribe(result => {
					SetDialogResult(window, result);
				});
		}

		private static IDisposable GetDialogResultSubscriber(DependencyObject obj) {
			return (IDisposable)obj.GetValue(DialogResultSubscriberProperty);
		}

		private static void SetDialogResultSubscriber(DependencyObject obj, IDisposable value) {
			obj.SetValue(DialogResultSubscriberProperty, value);
		}

		// Using a DependencyProperty as the backing store for DialogResultSubscriber.  This enables animation, styling, binding, etc...
		private static readonly DependencyProperty DialogResultSubscriberProperty =
			DependencyProperty.RegisterAttached("DialogResultSubscriber", typeof(IDisposable), typeof(WindowUtility), new PropertyMetadata(null));

		#endregion

		#region IsActive

		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static bool GetIsActive(DependencyObject obj) {
			return (bool)obj.GetValue(IsActiveProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static void SetIsActive(DependencyObject obj, bool value) {
			obj.SetValue(IsActiveProperty, value);
		}

		// Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsActiveProperty =
			DependencyProperty.RegisterAttached("IsActive", typeof(bool), typeof(WindowUtility), new FrameworkPropertyMetadata(false, (s, e) => {
				var win = (Window)s;

				if ((bool)e.NewValue) {
					win.Activate();
				}

				win.Activated -= Win_Activated;
				win.Deactivated -= Win_Activated;
				win.Activated += Win_Activated;
				win.Deactivated += Win_Activated;
			}) {
				BindsTwoWayByDefault = true
			});

		private static void Win_Activated(object sender, EventArgs e) {
			var win = (Window)sender;
			SetIsActive(win, win.IsActive);
		}

		#endregion

		#region GetMainWindowViewModel

		public static VM::Windows.MainWindowViewModel GetMainWindowViewModel(this DependencyObject obj) {
			obj.ThrowIfNull(nameof(obj));

			var window = Window.GetWindow(obj);
			return window.DataContext as VM::Windows.MainWindowViewModel;
		}

		#endregion
	}
}