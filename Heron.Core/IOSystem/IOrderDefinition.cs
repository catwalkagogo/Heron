using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.IOSystem;

namespace CatWalk.Heron.IOSystem {
	public interface IOrderDefinition {
		string Name { get; }
		string DisplayName { get; }
		IComparer<ISystemEntry> GetComparer(SortOrder order);
	}
}
