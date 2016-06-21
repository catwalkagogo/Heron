using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.InteropServices;

namespace CatWalk.Windows.Extensions{
	public static class CursorClipping {
		public static bool GetIsEnabled(DependencyObject obj) {
			return (bool)obj.GetValue(IsEnabledProperty);
		}

		public static void SetIsEnabled(DependencyObject obj, bool value) {
			obj.SetValue(IsEnabledProperty, value);
		}

		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(FrameworkElement),
			new UIPropertyMetadata(false, IsEnabled_Changed));

		public static void IsEnabled_Changed(DependencyObject dp, DependencyPropertyChangedEventArgs e){
			var element = (FrameworkElement)dp;
			if((bool)e.NewValue){
				var point = element.PointToScreen(new Point(0, 0));
				ClipCursor(point.X, point.Y, point.X + element.ActualWidth, point.Y + element.ActualHeight);
			}else{
				UnclipCursor();
			}
		}

		public static void ClipCursor(double x, double y, double width, double height){
			var rect = new RECT(){
				Left = (int)x,
				Top = (int)y,
				Bottom = (int)height,
				Right = (int)width,
			};
			ClipCursor(rect);
		}

		public static void UnclipCursor(){
			ClipCursor(null);
		}

		[DllImport("User32.dll")]
		private static extern bool ClipCursor(RECT lpRect);

		[StructLayout(LayoutKind.Sequential)]
		private class RECT{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}
	}
}
