using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CatWalk.Collections {
	internal class SimpleMonitor : IDisposable{
		private int _busyCount;

		public void Dispose(){
			Interlocked.Decrement(ref this._busyCount);
		}

		public void Enter(){
			Interlocked.Increment(ref this._busyCount);
		}

		// Properties
		public bool IsBusy{
			get{
				return (this._busyCount > 0);
			}
		}
	}
}
