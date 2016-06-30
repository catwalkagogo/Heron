using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using CatWalk.Collections;
using CatWalk.Heron.ViewModel.IOSystem;
using System.Collections;

namespace CatWalk.Heron.ViewModel.Windows {
	public class PanelCollectionViewModel : AppWindowsViewModel, IReadOnlyObservableList<PanelViewModel> {
		private readonly WrappedObservableList<PanelViewModel> _Panels;

		/// <summary>
		/// _Panelsへの変更をChildrenに伝える
		/// </summary>
		private readonly CollectionSynchronizer _Synchronizer;

		public PanelCollectionViewModel(Application app) : base(app) {
			this._Panels = new WrappedObservableList<PanelViewModel>(new SkipList<PanelViewModel>());

			this._Synchronizer = this._Panels.NotifyToCollection(this.Children);
			this.Disposables.Add(this._Synchronizer);

			this.AddPanelCommand = new ReactiveCommand<string>();
			this.AddPanelCommand.Subscribe(this.AddPanel);
			this.Disposables.Add(this.AddPanelCommand);
		}

		#region IReadOnlyObservableList<PanelViewModel>

		public PanelViewModel this[int index] {
			get {
				return _Panels[index];
			}
		}


		public int Count {
			get {
				return _Panels.Count;
			}
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public IEnumerator<PanelViewModel> GetEnumerator() {
			return _Panels.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return _Panels.GetEnumerator();
		}

		#endregion

		#region AddPanel

		public ReactiveCommand<string> AddPanelCommand {
			get;
			private set;
		}

		public void AddPanel(string path) {
			SystemEntryViewModel vm;
			var app = this.Application;
			if(path.IsNullOrEmpty()) {
				vm = app.Entry;
			} else {
				var result = app.ParseEntryPath(path);
				if (!result.Success) {
					vm = app.Entry;
				}else {
					vm = result.Entry;
				}
			}

			var panel = new PanelViewModel(this.Application);
			panel.ListView = new ListViewModel(this.Application, vm);
			this._Panels.Add(panel);
		}

		#endregion

		#region GetAnotherPanel

		public async Task<PanelViewModel> GetAnotherPanel(PanelViewModel current) {
			var others = this._Panels.Where(panel => panel != current).ToArray();
			if(others.Length == 0) {
				return null;
			} else if(others.Length == 1) {
				return others[0];
			} else {
				var m = new Messages.SelectItemsMessage(this._Panels);
				await this.Messenger.Post(m, this);
				return m.SelectedItems.Cast<PanelViewModel>().FirstOrDefault();
			}
		}

		#endregion

	}
}
