using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron.Scripting {
	public interface IScriptingHost : IDisposable {
		void Execute(string script);
		void ExecuteFile(string path);
		bool IsSupportedFileExtension(string extension);
		void AddHostType(string name, Type type);
		void AddHostObject(string name, object obj);
	}
}
