using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk;

namespace CatWalk.Heron.ViewModel.IOSystem {
	/*
	public class EntryFilterCollection : ICollection<ICombinationEntryFilter>, IEntryFilter {
		private LinkedList<ICombinationEntryFilter> _Collection;

		public EntryFilterCollection() {
			this._Collection = new LinkedList<ICombinationEntryFilter>();
		}

		public EntryFilterCollection(IEnumerable<ICombinationEntryFilter> list) {
			this._Collection = new LinkedList<ICombinationEntryFilter>(list);
		}

		public bool Filter(SystemEntryViewModel item) {
			var matched = true;
			foreach(var filter in this._Collection) {
				matched = filter.Filter(item, matched);
			}
			return matched;
		}

		#region ICollection<ICombinationEntryFilter> Members

		public void Add(ICombinationEntryFilter item) {
			this._Collection.AddLast(item);
		}

		public void Clear() {
			this._Collection.Clear();
		}

		public bool Contains(ICombinationEntryFilter item) {
			return this._Collection.Contains(item);
		}

		public void CopyTo(ICombinationEntryFilter[] array, int arrayIndex) {
			this._Collection.CopyTo(array, arrayIndex);
		}

		public int Count {
			get {
				return this._Collection.Count;
			}
		}

		public bool IsReadOnly {
			get {
				return ((ICollection<ICombinationEntryFilter>)this._Collection).IsReadOnly;
			}
		}

		public bool Remove(ICombinationEntryFilter item) {
			return this._Collection.Remove(item);
		}

		#endregion

		#region IEnumerable<ICombinationEntryFilter> Members

		public IEnumerator<ICombinationEntryFilter> GetEnumerator() {
			return this._Collection.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this._Collection.GetEnumerator();
		}

		#endregion
	}

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

	public sealed class DelegateEntryFilter : EntryFilter{
		private Predicate<SystemEntryViewModel> _Predicate;
		public DelegateEntryFilter(Predicate<SystemEntryViewModel> predicate)
			: this(predicate, FilterOperators.AND, false) {

		}

		public DelegateEntryFilter(Predicate<SystemEntryViewModel> predicate, FilterOperators op)
			: this(predicate, op, false) {

		}

		public DelegateEntryFilter(Predicate<SystemEntryViewModel> predicate, FilterOperators op, bool inverted) : base(op, inverted) {
			predicate.ThrowIfNull("predicate");
			this._Predicate = predicate;
		}

		protected override bool FilterEntry(SystemEntryViewModel entry) {
			return this._Predicate(entry);
		}
	}

	public sealed class CompositeEntryFilter : EntryFilter {
		private EntryFilterCollection _Includes = new EntryFilterCollection();
		private EntryFilterCollection _Excludes = new EntryFilterCollection();

		public EntryFilterCollection Includes {
			get {
				return this._Includes;
			}
		}

		public EntryFilterCollection Excludes {
			get {
				return this._Excludes;
			}
		}

		protected override bool FilterEntry(SystemEntryViewModel entry) {
			return this._Includes.Filter(entry) && !this._Excludes.Filter(entry);
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
	}*/
}
