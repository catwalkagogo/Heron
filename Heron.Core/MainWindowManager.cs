using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.ViewModel.Windows;

namespace CatWalk.Heron {
	public class MainWindowManager : WindowManager<MainWindowViewModel> {
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);

			this.RefreshWindowIndexes();
		}

		protected void RefreshWindowIndexes() {
			var index = 0;
			foreach(var win in this) {
				win.Index = index++;
			}
		}
	}
}
