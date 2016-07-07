using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace CatWalk.Windows {
	public static class VisualTreeExtensions {
		public static DependencyObject FindAncestor(this DependencyObject element, Func<DependencyObject, bool> predicate) {
			element.ThrowIfNull(nameof(element));
			predicate.ThrowIfNull(nameof(predicate));

			if (predicate(element)) {
				return element;
			}else {
				var parent = LogicalTreeHelper.GetParent(element);
				if(parent != null) {
					return parent.FindAncestor(predicate);
				}else {
					return null;
				}
			}
		}

		public static DependencyObject GetVisualChild(this DependencyObject element, Func<DependencyObject, bool> predicate) {
			element.ThrowIfNull(nameof(element));
			predicate.ThrowIfNull(nameof(predicate));

			return GetVisualChildInternal(element, predicate);
		}

		private static DependencyObject GetVisualChildInternal(DependencyObject element, Func<DependencyObject, bool> predicate) {
			(element as FrameworkElement)?.ApplyTemplate();

			var count = VisualTreeHelper.GetChildrenCount(element);
			for (var i = 0; i < count; i++) {
				var visual = VisualTreeHelper.GetChild(element, i) as DependencyObject;

				if(visual == null) {
					continue;
				}

				if (predicate(visual)) {
					return visual;
				}

				var found = GetVisualChildInternal(visual, predicate);
				if(found != null) {
					return found;
				}
			}

			return null;
		}

		/// <summary>
		/// SelectorでFocusで選択状態によって挙動が変わるのを制御
		/// </summary>
		/// <param name="list"></param>
		public static void FocusSelector(this Selector list) {
			list.ThrowIfNull(nameof(list));

			var selectedItem = (FrameworkElement)list.ItemContainerGenerator.ContainerFromItem(list.SelectedItem);

			if (selectedItem == null) {
				list.Focus();
			} else {
				selectedItem.Focus();
			}
		}
	}
}
