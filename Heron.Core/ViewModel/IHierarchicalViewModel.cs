using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron.ViewModel {
	public interface IHierarchicalViewModel<T> {
		T Parent { get; }
		IEnumerable<T> Children { get; }
	}
}
