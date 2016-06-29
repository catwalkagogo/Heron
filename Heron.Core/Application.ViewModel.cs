using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Input;
using CatWalk;
using CatWalk.IOSystem;
using CatWalk.Heron.IOSystem;
using CatWalk.Heron.ViewModel.IOSystem;
using CatWalk.Heron.ViewModel.Windows;
using CatWalk.Mvvm;
using CatWalk.Heron.ViewModel;
using CatWalk.Collections;
using System.Reactive;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;

namespace CatWalk.Heron {
	public abstract partial class Application : ControlViewModel, IJobManagerSite {
		private IJobManager _JobManager;
		private IObservableList<MainWindowViewModel> _MainWindows = new WrappedObservableList<MainWindowViewModel>(new SkipList<MainWindowViewModel>());
		private IReadOnlyObservableList<MainWindowViewModel> _ReadOnlyMainWindows;
		private CompositeDisposable _MainWindowsActivatedSubscribers = new CompositeDisposable();
		public MainWindowViewModel LastActiveMainWindow { get; private set; }

		protected virtual async Task InitializeViewModel() {
			this._JobManager = new JobManager();
			this.Disposables.Add(this._MainWindows.CollectionChangedAsObservable().Subscribe(e => {
				this._MainWindowsActivatedSubscribers.Dispose();
				this._MainWindowsActivatedSubscribers.Clear();

				var index = 0;
				foreach (var win in this._MainWindows) {
					// インデックス貼り直し
					win.Index = index++;

					this._MainWindowsActivatedSubscribers.Add(win.ObserveProperty(_ => _.IsActive).Subscribe(isActive => {
						this.LastActiveMainWindow = win;
					}));
				}
			}));
		}

		internal void AddMainWindow(MainWindowViewModel vm) {
			vm.ThrowIfNull("vm");
			this.Children.Add(vm);
			var v = this.ViewFactory.Create(vm);
			this._MainWindows.Add(vm);
		}

		internal bool RemoveMainWindow(MainWindowViewModel mainWindow) {
			return this._MainWindows.Remove(mainWindow);
		}

		public IReadOnlyObservableList<MainWindowViewModel> MainWindows {
			get {
				if(this._ReadOnlyMainWindows == null) {
					this._ReadOnlyMainWindows = this._MainWindows.AsReadOnlyList();
				}
				return this._ReadOnlyMainWindows;
			}
		}

		#region IJobManagerSite Members

		public IJobManager JobManager {
			get {
				return this._JobManager;
			}
		}

		#endregion

		#region ArrangeWindows

		private ICommand _ArrangeWindowsCommand;

		public ICommand ArrangeWindowsCommand {
			get {
				if(this._ArrangeWindowsCommand == null) {
					var cmd = this.MainWindows
						.CollectionChangedAsObservable()
						.Select(e => this.MainWindows.Count > 0)
						.ToReactiveCommand<ArrangeMode>();
					this.Disposables.Add(cmd.Subscribe(this.ArrangeWindows));
					this._ArrangeWindowsCommand = cmd;
				}

				return this._ArrangeWindowsCommand;
			}
		}

		public void ArrangeWindows(ArrangeMode mode) {
			this.Messenger.Send(new WindowMessages.ArrangeWindowsMessage(mode));
		}

		#endregion
	}
}
