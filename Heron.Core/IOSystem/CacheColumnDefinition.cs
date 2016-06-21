using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk;

namespace CatWalk.Heron.IOSystem {
	public abstract class CacheColumnDefinition<TSource, TValue> : ColumnDefinition<TValue>{
		public IColumnValueSource<TSource> Source { get; private set; }
		public CacheColumnDefinition(IColumnValueSource<TSource> source) {
			source.ThrowIfNull("source");
			this.Source = source;
		}

		protected override object GetValueImpl(CatWalk.IOSystem.ISystemEntry entry, bool noCache, System.Threading.CancellationToken token) {
			if(noCache) {
				this.Source.Reset();
			}
			return this.SelectValue(this.Source.GetValue(token));
		}

		protected abstract TValue SelectValue(TSource value);
	}

	public interface IColumnValueSource<T> {
		void Reset();
		T GetValue(CancellationToken token);
	}

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
