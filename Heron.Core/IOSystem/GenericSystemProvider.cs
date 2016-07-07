using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.ViewModel.IOSystem;
using CatWalk.IOSystem;
using CatWalk.Collections;

namespace CatWalk.Heron.IOSystem {
	
	public class GenericSystemProvider : ISystemProvider {
		public Factory<ISystemEntry, ISystemEntry> RootEntryFactory { get; private set; } = new Factory<ISystemEntry, ISystemEntry>();
		public Factory<object, SystemEntryViewModel, object, object> ViewModelFactory { get; private set; } = new Factory<object, SystemEntryViewModel, object, object>();
		public Factory<ISystemEntry, string, ParsePathResult> ParsePathFactory { get; private set; } = new Factory<ISystemEntry, string, ParsePathResult>();

		public string DisplayName {
			get {
				return this.Name;
			}
		}

		public string Name {
			get {
				return "GenericProvider";
			}
		}

		public IEnumerable<ISystemEntry> GetRootEntries(ISystemEntry parent) {
			var roots = this.RootEntryFactory.CreateAll(parent).ToArray();
			return roots;
		}

		public object GetViewModel(object parent, SystemEntryViewModel entry, object previous) {
			return this.ViewModelFactory.Create(parent, entry, previous);
		}

		public ParsePathResult ParsePath(ISystemEntry root, string path) {
			return this.ParsePathFactory.Create(root, path) ?? new ParsePathResult(false, null, false);
		}

		public bool CanGetColumnDefinitions(ISystemEntry entry) {
			return false;
		}

		public IEnumerable<IColumnDefinition> GetColumnDefinitions(ISystemEntry entry) {
			throw new NotImplementedException();
		}

		public bool CanGetViewModel(object parent, SystemEntryViewModel entry, object previous) {
			return this.ViewModelFactory.CanCreate(parent, entry, previous);
		}

		public bool CanGetGroupings(ISystemEntry entry) {
			return false;
		}

		public IEnumerable<IGroupDefinition> GetGroupings(ISystemEntry entry) {
			throw new NotImplementedException();
		}

		public bool CanGetOrderDefinitions(SystemEntryViewModel entry) {
			return false;
		}

		public IEnumerable<OrderDefinition> GetOrderDefinitions(SystemEntryViewModel entry) {
			throw new NotImplementedException();
		}
	}
}
