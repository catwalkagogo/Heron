using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.IOSystem;

namespace CatWalk.Heron.ViewModel.IOSystem {

	public class MonthlyGroupDescription<TColumn> : ColumnGroupDescription<TColumn, DateTime> where TColumn : IColumnDefinition<DateTime> {
		private static IDictionary<string, WeakReference<DateTimeGroup>> _Cache = new Dictionary<string, WeakReference<DateTimeGroup>>();

		protected override IEntryGroup GroupNameFromItem(SystemEntryViewModel entry, int level, System.Globalization.CultureInfo culture) {
			var dt = GetColumnValue(entry);
			var year = dt.Year;
			var month = dt.Month;
			var ym = DateTimeGroup.GetId(year, month);

			return _Cache.GetOrCreateWeakReference(ym, () => new DateTimeGroup(year, month), 32);
		}

		private class DateTimeGroup : EntryGroup<string> {
			public int Year {
				get;
				private set;
			}
			public int Month {
				get;
				private set;
			}

			public DateTimeGroup(int year, int month)
				: base(GetId(year, month), year + "-" + month) {
				this.Year = year;
				this.Month = month;
			}

			public static string GetId(int year, int month) {
				return year + "_" + month;
			}

			public override bool Filter(SystemEntryViewModel item) {
				var dt = GetColumnValue(item);
				return this.Year == dt.Year && this.Month == dt.Month;
			}
		}
	}
}
