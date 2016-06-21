using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CatWalk.Mvvm;

namespace CatWalk.Heron.Windows {
	public static class MessageUtility {
		public static DataContextMessageReceiver<TMessage> RegisterMessageReceiver<TMessage>(this FrameworkElement element, Messenger messenger, Action<TMessage> receiver, bool receiveDeliveredMessage = false) {
			element.ThrowIfNull("element");
			messenger.ThrowIfNull("messenger");
			receiver.ThrowIfNull("receiver");

			return new DataContextMessageReceiver<TMessage>(element, messenger, receiver, receiveDeliveredMessage);
		}

		/*
		public static DataContextMessageReceiver<TMessage, TState> RegisterMessageReceiver<TMessage, TState>(this FrameworkElement element, Messenger messenger, Action<TMessage, TState> receiver, TState state, bool receiveDeliveredMessage = false) {
			element.ThrowIfNull("element");
			messenger.ThrowIfNull("messenger");
			receiver.ThrowIfNull("receiver");

			return new DataContextMessageReceiver<TMessage, TState>(element, messenger, receiver, state, receiveDeliveredMessage);
		}

		public static DataContextMessageReceiver<TMessage, TState> RegisterMessageReceiver<TMessage, TState>(this FrameworkElement element, Messenger messenger, Action<TMessage, object, TState> receiver, TState state, bool receiveDeliveredMessage = false) {
			element.ThrowIfNull("element");
			messenger.ThrowIfNull("messenger");
			receiver.ThrowIfNull("receiver");

			return new DataContextMessageReceiver<TMessage, TState>(element, messenger, receiver, state, receiveDeliveredMessage);
		}
		*/
	}

	public class DataContextMessageReceiver<TMessage> : MessageReceiver<TMessage> {
		public FrameworkElement Element { get; private set; }

		public DataContextMessageReceiver(FrameworkElement element, Messenger messenger, Action<TMessage> receiver, bool receiveDeliveredMessage = false)
			: base(messenger, receiver, receiveDeliveredMessage) {
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
	/*
	public class DataContextMessageReceiver<TMessage, TState> : MessageReceiver<TMessage, TState> {
		public FrameworkElement Element { get; private set; }

		public DataContextMessageReceiver(FrameworkElement element, Messenger messenger, Action<TMessage, TState> receiver, TState state, bool receiveDeliveredMessage = false)
			: this(element, messenger, receiver, state, receiveDeliveredMessage, false){
		}

		public DataContextMessageReceiver(FrameworkElement element, Messenger messenger, Action<TMessage, object, TState> receiver, TState state, bool receiveDeliveredMessage = false)
			: this(element, messenger, receiver, state, receiveDeliveredMessage, true) {
		}

		protected DataContextMessageReceiver(FrameworkElement element, Messenger messenger, Delegate receiver, TState state, bool receiveDeliveredMessage, bool receiveToken)
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
