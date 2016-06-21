using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.ViewModel.Windows;
using CatWalk.Collections;

namespace CatWalk.Heron {
	public class WindowManager<T> : WrappedObservableCollection<T> where T : WindowViewModel {
		public WindowManager() : base(new LinkedList<T>()) {

		}
	}
}
