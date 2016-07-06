using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CatWalk.Heron.Windows {
	public static class PluginExtensions {
		[AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
		public static bool GetNotifyCreatedEvent(DependencyObject obj) {
			return (bool)obj.GetValue(NotifyCreatedEventProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
		public static void SetNotifyCreatedEvent(DependencyObject obj, bool value) {
			obj.SetValue(NotifyCreatedEventProperty, value);
		}

		// Using a DependencyProperty as the backing store for NotifyCreatedEvent.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty NotifyCreatedEventProperty =
			DependencyProperty.RegisterAttached("NotifyCreatedEvent", typeof(bool), typeof(PluginExtensions), new PropertyMetadata(false, (s, e) => {
				var element = (FrameworkElement)s;
				if ((bool)e.NewValue) {
					element.Loaded += Element_Loaded;
				}
			}));

		private static void Element_Loaded(object sender, RoutedEventArgs e) {
			var element = (FrameworkElement)sender;

			GetPlugin(element)?.OnFrameworkElementCreated(element);
		}

		public static WindowsPlugin GetPlugin(DependencyObject obj) {
			obj.ThrowIfNull(nameof(obj));

			var window = Window.GetWindow(obj);
			var mainWindow = window as MainWindow;
			if(mainWindow != null) {
				return mainWindow.Plugin;
			}

			return WindowsPlugin.Current;
		}
	}
}
