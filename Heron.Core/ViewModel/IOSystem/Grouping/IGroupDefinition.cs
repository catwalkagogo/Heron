using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.IOSystem;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public interface IGroupDefinition {
		IGroup GetGroupName(SystemEntryViewModel entry);
	}
}
