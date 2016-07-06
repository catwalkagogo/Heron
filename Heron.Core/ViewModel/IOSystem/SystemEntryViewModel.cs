using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;
using CatWalk;
using CatWalk.Collections;
using CatWalk.Mvvm;
using CatWalk.IOSystem;
using CatWalk.Heron.IOSystem;

namespace CatWalk.Heron.ViewModel.IOSystem {
	using ComponentModel;
	using Windows;
	using ColumnDictionaryKey = IColumnDefinition;

	public partial class SystemEntryViewModel : ViewModelBase, IHierarchicalViewModel<SystemEntryViewModel>{
		private readonly ResetLazy<ColumnDictionary> _Columns;
		public SystemEntryViewModel Parent { get; private set; }
		public ISystemEntry Entry { get; private set; }
		public ISystemProvider Provider { get; private set; }
		//private readonly IDictionary<string, ISet<IEntryGroup>> _ChildrenGroups = new ObservableDictionary<string, ISet<IEntryGroup>>();

		#region Constructor

		public SystemEntryViewModel(SystemEntryViewModel parent, ISystemProvider provider, ISystemEntry entry) {
			entry.ThrowIfNull("entry");
			provider.ThrowIfNull("provider");
			/*if(!parent.IsDirectory) {
				throw new ArgumentException("parent");
			}*/
			this.Parent = parent;
			this.Entry = entry;
			this.Provider = provider;
			this._Columns = new ResetLazy<ColumnDictionary>(() => new ColumnDictionary(this));
			if(this.IsDirectory) {
				this.InitDirectory();
			}

			// 初期ソート
			this._Orders = Seq.Make(new OrderDefinitionDirectionSet(OrderDefinition.FromColumnDefinition(ColumnDefinition.NameColumn), ListSortDirection.Ascending));
		}

		#endregion

		#region Host

		private ISystemEntryViewModelHost _Host;
		public ISystemEntryViewModelHost Host{
			get{
				return this._Host;
			}
			internal set {
				this._Host = value;
				this.OnPropertyChanged("Host");
			}
		}

		#endregion

		#region Order

		private IEnumerable<OrderDefinitionDirectionSet> _Orders;

		public IEnumerable<OrderDefinitionDirectionSet> Orders {
			get {
				return this._Orders;
			}
			set {
				this._Orders = value;
				this.OnPropertyChanged("Orders");
				this.OnPropertyChanged("EntryComparer");
			}
		}

		public SystemEntryViewModelComparer EntryComparer {
			get {
				var comparer =
					(this.Orders != null) ? new SystemEntryViewModelComparer(this._Orders) :
					new SystemEntryViewModelComparer(
						Seq.Make(
							new OrderDefinitionDirectionSet(
								OrderDefinition.FromColumnDefinition(ColumnDefinition.NameColumn),
								ListSortDirection.Ascending)));

				return comparer;
			}
		}

		#endregion

		#region IDisposable

		protected override void Dispose(bool disposing) {
			if(!this.IsDisposed) {
				if(this._CancellationTokenSource.IsValueCreated && !this._CancellationTokenSource.Value.IsCancellationRequested) {
					this._CancellationTokenSource.Value.Cancel();
				}
			}
			base.Dispose(disposing);
		}

		#endregion

		#region Cancellation

		private ResetLazy<CancellationTokenSource> _CancellationTokenSource = new ResetLazy<CancellationTokenSource>(() => new CancellationTokenSource());
		public CancellationToken CancellationToken {
			get {
				return this._CancellationTokenSource.Value.Token;
			}
		}

		public void CancelTokenProcesses() {
			this._CancellationTokenSource.Value.Cancel();
			this._CancellationTokenSource.Reset();
			this.OnPropertyChanged("CancellationToken");
		}

		#endregion

		#region Properties
		public IEnumerable<IColumnDefinition> ColumnDefinitions {
			get {
				return this.Provider.GetColumnDefinitions(this.Entry);
			}
		}

		public ColumnDictionary Columns {
			get {
				return this._Columns.Value;
			}
		}

		public string Name {
			get {
				return this.Entry.Name;
			}
		}
		public string Path {
			get {
				return this.Entry.Path;
			}
		}
		public bool IsDirectory {
			get {
				return this.Entry.IsDirectory;
			}
		}

