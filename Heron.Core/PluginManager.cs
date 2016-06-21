using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron {
	public class PluginManager {
		private static readonly string SETTINGS_KEY = typeof(PluginManager).FullName;
		private static readonly string ENABLED_PLUGINS_KEY = SETTINGS_KEY + ".EnabledPlugins";
		private Application _App;
		private IList<IPlugin> _Plugins = null;

		internal PluginManager(Application app) {
			app.ThrowIfNull("app");
			this._App = app;

			string[] enabledPlugins;
			object v;
			if(this._App.Configuration.TryGetValue(ENABLED_PLUGINS_KEY, out v)) {
				enabledPlugins = (string[])v;
			} else {
				enabledPlugins = this.GetPluginTypes().Select(t => t.FullName).ToArray();
				this._App.Configuration[ENABLED_PLUGINS_KEY] = enabledPlugins;
			}

			this.RefreshPluginInstances(enabledPlugins);
		}

		private void RefreshPluginInstances(string[] enabledPlugins) {
			Array.Sort(enabledPlugins);

			this._Plugins = this.GetPluginTypes()
				.Where(t => Array.BinarySearch(enabledPlugins, t.FullName) >= 0)
				.Select(t => (IPlugin)Activator.CreateInstance(t))
				.ToList();
		}

		public void Load() {
			if(this.Loaded) {
				throw new InvalidOperationException("Plugins are already loaded.");
			}
			this._Plugins.ForEach(p => p.Load(this._App));
			this.Loaded = true;
		}

		public void Unload() {
			if(!this.Loaded) {
				throw new InvalidOperationException("Plugins are not loaded.");
			}
			this._Plugins.ForEach(p => p.Unload(this._App));
			this.Loaded = false;
		}

		public void Reload() {
			if(!this.Loaded) {
				throw new InvalidOperationException("Plugins are not loaded.");
			}
			this.Unload();
			this.Load();
		}

		public IEnumerable<Type> GetPluginTypes() {
			return AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(asm => asm.GetTypes())
				.Where(t => t.IsSubclassOf(typeof(IPlugin)) && !t.IsAbstract && !t.IsInterface);
		}

		public string[] GetEnabledPlugins(){
			return (string[])((string[])this._App.Configuration[ENABLED_PLUGINS_KEY]).Clone();
		}

		public void SetEnabledPlugins(string[] enabledPlugins){
			enabledPlugins.ThrowIfNull("enabledPlugins");
			enabledPlugins = (string[])enabledPlugins.Clone();
			this._App.Configuration[ENABLED_PLUGINS_KEY] = enabledPlugins;
			this.Unload();

			this.RefreshPluginInstances(enabledPlugins);
			this.Load();
		}

		public bool Loaded {
			get;
			private set;
		}
	}
}
