/*
	$Id: FontDialog.xaml.cs 137 2010-12-19 10:01:30Z cs6m7y@bma.biglobe.ne.jp $
*/

using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Threading;
using CatWalk.Collections;

namespace CatWalk.Windows {
	public partial class FontDialog : Window{
		public FontDialog() {
			InitializeComponent();

			this.sizeListBox.ItemsSource = new double[]{6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,28,32,48,64};
			this.weightListBox.ItemsSource = new FontWeight[]{
				FontWeights.ExtraLight, FontWeights.Light, FontWeights.Normal, FontWeights.Medium, FontWeights.DemiBold,
				FontWeights.Bold, FontWeights.ExtraBold, FontWeights.Black, FontWeights.ExtraBlack
			};
			this.styleListBox.ItemsSource = new FontStyle[]{FontStyles.Normal, FontStyles.Italic, FontStyles.Oblique};
			this.stretchListBox.ItemsSource = new FontStretch[]{
				FontStretches.UltraCondensed, FontStretches.SemiCondensed, FontStretches.Normal,
				FontStretches.Medium, FontStretches.SemiExpanded, FontStretches.UltraExpanded};

			var fontFamilyConverter = new FontFamilyConverter();
			this.fontListBox.ItemsSource = new PrefixDictionary<FontFamily>(
				CharIgnoreCaseComparer.Comparer,
				Fonts.SystemFontFamilies
					.Select(family => new KeyValuePair<string, FontFamily>(
						(string)fontFamilyConverter.ConvertToString(family),
						family))
					.Distinct(pair => pair.Key)
					.ToDictionary(pair => pair.Key, pair => pair.Value));

			this.Loaded += this.LoadedHandler;
			this.sizeTextBox.TextChanged += delegate{
				var text = this.sizeTextBox.Text;
				double size;
				if(Double.TryParse(text, out size)){
					this.SelectedFontSize = size;
				}
			};
			this.fontTextBox.TextChanged += this.FontTextBox_TextChanged;
			this.sizeListBox.SelectionChanged += delegate{
				this.sizeListBox.ScrollIntoView(this.sizeListBox.SelectedItem);
			};
			this.fontListBox.SelectionChanged += delegate{
				this.fontListBox.ScrollIntoView(this.fontListBox.SelectedItem);
			};
		}

		private void LoadedHandler(object sender, EventArgs e){
			this.sizeListBox.ScrollIntoView(this.sizeListBox.SelectedItem);
			this.weightListBox.ScrollIntoView(this.weightListBox.SelectedItem);
			this.styleListBox.ScrollIntoView(this.styleListBox.SelectedItem);
			this.fontListBox.ScrollIntoView(this.fontListBox.SelectedItem);
		}
		
		private void FontTextBox_TextChanged(object sender, TextChangedEventArgs e){
			var text = this.fontTextBox.Text;
			if(!text.IsNullOrEmpty()){
				var dict = (PrefixDictionary<FontFamily>)this.fontListBox.ItemsSource;
				var found = dict.Search(text, false).FirstOrDefault();
				if(found.Value != null){
					var idx = this.fontTextBox.CaretIndex;
					this.SelectedFontFamily = found.Value;
					this.fontTextBox.TextChanged -= this.FontTextBox_TextChanged;
					this.fontTextBox.Text = text;
					this.fontTextBox.TextChanged += this.FontTextBox_TextChanged;
					this.fontTextBox.CaretIndex = idx;
				}
			}
		}
		
		#region コマンド

		private void Cancel_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}
		
		private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e){
			this.DialogResult = false;
			this.Close();
		}
		
		private void OK_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}
		
		private void OK_Executed(object sender, ExecutedRoutedEventArgs e){
			this.DialogResult = true;
			this.Close();
		}

		#endregion

		#region プロパティ

		public static readonly DependencyProperty SelectedFontFamilyProperty = DependencyProperty.Register("SelectedFontFamily", typeof(FontFamily), typeof(FontDialog));
		public FontFamily SelectedFontFamily{
			get{
				return (FontFamily)this.GetValue(SelectedFontFamilyProperty);
			}
			set{
				this.fontTextBox.TextChanged -= this.FontTextBox_TextChanged;
				this.SetValue(SelectedFontFamilyProperty, value);
				this.fontTextBox.TextChanged += this.FontTextBox_TextChanged;
			}
		}

		public static readonly DependencyProperty SelectedFontSizeProperty = DependencyProperty.Register("SelectedFontSize", typeof(double), typeof(FontDialog));
		public double SelectedFontSize{
			get{
				return (double)this.GetValue(SelectedFontSizeProperty);
			}
			set{
				this.SetValue(SelectedFontSizeProperty, value);
			}
		}

		public static readonly DependencyProperty SelectedFontWeightProperty = DependencyProperty.Register("SelectedFontWeight", typeof(FontWeight), typeof(FontDialog));
		public FontWeight SelectedFontWeight{
			get{
				return (FontWeight)this.GetValue(SelectedFontWeightProperty);
			}
			set{
				this.SetValue(SelectedFontWeightProperty, value);
			}
		}

		public static readonly DependencyProperty SelectedFontStyleProperty = DependencyProperty.Register("SelectedFontStyle", typeof(FontStyle), typeof(FontDialog));
		public FontStyle SelectedFontStyle{
			get{
				return (FontStyle)this.GetValue(SelectedFontStyleProperty);
			}
			set{
				this.SetValue(SelectedFontStyleProperty, value);
			}
		}

		public static readonly DependencyProperty SelectedFontStretchProperty = DependencyProperty.Register("SelectedFontStretch", typeof(FontStretch), typeof(FontDialog));
		public FontStretch SelectedFontStretch{
			get{
				return (FontStretch)this.GetValue(SelectedFontStretchProperty);
			}
			set{
				this.SetValue(SelectedFontStretchProperty, value);
			}
		}

		public static readonly DependencyProperty SampleTextProperty = DependencyProperty.Register("SampleText", typeof(string), typeof(FontDialog), new PropertyMetadata("Sample Text"));
		public string SampleText{
			get{
				return (string)this.GetValue(SampleTextProperty);
			}
			set{
				this.SetValue(SampleTextProperty, value);
			}
		}

		public Font SelectedFont{
			get{
				return new Font(this.SelectedFontFamily, this.SelectedFontSize, this.SelectedFontStyle, this.SelectedFontWeight, this.SelectedFontStretch);
			}
			set{
				value.ThrowIfNull();
				this.SelectedFontFamily = value.Family;
				this.SelectedFontSize = value.Size;
				this.SelectedFontStyle = value.Style;
				this.SelectedFontWeight = value.Weight;
			}
		}

		#endregion
	}
}
