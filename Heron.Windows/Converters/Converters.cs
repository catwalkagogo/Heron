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
	using System.Globalization;
	using System.Windows.Markup;
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

	public class WindowStateConverter : GenericTypeConverter<WindowState, CatWalk.Heron.ViewModel.Windows.WindowState> {
	}

	/*
	public class GenericFactory : MarkupExtension {
		[ConstructorArgument("genericType")]
		public Type GenericType { get; private set; }
		[ConstructorArgument("types")]
		public Type[] Types { get; private set; }

		public GenericFactory(Type genericType, Type[] types) {
			genericType.ThrowIfNull("genericType");
			types.ThrowIfNull("type1");
			this.GenericType = genericType;
			this.Types = types;
		}

		public override object ProvideValue(IServiceProvider serviceProvider) {
			Type type = this.GenericType.MakeGenericType(this.Types);
			return Activator.CreateInstance(type);
		}
	}*/

	public class GenericTypeConverter<T1, T2> : IValueConverter {
		#region IValueConverter
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is T1 && targetType == typeof(T2)) {
				var state = (T2)value;
				return state;
			} else if (value is T2 && targetType == typeof(T1)) {
				var state = (T1)value;
				return state;
			} else {
				return value;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return this.Convert(value, targetType, parameter, culture);
		}

		#endregion
	}


	public class RectToGenericRectConverter : IValueConverter {
		#region IValueConverter
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is Rect) {
				var rect = (Rect)value;
				if (targetType == typeof(Rect<double>)) {
					return new Rect<double>(rect.X, rect.Y, rect.Width, rect.Height);
				} else if (targetType == typeof(Rect<int>)) {
					return new Rect<int>((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
				} else if (targetType == typeof(Rect<long>)) {
					return new Rect<long>((long)rect.X, (long)rect.Y, (long)rect.Width, (long)rect.Height);
				} else if (targetType == typeof(Rect<float>)) {
					return new Rect<float>((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
				} else if (targetType == typeof(Rect<decimal>)) {
					return new Rect<decimal>((decimal)rect.X, (decimal)rect.Y, (decimal)rect.Width, (decimal)rect.Height);
				} else {
					throw new ArgumentException("targetType");
				}
			}else if(targetType == typeof(Rect)) {
				return this.ConvertBack(value, targetType, parameter, culture);
			}else {
				throw new ArgumentException("value");
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(Rect)) {
				if (value is Rect<double>) {
					var rect = (Rect<double>)value;
					return new Rect((double)rect.X, (double)rect.Y, (double)rect.Width, (double)rect.Height);
				} else if (value is Rect<int>) {
					var rect = (Rect<int>)value;
					return new Rect((double)rect.X, (double)rect.Y, (double)rect.Width, (double)rect.Height);
				} else if (value is Rect<long>) {
					var rect = (Rect<long>)value;
					return new Rect((double)rect.X, (double)rect.Y, (double)rect.Width, (double)rect.Height);
				} else if (value is Rect<float>) {
					var rect = (Rect<float>)value;
					return new Rect((double)rect.X, (double)rect.Y, (double)rect.Width, (double)rect.Height);
				} else if (value is Rect<decimal>) {
					var rect = (Rect<decimal>)value;
					return new Rect((double)rect.X, (double)rect.Y, (double)rect.Width, (double)rect.Height);
				} else {
					throw new ArgumentException("value");
				}
			}else if(value is Rect) {
				return this.Convert(value, targetType, parameter, culture);
			} else {
				throw new ArgumentException("targetType");
			}
		}

		#endregion
	}

	public class FactoryConverter : DependencyObject, IValueConverter {
		public Factory Factory {
			get { return (Factory)GetValue(FactoryProperty); }
			set { SetValue(FactoryProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Factory.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty FactoryProperty =
			DependencyProperty.Register("Factory", typeof(Factory), typeof(FactoryConverter), new PropertyMetadata(null));



		public FactoryConverter() {
		}

		#region IValueConverter
		public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return this.Factory.Create(new object[]{ value });
		}

		public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
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