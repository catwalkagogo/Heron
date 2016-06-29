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
		bool IsLoaded { get; }
		int Priority{get;}
	}
}
