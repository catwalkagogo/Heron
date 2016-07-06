using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.ViewModel.Windows;
using CatWalk.Heron.ViewModel.IOSystem;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CatWalk.Heron.Windows.Interop;

namespace CatWalk.Heron.Windows {
	using Win32 = CatWalk.Win32;

	public class WindowsPlugin : Plugin{
		public static WindowsPlugin Current {get; private set;	}

		protected override void OnLoaded(PluginEventArgs e) {
			base.OnLoaded(e);

			Current = this;

			var app = e.Application;

			app.Messenger.Subscribe<WindowMessages.ArrangeWindowsMessage>((Action<WindowMessages.ArrangeWindowsMessage>)OnArrangeWindowsMessage, app);

			InitializeFactory(app);
		}

		#region CreatedEvent

		public event EventHandler<FrameworkElementCreatedEventArgs> FrameworkElementCreated;

		internal void OnFrameworkElementCreated(FrameworkElement element) {
			element.ThrowIfNull(nameof(element));

			var handler = this.FrameworkElementCreated;
			if(handler != null) {
				handler(this, new FrameworkElementCreatedEventArgs(element));
			}
		}

		#endregion

		#region Factory

		private void InitializeFactory(Application app) {
			app.MainWindowViewFactory.Register(
				vm => vm is MainWindowViewModel,
				vm => {
					var window = new MainWindow(this);
					window.DataContext = vm;
					window.Show();
					return window;
				});

			app.Factories[Factories.PanelTemplateFactoryKey] = new Factory<SystemEntryViewModel, DataTemplate>();
			app.Factories[Factories.EntryListViewFactoryKey] = new Factory<SystemEntryViewModel, ViewBase>();
			app.Factories[Factories.EntryImageFactoryKey] = new Factory<SystemEntryViewModel, Size<double>, ImageSource>();

			EntryImageFactory.Register((p, s) => true, (p, s) => {
				// Shellからデフォルトアイコンを使用
				if(p.Entry.IsDirectory) {
					return IconUtility.GetShellIcon(IconUtility.FolderIconIndex, Win32::Shell.ShellIcon.GetIconSizeFromSize(s));
				} else {
					return IconUtility.GetShellIcon(IconUtility.FileIconIndex, Win32::Shell.ShellIcon.GetIconSizeFromSize(s));
				}
			}, Factory.PriorityLowest);

		}

		public Factory<SystemEntryViewModel, DataTemplate> PanelTemplateFactory {
			get {
				return this.Application.GetFactory<SystemEntryViewModel, DataTemplate>(Factories.PanelTemplateFactoryKey);
			}
		}

		public Factory<SystemEntryViewModel, ViewBase> EntryListViewFactory {
			get {
				return this.Application.GetFactory<SystemEntryViewModel, ViewBase>(Factories.EntryListViewFactoryKey);
			}
		}
		public Factory<SystemEntryViewModel, Size<double>, ImageSource> EntryImageFactory {
			get {
				return this.Application.GetFactory<SystemEntryViewModel, Size<double>, ImageSource>(Factories.EntryImageFactoryKey);
			}
		}

		#endregion

		public override bool CanUnload(Application app) {
			return false;
		}

		private static void OnArrangeWindowsMessage(WindowMessages.ArrangeWindowsMessage m) {
			WindowUtility.ArrangeMainWindows(m.Mode);
		}

		public override int Priority {
			get {
				return PriorityBuiltin;
			}
		}

		public override string DisplayName {
			get {
				return "WPF View";
			}
		}
	}

	public class FrameworkElementCreatedEventArgs : EventArgs {
		public FrameworkElement Element { get; private set; }

		public FrameworkElementCreatedEventArgs(FrameworkElement element) {
			element.ThrowIfNull(nameof(element));

			this.Element = element;
		}
	}
}
