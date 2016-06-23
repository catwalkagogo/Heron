using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.IOSystem;
using CatWalk.Collections;

namespace CatWalk.Heron.IOSystem {
	public abstract class OrderDefinition : IOrderDefinition {
		public abstract string DisplayName { get; }

		public abstract string Name { get; }

		public virtual IComparer<ISystemEntry> GetComparer(SortOrder order) {
			var comparer = this.GetAscendingComparer();
			if(order == SortOrder.Descending) {
				comparer = new ReversedComparer<ISystemEntry>(comparer);
			}
			return comparer;
		}

		protected abstract IComparer<ISystemEntry> GetAscendingComparer();

		public static OrderDefinition FromColumnDefinition(IColumnDefinition definition) {
			if (!definition.CanSort) {
				throw new ArgumentException("definition");
			}
			return new ColumnDefinitionOrderDefinition(definition);
		}

		private class ColumnDefinitionOrderDefinition : OrderDefinition {
			public IColumnDefinition ColumnDefinition { get; private set; }

			public ColumnDefinitionOrderDefinition(IColumnDefinition definition) {
				definition.ThrowIfNull("definition");

				this.ColumnDefinition = definition;
			}

			public override string DisplayName {
				get {
					return this.ColumnDefinition.DisplayName;
				}
			}

			public override string Name {
				get {
					return this.ColumnDefinition.Name;
				}
			}

			protected override IComparer<ISystemEntry> GetAscendingComparer() {
				return this.ColumnDefinition.GetComparer(SortOrder.Ascending);
			}
		}
	}
}
