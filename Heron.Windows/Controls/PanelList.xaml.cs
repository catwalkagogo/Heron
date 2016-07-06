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

		public PanelList() {
			InitializeComponent();

			this._ListBox.ItemTemplateSelector = new PanelTemplateSelector();

			/*
			this.ObserveProperty<object>(DataContextProperty).Take(1).Subscribe(_ => {
				var m = new WindowMessages.RequestMainWindow();
				WindowsPlugin.Current.Application.Messenger.Post<WindowMessages.RequestMainWindow>(m, this.DataContext).ContinueWith(t => {
					if (m.MainWindow != null) {
						var storage = m.MainWindow.Storage;
						storage.GetAsync<GridPositions>("PanelGridPositions").ContinueWith(t2 => {
							this.InitializeGrid(t2.Result);
						});
					} else {
						throw new InvalidOperationException();
					}
				});

			});*/
			this.InitializeGrid(/*new GridPositions()*/);
		}

		#region Grid

		private void InitializeGrid(/*GridPositions pos*/) {
			var gridOvservation = this.ObserveProperty<Orientation>(OrientationProperty)
				.Select(o => Tuple.Create(o, this._ListBox.Items.Count))
				.Merge(this._ListBox.Items.ObserveProperty(_ => _.Count)
					.Select(_ => Tuple.Create(this.Orientation, _)))
				.DistinctUntilChanged();

			var splitterSubscribe = gridOvservation.Subscribe(_ => {
				this._SplitterGrid.ColumnDefinitions.Clear();
				this._SplitterGrid.RowDefinitions.Clear();
				this._SplitterGrid.Children.Clear();

				// Column
				if (_.Item1 == Orientation.Horizontal) {
					var count = _.Item2;

					foreach (var def in this.GetColumnDefinitions(count, false/*, pos.ColumnLengths*/)) {
						this._SplitterGrid.ColumnDefinitions.Add(def);
					}
					foreach (var column in Enumerable.Range(0, count / 2).Select(i => i * 2 + 1)) {
						var splitter = new GridSplitter() {
							VerticalAlignment = VerticalAlignment.Stretch,
							ResizeBehavior = GridResizeBehavior.PreviousAndNext,
							Width = 8,
						};
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
				this.SetColumnsDefinition(_.Item1, _.Item2/*, pos.ColumnLengths*/);

				// Row
				/*
				if (_.Item1 == Orientation.Vertical) {
					var count = _.Item2;

					foreach (var def in this.GetRowDefinitions(count, false)) {
						this._SplitterGrid.RowDefinitions.Add(def);
					}
					foreach (var Row in Enumerable.Range(0, count / 2).Select(i => i * 2 + 1)) {
						var splitter = new GridSplitter() {
							VerticalAlignment = VerticalAlignment.Stretch,
							ResizeBehavior = GridResizeBehavior.PreviousAndNext,
							Width = 8,
						};
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
				this.SetRowsDefinition(_.Item1, _.Item2);
				*/
			});
			this._Disposables.Add(splitterSubscribe);
		}

		private void SetColumnsDefinition(Orientation o, int count/*, GridLength[] lengths*/) {
			IEnumerable<ColumnDefinition> defs;

			if (o == Orientation.Horizontal) {
				defs = this.GetColumnDefinitions(count, true/*, lengths*/);

			} else {
				defs = Seq.Make(new ColumnDefinition() {
					Width = new GridLength(1, GridUnitType.Star)
				});
			}
			GridItemsPanel.SetColumnDefinitionsSource(this._ListBox, defs);
		}

		private IEnumerable<ColumnDefinition> GetColumnDefinitions(int count, bool isList/*, GridLength[] lengths*/) {
		/*	if(lengths == null) {
				lengths = new GridLength[0];
			}*/

			for(var i = 0; i < count; i++) {
				//var width = (i < lengths.Length) ? lengths[i] : new GridLength(1, GridUnitType.Star);
				var width = new GridLength(1, GridUnitType.Star);

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
						Width = GridLength.Auto,
					};
					if (isList) {
						var binding = new Binding("Width");
						binding.Source = this._SplitterGrid.ColumnDefinitions[i * 2 + 1];
						binding.Mode = BindingMode.TwoWay;
						BindingOperations.SetBinding(sp, ColumnDefinition.WidthProperty, binding);
					}

					yield return sp;
				}
			}
		}

		private void SetRowsDefinition(Orientation o, int count) {
			IEnumerable<RowDefinition> defs;

			if (o == Orientation.Vertical) {
				defs = this.GetRowDefinitions(count, true);
			} else {
				defs = Seq.Make(new RowDefinition() {
					Height = new GridLength(1, GridUnitType.Star)
				});
			}
			GridItemsPanel.SetRowDefinitionsSource(this._ListBox, defs);
		}

		private IEnumerable<RowDefinition> GetRowDefinitions(int count, bool isList) {
			for (var i = 0; i < count; i++) {
				var def = new RowDefinition() {
					Height = new GridLength(1, GridUnitType.Star)
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
		/*
		internal struct GridPositions {
			public GridLength[] ColumnLengths;
			public GridLength[] RowLengths;
		}*/
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
