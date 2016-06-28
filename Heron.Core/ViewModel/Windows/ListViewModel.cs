using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk.Heron.ViewModel.IOSystem;
using CatWalk;
using CatWalk.Threading;
using CatWalk.Collections;
using CatWalk.Heron.IOSystem;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using CatWalk.IOSystem;

namespace CatWalk.Heron.ViewModel.Windows {
	public class ListViewModel : AppWindowsViewModel{
		private SystemEntryViewModel _Current;
		private object _EntryViewModel;
		private Job _NavigateJob;
		private readonly IHistoryStack<HistoryItem> _History = new HistoryStack<HistoryItem>();
		//private readonly ReactiveProperty<SystemEntryViewModel> _FocusedItem;

		private readonly Lazy<ReactiveCommand<string>> _OpenCommand;
		private readonly Lazy<ReactiveCommand> _GoUpCommand;
		private readonly Lazy<ReactiveCommand<ISystemEntry>> _CopyToCommand;
		private readonly Lazy<ReactiveCommand<ISystemEntry>> _MoveToCommand;
		private readonly Lazy<ReactiveCommand> _DeleteCommand;
		private readonly Lazy<ReactiveCommand> _RemoveCommand;
		private readonly Lazy<ReactiveProperty<PanelViewModel>> _Panel;
		private readonly Lazy<ReactiveProperty<PanelCollectionViewModel>> _Panels;
		private readonly Lazy<ReactiveProperty<IReadOnlyObservableList<PanelViewModel>>> _PanelsCollection;

		public ListViewModel(Application app, SystemEntryViewModel entry) : base(app){
			this._Current = entry;

			this.SelectedItems = new ObservableHashSet<SystemEntryViewModel>();
			/*
			this._FocusedItem =
				this.ObserveProperty(_ => _.CurrentEntry)
					.SelectMany(current => current.ChildrenView.ObserveProperty(_ => _.CurrentItem))
					.Cast<SystemEntryViewModel>()
					.ToReactiveProperty();
					*/
			this._OpenCommand = new Lazy<ReactiveCommand<string>>(() => {
				var cmd =
					this.ObserveProperty(_ => _.FocusedItem)
						.Select(_ => _ != null)
						.ToReactiveCommand<string>();
				this.Disposables.Add((cmd.Subscribe(this.Open)));
				this.Disposables.Add(cmd);
				return cmd;
			});
			this._GoUpCommand = new Lazy<ReactiveCommand>(() => {
				var cmd =
					this.ObserveProperty(_ => _.CurrentEntry)
						.Select(_ => _ != null && _.Parent != null)
						.ToReactiveCommand();
				this.Disposables.Add(cmd.Subscribe(_ => this.GoUp()));
				this.Disposables.Add(cmd);
				return cmd;
			});
			this._CopyToCommand = new Lazy<ReactiveCommand<ISystemEntry>>(() => {
				var cmd =
					Observable.Merge(
						this.SelectedItems.ObserveProperty(items => items.Count).Select(count => count > 0),
						this.PanelsCollection.Select(_ => _.Count > 1)
					).ToReactiveCommand<ISystemEntry>();
				this.Disposables.Add(cmd.Subscribe(this.CopyTo));
				this.Disposables.Add(cmd);
				return cmd;
			});
			this._MoveToCommand = new Lazy<ReactiveCommand<ISystemEntry>>(() => {
				var cmd =
					Observable.Merge(
						this.SelectedItems.ObserveProperty(items => items.Count).Select(count => count > 0),
						this.PanelsCollection.Select(_ => _.Count > 1)
					).ToReactiveCommand<ISystemEntry>();
				this.Disposables.Add(cmd.Subscribe(this.MoveTo));
				this.Disposables.Add(cmd);
				return cmd;
			});
			this._DeleteCommand = new Lazy<ReactiveCommand>(() => {
				var cmd =
					this.SelectedItems
						.ObserveProperty(items => items.Count)
						.Select(count => count > 0)
						.ToReactiveCommand();
				this.Disposables.Add(cmd.Subscribe(_ => this.Delete()));
				this.Disposables.Add(cmd);
				return cmd;
			});
			this._RemoveCommand = new Lazy<ReactiveCommand>(() => {
				var cmd =
					this.SelectedItems
						.ObserveProperty(items => items.Count)
						.Select(count => count > 0)
						.ToReactiveCommand();
				this.Disposables.Add(cmd.Subscribe(_ => this.Remove()));
				this.Disposables.Add(cmd);
				return cmd;
			});
			this._Panel = new Lazy<ReactiveProperty<PanelViewModel>>(() => {
				var prop =
					this.ObserveProperty(_ => _.Ancestors)
						.Select(_ => _.OfType<PanelViewModel>().FirstOrDefault())
						.ToReactiveProperty();
				this.Disposables.Add(prop.Subscribe(_ => this.OnPropertyChanged("Panel")));
				this.Disposables.Add(prop);
				return prop;
			});
			this._Panels = new Lazy<ReactiveProperty<PanelCollectionViewModel>>(() => {
				var prop =
					this.ObserveProperty(_ => _.Ancestors)
						.Select(_ => _.OfType<PanelCollectionViewModel>().FirstOrDefault())
						.ToReactiveProperty();
				this.Disposables.Add(prop.Subscribe(_ => this.OnPropertyChanged("Panels")));
				this.Disposables.Add(prop);
				return prop;
			});
			this._PanelsCollection = new Lazy<ReactiveProperty<IReadOnlyObservableList<PanelViewModel>>>(() => {
				var prop =
					this.Panels
						.Where(_ => _ != null)
						.SelectMany(_ => _.Panels.CollectionChangedAsObservable())
						.Select(_ => this.Panels.Value.Panels)
						.ToReactiveProperty();
				this.Disposables.Add(prop);
				return prop;
			});
		}

