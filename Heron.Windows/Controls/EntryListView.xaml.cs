using CatWalk.Heron.ViewModel.IOSystem;
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
	/// Interaction logic for LogList.xaml
	/// </summary>
	public partial class EntryListView : ListView {
		public static Factory<SystemEntryViewModel, DataTemplate> ItemDataTemplateFactory = new Factory<SystemEntryViewModel, DataTemplate>();

		public EntryListView() {
			InitializeComponent();

			this.ItemTemplateSelector = new EntryItemTemplateSelector();
		}

		private class EntryItemTemplateSelector : FactoryDataTemplateSelector<SystemEntryViewModel> {
			public EntryItemTemplateSelector() : base(ItemDataTemplateFactory, "DefaultItemTemplate") { }
		}
	}
}
