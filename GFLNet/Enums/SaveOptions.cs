/*
	$Id: SaveOptions.cs 188 2011-03-25 20:12:22Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	[Flags]
	public enum SaveOptions : uint{
		ReplaceExtension = 0x00000001,
		WantFilename     = 0x00000002,
		SaveAnyway       = 0x00000004,
		SaveIccProfile   = 0x00000008,
	}
}
