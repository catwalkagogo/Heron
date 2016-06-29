using CatWalk.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public struct OrderDefinitionDirectionSet {
		public IOrderDefinition Ordering { get; private set; }
		public ListSortDirection Direction { get; private set; }

		public OrderDefinitionDirectionSet(IOrderDefinition ordering, ListSortDirection order) : this() {
			ordering.ThrowIfNull("ordering");

			this.Ordering = ordering;
			this.Direction = order;
		}
	}
}
