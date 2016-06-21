/*
	$Id: ReadOnlyObservableList.cs 222 2011-06-23 07:17:01Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace CatWalk.Collections {
	public class WrappedReadOnlyObservableCollection<T> : IReadOnlyObservableCollection<T>{
		private IReadOnlyCollection<T> _Collection;
		protected IReadOnlyCollection<T> Collection{
			get{
				return this._Collection;
			}
		}

		public WrappedReadOnlyObservableCollection() : this(new ObservableCollection<T>()){
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="list">list must impliment INotifyCollectionChanged and INotifyPropertyChanged</param>
		public WrappedReadOnlyObservableCollection(IReadOnlyCollection<T> collection) {
			collection.ThrowIfNull("collection");
			var ncc = collection as INotifyCollectionChanged;
			if(ncc == null){
				throw new InvalidCastException();
			}
			var npc = collection as INotifyPropertyChanged;
			if(npc == null){
				throw new InvalidCastException();
			}

			ncc.CollectionChanged += ncc_CollectionChanged;
			npc.PropertyChanged += npc_PropertyChanged;

			this._Collection = collection;
		}
		
		#region INotifyCollectionChanged

		void ncc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			this.OnCollectionChanged(e);
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e){
			var handler = this.CollectionChanged;
			if(handler != null) {
				handler(this, e);
			}
		}
		
		#endregion
		
		#region INotifyPropertyChanged

		void npc_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			this.OnPropertyChanged(e);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
			var handler = this.PropertyChanged;
			if(handler != null) {
				handler(this, e);
			}
		}
		
		#endregion

		#region IReadOnlyCollection<T> Members

		public int Count {
			get {
				return this._Collection.Count;
			}
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator() {
			return this._Collection.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() {
			return this._Collection.GetEnumerator();
		}

		#endregion
	}
}
