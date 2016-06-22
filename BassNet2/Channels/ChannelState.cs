/*
	$Id: ChannelState.cs 159 2011-03-07 07:14:41Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassNet2.Channels{
	public enum ChannelState : uint{
		Stopped = 0,
		Playing = 1,
		Stalled = 2,
		Paused  = 3,
	}
}
