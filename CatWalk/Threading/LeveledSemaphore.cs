using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CatWalk.Threading {
	public class LeveledSemaphore {
		private SemaphoreSlim[] _Levels;

		public LeveledSemaphore(int level) {
			level.ThrowIfOutOfRange(1, "level");
			this._Levels = new SemaphoreSlim[level];
			for(int i = 0; i < level; i++) {
				this._Levels[i] = new SemaphoreSlim(1, 1);
			}
		}

		public void Wait(int level) {
			level.ThrowIfOutOfRange(1, "level");
			for(var i = 0; i < level; i++) {
				this._Levels[i].Wait();
			}
		}

		public bool IsAvailable(int level) {
			level.ThrowIfOutOfRange(1, "level");
			for(var i = 0; i < level; i++) {
				if(this._Levels[i].CurrentCount == 0) {
					return false;
				}
			}
			return true;
		}
	}
}
