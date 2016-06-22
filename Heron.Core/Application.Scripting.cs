using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CatWalk.Heron.Scripting;
using CatWalk.Heron.ViewModel;

namespace CatWalk.Heron {
	using IO = System.IO;
	public abstract partial class Application : ControlViewModel, IJobManagerSite {
		private Lazy<IScriptingHost> _ScriptingHost;

		public IScriptingHost ScriptingHost {
			get {
				return this._ScriptingHost.Value;
			}
		}

		private void InitializeScripting() {
			this._ScriptingHost = new Lazy<IScriptingHost>(() => {
				var host = new ClearScriptHost();

				return host;
			});

			var v = this._ScriptingHost.Value;

			this.Exit += Application_Scripting_Exit;
		}

		private void Application_Scripting_Exit(object sender, ApplicationExitEventArgs e) {
			if (this._ScriptingHost.IsValueCreated) {
				this._ScriptingHost.Value.Dispose();
			}
		}

		private void ExecuteScripts() {
			try {
				var scriptPath = this._ConfigurationFilePath.Resolve("scripts");
				IO::Directory.CreateDirectory(scriptPath.FullPath);
				var host = this.ScriptingHost;

				foreach(var file in IO::Directory.EnumerateFiles(scriptPath.FullPath, "*", IO::SearchOption.AllDirectories)) {
					if(host.IsSupportedFileExtension(IO::Path.GetExtension(file))) {
						try {
							host.ExecuteFile(file);
						} catch(Exception ex) {
							this._Logger.Value.Warn(ex, "Script Error");
						}
					}
				}
			} catch(IO::IOException ex) {
				this._Logger.Value.Warn(ex, "Script IO Error");
			}
		}
	}
}
