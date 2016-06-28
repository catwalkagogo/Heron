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
using CatWalk.Mvvm;

namespace CatWalk.Heron.ViewModel.Windows {
	public class WindowViewModel : AppWindowsViewModel {
		public WindowViewModel(Application app) : base(app){
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

		private Rect<double> _RestoreBounds;
		public virtual Rect<double> RestoreBounds {
			get {
				return this._RestoreBounds;
			}
			set {
				this._RestoreBounds = value;
				this.OnPropertyChanged("RestoreBounds");
			}
		}

		private Nullable<bool> _DialogResult;
		public Nullable<bool> DialogResult {
			get {
				return this._DialogResult;
			}
			set {
				this._DialogResult = value;
				this.OnPropertyChanged("DialogResult");
			}
		}

		private bool _IsActive = false;
		public bool IsActive {
			get {
				return this._IsActive;
			}
			set {
				this._IsActive = value;
				this.OnPropertyChanged("IsActive");
			}
		}

	}
}
