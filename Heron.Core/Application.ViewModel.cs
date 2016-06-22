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
using CatWalk.Collections;

namespace CatWalk.Heron {
	public abstract partial class Application : ControlViewModel, IJobManagerSite {
		private IJobManager _JobManager;
		private IObservableList<MainWindowViewModel> _MainWindows = new WrappedObservableList<MainWindowViewModel>(new SkipList<MainWindowViewModel>());
		private IReadOnlyObservableList<MainWindowViewModel> _ReadOnlyMainWindows;

		private void InitializeViewModel() {
			this._JobManager = new JobManager();
			this._MainWindows.CollectionChanged += (sender, e) => {
				var index = 0;
				foreach (var win in this._MainWindows) {
					win.Index = index++;
				}
			};
		}

		public MainWindowViewModel CreateMainWindow() {
			var vm = new MainWindowViewModel(this);
			this.Children.Add(vm);
			var v = this.ViewFactory.Create(vm);
			this._MainWindows.Add(vm);
			return vm;
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

		private DelegateCommand<ArrangeMode> _ArrangeWindowsCommand;

		public DelegateCommand<ArrangeMode> ArrangeWindowsCommand {
			get {
				return this._ArrangeWindowsCommand ?? (this._ArrangeWindowsCommand = new DelegateCommand<ArrangeMode>(this.ArrangeWindows));
			}
		}

		public void ArrangeWindows(ArrangeMode mode) {
			this.Messenger.Send(new WindowMessages.ArrangeWindowsMessage(this, mode));
		}

		#endregion
	}
}
