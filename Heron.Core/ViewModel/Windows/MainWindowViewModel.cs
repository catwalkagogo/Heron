using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using CatWalk.IOSystem;
using CatWalk.Heron.ViewModel.IOSystem;
using System.Windows;
using CatWalk.Heron.Configuration;

namespace CatWalk.Heron.ViewModel.Windows {
	/// <summary>
	/// MainWindowViewModel
	///		PanelCollectionViewModel
	///			PanelViewModel
	///				ListViewModel
	/// </summary>
	public class MainWindowViewModel : WindowViewModel, IJobManagerSite{
		private JobManager _JobManager = new JobManager();
		private PanelCollectionViewModel _Panels;
		public string ID { get; private set; }

		public MainWindowViewModel(Application app, string id = null) : base(app) {
			this.ID = id;
			this.Storage = new PartialStorage(this.GetStoragePrefix(), app.Configuration);

			this._Panels = new PanelCollectionViewModel(app);
			this.Children.Add(this._Panels);

			var messenger = this.Messenger;
			messenger.Subscribe<Messages.DataContextAttachedMessage>(m => {
				// 初期化処理

				// 設定読み込み
				var rect = this.Storage.Get<Rect<double>>("RestoreBounds", Rect<double>.Empty);
				if (rect != Rect<double>.Empty) {
					this.RestoreBounds = rect;
				}
				var windowState = this.Storage.Get<WindowState>("WindowState", WindowState.Normal);
				this.WindowState = windowState;
			}, this);

			messenger.Subscribe<WindowMessages.ClosingMessage>(m => {
				// 設定保存
				var bounds = this.RestoreBounds;
				this.Storage["RestoreBounds"] = this.RestoreBounds;
				var windowState = this.RestoreWindowState;
				var windwState2 = this.WindowState;
				this.Storage["WindowState"] = this.RestoreWindowState;
			}, this);

			messenger.Subscribe<WindowMessages.CloseMessage>(m => {
				this.Application.RemoveMainWindow(this);
			}, this);

			this.Application.AddMainWindow(this);
		}

		private string GetStoragePrefix() {
			return (this.ID.IsNullOrEmpty() ? "MainWindow" : "MainWindow_" + this.ID) + ".";
		}

		public IStorage Storage { get; private set; }

		#region Property

		public PanelCollectionViewModel Panels {
			get {
				return this._Panels;
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

	}
}
