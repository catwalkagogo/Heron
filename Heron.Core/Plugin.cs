using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.Configuration;
using System.Reflection;

namespace CatWalk.Heron {
	public abstract class Plugin : IPlugin {
		public IStorage Storage { get; private set; }

		public void Load(Application app) {
			app.ThrowIfNull("app");

			this.Storage = new PartialStorage(this.GetType().FullName, app.Configuration);

			this.OnLoaded(new PluginEventArgs(app));
		}

		public void Unload(Application app) {
			if(!this.CanUnload(app)) {
				throw new NotImplementedException();
			}

			app.ThrowIfNull("app");

			this.OnUnloaded(new PluginEventArgs(app));

			this.Storage.Dispose();
		}


		#region event

		public event EventHandler<PluginEventArgs> Loaded;
		protected virtual void OnLoaded(PluginEventArgs e) {
			var handler = this.Loaded;
			if(handler != null) {
				handler(this, e);
			}
		}

		public event EventHandler<PluginEventArgs> Unloaded;
		protected virtual void OnUnloaded(PluginEventArgs e) {
			var handler = this.Unloaded;
			if(handler != null) {
				handler(this, e);
			}
		}



		#endregion

		#region IPlugin Members

		public abstract string DisplayName { get; }

		public virtual bool CanUnload(Application app) {
			return true;
		}

		public virtual PluginPriority Priority {
			get {
				return PluginPriority.Normal;
			}
		}

		#endregion
	}

	public class PluginEventArgs : EventArgs {
		public Application Application { get; private set; }

		public PluginEventArgs(Application app) {
			app.ThrowIfNull("app");
			this.Application = app;
		}
	}

	public static class PluginExtensions {
		public static bool IsPluginAssembly(this Assembly asm) {
			asm.ThrowIfNull("asm");
			return asm.GetCustomAttributes<PluginAssemblyAttribute>().Count() > 0;
		}
	}

}
