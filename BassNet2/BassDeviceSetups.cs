/*
	$Id: BassDeviceSetups.cs 159 2011-03-07 07:14:41Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassNet2 {
	[Flags]
	public enum BassDeviceSetups : uint{
		None = 0,
		Resolution8Bits = 1,
		Mono = 2,
		Enable3D = 4,
		CalculateLatency = 256,
		ControlPanelSpeakers = 1024,
		ForceSpeakers = 2048,
		IgnoreSpeakerArrangement = 4096,
	}
}
