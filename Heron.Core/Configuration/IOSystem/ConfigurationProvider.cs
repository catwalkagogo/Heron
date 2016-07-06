using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.IOSystem;
using CatWalk.Heron.ViewModel.IOSystem;
using CatWalk.IOSystem;

namespace CatWalk.Heron.Configuration.IOSystem {
	public class ConfigurationProvider : ISystemProvider {
		public string DisplayName {
			get {
				return "Configuration";
			}
		}

		public string Name {
			get {
				return "Configuration";
			}
		}

		public bool CanGetColumnDefinitions(ISystemEntry entry) {
			return false;
		}

		public bool CanGetGroupings(ISystemEntry entry) {
			return false;
		}

		public bool CanGetOrderDefinitions(SystemEntryViewModel entry) {
			return false;
		}

		public bool CanGetViewModel(object parent, SystemEntryViewModel entry, object previous) {
			return false;
		}

		public IEnumerable<IColumnDefinition> GetColumnDefinitions(ISystemEntry entry) {
			throw new NotImplementedException();
		}

		public IEnumerable<IGroupDefinition> GetGroupings(ISystemEntry entry) {
			throw new NotImplementedException();
		}

		public IEnumerable<OrderDefinition> GetOrderDefinitions(SystemEntryViewModel entry) {
			throw new NotImplementedException();
		}

		public IEnumerable<ISystemEntry> GetRootEntries(ISystemEntry parent) {
			return Seq.Make(new ConfigurationDirectory(Application.Current.Configuration, parent, "Configuration"));
		}

		public object GetViewModel(object parent, SystemEntryViewModel entry, object previous) {
			return null;
		}

		public ParsePathResult ParsePath(ISystemEntry root, string path) {
			return new ParsePathResult(false, null, false);
		}
	}
}
