using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.ViewModel;
using CatWalk.Heron.ViewModel.Windows;
using System.Reactive;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;
using Reactive.Bindings;
using CatWalk.Heron.ViewModel.IOSystem;

namespace CatWalk.Heron.FileSystem {
	public class FileSystemViewModel : ControlViewModel{
		private ReactiveProperty<SystemEntryViewModel> _Entry;

		public FileSystemViewModel() {
			this._Entry = new ReactiveProperty<SystemEntryViewModel>();
		}

		public SystemEntryViewModel Entry {
			get {
				return this._Entry.Value;
			}
			set {
				this._Entry.Value = value;
			}
		}
	}
}
