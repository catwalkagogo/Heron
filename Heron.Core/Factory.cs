using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron {
	public abstract class Factory<TDest> {
		private ICollection<Creator> _Creators = new List<Creator>();

		protected void RegisterInternal(Delegate predicate, Delegate create) {
			predicate.ThrowIfNull("predicate");
			create.ThrowIfNull("create");

			this._Creators.Add(new Creator(predicate, create));
		}

		protected IEnumerable<Delegate> GetCreatorsInternal(params object[] args) {
			return this._Creators.Where(_ => (bool)_.Predicate.DynamicInvoke(args)).Select(_ => _.Create);
		}

		protected TDest CreateInternal(params object[] args) {
			return this.GetCreatorsInternal(args).Select(c => (TDest)c.DynamicInvoke(args)).FirstOrDefault();
		}

		protected IEnumerable<TDest> CreateAllInternal(params object[] args) {
			return this.GetCreatorsInternal(args).Select(c => (TDest)c.DynamicInvoke(args));
		}

		private struct Creator {
			public Delegate Predicate { get; private set; }
			public Delegate Create { get; private set; }

			public Creator(Delegate predicate, Delegate create) : this() {
				this.Create = create;
				this.Predicate = predicate;
			}
		}
	}

	public class Factory<TSource, TDest> : Factory<TDest> {

		public void Register(Func<TSource, bool> predicate, Func<TSource, TDest> create) {
			base.RegisterInternal(predicate, create);
		}

		public TDest Create(TSource src) {
			return this.CreateInternal(src);
		}

		public IEnumerable<TDest> CreateAll(TSource src) {
			return this.CreateAllInternal(src);
		}
	}

	public class Factory<T1, T2, TDest> : Factory<TDest> {

		public void Register(Func<T1, T2, bool> predicate, Func<T1, T2, TDest> create) {
			base.RegisterInternal(predicate, create);
		}

		public TDest Create(T1 p1, T2 p2) {
			return this.CreateInternal(p1, p2);
		}

		public IEnumerable<TDest> CreateAll(T1 p1, T2 p2) {
			return this.CreateAllInternal(p1, p2);
		}
	}

	public class Factory<T1, T2, T3, TDest> : Factory<TDest> {

		public void Register(Func<T1, T2, T3, bool> predicate, Func<T1, T2, T3, TDest> create) {
			base.RegisterInternal(predicate, create);
		}

		public TDest Create(T1 p1, T2 p2, T3 p3) {
			return this.CreateInternal(p1, p2, p3);
		}

		public IEnumerable<TDest> CreateAll(T1 p1, T2 p2, T3 p3) {
			return this.CreateAllInternal(p1, p2, p3);
		}
	}

	public class Factory<T1, T2, T3, T4, TDest> : Factory<TDest> {

		public void Register(Func<T1, T2, T3, T4, bool> predicate, Func<T1, T2, T3, T4, TDest> create) {
			base.RegisterInternal(predicate, create);
		}

		public TDest Create(T1 p1, T2 p2, T3 p3, T4 p4) {
			return this.CreateInternal(p1, p2, p3, p4);
		}

		public IEnumerable<TDest> CreateAll(T1 p1, T2 p2, T3 p3, T4 p4) {
			return this.CreateAllInternal(p1, p2, p3, p4);
		}
	}

}
