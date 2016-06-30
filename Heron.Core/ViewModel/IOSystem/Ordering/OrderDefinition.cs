using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.IOSystem;
using CatWalk.Collections;
using CatWalk.ComponentModel;
using CatWalk.Heron.IOSystem;
using System.Threading;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public abstract class OrderDefinition : IOrderDefinition {
		public abstract string DisplayName { get; }

		public abstract string Name { get; }

		public abstract IComparer<SystemEntryViewModel> GetComparer(ListSortDirection order);
		
		public static OrderDefinition FromColumnDefinition(IColumnDefinition column) {
			column.ThrowIfNull("column");
			if (!column.CanSort) {
				throw new ArgumentException("column");
			}
			return new ColumnDefinitionOrderDefinition(column);
		}
		
		public class ColumnDefinitionOrderDefinition : OrderDefinition {
			public IColumnDefinition Column { get; private set; }

			public ColumnDefinitionOrderDefinition(IColumnDefinition column) {
				column.ThrowIfNull("column");

				this.Column = column;
			}

			public override string DisplayName {
				get {
					return this.Column.DisplayName;
				}
			}

			public override string Name {
				get {
					return this.Column.Name;
				}
			}

			public override IComparer<SystemEntryViewModel> GetComparer(ListSortDirection order) {
				return new ColumnComparer(this, order);
			}

			private class ColumnComparer : IComparer<SystemEntryViewModel> {
				private ColumnDefinitionOrderDefinition _Self;
				private IComparer _Comparer;

				public ColumnComparer(ColumnDefinitionOrderDefinition self, ListSortDirection direction) {
					this._Self = self;
					this._Comparer = this._Self.Column.GetComparer(direction);
				}

				public int Compare(SystemEntryViewModel x, SystemEntryViewModel y) {
					var column = this._Self.Column;

					ColumnViewModel xColumn;
					if (!x.Columns.TryGetValue(column, out xColumn)) {
						throw new InvalidOperationException("column not found");
					}

					ColumnViewModel yColumn;
					if (!y.Columns.TryGetValue(column, out yColumn)) {
						throw new InvalidOperationException("column not found");
					}

					// 値未取得のものは後ろに持っていく。
					if (!xColumn.IsValueCreated && !yColumn.IsValueCreated) {
						return 0;
					}
					if (!xColumn.IsValueCreated) {
						xColumn.RefreshAsync(xColumn.SystemEntryViewModel.CancellationToken);
						return Int32.MinValue;
					}
					if (!yColumn.IsValueCreated) {
						yColumn.RefreshAsync(yColumn.SystemEntryViewModel.CancellationToken);
						return Int32.MaxValue;
					}

					return this._Comparer.Compare(xColumn.Value, yColumn.Value);
				}
			}
		}
	}
}
