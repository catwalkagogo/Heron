using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.IO;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using System.IO;

namespace CatWalk.Heron.Scripting {
	public class ClearScriptHost : DisposableObject, IScriptingHost {
		private Lazy<V8ScriptEngine> _Engine;

		public ClearScriptHost() {
			this._Engine = new Lazy<V8ScriptEngine>(() => {
				this.ThrowIfDisposed();

				var engine = new V8ScriptEngine();
				var clr = new HostTypeCollection("mscorlib", "System", "System.Core");
				engine.AddHostObject("clr", clr);

				return engine;
			});
		}

		public V8ScriptEngine Engine {
			get {
				return this._Engine.Value;
			}
		}

		public void AddHostObject(string name, object obj) {
			this.Engine.AddHostObject(name, obj);
		}

		public void AddHostType(string name, Type type) {
			this.Engine.AddHostType(name, type);
		}

		public void Execute(string script) {
			this.ThrowIfDisposed();

			this.Engine.Execute(script);
		}

		public void ExecuteFile(string path) {
			this.ThrowIfDisposed();
			this.Execute(File.ReadAllText(path));
		}

		public bool IsSupportedFileExtension(string extension) {
			return FilePathFormats.Windows.StringEqualityComparer.Equals(extension, ".js");
		}

		protected override void Dispose(bool disposing) {
			if (!this.IsDisposed) {
				if (this._Engine.IsValueCreated) {
					this.Engine.Dispose();
				}
			}

			base.Dispose(disposing);
		}
	}
}
