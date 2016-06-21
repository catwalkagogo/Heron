using System;
using System.ComponentModel;
using System.Windows.Input;

namespace CatWalk.Windows.Input {
	//I have NOT fleshed this class out fully - just enough to get this demo working
	public class MultiKeyBinding : InputBinding {
		[TypeConverter(typeof(MultiKeyGestureConverter))]
		public override InputGesture Gesture {
			get {
				return base.Gesture as MultiKeyGesture;
			}
			set {
				if(!(value is MultiKeyGesture)) {
					throw new ArgumentException();
				}

				base.Gesture = value;
			}
		}
	}
}