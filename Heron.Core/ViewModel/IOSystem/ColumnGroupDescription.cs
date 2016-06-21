using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.IOSystem;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public abstract class ColumnGroupDescription<TColumn> : EntryGroupDescription where TColumn : IColumnDefinition {
		public override string ColumnName {
			get {
				return typeof(TColumn).FullName;
			}
		}

		public static object GetColumnValue(SystemEntryViewModel vm) {
			vm.ThrowIfNull("vm");
			return vm.Columns[typeof(TColumn).FullName].Value;
		}
	}

	public abstract class ColumnGroupDescription<TColumn, TValue> : ColumnGroupDescription<TColumn> where TColumn : IColumnDefinition<TValue> {
		public new static TValue GetColumnValue(SystemEntryViewModel vm) {
			vm.ThrowIfNull("vm");
			return (TValue)vm.Columns[typeof(TColumn).FullName].Value;
		}
	}
}
