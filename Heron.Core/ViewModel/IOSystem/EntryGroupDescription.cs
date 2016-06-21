using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CatWalk.Heron.IOSystem;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public abstract class EntryGroupDescription : GroupDescription{
		public sealed override object GroupNameFromItem(object item, int level, System.Globalization.CultureInfo culture) {
			return this.GroupNameFromItem((SystemEntryViewModel)item, level, culture);
		}

		public virtual string ColumnName{
			get {
				return null;
			}
		}

		protected abstract IEntryGroup GroupNameFromItem(SystemEntryViewModel entry, int level, System.Globalization.CultureInfo culture);
	}

	public abstract class EntryGroup<T> : IEntryGroup, IComparable<EntryGroup<T>>, IEntryFilter, IComparable where T : IComparable<T> {
		public T Id { get; private set; }
		public string Name { get; private set; }

		public EntryGroup(T id, string name) {
			id.ThrowIfNull("id");
			name.ThrowIfNull("name");
			this.Id = id;
			this.Name = name;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public override bool Equals(object obj) {
			if(obj != null) {
				var grp = obj as EntryGroup<T>;
				if(grp != null) {
					return this.Id.Equals(grp.Id);
				}
			}
			return base.Equals(obj);
		}

		#region IComparable<EntryGroup<T>> Members

		public int CompareTo(EntryGroup<T> other) {
			return this.Id.CompareTo(other.Id);
		}

		#endregion

		#region IComparable Members

		public int CompareTo(object obj) {
			return this.CompareTo(obj as EntryGroup<T>);
		}

		#endregion

		#region IFilter<SystemEntryViewModel> Members

		public abstract bool Filter(SystemEntryViewModel item);

		#endregion
	}

	public interface IEntryGroup {
		string Name { get; }
	}

	public class DelegateEntryGroup<T> : EntryGroup<T> where T : IComparable<T> {
		private Predicate<SystemEntryViewModel> _Predicate;
		public DelegateEntryGroup(T id, string name, Predicate<SystemEntryViewModel> pred)
			: base(id, name) {
			pred.ThrowIfNull("pred");
			this._Predicate = pred;
		}

		public override bool Filter(SystemEntryViewModel item) {
			return this._Predicate(item);
		}
	}
}
