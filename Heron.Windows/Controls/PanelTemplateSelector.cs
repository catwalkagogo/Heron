using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CatWalk.Heron.ViewModel.Windows;
using CatWalk.IOSystem;

namespace CatWalk.Heron.Windows.Controls {
	using ViewFactory = Factory<ISystemEntry, DataTemplate>;

	public class PanelTemplateSelector : DataTemplateSelector{
		public ViewFactory ViewFactory { get; private set; } = new ViewFactory();

		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			// 現在のISystemItemに応じてViewModelを作成する
			var panelVM = (PanelViewModel)item;
			var entry = panelVM.ListView.CurrentEntry.Entry;

			var view = this.ViewFactory.Create(entry);

			var itemsControl = (ItemsControl)container;
			return view ?? itemsControl.ItemTemplate;
		}
	}
}
