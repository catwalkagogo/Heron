using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;

namespace CatWalk.Windows.Input{
	public class MergedInputBinding : InputBinding{
		public MergedInputBinding(){
		}

		public MergedInputBinding(ICommand command, IEnumerable<InputGesture> gestures){
			this.Command = command;
			this.InputGestures = new List<InputGesture>(gestures);
		}


		public static readonly DependencyProperty InputGesturesProperty =
			DependencyProperty.Register("InputGestures", typeof(ICollection<InputGesture>), typeof(MergedInputBinding),
				new PropertyMetadata(InputGestures_Changed));
		public ICollection<InputGesture> InputGestures{
			get{
				return (ICollection<InputGesture>)this.GetValue(InputGesturesProperty);
			}
			set{
				this.SetValue(InputGesturesProperty, value);
			}
		}

		private static void InputGestures_Changed(DependencyObject obj, DependencyPropertyChangedEventArgs e){
			var self = (MergedInputBinding)obj;
			self.Gesture = new MergedInputGesture((IEnumerable<InputGesture>)e.NewValue);
		}
	}
}
