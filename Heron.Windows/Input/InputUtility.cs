using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CatWalk.Collections;

namespace CatWalk.Heron.Windows.Input {
	public static class InputUtility {
		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static IEnumerable GetInputBindingsSource(DependencyObject obj) {
			return (IEnumerable)obj.GetValue(InputBindingsSourceProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(UIElement))]
		public static void SetInputBindingsSource(DependencyObject obj, IEnumerable value) {
			obj.SetValue(InputBindingsSourceProperty, value);
		}

		// Using a DependencyProperty as the backing store for InputBindingsSource.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty InputBindingsSourceProperty =
			DependencyProperty.RegisterAttached("InputBindingsSource", typeof(IEnumerable), typeof(InputUtility), new PropertyMetadata(null, InputBindingsSource_Changed));

		private static CollectionSynchronizer GetCollectionSynchronizer(DependencyObject obj) {
			return (CollectionSynchronizer)obj.GetValue(CollectionSynchronizerProperty);
		}

		private static void SetCollectionSynchronizer(DependencyObject obj, CollectionSynchronizer value) {
			obj.SetValue(CollectionSynchronizerProperty, value);
		}

		// Using a DependencyProperty as the backing store for CollectionSynchronizer.  This enables animation, styling, binding, etc...
		private static readonly DependencyProperty CollectionSynchronizerProperty =
			DependencyProperty.RegisterAttached("CollectionSynchronizer", typeof(CollectionSynchronizer), typeof(InputUtility), new PropertyMetadata(null));



		private static void InputBindingsSource_Changed(DependencyObject s, DependencyPropertyChangedEventArgs e) {
			var ui = (UIElement)s;

			if (e.OldValue != null) {
				var oldSource = (IEnumerable<InputBinding>)e.OldValue;
				var sync = GetCollectionSynchronizer(ui);
				if(sync != null) {
					sync.Dispose();
					SetCollectionSynchronizer(ui, null);
				}
			}

			if(e.NewValue != null) {
				var newSource = (IEnumerable<InputBinding>)e.NewValue;
				var sync = newSource.NotifyToCollection(ui.InputBindings);
				SetCollectionSynchronizer(ui, sync);
			}
		}
	}
}