/*
	$Id: Callbacks.cs 189 2011-03-26 19:07:34Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);

	public class ProgressEventArgs : EventArgs{
		public int ProgressPercentage{get; private set;}

		public ProgressEventArgs(int percent){
			this.ProgressPercentage = percent;
		}
	}
}
