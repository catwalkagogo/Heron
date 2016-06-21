using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CatWalk.ComponentModel {
	public interface ICancellable {
		void Cancel();
		bool IsCancellationRequested{get;}
		bool CanBeCanceled { get; }
		WaitHandle WaitHandle { get; }
	}
}
