using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using CatWalk.Mvvm;

namespace CatWalk.Heron.Windows {
	public static partial class Messaging {
		#region IsReceiveWindowMessages

		private static readonly DependencyProperty WindowMessageListenerProperty = DependencyProperty.RegisterAttached("WindowMessageListener", typeof(WindowMessageListener), typeof(Messaging));
		public static readonly DependencyProperty IsCommunicateWindowMessagesProperty = DependencyProperty.RegisterAttached(
			"IsCommunicateWindowMessages",
			typeof(bool),
			typeof(Messaging),
			new PropertyMetadata(
				false,
				(d, e) => {
					var window = d as Window;
					if(window != null) {
						var nv = (bool)e.NewValue;
						var ov = (bool)e.OldValue;
						if(nv != ov) {
							if(nv) {
								AttachWindowListeners(window);
							} else {
								DettachWindowListeners(window);
							}
						}
					}
				}
			)
		);

		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static void SetIsCommunicateWindowMessages(Window window, bool v) {
			window.SetValue(IsCommunicateWindowMessagesProperty, v);
		}

		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static bool GetIsCommunicateWindowMessages(Window window) {
			return (bool)window.GetValue(IsCommunicateWindowMessagesProperty);
		}


		private static void AttachWindowListeners(Window window) {
			AttachWindowListeners(window, window.DataContext);
		}
		private static void AttachWindowListeners(Window window, object vm) {
			var listener = new WindowMessageListener(window, Application.Current.Messenger, vm);
			window.SetValue(WindowMessageListenerProperty, listener);

			window.DataContextChanged += window_DataContextChanged;
		}

		private static void window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			var window = (Window)sender;
			DettachWindowListeners(window, e.OldValue);
			if(e.NewValue != null) {
				AttachWindowListeners(window, e.NewValue);
			}
		}

		private static void DettachWindowListeners(Window window) {
			DettachWindowListeners(window, window.DataContext);
		}

		private static void DettachWindowListeners(Window window, object vm) {
			var listener = (WindowMessageListener)window.GetValue(WindowMessageListenerProperty);
			if(listener != null) {
				window.SetValue(WindowMessageListenerProperty, null);
				listener.Dispose();
			}
		}

		#endregion

		#region WindowMessageListener
		private class WindowMessageListener : DisposableObject {
			public Window Window { get; private set; }
			public object DataContext { get; private set; }
			public Messenger Messenger { get; private set; }
			public WindowMessageListener(Window window, Messenger messenger, object dataContext) {
				window.ThrowIfNull("window");
				messenger.ThrowIfNull("messenger");

				this.Window = window;
				this.Messenger = messenger;
				this.DataContext = dataContext;

				this.Attach();
			}

			private void Attach() {
				this.ThrowIfDisposed();

				var vm = this.DataContext;

				this.Messenger.Register<WindowMessages.RequestRestoreBoundsMessage>(this.WindowMessages_RequestRestoreBoundsMessage, vm);
				this.Messenger.Register<WindowMessages.SetRestoreBoundsMessage>(this.WindowMessages_SetRestoreBoundsMessage, vm);
				this.Messenger.Register<WindowMessages.CloseMessage>(this.WindowMessages_CloseMessage, vm);
				this.Messenger.Register<WindowMessages.MessageBoxMessage>(this.WindowMessages_MessageBoxMessage, vm);
				this.Messenger.Register<WindowMessages.RequestDialogResultMessage>(this.WindowMessages_RequestDialogResultMessage, vm);
				this.Messenger.Register<WindowMessages.SetDialogResultMessage>(this.WindowMessages_SetDialogResultMessage, vm);
				this.Messenger.Register<WindowMessages.RequestIsActiveMessage>(this.WindowMessages_RequestIsActiveMessage, vm);
				this.Messenger.Register<WindowMessages.SetIsActiveMessage>(this.WindowMessages_SetIsActiveMessage, vm);
				this.Messenger.Register<WindowMessages.RequestHandleMessage>(this.WindowMessages_RequestHandleMessage, vm);

				var window = this.Window;
				window.Activated += window_Activated;
				window.Deactivated += window_Deactivated;
				window.Closing += Window_Closing;
			}

