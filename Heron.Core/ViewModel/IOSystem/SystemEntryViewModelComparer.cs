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
	using ColumnOrderDefinition = IEnumerable<ColumnOrderSet>;

	public class SystemEntryViewModelComparer : IComparer<SystemEntryViewModel>, System.Collections.IComparer{
		public CancellationToken CancellationToken { get; set; }
		public ICollection<Tuple<ColumnOrderSet, IComparer>> _Orders;

		public SystemEntryViewModelComparer(ColumnOrderDefinition order) {
			order.ThrowIfNull("order");

			this._Orders = order.Select(set => Tuple.Create(set, set.Column.GetComparer(set.Direction))).ToArray();
		}

		public int Compare(SystemEntryViewModel x, SystemEntryViewModel y) {
			foreach(var order in this._Orders) {
				var set = order.Item1;
				var comparer = order.Item2;
				var column = set.Column;

				ColumnViewModel xColumn;
				if(!x.Columns.TryGetValue(column, out xColumn)) {
					throw new InvalidOperationException("column not found");
				}

				ColumnViewModel yColumn;
				if (!y.Columns.TryGetValue(column, out yColumn)) {
					throw new InvalidOperationException("column not found");
				}

				if (!xColumn.IsValueCreated) {
					xColumn.Refresh(this.CancellationToken);
				}
				if (!yColumn.IsValueCreated) {
					yColumn.Refresh(this.CancellationToken);
				}

				var d = comparer.Compare(xColumn.Value, yColumn.Value);
				if (set.Direction == ListSortDirection.Descending) {
					d = -d;
				}
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
