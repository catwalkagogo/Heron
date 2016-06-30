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

		/// <summary>
		/// 指定されたパスを解析してエントリーを取得する
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public ParseEntryPathResult ParseEntryPath(string path) {
			path.ThrowIfNull("path");

			var root = this.Entry.Entry;
			var result = this._RootProvider.ParsePath(root, path);
			if (result.Success) {
				var entry = result.Entry;
				var stack = new Stack<ISystemEntry>();

				// ルートまでのエントリーの階層を取得する
				while(root != entry) {
					stack.Push(entry);
					entry = entry.Parent;
				}

				// ルートからViewModelを生成する
				var viewModel = this.Entry;
				while(stack.Count > 0) {
					viewModel = new SystemEntryViewModel(viewModel, this._RootProvider, stack.Pop());
				}
				return new ParseEntryPathResult(true, viewModel, result.TerminatedByDirectorySeparator);
			} else {
				return new ParseEntryPathResult(false, null, false);
			}
		}
		#endregion

		#region RootProvider

		/// <summary>
		/// ルートプロバイダ
		/// 登録されたプロバイダのルートエントリを束ねる
		/// </summary>
		internal class RootProvider : SystemProvider {
			public List<SystemProvider> Providers { get; private set; }
			public Application Application { get; private set; }
			public RootProvider(Application app) {
				app.ThrowIfNull("app");
				this.Application = app;
				this.Providers = new List<SystemProvider>();
			}

			public override ParsePathResult ParsePath(ISystemEntry root, string path) {
				root.ThrowIfNull("root");
				path.ThrowIfNull("path");
				foreach(var provider in this.Providers) {
					var result = provider.ParsePath(root, path);
					if (result.Success) {
						return result;
					}
				}

				var fragments = path.Split(SystemEntry.DirectorySeperatorChar);
				var entry = root;
				foreach (var name in fragments) {
					entry = entry.GetChild(name);
					if (entry == null) {
						return new ParsePathResult(false, null, false);
					}
				}
				return new ParsePathResult(true, entry, path.EndsWith(SystemEntry.DirectorySeperatorChar.ToString()));
			}

			public override IEnumerable<ISystemEntry> GetRootEntries(ISystemEntry parent) {
				return this.Providers.SelectMany(p => p.GetRootEntries(parent));
			}

			public override object GetViewModel(object parent, SystemEntryViewModel entry, object previous) {
				return this.Providers.Select(p => p.GetViewModel(parent, entry, previous)).FirstOrDefault(vm => vm != null);
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


	public class ParseEntryPathResult {
		public bool Success { get; private set; }
		public SystemEntryViewModel Entry { get; private set; }
		public bool TerminatedByDirectorySeparator { get; private set; }

		public ParseEntryPathResult(bool success, SystemEntryViewModel entry, bool terminatedByDirectorySeparator) {
			this.Success = success;
			this.Entry = entry;
			this.TerminatedByDirectorySeparator = terminatedByDirectorySeparator;
		}
	}
}
