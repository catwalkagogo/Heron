/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronJS.Hosting;

namespace CatWalk.Heron.Scripting {
	public class IronJSHost : IScriptingHost{
		private Lazy<CSharp.Context> _Context;
		public IronJSHost() {
			this._Context = new Lazy<CSharp.Context>(() => new CSharp.Context());
		}

		#region IScriptingHost Members

		public void ExecuteFile(string file) {
			this._Context.Value.ExecuteFile(file);
		}

		public bool IsSupportedFileExtension(string extension) {
			return StringComparer.OrdinalIgnoreCase.Equals(extension, ".js");
		}

		#endregion
	}
}
*/