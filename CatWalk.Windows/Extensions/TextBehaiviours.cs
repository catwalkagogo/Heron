/*
	$Id: TextBehaiviours.cs 137 2010-12-19 10:01:30Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace CatWalk.Windows.Extensions {
	public static class TextBoxBehaviors{
		public static readonly DependencyProperty IsSelectAllOnFocusProperty =
			DependencyProperty.RegisterAttached("IsSelectAllOnFocus", typeof(bool), typeof(TextBoxBehaviors), new UIPropertyMetadata(false, IsSelectAllOnFocusChanged));
		
		[AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
		public static bool GetIsSelectAllOnFocus(DependencyObject obj){
			return (bool)obj.GetValue(IsSelectAllOnFocusProperty);
		}
		
		[AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
		public static void SetIsSelectAllOnFocus(DependencyObject obj, bool value){
			obj.SetValue(IsSelectAllOnFocusProperty, value);
		}
		
		private static void IsSelectAllOnFocusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e){
			TextBoxBase textBox = (TextBoxBase)sender;
			
			bool newValue = (bool)e.NewValue;
			bool oldValue = (bool)e.OldValue;
			if(oldValue){
				textBox.GotFocus -= TextBox_GotFocus;
			}
			if(newValue){
				textBox.GotFocus += TextBox_GotFocus;
			}
		}
		
		private static void TextBox_GotFocus(object sender, RoutedEventArgs e){
			TextBoxBase textBox = (TextBoxBase)sender;
			textBox.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(delegate{
				textBox.SelectAll();
			}));
		}
	}
}