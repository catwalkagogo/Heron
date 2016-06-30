using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron.ViewModel.Windows {
	public class PanelViewModel : ControlViewModel{
		private ListViewModel _ListView;
		public PanelViewModel(Application app) : base(app) {
		}

		public ListViewModel ListView {
			get {
				return this._ListView;
			}
			set {
				var old = this._ListView;
				if(old != null) {
					this.Children.Remove(old);
				}

				this._ListView = value;
				this.OnPropertyChanged("ListView");

				if(value != null) {
					this.Children.Add(value);
				}
			}
		}
	}
}
