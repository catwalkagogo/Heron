/*
	$Id: Origin.cs 188 2011-03-25 20:12:22Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	public enum Origin : ushort{
		TopLeft = 0,
		BottomLeft = 2,
		TopRight = 1,
		BottomRight = 3,
	}
}