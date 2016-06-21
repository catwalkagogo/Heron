/*
	$Id: ExifEntryTypes.cs 188 2011-03-25 20:12:22Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	[Flags]
	public enum ExifEntryTypes : uint{
		Ifd0                 = 0x0001,
		MainIfd              = 0x0002,
		InterOperabilityIfd  = 0x0004,
		IfdThumbnail         = 0x0008,
		GpsIfd               = 0x0010,
		MakerNoteIfd         = 0x0020,
		Maker                = 0x010F,
		Model                = 0x0110,
		Orientation          = 0x0112,
		ExposureTime         = 0x829A,
		FNumber              = 0x829D,
		DateTimeOriginal     = 0x9003,
		ShutterSpeed         = 0x9201,
		Aperture             = 0x9202,
		MaxAperture          = 0x9205,
		FocalLength          = 0x920A,
	}
}
