using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.IOSystem;
using CatWalk.IOSystem.Win32;

namespace CatWalk.Heron.IOSystem.Win32 {
	public class Win32Plugin : Plugin {
		public override string DisplayName {
			get {
				return "Win32 Plugin";
			}
		}

		public override bool CanUnload(Application app) {
			return false;
		}

		protected override void OnLoaded(PluginEventArgs e) {
			base.OnLoaded(e);

			e.Application.RootProvider.RootEntryFactory.Register(
				entry => true,
				entry => new ProcessSystemDirectory(entry, "Processes"));

			e.Application.RootProvider.RootEntryFactory.Register(
				entry => true,
				entry => new EnvironmentVariableTargetDirectory(entry, "Environment Variables"));

			e.Application.RootProvider.RootEntryFactory.Register(
				entry => true,
				entry => new PerformanceSystemCategoryDirectory(entry, "Performance Counters"));

		}
	}
}
