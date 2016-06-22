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
		private IList<IPlugin> _Plugins = null;

		internal PluginManager(Application app) {
			app.ThrowIfNull("app");
			this._App = app;

			LoadBuiltinPluginAssemblies();

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

		private static void LoadBuiltinPluginAssemblies() {
			var builtinPluginDir = new FilePath(Assembly.GetEntryAssembly().Location).Resolve("../plugins").FullPath;
			var dlls = Directory.EnumerateFiles(builtinPluginDir, "*.dll", SearchOption.AllDirectories);
			var asmNames = dlls.ToDictionary(dll => AssemblyName.GetAssemblyName(dll));

			var loadedAsmNames = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(asm => asm.GetReferencedAssemblies().Concat(Seq.Make(asm.GetName())))
				.Distinct(asmName => asmName.FullName)
				.ToLookup(asmName => asmName.FullName);
			var asmsToLoad = asmNames.Where(asmName => !loadedAsmNames.Contains(asmName.Key.FullName)).Distinct(pair => pair.Key.FullName).ToArray();
			asmsToLoad.ForEach(asmName => {
				Assembly.LoadFile(asmName.Value);
			});
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
			var asms = AppDomain.CurrentDomain.GetAssemblies().ToArray();
			return AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(asm => GetTypesFromAssembly(asm))
				.Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);
		}

		private static IEnumerable<Type> GetTypesFromAssembly(Assembly asm) {
			try {
				return asm.GetTypes();
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
