using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CatWalk.Mvvm;
using System.Reactive;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Threading;

namespace CatWalk.Heron.ViewModel.Windows {
	public abstract class AppWindowsViewModel : ControlViewModel{
		public ReactiveProperty<MainWindowViewModel> MainWindow { get; private set; }
		public Application Application { get; private set; }

		public AppWindowsViewModel(Application app) : this(app, null, app.SynchronizationContext){}

		public AppWindowsViewModel(Application app, ControlViewModel parent) : this(app, parent, app.SynchronizationContext) {}

		public AppWindowsViewModel(Application app, ControlViewModel parent, SynchronizationContext invoke)
			: base(parent, invoke) {
			app.ThrowIfNull();
			this.Application = app;

			this.MainWindow = this.ObserveProperty(_ => _.Ancestors)
				.OfType<AppWindowsViewModel>()
				.Select(vm => vm.MainWindow.Value)
				.Concat(
					this.ObserveProperty(_ => _.Ancestors)
						.OfType<MainWindowViewModel>()
				)
				.ToReactiveProperty();
			this.Disposables.Add(this.MainWindow);
		}

		public Messenger Messenger {
			get {
				return this.Application.Messenger;
			}
		}

		protected override void Dispose(bool disposing) {
			if (!this.IsDisposed) {
				this.Disposables.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
