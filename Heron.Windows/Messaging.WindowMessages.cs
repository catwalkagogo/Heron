using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
			var listener = new WindowMessageListener(window);
			window.SetValue(WindowMessageListenerProperty, listener);
			listener.Attach(vm, window);

			window.DataContextChanged += window_DataContextChanged;
		}

		private static void window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			var window = (Window)sender;
			if(e.OldValue != null) {
				DettachWindowListeners(window, e.OldValue);
			}
			if(e.NewValue != null) {
				AttachWindowListeners(window, e.NewValue);
			}
		}

		private static void DettachWindowListeners(Window window) {
			DettachWindowListeners(window, window.DataContext);
		}

		private static void DettachWindowListeners(Window window, object vm) {
			if(vm != null) {
				var listener = (WindowMessageListener)window.GetValue(WindowMessageListenerProperty);
				if(listener != null) {
					window.SetValue(WindowMessageListenerProperty, null);
					listener.Dettach(vm, window);
				}
			}
		}

		#endregion

		#region WindowMessageListener
		private class WindowMessageListener {
			public Window Window { get; private set; }
			public WindowMessageListener(Window window) {
				window.ThrowIfNull("window");
				this.Window = window;
			}

			public void Attach(object vm, Window window) {
				Application.Current.Messenger.Register<WindowMessages.RequestRestoreBoundsMessage>(this.WindowMessages_RequestRestoreBoundsMessage, vm);
				Application.Current.Messenger.Register<WindowMessages.SetRestoreBoundsMessage>(this.WindowMessages_SetRestoreBoundsMessage, vm);
				Application.Current.Messenger.Register<WindowMessages.CloseMessage>(this.WindowMessages_CloseMessage, vm);
				Application.Current.Messenger.Register<WindowMessages.MessageBoxMessage>(this.WindowMessages_MessageBoxMessage, vm);
				Application.Current.Messenger.Register<WindowMessages.RequestDialogResultMessage>(this.WindowMessages_RequestDialogResultMessage, vm);
				Application.Current.Messenger.Register<WindowMessages.SetDialogResultMessage>(this.WindowMessages_SetDialogResultMessage, vm);
				Application.Current.Messenger.Register<WindowMessages.RequestIsActiveMessage>(this.WindowMessages_RequestIsActiveMessage, vm);
				Application.Current.Messenger.Register<WindowMessages.SetIsActiveMessage>(this.WindowMessages_SetIsActiveMessage, vm);

				window.Activated += window_Activated;
				window.Deactivated += window_Deactivated;
				window.Closing += Window_Closing;
			}

			public void Dettach(object vm, Window window) {
				Application.Current.Messenger.Unregister<WindowMessages.RequestRestoreBoundsMessage>(this.WindowMessages_RequestRestoreBoundsMessage, vm);
				Application.Current.Messenger.Unregister<WindowMessages.SetRestoreBoundsMessage>(this.WindowMessages_SetRestoreBoundsMessage, vm);
				Application.Current.Messenger.Unregister<WindowMessages.CloseMessage>(this.WindowMessages_CloseMessage, vm);
				Application.Current.Messenger.Unregister<WindowMessages.MessageBoxMessage>(this.WindowMessages_MessageBoxMessage, vm);
				Application.Current.Messenger.Unregister<WindowMessages.RequestDialogResultMessage>(this.WindowMessages_RequestDialogResultMessage, vm);
				Application.Current.Messenger.Unregister<WindowMessages.SetDialogResultMessage>(this.WindowMessages_SetDialogResultMessage, vm);
				Application.Current.Messenger.Unregister<WindowMessages.RequestIsActiveMessage>(this.WindowMessages_RequestIsActiveMessage, vm);
				Application.Current.Messenger.Unregister<WindowMessages.SetIsActiveMessage>(this.WindowMessages_SetIsActiveMessage, vm);

				window.Activated -= window_Activated;
				window.Deactivated -= window_Deactivated;
				window.Closing -= Window_Closing;

			}

			private static void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
				var win = (Window)sender;
				var m = new WindowMessages.ClosingMessage(sender);
				Application.Current.Messenger.Send(m, win.DataContext);
				e.Cancel = m.Cancel;
			}

			private static void window_Deactivated(object sender, EventArgs e) {
				var win = (Window)sender;
				Application.Current.Messenger.Send(new WindowMessages.DeactivatedMessage(sender), win.DataContext);
			}

			private static void window_Activated(object sender, EventArgs e) {
				var win = (Window)sender;
				Application.Current.Messenger.Send(new WindowMessages.ActivatedMessage(sender), win.DataContext);
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

		}
		#endregion
	}
}
