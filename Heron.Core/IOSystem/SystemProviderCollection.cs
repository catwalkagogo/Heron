using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk.Heron.ViewModel.IOSystem;
using CatWalk.IOSystem;

namespace CatWalk.Heron.IOSystem {
	public class SystemProviderCollection : ICollection<ISystemProvider>, ISystemProvider{
		private ICollection<ISystemProvider> _Collection = new LinkedList<ISystemProvider>();

		#region ISystemProvider

		public IEnumerable<IGroupDefinition> GetGroupings(ISystemEntry entry) {
			return this.Where(_ => _.CanGetGroupings(entry)).SelectMany(_ => _.GetGroupings(entry));
		}

		public IEnumerable<OrderDefinition> GetOrderDefinitions(SystemEntryViewModel entry) {
			return this.Where(_ => _.CanGetOrderDefinitions(entry)).SelectMany(_ => _.GetOrderDefinitions(entry));
		}

		public IEnumerable<ISystemEntry> GetRootEntries(ISystemEntry parent) {
			return this.SelectMany(_ => _.GetRootEntries(parent));
		}

		public object GetViewModel(object parent, SystemEntryViewModel entry, object previous) {
			return this.Where(_ => _.CanGetViewModel(parent, entry, previous)).FirstOrDefault(_ => _.CanGetViewModel(parent, entry, previous));
		}

		public ParsePathResult ParsePath(ISystemEntry root, string path) {
			return this.ParsePathInternal(root, path).Item1;
		}

		internal Tuple<ParsePathResult, ISystemProvider> ParsePathInternal(ISystemEntry root, string path) {
			root.ThrowIfNull("root");
			path.ThrowIfNull("path");

			foreach (var provider in this._Collection) {
				var result = provider.ParsePath(root, path);
				if (result.Success) {
					return Tuple.Create(result, (ISystemProvider)provider);
				}
			}

			// 汎用パース
			var fragments = path.Split(SystemEntry.DirectorySeperatorChar);
			var entry = root;
			foreach (var name in fragments) {
				entry = entry.GetChild(name);
				if (entry == null) {
					return Tuple.Create(new ParsePathResult(false, null, false), (ISystemProvider)null);
				}
			}
			return Tuple.Create(new ParsePathResult(true, entry, path.EndsWith(SystemEntry.DirectorySeperatorChar.ToString())), (ISystemProvider)this);
		}

		public virtual string DisplayName {
			get {
				return "Collection Provider";
			}
		}


		public virtual string Name {
			get {
				return "CollectionProvider";
			}
		}

		public bool CanGetColumnDefinitions(ISystemEntry entry) {
			return this._Collection.Any(_ => _.CanGetColumnDefinitions(entry));
		}

		/*public object CanGetEntryIcon(ISystemEntry entry, Size<int> size) {
			throw new NotImplementedException();
		}*/

		public bool CanGetGroupings(ISystemEntry entry) {
			return this._Collection.Any(_ => _.CanGetGroupings(entry));
		}

		public bool CanGetOrderDefinitions(SystemEntryViewModel entry) {
			return this._Collection.Any(_ => _.CanGetOrderDefinitions(entry));
		}

		public bool CanGetViewModel(object parent, SystemEntryViewModel entry, object previous) {
			return this._Collection.Any(_ => _.CanGetViewModel(parent, entry, previous));
		}

		public IEnumerable<IColumnDefinition> GetColumnDefinitions(ISystemEntry entry) {
			return this._Collection.Where(_ => _.CanGetColumnDefinitions(entry)).SelectMany(_ => _.GetColumnDefinitions(entry));
		}
		/*
		public object GetEntryIcon(ISystemEntry entry, Size<int> size, CancellationToken token) {
			throw new NotImplementedException();
		}
		*/
		#endregion


		#region ICollection

		public int Count {
			get {
				return _Collection.Count;
			}
		}


		public bool IsReadOnly {
			get {
				return _Collection.IsReadOnly;
			}
		}

		public void Add(ISystemProvider item) {
			_Collection.Add(item);
		}


		public void Clear() {
			_Collection.Clear();
		}

		public bool Contains(ISystemProvider item) {
			return _Collection.Contains(item);
		}

		public void CopyTo(ISystemProvider[] array, int arrayIndex) {
			_Collection.CopyTo(array, arrayIndex);
		}

		public IEnumerator<ISystemProvider> GetEnumerator() {
			return _Collection.GetEnumerator();
		}



		public bool Remove(ISystemProvider item) {
			return _Collection.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return _Collection.GetEnumerator();
		}

		#endregion
	}
}
