using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CatWalk.Windows {
	public class DelegateWeakEventManager<TEventSource, TEventArgs> : WeakEventManager where TEventArgs : EventArgs{
		private Action<TEventSource, EventHandler<TEventArgs>> _AddEventHandler;
		private Action<TEventSource, EventHandler<TEventArgs>> _RemoveEventHandler;

		public DelegateWeakEventManager(Action<TEventSource, EventHandler<TEventArgs>> addEventHandler, Action<TEventSource, EventHandler<TEventArgs>> removeEventHandler) {
			addEventHandler.ThrowIfNull("addEventHandler");
			removeEventHandler.ThrowIfNull("removeEventHandler");
			this._AddEventHandler = addEventHandler;
			this._RemoveEventHandler = removeEventHandler;
		}

		protected override void StartListening(object source) {
			TEventSource evSource = (TEventSource)source;
			this._AddEventHandler(evSource, new EventHandler<TEventArgs>(this.DeliverEvent));
		}

		protected override void StopListening(object source) {
			TEventSource evSource = (TEventSource)source;
			this._RemoveEventHandler(evSource, new EventHandler<TEventArgs>(this.DeliverEvent));
		}
	}
}
