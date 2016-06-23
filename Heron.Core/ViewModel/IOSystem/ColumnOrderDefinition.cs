using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CatWalk;
using CatWalk.Heron.IOSystem;

namespace CatWalk.Heron.ViewModel.IOSystem {
	/*public struct ColumnOrderDefinition {
		public IEnumerable<ColumnOrderSet> Orders { get; private set; }

		public ColumnOrderDefinition(IEnumerable<ColumnOrderSet> orders) : this(){
			orders.ThrowIfNull("orders");
			this.Orders = orders;
		}
	}
	*/
	public struct ColumnOrderSet {
		public IColumnDefinition Column { get; private set; }
		public ListSortDirection Direction { get; private set; }

		public ColumnOrderSet(IColumnDefinition column, ListSortDirection order) : this() {
			column.ThrowIfNull("column");

			this.Column = column;
			this.Direction = order;
		}
	}
}
