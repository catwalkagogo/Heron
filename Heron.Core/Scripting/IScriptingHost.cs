using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron.Scripting {
	public interface IScriptingHost {
		void Execute(string script);
		void ExecuteFile(string path);
		bool IsSupportedFileExtension(string extension);
	}
}
