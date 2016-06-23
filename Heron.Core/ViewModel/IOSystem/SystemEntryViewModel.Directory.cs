using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Windows.Data;
using CatWalk;
using CatWalk.Collections;
using CatWalk.Mvvm;
using CatWalk.IOSystem;
using CatWalk.Heron.IOSystem;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public partial class SystemEntryViewModel : ViewModelBase, IHierarchicalViewModel<SystemEntryViewModel>, IDisposable {
		private ResetLazy<ChildrenCollection> _Children;
		private ResetLazy<ChildrenCollectionView> _ChildrenView;
		private ILookup<string, EntryGroupDescription> _Groupings;
		private IIOSystemWatcher _Watcher;

		private void InitDirectory() {
			var entry = this.Entry;
			this._Children = new ResetLazy<ChildrenCollection>(() => new ChildrenCollection());
			this._ChildrenView = new ResetLazy<ChildrenCollectionView>(this.ChildrenViewFactory);
			var watchable = entry as IWatchable;
			if(watchable != null) {
				this._Watcher = watchable.Watcher;
				this._Watcher.IsEnabled = false;
				this._Watcher.CollectionChanged += _Watcher_CollectionChanged;
			}
			this._Groupings = this.Provider.GetGroupings(entry).ToLookup(grp => grp.ColumnName);
		}

		#region Enter / Exit

		public void Enter() {
			this.IsWatcherEnabled = true;
		}

		public void Exit() {
			this.IsWatcherEnabled = false;
			this.CancelTokenProcesses();
			this._Columns.Reset();
			this._Children.Reset();
			this._ChildrenView.Reset();
			this._ChildrenGroups.Clear();
		}

		#endregion

		#region Children

		private void ThrowIfNotDirectory() {
			if(!this.IsDirectory) {
				throw new InvalidOperationException("This entry is not a directory");
			}
		}
		
		public ChildrenCollection Children {
			get {
				this.ThrowIfNotDirectory();
				return this._Children.Value;
			}
		}
		
		IEnumerable<SystemEntryViewModel> IHierarchicalViewModel<SystemEntryViewModel>.Children {
			get {
				if(this.IsDirectory) {
					return this._Children.Value;
				} else {
					return new SystemEntryViewModel[0];
				}
			}
		}

		public void RefreshChildren() {
			this.RefreshChildren(this.CancellationToken, null);
		}
		public void RefreshChildren(CancellationToken token) {
			this.RefreshChildren(token, null);
		}
		public void RefreshChildren(CancellationToken token, IProgress<double> progress) {
			this.ThrowIfNotDirectory();

			this._Children.Value.Clear();

			var children = this.Entry.GetChildren(token, progress)
				.Select(child => GetViewModel(this, child));

			foreach(var child in children) {
				this._Children.Value.Add(child);
			}
		}

		#endregion

		#region ChildrenCollection
		public class ChildrenCollection : WrappedObservableList<SystemEntryViewModel> {
			private IDictionary<String, int> nameMap = new Dictionary<string, int>();

			public ChildrenCollection()
				: base(new SkipList<SystemEntryViewModel>()) {
				this.CollectionChanged += ChildrenCollection_CollectionChanged;
			}

			private void ChildrenCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
				lock(this.nameMap) {
					switch(e.Action) {
						case NotifyCollectionChangedAction.Add: {
								var i = e.NewStartingIndex;
								foreach(var item in e.NewItems.Cast<SystemEntryViewModel>()) {
									this.nameMap[item.Name] = i;
									i++;
								}
								break;
							}
						case NotifyCollectionChangedAction.Move: {
								foreach(var item in e.OldItems.Cast<SystemEntryViewModel>()) {
									this.nameMap.Remove(item.Name);
								}
								var i = e.NewStartingIndex;
								foreach(var item in e.NewItems.Cast<SystemEntryViewModel>()) {
									this.nameMap[item.Name] = i;
									i++;
								}
								break;
							}
						case NotifyCollectionChangedAction.Remove: {
								foreach(var item in e.OldItems.Cast<SystemEntryViewModel>()) {
									this.nameMap.Remove(item.Name);
								}
								break;
							}
						case NotifyCollectionChangedAction.Replace: {
								foreach(var item in e.OldItems.Cast<SystemEntryViewModel>()) {
									this.nameMap.Remove(item.Name);
								}
								var i = e.NewStartingIndex;
								foreach(var item in e.NewItems.Cast<SystemEntryViewModel>()) {
									this.nameMap[item.Name] = i;
									i++;
								}
								break;
							}
						case NotifyCollectionChangedAction.Reset: {
								this.nameMap.Clear();
								var i = e.NewStartingIndex;
								foreach(var item in e.NewItems.Cast<SystemEntryViewModel>()) {
									this.nameMap[item.Name] = i;
									i++;
								}
								break;
							}
					}
				}
			}

			#region IReadOnlyDictionary<string, SystemEntryViewModel>

			public bool ContainsKey(string key) {
				return this.nameMap.ContainsKey(key);
			}

			public IEnumerable<string> Keys {
				get {
					return this.nameMap.Keys;
				}
			}

			public bool TryGetValue(string key, out SystemEntryViewModel value) {
				int i;
				if(this.nameMap.TryGetValue(key, out i)) {
					value = this[i];
					return true;
				} else {
					value = null;
					return false;
				}
			}

			public IEnumerable<SystemEntryViewModel> Values {
				get {
					return this;
				}
			}

			public SystemEntryViewModel this[string key] {
				get {
					return this[this.nameMap[key]];
				}
			}

			#endregion

			public override int IndexOf(SystemEntryViewModel item) {
				item.ThrowIfNull();
				return this.IndexOf(item.Name);
			}

			public int IndexOf(ISystemEntry entry) {
				entry.ThrowIfNull();
				return this.IndexOf(entry.Name);
			}

			public int IndexOf(string name) {
				name.ThrowIfNull();
				int idx;
				if(this.nameMap.TryGetValue(name, out idx)) {
					return idx;
				} else {
					return -1;
				}
			}

			public bool RemoveByName(string name) {
				int i;
				if(this.nameMap.TryGetValue(name, out i)) {
					this.RemoveAt(i);
					return true;
				} else {
					return false;
				}
			}
		}

		#endregion

		#region ChildrenCollectionView

		private ChildrenCollectionView ChildrenViewFactory() {
			var view = new ChildrenCollectionView(this, this._Children.Value);
			using(view.DeferRefresh()) {
				view.ColumnOrder = Seq.Make(new ColumnOrderSet(ColumnDefinition.NameColumn, ListSortDirection.Ascending)).ToArray();
			}
			return view;
		}

		public ChildrenCollectionView ChildrenView {
			get {
				this.ThrowIfNotDirectory();
				return this._ChildrenView.Value;
			}
		}

		public class ChildrenCollectionView : ListCollectionView {
			public SystemEntryViewModel SourceEntry {
				get;
				private set;
			}
			public EntryFilterCollection Filters {
				get;
				private set;
			}
			private IEnumerable<ColumnOrderSet> _ColumnOrder;

			internal ChildrenCollectionView(SystemEntryViewModel source, System.Collections.IList collection)
				: base(collection) {
				source.ThrowIfNull("source");
				this.SourceEntry = source;
				this.Filters = new EntryFilterCollection();
				this.Filter += this.FilterPredicate;
			}

			protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args) {
				if(this.SourceEntry.SynchronizeInvoke.InvokeRequired) {
					this.SourceEntry.SynchronizeInvoke.BeginInvoke(new Action<NotifyCollectionChangedEventArgs>(this.InvokeOnCollectionChanged), new object[] { args });
				} else {
					base.OnCollectionChanged(args);
				}
			}

			private void InvokeOnCollectionChanged(NotifyCollectionChangedEventArgs e) {
				base.OnCollectionChanged(e);
			}

			private bool FilterPredicate(object obj) {
				if(this.Filters.Count > 0) {
					return true;
				} else {
					var entry = (SystemEntryViewModel)obj;
					return this.Filters.Filter(entry);
				}
			}

			public IEnumerable<ColumnOrderSet> ColumnOrder {
				get {
					return this._ColumnOrder;
				}
				set {
					this._ColumnOrder = value;

					this.CustomSort = new SystemEntryViewModelComparer(value);
				}
			}
		}

		#endregion

		#region Watcher

		private bool IsWatcherEnabled {
			get {
				return this._Watcher != null ? this._Watcher.IsEnabled : false;
			}
			set {
				if(this._Watcher != null) {
					this._Watcher.IsEnabled = value;
				}
			}
		}

		private void _Watcher_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			var children = this._Children.Value;
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add: {
						foreach(var item in e.NewItems.Cast<ISystemEntry>()) {
							children.Add(GetViewModel(this, item));
						}
						break;
					}
				case NotifyCollectionChangedAction.Remove: {
						foreach(var item in e.OldItems.Cast<ISystemEntry>()) {
							children.RemoveByName(item.Name);
						}
						break;
					}
				case NotifyCollectionChangedAction.Replace: {
						for(var i = 0; i < e.OldItems.Count; i++) {
							var oldItem = e.OldItems[i] as ISystemEntry;
							var newItem = e.NewItems[i] as ISystemEntry;

							var idx = children.IndexOf(oldItem);
							if(idx >= 0) {
								children[idx] = GetViewModel(this, newItem);
							}
						}
						break;
					}
				case NotifyCollectionChangedAction.Reset: {
						children.Clear();
						foreach(var item in e.NewItems.Cast<ISystemEntry>()) {
							children.Add(GetViewModel(this, item));
						}
						break;
					}
			}
		}

		#endregion
	}
}
