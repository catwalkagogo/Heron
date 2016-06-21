/*
	$Id: ColorMap.cs 182 2011-03-23 12:46:00Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GflNet {
	[Serializable]
	public class ColorMap : ReadOnlyCollection<Color>{
		internal ColorMap(IntPtr hMap) : base(new List<Color>(256)){
			var map = new Gfl.GflColorMap();
			Marshal.PtrToStructure(hMap, map);
			for(var i = 0; i < 256; i++){
				this.Items.Add(new Color(map.Red[i], map.Green[i], map.Blue[i]));
			}
		}
	}
}
