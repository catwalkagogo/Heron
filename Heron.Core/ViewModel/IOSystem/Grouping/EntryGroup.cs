using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public abstract class EntryGroup<T> : IGroup, IComparable<EntryGroup<T>>, IEntryFilter, IComparable where T : IComparable<T> {
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
			if (obj != null) {
				var grp = obj as EntryGroup<T>;
				if (grp != null) {
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

}
