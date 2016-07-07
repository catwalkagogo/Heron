using Reactive.Bindings;
using Reactive.Bindings.Extensions;
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
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.ComponentModel;
using System.Threading;
using CatWalk.Windows;

namespace CatWalk.Heron.Windows.Controls {
	/// <summary>
	/// Interaction logic for LogList.xaml
	/// </summary>
	public partial class Panel : UserControl {
		private CompositeDisposable _Disposables = new CompositeDisposable();

		public PanelList PanelList {
			get { return (PanelList)GetValue(PanelListProperty); }
			set { SetValue(PanelListProperty, value); }
		}

		// Using a DependencyProperty as the backing store for PanelList.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PanelListProperty =
			DependencyProperty.Register("PanelList", typeof(PanelList), typeof(Panel), new PropertyMetadata(null));



		public Panel() {
			InitializeComponent();

			this.Loaded += Panel_Loaded;
			this.Unloaded += Panel_Unloaded;
			this.GotFocus += Panel_GotFocus;
		}

		private void Panel_GotFocus(object sender, RoutedEventArgs e) {
			// フォーカスを受けたときにPanelListに通知する
			this.PanelList.Selector.SelectedItem = this.DataContext;
		}

		private void Panel_Unloaded(object sender, RoutedEventArgs e) {
			this._Disposables.Dispose();
		}

		private void Panel_Loaded(object sender, RoutedEventArgs e) {
			this.EntryListView.Selector.FocusSelector();

			var mainWindow = this.GetMainWindow();
			this.FocusNextPanelCommand = mainWindow.PanelList.Selector.Items
				.ObserveProperty(_ => _.Count)
				.Select(count => count > 1)
				.ToReactiveCommand();

			this._Disposables.Add(this.FocusNextPanelCommand);
			this._Disposables.Add(this.FocusNextPanelCommand.Subscribe(_ => {
				this.FocusNextPanel();
			}));
		}

		public ReactiveCommand FocusNextPanelCommand {
			get { return (ReactiveCommand)GetValue(FocusNextPanelCommandProperty); }
			set { SetValue(FocusNextPanelCommandProperty, value); }
		}

		// Using a DependencyProperty as the backing store for FocusNextPanelCommand.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty FocusNextPanelCommandProperty =
			DependencyProperty.Register("FocusNextPanelCommand", typeof(ReactiveCommand), typeof(Panel), new PropertyMetadata(null));

		public EntryListView EntryListView {
			get {
				return this._ListView;
			}
		}

		public void FocusNextPanel() {
			//var mainWindow = this.GetMainWindow();
			var current = this.PanelList.Selector.Items.IndexOf(this.DataContext);
			var next = (current + 1) % this.PanelList.Selector.Items.Count;
			//var nextItem = mainWindow.PanelList.Selector.Items[next];
			var container = (ListBoxItem)this.PanelList.Selector.ItemContainerGenerator.ContainerFromIndex(next);
			var nextPanel = (Panel)container.GetVisualChild(v => v is Panel);

			/*var focusedItem = nextPanel.EntryListView.Selector.Items
				.Cast<object>()
				.Select(item => nextPanel.EntryListView.Selector.ItemContainerGenerator.ContainerFromItem(item))
				.Cast<ListViewItem>()
				.FirstOrDefault(item => item.IsFocused);*/

			nextPanel.EntryListView.Selector.FocusSelector();

			/*Task.Delay(100).ContinueWith(t => {
				var element = FocusManager.GetFocusedElement(mainWindow);
				var focusable = element.Focusable;
			}, TaskScheduler.FromCurrentSynchronizationContext());*/
			//var nextPanel = ;
			//nextPanel.Focus();
		}
	}
}
