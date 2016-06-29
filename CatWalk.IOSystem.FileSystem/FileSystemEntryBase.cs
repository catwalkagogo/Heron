using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.IOSystem.FileSystem {
	public abstract class FileSystemEntryBase : SystemEntry {
		public FileSystemEntryBase(ISystemEntry parent, string name) : base(parent, name) {
		}

		protected override StringComparison StringComparison {
			get {
				return StringComparison.OrdinalIgnoreCase;
			}
		}
	}
}
