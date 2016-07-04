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
		public const string PanelTemplateFactoryKey = "Heron.Windows.PanelTemplateFactory";
		public const string EntryListViewFactoryKey = "Heron.Windows.EntryListViewFactory";
		public const string EntryImageFactoryKey = "Heron.Windows.EntryImageFactory";

		protected override void OnLoaded(PluginEventArgs e) {
			base.OnLoaded(e);

			Current = this;

			var app = e.Application;

			app.Messenger.Subscribe<WindowMessages.ArrangeWindowsMessage>((Action<WindowMessages.ArrangeWindowsMessage>)OnArrangeWindowsMessage, app);

			InitializeFactory(app);
		}

		private void InitializeFactory(Application app) {
			app.MainWindowViewFactory.Register(
				vm => vm is MainWindowViewModel,
				vm => {
					var window = new MainWindow(this);
					window.DataContext = vm;
					window.Show();
					return window;
				});

			app.Factories[PanelTemplateFactoryKey] = new Factory<SystemEntryViewModel, DataTemplate>();
			app.Factories[EntryListViewFactoryKey] = new Factory<SystemEntryViewModel, ViewBase>();
			app.Factories[EntryImageFactoryKey] = new Factory<SystemEntryViewModel, ImageSource>();

			EntryImageFactory.Register(p => true, p => {
				// Shellからデフォルトアイコンを使用
				if(p.Entry.IsDirectory) {
					return IconUtility.GetShellIcon(IconUtility.FolderIconIndex, Win32::IconSize.Small);
				} else {
					return IconUtility.GetShellIcon(IconUtility.FileIconIndex, Win32::IconSize.Small);
				}
			}, Factory.PriorityLowest);

		}


		public Factory<SystemEntryViewModel, DataTemplate> PanelTemplateFactory {
			get {
				return this.Application.GetFactory<SystemEntryViewModel, DataTemplate>(PanelTemplateFactoryKey);
			}
		}

		public Factory<SystemEntryViewModel, ViewBase> EntryListViewFactory {
			get {
				return this.Application.GetFactory<SystemEntryViewModel, ViewBase>(EntryListViewFactoryKey);
			}
		}
		public Factory<SystemEntryViewModel, ImageSource> EntryImageFactory {
			get {
				return this.Application.GetFactory<SystemEntryViewModel, ImageSource>(EntryImageFactoryKey);
			}
		}

		public override bool CanUnload(Application app) {
			return false;
		}

		private static void OnArrangeWindowsMessage(WindowMessages.ArrangeWindowsMessage m) {
			WindowUtility.ArrangeMainWindows(m.Mode);
		}

		public override int Priority {
			get {
				return PRIORITY_BUILTIN;
			}
		}

		public override string DisplayName {
			get {
				return "WPF View";
			}
		}
	}
}
