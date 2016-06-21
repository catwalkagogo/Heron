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
		private bool _Disposed = false;
		private readonly ResetLazy<ColumnDictionary> _Columns;
		public SystemEntryViewModel Parent { get; private set; }
		public ISystemEntry Entry { get; private set; }
		public ISystemProvider Provider { get; private set; }
		private readonly IDictionary<string, ISet<IEntryGroup>> _ChildrenGroups = new ObservableDictionary<string, ISet<IEntryGroup>>();

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
		}

		#endregion

		#region IDisposable
		public void Dispose() {
			this.Dispose(true);

			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing) {
			if(!this._Disposed) {
				if(this._CancellationTokenSource.IsValueCreated && !this._CancellationTokenSource.Value.IsCancellationRequested) {
					this._CancellationTokenSource.Value.Cancel();
				}
				this._Disposed = true;
			}
		}

		~SystemEntryViewModel() {
			this.Dispose(false);
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
		public string DisplayName {
			get {
				return this.Entry.DisplayName;
			}
		}
		public string DisplayPath {
			get {
				return this.Entry.DisplayPath;
			}
		}
		public bool IsDirectory {
			get {
				return this.Entry.IsDirectory;
			}
		}

		#endregion

		#region ColumnDictionary

		public class ColumnDictionary : IReadOnlyDictionary<string, ColumnViewModel> {
			private SystemEntryViewModel _this;
			private IDictionary<string, Tuple<IColumnDefinition, ColumnViewModel>> _columns = new Dictionary<string, Tuple<IColumnDefinition, ColumnViewModel>>();

			public ColumnDictionary(SystemEntryViewModel vm){
				this._this = vm;
				foreach(var definition in vm.ColumnDefinitions) {
					this._columns.Add(definition.Name, new Tuple<IColumnDefinition, ColumnViewModel>(definition, null));
				}
			}

			public int Count {
				get {
					return this._columns.Count;
				}
			}

			public IEnumerator<KeyValuePair<string, ColumnViewModel>> GetEnumerator() {
				this.CreateAll();
				return this._columns.Select(pair => new KeyValuePair<string, ColumnViewModel>(pair.Key, pair.Value.Item2)).GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				this.CreateAll();
				return this._columns.GetEnumerator();
			}

			public IEnumerable<string> Keys {
				get{
					return this._columns.Keys;
				}
			}

			public IEnumerable<ColumnViewModel> Values{
				get {
					this.CreateAll();
					return this._columns.Values.Select(v => v.Item2);
				}
			}

			public ColumnViewModel this[string column] {
				get{
					ColumnViewModel vm;
					if(this.TryGetValue(column, out vm)) {
						return vm;
					} else {
						throw new KeyNotFoundException();
					}
				}
			}

			public ColumnViewModel this[Type type] {
				get {
					ColumnViewModel vm;
					if(this.TryGetValue(type.FullName, out vm)) {
						return vm;
					} else {
						throw new KeyNotFoundException();
					}
				}
			}

			public bool TryGetValue(string column, out ColumnViewModel vm) {
				Tuple<IColumnDefinition, ColumnViewModel> v;
				if(this._columns.TryGetValue(column, out v)) {
					vm = v.Item2;
					if(vm == null) {
						vm = this.CreateViewModel(v.Item1);
						this._columns[column] = new Tuple<IColumnDefinition, ColumnViewModel>(v.Item1, vm);
					}
					return true;
				} else {
					vm = null;
					return false;
				}
			}

			public bool ContainsKey(string column) {
				return this._columns.ContainsKey(column);
			}

			private ColumnViewModel CreateViewModel(IColumnDefinition provider) {
				var vm = new ColumnViewModel(provider, _this);
				this._columns[provider.Name] = new Tuple<IColumnDefinition,ColumnViewModel>(provider, vm);
				vm.PropertyChanged += vm_PropertyChanged;
				return vm;
			}

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

			private void CreateAll(){
				foreach(var entry in this.Keys) {
					var v = this._columns[entry];
					if(v.Item2 == null) {
						this.CreateViewModel(v.Item1);
					}
				}
			}
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
