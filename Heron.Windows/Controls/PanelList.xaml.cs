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
using System.Globalization;
using System.Collections;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Windows.Controls.Primitives;
using System.Reactive.Concurrency;
using CatWalk.Heron.Configuration;
using CatWalk.Windows;

namespace CatWalk.Heron.Windows.Controls {
	/// <summary>
	/// Interaction logic for PanelList.xaml
	/// </summary>
	public partial class PanelList : UserControl {
		public const string DefaultPanelTemplateKey = "DefaultPanelTemplate";

		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register("Orientation", typeof(Orientation), typeof(PanelList), new PropertyMetadata(Orientation.Horizontal));


		private CompositeDisposable _Disposables = new CompositeDisposable();

		private GridPositions _GridPositions;

		public PanelList() {
			InitializeComponent();

			this._ListBox.ItemTemplateSelector = new PanelTemplateSelector();

			this._ArrangeGridCommand = this._ListBox.Items.ObserveProperty(_ => _.Count)
				.Select(c => c != 0)
				.ToReactiveCommand();
			this._ArrangeGridCommand.Subscribe(_ => {
				this.ArrangeGrid();
			});
			this._Disposables.Add(this._ArrangeGridCommand);

			this.ObserveProperty<object>(DataContextProperty).Take(1).Subscribe(_ => {
				var m = new WindowMessages.RequestMainWindow();
				WindowsPlugin.Current.Application.Messenger.Post<WindowMessages.RequestMainWindow>(m, this.DataContext).ContinueWith(t => {
					if (m.MainWindow != null) {
						var storage = m.MainWindow.Storage;
						storage.GetAsync<GridPositions>("PanelGridPositions").ContinueWith(t2 => {
							this._GridPositions = t2.Result;
							this.InitializeGrid();
						});
					} else {
						throw new InvalidOperationException();
					}
				});
			});

			this.Selector.SelectionChanged += Selector_SelectionChanged;

			this.Loaded += PanelList_Loaded;
			this.GotFocus += PanelList_GotFocus;
		}

		private void PanelList_GotFocus(object sender, RoutedEventArgs e) {
			/*var focused = FocusManager.GetFocusedElement(this.GetMainWindow()) as FrameworkElement;
			if(focused != null) {
				var panel = focused.FindAncestor(elm => elm is Panel) as FrameworkElement;
				if(panel != null) {
					var index = this.Selector.Items.IndexOf(panel.DataContext);
					if(index >= 0) {
						this.Selector.SelectedIndex = index;
					}
				}
			}*/
			/*
			var focused = this.Selector.Items.Cast<object>()
				.Select(item => (FrameworkElement)this.Selector.ItemContainerGenerator.ContainerFromItem(item))
				.FirstOrDefault(item => item.IsFocused);

			if (focused != null) {
				this.Selector.SelectedIndex = this.Selector.ItemContainerGenerator.IndexFromContainer(focused);
			}

			var focused = FocusManager.GetFocusedElement(this.GetMainWindow()) as FrameworkElement;
			if (focused != null) {
				foreach(var container in this.Selector.Items.Cast<object>().Select(item => (FrameworkElement)this.Selector.ItemContainerGenerator.ContainerFromItem(item))){
					var focused2 = container.GetVisualChild(d => d == focused);
					if(focused2 != null) {
						this.Selector.SelectedIndex = this.Selector.ItemContainerGenerator.IndexFromContainer(container);
						break;
					}
				}
			}*/
		}

		private void Selector_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			var selected = this.Selector.SelectedItem;
		}

		private void PanelList_Loaded(object sender, RoutedEventArgs e) {
			Window.GetWindow(this).Closing += PanelList_Closing;
		}

		private void PanelList_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			// 設定保存
			var pos = this.GetGridPositions();

