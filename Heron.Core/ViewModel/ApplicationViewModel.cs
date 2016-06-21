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

namespace CatWalk.Heron.ViewModel {
	public class ApplicationViewModel : ControlViewModel, IJobManagerSite {
		private SystemEntryViewModel _RootEntry;
		private RootProvider _RootProvider;
		private IJobManager _JobManager;
		private Messenger _Messenger;
		private ViewFactory _ViewFactory;
		private EntryOperatorCollection _EntryOperators = new EntryOperatorCollection();

		public ApplicationViewModel(Messenger messenger, ViewFactory viewFactory){
			this._Messenger = messenger;
			this._ViewFactory = viewFactory;
		}

		public void StartUp(Application.CommandLineOption option) {
			this._RootProvider = new RootProvider(this);
			this._RootEntry = new SystemEntryViewModel(null, this._RootProvider, new RootEntry(this));
			this._JobManager = new JobManager();
		}

		public MainWindowViewModel CreateMainWindow() {
			var vm = new MainWindowViewModel();
			var v = this._ViewFactory.Create(vm);
			this.Children.Add(vm);
			return vm;
		}

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

		#region Property

		public Messenger Messenger {
			get {
				return this._Messenger;
			}
		}

		internal EntryOperatorCollection EntryOperators {
			get {
				return this._EntryOperators;
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

		#region RootProvider

		internal class RootProvider : SystemProvider {
			public List<ISystemProvider> Providers { get; private set; }
			public ApplicationViewModel Application { get; private set; }
			public RootProvider(ApplicationViewModel app) {
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
			public ApplicationViewModel Application { get; private set; }
			public RootEntry(ApplicationViewModel app) : base(null, "") {
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

		#region IJobManagerSite Members

		public IJobManager JobManager {
			get {
				return this._JobManager;
			}
		}

		#endregion

		#region ArrangeWindows

		private DelegateCommand<ArrangeMode> _ArrangeWindowsCommand;

		public DelegateCommand<ArrangeMode> ArrangeWindowsCommand {
			get {
				return this._ArrangeWindowsCommand ?? (this._ArrangeWindowsCommand = new DelegateCommand<ArrangeMode>(this.ArrangeWindows));
			}
		}

		public void ArrangeWindows(ArrangeMode mode) {
			Application.Current.Messenger.Send(new WindowMessages.ArrangeWindowsMessage(this, mode));
		}

		#endregion
	}
}
