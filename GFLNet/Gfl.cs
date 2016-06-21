/*
	$Id: Gfl.cs 326 2014-01-09 10:15:01Z catwalkagogo@gmail.com $
*/
using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Linq;

namespace GflNet{
	public partial class Gfl : CatWalk.Win32.InteropObject{
		private LinkedList<WeakReference> _LoadedBitmap = new LinkedList<WeakReference>();
		public string DllName { get; private set; }

#if DEBUG
		public int LoadedBitmapCount{
			get{
				return this._LoadedBitmap.Count;
			}
		}
#endif
		#region Initialize

		public Gfl(string dllName) : base(dllName){
			this.DllName = dllName;
			Gfl.Error error = this.LibraryInit();
			if(error != Gfl.Error.None){
				throw new Win32Exception();
			}

			if(this.pluginPath != null){
				this.SetPluginPathname(this.pluginPath);
			}

			if(this.isEnableLZW != null){
				this.EnableLZW(this.isEnableLZW.Value);
			}
		}

		public string VersionString{
			get{
				this.ThrowIfDisposed();

				return this.GetVersion();
			}
		}
		
		private bool? isEnableLZW;
		public bool IsEnableLZW{
			get{
				return this.isEnableLZW.Value;
			}
			set{
				this.ThrowIfDisposed();

				this.isEnableLZW = value;

				if(this.Handle != IntPtr.Zero){
					this.EnableLZW(value);
				}
			}
		}
		
		private string pluginPath = null;
		public string PluginPath{
			get{
				return this.pluginPath;
			}
			set{
				this.ThrowIfDisposed();

				this.pluginPath = value;
				if(this.Handle != IntPtr.Zero){
					this.SetPluginPathname(value);
				}
			}
		}

		#endregion

		#region Format

		private ReadOnlyCollection<Format> _Formats;
		public ReadOnlyCollection<Format> Formats{
			get{
				if(this._Formats == null){
					this._Formats = new ReadOnlyCollection<Format>(this.GetFormats());
				}
				return this._Formats;
			}
		}

		private Format[] GetFormats(){
			this.ThrowIfDisposed();

			int num = this.GetNumberOfFormat();
			Format[] formats = new Format[num];
			for(int i = 0; i < num; i++){
				formats[i] = this.GetGflFormat(i);
			}
			return formats;
		}
		
		internal Format GetGflFormat(int index){
			this.ThrowIfDisposed();

			var formatInfo = new GflFormatInformation();
			this.ThrowIfError(this.GetFormatInformationByIndex(index, ref formatInfo));
			return new Format(ref formatInfo);
		}

		#endregion

		#region LoadBitmap

		public Bitmap LoadBitmap(string path){
			FileInformation info;
			return this.LoadBitmap(path, 0, this.GetDefaultLoadParameters(), out info, this);
		}

		public Bitmap LoadBitmap(string path, int frameIndex){
			FileInformation info;
			return this.LoadBitmap(path, frameIndex, this.GetDefaultLoadParameters(), out info, this);
		}

		public Bitmap LoadBitmap(string path, int frameIndex, LoadParameters parameters){
			FileInformation info;
			return this.LoadBitmap(path, frameIndex, parameters, out info, this);
		}

		public Bitmap LoadBitmap(string path, int frameIndex, LoadParameters parameters, out FileInformation info){
			return this.LoadBitmap(path, frameIndex, parameters, out info, this);
		}