			this.GetMainWindowViewModel().Storage.SetAsync("PanelGridPositions", pos);
		}

		#region Grid

		private void InitializeGrid() {
			var gridOvservation = this.ObserveProperty<Orientation>(OrientationProperty)
				.Select(o => Tuple.Create(o, this._ListBox.Items.Count))
				.Merge(this._ListBox.Items.ObserveProperty(_ => _.Count)
					.Select(_ => Tuple.Create(this.Orientation, _)))
				.DistinctUntilChanged();

			var splitterSubscribe = gridOvservation.Subscribe(_ => {
				var pos = this._GridPositions;
				this.SetSplitterDefinitions(_.Item1, _.Item2, pos);

				// Column
				this.SetPanelColumnsDefinition(_.Item1, _.Item2, pos.ColumnLengths);

				// Row
				this.SetPanelRowsDefinition(_.Item1, _.Item2, pos.RowLengths);
			});
			this._Disposables.Add(splitterSubscribe);
		}

		private void SetSplitterDefinitions(Orientation o, int count, GridPositions pos) {
			this._SplitterGrid.ColumnDefinitions.Clear();
			this._SplitterGrid.RowDefinitions.Clear();
			this._SplitterGrid.Children.Clear();

			// Column
			this.SetSplitterColumnsDefinition(o, count, pos.ColumnLengths);

			// Row
			this.SetSplitterRowDefinition(o, count, pos.RowLengths);

		}

		private void SetSplitterColumnsDefinition(Orientation o, int count, double[] lengths) {
			if (o == Orientation.Horizontal) {
				foreach (var def in this.GetColumnDefinitions(count, false, lengths)) {
					this._SplitterGrid.ColumnDefinitions.Add(def);
				}
				foreach (var column in Enumerable.Range(0, Math.Max(0, count - 1)).Select(i => i * 2 + 1)) {
					var splitter = this.GetSplitter(GridResizeDirection.Columns);

					Grid.SetColumn(splitter, column);
					this._SplitterGrid.Children.Add(splitter);
				}
			} else {
				foreach (var def in Seq.Make(new ColumnDefinition() {
					Width = new GridLength(1, GridUnitType.Star)
				})) {
					this._SplitterGrid.ColumnDefinitions.Add(def);
				}
			}

		}

		private GridSplitter GetSplitter(GridResizeDirection dir) {
			var splitter = new GridSplitter() {
				VerticalAlignment = VerticalAlignment.Stretch,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				ResizeBehavior = GridResizeBehavior.PreviousAndNext,
				ResizeDirection = dir,
			};
			splitter.InputBindings.Add(new MouseBinding(this._ArrangeGridCommand, new MouseGesture(MouseAction.LeftDoubleClick)));
			splitter.DragCompleted += Splitter_DragCompleted;
			return splitter;
		}

		private  void SetSplitterRowDefinition(Orientation o, int count, double[] lengths) {
			if (o == Orientation.Vertical) {
				foreach (var def in this.GetRowDefinitions(count, false, lengths)) {
					this._SplitterGrid.RowDefinitions.Add(def);
				}
				foreach (var Row in Enumerable.Range(0, Math.Max(0, count - 1)).Select(i => i * 2 + 1)) {
					var splitter = this.GetSplitter(GridResizeDirection.Rows);
					Grid.SetRow(splitter, Row);
					this._SplitterGrid.Children.Add(splitter);
				}
			} else {
				foreach (var def in Seq.Make(new RowDefinition() {
					Height = new GridLength(1, GridUnitType.Star)
				})) {
					this._SplitterGrid.RowDefinitions.Add(def);
				}
			}
		}

		private GridPositions GetGridPositions() {
			var pos = new GridPositions() {
				ColumnLengths = this._SplitterGrid.ColumnDefinitions.Where((_, i) => i % 2 == 0).Select(_ => _.Width.Value).ToArray(),
				RowLengths = this._SplitterGrid.RowDefinitions.Where((_, i) => i % 2 == 0).Select(_ => _.Height.Value).ToArray(),
			};

			return pos;
		}

		private void Splitter_DragCompleted(object sender, DragCompletedEventArgs e) {
			this._GridPositions = this.GetGridPositions();
		}

		private void SetPanelColumnsDefinition(Orientation o, int count, double[] lengths) {
			IEnumerable<ColumnDefinition> defs;

			if (o == Orientation.Horizontal) {
				defs = this.GetColumnDefinitions(count, true, lengths);

			} else {
				defs = Seq.Make(new ColumnDefinition() {
					Width = new GridLength(1, GridUnitType.Star)
				});
			}
			GridItemsPanel.SetColumnDefinitionsSource(this._ListBox, defs);
		}

		private IEnumerable<ColumnDefinition> GetColumnDefinitions(int count, bool isList, double[] lengths) {
			if(lengths == null) {
				lengths = new double[0];
			}

			for(var i = 0; i < count; i++) {
				var width = (i < lengths.Length && !isList) ? new GridLength(lengths[i], GridUnitType.Star) : new GridLength(1, GridUnitType.Star);

				var def = new ColumnDefinition() {
					Width = width
				};

				if (isList) {
					var binding = new Binding("Width");
					binding.Source = this._SplitterGrid.ColumnDefinitions[i * 2];
					binding.Mode = BindingMode.TwoWay;
					BindingOperations.SetBinding(def, ColumnDefinition.WidthProperty, binding);
				}

				yield return def;

				if(i != (count - 1)) {
					var sp = new ColumnDefinition() {
						Width = new GridLength(8),
					};
					/*
					if (isList) {
						var binding = new Binding("Width");
						binding.Source = this._SplitterGrid.ColumnDefinitions[i * 2 + 1];
						binding.Mode = BindingMode.TwoWay;
						BindingOperations.SetBinding(sp, ColumnDefinition.WidthProperty, binding);
					}
					*/
					yield return sp;
				}
			}
		}

		private void SetPanelRowsDefinition(Orientation o, int count, double[] lengths) {
			IEnumerable<RowDefinition> defs;

			if (o == Orientation.Vertical) {
				defs = this.GetRowDefinitions(count, true, lengths);
			} else {
				defs = Seq.Make(new RowDefinition() {
					Height = new GridLength(1, GridUnitType.Star)
				});
			}
			GridItemsPanel.SetRowDefinitionsSource(this._ListBox, defs);
		}

		private IEnumerable<RowDefinition> GetRowDefinitions(int count, bool isList, double[] lengths) {
			for (var i = 0; i < count; i++) {
				var height = (i < lengths.Length && !isList) ? new GridLength(lengths[i], GridUnitType.Star) : new GridLength(1, GridUnitType.Star);

				var def = new RowDefinition() {
					Height = height
				};

				if (isList) {
					var binding = new Binding("Width");
					binding.Source = this._SplitterGrid.RowDefinitions[i * 2];
					binding.Mode = BindingMode.TwoWay;
					BindingOperations.SetBinding(def, RowDefinition.HeightProperty, binding);
				}

				yield return def;

				if (i != (count - 1)) {
					var sp = new RowDefinition() {
						Height = GridLength.Auto,
					};
					if (isList) {
						var binding = new Binding("Width");
						binding.Source = this._SplitterGrid.RowDefinitions[i * 2 + 1];
						binding.Mode = BindingMode.TwoWay;
						BindingOperations.SetBinding(sp, RowDefinition.HeightProperty, binding);
					}

					yield return sp;
				}
			}
		}

		#endregion

		#region ArrangeGrid

		private ReactiveCommand _ArrangeGridCommand;

		public ReactiveCommand ArrangeGridCommand {
			get {
				return this._ArrangeGridCommand;
			}
		}

		public void ArrangeGrid() {
			var o = this.Orientation;
			var count = this._ListBox.Items.Count;
			var pos = new GridPositions();

			this.SetSplitterDefinitions(o, count, pos);

			// Column
			this.SetPanelColumnsDefinition(o, count, pos.ColumnLengths);

			// Row
			this.SetPanelRowsDefinition(o, count, pos.RowLengths);
		}


		#endregion

		public Selector Selector {
			get {
				return this._ListBox;
			}
		}

		private class PanelTemplateSelector : FactoryDataTemplateSelector<SystemEntryViewModel> {
			public PanelTemplateSelector() : base(WindowsPlugin.Current.PanelTemplateFactory, DefaultPanelTemplateKey) { }

			protected override SystemEntryViewModel ItemDataSelector(object item) {
				// 現在のISystemItemに応じてViewModelを作成する
				var panelVM = (PanelViewModel)item;
				var entry = panelVM.ListView.CurrentEntry;
				return entry;
			}
		}
		
		internal struct GridPositions {
			public double[] ColumnLengths;
			public double[] RowLengths;
		}
	}

	internal abstract class PanelGridIndexConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			return this.GetColumnIndex((int)values[0], (Orientation)values[1]);
		}

		protected abstract int GetColumnIndex(int itemIndex, Orientation o);

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}

	internal class PanelGridColumnConverter : PanelGridIndexConverter {
		protected override int GetColumnIndex(int itemIndex, Orientation o) {
			return o == Orientation.Horizontal ? itemIndex * 2 : 0;
		}
	}

	internal class PanelGridRowConverter : PanelGridIndexConverter {
		protected override int GetColumnIndex(int itemIndex, Orientation o) {
			return o == Orientation.Vertical ? itemIndex * 2 : 0;
		}
	}

}
