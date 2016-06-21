/*
	$Id: DialogCommands.cs 137 2010-12-19 10:01:30Z cs6m7y@bma.biglobe.ne.jp $
*/

using System;
using System.Windows;
using System.Windows.Input;

namespace CatWalk.Windows{
	public static class DialogCommands{
		public static readonly RoutedUICommand OK = new RoutedUICommand();
		public static readonly RoutedUICommand Cancel = new RoutedUICommand();
		public static readonly RoutedUICommand Apply = new RoutedUICommand();
	}
}