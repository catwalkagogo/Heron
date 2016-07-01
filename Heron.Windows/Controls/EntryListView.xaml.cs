using CatWalk.Heron.ViewModel.IOSystem;
using CatWalk.Heron.Windows.Converters;
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
using System.Globalization;
using System.Windows.Interop;

namespace CatWalk.Heron.Windows.Controls {
	using Interop;
	using Win32 = CatWalk.Win32;

	/// <summary>
	/// Interaction logic for LogList.xaml
	/// </summary>
	public partial class EntryListView : ListView {
		public const string DefaultItemTemplateKey = "DefaultItemTemplate";
		public const string DefaultViewKey = "DefaultView";
		public const string ChildrenViewKey = "ChildrenView";
		public const string ViewFactoryConverterKey = "ViewFactoryConverter";

		private CollectionViewSource _ChildrenView;

		//public static Factory<SystemEntryViewModel, DataTemplate> ItemDataTemplateFactory { get; private set; } = new Factory<SystemEntryViewModel, DataTemplate>();

		public static Factory<SystemEntryViewModel, ViewBase> ViewFactory { get; private set; } = new Factory<SystemEntryViewModel, ViewBase>();
		public static Factory<RequireEntryImageParameter, ImageSource> EntryImageFactory { get; private set; } = new Factory<RequireEntryImageParameter, ImageSource>();

		static EntryListView() {
			EntryImageFactory.Register(p => true, p => {
				// Shellからデフォルトアイコンを使用
				if (p.Entry.IsDirectory) {
					return IconUtility.GetShellIcon(IconUtility.FolderIconIndex, Win32::IconSize.Small);
				} else {
					return IconUtility.GetShellIcon(IconUtility.FileIconIndex, Win32::IconSize.Small);
				}
			}, Factory.PriorityLowest);
		}

		public EntryListView() {
			InitializeComponent();

			this._ChildrenView = (CollectionViewSource)this.FindResource(ChildrenViewKey);

			//this.ItemTemplateSelector = new EntryItemTemplateSelector();
		}

		/*private class EntryItemTemplateSelector : FactoryDataTemplateSelector<SystemEntryViewModel> {
			public EntryItemTemplateSelector() : base(ItemDataTemplateFactory, DefaultItemTemplateKey) { }
		}*/

	}

	public class RequireEntryImageParameter{
		public SystemEntryViewModel Entry { get; private set; }

		public RequireEntryImageParameter(SystemEntryViewModel entry) {
			entry.ThrowIfNull(nameof(entry));

			this.Entry = entry;
		}
	}

	internal class EntryListViewViewFactoryConverter : FactoryConverter {
		public EntryListView ListView { get; set; }

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var view = base.Convert(value, targetType, parameter, culture);

			if (view == null) {
				view = this.ListView.FindResource(EntryListView.DefaultViewKey);
			}

			return view;
		}
	}

	internal class EntryListViewEntryImageFactoryConverter : FactoryConverter {
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var request = new RequireEntryImageParameter((SystemEntryViewModel)value);
			var view = base.Convert(request, targetType, parameter, culture);
			return view;
		}
	}
}
