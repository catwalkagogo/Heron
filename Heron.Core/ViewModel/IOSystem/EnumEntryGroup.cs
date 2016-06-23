using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.IOSystem;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public class EnumEntryGroup<TID, TValue> : EntryGroup<TID>
		where TValue : struct, IConvertible
		where TID : IComparable<TID>{
		static EnumEntryGroup() {
			if(!typeof(TValue).IsEnum) {
				throw new ArgumentException(typeof(TID).FullName + " is not an Enum type");
			}
		}

		private IColumnDefinition _Column;
		private TValue _Value;

		public EnumEntryGroup(IColumnDefinition column, TValue _value, TID id, string name)
			: base(id, name) {
			this._Column = column;
			this._Value = _value;
		}

		public override bool Filter(SystemEntryViewModel item) {
			return this._Value.Equals(item.Columns[this._Column].Value);
		}
	}
}
