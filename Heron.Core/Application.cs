using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.IO;
using CatWalk.Heron.Configuration;
using CatWalk.Heron.Scripting;
using CatWalk.Heron.ViewModel;
using CatWalk.IO;
using CatWalk.Utils;
using Community.CsharpSqlite.SQLiteClient;
using NLog;

namespace CatWalk.Heron {
	public abstract partial class Application : ControlViewModel, IJobManagerSite {
		private Lazy<CachedStorage> _Configuration;
		private FilePath _ConfigurationFilePath;
		private Lazy<Logger> _Logger = new Lazy<Logger>(() => LogManager.GetCurrentClassLogger());

		#region static

		private static Application _Current;
		private static readonly object _SyncObject = new object();

		public static Application Current {
			get {
				return _Current;
			}
		}

		#endregion

		public Application(ISynchronizeInvoke invoke) : base(null, invoke) {

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

		public CommandLineOption StartUpOption { get; private set; }

		#endregion

		#region StartUp

		protected virtual void OnStartup(ApplicationStartUpEventArgs e) {
			if(ApplicationProcess.IsFirst) {
				this.RegisterRemoteCommands();
				this.OnFirstStartUp(e);
			} else {
				this.OnSecondStartUp(e);
			}

			var handler = this.StartUp;
			if(handler != null) {
				handler(this, e);
			}
		}

		protected virtual Task OnFirstStartUp(ApplicationStartUpEventArgs e) {
			return Task.Run(() => {
				// Configuration初期化
				this._ConfigurationFilePath = new FilePath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).Resolve("Heron");
				this._Configuration = new Lazy<CachedStorage>(() => {
					var connBldr = new SqliteConnectionStringBuilder();
					connBldr.DataSource = this._ConfigurationFilePath.Resolve("app.db").FullPath;
					connBldr.Version = 3;

					Directory.CreateDirectory(this._ConfigurationFilePath.FullPath);
					using (File.AppendText(connBldr.DataSource)) {

					}

					return new CachedStorage(
						256,
						new DBStorage(
							SqliteClientFactory.Instance,
							connBldr.ConnectionString,
							"configuration"));
				});
				var conf = this._Configuration.Value;
			}).ContinueWith(task => {
				this._ScriptingHosts = new Lazy<IList<IScriptingHost>>(() => {
					/*return AppDomain.CurrentDomain.GetAssemblies()
						.SelectMany(assm => assm.GetTypes()
							.Where(t => typeof(IScriptingHost).IsAssignableFrom(t)))
						.Select(t => Activator.CreateInstance(t) as IScriptingHost)
						.ToList();*/
					return Seq.Make<IScriptingHost>(new DlrHost()/*, new IronJSHost()*/).ToList();
				});

				var option = new CommandLineOption();
				var parser = new CommandLineParser();
				parser.Parse(option, e.Args);
				this.StartUpOption = option;

				this.InitializeIOSystem();
				this.InitializeViewModel();
				this.InitializePlugin();

				this.ExecuteScripts();
			});
		}

		protected virtual void OnSecondStartUp(ApplicationStartUpEventArgs e) {
			ApplicationProcess.InvokeRemote(AP_CommandLine, e.Args);
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

		protected virtual void OnSessionEnding(SessionEndingCancelEventArgs e) {
			var handler = this.SessionEnding;
			if(handler != null) {
				handler(this, e);
			}
		}

		public event SessionEndingCancelEventHandler SessionEnding;

		#endregion

		protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
			if(e.PropertyName == "SynchronizeInvoke") {
				this._Messenger.SynchronizeInvoke = this.SynchronizeInvoke;
			}
			base.OnPropertyChanged(e);
		}

		private const string AP_CommandLine = "_CommandLine";

		private void RegisterRemoteCommands() {
			ApplicationProcess.Actions.Add(AP_CommandLine, new Action<string[]>(prms => {
				var option = new CommandLineOption();
				var parser = new CommandLineParser();
				parser.Parse(option, prms);
			}));
		}

		public class CommandLineOption : Dictionary<string, string> {

		}
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
