/*
	$Id: ButtonBehaiviours.cs 195 2011-04-12 08:27:58Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace CatWalk.Windows.Controls{
	public static class ButtonBehaviours{
		public static readonly DependencyProperty DropDownMenuProperty =
			DependencyProperty.RegisterAttached("DropDownMenu", typeof(ContextMenu), typeof(ButtonBehaviours), new UIPropertyMetadata(null, DropDownMenuChanged));
		
		[AttachedPropertyBrowsableForType(typeof(ButtonBase))]
		public static ContextMenu GetDropDownMenu(DependencyObject obj){
			return (ContextMenu)obj.GetValue(DropDownMenuProperty);
		}
		
		[AttachedPropertyBrowsableForType(typeof(ButtonBase))]
		public static void SetDropDownMenu(DependencyObject obj, bool value){
			obj.SetValue(DropDownMenuProperty, value);
		}
		
		private static void DropDownMenuChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e){
			ButtonBase button = (ButtonBase)sender;
			
			if(e.OldValue != null){
				button.Click -= Button_Click;
			}
			if(e.NewValue != null){
				button.Click += Button_Click;
			}
		}
		
		private static void Button_Click(object sender, RoutedEventArgs e){
			ButtonBase button = (ButtonBase)sender;
			ContextMenu menu = GetDropDownMenu(button);
			if(menu != null){
				menu.PlacementTarget = button;
				menu.Placement = PlacementMode.Bottom; 
				menu.IsOpen = true;
				e.Handled = true;
			}
		}
	}
}