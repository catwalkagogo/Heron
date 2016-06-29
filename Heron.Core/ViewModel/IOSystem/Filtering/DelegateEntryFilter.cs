using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public sealed class DelegateEntryFilter : EntryFilter {
		private Predicate<SystemEntryViewModel> _Predicate;
		public DelegateEntryFilter(Predicate<SystemEntryViewModel> predicate)
			: this(predicate, FilterOperators.AND, false) {

		}

		public DelegateEntryFilter(Predicate<SystemEntryViewModel> predicate, FilterOperators op)
			: this(predicate, op, false) {

		}

		public DelegateEntryFilter(Predicate<SystemEntryViewModel> predicate, FilterOperators op, bool inverted) : base(op, inverted) {
			predicate.ThrowIfNull("predicate");
			this._Predicate = predicate;
		}

		protected override bool FilterEntry(SystemEntryViewModel entry) {
			return this._Predicate(entry);
		}
	}
}
