using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using CatWalk.ComponentModel;

namespace CatWalk.Windows.Threading {
	public class DispatcherSynchronizeInvoke : ISynchronizeInvoke{
		private readonly Dispatcher _Dispatcher;
		public DispatcherPriority Priority { get; set; }

		private static Lazy<DispatcherSynchronizeInvoke> _ApplicationCurrent =
			new Lazy<DispatcherSynchronizeInvoke>(() => new DispatcherSynchronizeInvoke(Application.Current.Dispatcher));
		public static DispatcherSynchronizeInvoke ApplicationCurrent {
			get {
				return _ApplicationCurrent.Value;
			}
		}

		public DispatcherSynchronizeInvoke(Dispatcher dispatcher) {
			this._Dispatcher = dispatcher;
			this.Priority = DispatcherPriority.Normal;
		}

		public bool InvokeRequired {
			get {
				return !this._Dispatcher.CheckAccess();
			}
		}

		public System.IAsyncResult BeginInvoke(System.Delegate method, object[] args) {
			var op = this._Dispatcher.BeginInvoke(method, this.Priority, args);
			return new DispatcherOperationAsyncResult(op);
		}

		public object EndInvoke(System.IAsyncResult result) {
			result.AsyncWaitHandle.WaitOne();
			return result.AsyncState;
		}

		public object Invoke(System.Delegate method, object[] args) {
			return this._Dispatcher.Invoke(method, this.Priority, args);
		}

		private class DispatcherOperationAsyncResult : IAsyncResult, ICancellable {
			private DispatcherOperation Operation{get; set;}

			public DispatcherOperationAsyncResult(DispatcherOperation operation) {
				operation.ThrowIfNull("operation");
				this._AsyncWaitHandle = new Lazy<WaitHandle>(this.AsyncWaitHandleFactory);
				this.Operation = operation;
			}

			public bool IsCompleted {
				get {
					return this.Operation.Status == DispatcherOperationStatus.Completed;
				}
			}

			public bool CompletedSynchronously {
				get {
					return false;
				}
			}

			public object AsyncState {
				get {
					return this.Operation.Result;
				}
			}

			private WaitHandle AsyncWaitHandleFactory() {
				EventWaitHandle handle = new EventWaitHandle(false, EventResetMode.ManualReset);
				this.Operation.Completed += delegate {
					handle.Set();
				};
				this.Operation.Aborted += delegate {
					handle.Set();
				};
				return handle;
			}

			private Lazy<WaitHandle> _AsyncWaitHandle;
			public WaitHandle AsyncWaitHandle {
				get {
					return this._AsyncWaitHandle.Value;
				}
			}

			#region ICancellable
			public void Cancel() {
				this.Operation.Abort();
				this.IsCancellationRequested = true;
			}
			public bool IsCancellationRequested { get; private set; }
			public bool CanBeCanceled {
				get {
					return this.Operation.Status == DispatcherOperationStatus.Pending;
				}
			}
			public WaitHandle WaitHandle {
				get {
					return this.AsyncWaitHandle;
				}
			}
			#endregion
		}	
	}
}
