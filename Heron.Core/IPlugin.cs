using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron {
	public interface IPlugin {
		string DisplayName { get; }
		void Load(Application app);
		void Unload(Application app);
		bool CanUnload(Application app);
		PluginPriority Priority{get;}
	}

	public enum PluginPriority {
		Lowest = 0x01,
		Normal = 0x0a,
		Builtin = 0xff,
	}
}
