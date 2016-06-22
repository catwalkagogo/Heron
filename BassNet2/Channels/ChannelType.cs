/*
	$Id: ChannelType.cs 159 2011-03-07 07:14:41Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassNet2.Channels{
	public enum ChannelType : uint{
		Sample		= 1,
		Record		= 2,
		Stream		= 0x10000,
		StreamOgg	= 0x10002,
		StreamMP1	= 0x10003,
		StreamMP2	= 0x10004,
		StreamMP3	= 0x10005,
		StreamAiff	= 0x10006,
		StreamCA	= 0x10007,
		StreamWav	= 0x40000, // WAVE flag, LOWORD=codec
		StreamWavPcm	= 0x50001,
		StreamWavFloat	= 0x50003,
		MusicMod	= 0x20000,
		MusicMtm	= 0x20001,
		MusicS3M	= 0x20002,
		MusicXM		= 0x20003,
		MusicIT		= 0x20004,
		MusicMO3	= 0x00100, // MO3 flag
	}
}
