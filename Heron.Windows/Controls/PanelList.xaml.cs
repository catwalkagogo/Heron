using CatWalk.Windows.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CatWalk.Heron.Windows.Controls {
	/// <summary>
	/// Interaction logic for PanelList.xaml
	/// </summary>
	public partial class PanelList : UserControl {
		public PanelList() {
			InitializeComponent();

			this.DataContextChanged += PanelList_DataContextChanged;
		}

		private void PanelList_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			var unko = e.NewValue;

			GridItemsPanel.SetIsEnabled(this._ListBox, true);
		}

		public PanelTemplateSelector PanelTemplateSelector {
			get {
				return this._PanelTemplateSelector;
			}
		}
	}
}
