/*
	$Id: BassDevice.cs 159 2011-03-07 07:14:41Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassNet2 {
	public struct BassDevice{
		public string Name{get; private set;}
		public string Device{get; private set;}
		public bool IsEnabled{get; private set;}
		public bool IsDefault{get; private set;}
		public bool IsInitialized{get; private set;}
		internal int Index{get; private set;}
		
		internal BassDevice(int index, Bass.DeviceInfo device) : this(){
			this.Index = index;
			this.Name = device.Name;
			this.Device = device.Device;
			this.IsEnabled = ((device.States & Bass.DeviceStates.Enabled) > 0);
			this.IsDefault = ((device.States & Bass.DeviceStates.Default) > 0);
			this.IsInitialized = ((device.States & Bass.DeviceStates.Initialized) > 0);
		}
	}
}
