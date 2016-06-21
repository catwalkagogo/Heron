/*
	$Id: Gfl.Structs.cs 319 2014-01-03 04:23:35Z catwalkagogo@gmail.com $
*/
using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace GflNet {
	public partial class Gfl : CatWalk.Win32.InteropObject{
		#region Struct
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct GflBitmap{
			public BitmapType Type;
			public Origin Origin;
			public Int32 Width;
			public Int32 Height;
			public UInt32 BytesPerLine;
			public Int16 LinePadding;
			public UInt16 BitsPerComponent;
			public UInt16 ComponentsPerPixel;
			public UInt16 BytesPerPixel;
			public UInt16 XDpi;
			public UInt16 YDpi;
			public Int16 TransparentIndex;
			public Int16 Reserved;
			public Int32 ColorUsed;
			public IntPtr ColorMap;
			public IntPtr Data;
			public string Comment;
			public IntPtr MetaData;
			
			public Int32 XOffset;
			public Int32 YOffset;
			public string Name;
		}
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct GflColor{
			public UInt16 Red;
			public UInt16 Green;
			public UInt16 Blue;
			public UInt16 Alpha;
		}
		
		[StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
		internal struct GflColorMap{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public byte[] Red;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public byte[] Green;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public byte[] Blue;
		}
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct GflFormatInformation{
			public int Index;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
			public string Name;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string Description;
			public Status Status;
			public int NumberOfExtension;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=16*8)]
			public string Extension;
		}
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct GflLoadParams{
			public LoadOptions Options;
			public int FormatIndex;
			public int ImageWanted;
			public Origin Origin;
			public BitmapType ColorModel;
			public uint LinePadding;
			public byte DefaultAlpha;
			
			public byte PsdNoAlphaForNonLayer;
			public byte PngComposeWithAlpha;
			public byte WMFHighResolution;
			
			// RAW/YUB only
			public int Width;
			public int Height;
			public uint Offset;
			
			// RAW only
			public ChannelOrder ChannelOrder;
			public ChannelType ChannelType;
			
			// PCD only
			public UInt16 PcdBase;
			
			// EPS/PS/AI/PDF Only
			public UInt16 EpsDpi;
			public int EpsWidth;
			public int EpsHeight;
			
			// DPX/Cineon only
			public LutType LutType;
			public UInt16 Reserved3;
			public IntPtr LutData;
			public string LutFilename;
			
			// Camera RAW only
			public byte CameraRawUseAutomaticBalance;
			public byte CameraRawUseCameraBalance;
			public byte CameraRawHighlight;
			public byte Reserved4;
			public float CameraRawGamma;
			public float CameraRawBrightness;
			public float CameraRawRedScaling;
			public float CameraRawBlueScaling;
			
			public GflLoadCallbacks Callbacks;
			
			public IntPtr UserParams;
		}
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct GflLoadCallbacks{
			public ReadCallback Read;
			public TellCallback Tell;
			public SeekCallback Seek;
			
			public AllocCallback AllocateBitmap; /* Global or not???? */
			public IntPtr AllocateBitmapParams;
			
			public ProgressCallback Progress;
			public IntPtr ProgressParams;
			
			public WantCancelCallback WantCancel;
			public IntPtr WantCancelParams;
			
			public IntPtr SetLine;
			public IntPtr SetLineParams;
		}
		
		internal delegate uint ReadCallback(IntPtr handle, IntPtr buffer, uint size);
		internal delegate uint TellCallback(IntPtr handle);
		internal delegate uint SeekCallback(IntPtr handle, int offset, SeekOrigin origin);
		internal delegate void ProgressCallback(int percent, IntPtr userParams);
		internal delegate IntPtr AllocCallback(IntPtr size, IntPtr userParam);
		internal delegate bool WantCancelCallback(IntPtr userParams);

		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct GflFileInformation{
			public BitmapType  Type;   /* Not used */
			public Origin      Origin;
			public int         Width;
			public int         Height;
			public int         FormatIndex;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
			public string      FormatName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string      Description;
			public UInt16      XDpi;
			public UInt16      YDpi;
			public UInt16      BitsPerComponent;  /* 1, 8, 10, 12, 16 */
			public UInt16      ComponentsPerPixel;/* 1, 3, 4  */
			public int         NumberOfImages;
			public uint        FileSize;
			public ColorModel  ColorModel;
			public Compression Compression;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string      CompressionDescription;
			public int         XOffset;
			public int         YOffset;
			public int[]      ExtraInfos;
		}
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct GflSaveParams{
			public SaveOptions Options;
			public int FormatIndex;
			
			public Compression Compression;
			public Int16 Quality;
			public Int16 CompressionLevel; // for PNG 1 to 7
			public bool Interlaced; // for GIF
			public bool Progressive; // for JPEG
			public bool OptimizeHuffmanTable; // for JPEG
			public bool InAscii; // for PPM
			
			// for DPX/Cineon
			public LutType LutType;
			public ByteOrder DpxByteOrder;
			public byte CompressRatio; // for JPEG2000
			public UInt32 MaxFileSize; // for JPEG2000
			
			public IntPtr LutData;
			public string LutFilename;
			
			// for RAW/YUV
			public UInt32 Offset;
			public ChannelOrder ChannelOrder;
			public ChannelType ChannelType;
			
			public GflSaveCallbacks Callbacks;
			
			public IntPtr UserParams;
		}
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct GflSaveCallbacks{
			public IntPtr Read;
			public IntPtr Tell;
			public IntPtr Seek;
			
			public IntPtr GetLine;
			public IntPtr GetLineParams;
		}
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct GflExifData{
			public int NumberOfItems;
			[MarshalAs(UnmanagedType.LPArray)]
			public GflExifEntry[] ItemList;
		}
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		internal struct GflExifEntry{
			public ExifEntryTypes Types;
			public UInt32 Tag;
			[MarshalAs(UnmanagedType.LPStr)]
			public string Name;
			[MarshalAs(UnmanagedType.LPStr)]
			public string Value;
		}
		
		#endregion
	}
}
