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

		public MainWindowViewModel(Application app) : base(app) {
			this._Panels = new PanelCollectionViewModel(app);
			this.Children.Add(this._Panels);

			var messenger = this.Messenger;
			messenger.Subscribe<Messages.DataContextAttachedMessage>(m => {
				// 初期化処理

				// 設定読み込み
				var rect = this.Application.Configuration.Get<Rect<double>>(this.GetConfigurationKey("RestoreBounds"), Rect<double>.Empty);
				if (rect != Rect<double>.Empty) {
					this.RestoreBounds = rect;
				}
				var windowState = this.Application.Configuration.Get<WindowState>(this.GetConfigurationKey("WindowState"), WindowState.Normal);
				this.WindowState = windowState;
			}, this);

			messenger.Subscribe<WindowMessages.ClosingMessage>(m => {
				// 設定保存
				var bounds = this.RestoreBounds;
				this.Application.Configuration[this.GetConfigurationKey("RestoreBounds")] = this.RestoreBounds;
				var windowState = this.RestoreWindowState;
				var windwState2 = this.WindowState;
				this.Application.Configuration[this.GetConfigurationKey("WindowState")] = this.RestoreWindowState;
			}, this);

			messenger.Subscribe<WindowMessages.CloseMessage>(m => {
				this.Application.RemoveMainWindow(this);
			}, this);

			this.Application.AddMainWindow(this);
		}

		protected string GetConfigurationKey(string prop) {
			var baseName = "MainWindow";
			if(this.Index > 0) {
				return baseName + "." + this.Index + "." + prop;
			}else {
				return baseName + "." + prop;
			}
		}

		#region IJobManagerSite Members

		public IJobManager JobManager {
			get {
				return this._JobManager;
			}
		}

		#endregion

		private int _Index = -1;
		public int Index {
			get {
				return this._Index;
			}
			set {
				this._Index = value;
				this.OnPropertyChanged("Index");
			}
		}
	}
}
