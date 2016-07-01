using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.Configuration;
using Reactive.Bindings.Extensions;
using Reactive.Bindings;

namespace CatWalk.Heron.FileSystem {
	public class FileSystemPlugin : Plugin{
		private FileSystemProvider _Provider;
		private FileSystemEntryOperator _Operator;

		protected override void OnLoaded(PluginEventArgs e) {
			this._Provider = new FileSystemProvider();
			this._Operator = FileSystemEntryOperator.Default;
			var app = e.Application;
			app.RegisterSystemProvider(this._Provider);
			app.RegisterEntryOperator(this._Operator);
			base.OnLoaded(e);
		}

		protected override void OnUnloaded(PluginEventArgs e) {
			var app = e.Application;
			app.UnregisterSystemProvider(this._Provider);
			app.UnregisterEntryOperator(this._Operator);
		}

		public override string DisplayName {
			get {
				return "FileSystem IOSystem";
			}
		}
	}
}