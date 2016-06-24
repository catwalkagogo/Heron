using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk.IOSystem;

namespace CatWalk.Heron.Configuration.IOSystem {
	public class ConfigurationDirectory : SystemEntry {
		public IStorage Storage { get; private set; }

		public ConfigurationDirectory(IStorage storage, ISystemEntry parent, string name) : base(parent, name) {
		}

		public override bool IsDirectory {
			get {
				return true;
			}
		}

		public override IEnumerable<ISystemEntry> GetChildren(CancellationToken token, IProgress<double> progress) {
			return this.Storage.Keys.Select(name => new ConfigurationEntry(this, name));
		}
	}
}
