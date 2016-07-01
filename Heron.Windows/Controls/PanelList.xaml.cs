using CatWalk.Heron.ViewModel.IOSystem;
using CatWalk.Heron.ViewModel.Windows;
using CatWalk.IOSystem;
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
	public partial class PanelList : ListBox {
		public const string DefaultPanelTemplateKey = "DefaultPanelTemplate";
		public static Factory<SystemEntryViewModel, DataTemplate> ItemDataTemplateFactory = new Factory<SystemEntryViewModel, DataTemplate>();

		public PanelList() {
			InitializeComponent();

			this.ItemTemplateSelector = new PanelTemplateSelector();
		}

		private class PanelTemplateSelector : FactoryDataTemplateSelector<SystemEntryViewModel> {
			public PanelTemplateSelector() : base(ItemDataTemplateFactory, DefaultPanelTemplateKey) { }

			protected override SystemEntryViewModel ItemDataSelector(object item) {
				// 現在のISystemItemに応じてViewModelを作成する
				var panelVM = (PanelViewModel)item;
				var entry = panelVM.ListView.CurrentEntry;
				return entry;
			}
		}
	}
}