		#endregion

		#region ColumnDictionary

		public class ColumnDictionary : IReadOnlyDictionary<ColumnDictionaryKey, ColumnViewModel> {
			private SystemEntryViewModel _this;

			/// <summary>
			/// backing dictionary
			/// </summary>
			private IDictionary<Type, Lazy<ColumnViewModel>> _columns = new Dictionary<Type, Lazy<ColumnViewModel>>();

			public ColumnDictionary(SystemEntryViewModel vm){
				this._this = vm;
				foreach(var definition in vm.ColumnDefinitions) {
					this._columns.Add(definition.GetType(), new Lazy<ColumnViewModel>(() => {
						return this.CreateViewModel(definition);
					}));
				}
			}

			#region IReadOnlyDictionary<ColumnDictionaryKey, ColumnViewModel>

			public int Count {
				get {
					return this._columns.Count;
				}
			}

			public IEnumerator<KeyValuePair<ColumnDictionaryKey, ColumnViewModel>> GetEnumerator() {
				return this._columns.Select(pair => new KeyValuePair<ColumnDictionaryKey, ColumnViewModel>(pair.Value.Value.Definition, pair.Value.Value)).GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return this.GetEnumerator();
			}

			public IEnumerable<ColumnDictionaryKey> Keys {
				get{
					return this._columns.Select(pair => pair.Value.Value.Definition);
				}
			}

			public IEnumerable<ColumnViewModel> Values{
				get {
					return this._columns.Values.Select(v => v.Value);
				}
			}

			public ColumnViewModel this[ColumnDictionaryKey column] {
				get{
					ColumnViewModel vm;
					if(this.TryGetValue(column, out vm)) {
						return vm;
					} else {
						throw new KeyNotFoundException();
					}
				}
			}

			public bool TryGetValue(ColumnDictionaryKey column, out ColumnViewModel vm) {
				if(column == null) {
					vm = null;
					return false;
				}

				return this.TryGetValue(column.GetType(), out vm);
			}

			public bool ContainsKey(ColumnDictionaryKey column) {
				if(column == null) {
					return false;
				}
				return this._columns.ContainsKey(column.GetType());
			}

			#endregion

			private ColumnViewModel CreateViewModel(IColumnDefinition definition) {
				definition.ThrowIfNull("definition");

				var vm = new ColumnViewModel(definition, _this);
				return vm;
			}
			/*
			private void vm_PropertyChanged(object sender, PropertyChangedEventArgs e) {
				var column = (ColumnViewModel)sender;
				if(e.PropertyName == "Value") {
					var v = column.Value;
					var c = column.Definition.Name;
					var parent = _this.Parent;
					if(parent != null){
						var grps = _this.Parent._Groupings[c]
							.EmptyIfNull()
							.Select(grp => (IEntryGroup)grp.GroupNameFromItem(_this.Entry, 0, Thread.CurrentThread.CurrentUICulture))
							.Memoize();
						column.Groups = grps;
						lock(parent._ChildrenGroups) {
							var list = parent._ChildrenGroups[c] ?? new ObservableHashSet<IEntryGroup>();
							list.UnionWith(grps);
						}
					}
				}
			}
			*/

			#region IReadOnlyDictionary<Type, ColumnViewModel>

			public ColumnViewModel this[Type key] {
				get {
					return this._columns[key].Value;
				}
			}

			public bool ContainsKey(Type key) {
				return this._columns.ContainsKey(key);
			}

			public bool TryGetValue(Type key, out ColumnViewModel value) {
				Lazy<ColumnViewModel> v;
				if (this._columns.TryGetValue(key, out v)) {
					value = v.Value;
					return true;
				} else {
					value = null;
					return false;
				}
			}

			#endregion
		}

		#endregion

		#region Selected

		private bool _IsSelected = false;
		public bool IsSelected {
			get {
				return this._IsSelected;
			}
			set {
				this._IsSelected = value;
				this.OnPropertyChanged("IsSelected");
			}
		}

		#endregion

		#region Static

		private SystemEntryViewModel GetViewModel(SystemEntryViewModel parent, ISystemEntry child) {
			return new SystemEntryViewModel(parent, parent.Provider, child);
		}

		#endregion
	}
}
