using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CatWalk.Mvvm;

namespace CatWalk.Heron.Configuration {
	public abstract class Storage : ViewModelBase, IStorage{
		protected abstract void AddItem(string key, object value);
		protected abstract bool TryGetItem(string key, out object value);
		protected abstract ICollection<string> GetKeys();
		protected abstract bool RemoveItem(string key);
		protected abstract void SetItem(string key, object value);
		protected abstract object GetItem(string key);
		protected abstract void ClearItems();
		protected abstract int GetCount();

		protected virtual bool ContainsItem(string key) {
			object v;
			return this.TryGetItem(key, out v);
		}
		protected virtual ICollection<object> GetValues() {
			var list = new List<object>();
			foreach(var key in this.GetKeys()) {
				list.Add(this.GetItem(key));
			}
			return list.AsReadOnly();
		}

		public void Add(string key, object value) {
			this.AddItem(key, value);
			this.OnPropertyChanged(Binding.IndexerName, "Count", "Values", "Keys");
		}

		public bool ContainsKey(string key) {
			return this.ContainsItem(key);
		}

		public ICollection<string> Keys {
			get {
				return this.GetKeys();
			}
		}

		public bool Remove(string key) {
			var r = this.RemoveItem(key);
			this.OnPropertyChanged(Binding.IndexerName, "Count", "Values", "Keys");
			return r;
		}

		public bool TryGetValue(string key, out object value) {
			return this.TryGetItem(key, out value);
		}

		public ICollection<object> Values {
			get {
				return this.GetValues();
			}
		}

		public object this[string key] {
			get {
				return this.GetItem(key);
			}
			set {
				this.SetItem(key, value);
				this.OnPropertyChanged(Binding.IndexerName, "Values", "Keys");
			}
		}

		public void Add(KeyValuePair<string, object> item) {
			this.AddItem(item.Key, item.Value);
			this.OnPropertyChanged(Binding.IndexerName, "Count", "Values", "Keys");
		}

		public void Clear() {
			this.ClearItems();
			this.OnPropertyChanged(Binding.IndexerName, "Count", "Values", "Keys");
		}

		public bool Contains(KeyValuePair<string, object> item) {
			return this.ContainsItem(item.Key);
		}

		public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) {
			foreach(var v in this) {
				array[arrayIndex++] = v;
			}
		}

		public int Count {
			get {
				return this.GetCount();
			}
		}

		public bool IsReadOnly {
			get {
				return false;
			}
		}

		public bool Remove(KeyValuePair<string, object> item) {
			return this.Remove(item.Key);
		}

		public virtual IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
			foreach(var key in this.GetKeys()) {
				yield return new KeyValuePair<string, object>(key, this.GetItem(key));
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		public virtual T Get<T>(string key, T def) {
			object v;
			if(this.TryGetValue(key, out v)) {
				if(v is T) {
					return (T)v;
				}else {
					return def;
				}
			}else {
				return def;
			}
		}

		public virtual Task<T> GetAsync<T>(string key, T def) {
			return Task.Run<T>(() => {
				return this.Get(key, def);
			});
		}

		public virtual Task SetAsync<T>(string key, T value) {
			return Task.Run(() => {
				this[key] = value;
			});
		}


		#region IDisposable Members

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {

		}

		~Storage() {
			this.Dispose(false);
		}

		#endregion
	}
}
