using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public class EnumEntryGroup<TID, TValue> : EntryGroup<TID>
		where TValue : struct, IConvertible
		where TID : IComparable<TID>{
		static EnumEntryGroup() {
			if(!typeof(TValue).IsEnum) {
				throw new ArgumentException(typeof(TID).FullName + " is not an Enum type");
			}
		}

		private string _ColumnName;
		private TValue _Value;

		public EnumEntryGroup(string columnName, TValue _value, TID id, string name)
			: base(id, name) {
			this._ColumnName = columnName;
			this._Value = _value;
		}

		public override bool Filter(SystemEntryViewModel item) {
			return this._Value.Equals(item.Columns[this._ColumnName].Value);
		}
	}
}
