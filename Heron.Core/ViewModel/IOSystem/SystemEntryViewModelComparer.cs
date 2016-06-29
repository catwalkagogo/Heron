using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk.Heron.IOSystem;

namespace CatWalk.Heron.ViewModel.IOSystem {
	using ComponentModel;
	using ColumnOrderDefinition = IEnumerable<OrderDirectionSet>;

	public class SystemEntryViewModelComparer : IComparer<SystemEntryViewModel>, System.Collections.IComparer{
		//public CancellationToken CancellationToken { get; set; }
		public ICollection<IComparer<SystemEntryViewModel>> _Comparers;

		public SystemEntryViewModelComparer(ColumnOrderDefinition order) {
			order.ThrowIfNull("order");

			this._Comparers = order.Select(set => set.Ordering.GetComparer(set.Direction)).ToArray();
		}

		public int Compare(SystemEntryViewModel x, SystemEntryViewModel y) {
			foreach(var comparer in this._Comparers) {
				var d = comparer.Compare(x, y);

				if(d != 0) {
					return d;
				}
			}
			return 0;
		}

		public int Compare(object x, object y) {
			return this.Compare((SystemEntryViewModel)x, (SystemEntryViewModel)y);
		}
	}
}
