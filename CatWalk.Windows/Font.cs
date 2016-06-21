/*
	$Id: Font.cs 137 2010-12-19 10:01:30Z cs6m7y@bma.biglobe.ne.jp $
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Markup;

namespace CatWalk.Windows {
	//using WinForms = System.Windows.Forms;
	//using Drawing = System.Drawing;

	[Serializable]
	public struct Font{
		private string familyName;
		public double Size{get; set;}
		private string styleName;
		private string weightName;
		private string stretchName;
		[NonSerialized]
		private FontFamily family;
		[NonSerialized]
		private FontStyle style;
		[NonSerialized]
		private FontWeight weight;
		[NonSerialized]
		private FontStretch stretch;

		public string FamilyName{
			get{
				return this.familyName;
			}
			set{
				this.familyName = value;
				if(!this.familyName.IsNullOrEmpty()){
					var conv = new FontFamilyConverter();
					this.family = (FontFamily)conv.ConvertFromString(this.familyName);
				}
			}
		}

		public string StyleName{
			get{
				return this.styleName;
			}
			set{
				this.styleName = value;
				if(!this.styleName.IsNullOrEmpty()){
					var conv = new FontStyleConverter();
					this.style = (FontStyle)conv.ConvertFromString(this.styleName);
				}
			}
		}

		public string WeightName{
			get{
				return this.weightName;
			}
			set{
				this.weightName = value;
				if(!this.weightName.IsNullOrEmpty()){
					var conv = new FontWeightConverter();
					this.weight = (FontWeight)conv.ConvertFromString(this.weightName);
				}
			}
		}

		public string StretchName{
			get{
				return this.stretchName;
			}
			set{
				this.stretchName = value;
				if(!this.stretchName.IsNullOrEmpty()){
					var conv = new FontStretchConverter();
					this.stretch = (FontStretch)conv.ConvertFromString(this.stretchName);
				}
			}
		}

		[XmlIgnore]
		public FontFamily Family{
			get{
				return this.family;
			}
			set{
				var conv = new FontFamilyConverter();
				this.family = value;
				this.familyName = conv.ConvertToString(value);
			}
		}

		[XmlIgnore]
		public FontStyle Style{
			get{
				return this.style;
			}
			set{
				var conv = new FontStyleConverter();
				this.style = value;
				this.styleName = conv.ConvertToString(value);
			}
		}

		[XmlIgnore]
		public FontWeight Weight{
			get{
				return this.weight;
			}
			set{
				var conv = new FontWeightConverter();
				this.weight = value;
				this.weightName = conv.ConvertToString(value);
			}
		}

		[XmlIgnore]
		public FontStretch Stretch{
			get{
				return this.stretch;
			}
			set{
				var conv = new FontStretchConverter();
				this.stretch = value;
				this.stretchName = conv.ConvertToString(value);
			}
		}

		public Font(string familyName, double size, string styleName, string weightName, string stretchName) : this(){
			this.FamilyName = familyName;
			this.Size = size;
			this.StyleName = styleName;
			this.WeightName = weightName;
			this.StretchName = stretchName;
		}

		public Font(FontFamily family, double size) : this(family, size, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal){}
		public Font(FontFamily family, double size, FontStyle style) : this(family, size, style, FontWeights.Normal, FontStretches.Normal){}
		public Font(FontFamily family, double size, FontStyle style, FontWeight weight) : this(family, size, style, weight, FontStretches.Normal){}
		public Font(FontFamily family, double size, FontStyle style, FontWeight weight, FontStretch stretch) : this(){
			this.Family = family;
			this.Size = size;
			this.Style = style;
			this.Weight = weight;
			this.Stretch = stretch;
		}
		/*
		public static Font FromGdiFont(Drawing.Font font){
			return new Font(
				new FontFamily(font.FontFamily.Name),
				(double)font.Size,
				(font.Style == Drawing.FontStyle.Italic) ? FontStyles.Italic : FontStyles.Normal,
				font.Bold ? FontWeights.Bold : FontWeights.Normal);
		}
		*/
	}
}
