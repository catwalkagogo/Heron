using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public class DelegateEntryGroup<T> : EntryGroup<T> where T : IComparable<T> {
		private Predicate<SystemEntryViewModel> _Predicate;
		public DelegateEntryGroup(T id, string name, Predicate<SystemEntryViewModel> pred)
			: base(id, name) {
			pred.ThrowIfNull("pred");
			this._Predicate = pred;
		}

		public override bool Filter(SystemEntryViewModel item) {
			return this._Predicate(item);
		}
	}
}
