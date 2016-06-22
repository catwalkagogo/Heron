using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace CatWalk.Collections {
	public static class CollectionUtility {
		public static CollectionSynchronizer NotifyToCollection<T>(this IEnumerable<T> source, ICollection<T> dest) {
			return source.NotifyToCollection(dest, v => true, v => v);
		}
		public static CollectionSynchronizer NotifyToCollection<T>(this IEnumerable<T> source, ICollection<T> dest, Func<T, bool> predicate) {
			return source.NotifyToCollection(dest, predicate, v => v);
		}
		public static CollectionSynchronizer NotifyToCollection<TSource, TDest>(this IEnumerable<TSource> source, ICollection<TDest> dest, Func<TSource, TDest> selector) {
			return source.NotifyToCollection(dest, v => true, selector);
		}
		public static CollectionSynchronizer NotifyToCollection<TSource, TDest>(this IEnumerable<TSource> source, ICollection<TDest> dest, Func<TSource, bool> predicate, Func<TSource, TDest> selector) {
			return new CollectionSynchronizer(source, dest, v => predicate((TSource)v), v => selector((TSource)v));
		}

		public static CollectionSynchronizer NotifyToCollection(this IEnumerable source, IEnumerable dest) {
			return source.NotifyToCollection(dest, v => true, v => v);
		}
		public static CollectionSynchronizer NotifyToCollection(this IEnumerable source, IEnumerable dest, Func<object, bool> predicate) {
			return source.NotifyToCollection(dest, predicate, v => v);
		}
		public static CollectionSynchronizer NotifyToCollection(this IEnumerable source, IEnumerable dest, Func<object, object> selector) {
			return source.NotifyToCollection(dest, v => true, selector);
		}
		public static CollectionSynchronizer NotifyToCollection(this IEnumerable source, IEnumerable dest, Func<object, bool> predicate, Func<object, object> selector) {
			return new CollectionSynchronizer(source, dest, predicate, selector);
		}
	}

	public class CollectionSynchronizer : DisposableObject {
		private Func<object, bool> _Predicate;
		private Func<object, object> _Selector;

		public IEnumerable Source{get; private set;}
		public IEnumerable Dest{get; private set;}
		private INotifyCollectionChanged _SourceNCC;
		protected INotifyCollectionChanged SourceNotifyCollectionChanged {
			get {
				return this._SourceNCC;
			}
		}

		private Lazy<Action<object>> _DestAdd;
		private Lazy<Action<object>> _DestRemove;
		private Lazy<Action> _DestClear;

		public CollectionSynchronizer(IEnumerable source, IEnumerable dest, Func<object, bool> predicate, Func<object, object> selector) {
			source.ThrowIfNull("source");
			dest.ThrowIfNull("dest");
			predicate.ThrowIfNull("predicate");
			selector.ThrowIfNull("selector");

			var ncc = source as INotifyCollectionChanged;
			//if(ncc == null) {
			//	throw new ArgumentException("source collection does not implement INotifyCollectionChanged");
			//}

			this.Source = source;
			this.Dest = dest;
			this._SourceNCC = ncc;
			this._Predicate = predicate;
			this._Selector = selector;

			this._DestAdd = new Lazy<Action<object>>(() => {
				var lambda = CollectionExpressions.GetAddFunction(this.Dest.GetType());
				return new Action<object>((v) => {
					lambda(this.Dest, v);
				});
			});
			this._DestRemove = new Lazy<Action<object>>(() => {
				var lambda = CollectionExpressions.GetRemoveFunction(this.Dest.GetType());
				return new Action<object>((v) => {
					lambda(this.Dest, v);
				});
			});
			this._DestClear = new Lazy<Action>(() => {
				var lambda = CollectionExpressions.GetClearFunction(this.Dest.GetType());
				return new Action(() => {
					lambda(this.Dest);
				});
			});

			this.Reset();

			this.Start();
		}

		public Func<object, bool> Predicate {
			get {
				return this._Predicate;
			}
			set {
				value.ThrowIfNull("value");
				this._Predicate = value;
			}
		}
		public Func<object, object> Selector {
			get {
				return this._Selector;
			}
			set {
				value.ThrowIfNull("value");
				this._Selector = value;
			}
		}

		void ncc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			this.OnSourceCollectionChanged(e);
		}

		protected void OnSourceCollectionChanged(NotifyCollectionChangedEventArgs e) {
			this.Stop();
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add: {
						foreach(var item in this.FilterItems(e.NewItems)) {
							this._DestAdd.Value(item);
						}
						break;
					}
				case NotifyCollectionChangedAction.Remove: {
						foreach(var item in this.FilterItems(e.NewItems)) {
							this._DestRemove.Value(item);
						}
						break;
					}
				case NotifyCollectionChangedAction.Move:
				case NotifyCollectionChangedAction.Replace: {
						foreach(var item in this.FilterItems(e.NewItems)) {
							this._DestRemove.Value(item);
						}
						foreach(var item in this.FilterItems(e.NewItems)) {
							this._DestAdd.Value(item);
						}
						break;
					}
				case NotifyCollectionChangedAction.Reset: {
						this.Reset();
						break;
					}
			}
			this.Start();
		}

		private void Reset() {
			this._DestClear.Value();
			foreach(var item in this.FilterItems(this.Source)) {
				this._DestAdd.Value(item);
			}
		}

		private IEnumerable FilterItems(IEnumerable source) {
			return source.Cast<object>().Where(this.Predicate).Select(this.Selector);
		}

		public virtual void Start() {
			this.ThrowIfDisposed();
			if (this._SourceNCC != null) {
				this._SourceNCC.CollectionChanged += this.ncc_CollectionChanged;
			}
		}

		public virtual void Stop() {
			if (this._SourceNCC != null) {
				this._SourceNCC.CollectionChanged -= this.ncc_CollectionChanged;
			}
		}

		protected override void Dispose(bool disposing) {
			if(!this.IsDisposed) {
				this.Stop();
			}
			base.Dispose(disposing);
		}
	}
}
