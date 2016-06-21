using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace CatWalk.Windows.Extensions {
	public static class DragMoveBehaviour {
		public static bool GetCanDragMove(UIElement elem) {
			return (bool)elem.GetValue(CanDragMoveProperty);
		}

		public static void SetCanDragMove(UIElement elem, bool val) {
			elem.SetValue(CanDragMoveProperty, val);
		}

		public static readonly DependencyProperty CanDragMoveProperty = DependencyProperty.RegisterAttached(
			"CanDragMove",
			typeof(bool),
			typeof(DragMoveBehaviour),
			new PropertyMetadata(OnPropertyChanged));

		private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			var target = sender as UIElement;
			if(target == null) return;
			if((bool)e.NewValue == true) {
				target.MouseDown += OnMouseDown;
			} else {
				//do nothing
			}
		}

		private static void OnMouseDown(object sender, MouseButtonEventArgs e) {
			var obj = sender as DependencyObject;
			if(obj == null) return;
			if(e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Pressed) {
				Window.GetWindow(obj).DragMove();
			}
		}
	}
}
