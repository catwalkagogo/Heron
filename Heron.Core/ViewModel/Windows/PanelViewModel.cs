using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;
using CatWalk.Heron.ViewModel.IOSystem;
using Reactive.Bindings;

namespace CatWalk.Heron.ViewModel.Windows {
	public class PanelViewModel : AppWindowsViewModel{
		public PanelCollectionViewModel Panels { get; private set; }
		private ReactiveProperty<bool> _IsActive;

		private ListViewModel _ListView;
		public PanelViewModel(PanelCollectionViewModel panels, SystemEntryViewModel entry) : base(panels.Application) {
			entry.ThrowIfNull("entry");

			this.Panels = panels;
			this.ListView = new ListViewModel(panels.Application, entry);

			// IsActive
			this._IsActive = this.Panels
				.ObserveProperty(_ => _.ActivePanel)
				.Select(panel => panel == this)
				.ToReactiveProperty();
			this.Disposables.Add(this._IsActive);
			this.Disposables.Add(this._IsActive.Subscribe(_ => {
				this.OnPropertyChanged("IsActive");
			}));
		}

		public bool IsActive {
			get {
				return this._IsActive.Value;
			}
			set {
				this.Panels.ActivePanel = this;
			}
		}

		public ListViewModel ListView {
			get {
				return this._ListView;
			}
			private set {
				var old = this._ListView;
				if(old != null) {
					this.Children.Remove(old);
				}

				this._ListView = value;
				this.OnPropertyChanged("ListView");

				if(value != null) {
					this.Children.Add(value);
				}
			}
		}
	}
}
