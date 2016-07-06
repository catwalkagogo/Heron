using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.Configuration;
using System.Reflection;

namespace CatWalk.Heron {
	public abstract class Plugin : DisposableObject, IPlugin {
		public const int PriorityNormal = 100;
		public const int PriorityBuiltin = Int32.MaxValue;
		public const int PriorityLowest = Int32.MinValue;

		public Application Application { get; private set; }

		/// <summary>
		/// プラグイン用Storage
		/// </summary>
		public IStorage Storage { get; private set; }

		public void Load(Application app) {
			app.ThrowIfNull("app");

			this.Application = app;
			this.Storage = new PartialStorage(this.GetType().FullName, app.Configuration);

			this.IsLoaded = true;

			this.OnLoaded(new PluginEventArgs(app));
		}

		public void Unload(Application app) {
			if(!this.CanUnload(app)) {
				throw new NotImplementedException();
			}

			app.ThrowIfNull("app");

			this.IsLoaded = false;

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

		public bool IsLoaded { get; private set; }

		public abstract string DisplayName { get; }

		public virtual bool CanUnload(Application app) {
			return true;
		}

		public virtual int Priority {
			get {
				return PriorityNormal;
			}
		}

		#endregion

		protected override void Dispose(bool disposing) {
			if (this.IsLoaded && this.CanUnload(this.Application)) {
				this.Unload(this.Application);
			}
			base.Dispose(disposing);
		}
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
