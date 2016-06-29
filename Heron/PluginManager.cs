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
		private PluginManager() {
		}

		public static PluginManager Create() {
			var builder = new RegistrationBuilder();
			builder
				.ForTypesDerivedFrom<IPlugin>()
				.ExportInterfaces();

			builder
				.ForType<PluginManager>()
			//	.ImportProperty(x => x.PluginInstances, b => b.AsMany())
				.Export<PluginManager>();

			var hostCatalog = new AssemblyCatalog(typeof(PluginManager).Assembly, builder);
			var catalog = new AggregateCatalog(hostCatalog);

			var builtinPluginDir = new FilePath(Assembly.GetEntryAssembly().Location, FilePathFormats.Windows).Resolve("../plugins").FullPath;
			var dlls = Directory.EnumerateFiles(builtinPluginDir, "*.dll", SearchOption.AllDirectories);
			foreach(var dll in dlls) {
				var dllCatalog = new AssemblyCatalog(Assembly.LoadFile(dll), builder);
				catalog.Catalogs.Add(dllCatalog);
			}

			var container = new CompositionContainer(catalog);
			var mngr = container.GetExportedValue<PluginManager>();
			return mngr;
		}

		[System.ComponentModel.Composition.ImportMany]
		private IEnumerable<IPlugin> PluginInstances { get; set; }

		public IEnumerable<IPlugin> Plugins {
			get {
				return this.PluginInstances.EmptyIfNull().OrderByDescending(p => p.Priority);
			}
		}
	}
}
