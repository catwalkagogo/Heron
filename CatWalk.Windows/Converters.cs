/*
	$Id: Converters.cs 318 2013-12-27 15:57:19Z catwalkagogo@gmail.com $
*/
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Markup;
using System.Threading;
using System.Globalization;

namespace CatWalk.Windows {
	public class FontFamilyNameConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var family = (FontFamily)value;
			if(family != null) {
				var lang = XmlLanguage.GetLanguage(culture.IetfLanguageTag);
				return
					(family.FamilyNames.ContainsKey(lang) ? family.FamilyNames[lang] : null) ??
					family.FamilyNames.Values.FirstOrDefault() ??
					family.ToString();
			} else {
				return null;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var name = (string)value;
			return new FontFamily(name);
		}
	}
}
