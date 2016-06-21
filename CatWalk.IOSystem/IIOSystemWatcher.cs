using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;

namespace CatWalk.IOSystem {
	public interface IIOSystemWatcher : INotifyCollectionChanged{
		bool IsEnabled { get; set; }
		ISystemEntry Target { get; }
	}
}
