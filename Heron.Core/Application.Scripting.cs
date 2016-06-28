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

		protected abstract IScriptingHost GetScriptingHost();

		protected virtual async Task InitializeScripting() {
			this._ScriptingHost = new Lazy<IScriptingHost>(this.GetScriptingHost);

			var v = this._ScriptingHost.Value;

			this.Exit += Application_Scripting_Exit;
		}

		private void Application_Scripting_Exit(object sender, ApplicationExitEventArgs e) {
			if (this._ScriptingHost.IsValueCreated) {
				this._ScriptingHost.Value.Dispose();
			}
		}
	}
}
