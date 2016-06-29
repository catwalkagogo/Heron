using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public sealed class CompositeEntryFilter : EntryFilter {
		private EntryFilterCollection _Includes = new EntryFilterCollection();
		private EntryFilterCollection _Excludes = new EntryFilterCollection();

		public EntryFilterCollection Includes {
			get {
				return this._Includes;
			}
		}

		public EntryFilterCollection Excludes {
			get {
				return this._Excludes;
			}
		}

		protected override bool FilterEntry(SystemEntryViewModel entry) {
			return this._Includes.Filter(entry) && !this._Excludes.Filter(entry);
		}
	}
}
