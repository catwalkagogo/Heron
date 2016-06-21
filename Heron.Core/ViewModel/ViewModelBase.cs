using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CatWalk;
using CatWalk.Mvvm;

namespace CatWalk.Heron.ViewModel {
	public class ViewModelBase : SynchronizeViewModel{
		public ViewModelBase() : base() {
		}

		public ViewModelBase(ISynchronizeInvoke invoker) : base(invoker) {
		}
	}
}
