using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.Collections {
	public class WeakLinkedList<T> : ICollection<T>, ICollection where T : class{
		private LinkedList<WeakReference<T>> _List;

		public WeakLinkedList() {
			this._List = new LinkedList<WeakReference<T>>();
		}

		public WeakLinkedList(IEnumerable<T> collection) {
			collection.ThrowIfNull("collection");
			this._List = new LinkedList<WeakReference<T>>(collection.Select(v => new WeakReference<T>(v)));
		}

		private void Clean() {
			var node = this._List.First;
			while(node != null) {
				var r = node.Value;
				T v;
				if(!r.TryGetTarget(out v)) {
					this._List.Remove(node);
				}
				node = node.Next;
			}
		}

		public void AddLast(T item) {
			this._List.AddLast(new WeakReference<T>(item));
		}

		public void AddFirst(T item) {
			this._List.AddFirst(new WeakReference<T>(item));
		}

		void ICollection<T>.Add(T item) {
			this._List.AddLast(new WeakReference<T>(item));
		}

		public void Clear() {
			this._List.Clear();
		}

		public bool Contains(T item) {
			return this._List.Contains(new WeakReference<T>(item), new WeakReferenceComparer());
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
			this.ToArray().CopyTo(array, arrayIndex);
		}

		public int Count {
			get {
				this.Clean();
				return this._List.Count;
			}
		}

		bool ICollection<T>.IsReadOnly {
			get {
				return ((ICollection<T>)this._List).IsReadOnly;
			}
		}

		public bool Remove(T item) {
			var comparer = EqualityComparer<T>.Default;
			var node = this._List.First;
			while(node != null) {
				var r = node.Value;
				T v;
				if(!r.TryGetTarget(out v)) {
					this._List.Remove(node);
				} else if(comparer.Equals(v, item)) {
					return true;
				}
				node = node.Next;
			}
			return false;
		}

		public IEnumerator<T> GetEnumerator() {
			foreach(var r in this._List) {
				T v;
				if(r.TryGetTarget(out v)) {
					yield return v;
				}
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			foreach(var r in this._List) {
				T v;
				if(r.TryGetTarget(out v)) {
					yield return v;
				}
			}
		}

		private class WeakReferenceComparer : IEqualityComparer<WeakReference<T>> {
			private IEqualityComparer<T> _Comparer = EqualityComparer<T>.Default;

			public bool Equals(WeakReference<T> x, WeakReference<T> y) {
				T xt;
				T yt;
				if(x.TryGetTarget(out xt)) {
					if(y.TryGetTarget(out yt)) {
						return this._Comparer.Equals(xt, yt);
					}
				}
				return false;
			}

			public int GetHashCode(WeakReference<T> obj) {
				throw new NotImplementedException();
			}
		}

		void ICollection.CopyTo(Array array, int index) {
			this.ToArray().CopyTo(array, index);
		}

		public bool IsSynchronized {
			get { throw new NotImplementedException(); }
		}

		public object SyncRoot {
			get { throw new NotImplementedException(); }
		}
	}
}
