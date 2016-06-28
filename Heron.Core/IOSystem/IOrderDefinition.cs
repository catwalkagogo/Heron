using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.IOSystem;
using CatWalk.ComponentModel;

namespace CatWalk.Heron.IOSystem {
	public interface IOrderDefinition {
		string Name { get; }
		string DisplayName { get; }
		IComparer GetComparer(ListSortDirection order);
	}
}
