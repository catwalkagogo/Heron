using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using CatWalk.Mvvm;
using Reactive.Bindings.Extensions;

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
			var listener = new WindowMessageListener(window, Application.Current.Messenger);
			window.SetValue(WindowMessageListenerProperty, listener);
		}

		private static void DettachWindowListeners(Window window) {
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
			public Messenger Messenger { get; private set; }
			private CompositeDisposable _Disposables = new CompositeDisposable();

			public WindowMessageListener(Window window, Messenger messenger) {
				window.ThrowIfNull("window");
				messenger.ThrowIfNull("messenger");

				this.Window = window;
				this.Messenger = messenger;

				this.Attach();
			}

			private void Attach() {
				this.ThrowIfDisposed();

				this._Disposables.Add(
					this.Messenger.ObserveDataContextMessage<WindowMessages.CloseMessage>(this.Window)
						.ObserveOnUIDispatcher()
						.Subscribe(this.WindowMessages_CloseMessage));
				this._Disposables.Add(
					this.Messenger.ObserveDataContextMessage<WindowMessages.MessageBoxMessage>(this.Window)
						.ObserveOnUIDispatcher()
						.Subscribe(this.WindowMessages_MessageBoxMessage));
				this._Disposables.Add(
					this.Messenger.ObserveDataContextMessage<WindowMessages.RequestHandleMessage>(this.Window)
						.ObserveOnUIDispatcher()
						.Subscribe(this.WindowMessages_RequestHandleMessage));

				var window = this.Window;
				window.Closing += Window_Closing;
			}

			private void Dettach() {
				this._Disposables.Dispose();

				var window = this.Window;
				window.Closing -= Window_Closing;

			}

			private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
				var win = (Window)sender;
				var m = new WindowMessages.ClosingMessage();
				this.Messenger.Send(m, win.DataContext);
				e.Cancel = m.Cancel;
			}

			private void WindowMessages_CloseMessage(WindowMessages.CloseMessage m) {
				this.Window.Close();
			}

			private void WindowMessages_MessageBoxMessage(WindowMessages.MessageBoxMessage m) {
				var result = MessageBox.Show(this.Window, m.Message, m.Title, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.None);
				m.Result =
					(result == MessageBoxResult.OK || result == MessageBoxResult.Yes) ? new Nullable<bool>(true) :
					(result == MessageBoxResult.No) ? new Nullable<bool>(false) :
					new Nullable<bool>();
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
