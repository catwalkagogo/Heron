using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using CatWalk.Win32;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace CatWalk.Heron.View {
	public class Window : System.Windows.Window{
		public Window() {
			this.DataContextChanged += this.Window_DataContextChanged;
			this.Activated += this.Window_Activated;
			this.Deactivated += this.Window_Deactivated;
		}

		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);

			if(this.DataContext != null) {
				var m = new WindowMessages.ClosingMessage(this);
				Application.Current.Messenger.Send(m, this.DataContext);

				e.Cancel = m.Cancel;
			}
		}

		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);

			if (this.DataContext != null) {
				Application.Current.Messenger.Send(new WindowMessages.CloseMessage(this), this.DataContext);
			}
		}

		protected override void OnStateChanged(EventArgs e) {
			base.OnStateChanged(e);

			if(this.WindowState != WindowState.Minimized) {
				this.RestoreWindowState = this.WindowState;
			}

		}

		public WindowState RestoreWindowState {
			get { return (WindowState)GetValue(RestoreWindowStateProperty); }
			set { SetValue(RestoreWindowStateProperty, value); }
		}

		// Using a DependencyProperty as the backing store for RestoreWindowState.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty RestoreWindowStateProperty =
			DependencyProperty.Register("RestoreWindowState", typeof(WindowState), typeof(Window), new FrameworkPropertyMetadata(WindowState.Normal) {
				BindsTwoWayByDefault = true,
			});


		#region Messaging

		private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			var messenger = Application.Current.Messenger;

			var vm = e.OldValue;
			if (vm != null) {
				// Unregister
				messenger.Unregister<WindowMessages.RequestIsActiveMessage>(this.RequestIsActiveMessage, vm);
				messenger.Unregister<WindowMessages.SetIsActiveMessage>(this.SetIsActiveMessage, vm);
				messenger.Unregister<WindowMessages.SetRestoreBoundsMessage>(this.SetRestoreBoundsMessage, vm);
				messenger.Unregister<WindowMessages.RequestRestoreBoundsMessage>(this.RequestRestoreBoundsMessage, vm);
				messenger.Unregister<WindowMessages.RequestDialogResultMessage>(this.RequestDialogResultMessage, vm);
				messenger.Unregister<WindowMessages.SetDialogResultMessage>(this.SetDialogResultMessage, vm);
			}

			vm = e.NewValue;
			if (vm != null) {
				messenger.Register<WindowMessages.RequestIsActiveMessage>(this.RequestIsActiveMessage, vm);
				messenger.Register<WindowMessages.SetIsActiveMessage>(this.SetIsActiveMessage, vm);
				messenger.Register<WindowMessages.SetRestoreBoundsMessage>(this.SetRestoreBoundsMessage, vm);
				messenger.Register<WindowMessages.RequestRestoreBoundsMessage>(this.RequestRestoreBoundsMessage, vm);
				messenger.Register<WindowMessages.RequestDialogResultMessage>(this.RequestDialogResultMessage, vm);
				messenger.Register<WindowMessages.SetDialogResultMessage>(this.SetDialogResultMessage, vm);
				messenger.Send<Messages.DataContextAttachedMessage>(new Messages.DataContextAttachedMessage(this), vm);
			}
		}

		private void RequestRestoreBoundsMessage(WindowMessages.RequestRestoreBoundsMessage m) {
			m.Bounds = this.RestoreBounds;
		}

		private void RequestDialogResultMessage(WindowMessages.RequestDialogResultMessage m) {
			m.DialogResult = this.DialogResult;
		}

		private void SetDialogResultMessage(WindowMessages.SetDialogResultMessage m) {
			this.DialogResult = m.DialogResult;
		}

		private void RequestIsActiveMessage(WindowMessages.RequestIsActiveMessage m) {
			m.IsActive = this.IsActive;
		}

		private void SetIsActiveMessage(WindowMessages.SetIsActiveMessage m) {
			this.Activate();
		}

		private void SetRestoreBoundsMessage(WindowMessages.SetRestoreBoundsMessage m) {
			/*var hwnd = new WindowInteropHelper(this).Handle;
			var placement = new WindowPlacement();

			User32.GetWindowPlacement(hwnd, ref placement);

			placement.NormalPosition = new Rectangle() {
				Left = (int)m.Bounds.Left,
				Right = (int)m.Bounds.Right,
				Top = (int)m.Bounds.Top,
				Bottom = (int)m.Bounds.Bottom,
			};
			User32.SetWindowPlacement(hwnd, ref placement);*/
			var prevWS = this.WindowState;
			if(prevWS == WindowState.Maximized) {
				this.WindowState = WindowState.Normal;
			}
			this.Left = m.Bounds.Left;
			this.Top = m.Bounds.Top;
			this.Width = m.Bounds.Width;
			this.Height = m.Bounds.Height;
			if(prevWS != this.WindowState) {
				this.WindowState = prevWS;
			}
		}

		#endregion

		private void Window_Activated(object sender, EventArgs e) {
			if (this.DataContext != null) {
				Application.Current.Messenger.Send(new WindowMessages.ActivatedMessage(this), this.DataContext);
			}
		}

		private void Window_Deactivated(object sender, EventArgs e) {
			if (this.DataContext != null) {
				Application.Current.Messenger.Send(new WindowMessages.DeactivatedMessage(this), this.DataContext);
			}
		}
	}
}