			private void Dettach() {
				var vm = this.DataContext;

				this.Messenger.Unregister<WindowMessages.RequestRestoreBoundsMessage>(this.WindowMessages_RequestRestoreBoundsMessage, vm);
				this.Messenger.Unregister<WindowMessages.SetRestoreBoundsMessage>(this.WindowMessages_SetRestoreBoundsMessage, vm);
				this.Messenger.Unregister<WindowMessages.CloseMessage>(this.WindowMessages_CloseMessage, vm);
				this.Messenger.Unregister<WindowMessages.MessageBoxMessage>(this.WindowMessages_MessageBoxMessage, vm);
				this.Messenger.Unregister<WindowMessages.RequestDialogResultMessage>(this.WindowMessages_RequestDialogResultMessage, vm);
				this.Messenger.Unregister<WindowMessages.SetDialogResultMessage>(this.WindowMessages_SetDialogResultMessage, vm);
				this.Messenger.Unregister<WindowMessages.RequestIsActiveMessage>(this.WindowMessages_RequestIsActiveMessage, vm);
				this.Messenger.Unregister<WindowMessages.SetIsActiveMessage>(this.WindowMessages_SetIsActiveMessage, vm);
				this.Messenger.Unregister<WindowMessages.RequestHandleMessage>(this.WindowMessages_RequestHandleMessage, vm);

				var window = this.Window;
				window.Activated -= window_Activated;
				window.Deactivated -= window_Deactivated;
				window.Closing -= Window_Closing;

			}

			private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
				var win = (Window)sender;
				var m = new WindowMessages.ClosingMessage(sender);
				this.Messenger.Send(m, win.DataContext);
				e.Cancel = m.Cancel;
			}

			private void window_Deactivated(object sender, EventArgs e) {
				var win = (Window)sender;
				this.Messenger.Send(new WindowMessages.DeactivatedMessage(sender), win.DataContext);
			}

			private void window_Activated(object sender, EventArgs e) {
				var win = (Window)sender;
				this.Messenger.Send(new WindowMessages.ActivatedMessage(sender), win.DataContext);
			}



			private void WindowMessages_RequestRestoreBoundsMessage(WindowMessages.RequestRestoreBoundsMessage m) {
				m.Bounds = this.Window.RestoreBounds;
			}

			private void WindowMessages_SetRestoreBoundsMessage(WindowMessages.SetRestoreBoundsMessage m) {
				var state = this.Window.WindowState;
				this.Window.WindowState = WindowState.Normal;
				this.Window.Top = m.Bounds.Top;
				this.Window.Left = m.Bounds.Left;
				this.Window.Width = m.Bounds.Width;
				this.Window.Height = m.Bounds.Height;
				this.Window.WindowState = state;
			}

			private void WindowMessages_CloseMessage(WindowMessages.CloseMessage m) {
				this.Window.Close();
			}

			private void WindowMessages_MessageBoxMessage(WindowMessages.MessageBoxMessage m) {
				m.Result = MessageBox.Show(this.Window, m.Message, m.Title, m.Button, m.Image, m.Default, m.Options);
			}

			private void WindowMessages_RequestDialogResultMessage(WindowMessages.RequestDialogResultMessage m) {
				m.DialogResult = this.Window.DialogResult;
			}

			private void WindowMessages_SetDialogResultMessage(WindowMessages.SetDialogResultMessage m) {
				this.Window.DialogResult = m.DialogResult;
			}

			private void WindowMessages_RequestIsActiveMessage(WindowMessages.RequestIsActiveMessage m) {
				m.IsActive = this.Window.IsActive;
			}

			private void WindowMessages_SetIsActiveMessage(WindowMessages.SetIsActiveMessage m) {
				if(m.IsActive) {
					this.Window.Activate();
				} else {
				}
			}

			private void WindowMessages_RequestHandleMessage(WindowMessages.RequestHandleMessage m) {
				var wint = new WindowInteropHelper(this.Window);
				wint.EnsureHandle();
				using (var source = HwndSource.FromHwnd(wint.Handle)) {
					m.Handle = m.Handle;
				}
			}

			protected override void Dispose(bool disposing) {
				this.Dettach();

				base.Dispose(disposing);
			}
		}
		#endregion
	}
}
