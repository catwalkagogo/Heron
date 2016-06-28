using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CatWalk.ComponentModel;

namespace CatWalk.Mvvm {
	public class SynchronizeViewModel : ViewModelBase{
		private SynchronizationContext _Invoker;
		//private IDictionary<string, IAsyncResult> PropertyChangedPool = new Dictionary<string, IAsyncResult>();
		public SynchronizationContext SynchronizationContext {
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
			: this(DefaultSynchronizationContext.Default) {

		}

		public SynchronizeViewModel(SynchronizationContext invoker) {
			invoker.ThrowIfNull("invoker");
			this._Invoker = invoker;
		}

		public SynchronizationContext Invoker {
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
			this._Invoker.Post(new SendOrPostCallback((state) => {
				base.OnPropertyChanged((PropertyChangedEventArgs)e);
			}), e);
		}		

		public class DefaultSynchronizationContext : SynchronizationContext {
			private DefaultSynchronizationContext() { }

			private static Lazy<DefaultSynchronizationContext> _Default = new Lazy<DefaultSynchronizationContext>(() => {
				return new DefaultSynchronizationContext();
			});
			public static DefaultSynchronizationContext Default {
				get {
					return _Default.Value;
				}
			}

			public override void Post(SendOrPostCallback d, object state) {
				d(state);
			}

			public override void Send(SendOrPostCallback d, object state) {
				d(state);
			}

			public override void OperationCompleted() {
			}

			public override void OperationStarted() {
			}
		}
	}
}
