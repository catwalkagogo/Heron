using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Windows.Controls.Ribbon;
using System.Windows.Interop;
using CatWalk.Win32;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using CatWalk.Heron.Windows.Controls;
using CatWalk.Heron.Configuration;
using System.ComponentModel;

namespace CatWalk.Heron.Windows {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		const string OUTPUT_ROW_HEIGHT_KEY = "OutputRowHeight";

		public WindowsPlugin Plugin { get; private set; }
		private HotKeyManager _HotKeyManager;
		private HwndSource _HwndSource;
		private CompositeDisposable _Disposables = new CompositeDisposable();

		public MainWindow(WindowsPlugin plugin) {
			plugin.ThrowIfNull("plugin");

			InitializeComponent();

			this.Plugin = plugin;

			var wint = new WindowInteropHelper(this);
			wint.EnsureHandle();
			this._HwndSource = HwndSource.FromHwnd(wint.Handle);
			this._HotKeyManager = new HotKeyManager(this._HwndSource.Handle);
			this._HwndSource.AddHook(this.WndProc);

			this._Disposables.Add(Observable.FromEventPattern<EventArgs>(this, nameof(this.Activated)).Subscribe(e => {
				LatestActiveWindow = this;
			}));

			this.LoadConfig();
		}

		private async Task LoadConfig() {
			var height = await this.Plugin.Storage.GetAsync<double>(OUTPUT_ROW_HEIGHT_KEY, 120);
			this._OutputRowDefinition.Height = new GridLength(height);
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			return this._HotKeyManager.WndProc(hwnd, msg, wParam, lParam, ref handled);
		}

		#region LastActiveWindow

		private static MainWindow _LatestMainWindow;
		public static MainWindow LatestActiveWindow {
			get {
				return _LatestMainWindow ?? WindowUtility.MainWindows.FirstOrDefault(win => win.IsActive) ?? WindowUtility.MainWindows.FirstOrDefault();
			}
			internal set {
				_LatestMainWindow = value;
			}
		}

		#endregion

		#region OnClosing

		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);

			// 設定保存
			this.Plugin.Storage[OUTPUT_ROW_HEIGHT_KEY] = this._OutputRowDefinition.Height.Value;
		}

		#endregion

		#region Property

		public HotKeyManager HotKeyManager {
			get {
				return this._HotKeyManager;
			}
		}

		public PanelList PanelList {
			get {
				return this._PanelList;
			}
		}

		#endregion

		#region SwitchWindowCommand

		private ReactiveCommand<Direction?> _SwitchWindowCommand;

		public ReactiveCommand<Direction?> SwitchWindowCommand {
			get {
				if(this._SwitchWindowCommand == null) {
					this._SwitchWindowCommand = new ReactiveCommand<Direction?>();
					this._SwitchWindowCommand.Subscribe<Direction?>(this.SwitchWindow);
				}
				return this._SwitchWindowCommand;
			}
		}

		public void SwitchWindow(Direction? mode) {
			if (!mode.HasValue) {
				mode = Direction.Next;
			}

			var windows = WindowUtility.MainWindows.OrderWindowByZOrder().ToArray();
			var screen = this.GetCurrentScreen();
			var dlg = new Dialogs.SelectWindowDialog() {
				ItemsSource = windows,
				Left = screen.ScreenArea.X,
				Top = screen.ScreenArea.Y,
			};

			if(mode == Direction.Previous) {
				dlg.SelectedValue = (windows.Length > 1) ? windows[windows.Length - 1] : windows[0];
			} else {
				dlg.SelectedValue = (windows.Length > 1) ? windows[1] : windows[0];
			}

			dlg.ShowDialog();
			var selected = dlg.SelectedValue as Window;
			if(selected != null){
				selected.Activate();
			}
		}	

		#endregion
	}

	public enum Direction{
		Next,
		Previous,
	}

	public class JobCountToProgressStateConverter : IValueConverter {
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(targetType == typeof(TaskbarItemProgressState)) {
				return ((int)value > 0) ? TaskbarItemProgressState.Normal : TaskbarItemProgressState.None;
			} else {
				return null;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}
}
