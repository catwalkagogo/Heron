using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron.ViewModel.Windows {
	public class PanelViewModel : ControlViewModel{
		private ControlViewModel _Content;
		public PanelViewModel(Application app) : base(app) {
		}

		public ControlViewModel Content {
			get {
				return this._Content;
			}
			set {
				var old = this._Content;
				if(old != null) {
					this.Children.Remove(old);
				}

				this._Content = value;
				this.OnPropertyChanged("Content");

				if(value != null) {
					this.Children.Add(value);
				}
			}
		}
	}
}
