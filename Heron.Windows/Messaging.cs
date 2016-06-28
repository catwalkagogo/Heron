using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CatWalk.Mvvm;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;

namespace CatWalk.Heron.Windows {
	public static partial class Messaging {
		public static IObservable<TMessage> ObserveDataContextMessage<TMessage>(this Messenger messenger, FrameworkElement element, bool isReceiveDerivedMessages = false) {
			messenger.ThrowIfNull("messenger");
			return messenger.ToObservableWithToken<TMessage>(null, isReceiveDerivedMessages)
				//.ObserveOnUIDispatcher()
				.Where(_ => _.Token == element.DataContext)
				.Select(_ => _.Message);
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
				Application.Current.Messenger.Send(new Messages.DataContextAttachedMessage(), e.NewValue);
			}
		}

	}

	
}
