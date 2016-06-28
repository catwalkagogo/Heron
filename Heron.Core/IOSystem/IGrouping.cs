using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.IOSystem;

namespace CatWalk.Heron.IOSystem {
	public interface IGrouping {
		IGroupName GetGroupName(ISystemEntry entry);
	}

	public interface IGroupName {
		string Name { get; }
	}
}
