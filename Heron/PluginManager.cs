using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition.Registration;
using System.ComponentModel.Composition.Hosting;
using CatWalk.IO;
using System.IO;
using System.Reflection;

namespace CatWalk.Heron {
	public class PluginManager : IPluginManager{
		private PluginHost _PluginHost;

		public PluginManager() {
			var builder = new RegistrationBuilder();
			builder
				.ForTypesDerivedFrom<IPlugin>()
				.ExportInterfaces();

			builder
				.ForType<PluginHost>()
				//	.ImportProperty(x => x.PluginInstances, b => b.AsMany())
				.Export<PluginHost>();

			var hostCatalog = new AssemblyCatalog(typeof(PluginHost).Assembly, builder);
			var catalog = new AggregateCatalog(hostCatalog);

			var builtinPluginDir = new FilePath(Assembly.GetEntryAssembly().Location, FilePathFormats.Windows).Resolve("../plugins").FullPath;
			var dlls = Directory.EnumerateFiles(builtinPluginDir, "*.dll", SearchOption.AllDirectories);
			foreach (var dll in dlls) {
				var dllCatalog = new AssemblyCatalog(Assembly.LoadFile(dll), builder);
				catalog.Catalogs.Add(dllCatalog);
			}

			var container = new CompositionContainer(catalog);
			var host = container.GetExportedValue<PluginHost>();
			if(host == null) {
				throw new InvalidOperationException();
			}

			this._PluginHost = host;
		}

		public IEnumerable<IPlugin> Plugins {
			get {
				return this._PluginHost.PluginInstances.EmptyIfNull().OrderByDescending(p => p.Priority);
			}
		}

		private class PluginHost {
			[System.ComponentModel.Composition.ImportMany(AllowRecomposition = true)]
			public IEnumerable<IPlugin> PluginInstances { get; set; }
		}
	}
}
