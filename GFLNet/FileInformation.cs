/*
	$Id: FileInformation.cs 279 2011-08-04 10:24:25Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GflNet{
	[Serializable]
	public class FileInformation{
		private Gfl gfl;
		private Lazy<Format> format;
		public int Width{get; private set;}
		public int Height{get; private set;}
		public int XDpi{get; private set;}
		public int YDpi{get; private set;}
		public int ImageCount{get; private set;}
		public string Description{get; private set;}
		public ColorModel ColorModel{get; private set;}
		public Compression Compression{get; private set;}
		public long Size{get; private set;}
		public int BitsPerComponent{get; private set;}
		public int ComponentsPerPixel{get; private set;}
		public string CompressionDescription{get; private set;}
		public int XOffset{get; private set;}
		public int YOffset{get; private set;}
		public int[] ExtraInfos{get; private set;}
		internal int FormatIndex{get; private set;}

		internal FileInformation(Gfl gfl, IntPtr pInfo){
			this.gfl = gfl;
			var info = (Gfl.GflFileInformation)Marshal.PtrToStructure(pInfo, typeof(Gfl.GflFileInformation));
			this.format = new Lazy<Format>(this.GetFormat);
			this.FormatIndex = info.FormatIndex;
			this.Width = info.Width;
			this.Height = info.Height;
			this.XDpi = info.XDpi;
			this.YDpi = info.YDpi;
			this.ImageCount = info.NumberOfImages;
			this.Description = info.Description;
			this.ColorModel = info.ColorModel;
			this.Compression = info.Compression;
			this.Size = info.FileSize;
			this.BitsPerComponent = info.BitsPerComponent;
			this.ComponentsPerPixel = info.ComponentsPerPixel;
			this.CompressionDescription = info.CompressionDescription;
			this.XOffset = info.XOffset;
			this.YOffset = info.YOffset;
			this.ExtraInfos = info.ExtraInfos;
			gfl.FreeFileInformation(pInfo);
		}

		private Format GetFormat(){
			var format = this.gfl.GetGflFormat(this.FormatIndex);
			this.gfl = null;
			return format;
		}
		
		public Format Format{
			get{
				return this.format.Value;
			}
		}
	}
}
