using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;

namespace CatWalk.Heron {
	[AttributeUsage(AttributeTargets.Assembly)]
	public class PluginAssemblyAttribute : Attribute{
		private Type[] _PluginTypes;

		//public PluginAssemblyAttribute() { }

		public PluginAssemblyAttribute(params Type[] pluginTypes) {
			this._PluginTypes = pluginTypes;
		}

		public IEnumerable<Type> PluginTypes {
			get {
				return Seq.Make(this._PluginTypes);
			}
		}
	}
}
