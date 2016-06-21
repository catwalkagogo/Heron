using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public class SystemEntryViewModelComparer : IComparer<SystemEntryViewModel>, System.Collections.IComparer{
		public ColumnOrderDefinition OrderDefinition { get; private set; }
		public CancellationToken CancellationToken { get; set; }

		public SystemEntryViewModelComparer(ColumnOrderDefinition order) {
			this.OrderDefinition = order;
		}

		public int Compare(SystemEntryViewModel x, SystemEntryViewModel y) {
			foreach(var set in this.OrderDefinition.Orders) {
				var xColumn = x.Columns[set.Column.Name];
				var yColumn = y.Columns[set.Column.Name];

				if(!xColumn.IsValueCreated) {
					xColumn.Refresh(this.CancellationToken);
				}
				if(!yColumn.IsValueCreated) {
					yColumn.Refresh(this.CancellationToken);
				}

				var d = xColumn.CompareTo(yColumn);
				if(set.Direction == ListSortDirection.Descending) {
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
