using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CatWalk.IOSystem {
	public interface IPartialSystemEntry {
		void GetNextChildren(int count);
		void GetNextChildren(int count, CancellationToken token);
		void GetPreviousChildren(int count);
		void GetPreviousChildren(int count, CancellationToken token);
	}
}
