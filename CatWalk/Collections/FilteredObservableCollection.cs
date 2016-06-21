using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace CatWalk.Collections {
	public interface IReadOnlyObservableCollection<T> : IReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged{}

	public interface IReadOnlyObservableList<T> : IReadOnlyList<T>, IReadOnlyObservableCollection<T> {}

	public class FilteredObservableCollection<TSource, TValue> : DisposableObject, IReadOnlyObservableCollection<TValue>{
		private IEnumerable _Source;
		private ICollection<TValue> _Collection;
		private Func<TSource, bool> _Predicate;
		private Func<TSource, TValue> _Selector;
		private INotifyCollectionChanged _INotifyCollectionChanged;

		protected IEnumerable Source {
			get {
				return this._Source;
			}
		}

		protected ICollection<TValue> Collection {
			get {
				return this._Collection;
			}
		}

		public FilteredObservableCollection(IEnumerable source, Func<TSource, bool> filter, Func<TSource, TValue> selector)
			: this(source, filter, selector, new List<TValue>()){
		}

		public FilteredObservableCollection(IEnumerable source, Func<TSource, bool> filter, Func<TSource, TValue> selector, ICollection<TValue> collection) {
			source.ThrowIfNull("source");
			collection.ThrowIfNull("collection");
			filter.ThrowIfNull("filter");
			selector.ThrowIfNull("selector");

			var ncc = source as INotifyCollectionChanged;
			if(ncc == null){
				throw new ArgumentException("source collection does not implement INotifyCollectionChanged");
			}
			this._INotifyCollectionChanged = ncc;

			this._Source = source;
			this._Collection = collection;
			this._Predicate = filter;
			this._Selector = selector;

			ncc.CollectionChanged += _OnSourceCollectionChanged;
		}

		private void _OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			this.OnSourceCollectionChaged(e);
		}

		protected virtual void OnSourceCollectionChaged(NotifyCollectionChangedEventArgs e) {
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add: {
					foreach(var item in this.FilterItems(e.NewItems)) {
						var count = this._Collection.Count;
						this._Collection.Add(item);
						if(count != this._Collection.Count) {
							this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
								NotifyCollectionChangedAction.Add,
								item));
							this.OnPropertyChanged("Count");
						}
					}
					break;
				}
				case NotifyCollectionChangedAction.Remove: {
					foreach(var item in this.FilterItems(e.OldItems)) {
						if(this._Collection.Remove(item)) {
							this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
								NotifyCollectionChangedAction.Remove,
								item));
							this.OnPropertyChanged("Count");
						}
					}
					break;
				}
				case NotifyCollectionChangedAction.Move:
				case NotifyCollectionChangedAction.Replace: {
						foreach(var item in this.FilterItems(e.OldItems)) {
							if(this._Collection.Remove(item)) {
								this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
									NotifyCollectionChangedAction.Remove,
									item));
								this.OnPropertyChanged("Count");
							}
						}
						foreach(var item in this.FilterItems(e.NewItems)) {
							var count = this._Collection.Count;
							this._Collection.Add(item);
							if(count != this._Collection.Count) {
								this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
									NotifyCollectionChangedAction.Add,
									item));
								this.OnPropertyChanged("Count");
							}
						}
						break;
					}
				case NotifyCollectionChangedAction.Reset: {
						this._Collection.Clear();
						this.FilterItems(this._Source)
							.ForEach(this._Collection.Add);
						this.OnPropertyChanged("Count");
						break;
					}
			}
		}

		protected IEnumerable<TValue> FilterItems(System.Collections.IEnumerable source) {
			return source.Cast<TSource>().Where(this._Predicate).Select(this._Selector);
		}

		protected IEnumerable<TValue> FilterItems(IEnumerable<TSource> source) {
			return source.Where(this._Predicate).Select(this._Selector);
		}

		#region IReadOnlyCollection<TValue> Members

		public int Count {
			get {
				return this._Collection.Count;
			}
		}

		#endregion

		#region IEnumerable<TValue> Members

		public IEnumerator<TValue> GetEnumerator() {
			return this._Collection.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this._Collection.GetEnumerator();
		}

		#endregion

		#region INotifyCollectionChanged

		public event NotifyCollectionChangedEventHandler CollectionChanged;
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			var eh = this.CollectionChanged;
			if(eh != null) {
				eh(this, e);
			}
		}

		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
			var eh = this.PropertyChanged;
			if(eh != null) {
				eh(this, e);
			}
		}

		protected void OnPropertyChanged(string propName) {
			this.OnPropertyChanged(new PropertyChangedEventArgs(propName));
		}

		#endregion

		protected override void Dispose(bool disposing) {
			if(!this.IsDisposed) {
				this._INotifyCollectionChanged.CollectionChanged -= this._OnSourceCollectionChanged;
			}
			base.Dispose(disposing);
		}
	}

	public class FilteredObservableList<TSource, TValue> : FilteredObservableCollection<TSource, TValue>, IReadOnlyObservableList<TValue> {
		protected IList<TValue> List {
			get {
				return (IList<TValue>)this.Collection;
			}
		}

		public FilteredObservableList(IEnumerable source, Func<TSource, bool> filter, Func<TSource, TValue> selector)
			: this(source, filter, selector, new List<TValue>()){
		}

		public FilteredObservableList(IEnumerable source, Func<TSource, bool> filter, Func<TSource, TValue> selector, IList<TValue> collection)
			: base(source, filter, selector, collection) {

		}

		#region IReadOnlyList<TValue> Members

		public TValue this[int index] {
			get {
				return this.List[index];
			}
		}

		#endregion
	}
}
