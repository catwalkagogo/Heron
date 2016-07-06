using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron {
	internal static class I18N { 
		public static string _(string name) {
			return Properties.Resources.ResourceManager.GetString(name);
		}

		public static string _(string name, params object[] prms) {
			return String.Format(Properties.Resources.ResourceManager.GetString(name), prms);
		}

	}
}
