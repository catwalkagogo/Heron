using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.ViewModel.Windows;

namespace CatWalk.Heron.Windows {
	public class WindowsPlugin : Plugin{
		protected override void OnLoaded(PluginEventArgs e) {
			base.OnLoaded(e);

			var app = e.Application;

			app.Messenger.Subscribe<WindowMessages.ArrangeWindowsMessage>((Action<WindowMessages.ArrangeWindowsMessage>)OnArrangeWindowsMessage, app);

			app.ViewFactory.Register<MainWindowViewModel>((type, vm) => {
				var window = new MainWindow(this);
				window.DataContext = vm;
				window.Show();
				return window;
			});
		}

		public override bool CanUnload(Application app) {
			return false;
		}

		private static void OnArrangeWindowsMessage(WindowMessages.ArrangeWindowsMessage m) {
			WindowUtility.ArrangeMainWindows(m.Mode);
		}

		public override PluginPriority Priority {
			get {
				return PluginPriority.Builtin;
			}
		}

		public override string DisplayName {
			get {
				return "WPF View";
			}
		}
	}
}
