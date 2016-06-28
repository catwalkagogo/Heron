/*
	$Id: ObservableHashSet.cs 331 2014-01-14 16:05:48Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;

namespace CatWalk.Collections {
	public class ObservableHashSet<T> : ISet<T>, IObservableCollection<T>{
		private readonly HashSet<T> _Items;

		public ObservableHashSet(){
			this._Items = new HashSet<T>();
		}
		public ObservableHashSet(IEnumerable<T> list){
			this._Items = new HashSet<T>(list);
		}
		public ObservableHashSet(IEqualityComparer<T> comparer) {
			this._Items = new HashSet<T>(comparer);
		}
		public ObservableHashSet(IEnumerable<T> list, IEqualityComparer<T> comparer) {
			this._Items = new HashSet<T>(list, comparer);
		}

		#region ISerializable Members
		/*
		public void GetObjectData(SerializationInfo info, StreamingContext context){
			this._Items.GetObjectData(info, context);
		}
		*/
		#endregion
	
		#region Reentrancy
		
		private SimpleMonitor monitor = new SimpleMonitor();
		
		protected IDisposable BlockReentrancy(){
			this.monitor.Enter();
			return this.monitor;
		}
		
		protected void CheckReentrancy(){
			if((this.monitor.IsBusy && (this.CollectionChanged != null)) && (this.CollectionChanged.GetInvocationList().Length > 1)){
				throw new InvalidOperationException();
			}
		}
		
		#endregion

		#region IDeserializationCallback Members
		/*
		public void OnDeserialization(object sender){
			this._Items.OnDeserialization(sender);
		}
		*/
		#endregion

		#region ISet<T> Members

		public bool Add(T item){
			this.CheckReentrancy();
			if(this._Items.Add(item)){
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
				this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
				this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
				return true;
			}else{
				return false;
			}
		}

		public void ExceptWith(IEnumerable<T> other){
			this.CheckReentrancy();
			this._Items.ExceptWith(other);
			this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public void IntersectWith(IEnumerable<T> other){
			this.CheckReentrancy();
			this._Items.IntersectWith(other);
			this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public bool IsProperSubsetOf(IEnumerable<T> other){
			return this._Items.IsProperSubsetOf(other);
		}

		public bool IsProperSupersetOf(IEnumerable<T> other){
			return this._Items.IsProperSupersetOf(other);
		}

		public bool IsSubsetOf(IEnumerable<T> other){
			return this._Items.IsSubsetOf(other);
		}

		public bool IsSupersetOf(IEnumerable<T> other){
			return this._Items.IsSupersetOf(other);
		}

		public bool Overlaps(IEnumerable<T> other){
			return this._Items.Overlaps(other);
		}

		public bool SetEquals(IEnumerable<T> other){
			return this._Items.SetEquals(other);
		}

		public void SymmetricExceptWith(IEnumerable<T> other){
			this.CheckReentrancy();
			this._Items.SymmetricExceptWith(other);
			this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public void UnionWith(IEnumerable<T> other){
			this.CheckReentrancy();
			this._Items.UnionWith(other);
			this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		#endregion

		#region ICollection<T> Members

		void ICollection<T>.Add(T item){
			this.CheckReentrancy();
			this._Items.Add(item);
			this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
		}

		public void Clear(){
			this.CheckReentrancy();
			var items = this._Items.ToArray();
			this._Items.Clear();
			this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items));
		}

		public bool Contains(T item){
			return this._Items.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex){
			this._Items.CopyTo(array, arrayIndex);
		}

		public int Count{
			get{
				return this._Items.Count;
			}
		}

		public bool IsReadOnly{
			get{
				return false;
			}
		}

		public bool Remove(T item){
			this.CheckReentrancy();
			if(this._Items.Remove(item)){
				this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
				this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
				return true;
			}else{
				return false;
			}
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator(){
			return this._Items.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator(){
			return ((IEnumerable)this._Items).GetEnumerator();
		}

		#endregion

		#region INotifyCollectionChanged Members

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e){
			var eh = this.CollectionChanged;
			if(eh != null){
				using(this.BlockReentrancy()){
					eh(this, e);
				}
			}
		}

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e){
			var eh = this.PropertyChanged;
			if(eh != null){
				using(this.BlockReentrancy()){
					eh(this, e);
				}
			}
		}

		#endregion

		#region ICollection Members

		public void CopyTo(Array array, int index) {
			this.ToArray().CopyTo(array, index);
		}

		public bool IsSynchronized {
			get {
				return false;
			}
		}

		public object SyncRoot {
			get {
				return null;
			}
		}

		#endregion
	}
}
