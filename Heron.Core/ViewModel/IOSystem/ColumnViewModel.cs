using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk;
using CatWalk.Mvvm;
using CatWalk.Heron.IOSystem;
using CatWalk.IOSystem;
using CatWalk.ComponentModel;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public class ColumnViewModel : ViewModelBase/*, IColumnDefinition, IComparer<ISystemEntry> */{
		public SystemEntryViewModel SystemEntryViewModel { get; private set; }
		public IColumnDefinition Definition { get; private set; }
		private object _Value;
		//private IEnumerable<IEntryGroup> _Groups;
		//private Lazy<IComparer<ISystemEntry>> _ColumnComparer;
		private Task _GetValueTask;
		private object _Sync = new object();
		private bool _IsShow = true;

		public ColumnViewModel(IColumnDefinition definition, SystemEntryViewModel vm) {
			definition.ThrowIfNull("provider");
			vm.ThrowIfNull("vm");
			this.Definition = definition;
			this.SystemEntryViewModel = vm;

			/*this._ColumnComparer = new Lazy<IComparer<ISystemEntry>>(() => {
				return this.Definition.GetComparer(ListSortDirection.Ascending);
			});*/
		}

		public void Refresh() {
			this.Refresh(CancellationToken.None);
		}

		public void Refresh(CancellationToken token) {
			this.GetValue(true, token);
		}

		public Task RefreshAsync(CancellationToken token) {
			return this.RunGetValueTask(token);
		}

		private void GetValue(bool refresh, CancellationToken token) {
			token.ThrowIfCancellationRequested();
			this._Value = this.Definition.GetValue(this.SystemEntryViewModel.Entry, refresh, token);
			this.IsValueCreated = true;
			this.OnPropertyChanged("Value");
		}

		#region IColumnDefinition

		public object GetValue(ISystemEntry entry) {
			return Definition.GetValue(entry);
		}

		public object GetValue(ISystemEntry entry, bool noCache) {
			return Definition.GetValue(entry, noCache);
		}

		public object GetValue(ISystemEntry entry, bool noCache, CancellationToken token) {
			return Definition.GetValue(entry, noCache, token);
		}

		public IComparer GetComparer(ListSortDirection order) {
			return Definition.GetComparer(order);
		}

		#endregion

		/*
public int Compare(ISystemEntry x, ISystemEntry y) {
	if (!this.CanSort) {
		throw new InvalidOperationException("This column is not able to sort.");
	}
	return this._ColumnComparer.Value.Compare(x, y);
}
*/
		private Task RunGetValueTask(CancellationToken token) {
			lock (this._Sync) {
				// タスクがすでに実行されてる場合はなにもしない
				if (this._GetValueTask != null && !this._GetValueTask.IsCompleted) {
					return this._GetValueTask;
				}

				return this._GetValueTask = Task.Run(delegate {
					this.GetValue(false, token);
				}, token);
			}
		}

		public object Value {
			get{
				if(!this.IsValueCreated) {
					this.RunGetValueTask(this.SystemEntryViewModel.CancellationToken);
				}
				return this._Value;
			}
		}

		public bool IsValueCreated {
			get;
			private set;
		}

		public string DisplayName {
			get {
				return Definition.DisplayName;
			}
		}

		public string Name {
			get {
				return Definition.Name;
			}
		}

		public bool CanSort {
			get {
				return Definition.CanSort;
			}
		}

		public bool IsShow {
			get {
				return this._IsShow;
			}
			set {
				this.IsShow = value;
				this.OnPropertyChanged(nameof(IsShow));
			}
		}
		/*
public IEnumerable<IEntryGroup> Groups {
	get {
		return this._Groups;
	}
	set {
		this._Groups = value;
		this.OnPropertyChanged("Groups");
	}
}
*/
		#region IComparable<ColumnViewModel>
		/*
		public int CompareTo(object obj) {
			var column = obj as ColumnViewModel;
			if(column != null){
				return this.CompareTo(column);
			}else if(this._Value == null){
				return obj != null ? Int32.MaxValue : 0;
			}else{
				var comparable = (IComparable)this._Value;
				if(comparable != null){
					return comparable.CompareTo(obj != null);
				}else{
					return Int32.MaxValue;
				}
			}
		}

		public int CompareTo(ColumnViewModel other) {
			if(this._Value == null) {
				return other != null ? -1 : 0;
			} else {
				var comparable = (IComparable)this._Value;
				if(comparable != null) {
					return comparable.CompareTo((other != null) ? other._Value : null);
				} else {
					return Int32.MaxValue;
				}
			}
		}

	*/
		#endregion
	}
}
