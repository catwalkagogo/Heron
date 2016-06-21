/*
	$Id: Gfl.NativeMethods.cs 319 2014-01-03 04:23:35Z catwalkagogo@gmail.com $
*/
using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace GflNet{
	public partial class Gfl : CatWalk.Win32.InteropObject{

		#region Initialization
		
		private delegate Error LibraryInitDelegate();
		private LibraryInitDelegate _LibraryInitDelegate;
		internal Error LibraryInit(){
			this.ThrowIfDisposed();
			if(this._LibraryInitDelegate == null){
				this._LibraryInitDelegate = this.LoadMethod<LibraryInitDelegate>("gflLibraryInit");
			}
			return this._LibraryInitDelegate();
		}

		private LibraryInitDelegate _LibraryExitDelegate;
		internal Error LibraryExit(){
			this.ThrowIfDisposed();
			if(this._LibraryExitDelegate == null){
				this._LibraryExitDelegate = this.LoadMethod<LibraryInitDelegate>("gflLibraryExit");
			}
			return this._LibraryExitDelegate();
		}

		private delegate void EnableLZWDelegate(bool enable);
		private EnableLZWDelegate _EnableLZWDelegate;
		internal void EnableLZW(bool enable){
			this.ThrowIfDisposed();
			if(this._EnableLZWDelegate == null){
				this._EnableLZWDelegate = this.LoadMethod<EnableLZWDelegate>("gflEnableLZW");
			}
			this._EnableLZWDelegate(enable);
		}

		
		private delegate IntPtr GetVersionDelegate();
		private GetVersionDelegate _GetVersionDelegate;
		internal string GetVersion(){
			this.ThrowIfDisposed();
			if(this._GetVersionDelegate == null){
				this._GetVersionDelegate = this.LoadMethod<GetVersionDelegate>("gflGetVersion");
			}
			return Marshal.PtrToStringAnsi(this._GetVersionDelegate());
		}

		private delegate void SetPluginsPathnameDelegate(string path);
		private SetPluginsPathnameDelegate _SetPluginPathnameDelegate;
		internal void SetPluginPathname(string path){
			this.ThrowIfDisposed();
			if(this._SetPluginPathnameDelegate == null){
				this._SetPluginPathnameDelegate = this.LoadMethod<SetPluginsPathnameDelegate>("gflSetPluginsPathname");
			}
			this._SetPluginPathnameDelegate(path);
		}
		
		#endregion
		
		#region Allocation
		
		private delegate IntPtr AllockBitmapDelegate(BitmapType type, int width, int height, uint linePadding, ref GflColor backgroundColor);
		private AllockBitmapDelegate _AllockBitmapDelegate;
		internal IntPtr AllockBitmap(BitmapType type, int width, int height, uint linePadding, ref GflColor backgroundColor){
			this.ThrowIfDisposed();
			if(this._AllockBitmapDelegate == null){
				this._AllockBitmapDelegate = this.LoadMethod<AllockBitmapDelegate>("gflAllockBitmap");
			}
			return this._AllockBitmapDelegate(type, width, height, linePadding, ref backgroundColor);
		}

		private delegate void FreeBitmapDelegate(IntPtr bitmap);
		private FreeBitmapDelegate _FreeBitmapDelegate;
		internal void FreeBitmap(Bitmap bitmap){
			this.ThrowIfDisposed();
			if(this._FreeBitmapDelegate == null){
				this._FreeBitmapDelegate = this.LoadMethod<FreeBitmapDelegate>("gflFreeBitmap");
			}
			this._FreeBitmapDelegate(bitmap.Handle);
		}
		
		private delegate IntPtr CloneBitmapDelegate(IntPtr bitmap);
		private CloneBitmapDelegate _CloneBitmapDelegate;
		internal IntPtr CloneBitmap(Bitmap bitmap){
			this.ThrowIfDisposed();
			if(this._CloneBitmapDelegate == null){
				this._CloneBitmapDelegate = this.LoadMethod<CloneBitmapDelegate>("gflCloneBitmap");
			}
			return this._CloneBitmapDelegate(bitmap.Handle);
		}

		private delegate void FreeBitmapDataDelegate(IntPtr bitmap);
		private FreeBitmapDataDelegate _FreeBitmapDataDelegate;
		internal void FreeBitmapData(Bitmap bitmap){
			this.ThrowIfDisposed();
			if(this._FreeBitmapDataDelegate == null){
				this._FreeBitmapDataDelegate = this.LoadMethod<FreeBitmapDataDelegate>("gflFreeBitmapData");
			}
			this._FreeBitmapDataDelegate(bitmap.Handle);
		}

		private delegate void FreeFileInformationDelegate(IntPtr info);
		private FreeFileInformationDelegate _FreeFileInformationDelegate;
		internal void FreeFileInformation(IntPtr info){
			this.ThrowIfDisposed();
			if(this._FreeFileInformationDelegate == null){
				this._FreeFileInformationDelegate = this.LoadMethod<FreeFileInformationDelegate>("gflFreeFileInformation");
			}
			this._FreeFileInformationDelegate(info);
		}
		
		#endregion
		
		#region Format
		
		private delegate int GetNumberOfFormatDelegate();
		private GetNumberOfFormatDelegate _GetNumberOfFormatDelegate;
		internal int GetNumberOfFormat(){
			this.ThrowIfDisposed();
			if(this._GetNumberOfFormatDelegate == null){
				this._GetNumberOfFormatDelegate = this.LoadMethod<GetNumberOfFormatDelegate>("gflGetNumberOfFormat");
			}
			return this._GetNumberOfFormatDelegate();
		}
		
		private delegate bool FormatIsSupportedDelegate(string name);
		private FormatIsSupportedDelegate _FormatIsSupportedDelegate;
		internal bool FormatIsSupported(string name){
			this.ThrowIfDisposed();
			if(this._FormatIsSupportedDelegate == null){
				this._FormatIsSupportedDelegate = this.LoadMethod<FormatIsSupportedDelegate>("gflFormatIsSupported");
			}
			return this._FormatIsSupportedDelegate(name);
		}

		private delegate IntPtr GetFormatNameByIndexDelegate(int index);
		private GetFormatNameByIndexDelegate _GetFormatNameByIndexDelegate;
		internal string GetFormatNameByIndex(int index){
			this.ThrowIfDisposed();
			if(this._GetFormatNameByIndexDelegate == null){
				this._GetFormatNameByIndexDelegate = this.LoadMethod<GetFormatNameByIndexDelegate>("gflGetFormatNameByIndex");
			}
			return Marshal.PtrToStringAnsi(this._GetFormatNameByIndexDelegate(index));
		}

		private delegate IntPtr GetFormatIndexByNameDelegate(string name);
		private GetFormatIndexByNameDelegate _GetFormatIndexByNameDelegate;
		internal string GetFormatIndexByName(string name){
			this.ThrowIfDisposed();
			if(this._GetFormatIndexByNameDelegate == null){
				this._GetFormatIndexByNameDelegate = this.LoadMethod<GetFormatIndexByNameDelegate>("gflGetFormatIndexByName");
			}
			return Marshal.PtrToStringAnsi(this._GetFormatIndexByNameDelegate(name));
		}

		private delegate bool FormatIsReadableByIndexDelegate(int index);
		private FormatIsReadableByIndexDelegate _FormatIsReadableByIndexDelegate;
		internal bool FormatIsReadableByIndex(int index){
			this.ThrowIfDisposed();
			if(this._FormatIsReadableByIndexDelegate == null){
				this._FormatIsReadableByIndexDelegate = this.LoadMethod<FormatIsReadableByIndexDelegate>("gflFormatIsReadableByIndex");
			}
			return this._FormatIsReadableByIndexDelegate(index);
		}

		private delegate bool FormatIsReadableByNameDelegate(string name);
		private FormatIsReadableByNameDelegate _FormatIsReadableByNameDelegate;
		internal bool FormatIsReadableByName(string name){
			this.ThrowIfDisposed();
			if(this._FormatIsReadableByNameDelegate == null){
				this._FormatIsReadableByNameDelegate = this.LoadMethod<FormatIsReadableByNameDelegate>("gflFormatIsReadableByName");
			}
			return this._FormatIsReadableByNameDelegate(name);
		}

		private delegate bool FormatIsWritableByIndexDelegate(int index);
		private FormatIsWritableByIndexDelegate _FormatIsWritableByIndexDelegate;
		internal bool FormatIsWritableByIndex(int index){
			this.ThrowIfDisposed();
			if(this._FormatIsWritableByIndexDelegate == null){
				this._FormatIsWritableByIndexDelegate = this.LoadMethod<FormatIsWritableByIndexDelegate>("gflFormatIsWritableByIndex");
			}
			return this._FormatIsWritableByIndexDelegate(index);
		}

		private delegate bool FormatIsWritableByNameDelegate(string name);
		private FormatIsWritableByNameDelegate _FormatIsWritableByNameDelegate;
		internal bool FormatIsWritableByName(string name){
			this.ThrowIfDisposed();
			if(this._FormatIsWritableByNameDelegate == null){
				this._FormatIsWritableByNameDelegate = this.LoadMethod<FormatIsWritableByNameDelegate>("gflFormatIsWritableByName");
			}
			return this._FormatIsWritableByNameDelegate(name);
		}

		private delegate IntPtr GetFormatDescriptionByNameDelegate(string name);
		private GetFormatDescriptionByNameDelegate _GetFormatDescriptionByNameDelegate;
		internal string GetFormatDescriptionByName(string name){
			this.ThrowIfDisposed();
			if(this._GetFormatDescriptionByNameDelegate == null){
				this._GetFormatDescriptionByNameDelegate = this.LoadMethod<GetFormatDescriptionByNameDelegate>("gflGetFormatDescriptionByName");
			}
			return Marshal.PtrToStringAnsi(this._GetFormatDescriptionByNameDelegate(name));
		}

		private delegate IntPtr GetFormatDescriptionByIndexDelegate(int index);
		private GetFormatDescriptionByIndexDelegate _GetFormatDescriptionByIndexDelegate;
		internal string GetFormatDescriptionByIndex(int index){
			this.ThrowIfDisposed();
			if(this._GetFormatDescriptionByIndexDelegate == null){
				this._GetFormatDescriptionByIndexDelegate = this.LoadMethod<GetFormatDescriptionByIndexDelegate>("gflGetFormatDescriptionByIndex");
			}
			return Marshal.PtrToStringAnsi(this._GetFormatDescriptionByIndexDelegate(index));
		}

		private delegate IntPtr GetDefaultFormatSuffixByIndexByNameDelegate(string name);
		private GetDefaultFormatSuffixByIndexByNameDelegate _GetDefaultFormatSuffixByIndexByNameDelegate;
		internal string GetDefaultFormatSuffixByIndexByName(string name){
			this.ThrowIfDisposed();
			if(this._GetDefaultFormatSuffixByIndexByNameDelegate == null){
				this._GetDefaultFormatSuffixByIndexByNameDelegate = this.LoadMethod<GetDefaultFormatSuffixByIndexByNameDelegate>("gflGetDefaultFormatSuffixByIndexByName");
			}
			return Marshal.PtrToStringAnsi(this._GetDefaultFormatSuffixByIndexByNameDelegate(name));
		}

		private delegate IntPtr GetDefaultFormatSuffixByIndexDelegate(int index);
		private GetDefaultFormatSuffixByIndexDelegate _GetDefaultFormatSuffixByIndexDelegate;
		internal string GetDefaultFormatSuffixByIndex(int index){
			this.ThrowIfDisposed();
			if(this._GetDefaultFormatSuffixByIndexDelegate == null){
				this._GetDefaultFormatSuffixByIndexDelegate = this.LoadMethod<GetDefaultFormatSuffixByIndexDelegate>("gflGetDefaultFormatSuffixByIndex");
			}
			return Marshal.PtrToStringAnsi(this._GetDefaultFormatSuffixByIndexDelegate(index));
		}

		private delegate Error GetFormatInformationByNameDelegate(string name, ref GflFormatInformation info);
		private GetFormatInformationByNameDelegate _GetFormatInformationByNameDelegate;
		internal Error GetFormatInformationByName(string name, ref GflFormatInformation info){
			this.ThrowIfDisposed();
			if(this._GetFormatInformationByNameDelegate == null){
				this._GetFormatInformationByNameDelegate = this.LoadMethod<GetFormatInformationByNameDelegate>("gflGetFormatInformationByName");
			}
			return this._GetFormatInformationByNameDelegate(name, ref info);
		}

		private delegate Error GetFormatInformationByIndexDelegate(int index, ref GflFormatInformation info);
		private GetFormatInformationByIndexDelegate _GetFormatInformationByIndexDelegate;
		internal Error GetFormatInformationByIndex(int index, ref GflFormatInformation info){
			this.ThrowIfDisposed();
			if(this._GetFormatInformationByIndexDelegate == null){
				this._GetFormatInformationByIndexDelegate = this.LoadMethod<GetFormatInformationByIndexDelegate>("gflGetFormatInformationByIndex");
			}
			return this._GetFormatInformationByIndexDelegate(index, ref info);
		}
		
		#endregion
		
		#region Read
		
		private delegate void GetDefaultLoadParamsDelegate(ref GflLoadParams prms);
		private GetDefaultLoadParamsDelegate _GetDefaultLoadParamsDelegate;
		internal void GetDefaultLoadParams(ref GflLoadParams prms){
			this.ThrowIfDisposed();
			if(this._GetDefaultLoadParamsDelegate == null){
				this._GetDefaultLoadParamsDelegate = this.LoadMethod<GetDefaultLoadParamsDelegate>("gflGetDefaultLoadParams");
			}
			this._GetDefaultLoadParamsDelegate(ref prms);
		}

		private delegate void GetDefaultThumbailParamsDelegate(ref GflLoadParams prms);
		private GetDefaultThumbailParamsDelegate _GetDefaultThumbailParamsDelegate;
		internal void GetDefaultThumbailParams(ref GflLoadParams prms){
			this.ThrowIfDisposed();
			if(this._GetDefaultThumbailParamsDelegate == null){
				this._GetDefaultThumbailParamsDelegate = this.LoadMethod<GetDefaultThumbailParamsDelegate>("gflGetDefaultThumbailParams");
			}
			this._GetDefaultThumbailParamsDelegate(ref prms);
		}

		private delegate Error LoadBitmapDelegate(string filename, ref IntPtr bitmap, ref GflLoadParams prms, IntPtr info);
		private LoadBitmapDelegate _LoadBitmapDelegate;
		internal Error LoadBitmap(string filename, ref IntPtr bitmap, ref GflLoadParams prms, IntPtr info){
			this.ThrowIfDisposed();
			if(this._LoadBitmapDelegate == null){
				this._LoadBitmapDelegate = this.LoadMethod<LoadBitmapDelegate>("gflLoadBitmap");
			}
			return this._LoadBitmapDelegate(filename, ref bitmap, ref prms, info);
		}

		private delegate Error LoadBitmapFromMemoryDelegate(IntPtr data, uint data_length, ref IntPtr bitmap, ref GflLoadParams prms, IntPtr info);
		private LoadBitmapFromMemoryDelegate _LoadBitmapFromMemoryDelegate;
		internal Error LoadBitmapFromMemory(IntPtr data, uint data_length, ref IntPtr bitmap, ref GflLoadParams prms, IntPtr info){
			this.ThrowIfDisposed();
			if(this._LoadBitmapFromMemoryDelegate == null){
				this._LoadBitmapFromMemoryDelegate = this.LoadMethod<LoadBitmapFromMemoryDelegate>("gflLoadBitmapFromMemory");
			}
			return this._LoadBitmapFromMemoryDelegate(data, data_length, ref bitmap, ref prms, info);
		}

		private delegate Error LoadBitmapFromHandleDelegate(IntPtr handle, ref IntPtr bitmap, ref GflLoadParams prms, IntPtr info);
		private LoadBitmapFromHandleDelegate _LoadBitmapFromHandleDelegate;
		internal Error LoadBitmapFromHandle(IntPtr handle, ref IntPtr bitmap, ref GflLoadParams prms, IntPtr info){
			this.ThrowIfDisposed();
			if(this._LoadBitmapFromHandleDelegate == null){
				this._LoadBitmapFromHandleDelegate = this.LoadMethod<LoadBitmapFromHandleDelegate>("gflLoadBitmapFromHandle");
			}
			return this._LoadBitmapFromHandleDelegate(handle, ref bitmap, ref prms, info);
		}
		
		private delegate Error GetFileInformationDelegate(string filename, int index, IntPtr info);
		private GetFileInformationDelegate _GetFileInformationDelegate;
		internal Error GetFileInformation(string filename, int index, IntPtr info){
			this.ThrowIfDisposed();
			if(this._GetFileInformationDelegate == null){
				this._GetFileInformationDelegate = this.LoadMethod<GetFileInformationDelegate>("gflGetFileInformation");
			}
			return this._GetFileInformationDelegate(filename, index, info);
		}

		private delegate Error GetFileInformationFromMemoryDelegate(IntPtr data, uint size, int index, IntPtr info);
		private GetFileInformationFromMemoryDelegate _GetFileInformationFromMemoryDelegate;
		internal Error GetFileInformationFromMemory(IntPtr data, uint size, int index, IntPtr info){
			this.ThrowIfDisposed();
			if(this._GetFileInformationFromMemoryDelegate == null){
				this._GetFileInformationFromMemoryDelegate = this.LoadMethod<GetFileInformationFromMemoryDelegate>("gflGetFileInformationFromMemory");
			}
			return this._GetFileInformationFromMemoryDelegate(data, size, index, info);
		}

		private delegate Error GetFileInformationFromHandleDelegate(IntPtr handle, int index, ref GflLoadCallbacks callbacks, IntPtr info);
		private GetFileInformationFromHandleDelegate _GetFileInformationFromHandleDelegate;
		internal Error GetFileInformationFromHandle(IntPtr handle, int index, ref GflLoadCallbacks callbacks, IntPtr info){
			this.ThrowIfDisposed();
			if(this._GetFileInformationFromHandleDelegate == null){
				this._GetFileInformationFromHandleDelegate = this.LoadMethod<GetFileInformationFromHandleDelegate>("gflGetFileInformationFromHandle");
			}
			return this._GetFileInformationFromHandleDelegate(handle, index, ref callbacks, info);
		}

		private delegate Error LoadThumbnailDelegate(string filename, int width, int height, ref IntPtr bitmap, ref GflLoadParams prms, IntPtr info);
		private LoadThumbnailDelegate _LoadThumbnailDelegate;
		internal Error LoadThumbnail(string filename, int width, int height, ref IntPtr bitmap, ref GflLoadParams prms, IntPtr info){
			this.ThrowIfDisposed();
			if(this._LoadThumbnailDelegate == null){
				this._LoadThumbnailDelegate = this.LoadMethod<LoadThumbnailDelegate>("gflLoadThumbnail");
			}
			return this._LoadThumbnailDelegate(filename, width, height, ref bitmap, ref prms, info);
		}

		private delegate Error LoadThumbnailFromMemoryDelegate(IntPtr data, uint data_length, int width, int height, ref IntPtr bitmap, ref GflLoadParams prms, IntPtr info);
		private LoadThumbnailFromMemoryDelegate _LoadThumbnailFromMemoryDelegate;
		internal Error LoadThumbnailFromMemory(IntPtr data, uint data_length, int width, int height, ref IntPtr bitmap, ref GflLoadParams prms, IntPtr info){
			this.ThrowIfDisposed();
			if(this._LoadThumbnailFromMemoryDelegate == null){
				this._LoadThumbnailFromMemoryDelegate = this.LoadMethod<LoadThumbnailFromMemoryDelegate>("gflLoadThumbnailFromMemory");
			}
			return this._LoadThumbnailFromMemoryDelegate(data, data_length, width, height, ref bitmap, ref prms, info);
		}

		private delegate Error LoadThumbnailFromHandleDelegate(IntPtr handle, int width, int height, ref IntPtr bitmap, ref GflLoadParams prms, IntPtr info);
		private LoadThumbnailFromHandleDelegate _LoadThumbnailFromHandleDelegate;
		internal Error LoadThumbnailFromHandle(IntPtr handle, int width, int height, ref IntPtr bitmap, ref GflLoadParams prms, IntPtr info){
			this.ThrowIfDisposed();
			if(this._LoadThumbnailFromHandleDelegate == null){
				this._LoadThumbnailFromHandleDelegate = this.LoadMethod<LoadThumbnailFromHandleDelegate>("gflLoadThumbnailFromHandle");
			}
			return this._LoadThumbnailFromHandleDelegate(handle, width, height, ref bitmap, ref prms, info);
		}

		#endregion
		
		#region Write
		
		private delegate void GetDefaultSaveParamsDelegate(ref GflSaveParams prms);
		private GetDefaultSaveParamsDelegate _GetDefaultSaveParamsDelegate;
		internal void GetDefaultSaveParams(ref GflSaveParams prms){
			this.ThrowIfDisposed();
			if(this._GetDefaultSaveParamsDelegate == null){
				this._GetDefaultSaveParamsDelegate = this.LoadMethod<GetDefaultSaveParamsDelegate>("gflGetDefaultSaveParams");
			}
			this._GetDefaultSaveParamsDelegate(ref prms);
		}

		private delegate Error SaveBitmapDelegate(string filename, IntPtr bitmap, ref GflSaveParams prms);
		private SaveBitmapDelegate _SaveBitmapDelegate;
		internal Error SaveBitmap(string filename, Bitmap bitmap, ref GflSaveParams prms){
			this.ThrowIfDisposed();
			if(this._SaveBitmapDelegate == null){
				this._SaveBitmapDelegate = this.LoadMethod<SaveBitmapDelegate>("gflSaveBitmap");
			}
			return this._SaveBitmapDelegate(filename, bitmap.Handle, ref prms);
		}
		
		private delegate Error FileCreateDelegate(IntPtr handle, string filename, int imageCount, ref GflSaveParams prms);
		private FileCreateDelegate _FileCreateDelegate;
		internal Error FileCreate(IntPtr handle, string filename, int imageCount, ref GflSaveParams prms){
			this.ThrowIfDisposed();
			if(this._FileCreateDelegate == null){
				this._FileCreateDelegate = this.LoadMethod<FileCreateDelegate>("gflFileCreate");
			}
			return this._FileCreateDelegate(handle, filename, imageCount, ref prms);
		}

		private delegate Error FileAddPictureDelegate(IntPtr handle, IntPtr bitmap);
		private FileAddPictureDelegate _FileAddPictureDelegate;
		internal Error FileAddPicture(IntPtr handle, Bitmap bitmap){
			this.ThrowIfDisposed();
			if(this._FileAddPictureDelegate == null){
				this._FileAddPictureDelegate = this.LoadMethod<FileAddPictureDelegate>("gflFileAddPicture");
			}
			return this._FileAddPictureDelegate(handle, bitmap.Handle);
		}

		private delegate void FileCloseDelegate(IntPtr handle);
		private FileCloseDelegate _FileCloseDelegate;
		internal void FileClose(IntPtr handle){
			this.ThrowIfDisposed();
			if(this._FileCloseDelegate == null){
				this._FileCloseDelegate = this.LoadMethod<FileCloseDelegate>("gflFileClose");
			}
			this._FileCloseDelegate(handle);
		}

		#endregion
		
		#region Metadata

		private delegate bool BitmapHasEXIFDelegate(IntPtr bitmap);
		private BitmapHasEXIFDelegate _BitmapHasEXIFDelegate;
		internal bool BitmapHasEXIF(Bitmap bitmap){
			this.ThrowIfDisposed();
			if(this._BitmapHasEXIFDelegate == null){
				this._BitmapHasEXIFDelegate = this.LoadMethod<BitmapHasEXIFDelegate>("gflBitmapHasEXIF");
			}
			return this._BitmapHasEXIFDelegate(bitmap.Handle);
		}

		private delegate bool BitmapHasIPTCDelegate(IntPtr bitmap);
		private BitmapHasIPTCDelegate _BitmapHasIPTCDelegate;
		internal bool BitmapHasIPTC(Bitmap bitmap){
			this.ThrowIfDisposed();
			if(this._BitmapHasIPTCDelegate == null){
				this._BitmapHasIPTCDelegate = this.LoadMethod<BitmapHasIPTCDelegate>("gflBitmapHasIPTC");
			}
			return this._BitmapHasIPTCDelegate(bitmap.Handle);
		}

		private delegate bool BitmapHasICCProfileDelegate(IntPtr bitmap);
		private BitmapHasICCProfileDelegate _BitmapHasICCProfileDelegate;
		internal bool BitmapHasICCProfile(Bitmap bitmap){
			this.ThrowIfDisposed();
			if(this._BitmapHasICCProfileDelegate == null){
				this._BitmapHasICCProfileDelegate = this.LoadMethod<BitmapHasICCProfileDelegate>("gflBitmapHasICCProfile");
			}
			return this._BitmapHasICCProfileDelegate(bitmap.Handle);
		}

		private delegate IntPtr BitmapGetEXIFDelegate(IntPtr bitmap, GetExifOptions options);
		private BitmapGetEXIFDelegate _BitmapGetEXIFDelegate;
		internal IntPtr BitmapGetEXIF(Bitmap bitmap, GetExifOptions options){
			this.ThrowIfDisposed();
			if(this._BitmapGetEXIFDelegate == null){
				this._BitmapGetEXIFDelegate = this.LoadMethod<BitmapGetEXIFDelegate>("gflBitmapGetEXIF");
			}
			return this._BitmapGetEXIFDelegate(bitmap.Handle, options);
		}

		private delegate IntPtr FreeEXIFDelegate(IntPtr exifData);
		private FreeEXIFDelegate _FreeEXIFDelegate;
		internal IntPtr FreeEXIF(IntPtr exifData){
			this.ThrowIfDisposed();
			if(this._FreeEXIFDelegate == null){
				this._FreeEXIFDelegate = this.LoadMethod<FreeEXIFDelegate>("gflFreeEXIF");
			}
			return this._FreeEXIFDelegate(exifData);
		}
		
		private delegate Error BitmapRemoveEXIFThumbnailDelegate(IntPtr bitmap);
		private BitmapRemoveEXIFThumbnailDelegate _BitmapRemoveEXIFThumbnailDelegate;
		internal Error BitmapRemoveEXIFThumbnail(Bitmap bitmap){
			this.ThrowIfDisposed();
			if(this._BitmapRemoveEXIFThumbnailDelegate == null){
				this._BitmapRemoveEXIFThumbnailDelegate = this.LoadMethod<BitmapRemoveEXIFThumbnailDelegate>("gflBitmapRemoveEXIFThumbnail");
			}
			return this._BitmapRemoveEXIFThumbnailDelegate(bitmap.Handle);
		}
		
		private delegate void BitmapRemoveMetaDataDelegate(IntPtr bitmap);
		private BitmapRemoveMetaDataDelegate _BitmapRemoveMetaDataDelegate;
		internal void BitmapRemoveMetaData(Bitmap bitmap){
			this.ThrowIfDisposed();
			if(this._BitmapRemoveMetaDataDelegate == null){
				this._BitmapRemoveMetaDataDelegate = this.LoadMethod<BitmapRemoveMetaDataDelegate>("gflBitmapRemoveMetaData");
			}
			this._BitmapRemoveMetaDataDelegate(bitmap.Handle);
		}

		private delegate void BitmapSetEXIFThumbnailDelegate(IntPtr bitmap, IntPtr thumbnail);
		private BitmapSetEXIFThumbnailDelegate _BitmapSetEXIFThumbnailDelegate;
		internal void BitmapSetEXIFThumbnail(Bitmap bitmap, Bitmap thumbnail){
			this.ThrowIfDisposed();
			if(this._BitmapSetEXIFThumbnailDelegate == null){
				this._BitmapSetEXIFThumbnailDelegate = this.LoadMethod<BitmapSetEXIFThumbnailDelegate>("gflBitmapSetEXIFThumbnail");
			}
			this._BitmapSetEXIFThumbnailDelegate(bitmap.Handle, thumbnail.Handle);
		}

		#endregion
		
		#region Error
		
		private delegate IntPtr GetErrorStringDelegate(Error error);
		private GetErrorStringDelegate _GetErrorStringDelegate;
		internal string GetErrorString(Error error){
			this.ThrowIfDisposed();
			if(this._GetErrorStringDelegate == null){
				this._GetErrorStringDelegate = this.LoadMethod<GetErrorStringDelegate>("gflGetErrorString");
			}
			return Marshal.PtrToStringAnsi(this._GetErrorStringDelegate(error));
		}

		#endregion

		#region Advanced

		private delegate Error ResizeDelegate(IntPtr src, ref IntPtr dst, int width, int height, ResizeMethod method, int flags);
		private ResizeDelegate _ResizeDelegate;
		internal Error Resize(IntPtr src, ref IntPtr dst, int width, int height, ResizeMethod method){
			this.ThrowIfDisposed();
			if(this._ResizeDelegate == null){
				this._ResizeDelegate = this.LoadMethod<ResizeDelegate>("gflResize");
			}
			return this._ResizeDelegate(src, ref dst, width, height, method, 0);
		}

		private delegate Error ResizeCanvasDelegate(IntPtr src, ref IntPtr dst, int width, int height, ResizeMethod method, ResizeCanvasOrigin origin, ref GflColor background);
		private ResizeCanvasDelegate _ResizeCanvasDelegate;
		internal Error ResizeCanvas(IntPtr src, ref IntPtr dst, int width, int height, ResizeMethod method, ResizeCanvasOrigin origin, ref GflColor background){
			this.ThrowIfDisposed();
			if(this._ResizeCanvasDelegate == null){
				this._ResizeCanvasDelegate = this.LoadMethod<ResizeCanvasDelegate>("gflResizeCanvas");
			}
			return this._ResizeCanvasDelegate(src, ref dst, width, height, method, origin, ref background);
		}

		private delegate Error RotateDelegate(IntPtr src, ref IntPtr dst, int angle, ref GflColor background);
		private RotateDelegate _RotateDelegate;
		internal Error Rotate(IntPtr src, ref IntPtr dst, int angle, ref GflColor background){
			this.ThrowIfDisposed();
			if(this._RotateDelegate == null){
				this._RotateDelegate = this.LoadMethod<RotateDelegate>("gflRotate");
			}
			return this._RotateDelegate(src, ref dst, angle, ref background);
		}

		private delegate Error RotateFineDelegate(IntPtr src, ref IntPtr dst, double angle, ref GflColor background);
		private RotateFineDelegate _RotateFineDelegate;
		internal Error RotateFine(IntPtr src, ref IntPtr dst, double angle, ref GflColor background){
			this.ThrowIfDisposed();
			if(this._RotateFineDelegate == null){
				this._RotateFineDelegate = this.LoadMethod<RotateFineDelegate>("gflRotateFine");
			}
			return this._RotateFineDelegate(src, ref dst, angle, ref background);
		}

		private delegate Error FlipHorizontalDelegate(IntPtr src, ref IntPtr dst);
		private FlipHorizontalDelegate _FlipHorizontalDelegate;
		internal Error FlipHorizontal(IntPtr src, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._FlipHorizontalDelegate == null){
				this._FlipHorizontalDelegate = this.LoadMethod<FlipHorizontalDelegate>("gflFlipHorizontal");
			}
			return this._FlipHorizontalDelegate(src, ref dst);
		}

		private delegate Error FlipVerticalDelegate(IntPtr src, ref IntPtr dst);
		private FlipVerticalDelegate _FlipVerticalDelegate;
		internal Error FlipVertical(IntPtr src, ref IntPtr dst){
			this.ThrowIfDisposed();
			if(this._FlipVerticalDelegate == null){
				this._FlipVerticalDelegate = this.LoadMethod<FlipVerticalDelegate>("gflFlipVertical");
			}
			return this._FlipVerticalDelegate(src, ref dst);
		}

		#endregion

		#region Advanced Destructive

		private delegate Error ResizeDestDelegate(IntPtr src, IntPtr dst, int width, int height, ResizeMethod method, int flags);
		private ResizeDestDelegate _ResizeDestDelegate;
		internal Error Resize(IntPtr src, int width, int height, ResizeMethod method){
			this.ThrowIfDisposed();
			if(this._ResizeDestDelegate == null){
				this._ResizeDestDelegate = this.LoadMethod<ResizeDestDelegate>("gflResize");
			}
			return this._ResizeDestDelegate(src, IntPtr.Zero, width, height, method, 0);
		}

		private delegate Error ResizeCanvasDestDelegate(IntPtr src, IntPtr dst, int width, int height, ResizeMethod method, ResizeCanvasOrigin origin, ref GflColor background);
		private ResizeCanvasDestDelegate _ResizeCanvasDestDelegate;
		internal Error ResizeCanvas(IntPtr src, int width, int height, ResizeMethod method, ResizeCanvasOrigin origin, ref GflColor background){
			this.ThrowIfDisposed();
			if(this._ResizeCanvasDestDelegate == null){
				this._ResizeCanvasDestDelegate = this.LoadMethod<ResizeCanvasDestDelegate>("gflResizeCanvas");
			}
			return this._ResizeCanvasDestDelegate(src, IntPtr.Zero, width, height, method, origin, ref background);
		}

		private delegate Error RotateDestDelegate(IntPtr src, IntPtr dst, int angle, ref GflColor background);
		private RotateDestDelegate _RotateDestDelegate;
		internal Error Rotate(IntPtr src, int angle, ref GflColor background){
			this.ThrowIfDisposed();
			if(this._RotateDestDelegate == null){
				this._RotateDestDelegate = this.LoadMethod<RotateDestDelegate>("gflRotate");
			}
			return this._RotateDestDelegate(src, IntPtr.Zero, angle, ref background);
		}

		private delegate Error RotateFineDestDelegate(IntPtr src, IntPtr dst, double angle, ref GflColor background);
		private RotateFineDestDelegate _RotateFineDestDelegate;
		internal Error RotateFine(IntPtr src, double angle, ref GflColor background){
			this.ThrowIfDisposed();
			if(this._RotateFineDestDelegate == null){
				this._RotateFineDestDelegate = this.LoadMethod<RotateFineDestDelegate>("gflRotateFine");
			}
			return this._RotateFineDestDelegate(src, IntPtr.Zero, angle, ref background);
		}

		private delegate Error FlipHorizontalDestDelegate(IntPtr src, IntPtr dst);
		private FlipHorizontalDestDelegate _FlipHorizontalDestDelegate;
		internal Error FlipHorizontal(IntPtr src){
			this.ThrowIfDisposed();
			if(this._FlipHorizontalDestDelegate == null){
				this._FlipHorizontalDestDelegate = this.LoadMethod<FlipHorizontalDestDelegate>("gflFlipHorizontal");
			}
			return this._FlipHorizontalDestDelegate(src, IntPtr.Zero);
		}

		private delegate Error FlipVerticalDestDelegate(IntPtr src, IntPtr dst);
		private FlipVerticalDestDelegate _FlipVerticalDestDelegate;
		internal Error FlipVertical(IntPtr src){
			this.ThrowIfDisposed();
			if(this._FlipVerticalDestDelegate == null){
				this._FlipVerticalDestDelegate = this.LoadMethod<FlipVerticalDestDelegate>("gflFlipVertical");
			}
			return this._FlipVerticalDestDelegate(src, IntPtr.Zero);
		}

		#endregion
	}
}
