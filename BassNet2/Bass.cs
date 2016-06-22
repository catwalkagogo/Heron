/*
	$Id: Bass.cs 159 2011-03-07 07:14:41Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassNet2 {
	public static partial class Bass{
		private static bool initialized = false;
		
		public static bool IsInitialized{
			get{
				return initialized;
			}
		}
		
		public static void Initialize(int freq, BassDeviceSetups flags, IntPtr hwnd){
			if(Bass.Init(-1, (uint)freq, flags, hwnd, IntPtr.Zero)){	// 成功
				initialized = true;
			}else{
				BassErrorCode error = Bass.GetErrorCode();
				if(error == BassErrorCode.Already){
					throw new InvalidOperationException();
				}else{
					throw new BassException(error);
				}
			}
		}
		
		public static void Free(){
			if(Bass.FreeInternal()){
				initialized = false;
			}else{
				throw new InvalidOperationException();
			}
		}
		
		public static void Start(){
			if(Bass.StartInternal()){
				return;
			}else{
				throw new BassException(Bass.GetErrorCode());
			}
		}
		
		public static void Pause(){
			if(Bass.PauseInternal()){
				return;
			}else{
				throw new BassException(Bass.GetErrorCode());
			}
		}
		
		public static void Stop(){
			if(Bass.StopInternal()){
				return;
			}else{
				throw new BassException(Bass.GetErrorCode());
			}
		}
		
		public static float Volume{
			get{
				float volume = Bass.GetVolume();
				if(volume != -1){
					return volume;
				}else{
					throw new BassException(Bass.GetErrorCode());
				}
			}
			set{
				if(Bass.SetVolume(value)){
					return;
				}else{
					throw new BassException(Bass.GetErrorCode());
				}
			}
		}
		
		public static BassDevice[] GetDevices(){
			List<BassDevice> devices = new List<BassDevice>();
			Bass.DeviceInfo device;
			for(int i = 0; Bass.GetDeviceInfo(i, out device); i++){
				devices.Add(new BassDevice(i, device));
			}
			return devices.ToArray();
		}
		
		public static BassDevice Device{
			get{
				int device = Bass.GetCurrentDevice();
				if(device != -1){
					Bass.DeviceInfo info;
					Bass.GetDeviceInfo(device, out info);
					return new BassDevice(device, info);
				}else{
					throw new InvalidOperationException();
				}
			}
			set{
				if(Bass.SetDevice(value.Index)){
					return;
				}else{
					throw new BassException(Bass.GetErrorCode());
				}
			}
		}
	}
}
