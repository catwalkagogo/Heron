using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public class EntryFilterCollection : ICollection<ICombinationEntryFilter>, IEntryFilter {
		private LinkedList<ICombinationEntryFilter> _Collection;

		public EntryFilterCollection() {
			this._Collection = new LinkedList<ICombinationEntryFilter>();
		}

		public EntryFilterCollection(IEnumerable<ICombinationEntryFilter> list) {
			this._Collection = new LinkedList<ICombinationEntryFilter>(list);
		}

		public bool Filter(SystemEntryViewModel item) {
			var matched = true;
			foreach (var filter in this._Collection) {
				matched = filter.Filter(item, matched);
			}
			return matched;
		}

		#region ICollection<ICombinationEntryFilter> Members

		public void Add(ICombinationEntryFilter item) {
			this._Collection.AddLast(item);
		}

		public void Clear() {
			this._Collection.Clear();
		}

		public bool Contains(ICombinationEntryFilter item) {
			return this._Collection.Contains(item);
		}

		public void CopyTo(ICombinationEntryFilter[] array, int arrayIndex) {
			this._Collection.CopyTo(array, arrayIndex);
		}

		public int Count {
			get {
				return this._Collection.Count;
			}
		}

		public bool IsReadOnly {
			get {
				return ((ICollection<ICombinationEntryFilter>)this._Collection).IsReadOnly;
			}
		}

		public bool Remove(ICombinationEntryFilter item) {
			return this._Collection.Remove(item);
		}

		#endregion

		#region IEnumerable<ICombinationEntryFilter> Members

		public IEnumerator<ICombinationEntryFilter> GetEnumerator() {
			return this._Collection.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this._Collection.GetEnumerator();
		}

		#endregion
	}
}
