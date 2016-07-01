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

namespace CatWalk.Windows.Extensions {
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

		[AttachedPropertyBrowsableForType(typeof(ListView))]
		public static int GetAutoFitGridColumnOffset(DependencyObject obj) {
			return (int)obj.GetValue(AutoFitGridColumnOffsetProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(ListView))]
		public static void SetAutoFitGridColumnOffset(DependencyObject obj, int value) {
			obj.SetValue(AutoFitGridColumnOffsetProperty, value);
		}

		/// <summary>
		/// カラム自動調整幅計算時加算するオフセット値
		/// </summary>
		public static readonly DependencyProperty AutoFitGridColumnOffsetProperty =
			DependencyProperty.RegisterAttached("AutoFitGridColumnOffset", typeof(int), typeof(GridViewBehaviours), new FrameworkPropertyMetadata(-6));



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
				.Subscribe(_ => {
					AutoFitGrid(lv);
				}));

			disposables.Add(Observable.FromEventPattern<EventArgs>(lv, nameof(lv.LayoutUpdated))
				.Subscribe(_ => {
					AutoFitGrid(lv);
				}));

			var sv = (ScrollViewer)lv.GetVisualChild(v => v is ScrollViewer);
			if(sv != null) {
				disposables.Add(sv.ObserveProperty<double>(ScrollViewer.ViewportWidthProperty).Subscribe(_ => {
					AutoFitGrid(lv);
				}));
			}

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

			var idx = GetAutoFitGridIndex(lv);
			if(idx < 0 || idx >= columns.Count) {
				return;
			}

			// 調整
			DettachGridViewColumnEvents(lv);

			var widthButThis = columns.Where((c, i) => i != idx).Sum(c => c.ActualWidth);
			var sv = (ScrollViewer)lv.GetVisualChild(v => v is ScrollViewer);

			var totalWidth = (sv != null) ? Math.Max(0, sv.ViewportWidth + GetAutoFitGridColumnOffset(lv)) : lv.ActualWidth;
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
