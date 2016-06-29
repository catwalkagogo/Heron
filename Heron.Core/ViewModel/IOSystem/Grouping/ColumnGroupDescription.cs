using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.IOSystem;
using CatWalk.IOSystem;

namespace CatWalk.Heron.ViewModel.IOSystem {
	
	public abstract class ColumnGroupDescription<TColumn> : IGroupDefinition where TColumn : IColumnDefinition {
		public static object GetColumnValue(SystemEntryViewModel vm) {
			vm.ThrowIfNull("vm");
			return vm.Columns[typeof(TColumn)].Value;
		}

		public abstract IGroup GetGroupName(SystemEntryViewModel entry);
	}

	public abstract class ColumnGroupDescription<TColumn, TValue> : ColumnGroupDescription<TColumn> where TColumn : IColumnDefinition<TValue> {
		public new static TValue GetColumnValue(SystemEntryViewModel vm) {
			vm.ThrowIfNull("vm");
			return (TValue)vm.Columns[typeof(TColumn)].Value;
		}
	}
}
