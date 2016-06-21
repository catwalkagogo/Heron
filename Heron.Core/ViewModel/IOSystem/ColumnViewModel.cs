using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk;
using CatWalk.Mvvm;
using CatWalk.Heron.IOSystem;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public class ColumnViewModel : ViewModelBase, IComparable<ColumnViewModel> {
		public SystemEntryViewModel SystemEntryViewModel { get; private set; }
		public IColumnDefinition Definition { get; private set; }
		private object _Value;
		private IEnumerable<IEntryGroup> _Groups;

		public ColumnViewModel(IColumnDefinition definition, SystemEntryViewModel vm) {
			definition.ThrowIfNull("provider");
			vm.ThrowIfNull("vm");
			this.Definition = definition;
			this.SystemEntryViewModel = vm;
		}

		public void Refresh() {
			this.Refresh(CancellationToken.None);
		}

		public void Refresh(CancellationToken token) {
			this.GetValue(true, token);
		}

		private void GetValue(bool refresh, CancellationToken token) {
			token.ThrowIfCancellationRequested();
			this._Value = this.Definition.GetValue(this.SystemEntryViewModel.Entry, refresh, token);
			this.IsValueCreated = true;
			this.OnPropertyChanged("Value");
		}

		public object Value {
			get{
				if(!this.IsValueCreated) {
					Task.Run(delegate {
						this.GetValue(false, this.SystemEntryViewModel.CancellationToken);
					});
				}
				return this._Value;
			}
		}

		public bool IsValueCreated {
			get;
			private set;
		}

		public IEnumerable<IEntryGroup> Groups {
			get {
				return this._Groups;
			}
			set {
				this._Groups = value;
				this.OnPropertyChanged("Groups");
			}
		}

		#region IComparable<ColumnViewModel>

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


		#endregion
	}
}
