/*
	$Id: AboutBox.xaml.cs 210 2011-04-23 16:36:01Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Reflection;

namespace CatWalk.Windows{
	public partial class AboutBox : Window{
		public AboutBox() : this(Assembly.GetEntryAssembly()){
		}

		public AboutBox(Assembly asm){
			var asmName = asm.GetName();
			this.AppName = asmName.Name;
			this.AppDescription = asm.GetDescription();
			this.Version = asm.GetInformationalVersion();
			this.Copyright = asm.GetCopyright();
			//this.AppIcon = ShellIcon.GetIconImageSource(asm.Location, IconSize.Large);
			//this.Loaded += this.LoadedHandler;

			this.InitializeComponent();
		}
		/*
		private void LoadedHandler(object sender, EventArgs e){
		}
		*/
		private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}
		
		private void Close_Executed(object sender, ExecutedRoutedEventArgs e){
			this.DialogResult = true;
			this.Close();
		}

		#region プロパティ

		public static readonly DependencyProperty AppNameProperty = DependencyProperty.Register("AppName", typeof(string), typeof(AboutBox));
		public string AppName{
			get{
				return (string)this.GetValue(AppNameProperty);
			}
			set{
				this.SetValue(AppNameProperty, value);
			}
		}

		public static readonly DependencyProperty AppDescriptionProperty = DependencyProperty.Register("AppDescription", typeof(string), typeof(AboutBox));
		public string AppDescription{
			get{
				return (string)this.GetValue(AppDescriptionProperty);
			}
			set{
				this.SetValue(AppDescriptionProperty, value);
			}
		}

		public static readonly DependencyProperty AppIconProperty = DependencyProperty.Register("AppIcon", typeof(ImageSource), typeof(AboutBox));
		public ImageSource AppIcon{
			get{
				return (ImageSource)this.GetValue(AppIconProperty);
			}
			set{
				this.SetValue(AppIconProperty, value);
			}
		}

		public static readonly DependencyProperty VersionProperty = DependencyProperty.Register("Version", typeof(string), typeof(AboutBox));
		public string Version{
			get{
				return (string)this.GetValue(VersionProperty);
			}
			set{
				this.SetValue(VersionProperty, value);
			}
		}

		public static readonly DependencyProperty CopyrightProperty = DependencyProperty.Register("Copyright", typeof(string), typeof(AboutBox));
		public string Copyright{
			get{
				return (string)this.GetValue(CopyrightProperty);
			}
			set{
				this.SetValue(CopyrightProperty, value);
			}
		}

		public static readonly DependencyProperty AdditionalInformationsProperty = DependencyProperty.Register("AdditionalInformations", typeof(ICollection<KeyValuePair<string, string>>), typeof(AboutBox));
		public ICollection<KeyValuePair<string, string>> AdditionalInformations{
			get{
				return (ICollection<KeyValuePair<string, string>>)this.GetValue(AdditionalInformationsProperty);
			}
			set{
				this.SetValue(AdditionalInformationsProperty, value);
			}
		}

		#endregion
	}
}