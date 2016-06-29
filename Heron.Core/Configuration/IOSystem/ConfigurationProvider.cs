using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.IOSystem;
using CatWalk.Heron.ViewModel.IOSystem;
using CatWalk.IOSystem;

namespace CatWalk.Heron.Configuration.IOSystem {
	public class ConfigurationProvider : SystemProvider {
		public override IEnumerable<ISystemEntry> GetRootEntries(ISystemEntry parent) {
			return Seq.Make(new ConfigurationDirectory(Application.Current.Configuration, parent, "Configuration"));
		}

		public override object GetViewModel(object parent, SystemEntryViewModel entry, object previous) {
			return null;
		}

		public override ParsePathResult ParsePath(ISystemEntry root, string path) {
			return new ParsePathResult(false, null, false);
		}
	}
}
