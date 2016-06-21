/*
	$Id: Converters.cs 326 2014-01-09 10:15:01Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CatWalk;
using CatWalk.Win32;
using CatWalk.Win32.Shell;
using System.Reflection;

namespace CatWalk.Heron.Windows.Converters{
	using Gdi = System.Drawing;
	using GdiImaging = System.Drawing.Imaging;
	using IO = System.IO;

	public class ShellIconConverter : IValueConverter{
		#region IValueConverter Members

		private ImageList _ImageList = new ImageList(ImageListSize.Small);

		public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var file = (string)value;
			if(!String.IsNullOrEmpty(file)){
				try{
					int overlay;
					var idx = this._ImageList.GetIconIndexWithOverlay(file, out overlay);
					using(var bitmap = this._ImageList.Draw(idx, overlay, ImageListDrawOptions.Transparent)){
						var image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
						image.Freeze();
						return image;
					}
				}catch{
					Gdi::Icon icon = null;
					try{
						icon = ShellIcon.GetUnknownIconImage(IconSize.Small);
						var image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon.Handle, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
						image.Freeze();
						User32.DestroyIcon(icon.Handle);
						return image;
					}catch{
					}finally{
						if(icon != null){
							User32.DestroyIcon(icon.Handle);
						}
					}
				}
			}
			return null;
		}

		public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}

	public class ShellIconImageConverter : ShellIconConverter{
		#region IValueConverter Members

		public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var image = (ImageSource)base.Convert(value, targetType, parameter, culture);
			return new Image(){Source=image};
		}

		public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}
	
	public class AddConverter : IValueConverter{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var v = (double)value;
			var a = Double.Parse((string)parameter);
			return v + a;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var v = (double)value;
			var a = Double.Parse((string)parameter);
			return v - a;
		}

		#endregion
	}

	public class DoubleToPercentageConverter : IValueConverter{
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var d = (double)value;
			if(Double.IsNaN(d)){
				return "";
			}else{
				return Math.Round(d * 100) + "%";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			return (double)value / 100;
		}
		#endregion
	}

	public class InputBindingsToTextConverter : IValueConverter{
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var inputBindings = value as IEnumerable<InputBinding>;
			if(inputBindings != null){
				var list = new List<string>();
				foreach(var binding in inputBindings.Where(bind => bind.CommandParameter == parameter)){
					var converter = TypeDescriptor.GetConverter(binding.Gesture.GetType());
					list.Add((string)converter.ConvertTo(binding.Gesture, typeof(string)));
				}
				return String.Join(" / ", list);
			}else{
				return null;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}

	public class BoolToVisibilityConverter : IValueConverter{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value is bool){
				return (bool)value ? Visibility.Visible : Visibility.Collapsed;
			}else if(value is bool?){
				var bShow = (bool?)value;
				bool isShow = (bShow == null) ? !SystemParameters.IsGlassEnabled : bShow.Value;
				return isShow ? Visibility.Visible : Visibility.Collapsed;
			}else{
				return Visibility.Visible;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}

	public class NullableConverter : IValueConverter{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value == null){
				return parameter;
			}else{
				return value;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}

	public class FilePathConverter : IValueConverter {
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			var path = (string)value;
			switch((FilePathTransform)parameter){
				case FilePathTransform.FileName:
					return IO.Path.GetFileName(path);
				case FilePathTransform.DirectoryName:
					return IO.Path.GetDirectoryName(path);
				case FilePathTransform.ExtensionName:
					return IO.Path.GetExtension(path);
				case FilePathTransform.ExtensionNameWithoutDot:
					return IO.Path.GetExtension(path).TrimStart('.');
				case FilePathTransform.PathRoot:
					return IO.Path.GetPathRoot(path);
				default:
					return path;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
			throw new NotImplementedException();
		}

		#endregion
	}

	public class RecentFilesMenuItemConverter : IValueConverter{
		private static readonly char[] CharMap = new char[]{'1','2','3','4','5','6','7','8','9','0',
			'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var files = (string[])value;
			return files.EmptyIfNull().Select((file, idx) => new KeyValuePair<char, string>(CharMap[idx % CharMap.Length], file));
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}

	public class ScaleToScalingModeConverter : IValueConverter {
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var scale = (double)value;
			if(scale > 1){
				return BitmapScalingMode.NearestNeighbor;
			}else{
				return BitmapScalingMode.Fant;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}

	public class RoundNumberConverter : IValueConverter{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value is double){
				return Math.Round((double)value);
			}else if(value is float){
				return Math.Round((float)value);
			}else if(value is decimal){
				return Math.Round((decimal)value);
			}else{
				return value;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}

		#endregion
	}

	public class IsNaNConverter : IValueConverter {
		#region IValueConverter

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return Double.IsNaN((double)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}

	public class AlphaColorConverter : IValueConverter{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var color = (Color)value;
			return Color.FromArgb((byte)parameter, color.R, color.G, color.B);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}

	public enum FilePathTransform{
		None,
		FileName,
		DirectoryName,
		ExtensionName,
		ExtensionNameWithoutDot,
		PathRoot,
	}
}