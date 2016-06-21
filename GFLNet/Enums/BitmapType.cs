/*
	$Id: BitmapType.cs 188 2011-03-25 20:12:22Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet{
	[Flags]
	public enum BitmapType : ushort{
		Binary = 0x0001,
		Grey   = 0x0002,
		Colors = 0x0004,
		Rgb    = 0x0010,
		Rgba   = 0x0020,
		Bgr    = 0x0040,
		Abgr   = 0x0080,
		Bgra   = 0x0100,
		Argb   = 0x0200,
		Cmyk   = 0x0400,
	}
}
