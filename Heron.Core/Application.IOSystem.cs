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
		private RootSystemProvider _RootProvider;

		protected virtual async Task InitializeIOSystem() {
			this._RootProvider = new RootSystemProvider();
			this._RootEntry = new SystemEntryViewModel(null, this._RootProvider, new RootSystemEntry(this));

			this.RegisterSystemProvider(new ConfigurationProvider());
		}

		#region Property

		private Messenger _Messenger = null;
		public Messenger Messenger {
			get {
				return this._Messenger ?? (this._Messenger = new Messenger(this.SynchronizationContext));
			}
		}

		public RootSystemProvider RootProvider {
			get {
				return this._RootProvider;
			}
		}

		internal SystemEntryViewModel RootEntry {
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

			var root = this.RootEntry.Entry;
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
				var viewModel = this.RootEntry;
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
		public class RootSystemProvider : SystemProviderCollection {
			private GenericSystemProvider _GenericProvider;

			public RootSystemProvider() {
				this.Add(new BuiltinSystemProvider());
				this._GenericProvider = new GenericSystemProvider();
				this.Add(this._GenericProvider);
			}

			public Factory<ISystemEntry, ISystemEntry> RootEntryFactory {
				get {
					return this._GenericProvider.RootEntryFactory;
				}
			}
			public Factory<object, SystemEntryViewModel, object, object> ViewModelFactory {
				get {
					return this._GenericProvider.ViewModelFactory;
				}
			}
			public Factory<ISystemEntry, string, ParsePathResult> ParsePathFactory {
				get {
					return this._GenericProvider.ParsePathFactory;
				}
			}

		}

		#endregion

		#region RootEntry

		private class RootSystemEntry : SystemEntry {
			public Application Application { get; private set; }
			public RootSystemEntry(Application app)
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
