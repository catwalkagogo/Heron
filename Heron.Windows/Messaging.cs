using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CatWalk.Mvvm;

namespace CatWalk.Heron.Windows {
	public static partial class Messaging {
		public static FrameworkMessageReceiver<TMessage> RegisterMessageReceiver<TMessage>(this FrameworkElement element, Messenger messenger, Action<TMessage> receiver, bool receiveDeliveredMessage = false) {
			element.ThrowIfNull("element");
			messenger.ThrowIfNull("messenger");
			receiver.ThrowIfNull("receiver");

			return new FrameworkMessageReceiver<TMessage>(element, messenger, receiver, receiveDeliveredMessage);
		}

		public static bool GetIsCommunicateViewModelMessages(DependencyObject obj) {
			return (bool)obj.GetValue(IsCommunicateViewModelMessagesProperty);
		}

		public static void SetIsCommunicateViewModelMessages(DependencyObject obj, bool value) {
			obj.SetValue(IsCommunicateViewModelMessagesProperty, value);
		}

		// Using a DependencyProperty as the backing store for IsCommunicateViewModelMessages.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsCommunicateViewModelMessagesProperty =
			DependencyProperty.RegisterAttached("IsCommunicateViewModelMessages", typeof(bool), typeof(Messaging), new PropertyMetadata(false, (s, e) => {
				var elm = (FrameworkElement)s;
				if ((bool)e.OldValue) {
					elm.DataContextChanged -= IsCommunicateViewModelMessages_DataContextChanged;
				}
				if ((bool)e.NewValue) {
					elm.DataContextChanged += IsCommunicateViewModelMessages_DataContextChanged;
				}
			}));

		private static void IsCommunicateViewModelMessages_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if (e.NewValue != null) {
				Application.Current.Messenger.Send(new Messages.DataContextAttachedMessage(sender), e.NewValue);
			}
		}

		/*
		public static FrameworkMessageReceiver<TMessage, TState> RegisterMessageReceiver<TMessage, TState>(this FrameworkElement element, Messenger messenger, Action<TMessage, TState> receiver, TState state, bool receiveDeliveredMessage = false) {
			element.ThrowIfNull("element");
			messenger.ThrowIfNull("messenger");
			receiver.ThrowIfNull("receiver");

			return new FrameworkMessageReceiver<TMessage, TState>(element, messenger, receiver, state, receiveDeliveredMessage);
		}

		public static FrameworkMessageReceiver<TMessage, TState> RegisterMessageReceiver<TMessage, TState>(this FrameworkElement element, Messenger messenger, Action<TMessage, object, TState> receiver, TState state, bool receiveDeliveredMessage = false) {
			element.ThrowIfNull("element");
			messenger.ThrowIfNull("messenger");
			receiver.ThrowIfNull("receiver");

			return new FrameworkMessageReceiver<TMessage, TState>(element, messenger, receiver, state, receiveDeliveredMessage);
		}
		*/
	}

	
	/*
	public class FrameworkMessageReceiver<TMessage, TState> : MessageReceiver<TMessage, TState> {
		public FrameworkElement Element { get; private set; }

		public FrameworkMessageReceiver(FrameworkElement element, Messenger messenger, Action<TMessage, TState> receiver, TState state, bool receiveDeliveredMessage = false)
			: this(element, messenger, receiver, state, receiveDeliveredMessage, false){
		}

		public FrameworkMessageReceiver(FrameworkElement element, Messenger messenger, Action<TMessage, object, TState> receiver, TState state, bool receiveDeliveredMessage = false)
			: this(element, messenger, receiver, state, receiveDeliveredMessage, true) {
		}

		protected FrameworkMessageReceiver(FrameworkElement element, Messenger messenger, Delegate receiver, TState state, bool receiveDeliveredMessage, bool receiveToken)
			: base(messenger, receiver, state, receiveDeliveredMessage, receiveToken) {
			element.ThrowIfNull("element");

			this.Element = element;
			this.Token = element.DataContext;

			element.DataContextChanged += Element_DataContextChanged;
		}

		private void Element_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			this.Token = e.NewValue;
		}

		protected override void Dispose(bool disposing) {
			if(!this.IsDisposed) {
				this.Element.DataContextChanged -= this.Element_DataContextChanged;
			}
			base.Dispose(disposing);
		}
	}
	*/
}
