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
		private Lazy<IList<IScriptingHost>> _ScriptingHosts;

		public IList<IScriptingHost> ScriptingHosts {
			get {
				return this._ScriptingHosts.Value;
			}
		}

		private void ExecuteScripts() {
			try {
				var scriptPath = this._ConfigurationFilePath.Resolve("Scripts");
				IO::Directory.CreateDirectory(scriptPath.FullPath);
				foreach(var file in IO::Directory.EnumerateFiles(scriptPath.FullPath, "*", IO::SearchOption.AllDirectories)) {
					foreach(var host in this._ScriptingHosts.Value) {
						if(host.IsSupportedFileExtension(IO::Path.GetExtension(file))) {
							try {
								host.ExecuteFile(file);
							} catch(Exception ex) {
								this._Logger.Value.Warn(ex, "Script Error");
							}
						}
					}
				}
			} catch(IO::IOException ex) {
				this._Logger.Value.Warn(ex, "Script IO Error");
			}
		}
	}
}
