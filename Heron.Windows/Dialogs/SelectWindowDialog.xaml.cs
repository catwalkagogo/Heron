using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;
using CatWalk.Windows.Extensions;

namespace CatWalk.Heron.Windows.Dialogs {
	using Win32 = CatWalk.Win32;

	/// <summary>
	/// Interaction logic for SelectWindowDialog.xaml
	/// </summary>
	public partial class SelectWindowDialog : Window {
		public SelectWindowDialog() {
			InitializeComponent();

			this.AddHandler(CatWalk.Windows.Extensions.HoldingKeys.HoldingKeysReleasedEvent, new RoutedEventHandler(this.OnHoldingKeyReleased));
		}

		private void OnHoldingKeyReleased(object sender, RoutedEventArgs e){
			this.Close();
		}

		public IEnumerable ItemsSource {
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SelectWindowDialog), new UIPropertyMetadata(null));

		public object SelectedValue {
			get { return (object)GetValue(SelectedValueProperty); }
			set { SetValue(SelectedValueProperty, value); }
		}

		public static readonly DependencyProperty SelectedValueProperty =
			DependencyProperty.Register("SelectedValue", typeof(object), typeof(SelectWindowDialog), new UIPropertyMetadata(null));

		public IReadOnlyCollection<Key> HoldingKeys {
			get {
				return CatWalk.Windows.Extensions.HoldingKeys.GetHoldingKeys(this);
			}
			set {
				CatWalk.Windows.Extensions.HoldingKeys.SetHoldingKeys(this, value);
			}
		}

		private void _this_Loaded(object sender, RoutedEventArgs e) {
			if(SystemParameters.IsGlassEnabled){
				var src = ((HwndSource)HwndSource.FromVisual(this));
				src.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

				var blur = new Win32::DwmBlurBehind();
				blur.Enabled = true;

				Win32::Dwm.EnableBlurBehindWindow(src.Handle, blur);
			}
			var screen = this.GetCurrentScreen();
			this.MaxWidth = screen.ScreenArea.Width / 3d * 2d;
			this.MaxHeight = screen.ScreenArea.Height / 3d * 2d;
			this.AdjustPosition();
		}

		protected override void OnDeactivated(EventArgs e){
			base.OnDeactivated(e);
			try{
				this.Close();
			}catch{
			}
		}

		private bool _IsKeyUp = false;
		protected override void OnPreviewKeyUp(KeyEventArgs e){
			if(!this._IsKeyUp){
				this._IsKeyUp = true;
				e.Handled = true;
			}
			base.OnPreviewKeyUp(e);
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e){
			base.OnPreviewKeyDown(e);
			if(e.Key == Key.Tab){
				e.Handled = true;
				if(this._IsKeyUp){
					if((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift){
						this.SelectPrevious();
					}else{
						this.SelectNext();
					}
				}
			}
		}

		private void SelectPrevious(){
			if(this._SelectBox.Items.Count > 0){
				var idx = this._SelectBox.SelectedIndex;
				if(idx < 0){
					idx = 0;
				}else{
					idx--;
				}
				if(idx < 0){
					idx = this._SelectBox.Items.Count - 1;
				}
				this._SelectBox.SelectedIndex = idx;
			}
		}

		private void SelectNext(){
			if(this._SelectBox.Items.Count > 0){
				var idx = this._SelectBox.SelectedIndex;
				if(idx < 0){
					idx = 0;
				}else{
					idx++;
				}
				if(this._SelectBox.Items.Count <= idx){
					idx = 0;
				}
				this._SelectBox.SelectedIndex = idx;
			}
		}

		private void _this_SizeChanged(object sender, SizeChangedEventArgs e) {
			this.AdjustPosition();
		}

		private void AdjustPosition(){
			var screen = this.GetCurrentScreen();
			this.Left = (screen.ScreenArea.Width - this.ActualWidth) / 2d + screen.ScreenArea.X;
			this.Top = (screen.ScreenArea.Height - this.ActualHeight) / 2d + screen.ScreenArea.Y;
		}
	}
}
