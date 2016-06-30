using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CatWalk.Heron.IOSystem {
	public class ResetLazyColumnValueSource<T> : IColumnValueSource<T> {
		private ResetLazy<T> _Source;

		public ResetLazyColumnValueSource(Func<T> valueFactory) {
			this._Source = new ResetLazy<T>(valueFactory);
		}

		public void Reset() {
			this._Source.Reset();
		}

		public T GetValue(CancellationToken token) {
			return this._Source.Value;
		}
	}
}
