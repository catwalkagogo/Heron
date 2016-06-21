/*
	$Id: Bitmap.cs 274 2011-08-01 10:00:09Z catwalkagogo@gmail.com $
*/
using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace GflNet{
	public class Bitmap : IDisposable{
		internal IntPtr Handle{get; private set;}
		private Gfl.GflBitmap _GflBitmap;
		internal Gfl Gfl{get; private set;}
		
		internal Bitmap(Gfl gfl, IntPtr handle){
			this.Handle = handle;
			this.Gfl = gfl;
			this.RefreshGflBitmap(handle);
			this.Gfl.AddBitmap(this);
		}

		private void RefreshGflBitmap(IntPtr handle){
			this._GflBitmap = (Gfl.GflBitmap)Marshal.PtrToStructure(handle, typeof(Gfl.GflBitmap));
		}

		#region Functions

		public void Resize(int width, int height, ResizeMethod method){
			this.ThrowIfDisposed();
			this.Gfl.ThrowIfError(this.Gfl.Resize(this.Handle, width, height, method));
			this.RefreshGflBitmap(this.Handle);
		}
		
		public void ResizeCanvas(int width, int height, ResizeMethod method, ResizeCanvasOrigin origin, Color background){
			this.ThrowIfDisposed();
			var bg = background.ToGflColor();
			this.Gfl.ThrowIfError(this.Gfl.ResizeCanvas(this.Handle, width, height, method, origin, ref bg));
			this.RefreshGflBitmap(this.Handle);
		}

		public void Rotate(int angle, Color background){
			this.ThrowIfDisposed();
			var bg = background.ToGflColor();
			this.Gfl.ThrowIfError(this.Gfl.Rotate(this.Handle, angle, ref bg));
			this.RefreshGflBitmap(this.Handle);
		}

		public void RotateFine(double angle, Color background){
			this.ThrowIfDisposed();
			var bg = background.ToGflColor();
			this.Gfl.ThrowIfError(this.Gfl.RotateFine(this.Handle, angle, ref bg));
			this.RefreshGflBitmap(this.Handle);
		}

		public void FlipHorizontal(){
			this.ThrowIfDisposed();
			this.Gfl.ThrowIfError(this.Gfl.FlipHorizontal(this.Handle));
			this.RefreshGflBitmap(this.Handle);
		}

		public void FlipVertical(){
			this.ThrowIfDisposed();
			this.Gfl.ThrowIfError(this.Gfl.FlipVertical(this.Handle));
			this.RefreshGflBitmap(this.Handle);
		}

		#endregion

		#region Functions Non Destructive

		public static Bitmap Resize(Bitmap src, int width, int height, ResizeMethod method){
			src.ThrowIfDisposed();
			var dst = IntPtr.Zero;
			src.Gfl.ThrowIfError(src.Gfl.Resize(src.Handle, ref dst, width, height, method));
			return new Bitmap(src.Gfl, dst);
		}

		public static Bitmap ResizeCanvas(Bitmap src, int width, int height, ResizeMethod method, ResizeCanvasOrigin origin, Color background){
			src.ThrowIfDisposed();
			var dst = IntPtr.Zero;
			var bg = background.ToGflColor();
			src.Gfl.ThrowIfError(src.Gfl.ResizeCanvas(src.Handle, width, height, method, origin, ref bg));
			return new Bitmap(src.Gfl, dst);
		}

		public static Bitmap Rotate(Bitmap src, int angle, Color background){
			src.ThrowIfDisposed();
			var dst = IntPtr.Zero;
			var bg = background.ToGflColor();
			src.Gfl.ThrowIfError(src.Gfl.Rotate(src.Handle, angle, ref bg));
			return new Bitmap(src.Gfl, dst);
		}

		public static Bitmap RotateFine(Bitmap src, double angle, Color background){
			src.ThrowIfDisposed();
			var dst = IntPtr.Zero;
			var bg = background.ToGflColor();
			src.Gfl.ThrowIfError(src.Gfl.RotateFine(src.Handle, angle, ref bg));
			return new Bitmap(src.Gfl, dst);
		}

		public static Bitmap FlipHorizontal(Bitmap src){
			src.ThrowIfDisposed();
			var dst = IntPtr.Zero;
			src.Gfl.ThrowIfError(src.Gfl.FlipHorizontal(src.Handle));
			return new Bitmap(src.Gfl, dst);
		}

		public static Bitmap FlipVertical(Bitmap src){
			src.ThrowIfDisposed();
			var dst = IntPtr.Zero;
			src.Gfl.ThrowIfError(src.Gfl.FlipVertical(src.Handle));
			return new Bitmap(src.Gfl, dst);
		}

		#endregion

		#region Property

		public int BytesPerLine{
			get{
				this.ThrowIfDisposed();
				return (int)this._GflBitmap.BytesPerLine;
			}
		}

		public int BytesPerPixel{
			get{
				this.ThrowIfDisposed();
				return (int)this._GflBitmap.BytesPerPixel;
			}
		}

		public int Width{
			get{
				this.ThrowIfDisposed();
				return this._GflBitmap.Width;
			}
		}

		public int Height{
			get{
				this.ThrowIfDisposed();
				return this._GflBitmap.Height;
			}
		}

		public IntPtr Scan0{
			get{
				this.ThrowIfDisposed();
				return this._GflBitmap.Data;
			}
		}

		public int TransparentIndex{
			get{
				this.ThrowIfDisposed();
				return this._GflBitmap.TransparentIndex;
			}
		}

		public int BitsPerComponent{
			get{
				this.ThrowIfDisposed();
				return this._GflBitmap.BitsPerComponent;
			}
		}

		public int LinePadding{
			get{
				this.ThrowIfDisposed();
				return this._GflBitmap.LinePadding;
			}
		}

		public Origin Origin{
			get{
				this.ThrowIfDisposed();
				return this._GflBitmap.Origin;
			}
		}

		public int UsedColorCount{
			get{
				this.ThrowIfDisposed();
				return this._GflBitmap.ColorUsed;
			}
		}

		public IntPtr MetaData{
			get{
				this.ThrowIfDisposed();
				return this._GflBitmap.MetaData;
			}
		}
		
		private ColorMap colorMap = null;
		public ColorMap ColorMap{
			get{
				this.ThrowIfDisposed();
				if((this.colorMap == null) && (this._GflBitmap.ColorMap != IntPtr.Zero)){
					this.colorMap = new ColorMap(this._GflBitmap.ColorMap);
				}
				return this.colorMap;
			}
		}
		
		private Exif exif = null;
		public Exif Exif{
			get{
				this.ThrowIfDisposed();
				if(this.exif == null && this._GflBitmap.MetaData != IntPtr.Zero){
					var ptr = IntPtr.Zero;
					try{
						this.Gfl.BitmapGetEXIF(this, Gfl.GetExifOptions.WantMakerNotes);
						var exifData = (Gfl.GflExifData)Marshal.PtrToStructure(ptr, typeof(Gfl.GflExifData));
						this.exif = new Exif(exifData);
					}finally{
						if(ptr != IntPtr.Zero){
							this.Gfl.FreeEXIF(ptr);
						}
					}
				}
				return this.exif;
			}
		}

		public void SaveBitmap(string filename){
			this.ThrowIfDisposed();
			var prms = new Gfl.GflSaveParams();
			this.Gfl.GetDefaultSaveParams(ref prms);
			this.Gfl.ThrowIfError(this.Gfl.SaveBitmap(filename, this, ref prms));
		}

		#endregion

		#region IDisposable

		internal void ThrowIfDisposed(){
			if(this.Disposed){
				throw new ObjectDisposedException("Bitmap");
			}
		}

		public void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~Bitmap(){
			this.Dispose(false);
		}
		
		internal bool Disposed = false;
		protected virtual void Dispose(bool disposing){
			try{
				this.Gfl.DisposeBitmap(this);
			}catch{
			}
		}

		#endregion
	}
}
