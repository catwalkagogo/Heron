using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.ViewModel.IOSystem;
using CatWalk.IOSystem;
using CatWalk.Collections;

namespace CatWalk.Heron.IOSystem {
	public class GenericSystemProvider : SystemProvider {
		public Factory<ISystemEntry, ISystemEntry> RootEntryFactory { get; private set; } = new Factory<ISystemEntry, ISystemEntry>();
		public Factory<object, SystemEntryViewModel, object, object> ViewModelFactory { get; private set; } = new Factory<object, SystemEntryViewModel, object, object>();
		public Factory<ISystemEntry, string, ParsePathResult> ParsePathFactory { get; private set; } = new Factory<ISystemEntry, string, ParsePathResult>();

		public override IEnumerable<ISystemEntry> GetRootEntries(ISystemEntry parent) {
			return this.RootEntryFactory.CreateAll(parent);
		}

		public override object GetViewModel(object parent, SystemEntryViewModel entry, object previous) {
			return this.ViewModelFactory.Create(parent, entry, previous);
		}

		public override ParsePathResult ParsePath(ISystemEntry root, string path) {
			return this.ParsePathFactory.Create(root, path) ?? new ParsePathResult(false, null, false);
		}
	}
}
