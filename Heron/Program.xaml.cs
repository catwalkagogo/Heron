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

			public WindowsApplication(System.Windows.Application app) : base(new DispatcherSynchronizationContext(app.Dispatcher)) {
				this._App = app;

				this._App.Exit += _App_Exit;
				this._App.SessionEnding += _App_SessionEnding;
			}

			private IEnumerable<string> GetBuiltinPlugins() {
				var builtinPluginDir = new FilePath(Assembly.GetEntryAssembly().Location, FilePathFormats.Windows).Resolve("../plugins").FullPath;
				var dlls = Directory.EnumerateFiles(builtinPluginDir, "*.dll", SearchOption.AllDirectories);
				var asmNames = dlls.Select(dll => Tuple.Create(dll, AssemblyName.GetAssemblyName(dll).FullName)).Distinct(item => item.Item2);

				var loadedAsmNames = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(asm => asm.GetReferencedAssemblies().Concat(Seq.Make(asm.GetName())))
					.Select(asmName => asmName.FullName)
					.Distinct()
					.ToLookup(asmName => asmName);
				var asmsToLoad = asmNames.Where(asmName => !loadedAsmNames.Contains(asmName.Item2)).Distinct(pair => pair.Item2).ToArray();
				/*
				var loadedNames = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(asm => asm.GetReferencedAssemblies().Concat(Seq.Make(asm.GetName())))
					.Select(asmNAme => asmNAme.FullName)
					.Distinct().OrderBy(name => name).ToArray();
				var asmNames2 = dlls.Select(dll => AssemblyName.GetAssemblyName(dll).FullName).OrderBy(name => name).ToArray();
				*/
				var pluginAsms = new List<string>();
				asmsToLoad.ForEach(asmName => {
					var asm = Assembly.LoadFile(asmName.Item1);
					if (asm.IsPluginAssembly()) {
						pluginAsms.Add(asmName.Item1);
					}
				});
				return pluginAsms;
			}

			private void _App_SessionEnding(object sender, SessionEndingCancelEventArgs e) {
				this.OnSessionEnding(e);
			}

			private void _App_Exit(object sender, System.Windows.ExitEventArgs e) {
				this.OnExit(new ApplicationExitEventArgs(e.ApplicationExitCode));
			}

			protected override Task OnFirstStartUp(ApplicationStartUpEventArgs e) {
				return base.OnFirstStartUp(e).ContinueWith(task => {
					this.CreateMainWindow();
				}, TaskScheduler.FromCurrentSynchronizationContext());
			}

			protected override void ExitApplication(ApplicationExitEventArgs e) {
				this._App.Shutdown(e.ApplicationExitCode);
			}

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

			protected override IScriptingHost GetScriptingHost() {
				var host = new ClearScriptHost();
				return host;
			}

			protected override async Task InitializePlugin() {
				await base.InitializePlugin();

				AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

				var plugins = this.GetBuiltinPlugins();
				plugins.Select(file => AppDomain.CurrentDomain.Load(file)).ForEach(this.PluginManager.RegisterAssembly);
				this.PluginManager.RestoreEnabledPluginsFromConfiguration();
			}

			private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
				/*var asm1 = AppDomain.CurrentDomain.GetAssemblies().Where(_ => _.GetName().FullName == args.Name).FirstOrDefault();
				if(asm1 != null) {
					return asm1;
				}*/
				if(args.RequestingAssembly == null) {
					return Assembly.LoadFile(args.Name);
				}

				var location = args.RequestingAssembly.Location;
				var dir = new FilePath(location, FilePathFormats.Windows).Resolve("..").FullPath;
				var asm = System.IO.Directory.EnumerateFiles(dir, "*.dll")
					.Select(file => Tuple.Create(file, AssemblyName.GetAssemblyName(file)))
					.Where(name => name.Item2.FullName == args.Name)
					.FirstOrDefault();

				return asm != null ? Assembly.LoadFile(asm.Item1) : null;
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
		}
	}
}
