using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron.Configuration {
	public class PartialStorage : Storage {
		public string Prefix { get; private set; }
		public IStorage Storage { get; private set; }

		public PartialStorage(string prefix, IStorage storage) {
			prefix.ThrowIfNull("prefix");
			storage.ThrowIfNull("storage");

			this.Prefix = prefix;
			this.Storage = storage;
		}

		private string GetKey(string key) {
			return this.Prefix + key;
		}

		protected override void AddItem(string key, object value) {
			this.Storage.Add(this.GetKey(key), value);
		}

		protected override bool TryGetItem(string key, out object value) {
			return this.Storage.TryGetValue(this.GetKey(key), out value);
		}

		protected override ICollection<string> GetKeys() {
			return this.Storage.Keys;
		}

		protected override bool RemoveItem(string key) {
			return this.Storage.Remove(this.GetKey(key));
		}

		protected override void SetItem(string key, object value) {
			this.Storage[this.GetKey(key)] = value;
		}

		protected override object GetItem(string key) {
			return this.Storage[this.GetKey(key)];
		}

		protected override void ClearItems() {
			this.Storage.Clear();
		}

		protected override int GetCount() {
			return this.Storage.Count;
		}

		protected override void Dispose(bool disposing) {
			this.Storage.Dispose();
			base.Dispose(disposing);
		}
	}
}
