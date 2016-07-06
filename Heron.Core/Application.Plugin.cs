using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using CatWalk;
using CatWalk.IOSystem;
using CatWalk.Heron.IOSystem;
using CatWalk.Heron.ViewModel.IOSystem;
using CatWalk.Heron.ViewModel.Windows;
using CatWalk.Mvvm;
using CatWalk.Heron.ViewModel;

namespace CatWalk.Heron {
	public abstract partial class Application : ControlViewModel, IJobManagerSite {
		public const string MainWindowViewFactoryName = "MainWindowView";

		protected virtual async Task InitializePlugin() {
			this._PluginManager = new Lazy<IPluginManager>(() => {
				return this.GetPluginManager();
			});
			this._Factories[MainWindowViewFactoryName] = new Factory<MainWindowViewModel, object>();
		}

		protected abstract IPluginManager GetPluginManager();

		private EntryOperatorCollection _EntryOperators = new EntryOperatorCollection();
		public IEntryOperator EntryOperator {
			get {
				return this._EntryOperators;
			}
		}

		#region Factory

		private IDictionary<string, Factory> _Factories = new Dictionary<string, Factory>();
		public IDictionary<string, Factory> Factories {
			get {
				return this._Factories;
			}
		}

		public Factory GetFactory(string name) {
			Factory factory;
			if(this.Factories.TryGetValue(name, out factory)) {
				return factory;
			}

			return null;
		}

		public Factory<TSource, TDest> GetFactory<TSource, TDest>(string name) {
			return this.GetFactory(name) as Factory<TSource, TDest>;
		}

		public Factory<T1, T2, TDest> GetFactory<T1, T2, TDest>(string name) {
			return this.GetFactory(name) as Factory<T1, T2, TDest>;
		}

		public Factory<T1, T2, T3, TDest> GetFactory<T1, T2, T3, TDest>(string name) {
			return this.GetFactory(name) as Factory<T1, T2, T3, TDest>;
		}

		public Factory<T1, T2, T3, T4, TDest> GetFactory<T1, T2, T3, T4, TDest>(string name) {
			return this.GetFactory(name) as Factory<T1, T2, T3, T4, TDest>;
		}

		public Factory<MainWindowViewModel, object> MainWindowViewFactory {
			get {
				return this.GetFactory<MainWindowViewModel, object>(MainWindowViewFactoryName);
			}
		}

		#endregion

		#region Plugin

		private Lazy<IPluginManager> _PluginManager;
		public IPluginManager PluginManager {
			get {
				return this._PluginManager.Value;
			}
		}

		public void RegisterSystemProvider(ISystemProvider provider) {
			provider.ThrowIfNull("provider");

			this._RootProvider.Add(provider);
		}

		public bool UnregisterSystemProvider(ISystemProvider provider) {
			return this._RootProvider.Remove(provider);
		}

		public void RegisterEntryOperator(IEntryOperator op) {
			op.ThrowIfNull("op");

			this._EntryOperators.Add(op);
		}

		public bool UnregisterEntryOperator(IEntryOperator op) {
			return this._EntryOperators.Remove(op);
		}

		#endregion
	}
}
