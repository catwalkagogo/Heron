using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace CatWalk {
	public class AsyncLazy<T> : Lazy<Task<T>>{
		public AsyncLazy(Func<T> valueFactory) :
			base(() => Task.Factory.StartNew(valueFactory)) { }

		public AsyncLazy(Func<Task<T>> taskFactory) :
			base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap()) { }

		public TaskAwaiter<T> GetAwaiter() {
			return this.Value.GetAwaiter();
		}
	}
}
