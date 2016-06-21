/*
	$Id: RefreshableLazy.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CatWalk.IOSystem {
	internal class RefreshableLazy<T> {
		private Lazy<T> _Lazy;
		private Func<T> _ValueFactory;
		private LazyThreadSafetyMode _Mode;

		public RefreshableLazy() : this(() => default(T), LazyThreadSafetyMode.None){}
		public RefreshableLazy(bool isThreadSafe) : this(() => default(T), (isThreadSafe) ? LazyThreadSafetyMode.ExecutionAndPublication : LazyThreadSafetyMode.None){}
		public RefreshableLazy(Func<T> valueFactory) : this(valueFactory, LazyThreadSafetyMode.None){}
		public RefreshableLazy(Func<T> valueFactory, bool isThreadSafe) : this(valueFactory, (isThreadSafe) ? LazyThreadSafetyMode.ExecutionAndPublication : LazyThreadSafetyMode.None){}
		public RefreshableLazy(Func<T> valueFactory, LazyThreadSafetyMode mode){
			if(valueFactory == null){
				throw new ArgumentNullException("valueFactory");
			}
			this._ValueFactory = valueFactory;
			this._Mode = mode;
			this.Refresh();
		}

		public T Value{
			get{
				return this._Lazy.Value;
			}
		}

		public bool IsValueCreated{
			get{
				return this._Lazy.IsValueCreated;
			}
		}

		public void Refresh(){
			this._Lazy = new Lazy<T>(this._ValueFactory, this._Mode);
		}
	}
}
