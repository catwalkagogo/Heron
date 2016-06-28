using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using CatWalk.IO;
using System.IO;

namespace CatWalk.Heron {
	public class PluginManager {
		private static readonly string SETTINGS_KEY = typeof(PluginManager).FullName;
		private static readonly string ENABLED_PLUGINS_KEY = SETTINGS_KEY + ".EnabledPlugins";
		private Application _App;
		private IList<IPlugin> _Plugins = new IPlugin[0];
		private IList<Assembly> _Assemblies = new List<Assembly>();

		internal PluginManager(Application app) {
			app.ThrowIfNull("app");
			this._App = app;
		}

		public void RestoreEnabledPluginsFromConfiguration() {
			string[] enabledPlugins;
			object v;
			if (this._App.Configuration.TryGetValue(ENABLED_PLUGINS_KEY, out v)) {
				enabledPlugins = (string[])v;
			} else {
				enabledPlugins = this.GetPluginTypes().Select(t => t.FullName).ToArray();
				this._App.Configuration[ENABLED_PLUGINS_KEY] = enabledPlugins;
			}

			this.SetEnabledPlugins(enabledPlugins);
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

		/// <summary>
		/// AppDomainに読み込まれているプラグインクラスを全て取得する
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Type> GetPluginTypes() {
			var asms = this._Assemblies;
			return asms
				.SelectMany(asm => GetTypesFromAssembly(asm))
				.Where(t => typeof(IPlugin).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()) && !t.GetTypeInfo().IsAbstract && !t.GetTypeInfo().IsInterface);
		}

		private static IEnumerable<Type> GetTypesFromAssembly(Assembly asm) {
			try {
				return asm.ExportedTypes;
			}catch(ReflectionTypeLoadException ex) {
				return ex.Types.Where(t => t != null);
			}
		}

		public string[] GetEnabledPlugins(){
			return (string[])((string[])this._App.Configuration[ENABLED_PLUGINS_KEY]).Clone();
		}

		public void SetEnabledPlugins(string[] enabledPlugins){
			enabledPlugins.ThrowIfNull("enabledPlugins");
			enabledPlugins = (string[])enabledPlugins.Clone();
			this._App.Configuration[ENABLED_PLUGINS_KEY] = enabledPlugins;
			if (this.Loaded) {
				this.Unload();
			}

			this.RefreshPluginInstances(enabledPlugins);
			this.Load();
		}

		public void RegisterAssembly(Assembly asm) {
			asm.ThrowIfNull("asm");
			this._Assemblies.Add(asm);
		}

		public bool UnregisterAssembly(Assembly asm) {
			return this._Assemblies.Remove(asm);
		}

		public bool Loaded {
			get;
			private set;
		}
	}
}
