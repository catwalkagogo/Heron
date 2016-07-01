using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CatWalk.Windows {
	public static class VisualTreeExtensions {
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
	}
}
