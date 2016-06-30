using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CatWalk.Heron.IOSystem {

	public interface IColumnValueSource<T> {
		void Reset();
		T GetValue(CancellationToken token);
	}
}
