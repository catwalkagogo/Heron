using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.Collections {
	public class SkipListDictionary<TKey, TValue> : SortedSkipList<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue> {
		public SkipListDictionary()
			: this(Comparer<TKey>.Default) {
		}

		public SkipListDictionary(IComparer<TKey> comparer)
			: base(new KeyComparer(comparer), false) {
		}

		public void Add(TKey key, TValue value) {
			this.Add(new KeyValuePair<TKey, TValue>(key, value));
		}

		public bool Remove(TKey key) {
			return this.Remove(new KeyValuePair<TKey, TValue>(key, default(TValue)));
		}

		public bool ContainsKey(TKey key) {
			return this.Contains(new KeyValuePair<TKey, TValue>(key, default(TValue)));
		}

		public bool TryGetValue(TKey key, out TValue value) {
			int index = this.IndexOf(new KeyValuePair<TKey, TValue>(key, default(TValue)));
			if(index < 0) {
				value = default(TValue);
				return false;
			} else {
				value = this[index].Value;
				return true;
			}
		}

		public TValue this[TKey key] {
			get {
				var index = this.IndexOf(new KeyValuePair<TKey, TValue>());
				if(index < 0) {
					throw new KeyNotFoundException();
				} else {
					return this.GetNodeAt(index).Value.Value;
				}
			}
			set {
				var index = this.IndexOf(new KeyValuePair<TKey, TValue>());
				if(index < 0) {
					throw new KeyNotFoundException();
				} else {
					this.GetNodeAt(index).Value = new KeyValuePair<TKey, TValue>(key, value);
				}
			}
		}

		public ICollection<TKey> Keys {
			get {
				var list = new List<TKey>();
				foreach(var node in this.Nodes) {
					list.Add(node.Value.Key);
				}
				return list;
			}
		}

		public ICollection<TValue> Values {
			get {
				var list = new List<TValue>();
				foreach(var node in this.Nodes) {
					list.Add(node.Value.Value);
				}
				return list;
			}
		}

		private class KeyComparer : IComparer<KeyValuePair<TKey, TValue>> {
			private IComparer<TKey> comparer;

			public KeyComparer(IComparer<TKey> comparer) {
				this.comparer = comparer;
			}

			public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y) {
				return this.comparer.Compare(x.Key, y.Key);
			}
		}
	}
}
