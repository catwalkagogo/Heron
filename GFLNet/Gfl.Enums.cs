/*
	$Id: Gfl.Enums.cs 319 2014-01-03 04:23:35Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	public partial class Gfl : CatWalk.Win32.InteropObject{
		#region enum
		
		internal enum Error : ushort{
			None = 0,
			FileOpen = 1,
			FileRead = 2,
			FileCreate = 3,
			FileWrite = 4,
			NoMemory = 5,
			UnknownFormat = 6,
			BadBitmap = 7,
			BadFormatIndex = 10,
			BadParameters = 50,
			UnknownError = 255,
		}
		
		[Flags]
		internal enum Status : uint{
			None = 0,
			Read = 1,
			Write = 2,
		}
		
		internal enum ChannelOrder : ushort{
			Interleaved = 0,
			Sequential = 1,
			Separate = 2,
		}
		
		internal enum ChannelType : ushort{
			GreyScale  = 0,
			Rgb        = 1,
			Bgr        = 2,
			Rgba       = 3,
			Abgr       = 4,
			Cmy        = 5,
			CMYK       = 6,
		}
		
		internal enum LutType : ushort{
			To8Bits  = 1,
			To10Bits = 2,
			To12Bits = 3,
			To16Bits = 4,
		}
		
		internal enum ByteOrder : byte{
			Default = 0,
			LSBF = 1,
			MDBF = 2,
		}
		
		internal enum GetExifOptions : uint{
			None = 0,
			WantMakerNotes = 1,
		}
		
		#endregion
	}
}
