using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CatWalk.Mvvm;

namespace CatWalk.Heron.Windows {
	/*
	public class FrameworkMessageReceiver<TMessage> : MessageReceiver<TMessage> {
		public FrameworkElement Element { get; private set; }

		public FrameworkMessageReceiver(FrameworkElement element, Messenger messenger, Action<TMessage> receiver, bool receiveDeliveredMessage = false)
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
			if (!this.IsDisposed) {
				this.Element.DataContextChanged -= this.Element_DataContextChanged;
			}
			base.Dispose(disposing);
		}
	}*/
}
