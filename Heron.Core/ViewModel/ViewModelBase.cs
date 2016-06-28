using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using CatWalk;
using CatWalk.Mvvm;
using System.Reactive.Disposables;

namespace CatWalk.Heron.ViewModel {
	public class ViewModelBase : SynchronizeViewModel, IDisposable {
		protected CompositeDisposable Disposables { get; private set; }

		public ViewModelBase() : base() {
			this.Disposables = new CompositeDisposable();
		}

		public ViewModelBase(SynchronizationContext invoker) : base(invoker) {
			this.Disposables = new CompositeDisposable();
		}

		#region IDisposable Members

		protected void ThrowIfDisposed() {
			throw new ObjectDisposedException("this");
		}

		protected bool IsDisposed { get; private set; }

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			this.IsDisposed = true;
		}

		~ViewModelBase() {
			this.Dispose(false);
		}

		#endregion
	}
}
