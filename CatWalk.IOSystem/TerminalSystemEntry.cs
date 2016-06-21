using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IOSystem {
	public class TerminalSystemEntry : SystemEntry{
		public TerminalSystemEntry(ISystemEntry parent, string name)
			: base(parent, name) {

		}

		public override sealed bool IsDirectory {
			get {
				return false;
			}
		}

		public override sealed IEnumerable<ISystemEntry> GetChildren(System.Threading.CancellationToken token, IProgress<double> progress) {
			this.ThrowIfNotDirectory();
			return null;
		}
	}
}
