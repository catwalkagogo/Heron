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
	public interface IObservableList<T> : IObservableCollection<T>, IList<T>, IList, IReadOnlyObservableList<T>{}

	/// <summary>
	/// 
	/// </summary>
	/// <remarks>追加順にアイテムを保持しないコレクションには使用不能</remarks>
	/// <typeparam name="T"></typeparam>
	public class WrappedObservableList<T> : WrappedObservableCollection<T>, IObservableList<T> {
		protected IList<T> Items{
			get{
				return (IList<T>)this.Collection;
			}
		}
		
		public WrappedObservableList() {
		}

		public WrappedObservableList(IList<T> list) : base(list) {
		}
		
		#region IList
		
		public virtual void Insert(int index, T item){
			this.CheckReentrancy();
			this.Items.Insert(index, item);
			this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
		}
		
		public override bool Remove(T item){
			var index = this.Items.IndexOf(item);
			if(index >= 0){
				this.CheckReentrancy();
				this.Items.RemoveAt(index);
				this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
				this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
				return true;
			}else{
				return false;
			}
		}

		public virtual void RemoveAt(int index){
			this.CheckReentrancy();
			T item = this.Items[index];
			this.Items.RemoveAt(index);
			this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
		}
		
		public T this[int index]{
			get{
				return this.Items[index];
			}
			set{
				T item = this.Items[index];
				this.Items[index] = value;
				this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, item, index));
			}
		}
		
		public virtual int IndexOf(T item){
			return this.Items.IndexOf(item);
		}
		
		#endregion

		#region IList

		object IList.this[int index]{
			get{
				return this[index];			
			}
			set {
				this[index] = (T)value;
			}
		}

		public bool IsFixedSize {
			get {
				return false;
			}
		}

		public void Remove(object item) {
			this.Remove((T) item);
		}

		public void Insert(int index, object item) {
			this.Insert(index, (T)item);
		}

		public int IndexOf(object item) {
			return this.Items.IndexOf((T)item);
		}

		public bool Contains(object item) {
			return this.Items.Contains((T)item);
		}

		public int Add(object item) {
			this.Items.Add((T)item);
			return this.Count - 1;
		}

		#endregion
	}
}