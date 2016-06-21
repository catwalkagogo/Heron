using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;

namespace CatWalk.Windows.Input{
	[TypeConverter(typeof(MergedInputGestureConverter))]
	public class MergedInputGesture : InputGesture{
		public MergedInputGesture(IEnumerable<InputGesture> inputGestures){
			var array = inputGestures.ToArray();
			this._InputGestures = new List<InputGesture>(array.Length);
			this._InputGestures.AddRange(array);
		}

		private List<InputGesture> _InputGestures;
		private ReadOnlyCollection<InputGesture> _ReadOnlyInputGestures;
		public ReadOnlyCollection<InputGesture> InputGestures{
			get{
				if(this._ReadOnlyInputGestures == null){
					this._ReadOnlyInputGestures = new ReadOnlyCollection<InputGesture>(this._InputGestures);
				}
				return this._ReadOnlyInputGestures;
			}
		}

		public override bool Matches(object targetElement, InputEventArgs inputEventArgs){
			var isMatch = false;
			foreach(var gesture in this._InputGestures){
				isMatch = isMatch || gesture.Matches(targetElement, inputEventArgs);
			}
			return isMatch;
		}
	}

	public class MergedInputGestureConverter : TypeConverter{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(MergedInputGesture);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(value.GetType() == typeof(MergedInputGesture)){
				return GetString((MergedInputGesture)value);
			}else{
				return null;
			}
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string)){
				return GetString((MergedInputGesture)value);
			}else{
				return null;
			}
		}

		private static string GetString(MergedInputGesture mergedGesture){
			var list = new List<string>();
			foreach(var gesture in mergedGesture.InputGestures){
				var converter = TypeDescriptor.GetConverter(gesture.GetType());
				list.Add((string)converter.ConvertTo(gesture, typeof(string)));
			}
			return String.Join("; ", list);
		}
	}
}
