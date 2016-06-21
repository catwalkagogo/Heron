using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CatWalk.Threading {
	public static class Extension {
		#region SemaphoreSlim

		public static bool IsAvailable(this SemaphoreSlim sem) {
			return sem.CurrentCount != 0;
		}

		#endregion
	}
}
