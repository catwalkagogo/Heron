using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk;

namespace CatWalk.Heron.ViewModel.IOSystem {

	public interface IFilter<T> {
		bool Filter(T item);
	}

	public interface ICombinationFilter<T> : IFilter<T> {
		bool Filter(T item, bool matched);
		FilterOperators Operator { get; set; }
	}

	public interface IEntryFilter : IFilter<SystemEntryViewModel> {

	}

	public interface ICombinationEntryFilter : IEntryFilter, ICombinationFilter<SystemEntryViewModel> {

	}

	public abstract class EntryFilter : ICombinationEntryFilter {
		private FilterOperators _Operator;

		public EntryFilter() : this(FilterOperators.AND, false){
		}
		public EntryFilter(FilterOperators op) : this(op, false) {
		}
		public EntryFilter(FilterOperators op, bool inverted) {
			this.Operator = op;
			this.IsInverted = inverted;
		}

		public bool Filter(SystemEntryViewModel entry) {
			return this.IsInverted ? !this.FilterEntry(entry) : this.FilterEntry(entry);
		}

		protected abstract bool FilterEntry(SystemEntryViewModel entry);

		public virtual bool Filter(SystemEntryViewModel entry, bool matched) {
			var op = this.Operator;
			switch(op & ~FilterOperators.NOT) {
				case FilterOperators.AND:
					matched &= this.Filter(entry);
					break;
				case FilterOperators.OR:
					matched |= matched || this.Filter(entry);
					break;
				case FilterOperators.XOR:
					matched ^= this.Filter(entry);
					break;
				default:
					throw new InvalidOperationException();
			}
			return op.HasFlag(FilterOperators.NOT) ? !matched : matched;
		}
		public FilterOperators Operator {
			get {
				return this._Operator;
			}
			set {
				if(!IsValidFilterOperator(value)) {
					throw new ArgumentException("value");
				}
				this._Operator = value;
			}
		}
		public bool IsInverted { get; set; }

		protected static bool IsValidFilterOperator(FilterOperators op) {
			return op.HasFlag(FilterOperators.AND) ^ op.HasFlag(FilterOperators.OR) ^ op.HasFlag(FilterOperators.XOR);
		}
	}

	[Flags]
	public enum FilterOperators {
		None = 0,
		NOT = 1,
		AND = 2,
		OR = 4,
		XOR = 8,
		NAND = NOT | AND,
		NOR = NOT | OR,
		XNOR = NOT | XOR,
	}
}
