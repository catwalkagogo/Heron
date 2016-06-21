using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GflNet {
	public partial class GflExtended : CatWalk.Win32.InteropObject{

		#region Colors

		private GflBitmapFuncInt32 _SepiaDelegate;
		internal Gfl.Error Sepia(IntPtr pSrc, ref IntPtr dst, int percentage){
			this.ThrowIfDisposed();
			if(this._SepiaDelegate == null){
				this._SepiaDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflSepia");
			}
			return this._SepiaDelegate(pSrc, ref dst, percentage);
		}

		private GflBitmapFuncInt32GflColor _SepiaExDelegate;
		internal Gfl.Error Sepia(IntPtr pSrc, ref IntPtr dst, int percentage, ref Gfl.GflColor color){
			this.ThrowIfDisposed();
			if(this._SepiaExDelegate == null){
				this._SepiaExDelegate = this.LoadMethod<GflBitmapFuncInt32GflColor>("gflSepia");
			}
			return this._SepiaExDelegate(pSrc, ref dst, percentage, ref color);
		}
		/*
		private GflBitmapFuncSwapColors _SwapColorsDelegate;
		internal Gfl.Error SwapColors(IntPtr pSrc, ref IntPtr dst, SwapColorsFilter filter){
			this.ThrowIfDisposed();
			if(this._SwapColorsDelegate == null){
				this._SwapColorsDelegate = this.LoadMethod<GflBitmapFuncSwapColors>("gflSwapColors");
			}
			return this._SwapColorsDelegate(pSrc, ref dst, filter);
		}
		*/
		#endregion

		#region Colors Destructive
		/*
		private GflBitmapFuncDestSwapColors _SwapColorsDestDelegate;
		internal Gfl.Error SwapColors(IntPtr pSrc, SwapColorsFilter filter){
			this.ThrowIfDisposed();
			if(this._SwapColorsDestDelegate == null){
				this._SwapColorsDestDelegate = this.LoadMethod<GflBitmapFuncDestSwapColors>("gflSwapColors");
			}
			return this._SwapColorsDestDelegate(pSrc, IntPtr.Zero, filter);
		}
		*/
		private GflBitmapFuncDestInt32 _SepiaDestDelegate;
		internal Gfl.Error Sepia(IntPtr pSrc, int percentage){
			this.ThrowIfDisposed();
			if(this._SepiaDestDelegate == null){
				this._SepiaDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflSepia");
			}
			return this._SepiaDestDelegate(pSrc, IntPtr.Zero, percentage);
		}

		private GflBitmapFuncDestInt32GflColor _SepiaExDestDelegate;
		internal Gfl.Error Sepia(IntPtr pSrc, int percentage, ref Gfl.GflColor color){
			this.ThrowIfDisposed();
			if(this._SepiaExDestDelegate == null){
				this._SepiaExDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32GflColor>("gflSepia");
			}
			return this._SepiaExDestDelegate(pSrc, IntPtr.Zero, percentage, ref color);
		}

		#endregion

		#region Filters

		private GflBitmapFuncInt32 _AverageDelegate;
		internal Gfl.Error Average(IntPtr pSrc, ref IntPtr dst, int filterSize){
			this.ThrowIfDisposed();
			if(this._AverageDelegate == null){
				this._AverageDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflAverage");
			}
			return this._AverageDelegate(pSrc, ref dst, filterSize);
		}

		private GflBitmapFuncInt32 _SoftenDelegate;
		internal Gfl.Error Soften(IntPtr pSrc, ref IntPtr dst, int percentage){
			this.ThrowIfDisposed();
			if(this._SoftenDelegate == null){
				this._SoftenDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflSoften");
			}
			return this._SoftenDelegate(pSrc, ref dst, percentage);
		}

		private GflBitmapFuncInt32 _BlurDelegate;
		internal Gfl.Error Blur(IntPtr pSrc, ref IntPtr dst, int percentage){
			this.ThrowIfDisposed();
			if(this._BlurDelegate == null){
				this._BlurDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflBlur");
			}
			return this._BlurDelegate(pSrc, ref dst, percentage);
		}

		private GflBitmapFuncInt32 _GaussianBlurDelegate;
		internal Gfl.Error GaussianBlur(IntPtr pSrc, ref IntPtr dst, int filterSize){
			this.ThrowIfDisposed();
			if(this._GaussianBlurDelegate == null){
				this._GaussianBlurDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflGaussianBlur");
			}
			return this._GaussianBlurDelegate(pSrc, ref dst, filterSize);
		}

		private GflBitmapFuncInt32 _MaximumDelegate;
		internal Gfl.Error Maximum(IntPtr pSrc, ref IntPtr dst, int filterSize){
			this.ThrowIfDisposed();
			if(this._MaximumDelegate == null){
				this._MaximumDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflMaximum");
			}
			return this._MaximumDelegate(pSrc, ref dst, filterSize);
		}

		private GflBitmapFuncInt32 _MinimumDelegate;
		internal Gfl.Error Minimum(IntPtr pSrc, ref IntPtr dst, int filterSize){
			this.ThrowIfDisposed();
			if(this._MinimumDelegate == null){
				this._MinimumDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflMinimum");
			}
			return this._MinimumDelegate(pSrc, ref dst, filterSize);
		}

		private GflBitmapFuncInt32 _MedianBoxDelegate;
		internal Gfl.Error MedianBox(IntPtr pSrc, ref IntPtr dst, int filterSize){
			this.ThrowIfDisposed();
			if(this._MedianBoxDelegate == null){
				this._MedianBoxDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflMedianBox");
			}
			return this._MedianBoxDelegate(pSrc, ref dst, filterSize);
		}

		private GflBitmapFuncInt32 _MedianCrossDelegate;
		internal Gfl.Error MedianCross(IntPtr pSrc, ref IntPtr dst, int filterSize){
			this.ThrowIfDisposed();
			if(this._MedianCrossDelegate == null){
				this._MedianCrossDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflMedianCross");
			}
			return this._MedianCrossDelegate(pSrc, ref dst, filterSize);
		}

		private GflBitmapFuncInt32 _SharpenDelegate;
		internal Gfl.Error Sharpen(IntPtr pSrc, ref IntPtr dst, int percentage){
			this.ThrowIfDisposed();
			if(this._SharpenDelegate == null){
				this._SharpenDelegate = this.LoadMethod<GflBitmapFuncInt32>("gflSharpen");
			}
			return this._SharpenDelegate(pSrc, ref dst, percentage);
		}

		private GflBitmapFunc _EnhanceDetailDelegate;
		internal Gfl.Error EnhanceDetail(IntPtr pSrc, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._EnhanceDetailDelegate == null){
				this._EnhanceDetailDelegate = this.LoadMethod<GflBitmapFunc>("gflEnhanceDetail");
			}
			return this._EnhanceDetailDelegate(pSrc, ref dst);
		}

		private GflBitmapFunc _EnhanceFocusDelegate;
		internal Gfl.Error EnhanceFocus(IntPtr pSrc, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._EnhanceFocusDelegate == null){
				this._EnhanceFocusDelegate = this.LoadMethod<GflBitmapFunc>("gflEnhanceFocus");
			}
			return this._EnhanceFocusDelegate(pSrc, ref dst);
		}

		private GflBitmapFunc _FocusRestorationDelegate;
		internal Gfl.Error FocusRestoration(IntPtr pSrc, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._FocusRestorationDelegate == null){
				this._FocusRestorationDelegate = this.LoadMethod<GflBitmapFunc>("gflFocusRestoration");
			}
			return this._FocusRestorationDelegate(pSrc, ref dst);
		}

		private GflBitmapFunc _EdgeDetectLightDelegate;
		internal Gfl.Error EdgeDetectLight(IntPtr pSrc, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._EdgeDetectLightDelegate == null){
				this._EdgeDetectLightDelegate = this.LoadMethod<GflBitmapFunc>("gflEdgeDetectLight");
			}
			return this._EdgeDetectLightDelegate(pSrc, ref dst);
		}

		private GflBitmapFunc _EdgeDetectMediumDelegate;
		internal Gfl.Error EdgeDetectMedium(IntPtr pSrc, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._EdgeDetectMediumDelegate == null){
				this._EdgeDetectMediumDelegate = this.LoadMethod<GflBitmapFunc>("gflEdgeDetectMedium");
			}
			return this._EdgeDetectMediumDelegate(pSrc, ref dst);
		}

		private GflBitmapFunc _EdgeDetectHeavyDelegate;
		internal Gfl.Error EdgeDetectHeavy(IntPtr pSrc, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._EdgeDetectHeavyDelegate == null){
				this._EdgeDetectHeavyDelegate = this.LoadMethod<GflBitmapFunc>("gflEdgeDetectHeavy");
			}
			return this._EdgeDetectHeavyDelegate(pSrc, ref dst);
		}

		private GflBitmapFunc _EmbossDelegate;
		internal Gfl.Error Emboss(IntPtr pSrc, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._EmbossDelegate == null){
				this._EmbossDelegate = this.LoadMethod<GflBitmapFunc>("gflEmboss");
			}
			return this._EmbossDelegate(pSrc, ref dst);
		}

		private GflBitmapFunc _EmbossMoreDelegate;
		internal Gfl.Error EmbossMore(IntPtr pSrc, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._EmbossMoreDelegate == null){
				this._EmbossMoreDelegate = this.LoadMethod<GflBitmapFunc>("gflEmbossMore");
			}
			return this._EmbossMoreDelegate(pSrc, ref dst);
		}

		private GflBitmapFunc _ReduceNoiseDelegate;
		internal Gfl.Error Sepia(IntPtr pSrc, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._ReduceNoiseDelegate == null){
				this._ReduceNoiseDelegate = this.LoadMethod<GflBitmapFunc>("gflReduceNoise");
			}
			return this._ReduceNoiseDelegate(pSrc, ref dst);
		}

		private GflBitmapFuncInt32Int32Bool _DropShadowDelegate;
		internal Gfl.Error Sepia(IntPtr pSrc, ref IntPtr dst, int size, int depth, bool keepSize){
			this.ThrowIfDisposed();
			if(this._DropShadowDelegate == null){
				this._DropShadowDelegate = this.LoadMethod<GflBitmapFuncInt32Int32Bool>("gflDropShadow");
			}
			return this._DropShadowDelegate(pSrc, ref dst, size, depth, keepSize);
		}

		private GflBitmapFuncFilter _ConvolveDelegate;
		internal Gfl.Error Convolve(IntPtr pSrc, ref IntPtr dst, ref GflFilter filter){
			this.ThrowIfDisposed();
			if(this._ConvolveDelegate == null){
				this._ConvolveDelegate = this.LoadMethod<GflBitmapFuncFilter>("gflConvolve");
			}
			return this._ConvolveDelegate(pSrc, ref dst, ref filter);
		}

		#endregion

		#region Filters Destructive

		private GflBitmapFuncDestInt32 _AverageDestDelegate;
		internal Gfl.Error Average(IntPtr pSrc, int filterSize){
			this.ThrowIfDisposed();
			if(this._AverageDestDelegate == null){
				this._AverageDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflAverage");
			}
			return this._AverageDestDelegate(pSrc, IntPtr.Zero, filterSize);
		}

		private GflBitmapFuncDestInt32 _SoftenDestDelegate;
		internal Gfl.Error Soften(IntPtr pSrc, int percentage){
			this.ThrowIfDisposed();
			if(this._SoftenDestDelegate == null){
				this._SoftenDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflSoften");
			}
			return this._SoftenDestDelegate(pSrc, IntPtr.Zero, percentage);
		}

		private GflBitmapFuncDestInt32 _BlurDestDelegate;
		internal Gfl.Error Blur(IntPtr pSrc, int percentage){
			this.ThrowIfDisposed();
			if(this._BlurDestDelegate == null){
				this._BlurDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflBlur");
			}
			return this._BlurDestDelegate(pSrc, IntPtr.Zero, percentage);
		}

		private GflBitmapFuncDestInt32 _GaussianBlurDestDelegate;
		internal Gfl.Error GaussianBlur(IntPtr pSrc, int filterSize){
			this.ThrowIfDisposed();
			if(this._GaussianBlurDestDelegate == null){
				this._GaussianBlurDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflGaussianBlur");
			}
			return this._GaussianBlurDestDelegate(pSrc, IntPtr.Zero, filterSize);
		}

		private GflBitmapFuncDestInt32 _MaximumDestDelegate;
		internal Gfl.Error Maximum(IntPtr pSrc, int filterSize){
			this.ThrowIfDisposed();
			if(this._MaximumDestDelegate == null){
				this._MaximumDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflMaximum");
			}
			return this._MaximumDestDelegate(pSrc, IntPtr.Zero, filterSize);
		}

		private GflBitmapFuncDestInt32 _MinimumDestDelegate;
		internal Gfl.Error Minimum(IntPtr pSrc, int filterSize){
			this.ThrowIfDisposed();
			if(this._MinimumDestDelegate == null){
				this._MinimumDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflMinimum");
			}
			return this._MinimumDestDelegate(pSrc, IntPtr.Zero, filterSize);
		}

		private GflBitmapFuncDestInt32 _MedianBoxDestDelegate;
		internal Gfl.Error MedianBox(IntPtr pSrc, int filterSize){
			this.ThrowIfDisposed();
			if(this._MedianBoxDestDelegate == null){
				this._MedianBoxDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflMedianBox");
			}
			return this._MedianBoxDestDelegate(pSrc, IntPtr.Zero, filterSize);
		}

		private GflBitmapFuncDestInt32 _MedianCrossDestDelegate;
		internal Gfl.Error MedianCross(IntPtr pSrc, int filterSize){
			this.ThrowIfDisposed();
			if(this._MedianCrossDestDelegate == null){
				this._MedianCrossDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflMedianCross");
			}
			return this._MedianCrossDestDelegate(pSrc, IntPtr.Zero, filterSize);
		}

		private GflBitmapFuncDestInt32 _SharpenDestDelegate;
		internal Gfl.Error Sharpen(IntPtr pSrc, int percentage){
			this.ThrowIfDisposed();
			if(this._SharpenDestDelegate == null){
				this._SharpenDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32>("gflSharpen");
			}
			return this._SharpenDestDelegate(pSrc, IntPtr.Zero, percentage);
		}

		private GflBitmapFuncDest _EnhanceDetailDestDelegate;
		internal Gfl.Error EnhanceDetail(IntPtr pSrc){
			this.ThrowIfDisposed();
			if(this._EnhanceDetailDestDelegate == null){
				this._EnhanceDetailDestDelegate = this.LoadMethod<GflBitmapFuncDest>("gflEnhanceDetail");
			}
			return this._EnhanceDetailDestDelegate(pSrc, IntPtr.Zero);
		}

		private GflBitmapFuncDest _EnhanceFocusDestDelegate;
		internal Gfl.Error EnhanceFocus(IntPtr pSrc){
			this.ThrowIfDisposed();
			if(this._EnhanceFocusDestDelegate == null){
				this._EnhanceFocusDestDelegate = this.LoadMethod<GflBitmapFuncDest>("gflEnhanceFocus");
			}
			return this._EnhanceFocusDestDelegate(pSrc, IntPtr.Zero);
		}

		private GflBitmapFuncDest _FocusRestorationDestDelegate;
		internal Gfl.Error FocusRestoration(IntPtr pSrc){
			this.ThrowIfDisposed();
			if(this._FocusRestorationDestDelegate == null){
				this._FocusRestorationDestDelegate = this.LoadMethod<GflBitmapFuncDest>("gflFocusRestoration");
			}
			return this._FocusRestorationDestDelegate(pSrc, IntPtr.Zero);
		}

		private GflBitmapFuncDest _EdgeDetectLightDestDelegate;
		internal Gfl.Error EdgeDetectLight(IntPtr pSrc){
			this.ThrowIfDisposed();
			if(this._EdgeDetectLightDestDelegate == null){
				this._EdgeDetectLightDestDelegate = this.LoadMethod<GflBitmapFuncDest>("gflEdgeDetectLight");
			}
			return this._EdgeDetectLightDestDelegate(pSrc, IntPtr.Zero);
		}

		private GflBitmapFuncDest _EdgeDetectMediumDestDelegate;
		internal Gfl.Error EdgeDetectMedium(IntPtr pSrc){
			this.ThrowIfDisposed();
			if(this._EdgeDetectMediumDestDelegate == null){
				this._EdgeDetectMediumDestDelegate = this.LoadMethod<GflBitmapFuncDest>("gflEdgeDetectMedium");
			}
			return this._EdgeDetectMediumDestDelegate(pSrc, IntPtr.Zero);
		}

		private GflBitmapFuncDest _EdgeDetectHeavyDestDelegate;
		internal Gfl.Error EdgeDetectHeavy(IntPtr pSrc){
			this.ThrowIfDisposed();
			if(this._EdgeDetectHeavyDestDelegate == null){
				this._EdgeDetectHeavyDestDelegate = this.LoadMethod<GflBitmapFuncDest>("gflEdgeDetectHeavy");
			}
			return this._EdgeDetectHeavyDestDelegate(pSrc, IntPtr.Zero);
		}

		private GflBitmapFuncDest _EmbossDestDelegate;
		internal Gfl.Error Emboss(IntPtr pSrc){
			this.ThrowIfDisposed();
			if(this._EmbossDestDelegate == null){
				this._EmbossDestDelegate = this.LoadMethod<GflBitmapFuncDest>("gflEmboss");
			}
			return this._EmbossDestDelegate(pSrc, IntPtr.Zero);
		}

		private GflBitmapFuncDest _EmbossMoreDestDelegate;
		internal Gfl.Error EmbossMore(IntPtr pSrc){
			this.ThrowIfDisposed();
			if(this._EmbossMoreDestDelegate == null){
				this._EmbossMoreDestDelegate = this.LoadMethod<GflBitmapFuncDest>("gflEmbossMore");
			}
			return this._EmbossMoreDestDelegate(pSrc, IntPtr.Zero);
		}

		private GflBitmapFuncDest _ReduceNoiseDestDelegate;
		internal Gfl.Error Sepia(IntPtr pSrc){
			this.ThrowIfDisposed();
			if(this._ReduceNoiseDestDelegate == null){
				this._ReduceNoiseDestDelegate = this.LoadMethod<GflBitmapFuncDest>("gflReduceNoise");
			}
			return this._ReduceNoiseDestDelegate(pSrc, IntPtr.Zero);
		}

		private GflBitmapFuncDestInt32Int32Bool _DropShadowDestDelegate;
		internal Gfl.Error Sepia(IntPtr pSrc, int size, int depth, bool keepSize){
			this.ThrowIfDisposed();
			if(this._DropShadowDestDelegate == null){
				this._DropShadowDestDelegate = this.LoadMethod<GflBitmapFuncDestInt32Int32Bool>("gflDropShadow");
			}
			return this._DropShadowDestDelegate(pSrc, IntPtr.Zero, size, depth, keepSize);
		}

		private GflBitmapFuncDestFilter _ConvolveDestDelegate;
		internal Gfl.Error Convolve(IntPtr pSrc, ref GflFilter filter){
			this.ThrowIfDisposed();
			if(this._ConvolveDestDelegate == null){
				this._ConvolveDestDelegate = this.LoadMethod<GflBitmapFuncDestFilter>("gflConvolve");
			}
			return this._ConvolveDestDelegate(pSrc, IntPtr.Zero, ref filter);
		}

		#endregion

		#region Misc
		/*
		private delegate Gfl.Error GflJpegLosslessTransformDelegate(string path, JpegLosslessTransform transform);
		private GflJpegLosslessTransformDelegate _JpegLosslessTransformDelegate;
		internal Gfl.Error JpegLosslessTransformInternal(string path, JpegLosslessTransform filter){
			this.ThrowIfDisposed();
			if(this._JpegLosslessTransformDelegate == null){
				this._JpegLosslessTransformDelegate = this.LoadMethod<GflJpegLosslessTransformDelegate>("gflJpegLosslessTransform");
			}
			return this._JpegLosslessTransformDelegate(path, filter);
		}
		*/
		#endregion

		#region Windows

		private delegate Gfl.Error ExportIntoClipboardDelegate(IntPtr src);
		private ExportIntoClipboardDelegate _ExportIntoClipboardDelegate;
		internal Gfl.Error ExportIntoClipboard(IntPtr pSrc){
			this.ThrowIfDisposed();
			if(this._ExportIntoClipboardDelegate == null){
				this._ExportIntoClipboardDelegate = this.LoadMethod<ExportIntoClipboardDelegate>("gflExportIntoClipboard");
			}
			return this._ExportIntoClipboardDelegate(pSrc);
		}

		private delegate Gfl.Error ImportFromClipboardDelegate(ref IntPtr dst);
		private ImportFromClipboardDelegate _ImportFromClipboardDelegate;
		internal Gfl.Error ImportFromClipboard(ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._ImportFromClipboardDelegate == null){
				this._ImportFromClipboardDelegate = this.LoadMethod<ImportFromClipboardDelegate>("gflImportFromClipboard");
			}
			return this._ImportFromClipboardDelegate(ref dst);
		}

		#endregion

		#region Delegates

		private delegate Gfl.Error GflBitmapFunc(IntPtr src, ref IntPtr dst);
		private delegate Gfl.Error GflBitmapFuncInt32(IntPtr src, ref IntPtr dst, int prm);
		private delegate Gfl.Error GflBitmapFuncInt32GflColor(IntPtr src, ref IntPtr dst, int prm, ref Gfl.GflColor color);
		private delegate Gfl.Error GflBitmapFuncInt32Int32Bool(IntPtr src, ref IntPtr dst, int prm, int prm2, bool prm3);
		private delegate Gfl.Error GflBitmapFuncFilter(IntPtr src, ref IntPtr dst, ref GflFilter filter);
		//private delegate Gfl.Error GflBitmapFuncSwapColors(IntPtr src, ref IntPtr dst, SwapColorsFilter filter);

		private delegate Gfl.Error GflBitmapFuncDest(IntPtr src, IntPtr dst);
		private delegate Gfl.Error GflBitmapFuncDestInt32(IntPtr src, IntPtr dst, int prm);
		private delegate Gfl.Error GflBitmapFuncDestInt32GflColor(IntPtr src, IntPtr dst, int prm, ref Gfl.GflColor color);
		private delegate Gfl.Error GflBitmapFuncDestInt32Int32Bool(IntPtr src, IntPtr dst, int prm, int prm2, bool prm3);
		private delegate Gfl.Error GflBitmapFuncDestFilter(IntPtr src, IntPtr dst, ref GflFilter filter);
		//private delegate Gfl.Error GflBitmapFuncDestSwapColors(IntPtr src, IntPtr dst, SwapColorsFilter filter);

		#endregion

		#region Structs

		internal struct GflFilter{
			public uint Size;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=7*7)]
			public UInt16[] Matrix;
			public UInt16 Divisor;
			public UInt16 Bias;
		}

		#endregion

	}
}
