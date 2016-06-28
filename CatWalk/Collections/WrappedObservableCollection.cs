/*
	$Id: ObservableList.cs 326 2014-01-09 10:15:01Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace CatWalk.Collections{
	public interface IObservableCollection<T> : ICollection<T>, IReadOnlyObservableCollection<T>, ICollection{
		new int Count { get; }
	}

	/// <summary>
	/// 
	/// </summary>
	/// <remarks>追加順にアイテムを保持しないコレクションには使用不能</remarks>
	/// <typeparam name="T"></typeparam>
	public class WrappedObservableCollection<T> : IObservableCollection<T>{
		protected ICollection<T> Collection{get; private set;}
		
		public WrappedObservableCollection(){
			this.Collection = new List<T>();
		}
		
		public WrappedObservableCollection(ICollection<T> list){
			list.ThrowIfNull("list");
			this.Collection = list;
		}
		
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
		
		#region ICollection
		
		public virtual void Add(T item){
			this.CheckReentrancy();
			var count = this.Collection.Count;
			this.Collection.Add(item);
			this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, count));
		}
		
		public virtual void AddRange(IEnumerable<T> items){
			this.CheckReentrancy();
			var count = this.Collection.Count;
			var itemArray = items.ToArray();
			foreach(var item in itemArray){
				this.Collection.Add(item);
			}
			this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemArray, count));
		}

		public virtual void Clear(){
			if(this.Count > 0){
				this.CheckReentrancy();
				var items = this.Collection.ToArray();
				this.Collection.Clear();
				this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
				this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items));
			}
		}
		
		public bool Contains(T item){
			return this.Collection.Contains(item);
		}
		
		public virtual bool Remove(T item){
			this.CheckReentrancy();
			if(this.Collection.Remove(item)){
				this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
				this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
				return true;
			}else{
				return false;
			}
		}

		IEnumerator IEnumerable.GetEnumerator(){
			return this.GetEnumerator();
		}
		
		public IEnumerator<T> GetEnumerator(){
			return this.Collection.GetEnumerator();
		}
		
		public void CopyTo(T[] array, int index){
			this.Collection.CopyTo(array, index);
		}
		
		public int Count{
			get{
				return this.Collection.Count;
			}
		}
		
		public bool IsReadOnly{
			get{
				return this.Collection.IsReadOnly;
			}
		}
		
		#endregion
		
		#region INotifyCollectionChanged
		
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
		
		#region INotifyPropertyChanged
		
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e){
			var eh = this.PropertyChanged;
			if(eh != null){
				eh(this, e);
			}
		}
		
		#endregion

		#region ICollection
		public virtual void CopyTo(Array array, int index) {
			this.ToArray().CopyTo(array, index);
		}

		public virtual bool IsSynchronized {
			get {
				return false;
			}
		}

		public virtual object SyncRoot {
			get {
				return null;
			}
		}
		#endregion

	}
}