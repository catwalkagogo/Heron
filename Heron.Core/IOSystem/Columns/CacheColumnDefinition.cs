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
}
