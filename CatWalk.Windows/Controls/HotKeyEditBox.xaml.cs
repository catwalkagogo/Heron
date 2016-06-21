/*
	$Id: HotKeyEditBox.xaml.cs 137 2010-12-19 10:01:30Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;

namespace CatWalk.Windows{

	public partial class HotKeyEditBox : UserControl{
		public HotKeyEditBox(){
			this.InitializeComponent();

			this.keyBox.ItemsSource = Enum.GetValues(typeof(Key));
			this.ctrlBox.Checked += this.CheckChanged;
			this.RefreshChecks();
		}
		
		#region 関数
		
		private void CheckChanged(object sender, RoutedEventArgs e){
			this.Modifiers = this.GetModifiers();
		}

		private void RefreshChecks(){
			this.shiftBox.IsChecked = (this.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
			this.ctrlBox.IsChecked = (this.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
			this.altBox.IsChecked = (this.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
			this.winBox.IsChecked = (this.Modifiers & ModifierKeys.Windows) > ModifierKeys.Windows;
		}

		private ModifierKeys GetModifiers(){
			var mods = ModifierKeys.None;
			if(this.shiftBox.IsChecked.Value){
				mods |= ModifierKeys.Shift;
			}
			if(this.ctrlBox.IsChecked.Value){
				mods |= ModifierKeys.Control;
			}
			if(this.altBox.IsChecked.Value){
				mods |= ModifierKeys.Alt;
			}
			if(this.winBox.IsChecked.Value){
				mods |= ModifierKeys.Windows;
			}
			return mods;
		}

		#endregion
		
		#region プロパティ
		
		public static readonly DependencyProperty KeyProperty =
			DependencyProperty.Register("Key", typeof(Key), typeof(HotKeyEditBox),
				new FrameworkPropertyMetadata(Key.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, KeyPropertyChanged));
		public Key Key{
			get{
				return (Key)this.GetValue(KeyProperty);
			}
			set{
				this.SetValue(KeyProperty, value);
			}
		}
		
		private static void KeyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e){
		}
		
		public static readonly DependencyProperty ModifiersProperty =
			DependencyProperty.Register("Modifiers", typeof(ModifierKeys), typeof(HotKeyEditBox),
			new FrameworkPropertyMetadata(ModifierKeys.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ModifiersPropertyChanged));
		public ModifierKeys Modifiers{
			get{
				return (ModifierKeys)this.GetValue(ModifiersProperty);
			}
			set{
				this.SetValue(ModifiersProperty, value);
			}
		}
		
		private static void ModifiersPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e){
			((HotKeyEditBox)sender).RefreshChecks();
		}
		
		#endregion
	}
}