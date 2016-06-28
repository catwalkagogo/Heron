/*
	$Id: ObservableDictionary.cs 327 2014-01-10 10:26:18Z catwalkagogo@gmail.com $
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
	public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged{
		private IDictionary<TKey, TValue> dictionary;
		
		public ObservableDictionary() : this(new Dictionary<TKey, TValue>()){
		}
		
		public ObservableDictionary(IDictionary<TKey, TValue> dic){
			this.dictionary = dic;
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
		
		#region IDictionary
		
		public void Add(KeyValuePair<TKey, TValue> item){
			this.CheckReentrancy();
			this.dictionary.Add(item);
			this.OnPropertyChanged("Count");
			this.OnPropertyChanged("Item[]");
			this.OnPropertyChanged("Keys");
			this.OnPropertyChanged("Values");
			this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
		}
		
		public void Add(TKey key, TValue value){
			this.Add(new KeyValuePair<TKey, TValue>(key, value));
		}
		
		public void Clear(){
			this.CheckReentrancy();
			var items = this.dictionary.ToArray();
			this.dictionary.Clear();
			this.OnPropertyChanged("Count");
			this.OnPropertyChanged("Item[]");
			this.OnPropertyChanged("Keys");
			this.OnPropertyChanged("Values");
			this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, items);
		}
		
		public bool Contains(KeyValuePair<TKey, TValue> item){
			return this.dictionary.Contains(item);
		}
		
		public bool ContainsKey(TKey key){
			return this.dictionary.ContainsKey(key);
		}
		
		public bool Remove(KeyValuePair<TKey, TValue> item){
			if(this.dictionary.Remove(item)){
				this.CheckReentrancy();
				this.OnPropertyChanged("Count");
				this.OnPropertyChanged("Item[]");
				this.OnPropertyChanged("Keys");
				this.OnPropertyChanged("Values");
				this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
				return true;
			}else{
				return false;
			}
		}
		
		public bool Remove(TKey key){
			if(this.dictionary.ContainsKey(key)){
				var old = new KeyValuePair<TKey, TValue>(key, this.dictionary[key]);
				this.dictionary.Remove(key);
				this.CheckReentrancy();
				this.OnPropertyChanged("Count");
				this.OnPropertyChanged("Item[]");
				this.OnPropertyChanged("Keys");
				this.OnPropertyChanged("Values");
				this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, old);
				return true;
			}else{
				return false;
			}
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return this.GetEnumerator();
		}
		
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator(){
			return this.dictionary.GetEnumerator();
		}
		
		public bool TryGetValue(TKey key, out TValue value){
			return this.dictionary.TryGetValue(key, out value);
		}
		
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index){
			this.dictionary.CopyTo(array, index);
		}
		
		public int Count{
			get{
				return this.dictionary.Count;
			}
		}
		
		public bool IsReadOnly{
			get{
				return this.dictionary.IsReadOnly;
			}
		}
		
		public TValue this[TKey key]{
			get{
				return this.dictionary[key];
			}
			set{
				this.CheckReentrancy();
				TValue old = this.dictionary[key];
				this.dictionary[key] = value;
				this.OnPropertyChanged("Item[]");
				this.OnPropertyChanged("Keys");
				this.OnPropertyChanged("Values");
				this.OnCollectionChanged(
					NotifyCollectionChangedAction.Replace,
					new KeyValuePair<TKey, TValue>(key, value),
					new KeyValuePair<TKey, TValue>(key, old));
			}
		}
		
		public ICollection<TKey> Keys{
			get{
				return this.dictionary.Keys;
			}
		}
		
		public ICollection<TValue> Values{
			get{
				return this.dictionary.Values;
			}
		}
		
		#endregion
		
		#region INotifyCollectionChanged
		
		private void OnCollectionChanged(NotifyCollectionChangedAction action, IList<KeyValuePair<TKey, TValue>> list){
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, list));
		}
		private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem){
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem));
		}
		
		private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> item){
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item));
		}
		
		private void OnCollectionChanged(NotifyCollectionChangedAction action){
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action));
		}
		
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
		
		private void OnPropertyChanged(string prop){
			if(this.PropertyChanged != null){
				this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e){
			if(this.PropertyChanged != null){
				this.PropertyChanged(this, e);
			}
		}
		
		#endregion
	}
}