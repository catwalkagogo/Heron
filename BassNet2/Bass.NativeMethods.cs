/*
	$Id: Bass.NativeMethods.cs 159 2011-03-07 07:14:41Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using BassNet2.Channels;

namespace BassNet2{
	public static partial class Bass{
		const string BassDllName = "bass.dll";
		
		[DllImport(BassDllName, EntryPoint = "BASS_Init", CharSet = CharSet.Auto)]
		internal static extern bool Init(int device, uint freq, BassDeviceSetups flags, IntPtr hwnd, IntPtr clsid);
		
		[DllImport(BassDllName, EntryPoint = "BASS_Free", CharSet = CharSet.Auto)]
		internal static extern bool FreeInternal();
		
		[DllImport(BassDllName, EntryPoint = "BASS_GetVolume", CharSet = CharSet.Auto)]
		internal static extern float GetVolume();
		
		[DllImport(BassDllName, EntryPoint = "BASS_SetVolume", CharSet = CharSet.Auto)]
		internal static extern bool SetVolume(float volume);
		
		[DllImport(BassDllName, EntryPoint = "BASS_Start", CharSet = CharSet.Auto)]
		internal static extern bool StartInternal();
		
		[DllImport(BassDllName, EntryPoint = "BASS_Pause", CharSet = CharSet.Auto)]
		internal static extern bool PauseInternal();
		
		[DllImport(BassDllName, EntryPoint = "BASS_Stop", CharSet = CharSet.Auto)]
		internal static extern bool StopInternal();
		
		[DllImport(BassDllName, EntryPoint = "BASS_SetDevice", CharSet = CharSet.Auto)]
		internal static extern bool SetDevice(int device);
		
		[DllImport(BassDllName, EntryPoint = "BASS_GetDevice", CharSet = CharSet.Auto)]
		internal static extern int GetCurrentDevice();
		
		[DllImport(BassDllName, EntryPoint = "BASS_GetDeviceInfo", CharSet = CharSet.Auto)]
		internal static extern bool GetDeviceInfo(int device, out DeviceInfo deviceInfo);
		
		[DllImport(BassDllName, EntryPoint = "BASS_ErrorGetCode", CharSet = CharSet.Auto)]
		internal static extern BassErrorCode GetErrorCode();
		
		[DllImport(BassDllName, EntryPoint = "BASS_StreamCreateFile", CharSet = CharSet.Auto)]
		internal static extern IntPtr CreateStreamFromFile(bool mem, [MarshalAs(UnmanagedType.LPStr)] string file, long offset, long length, Options options);
		
		[DllImport(BassDllName, EntryPoint = "BASS_StreamFree", CharSet = CharSet.Auto)]
		internal static extern bool FreeStream(IntPtr handle);
		
		[DllImport(BassDllName, EntryPoint = "BASS_ChannelGetLength", CharSet = CharSet.Auto)]
		internal static extern long GetChannelLength(IntPtr handle, PositionMode mode);
		
		[DllImport(BassDllName, EntryPoint = "BASS_ChannelBytes2Seconds", CharSet = CharSet.Auto)]
		internal static extern double ChannelBytes2Seconds(IntPtr handle, long bytes);
		
		[DllImport(BassDllName, EntryPoint = "BASS_ChannelSeconds2Bytes", CharSet = CharSet.Auto)]
		internal static extern long ChannelSeconds2Bytes(IntPtr handle, double pos);
		
		[DllImport(BassDllName, EntryPoint = "BASS_ChannelPlay", CharSet = CharSet.Auto)]
		internal static extern bool PlayChannel(IntPtr handle, bool restart);
		
		[DllImport(BassDllName, EntryPoint = "BASS_ChannelPause", CharSet = CharSet.Auto)]
		internal static extern bool PauseChannel(IntPtr handle);
		
		[DllImport(BassDllName, EntryPoint = "BASS_ChannelStop", CharSet = CharSet.Auto)]
		internal static extern bool StopChannel(IntPtr handle);
		
		[DllImport(BassDllName, EntryPoint = "BASS_ChannelSetPosition", CharSet = CharSet.Auto)]
		internal static extern bool SetChannelPosition(IntPtr handle, long pos, PositionMode mode);
		
		[DllImport(BassDllName, EntryPoint = "BASS_ChannelGetPosition", CharSet = CharSet.Auto)]
		internal static extern long GetChannelPosition(IntPtr handle, PositionMode mode);
		
		[DllImport(BassDllName, EntryPoint = "BASS_ChannelGetLevel", CharSet = CharSet.Auto)]
		internal static extern int GetChannelLevel(IntPtr handle);
		
		[DllImport(BassDllName, EntryPoint = "BASS_ChannelIsActive", CharSet = CharSet.Auto)]
		internal static extern ChannelState GetChannelState(IntPtr handle);
		
		[DllImport(BassDllName, EntryPoint = "BASS_ChannelGetInfo", CharSet = CharSet.Auto)]
		internal static extern bool GetChannelInfo(IntPtr handle, out ChannelInfo info);
		
		[DllImport(BassDllName, EntryPoint = "BASS_ChannelSetFX", CharSet = CharSet.Auto)]
		internal static extern IntPtr SetChannelEffect(IntPtr handle, EffectType type, int priority);
		
		[DllImport(BassDllName, EntryPoint = "BASS_ChannelRemoveFX", CharSet = CharSet.Auto)]
		internal static extern bool RemoveChannelEffect(IntPtr hCh, IntPtr hFx);

		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct DeviceInfo{
			[MarshalAs(UnmanagedType.LPStr)]
			public string Name;
			[MarshalAs(UnmanagedType.LPStr)]
			public string Device;
			public DeviceStates States;
		}
		
		[Flags]
		internal enum DeviceStates : uint{
			None = 0,
			Enabled = 1,
			Default = 2,
			Initialized = 4,
		}
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct ChannelInfo{
			public int Frequency;
			public int Channels;
			public Options Options;
			public ChannelType ChannelType;
			public int OriginalResolution;
			public IntPtr Plugin;
			public IntPtr Sample;
			[MarshalAs(UnmanagedType.LPStr)]
			public string Filename;
		}
		
		internal enum EffectType : int{
			Chorus,
			Compressor,
			Distortion,
			Echo,
			Flanger,
			Gargle,
			I3DL2Reverb,
			ParamEq,
			Reverb,
		}

		[Flags]
		internal enum Options : uint{
			None = 0,
			Sample8Bits = 1,
			SampleFloat = 256,
			SampleMono = 2,
			SampleLoop = 4,
			Sample3D = 8,
			SampleSoftware = 16,
			SampleMuteMax = 32,
			SampleVam = 64,
			SampleEffects = 128,
			SampleOverVol = 0x10000,
			SampleOverPos = 0x20000,
			SampleOverDist = 0x30000,
		
			StreamPreScan = 0x20000,
			Mp3SetPos = StreamPreScan,
			StreamAutoFree = 0x40000,
			StreamRestRate = 0x80000,
			StreamBlock = 0x100000,
			StreamDecode = 0x200000,
			StreamStatus = 0x800000,
		
			SpeakerFront = 0x1000000,
			SpeakerRear = 0x2000000,
			SpeakerCenterLfe = 0x3000000,
			SpeakerRear2 = 0x4000000,
			Speaker1     =  1<<24,
			Speaker2     =  2<<24,
			Speaker3     =  3<<24,
			Speaker4     =  4<<24,
			Speaker5     =  5<<24,
			Speaker6     =  6<<24,
			Speaker7     =  7<<24,
			Speaker8     =  8<<24,
			Speaker9     =  9<<24,
			Speaker10    = 10<<24,
			Speaker11    = 11<<24,
			Speaker12    = 12<<24,
			Speaker13    = 13<<24,
			Speaker14    = 14<<24,
			Speaker15    = 15<<24,
			SpeakerLeft = 0x10000000,
			SpeakerRight = 0x20000000,
			Unicode = 0x80000000,
		}

		internal enum PositionMode : uint{
			Byte = 0,
			MusicOrder = 1,
		}
	}
}
