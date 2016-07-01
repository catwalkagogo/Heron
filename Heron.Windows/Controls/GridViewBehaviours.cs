using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;
using System.ComponentModel;
using CatWalk.Windows.Extensions;

namespace CatWalk.Heron.Windows.Controls {
	public static class GridViewBehaviours {

		[AttachedPropertyBrowsableForType(typeof(ListView))]
		public static int GetAutoFitGridIndex(DependencyObject obj) {
			return (int)obj.GetValue(AutoFitGridIndexProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(ListView))]
		public static void SetAutoFitGridIndex(DependencyObject obj, int value) {
			obj.SetValue(AutoFitGridIndexProperty, value);
		}

		// Using a DependencyProperty as the backing store for AutoFitGridIndex.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty AutoFitGridIndexProperty =
			DependencyProperty.RegisterAttached("AutoFitGridIndex", typeof(int), typeof(GridViewBehaviours), new FrameworkPropertyMetadata(Int32.MinValue, (s, e) => {
				var lv = (ListView)s;

				// detach
				GetAutoFitGridItemDisposables(lv)?.Dispose();

				if((int)e.NewValue < 0) {
					return;
				}

				AttachListViewEvents(lv);
			}));

		private static void AttachListViewEvents(ListView lv) {
			// detach
			GetAutoFitGridItemDisposables(lv)?.Dispose();

			var disposables = new CompositeDisposable();
			SetAutoFitGridItemDisposables(lv, disposables);

			disposables.Add(lv.ObserveProperty<ViewBase>(ListView.ViewProperty).Subscribe(_ => {
				AutoFitGrid(lv);

				AttachListViewEvents(lv);
			}));

			var gv = lv.View as GridView;
			if(gv == null) {
				return;
			}

			disposables.Add(gv.Columns
				.CollectionChangedAsObservable()
				.Subscribe(_ => {
					AutoFitGrid(lv);
				}));

			disposables.Add(Observable.FromEventPattern<SizeChangedEventArgs>(lv, nameof(lv.SizeChanged))
				.Delay(TimeSpan.FromMilliseconds(10))
				.ObserveOnUIDispatcher()
				.Subscribe(_ => {
					AutoFitGrid(lv);
				}));

			disposables.Add(Observable.FromEventPattern<EventArgs>(lv, nameof(lv.LayoutUpdated))
				.Delay(TimeSpan.FromMilliseconds(10))
				.ObserveOnUIDispatcher()
				.Subscribe(_ => {
					AutoFitGrid(lv);
				}));
		}

		private static void DettachGridViewColumnEvents(ListView lv) {
			GetGridViewColumnDisposables(lv)?.Dispose();
		}

		private static void AttachGridViewColumnEvents(ListView lv) {
			var d = GetGridViewColumnDisposables(lv);
			d?.Dispose();

			var gv = lv.View as GridView;
			if(gv == null) {
				return;
			}

			var disposables = new CompositeDisposable();
			SetGridViewColumnDisposables(lv, disposables);

			var columns = gv.Columns;
			foreach(var column in columns) {
				// カラム自動調整
				/*disposables.Add(column
					.ObserveProperty(_ => _.ActualWidth)
					.Subscribe(_ => {
						AutoFitGrid(lv);
					}));*/
				disposables.Add(
					column
						.ObserveProperty<double>(GridViewColumn.WidthProperty)
						.Subscribe(_ => {
							AutoFitGrid(lv);
						})
				);
			}

		}

		private static void AutoFitGrid(ListView lv) {
			var gv = lv.View as GridView;
			if(gv == null) {
				return;
			}

			var columns = gv.Columns;
			// 調整

			DettachGridViewColumnEvents(lv);

			var idx = GetAutoFitGridIndex(lv);
			var widthButThis = columns.Where((c, i) => i != idx).Sum(c => c.ActualWidth);
			var sv = (ScrollViewer)lv.GetVisualChild(v => v is ScrollViewer);

			var totalWidth = (sv != null) ? Math.Max(0, sv.ViewportWidth - 6) : lv.ActualWidth;
			columns[idx].Width = totalWidth - widthButThis;

			AttachGridViewColumnEvents(lv);
		}

		private static CompositeDisposable GetAutoFitGridItemDisposables(DependencyObject obj) {
			return (CompositeDisposable)obj.GetValue(AutoFitGridItemDisposablesProperty);
		}

		private static void SetAutoFitGridItemDisposables(DependencyObject obj, CompositeDisposable value) {
			obj.SetValue(AutoFitGridItemDisposablesProperty, value);
		}

		// Using a DependencyProperty as the backing store for AutoFitGridItemDisposables.  This enables animation, styling, binding, etc...
		private static readonly DependencyProperty AutoFitGridItemDisposablesProperty =
			DependencyProperty.RegisterAttached("AutoFitGridItemDisposables", typeof(CompositeDisposable), typeof(GridViewBehaviours), new PropertyMetadata(null));



		private static CompositeDisposable GetGridViewColumnDisposables(DependencyObject obj) {
			return (CompositeDisposable)obj.GetValue(GridViewColumnDisposablesProperty);
		}

		private static void SetGridViewColumnDisposables(DependencyObject obj, CompositeDisposable value) {
			obj.SetValue(GridViewColumnDisposablesProperty, value);
		}

		// Using a DependencyProperty as the backing store for GridViewColumnDisposables.  This enables animation, styling, binding, etc...
		private static readonly DependencyProperty GridViewColumnDisposablesProperty =
			DependencyProperty.RegisterAttached("GridViewColumnDisposables", typeof(CompositeDisposable), typeof(GridViewBehaviours), new PropertyMetadata(null));


	}
}
