using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CatWalk.Threading {
	public class Timer : DisposableObject {
		public Timer(Action<Object> action, object state, int dueTime, int period) : this(action, state, dueTime, period, CancellationToken.None) {
		}

		public Timer(Action<Object> action, object state, int dueTime, int period, CancellationToken token) {
			action.ThrowIfNull("action");

			Task.Delay(dueTime, token).ContinueWith(t => {
				token.ThrowIfCancellationRequested();
				while (!this.IsDisposed) {
					Task.Run(() => {
						action(state);
					}, token).Wait();
					Task.Delay(period, token).Wait();
				}
			});
		}
	}
}
