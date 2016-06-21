/*
	$Id: ProgressWindow.xaml.cs 137 2010-12-19 10:01:30Z cs6m7y@bma.biglobe.ne.jp $
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

namespace CatWalk.Windows{
	public partial class ProgressWindow : Window{
		public ProgressWindow(){
			this.InitializeComponent();
		}
		
		#region プロパティ
		
		public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(ProgressWindow));
		public string Message{
			get{
				return (string)this.GetValue(MessageProperty);
			}
			set{
				this.SetValue(MessageProperty, value);
			}
		}
		
		public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(ProgressWindow));
		public bool IsIndeterminate{
			get{
				return (bool)this.GetValue(IsIndeterminateProperty);
			}
			set{
				this.SetValue(IsIndeterminateProperty, value);
			}
		}
		
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(ProgressWindow));
		public double Value{
			get{
				return (double)this.GetValue(ValueProperty);
			}
			set{
				this.SetValue(ValueProperty, value);
			}
		}
		
		#endregion
	}
}