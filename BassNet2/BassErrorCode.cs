/*
	$Id: BassErrorCode.cs 159 2011-03-07 07:14:41Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassNet2 {
	public enum BassErrorCode : int{
		None = 0,
		Memory = 1,
		FileOpen = 2,
		Driver = 3,
		BufferLost = 4,
		Handle = 5,
		Format = 6,
		Position = 7,
		Initialization = 8,
		Starting = 9,
		Already = 14,
		NoChannel = 18,
		IllegalType = 19,
		IllegalParameter = 20,
		No3D = 21,
		NoEax = 22,
		Device = 23,
		NotPlaying = 24,
		Frequency = 25,
		NotFile = 27,
		NoHardware = 29,
		Empty = 31,
		NoNet = 32,
		Creation = 33,
		NoEffects = 34,
		NotAvailable = 37,
		DecodingChannel = 38,
		DirectX = 39,
		TimeOut = 40,
		FileFormat = 41,
		Speaker = 42,
		Version = 43,
		Codec = 44,
		Ended = 45,
		Unknown = -1,
	}

}
