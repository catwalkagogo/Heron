using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Reactive;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;


namespace CatWalk.Heron.ViewModel.Windows {
	public class WindowViewModel : AppWindowsViewModel {
		public WindowViewModel(Application app) : base(app){
			var messenger = this.Messenger;
			messenger.Register<WindowMessages.ActivatedMessage>(this.Activated, this);
			messenger.Register<WindowMessages.DeactivatedMessage>(this.Deactivated, this);

			// 最小化から復元時のウィンドウの状態を取得
			/*
			if(this.WindowState != WindowState.Minimized) {
				this.RestoreWindowState = this.WindowState;
			}
			this.Disposables.Add(this.ObserveProperty(self => self.WindowState)
				.Where(state => state != WindowState.Minimized)
				.Subscribe(state => {
					this.RestoreWindowState = state;
				}));*/
		}

		private string _Title;
		public virtual string Title {
			get {
				return this._Title;
			}
			set {
				this._Title = value;
				this.OnPropertyChanged("Title");
			}
		}

		private double _Top;
		public virtual double Top {
			get {
				return this._Top;
			}
			set {
				this._Top = value;
				this.OnPropertyChanged("Top");
			}
		}


		private double _Left;
		public virtual double Left {
			get {
				return this._Left;
			}
			set {
				this._Left = value;
				this.OnPropertyChanged("Left");
			}
		}

		private double _Width = 480;
		public virtual double Width {
			get {
				return this._Width;
			}
			set {
				this._Width = value;
				this.OnPropertyChanged("Width");
			}
		}

		private double _Height = 320;
		public virtual double Height {
			get {
				return this._Height;
			}
			set {
				this._Height = value;
				this.OnPropertyChanged("Height");
			}
		}

		private WindowState _WindowState = WindowState.Normal;
		public virtual WindowState WindowState {
			get {
				return this._WindowState;
			}
			set {
				this._WindowState = value;
				this.OnPropertyChanged("WindowState");
			}
		}

		private WindowState _RestoreWindowState = WindowState.Normal;
		public virtual WindowState RestoreWindowState {
			get {
				return this._RestoreWindowState;
			}
			set {
				this._RestoreWindowState = value;
				this.OnPropertyChanged("RestoreWindowState");
			}
		}

		public virtual Rect RestoreBounds {
			get {
				var m = new WindowMessages.RequestRestoreBoundsMessage(this);
				this.Messenger.Send(m, this);
				return m.Bounds;
			}
			set {
				var m = new WindowMessages.SetRestoreBoundsMessage(this, value);
				this.Messenger.Post(m, this);
			}
		}

		public Nullable<bool> DialogResult {
			get {
				var m = new WindowMessages.RequestDialogResultMessage(this);
				this.Messenger.Send(m, this);
				return m.DialogResult;
			}
			set {
				this.Messenger.Post(new WindowMessages.SetDialogResultMessage(this, value), this);
			}
		}

		private bool _IsActive = false;
		public bool IsActive {
			get {
				var m = new WindowMessages.RequestIsActiveMessage(this);
				this.Messenger.Send(m, this);
				return m.IsActive;
			}
			set {
				this._IsActive = value;
				this.Messenger.Post(new WindowMessages.SetIsActiveMessage(this, value), this);
				this.OnPropertyChanged("IsActive");
			}
		}

		private void Activated(WindowMessages.ActivatedMessage m) {
			this._IsActive = true;
			this.OnPropertyChanged("IsActive");
		}

		private void Deactivated(WindowMessages.DeactivatedMessage m) {
			this._IsActive = false;
			this.OnPropertyChanged("IsActive");
		}
	}
}
