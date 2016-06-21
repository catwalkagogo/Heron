using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace GflNet {
	public partial class GflExtended : CatWalk.Win32.InteropObject{
		public GflExtended(string dllName) : base(dllName){
		}

		#region Filter

		public void Sharpen(Bitmap src, int percentage, out Bitmap dst){
			if(percentage < 0 || percentage >= 100){
				throw new ArgumentOutOfRangeException("percentage");
			}
			this.ThrowIfDisposed();
			src.ThrowIfDisposed();
			var pdst = IntPtr.Zero;
			src.Gfl.ThrowIfError(this.Sharpen(src.Handle, ref pdst, percentage));
			dst = new Bitmap(src.Gfl, pdst);
		}

		#endregion

		#region Filter Destructive

		public void Sharpen(Bitmap src, int percentage){
			if(percentage < 0 || percentage >= 100){
				throw new ArgumentOutOfRangeException("percentage");
			}
			this.ThrowIfDisposed();
			src.ThrowIfDisposed();
			src.Gfl.ThrowIfError(this.Sharpen(src.Handle, percentage));
		}

		#endregion

		#region Misc
		/*
		public void JpegLosslessTransform(string path, JpegLosslessTransform transform){
			this.ThrowIfDisposed();
			if(this.JpegLosslessTransformInternal(path, transform) != Gfl.Error.None){
				throw new IOException();
			}
		}
		*/
		#endregion
	}
}
