using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CatWalk.Heron.View;
using CatWalk.Heron.ViewModel.Windows;
using CatWalk.Windows;

namespace CatWalk.Heron {
	using Win32 = CatWalk.Win32;

	public partial class App : Application{
		public IEnumerable<ViewViewModelPair<MainWindow, MainWindowViewModel>> MainWindows {
			get {
				return this.Windows.OfType<MainWindow>()
					.Select(w => new ViewViewModelPair<MainWindow, MainWindowViewModel>(w, w.DataContext as MainWindowViewModel));
			}
		}

		public void ArrangeMainWindows(ArrangeMode mode) {
			Arranger arranger = null;
			switch(mode) {
				case ArrangeMode.Cascade: arranger = new CascadeArranger(); break;
				case ArrangeMode.TileHorizontal: arranger = new TileHorizontalArranger(); break;
				case ArrangeMode.TileVertical: arranger = new TileVerticalArranger(); break;
				case ArrangeMode.StackHorizontal: arranger = new StackHorizontalArranger(); break;
				case ArrangeMode.StackVertical: arranger = new StackVerticalArranger(); break;
			}

			foreach(var screen in Win32::Screen.GetMonitors()) {
				var windows = this.MainWindows
					.Select(p => p.View)
					.Where(w => w.WindowState != WindowState.Minimized)
					.OrderWindowByZOrder()
					.Where(w => {
						var rect = new Int32Rect((int)w.Left, (int)w.Top, (int)w.Width, (int)w.Height);
						var inter = rect.Intersect(screen.ScreenArea);
						var area = inter.Area;
						return area >= rect.Area - area;
					}).ToArray();

				var rects = arranger.Arrange(new Size(screen.WorkingArea.Width, screen.WorkingArea.Height), windows.Length);

				for(var i = 0; i < rects.Length; i++) {
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
	}

	public enum ArrangeMode {
		Cascade,
		TileHorizontal,
		TileVertical,
		StackHorizontal,
		StackVertical,
	}

	public interface IViewViewModelPair<out TView, out TViewModel> {
		TView View { get; }
		TViewModel ViewModel { get; }
	}

	public struct ViewViewModelPair<TView, TViewModel> : IViewViewModelPair<TView, TViewModel>, IEquatable<IViewViewModelPair<TView, TViewModel>>
		where TView : Window {
		public TView View { get; private set; }
		public TViewModel ViewModel { get; private set; }

		public ViewViewModelPair(TView view, TViewModel vm)
			: this() {
			this.View = view;
			this.ViewModel = vm;
		}

		#region IEquatable<IViewViewModelPair<TView,TViewModel>> Members

		public bool Equals(IViewViewModelPair<TView, TViewModel> other) {
			return EqualityComparer<TView>.Default.Equals(this.View, other.View) && EqualityComparer<TViewModel>.Default.Equals(this.ViewModel, other.ViewModel);
		}

		public override bool Equals(object obj) {
			if(obj is ViewViewModelPair<TView, TViewModel>) {
				return this.Equals((ViewViewModelPair<TView, TViewModel>)obj);
			} else {
				return base.Equals(obj);
			}
		}

		public override int GetHashCode() {
			var h = this.View.GetHashCode();
			if(this.ViewModel != null) {
				h ^= this.ViewModel.GetHashCode();
			}
			return h;
		}

		#endregion

	}
}