		/// <summary>
		/// 現在リスト表示に使用しているSystemEntryViewModelを設定・取得する
		/// </summary>
		public SystemEntryViewModel CurrentEntry {
			get {
				return this._Current;
			}
			private set {
				value.ThrowIfNull("value");
				this._Current = value;
				this.OnPropertyChanged("CurrentEntry");

				var provider = value.Provider;
				this.EntryViewModel = provider.GetViewModel(this, value, this.EntryViewModel);
			}
		}

		public object EntryViewModel {
			get {
				return this._EntryViewModel;
			}
			private set {
				if(value != this._EntryViewModel) {
					var old = this._EntryViewModel;
					if(old != null) {
						var oldControl = old as ControlViewModel;
						if(oldControl != null) {
							this.Children.Remove(oldControl);
						}
					}
					this._EntryViewModel = value;
					var control = value as ControlViewModel;
					if(control != null) {
						this.Children.Add(control);
					}

					this.OnPropertyChanged("EntryViewModel");
				}
			}
		}

		#region SelectedItems

		public IObservableCollection<SystemEntryViewModel> SelectedItems { get; private set; }

		#endregion

		#region FocusedItem

		private SystemEntryViewModel _FocusedItem;
		public SystemEntryViewModel FocusedItem {
			get {
				return this._FocusedItem;
			}
			set {
				this._FocusedItem = value;
				this.OnPropertyChanged("FocusedItem");
			}
		}

		#endregion

		#region Navigate

		private void Navigate(SystemEntryViewModel entry) {
			this.Navigate(entry, null);
		}

		private void Navigate(SystemEntryViewModel entry, string focusName) {
			if(this._NavigateJob != null) {
				this._NavigateJob.Cancel();
			}

			var old = this.CurrentEntry;
			if(old != null) {
				old.Exit();
			}
			entry.Enter();

			// change current
			this.CurrentEntry = entry;

			// add history
			{
				var focused = this.FocusedItem;
				var focusedName = focused != null ? focused.Name : null;
				this._History.Add(new HistoryItem(
					old.DisplayName,
					() => {
						this.Navigate(old, focusedName);
					}
				));
			}

			var job = this.CreateJob(_ => {
				entry.RefreshChildren(_.CancellationToken, _);
				SystemEntryViewModel focus;
				if(entry.Children.TryGetValue(focusName, out focus)) {
					//this.FocusedItem = focus;
					// TODO:
				}
			});
			this._NavigateJob = job;
			job.Start();
		}

		#endregion

