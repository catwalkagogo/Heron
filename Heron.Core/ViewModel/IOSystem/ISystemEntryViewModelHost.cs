using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public interface ISystemEntryViewModelHost : INotifyPropertyChanged{
		SystemEntryViewModel CurrentEntry { get; }
	}
}
