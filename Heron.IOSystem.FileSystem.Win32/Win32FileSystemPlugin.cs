using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.Configuration;
using Reactive.Bindings.Extensions;
using Reactive.Bindings;

namespace CatWalk.Heron.FileSystem.Win32 {
	public class Win32FileSystemPlugin : Plugin{
		private Win32FileSystemProvider _Provider;
		private Win32FileSystemEntryOperator _Operator;

		protected override void OnLoaded(PluginEventArgs e) {
			this._Provider = new Win32FileSystemProvider();
			this._Operator = Win32FileSystemEntryOperator.Default;
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