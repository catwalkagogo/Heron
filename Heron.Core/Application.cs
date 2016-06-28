using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using CatWalk.Heron.Configuration;
using CatWalk.Heron.Scripting;
using CatWalk.Heron.ViewModel;
using CatWalk.IO;

namespace CatWalk.Heron {
	public abstract partial class Application : ControlViewModel, IJobManagerSite {
		private Lazy<IStorage> _Configuration;
		public abstract FilePath ConfigurationFilePath { get; }
		//private Lazy<Logger> _Logger = new Lazy<Logger>(() => LogManager.GetCurrentClassLogger());

		#region static

		private static Application _Current;
		private static readonly object _SyncObject = new object();

		public static Application Current {
			get {
				return _Current;
			}
		}

		#endregion

		public Application(SynchronizationContext context) : base(null, context) {

		}

		#region Run

		public void Run() {
			this.Run(new string[0]);
		}

		public void Run(IReadOnlyList<string> args) {
			args.ThrowIfNull("args");
			lock(_SyncObject) {
				if(_Current != null) {
					throw new InvalidOperationException("Application is already running.");
				}
				_Current = this;
			}
			this.OnStartup(new ApplicationStartUpEventArgs(args));
		}

		#endregion

		#region Property

		public IStorage Configuration {
			get {
				return this._Configuration.Value;
			}
		}

		#endregion

		#region StartUp

		protected abstract bool IsFirst {
			get;
		}

		protected virtual void OnStartup(ApplicationStartUpEventArgs e) {
			if(this.IsFirst) {
				this.OnFirstStartUp(e);
			} else {
				this.OnSecondStartUp(e);
			}

			var handler = this.StartUp;
			if(handler != null) {
				handler(this, e);
			}
		}

		protected abstract IStorage GetStorage();

		protected virtual Task OnFirstStartUp(ApplicationStartUpEventArgs e) {
			return Task.Run(() => {
				// Configuration初期化
				this._Configuration = new Lazy<IStorage>(() => {
					return this.GetStorage();
				});
				var conf = this._Configuration.Value;
			}).ContinueWith(task => {
				Task.WaitAll(
					this.InitializeScripting(),
					this.InitializeIOSystem(),
					this.InitializeViewModel(),
					this.InitializePlugin());
			});
		}

		protected virtual void OnSecondStartUp(ApplicationStartUpEventArgs e) {
			this.Shutdown();
		}

		public event EventHandler<ApplicationStartUpEventArgs> StartUp;

		#endregion

		#region Exit

		protected virtual void OnExit(ApplicationExitEventArgs e) {
			if(this._Configuration.IsValueCreated) {
				this._Configuration.Value.Dispose();
			}
			lock(_SyncObject) {
				_Current = null;
			}
			var handler = this.Exit;
			if(handler != null) {
				handler(this, e);
			}

			this.ExitApplication(e);
		}

		public void Shutdown(int exitCode = 0) {
			this.OnExit(new ApplicationExitEventArgs(exitCode));
		}

		public event EventHandler<ApplicationExitEventArgs> Exit;

		protected abstract void ExitApplication(ApplicationExitEventArgs e);

		#endregion

		#region SessionEnding

		protected virtual void OnSessionEnding(CancelEventArgs e) {
			var handler = this.SessionEnding;
			if(handler != null) {
				handler(this, e);
			}
		}

		public event EventHandler<CancelEventArgs> SessionEnding;

		#endregion

		protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
			if(e.PropertyName == "SynchronizationContext") {
				this._Messenger.SynchronizationContext = this.SynchronizationContext;
			}
			base.OnPropertyChanged(e);
		}

		private const string AP_CommandLine = "_CommandLine";

	}

	public class ViewFactory{
		private Factory<Type, object> _Factory = new Factory<Type, object>();

		public void Register<T>(Func<Type, T, object> d) {
			this._Factory.Register(typeof(T), d);
		}
		public object Create(object vm) {
			vm.ThrowIfNull("vm");
			return this._Factory.Create(vm.GetType(), vm);
		}
	}

	public class ApplicationStartUpEventArgs : EventArgs {
		public IReadOnlyList<string> Args { get; private set; }

		public ApplicationStartUpEventArgs(IReadOnlyList<string> args) {
			args.ThrowIfNull("args");
			this.Args = args;
		}
	}

	public class ApplicationExitEventArgs : EventArgs {
		public int ApplicationExitCode { get; set; }

		public ApplicationExitEventArgs(int exitCode = 0){
			this.ApplicationExitCode = exitCode;
		}
	}
}
