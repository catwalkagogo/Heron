using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Mvvm;

namespace CatWalk.Mvvm {
	public class MessageReceiver<TMessage> : DisposableObject {
		private object _Token;
		protected Messenger Messenger { get; private set; }
		protected Delegate Receiver { get; private set; }
		protected bool IsReceiveDeliveredMessage { get; private set; }

		public MessageReceiver(Messenger messenger, Action<TMessage> receiver, bool receiveDeliveredMessage) {
			messenger.ThrowIfNull("messenger");
			receiver.ThrowIfNull("receiver");

			this.Messenger = messenger;
			this.Receiver = receiver;
			this.IsReceiveDeliveredMessage = receiveDeliveredMessage;
		}

		public object Token {
			get {
				return this._Token;
			}
			set {
				if(this._Token != null) {
					this.Unregister(this._Token);
				}

				this._Token = value;

				if(this._Token != null) {
					this.Register(this._Token);
				}
			}
		}

		protected virtual void Register(object context) {
			this.Messenger.Register<TMessage>((Action<TMessage>)this.Receiver, context, this.IsReceiveDeliveredMessage);
		}

		protected virtual void Unregister(object context) {
			this.Messenger.Unregister<TMessage>((Action<TMessage>)this.Receiver, context, this.IsReceiveDeliveredMessage);
		}


		protected override void Dispose(bool disposing) {
			if(!this.IsDisposed) {
				if(this.Token != null) {
					this.Unregister(this.Token);
				}
			}
			base.Dispose(disposing);
		}
	}
	/*

	public class MessageReceiver<TMessage, TState> : MessageReceiver<TMessage> {
		public TState State { get; private set; }

		public MessageReceiver(Messenger messenger, Action<TMessage, TState> receiver, TState state, bool receiveDeliveredMessage)
			: this(messenger, receiver, state, receiveDeliveredMessage, false){
		}

		public MessageReceiver(Messenger messenger, Action<TMessage, object, TState> receiver, TState state, bool receiveDeliveredMessage)
			: this(messenger, receiver, state, receiveDeliveredMessage, true) {
		}

		protected MessageReceiver(Messenger messenger, Delegate receiver, TState state, bool receiveDeliveredMessage, bool receiveToken)
			: base(messenger, receiver, receiveDeliveredMessage, receiveToken) {
				this.State = state;
		}

		protected override void Register(object context) {
			if(this.IsReceiveToken) {
				this.Messenger.Register<TMessage, TState>((Action<TMessage, object, TState>)this.Receiver, this.State, context, this.IsReceiveDeliveredMessage);
			} else {
				this.Messenger.Register<TMessage, TState>((Action<TMessage, TState>)this.Receiver, this.State, context, this.IsReceiveDeliveredMessage);
			}
		}

		protected override void Unregister(object context) {
			if(this.IsReceiveToken) {
				this.Messenger.Unregister<TMessage, TState>((Action<TMessage, object, TState>)this.Receiver, context, this.IsReceiveDeliveredMessage);
			} else {
				this.Messenger.Unregister<TMessage, TState>((Action<TMessage, TState>)this.Receiver, context, this.IsReceiveDeliveredMessage);
			}
		}
	}
	*/
}
