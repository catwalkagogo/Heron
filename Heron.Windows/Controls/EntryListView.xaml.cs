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
using CatWalk.Collections;
using System.Reactive;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace CatWalk.Heron.Windows.Controls {
	using Interop;
	using System.Collections.Specialized;
	using System.Reactive.Disposables;
	using System.Windows.Controls.Primitives;
	/// <summary>
	/// Interaction logic for LogList.xaml
	/// </summary>
	public partial class EntryListView : UserControl {
		public const string DefaultItemTemplateKey = "DefaultItemTemplate";
		public const string DefaultViewKey = "DefaultView";
		public const string ChildrenViewKey = "ChildrenView";
		public const string ViewFactoryConverterKey = "ViewFactoryConverter";

		private CollectionViewSource _ChildrenView;
		private GridView _DefaultGridView;
		private CompositeDisposable _Disposables = new CompositeDisposable();

		public Selector Selector {
			get {
				return this._ListView;
			}
		}

		public IEnumerable<GridViewColumn> GridViewColumns {
			get { return (IEnumerable<GridViewColumn>)GetValue(GridViewColumnsProperty); }
			set { SetValue(GridViewColumnsProperty, value); }
		}

		// Using a DependencyProperty as the backing store for GridViewColumns.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty GridViewColumnsProperty =
			DependencyProperty.Register("GridViewColumns", typeof(IEnumerable<GridViewColumn>), typeof(EntryListView), new PropertyMetadata(null, (s, e) => {
				var listView = (EntryListView)s;
				var notify = (IEnumerable<GridViewColumn>)e.NewValue;
				notify.NotifyToCollection(listView._DefaultGridView.Columns);
			})); 

		public EntryListView() {
			InitializeComponent();

			this._ChildrenView = (CollectionViewSource)this.FindResource(ChildrenViewKey);

			this._DefaultGridView = (GridView)this.FindResource(DefaultViewKey);
		}

		private CompositeDisposable _FitColumnEvents = new CompositeDisposable();

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
			var view =  this.Factory.Create(new object[] { value, new Size<double>(16, 16) });
			return view;
		}
	}
}
