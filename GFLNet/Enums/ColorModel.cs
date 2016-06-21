/*
	$Id: ColorModel.cs 188 2011-03-25 20:12:22Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	public enum ColorModel : ushort{
		Rgb    = 0, // Red-Green-Blue 
		Grey   = 1, // Greyscale 
		Cmy    = 2, // Cyan-Magenta-Yellow 
		Cmyk   = 3, // Cyan-Magenta-Yellow-Black 
		YCbCr  = 4, // YCbCr 
		Yuv16  = 5, // YUV 16bits 
		Lab    = 6, // Lab 
		LogLuv = 7, // Log Luv 
		LogL   = 8, // Log L 
	}
}
