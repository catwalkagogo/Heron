using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Scripting.Hosting;
using IronPython;
using IronPython.Hosting;
using IronRuby;
using IronRuby.Builtins;

namespace CatWalk.Heron.Scripting {
	using IO = System.IO;
	public class DlrHost : IScriptingHost{
		private Lazy<ScriptRuntime> _Runtime;
		private Lazy<ScriptScope> _Scope;
		private Lazy<ISet<string>> _Extensions;

		public DlrHost() {
			this._Runtime = new Lazy<ScriptRuntime>(() => {
				var setup = new ScriptRuntimeSetup();
				setup.DebugMode = true;

				setup.LanguageSetups.Add(Python.CreateLanguageSetup(null));
				setup.AddRubySetup();

				var runtime = new ScriptRuntime(setup);
				AppDomain.CurrentDomain.GetAssemblies().ForEach(assm => runtime.LoadAssembly(assm));
				return runtime;
			});

			this._Scope = new Lazy<ScriptScope>(() => {
				var scope = this._Runtime.Value.CreateScope();
				return scope;
			});

			this._Extensions = new Lazy<ISet<string>>(() => {
				return new HashSet<string>(this._Runtime.Value.Setup.LanguageSetups.SelectMany(lang => lang.FileExtensions), StringComparer.OrdinalIgnoreCase);
			});
		}

		#region IScriptingHost Members

		public void Execute(string script) {
			this.Execute(script, "python");
		}

		public void Execute(string script, string languageName) {
			var engine = this._Runtime.Value.GetEngine(languageName);
			var source = engine.CreateScriptSourceFromString(script);
			var code = source.Compile();
			code.Execute(this._Scope.Value);
		}

		public void ExecuteFile(string path) {
			path = IO::Path.GetFullPath(path);
			var engine = this._Runtime.Value.GetEngineByFileExtension(IO::Path.GetExtension(path));
			var source = engine.CreateScriptSourceFromFile(path);
			var code = source.Compile();
			code.Execute(this._Scope.Value);
		}

		public bool IsSupportedFileExtension(string extension) {
			return this._Extensions.Value.Contains(extension);
		}

		#endregion
	}
}