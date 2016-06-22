using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using CatWalk;
using CatWalk.Windows.Input;
using CatWalk.IOSystem;
using CatWalk.Heron.IOSystem;
using CatWalk.Heron.ViewModel.IOSystem;
using CatWalk.Heron.ViewModel.Windows;
using CatWalk.Mvvm;
using CatWalk.Heron.ViewModel;

namespace CatWalk.Heron {
	public abstract partial class Application : ControlViewModel, IJobManagerSite {
		private void InitializePlugin() {
			this.PluginManager = new PluginManager(this);
			this.PluginManager.Load();
		}

		private EntryOperatorCollection _EntryOperators = new EntryOperatorCollection();
		public IEntryOperator EntryOperator {
			get {
				return this._EntryOperators;
			}
		}


		#region Plugin

		private Lazy<ViewFactory> _ViewFactory = new Lazy<ViewFactory>(() => new ViewFactory());
		public ViewFactory ViewFactory {
			get {
				return this._ViewFactory.Value;
			}
		}

		public PluginManager PluginManager { get; private set; }

		public void RegisterSystemProvider(ISystemProvider provider) {
			provider.ThrowIfNull("provider");

			this.Provider.Providers.Add(provider);
		}

		public void UnregisterSystemProvider(ISystemProvider provider) {
			this.Provider.Providers.RemoveAll(p => provider == p);
		}

		public void RegisterEntryOperator(IEntryOperator op) {
			op.ThrowIfNull("op");

			this._EntryOperators.Add(op);
		}

		public void UnregisterEntryOperator(IEntryOperator op) {
			var c = this._EntryOperators;
			for(var i = c.Count - 1; i > 0; i--) {
				var o = c[i];
				if(o == op) {
					c.RemoveAt(i);
				}
			}
		}

		#endregion
	}
}
