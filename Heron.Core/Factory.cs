using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Collections;

namespace CatWalk.Heron {
	public class Factory {
		public const int PriorityNormal = 10;
		public const int PriorityLowest = Int32.MinValue;

		private SortedSkipList<Creator> _Creators = new SortedSkipList<Creator>();

		public void Register(Delegate predicate, Delegate create, int priority = PriorityNormal) {
			predicate.ThrowIfNull("predicate");
			create.ThrowIfNull("create");

			this._Creators.Add(new Creator(predicate, create, priority));
		}

		protected IEnumerable<Delegate> GetCreators(object[] args) {
			return this._Creators.Where(_ => (bool)_.Predicate.DynamicInvoke(args)).Select(_ => _.Create);
		}

		public object Create(object[] args) {
			return this.GetCreators(args).Select(c => c.DynamicInvoke(args)).FirstOrDefault();
		}

		public IEnumerable CreateAll(object[] args) {
			return this.GetCreators(args).Select(c => c.DynamicInvoke(args));
		}

		private struct Creator : IComparable<Creator>{
			public Delegate Predicate { get; private set; }
			public Delegate Create { get; private set; }
			public int Priority { get; private set; }

			public Creator(Delegate predicate, Delegate create, int priority) : this() {
				this.Create = create;
				this.Predicate = predicate;
				this.Priority = priority;
			}

			public int CompareTo(Creator other) {
				return Comparer<int>.Default.Compare(other.Priority, this.Priority);
			}
		}
	}

	public class Factory<TSource, TDest> : Factory {

		public void Register(Func<TSource, bool> predicate, Func<TSource, TDest> create, int priority = PriorityNormal) {
			base.Register(predicate, create, priority);
		}

		public TDest Create(TSource src) {
			return (TDest)this.Create(new object[] { src});
		}

		public IEnumerable<TDest> CreateAll(TSource src) {
			return this.CreateAll(new object[] { src }).Cast<TDest>();
		}
	}

	public class Factory<T1, T2, TDest> : Factory {

		public void Register(Func<T1, T2, bool> predicate, Func<T1, T2, TDest> create, int priority = PriorityNormal) {
			base.Register(predicate, create, priority);
		}

		public TDest Create(T1 p1, T2 p2) {
			return (TDest)this.Create(new object[] { p1, p2 });
		}

		public IEnumerable<TDest> CreateAll(T1 p1, T2 p2) {
			return this.CreateAll(new object[] { p1, p2 }).Cast<TDest>();
		}
	}

	public class Factory<T1, T2, T3, TDest> : Factory {

		public void Register(Func<T1, T2, T3, bool> predicate, Func<T1, T2, T3, TDest> create, int priority = PriorityNormal) {
			base.Register(predicate, create, priority);
		}

		public TDest Create(T1 p1, T2 p2, T3 p3) {
			return (TDest)this.Create(new object[] { p1, p2, p3 });
		}

		public IEnumerable<TDest> CreateAll(T1 p1, T2 p2, T3 p3) {
			return this.CreateAll(new object[] { p1, p2, p3 }).Cast<TDest>();
		}
	}

	public class Factory<T1, T2, T3, T4, TDest> : Factory {

		public void Register(Func<T1, T2, T3, T4, bool> predicate, Func<T1, T2, T3, T4, TDest> create, int priority = PriorityNormal) {
			base.Register(predicate, create, priority);
		}

		public TDest Create(T1 p1, T2 p2, T3 p3, T4 p4) {
			return (TDest)this.Create(new object[] { p1, p2, p3, p4 });
		}

		public IEnumerable<TDest> CreateAll(T1 p1, T2 p2, T3 p3, T4 p4) {
			return this.CreateAll(new object[] { p1, p2, p3, p4 }).Cast<TDest>();
		}
	}

}
