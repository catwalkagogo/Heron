using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IOSystem {
	public interface IWatchable {
		IIOSystemWatcher Watcher { get; }
	}
}
