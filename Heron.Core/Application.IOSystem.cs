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
using CatWalk.Heron.Configuration.IOSystem;

namespace CatWalk.Heron {
	public abstract partial class Application : ControlViewModel, IJobManagerSite {
		private SystemEntryViewModel _RootEntry;
		private RootProvider _RootProvider;

		protected virtual async Task InitializeIOSystem() {
			this._RootProvider = new RootProvider(this);
			this._RootEntry = new SystemEntryViewModel(null, this._RootProvider, new RootEntry(this));

			this.RegisterSystemProvider(new ConfigurationProvider());
		}

		#region Property

		private Messenger _Messenger = null;
		public Messenger Messenger {
			get {
				return this._Messenger ?? (this._Messenger = new Messenger(this.SynchronizationContext));
			}
		}

		internal RootProvider Provider {
			get {
				return this._RootProvider;
			}
		}

		internal SystemEntryViewModel Entry {
			get {
				return this._RootEntry;
			}
		}

		#endregion

		#region TryParseEntryPath

		public bool TryParseEntryPath(string path, out SystemEntryViewModel viewModel) {
			path.ThrowIfNull("path");
			ISystemEntry entry;
			ISystemProvider provider;

			viewModel = null;
			var root = this.Entry.Entry;
			if(this._RootProvider.TryParsePath(root, path, out entry, out provider)) {
				var stack = new Stack<ISystemEntry>();
				while(root != entry) {
					stack.Push(entry);
					entry = entry.Parent;
				}

				viewModel = this.Entry;
				while(stack.Count > 0) {
					viewModel = new SystemEntryViewModel(viewModel, provider, stack.Pop());
				}
				return true;
			} else {
				return false;
			}
		}
		#endregion

		#region RootProvider

		internal class RootProvider : SystemProvider {
			public List<ISystemProvider> Providers { get; private set; }
			public Application Application { get; private set; }
			public RootProvider(Application app) {
				app.ThrowIfNull("app");
				this.Application = app;
				this.Providers = new List<ISystemProvider>();
			}

			public override bool TryParsePath(ISystemEntry root, string path, out ISystemEntry entry) {
				entry = null;
				foreach(var provider in this.Providers) {
					if(provider.TryParsePath(root, path, out entry)) {
						return true;
					}
				}
				return false;
			}

			public bool TryParsePath(ISystemEntry root, string path, out ISystemEntry entry, out ISystemProvider provider) {
				entry = null;
				provider = null;
				foreach(var pro in this.Providers) {
					if(pro.TryParsePath(root, path, out entry)) {
						provider = pro;
						return true;
					}
				}
				return false;
			}


			public override IEnumerable<ISystemEntry> GetRootEntries(ISystemEntry parent) {
				return this.Providers.SelectMany(p => p.GetRootEntries(parent));
			}

			public override object GetViewModel(object parent, SystemEntryViewModel entry, object previous) {
				return this.Providers.Select(p => p.GetViewModel(parent, entry, previous)).Where(vm => vm != null).FirstOrDefault();
			}
		}

		#endregion

		#region RootEntry

		private class RootEntry : SystemEntry {
			public Application Application { get; private set; }
			public RootEntry(Application app)
				: base(null, "") {
				app.ThrowIfNull("app");
				this.Application = app;
			}

			public override bool IsDirectory {
				get {
					return true;
				}
			}

			public override IEnumerable<ISystemEntry> GetChildren(System.Threading.CancellationToken token, IProgress<double> progress) {
				return this.Application._RootProvider.GetRootEntries(this);
			}
		}

		#endregion
	}
}