		#region Open

		public ReactiveCommand<string> OpenCommand {
			get {
				return this._OpenCommand.Value;
			}
		}

		public void Open(string name){
			var entry = this.FocusedItem;
			if(entry.IsDirectory) {
				this.Navigate(entry);
			} else {
				var entries = this.SelectedItems.Select(ent => ent.Entry).ToArray();
				this.CreateJob(job => {
					this.Application.EntryOperator.Open(entries, job.CancellationToken, job);
				}).Start();
			}
		}

		public bool CanOpen(string name) {
			return this.FocusedItem != null || !name.IsNullOrEmpty();
		}

		#endregion

		#region GoUp

		public ReactiveCommand GoUpCommand {
			get {
				return this._GoUpCommand.Value;
			}
		}

		private void GoUp() {
			this.Navigate(this.CurrentEntry.Parent);
		}

		private bool CanGoUp() {
			return this.CurrentEntry != null && this.CurrentEntry.Parent != null;
		}

		#endregion

		#region History
		private class HistoryItem {
			public string Text { get; private set; }
			public Action _Back;

			public HistoryItem(string text, Action back) {
				this.Text = text;
				back.ThrowIfNull("back");
				this._Back = back;
			}

			public void Back() {
				this._Back();
			}

		}
		#endregion

		#region CopyTo


		public ReactiveCommand<ISystemEntry> CopyToCommand {
			get {
				return this._CopyToCommand.Value;
			}
		}

		public void CopyTo(ISystemEntry dest) {
			if(dest == null) {
				var panel = this.Panels.Value.GetAnotherPanel(this.Panel.Value);
				if(panel == null) {
					return;
				}
			}

			var entries = this.SelectedItems.Select(item => item.Entry).ToArray();

			this.CreateJob(job => {
				this.Application.EntryOperator.CopyTo(entries, dest, job.CancellationToken, job).Wait();
			}).Start();
		}

		#endregion

		#region Move


		public ReactiveCommand<ISystemEntry> MoveToCommand {
			get {
				return this._MoveToCommand.Value;
			}
		}

		public void MoveTo(ISystemEntry dest) {
			if(dest == null) {
				var panel = this.Panels.Value.GetAnotherPanel(this.Panel.Value);
				if(panel == null) {
					return;
				}
			}

			var entries = this.SelectedItems.Select(item => item.Entry).ToArray();
			this.CreateJob(job => {
				this.Application.EntryOperator.MoveTo(entries, dest, job.CancellationToken, job).Wait();
			}).Start();
		}

		#endregion

		#region Delete

		public ReactiveCommand DeleteCommand {
			get {
				return this._DeleteCommand.Value;
			}
		}

		public void Delete() {
			var entries = this.SelectedItems.Select(item => item.Entry).ToArray();
			this.CreateJob(job => {
				this.Application.EntryOperator.Delete(entries, false, job.CancellationToken, job).Wait();
			}).Start();
		}

		#endregion

		#region Remove


		public ReactiveCommand RemoveCommand {
			get {
				return this._RemoveCommand.Value;
			}
		}

		public void Remove() {
			var entries = this.SelectedItems.Select(item => item.Entry).ToArray();
			this.CreateJob(job => {
				this.Application.EntryOperator.Delete(entries, true, job.CancellationToken, job).Wait();
			}).Start();
		}

		#endregion

		#region NewItem
		/*
		public ReactiveCommand NewItemCommand {
			get {
				return this._NewItemCommand.Value;
			}
		}

		public void NewItem(string newName = null) {
			if(newName == null) {
				
			}
		}
		*/
		#endregion

		#region Panel

		public ReactiveProperty<PanelViewModel> Panel {
			get {
				return this._Panel.Value;
			}
		}

		public ReactiveProperty<PanelCollectionViewModel> Panels {
			get {
				return this._Panels.Value;
			}
		}

		public ReactiveProperty<IReadOnlyObservableList<PanelViewModel>> PanelsCollection {
			get {
				return this._PanelsCollection.Value;
			}
		}

		#endregion

		public class NewItemPromptMessage : WindowMessages.PromptMessage {
			public NewItemPromptMessage(object sender) { }
		}
	}
}
