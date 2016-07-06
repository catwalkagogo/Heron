using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using System.Collections.Specialized;

namespace CatWalk.Windows.Extensions {
	public static class GridItemsPanel {
		private static void RefreshGrid(ItemsControl itemsControl) {
			if (GetIsEnabled(itemsControl)) {
				//itemsControl.ItemContainerStyle = new Style(typeof(GridItem));
				itemsControl.ItemsPanel = CreateItemsPanel(itemsControl);
			} else {
				//itemsControl.ItemContainerStyle = null;
				itemsControl.ItemsPanel = null;
			}
		}

		private static ItemsPanelTemplate CreateItemsPanel(ItemsControl itemsControl) {
			var gridFactory = new FrameworkElementFactory(typeof(Grid));
			gridFactory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler((s, e) => {
				var grid = (Grid)s;

				{
					var columnSource = GetColumnDefinitionsSource(itemsControl);
					if (columnSource != null) {
						foreach (var def in columnSource) {
							grid.ColumnDefinitions.Add(def);
						}
					} else {
						var columnCount = GetColumnCount(itemsControl);
						for (var i = 0; i < columnCount; i++) {
							grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
						}
					}
				}

				{
					var rowSource = GetRowDefinitionsSource(itemsControl);
					if (rowSource != null) {
						foreach (var def in rowSource) {
							grid.RowDefinitions.Add(def);
						}
					} else {
						var rowCount = GetRowCount(itemsControl);
						for (var i = 0; i < rowCount; i++) {
							grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
						}
					}
				}
			}), true);
			gridFactory.SetBinding(Grid.ShowGridLinesProperty, new Binding("ShowGridLines") { Source = itemsControl });
			return new ItemsPanelTemplate(gridFactory);
		}

		#region IsEnabled

		public static bool GetIsEnabled(ItemsControl obj) {
			return (bool)obj.GetValue(IsEnabledProperty);
		}

		public static void SetIsEnabled(ItemsControl obj, bool value) {
			obj.SetValue(IsEnabledProperty, value);
		}

		// Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.RegisterAttached(
				"IsEnabled",
				typeof(bool),
				typeof(GridItemsPanel),
				new UIPropertyMetadata(false, OnIsEnabledPropertyChanged));

		private static void OnIsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var itemsControl = (ItemsControl)d;

			var old = (bool)e.OldValue;
			var now = (bool)e.NewValue;
			if (old != now) {
				if (old) {
					SetRowCount(itemsControl, 0);
					SetColumnCount(itemsControl, 0);
					SetColumnDefinitionsSource(itemsControl, null);
					itemsControl.SetValue(StateObjectProperty, null);
				}
			}

			RefreshGrid(itemsControl);
		}

		#endregion

		#region RowCount

		public static readonly DependencyProperty RowCountProperty =
			DependencyProperty.RegisterAttached(
				"RowCount",
				typeof(int),
				typeof(GridItemsPanel),
				new PropertyMetadata(0, OnRowCountPropertyChanged));

		private static void OnRowCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RefreshGrid((ItemsControl)d);
		}

		public static void SetRowCount(ItemsControl itemsControl, int rowCount) {
			itemsControl.SetValue(RowCountProperty, rowCount);
		}

		public static int GetRowCount(ItemsControl itemsControl) {
			return (int)itemsControl.GetValue(RowCountProperty);
		}

		#endregion

		#region ColumnCount

		public static int GetColumnCount(ItemsControl obj) {
			return (int)obj.GetValue(ColumnCountProperty);
		}

		public static void SetColumnCount(ItemsControl obj, int value) {
			obj.SetValue(ColumnCountProperty, value);
		}

		public static readonly DependencyProperty ColumnCountProperty =
			DependencyProperty.RegisterAttached(
			"ColumnCount",
			typeof(int),
			typeof(GridItemsPanel),
			new PropertyMetadata(0, OnColumnCountPropertyChanged));

		private static void OnColumnCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RefreshGrid((ItemsControl)d);
		}

		#endregion

		#region ShowGridLines

		public static bool GetShowGridLines(ItemsControl obj) {
			return (bool)obj.GetValue(ShowGridLinesProperty);
		}

		public static void SetShowGridLines(ItemsControl obj, bool value) {
			obj.SetValue(ShowGridLinesProperty, value);
		}

		// Using a DependencyProperty as the backing store for ShowGridLines.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ShowGridLinesProperty =
			DependencyProperty.RegisterAttached(
				"ShowGridLines",
				typeof(bool),
				typeof(GridItemsPanel),
				new PropertyMetadata(false, OnShowGridLinesPropertyChanged));

		private static void OnShowGridLinesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RefreshGrid((ItemsControl)d);
		}

		#endregion

		#region ColumnDefinitionsSource

		public static IEnumerable<ColumnDefinition> GetColumnDefinitionsSource(ItemsControl obj) {
			return (IEnumerable<ColumnDefinition>)obj.GetValue(ColumnDefinitionsSourceProperty);
		}

		public static void SetColumnDefinitionsSource(ItemsControl obj, IEnumerable<ColumnDefinition> value) {
			obj.SetValue(ColumnDefinitionsSourceProperty, value);
		}

		// Using a DependencyProperty as the backing store for ColumnDefinitionsSource.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ColumnDefinitionsSourceProperty =
			DependencyProperty.RegisterAttached("ColumnDefinitionsSource", typeof(IEnumerable<ColumnDefinition>), typeof(GridItemsPanel), new UIPropertyMetadata(null, OnColumnDefinitionsSourceChanged));

		private static void OnColumnDefinitionsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var itemsControl = (ItemsControl)d;
			var old = (IEnumerable<ColumnDefinition>)e.OldValue;
			var now = (IEnumerable<ColumnDefinition>)e.NewValue;

			if (old != null) {
				var notify = old as INotifyCollectionChanged;
				if (notify != null) {
					var state = (StateObject)itemsControl.GetValue(StateObjectProperty);
					if (state != null) {
						CollectionChangedEventManager.RemoveListener(notify, state);
					}
				}
			}

			if (now != null) {
				SetColumnCount(itemsControl, 0);

				var notify = now as INotifyCollectionChanged;
				if (notify != null) {
					var state = GetOrCreateStateObject(itemsControl);
					CollectionChangedEventManager.AddListener(notify, state);
				}
			}

			RefreshGrid(itemsControl);
		}

		#endregion

		#region RowDefinitionsSource

		public static IEnumerable<RowDefinition> GetRowDefinitionsSource(ItemsControl obj) {
			return (IEnumerable<RowDefinition>)obj.GetValue(RowDefinitionsSourceProperty);
		}

		public static void SetRowDefinitionsSource(ItemsControl obj, IEnumerable<RowDefinition> value) {
			obj.SetValue(RowDefinitionsSourceProperty, value);
		}

		// Using a DependencyProperty as the backing store for RowDefinitionsSource.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty RowDefinitionsSourceProperty =
			DependencyProperty.RegisterAttached("RowDefinitionsSource", typeof(IEnumerable<RowDefinition>), typeof(GridItemsPanel), new UIPropertyMetadata(null, OnRowDefinitionsSourceChanged));

		private static void OnRowDefinitionsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var itemsControl = (ItemsControl)d;
			var old = (IEnumerable<RowDefinition>)e.OldValue;
			var now = (IEnumerable<RowDefinition>)e.NewValue;

			if (old != null) {
				var notify = old as INotifyCollectionChanged;
				if (notify != null) {
					var state = (StateObject)itemsControl.GetValue(StateObjectProperty);
					if (state != null) {
						CollectionChangedEventManager.RemoveListener(notify, state);
					}
				}
			}

			if (now != null) {
				SetRowCount(itemsControl, 0);

				var notify = now as INotifyCollectionChanged;
				if (notify != null) {
					var state = GetOrCreateStateObject(itemsControl);
					CollectionChangedEventManager.AddListener(notify, state);
				}
			}

			RefreshGrid(itemsControl);
		}

		#endregion

		private class StateObject : IWeakEventListener {
			public ItemsControl ItemsControl { get; private set; }

			public StateObject(ItemsControl itemsControl) {
				this.ItemsControl = itemsControl;
			}

			#region IWeakEventListener Members

			public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
				if (managerType == typeof(CollectionChangedEventManager)) {
					this.OnCollectionChanged(sender, (NotifyCollectionChangedEventArgs)e);
					return true;
				}
				return false;
			}

			#endregion

			private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
				ItemsControl.Dispatcher.BeginInvoke(new Action<ItemsControl>(RefreshGrid), ItemsControl);
			}
		}

		private static StateObject GetOrCreateStateObject(ItemsControl itemsControl) {
			var stateObject = (StateObject)itemsControl.GetValue(StateObjectProperty);
			if (stateObject == null) {
				stateObject = new StateObject(itemsControl);
				itemsControl.SetValue(StateObjectProperty, stateObject);
			}
			return stateObject;
		}

		// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
		private static readonly DependencyProperty StateObjectProperty =
			DependencyProperty.RegisterAttached("StateObject", typeof(StateObject), typeof(GridItemsPanel));


	}
}
