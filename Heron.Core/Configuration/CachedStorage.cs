using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using CatWalk;

namespace CatWalk.Heron.Configuration {
	public class CachedStorage : Storage{
		private OrderedDictionary _Cache = new OrderedDictionary();
		private int _CacheSize;
		private IStorage _Storage;

		public int CacheSize {
			get {
				return this._CacheSize;
			}
			set {
				value.ThrowIfOutOfRange(1, "value");
				this.TrimCache();
			}
		}

		public CachedStorage(int cacheSize, IStorage storage) {
			cacheSize.ThrowIfOutOfRange(0, "cacheSize");
			storage.ThrowIfNull("storage");
			this._CacheSize = cacheSize;
			this._Storage = storage;
		}

		private void TrimCache() {
			this.ThrowIfDisposed();
			var count = this._Cache.Count - this._CacheSize;
			if(count > 0) {
				var keys = this._Cache.Keys.Cast<string>().Take(count).ToArray();
				foreach(var key in keys) {
					var v = this._Cache[key];
					this._Storage[key] = v;
					this._Cache.Remove(key);
				}
			}
		}

		protected void ClearCache() {
			this.ThrowIfDisposed();
			var keys = this._Cache.Keys.Cast<string>().ToArray();
			foreach(var key in keys) {
				var v = this._Cache[key];
				this._Storage[key] =  v;
			}
			this._Cache.Clear();
		}

		public void PreloadCache() {
			this.ThrowIfDisposed();
			foreach(var pair in this.GetItems(this._CacheSize)) {
				this._Cache.Add(pair.Key, pair.Value);
			}
		}

		protected virtual IEnumerable<KeyValuePair<string, object>> GetItems(int count) {
			this.ThrowIfDisposed();
			return this.Take(count);
		}

		protected override void AddItem(string key, object value) {
			this.ThrowIfDisposed();
			this._Cache.Add(key, value);
			this.TrimCache();
		}

		protected override bool TryGetItem(string key, out object value) {
			this.ThrowIfDisposed();
			if(this._Cache.Contains(key)) {
				value = this._Cache[key];
				return true;
			} else {
				if(this._Storage.TryGetValue(key, out value)) {
					this._Cache[key] = value;
					this.TrimCache();
					return true;
				} else {
					return false;
				}
			}
		}

		protected override bool RemoveItem(string key) {
			this.ThrowIfDisposed();
			this._Cache.Remove(key);
			return this._Storage.Remove(key);
		}

		protected override void SetItem(string key, object value) {
			this.ThrowIfDisposed();
			this._Cache[key] = value;
			this.TrimCache();
		}

		protected override object GetItem(string key) {
			this.ThrowIfDisposed();
			if(this._Cache.Contains(key)) {
				return this._Cache[key];
			} else {
				var v = this._Storage[key];
				this._Cache[key] = v;
				this.TrimCache();
				return v;
			}
		}

		protected override void ClearItems() {
			this.ThrowIfDisposed();
			this._Cache.Clear();
			this._Storage.Clear();
		}

		protected override ICollection<string> GetKeys() {
			this.ThrowIfDisposed();
			return this._Storage.Keys;
		}

		protected override ICollection<object> GetValues() {
			this.ThrowIfDisposed();
			return this._Storage.Values;
		}

		protected override int GetCount() {
			this.ThrowIfDisposed();
			return this._Storage.Count;
		}

		protected override bool ContainsItem(string key) {
			this.ThrowIfDisposed();
			if(this._Cache.Contains(key)) {
				return true;
			} else {
				return this._Storage.ContainsKey(key);
			}
		}

		private bool _IsDisposed = false;
		private void ThrowIfDisposed(){
			if (this._IsDisposed) {
				throw new ObjectDisposedException("this");
			}
		}

		protected override void Dispose(bool disposing) {
			if(!this._IsDisposed) {
				this.ClearCache();
				this._Storage.Dispose();
				this._IsDisposed = true;
			}
			base.Dispose(disposing);
		}
	}
}
