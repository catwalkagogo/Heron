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

namespace CatWalk.Heron.View {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : RibbonWindow {
		public MainWindow() {
			InitializeComponent();
		}

		public Ribbon Ribbon {
			get {
				return this._Ribbon;
			}
		}
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
