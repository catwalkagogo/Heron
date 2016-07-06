using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Threading.Tasks;
using CatWalk.Heron.Configuration;
using CatWalk.Heron.Scripting;
using CatWalk.Heron.ViewModel;
using CatWalk.Heron.ViewModel.Windows;
using CatWalk.IO;
using System.Reflection;
using System.IO;
using Community.CsharpSqlite;
using Community.CsharpSqlite.SQLiteClient;

namespace CatWalk.Heron {
	using IO = System.IO;
	public partial class Program : System.Windows.Application {
		protected override void OnStartup(StartupEventArgs e) {
			var app = new WindowsApplication(this);
			app.Run(e.Args);
		}

		/// <summary>
		/// WPFのAppとVMをバインディングする
		/// </summary>
		private class WindowsApplication : Application {
			private System.Windows.Application _App;

			#region Property

			protected override bool IsFirst {
				get {
					return CatWalk.Win32.ApplicationProcess.IsFirst;
				}
			}

			public override FilePath ConfigurationFilePath {
				get {
					return new FilePath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), FilePathFormats.Windows).Resolve("Heron");
				}

			}

			#endregion

			#region Bridge WPF App

			public WindowsApplication(System.Windows.Application app) : base(new DispatcherSynchronizationContext(app.Dispatcher)) {
				this._App = app;

				this._App.Exit += _App_Exit;
				this._App.SessionEnding += _App_SessionEnding;
			}

			private void _App_SessionEnding(object sender, SessionEndingCancelEventArgs e) {
				this.OnSessionEnding(e);
			}

			private void _App_Exit(object sender, System.Windows.ExitEventArgs e) {
				this.OnExit(new ApplicationExitEventArgs(e.ApplicationExitCode));
			}

			protected override void ExitApplication(ApplicationExitEventArgs e) {
				this._App.Shutdown(e.ApplicationExitCode);
			}

			#endregion

			#region Startup

			protected override Task OnFirstStartUp(ApplicationStartUpEventArgs e) {
				return base.OnFirstStartUp(e).ContinueWith(task => {
					var mainWindow = new MainWindowViewModel(this);
					mainWindow.Panels.AddPanel(null);
					mainWindow.Panels.AddPanel(null);
				}, TaskScheduler.FromCurrentSynchronizationContext());
			}

			#endregion

			#region Storage

			protected override IStorage GetStorage() {
				var connBldr = new SqliteConnectionStringBuilder();
				connBldr.DataSource = this.ConfigurationFilePath.Resolve("app.db").FullPath;
				connBldr.Version = 3;

				Directory.CreateDirectory(this.ConfigurationFilePath.FullPath);
				using (File.AppendText(connBldr.DataSource)) {

				}

				return new CachedStorage(
					256,
					new DBStorage(
						SqliteClientFactory.Instance,
						connBldr.ConnectionString,
						"configuration"));
			}

			#endregion

			#region Scripting

			protected override IScriptingHost GetScriptingHost() {
				var host = new ClearScriptHost();
				return host;
			}


			private void ExecuteScripts() {
				try {
					var scriptPath = this.ConfigurationFilePath.Resolve("scripts");
					IO::Directory.CreateDirectory(scriptPath.FullPath);
					var host = this.ScriptingHost;

					foreach (var file in IO::Directory.EnumerateFiles(scriptPath.FullPath, "*", IO::SearchOption.AllDirectories)) {
						if (host.IsSupportedFileExtension(IO::Path.GetExtension(file))) {
							try {
								host.ExecuteFile(file);
							} catch (Exception ex) {
								//this._Logger.Value.Warn(ex, "Script Error");
							}
						}
					}
				} catch (IO::IOException ex) {
					//this._Logger.Value.Warn(ex, "Script IO Error");
				}
			}

			#endregion

			#region Plugin

			protected override async Task InitializePlugin() {
				await base.InitializePlugin();

				AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

				this.PluginManager.Plugins.ForEach(p => p.Load(this));
			}

			protected override IPluginManager GetPluginManager() {
				return new PluginManager();
			}

			private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
				var asm1 = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(_ => _.GetName().FullName == args.Name);
				if(asm1 != null) {
					return asm1;
				}
				if(args.RequestingAssembly == null) {
					return Assembly.LoadFile(args.Name);
				}

				var location = args.RequestingAssembly.Location;
				var dir = new FilePath(location, FilePathFormats.Windows).Resolve("..").FullPath;
				var asm = System.IO.Directory.EnumerateFiles(dir, "*.dll")
					.Select(file => Tuple.Create(file, AssemblyName.GetAssemblyName(file)))
					.FirstOrDefault(name => name.Item2.FullName == args.Name);

				return asm != null ? Assembly.LoadFile(asm.Item1) : null;
			}

			#endregion
		}
	}
}
