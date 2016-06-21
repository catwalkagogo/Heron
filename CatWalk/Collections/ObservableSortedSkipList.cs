using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CatWalk.Collections {
	public class ObservableSortedSkipList<T> : SortedSkipList<T>, IReadOnlyObservableList<T>{
		public ObservableSortedSkipList() : base(){}
		public ObservableSortedSkipList(bool isAllowDuplicates) : base(isAllowDuplicates){}
		public ObservableSortedSkipList(IComparer<T> comparer) : base(comparer){}
		public ObservableSortedSkipList(IComparer<T> comparer, bool isAllowDuplicates) : base(comparer, isAllowDuplicates){}
		public ObservableSortedSkipList(IEnumerable<T> source) : base(source){}
		public ObservableSortedSkipList(IEnumerable<T> source, bool isAllowDuplicates) : base(source, isAllowDuplicates){}
		public ObservableSortedSkipList(IEnumerable<T> source, IComparer<T> comparer) : base(source, comparer){}
		public ObservableSortedSkipList(IEnumerable<T> source, IComparer<T> comparer, bool isAllowDuplicates) : base(source, comparer, isAllowDuplicates){}

		#region Reentrancy
		
		private SimpleMonitor monitor = new SimpleMonitor();
		
		private IDisposable BlockReentrancy(){
			this.monitor.Enter();
			return this.monitor;
		}
		
		private void CheckReentrancy(){
			if((this.monitor.IsBusy && (this.CollectionChanged != null)) && (this.CollectionChanged.GetInvocationList().Length > 1)){
				throw new InvalidOperationException();
			}
		}
		
		#endregion

		protected override void BaseInsert(int index, T item) {
			this.CheckReentrancy();
			base.BaseInsert(index, item);
			this.OnPropertyChanged("Count");
			this.OnPropertyChanged("Item[]");
			this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
		}

		public override bool Remove(T item) {
			int index = this.IndexOf(item);
			if(index >= 0){
				this.CheckReentrancy();
				base.RemoveAt(index);
				this.OnPropertyChanged("Count");
				this.OnPropertyChanged("Item[]");
				this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
				return true;
			}else{
				return false;
			}
		}

		public override void RemoveAt(int index) {
			if(index < 0 || this.Count <= index){
				throw new ArgumentOutOfRangeException("index");
			}
			this.CheckReentrancy();
			var item = this[index];
			base.RemoveAt(index);
			this.OnPropertyChanged("Count");
			this.OnPropertyChanged("Item[]");
			this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
		}

		private void OnCollectionChanged(NotifyCollectionChangedAction action, T item, int index){
			var handler = this.CollectionChanged;
			if(handler != null){
				handler(this, new NotifyCollectionChangedEventArgs(action, item, index));
			}
		}
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		private void OnPropertyChanged(string name){
			var handler = this.PropertyChanged;
			if(handler != null){
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