		internal Bitmap LoadBitmap(string path, int frameIndex, LoadParameters parameters, out FileInformation info, object sender){
			this.ThrowIfDisposed();

			path = Path.GetFullPath(path);
			if(parameters == null){
				throw new ArgumentNullException("parameters");
			}
			
			Gfl.GflLoadParams prms = new Gfl.GflLoadParams();
			this.GetDefaultLoadParams(ref prms);
			parameters.ToGflLoadParams(sender, ref prms);
			prms.ImageWanted = frameIndex;

			IntPtr pBitmap = IntPtr.Zero;
			var pInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GflFileInformation)));
			try{
				this.ThrowIfError(this.LoadBitmap(path, ref pBitmap, ref prms, pInfo));

				var bitmap = new Bitmap(this, pBitmap);
				info = new FileInformation(this, pInfo);
				return bitmap;
			}finally{
				Marshal.FreeHGlobal(pInfo);
			}
		}

		public Bitmap LoadBitmap(Stream stream){
			FileInformation info;
			return this.LoadBitmap(stream, 0, this.GetDefaultLoadParameters(), out info, this);
		}

		public Bitmap LoadBitmap(Stream stream, int frameIndex){
			FileInformation info;
			return this.LoadBitmap(stream, frameIndex, this.GetDefaultLoadParameters(), out info, this);
		}

		public Bitmap LoadBitmap(Stream stream, int frameIndex, LoadParameters parameters){
			FileInformation info;
			return this.LoadBitmap(stream, frameIndex, parameters, out info, this);
		}

		public Bitmap LoadBitmap(Stream stream, int frameIndex, LoadParameters parameters, out FileInformation info){
			return this.LoadBitmap(stream, frameIndex, parameters, out info, this);
		}

		internal Bitmap LoadBitmap(Stream stream, int frameIndex, LoadParameters parameters, out FileInformation info, object sender){
			this.ThrowIfDisposed();

			if(stream == null){
				throw new ArgumentNullException("stream");
			}
			if(parameters == null){
				throw new ArgumentNullException("parameters");
			}
			
			Gfl.GflLoadParams prms = new Gfl.GflLoadParams();
			this.GetDefaultLoadParams(ref prms);
			parameters.StreamToHandle = stream;
			parameters.ToGflLoadParams(sender, ref prms);
			prms.ImageWanted = frameIndex;

			IntPtr pBitmap = IntPtr.Zero;
			var pInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GflFileInformation)));
			try{
				var fs = stream as FileStream;
				if(fs != null){
					bool success = false;
					fs.SafeFileHandle.DangerousAddRef(ref success);
					if(!success){
						throw new IOException();
					}
					try{
						this.ThrowIfError(this.LoadBitmapFromHandle(fs.SafeFileHandle.DangerousGetHandle(), ref pBitmap, ref prms, pInfo));
					}finally{
						fs.SafeFileHandle.DangerousRelease();
					}
				}else{
					// Dummy pointer is needed for gflLoadBitmapFromHandle otherwise it causes memory access violation.
					var pDummy = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
					try{
						this.ThrowIfError(this.LoadBitmapFromHandle(pDummy, ref pBitmap, ref prms, pInfo));
					}finally{
						Marshal.FreeHGlobal(pDummy);
					}
				}

				info = new FileInformation(this, pInfo);
				var bitmap = new Bitmap(this, pBitmap);
				return bitmap;
			}finally{
				Marshal.FreeHGlobal(pInfo);
			}
		}

		#endregion

		#region LoadMultiBitmap

		public MultiBitmap LoadMultiBitmap(string filename){
			FileInformation info;
			return this.LoadMultiBitmap(filename, out info);
		}

		public MultiBitmap LoadMultiBitmap(string filename, out FileInformation info){
			this.ThrowIfDisposed();

			filename = Path.GetFullPath(filename);
			info = this.GetFileInformation(filename);
			return new MultiBitmap(this, filename, info);
		}

		public MultiBitmap LoadMultiBitmap(Stream stream){
			FileInformation info;
			return this.LoadMultiBitmap(stream, out info);
		}

		public MultiBitmap LoadMultiBitmap(Stream stream, out FileInformation info){
			this.ThrowIfDisposed();

			info = this.GetFileInformation(stream);
			return new MultiBitmap(this, stream, info);
		}

		#endregion

		#region LoadThumbnail

		public Bitmap LoadThumbnail(string path, int width, int height){
			FileInformation info;
			return this.LoadThumbnail(path, width, height, this.GetDefaultLoadParameters(), out info, this);
		}

		public Bitmap LoadThumbnail(string path, int width, int height, LoadParameters parameters){
			FileInformation info;
			return this.LoadThumbnail(path, width, height, parameters, out info, this);
		}

		public Bitmap LoadThumbnail(string path, int width, int height, LoadParameters parameters, out FileInformation info){
			return this.LoadThumbnail(path, width, height, parameters, out info, this);
		}

		internal Bitmap LoadThumbnail(string path, int width, int height, LoadParameters parameters, out FileInformation info, object sender){
			this.ThrowIfDisposed();

			path = Path.GetFullPath(path);
			if(parameters == null){
				throw new ArgumentNullException("parameters");
			}
			
			Gfl.GflLoadParams prms = new Gfl.GflLoadParams();
			this.GetDefaultLoadParams(ref prms);
			parameters.ToGflLoadParams(sender, ref prms);

			IntPtr pBitmap = IntPtr.Zero;
			var pInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GflFileInformation)));
			try{
				this.ThrowIfError(this.LoadThumbnail(path, width, height, ref pBitmap, ref prms, pInfo));

				var bitmap = new Bitmap(this, pBitmap);
				info = new FileInformation(this, pInfo);
				return bitmap;
			}finally{
				Marshal.FreeHGlobal(pInfo);
			}
		}

		public Bitmap LoadThumbnail(Stream stream, int width, int height){
			FileInformation info;
			return this.LoadThumbnail(stream, width, height, this.GetDefaultLoadParameters(), out info, this);
		}

		public Bitmap LoadThumbnail(Stream stream, int width, int height, LoadParameters parameters){
			FileInformation info;
			return this.LoadThumbnail(stream, width, height, parameters, out info, this);
		}

		public Bitmap LoadThumbnail(Stream stream, int width, int height, LoadParameters parameters, out FileInformation info){
			return this.LoadThumbnail(stream, width, height, parameters, out info, this);
		}

		internal Bitmap LoadThumbnail(Stream stream, int width, int height, LoadParameters parameters, out FileInformation info, object sender){
			this.ThrowIfDisposed();

			if(stream == null){
				throw new ArgumentNullException("stream");
			}
			if(parameters == null){
				throw new ArgumentNullException("parameters");
			}
			
			Gfl.GflLoadParams prms = new Gfl.GflLoadParams();
			this.GetDefaultLoadParams(ref prms);
			parameters.ToGflLoadParams(sender, ref prms);

			IntPtr pBitmap = IntPtr.Zero;
			var pInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GflFileInformation)));
			try{
				var fs = stream as FileStream;
				if(fs != null){
					bool success = false;
					fs.SafeFileHandle.DangerousAddRef(ref success);
					if(!success){
						throw new IOException();
					}
					try{
						this.ThrowIfError(this.LoadThumbnailFromHandle(fs.SafeFileHandle.DangerousGetHandle(), width, height, ref pBitmap, ref prms, pInfo));
					}finally{
						fs.SafeFileHandle.DangerousRelease();
					}
				}else{
					var pDummy = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
					try{
						this.ThrowIfError(this.LoadThumbnailFromHandle(pDummy, width, height, ref pBitmap, ref prms, pInfo));
					}finally{
						Marshal.FreeHGlobal(pDummy);
					}
				}

				info = new FileInformation(this, pInfo);
				var bitmap = new Bitmap(this, pBitmap);
				return bitmap;
			}finally{
				Marshal.FreeHGlobal(pInfo);
			}
		}


		#endregion

		#region GetDefaultLoadParameters

		public LoadParameters GetDefaultLoadParameters(){
			this.ThrowIfDisposed();

			var prms = new Gfl.GflLoadParams();
			this.GetDefaultLoadParams(ref prms);
			return new LoadParameters(prms);
		}

		#endregion

		#region GetFileInformation

		public FileInformation GetFileInformation(string filename){
			return this.GetFileInformation(filename, -1);
		}

		public FileInformation GetFileInformation(string filename, Format format){
			return this.GetFileInformation(filename, format.Index);
		}

		internal FileInformation GetFileInformation(string filename, int formatIndex){
			this.ThrowIfDisposed();

			filename = Path.GetFullPath(filename);
			//var info = new GflFileInformation();
			var pInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GflFileInformation)));
			try{
				this.ThrowIfError(this.GetFileInformation(filename, formatIndex, pInfo));
				return new FileInformation(this, pInfo);
			}finally{
				Marshal.FreeHGlobal(pInfo);
			}
		}

		public FileInformation GetFileInformation(Stream stream){
			return this.GetFileInformation(stream, -1);
		}

		public FileInformation GetFileInformation(Stream stream, Format format){
			return this.GetFileInformation(stream, format.Index);
		}

		internal FileInformation GetFileInformation(Stream stream, int formatIndex){
			this.ThrowIfDisposed();

			var prms = new GflLoadParams();
			var param = new LoadParameters(prms);
			param.StreamToHandle = stream;
			param.ToGflLoadParams(this, ref prms);
			var callbacks = prms.Callbacks;
			var pInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GflFileInformation)));
			try{
				var fs = stream as FileStream;
				if(fs != null){
					bool success = false;
					fs.SafeFileHandle.DangerousAddRef(ref success);
					if(!success){
						throw new IOException();
					}
					try{
						this.ThrowIfError(this.GetFileInformationFromHandle(fs.SafeFileHandle.DangerousGetHandle(), formatIndex, ref callbacks, pInfo));
					}finally{
						fs.SafeFileHandle.DangerousRelease();
					}
				}else{
					this.ThrowIfError(this.GetFileInformationFromHandle(IntPtr.Zero, formatIndex, ref callbacks, pInfo));
				}
				return new FileInformation(this, pInfo);
			}finally{
				Marshal.FreeHGlobal(pInfo);
			}
		}

		#endregion

		#region Exception

		internal void ThrowIfError(Error error){
			this.ThrowIfDisposed();
			
			switch(error){
				case Gfl.Error.None:
					return;
				case Gfl.Error.FileOpen:
				case Gfl.Error.FileRead:
				case Gfl.Error.FileCreate:
				case Gfl.Error.FileWrite:
					throw new IOException(this.GetErrorString(error));
				case Gfl.Error.NoMemory:
					throw new OutOfMemoryException(this.GetErrorString(error));
				case Gfl.Error.BadBitmap:
				case Gfl.Error.BadFormatIndex:
				case Gfl.Error.UnknownFormat:
					throw new FormatException(this.GetErrorString(error));
				case Gfl.Error.BadParameters:
					throw new ArgumentException(this.GetErrorString(error));
				default:
					throw new Win32Exception(this.GetErrorString(error));
			}
		}

		#endregion

		#region IDisposable
		
		private readonly object _SyncObject = new object();
		private bool _Disposed = false;
		protected override void Dispose(bool disposing){
			if(!this._Disposed){
				lock(this._SyncObject){
					foreach(var bitmapRef in this._LoadedBitmap.Where(wref => wref.IsAlive)){
						var bitmap = (Bitmap)bitmapRef.Target;
						if(!bitmap.Disposed){
							this.FreeBitmap(bitmap);
							bitmap.Disposed = true;
						}
					}
					this.LibraryExit();
					this._Disposed = true;
				}
			}
			base.Dispose(disposing);
		}

		internal void AddBitmap(Bitmap bitmap){
			lock(this._SyncObject){
				this.ThrowIfDisposed();
				this._LoadedBitmap.AddLast(new WeakReference(bitmap));
			}
		}

		internal void DisposeBitmap(Bitmap bitmap){
			lock(this._SyncObject){
				if(!bitmap.Disposed){
					this.ThrowIfDisposed();
					this.FreeBitmap(bitmap);
					bitmap.Disposed = true;
					var node = this._LoadedBitmap.First;
					while(node != null){
						var next = node.Next;
						if(!node.Value.IsAlive || node.Value.Target == bitmap){
							this._LoadedBitmap.Remove(node);
						}
						node = next;
					}
				}
			}
		}

		#endregion
	}
}
