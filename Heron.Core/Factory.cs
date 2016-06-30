using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron {
	public class Factory<TSource, TDest> {
		private ICollection<Creator> _Creators = new List<Creator>();

		public void Register(Func<TSource, bool> predicate, Func<TSource, TDest> create) {
			predicate.ThrowIfNull("predicate");
			create.ThrowIfNull("create");

			this._Creators.Add(new Creator(predicate, create));
		}

		public TDest Create(TSource vm) {
			foreach(var creator in this._Creators){
				if (creator.Predicate(vm)) {
					var obj = creator.Create(vm);
					return obj;
				}
			}

			return default(TDest);
		}

		private struct Creator {
			public Func<TSource, bool> Predicate { get; private set; }
			public Func<TSource, TDest> Create { get; private set; }

			public Creator(Func<TSource, bool> predicate, Func<TSource, TDest> create) : this(){
				this.Create = create;
				this.Predicate = predicate;
			}
		}
	}
}
