using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
using CatWalk.Collections;

namespace CatWalk.Windows.Extensions {
	public static class MultiSelector {


		public static IEnumerable GetSelectedItems(DependencyObject obj) {
			return (IEnumerable)obj.GetValue(SelectedItemsProperty);
		}

		public static void SetSelectedItems(DependencyObject obj, IEnumerable value) {
			obj.SetValue(SelectedItemsProperty, value);
		}

		// Using a DependencyProperty as the backing store for SelectedItems.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SelectedItemsProperty =
			DependencyProperty.RegisterAttached("SelectedItems", typeof(IEnumerable), typeof(MultiSelector), new PropertyMetadata(null, OnSelectedItemsChanged));

		private static readonly DependencyProperty CollectionSynchronizerProperty =
			DependencyProperty.RegisterAttached("CollectionSynchronizer", typeof(MultiSelectorSynchronizer), typeof(MultiSelector));

		private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(e.OldValue != null) {
				var sync = d.GetValue(CollectionSynchronizerProperty) as MultiSelectorSynchronizer;
				sync.Stop();
			}

			if(e.NewValue != null) {
				var list = (IEnumerable)e.NewValue;
				var sync = new MultiSelectorSynchronizer((Selector)d, list);
				sync.Start();
				d.SetValue(CollectionSynchronizerProperty, sync);
			}
		}

		private static IList GetSelectedItemsList(DependencyObject d) {
			var sel = d as System.Windows.Controls.Primitives.MultiSelector;
			if(sel != null) {
				return sel.SelectedItems;
			} else {
				var listBox = d as ListBox;
				if(listBox != null) {
					return listBox.SelectedItems;
				} else {
					throw new ArgumentException("d");
				}
			}
		}

		private class MultiSelectorSynchronizer {
			private IEnumerable _Collection;
			private INotifyCollectionChanged _NotifyCollectionChanged;
			private Selector _Selector;
			private Lazy<Action<object>> _CollectionAdd;
			private Lazy<Action<object>> _CollectionRemove;
			private CollectionSynchronizer _Connector;

			public MultiSelectorSynchronizer(Selector selector, IEnumerable list) {
				this._Selector = selector;
				this._Collection = list;
				this._NotifyCollectionChanged = list as INotifyCollectionChanged;

				this._CollectionAdd = new Lazy<Action<object>>(() => {
					var lambda = CollectionExpressions.GetAddFunction(this._Collection.GetType());
					return new Action<object>((v) => {
						lambda(this._Collection, v);
					});
				});
				this._CollectionRemove = new Lazy<Action<object>>(() => {
					var lambda = CollectionExpressions.GetRemoveFunction(this._Collection.GetType());
					return new Action<object>((v) => {
						lambda(this._Collection, v);
					});
				});

				var selList = GetSelectedItemsList(selector);
				selList.Clear();
				foreach(var item in list) {
					selList.Add(item);
				}

			}

			void selector_SelectionChanged(object sender, SelectionChangedEventArgs e) {
				this.Stop();
				foreach(var item in e.RemovedItems) {
					this._CollectionRemove.Value(item);
				}
				foreach(var item in e.AddedItems) {
					this._CollectionAdd.Value(item);
				}
				this.Start();
			}

			public void Start() {
				this._Selector.SelectionChanged += selector_SelectionChanged;
				if(this._Connector != null) {
					throw new InvalidOperationException();
				}
				this._Connector = this._Collection.NotifyToCollectionWeak(GetSelectedItemsList(this._Selector));
			}

			public void Stop() {
				this._Selector.SelectionChanged -= selector_SelectionChanged;
				this._Connector.Stop();
				this._Connector = null;
			}
		}
	}
}
