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
using CatWalk.Windows;
using CatWalk.Heron.ViewModel.IOSystem;

namespace CatWalk.Heron.ViewModel.Windows {
	public class PanelCollectionViewModel : AppWindowsViewModel{
		private readonly WrappedObservableList<PanelViewModel> _Panels;

		/// <summary>
		/// _Panelsへの変更をChildrenに伝える
		/// </summary>
		private readonly ObservableCollectionSynchronizer _Synchronizer;

		public PanelCollectionViewModel(Application app) : base(app) {
			this._Panels = new WrappedObservableList<PanelViewModel>(new SkipList<PanelViewModel>());

			this._Synchronizer = this._Panels.NotifyToCollectionWeak(this.Children);

			this.AddPanelCommand = new ReactiveCommand<string>();
			this.AddPanelCommand.Subscribe(this.AddPanel);
		}

		public IReadOnlyObservableList<PanelViewModel> Panels {
			get {
				return this._Panels;
			}
		}

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
				if(!app.TryParseEntryPath(path, out vm)) {
					vm = app.Entry;
				}
			}

			var panel = new PanelViewModel(this.Application);
			panel.Content = new ListViewModel(this.Application, vm);
			this._Panels.Add(panel);
		}

		#endregion

		#region GetAnotherPanel

		public async Task<PanelViewModel> GetAnotherPanel(PanelViewModel current) {
			var others = this.Panels.Where(panel => panel != current).ToArray();
			if(others.Length == 0) {
				return null;
			} else if(others.Length == 1) {
				return others[0];
			} else {
				var m = new Messages.SelectItemsMessage(this, this._Panels);
				await this.Messenger.Post(m, this);
				return m.SelectedItems.Cast<PanelViewModel>().FirstOrDefault();
			}
		}

		#endregion

		protected override void Dispose(bool disposing) {
			new IDisposable[]{
				this.AddPanelCommand,
				this._Synchronizer,
			}.Dispose();
			base.Dispose(disposing);
		}
	}
}
