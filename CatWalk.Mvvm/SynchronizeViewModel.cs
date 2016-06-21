using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CatWalk.ComponentModel;

namespace CatWalk.Mvvm {
	public class SynchronizeViewModel : ViewModelBase{
		private ISynchronizeInvoke _Invoker;
		//private IDictionary<string, IAsyncResult> PropertyChangedPool = new Dictionary<string, IAsyncResult>();
		public ISynchronizeInvoke SynchronizeInvoke {
			get {
				return this._Invoker;
			}
			set {
				value.ThrowIfNull("value");
				this._Invoker = value;
				this.OnPropertyChanged("SynchronizeInvoke");
			}
		}

		public SynchronizeViewModel()
			: this(new DefaultSynchronizeInvoke()) {

		}

		public SynchronizeViewModel(ISynchronizeInvoke invoker) {
			invoker.ThrowIfNull("invoker");
			this._Invoker = invoker;
		}

		public ISynchronizeInvoke Invoker {
			get {
				return this._Invoker;
			}
			set {
				this._Invoker = value;
			}
		}

		/*
		protected override void OnPropertyChanging(PropertyChangingEventArgs e) {
			if(this._Invoker.InvokeRequired) {
				this._Invoker.BeginInvoke(new Action<PropertyChangingEventArgs>(this.InvokeOnPropertyChanging), new object[]{e});
			} else {
				base.OnPropertyChanging(e);
			}
		}

		private void InvokeOnPropertyChanging(PropertyChangingEventArgs e) {
			base.OnPropertyChanging(e);
		}
		*/
		
		protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
			if(this._Invoker.InvokeRequired) {
				IAsyncResult result;
				/*if(this.PropertyChangedPool.TryGetValue(e.PropertyName, out result)) {
					if(!result.IsCompleted) {
						return;
					}
				}*/
				result = this._Invoker.BeginInvoke(new Action<PropertyChangedEventArgs>(this.InvokeOnPropertyChanged), new object[] { e });
				//this.PropertyChangedPool[e.PropertyName] = result;
			} else {
				base.OnPropertyChanged(e);
			}
		}
		
		private void InvokeOnPropertyChanged(PropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
		}

		public class DefaultSynchronizeInvoke : ISynchronizeInvoke {
			public System.IAsyncResult BeginInvoke(System.Delegate method, object[] args) {
				return new AsyncResult(method.DynamicInvoke(method, args));
			}

			public object EndInvoke(System.IAsyncResult result) {
				return result.AsyncState;
			}

			public object Invoke(System.Delegate method, object[] args) {
				return method.DynamicInvoke(args);
			}

			public bool InvokeRequired {
				get {
					return false;
				}
			}

			private class AsyncResult : IAsyncResult {
				public AsyncResult(object state) {
					this.AsyncState = state;
				}

				public bool IsCompleted {
					get {
						return true;
					}
				}

				public bool CompletedSynchronously {
					get {
						return true;
					}
				}

				public object AsyncState { get; private set; }

				private Lazy<WaitHandle> _AsyncWaitHandle = new Lazy<WaitHandle>(() => new EventWaitHandle(true, EventResetMode.ManualReset));
				public WaitHandle AsyncWaitHandle {
					get {
						return this._AsyncWaitHandle.Value;
					}
				}
			}
		}
	}
}
