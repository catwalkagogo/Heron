using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk.IOSystem;

namespace CatWalk.Heron.Configuration.IOSystem {
	public class ConfigurationEntry : SystemEntry {
		public ConfigurationDirectory Configuration { get; private set; }

		public ConfigurationEntry(ConfigurationDirectory parent, string name) : base(parent, name) {
			this.Configuration = parent;
		}

		public T GetValue<T>() {
			return this.Configuration.Storage.Get(this.Name, default(T));
		}

		public Task<T> GetValueAsync<T>() {
			return this.Configuration.Storage.GetAsync(this.Name, default(T), CancellationToken.None);
		}

		public Task<T> GetValueAsync<T>(CancellationToken token) {
			return this.Configuration.Storage.GetAsync(this.Name, default(T), token);
		}

		public void SetValue<T>(T value) {
			this.Configuration.Storage[this.Name] = value;
		}

		public Task SetValueAsync<T>(T value) {
			return this.Configuration.Storage.SetAsync(this.Name, value, CancellationToken.None);
		}

		public Task SetValueAsync<T>(T value, CancellationToken token) {
			return this.Configuration.Storage.SetAsync(this.Name, value, token);
		}


		public override bool IsDirectory {
			get {
				return false;
			}
		}

		public override IEnumerable<ISystemEntry> GetChildren(CancellationToken token, IProgress<double> progress) {
			return new ConfigurationEntry[0];
		}
	}
}
