using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.ViewModel.IOSystem;
using CatWalk.IOSystem;

namespace CatWalk.Heron.IOSystem.Generic {
	public class GenericSystemProvider : SystemProvider {


		public override IEnumerable<ISystemEntry> GetRootEntries(ISystemEntry parent) {
			throw new NotImplementedException();
		}

		public override object GetViewModel(object parent, SystemEntryViewModel entry, object previous) {
			throw new NotImplementedException();
		}

		public override ParsePathResult ParsePath(ISystemEntry root, string path) {
			throw new NotImplementedException();
		}
	}
}
