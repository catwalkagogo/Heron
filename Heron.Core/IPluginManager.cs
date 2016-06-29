using System;
using System.Collections.Generic;
using System.Reflection;

namespace CatWalk.Heron {
	public interface IPluginManager {
		IEnumerable<IPlugin> Plugins { get; }
	}
}