using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CatWalk.Heron.Windows.Controls {
	public static class ControlUtility {
		public static Visual GetVisualChild(this Visual element, Func<Visual, bool> predicate) {
			element.ThrowIfNull(nameof(element));
			predicate.ThrowIfNull(nameof(predicate));

			return GetVisualChildInternal(element, predicate);
		}

		private static Visual GetVisualChildInternal(Visual element, Func<Visual, bool> predicate) {
			(element as FrameworkElement)?.ApplyTemplate();

			var count = VisualTreeHelper.GetChildrenCount(element);
			for (var i = 0; i < count; i++) {
				var visual = VisualTreeHelper.GetChild(element, i) as Visual;

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
