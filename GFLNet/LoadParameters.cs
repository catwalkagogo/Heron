/*
	$Id: LoadParameters.cs 319 2014-01-03 04:23:35Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace GflNet{
	using IO = System.IO;

	[Serializable]
	public class LoadParameters{
		public BitmapType BitmapType{get; set;}
		public LoadOptions Options{get; set;}
		public Origin Origin{get; set;}
		public Format Format{get; set;}
		public byte DefaultAlpha{get; set;}
		internal IO::Stream StreamToHandle{get; set;}

		internal LoadParameters(Gfl.GflLoadParams prms){
			this.BitmapType = prms.ColorModel;
			this.Options = prms.Options;
			this.Origin = prms.Origin;
			this.Format = Format.AnyFormats;
			this.DefaultAlpha = prms.DefaultAlpha;
		}

		internal void ToGflLoadParams(object sender, ref Gfl.GflLoadParams prms){
			prms.Options = this.Options;
			prms.ColorModel = this.BitmapType;
			prms.Origin = this.Origin;
			prms.FormatIndex = this.Format.Index;
			prms.DefaultAlpha = this.DefaultAlpha;
			prms.Callbacks.Progress = this.GetProgressCallback(sender);
			prms.Callbacks.WantCancel = this.GetWantCancelCallback(sender);
			if(this.StreamToHandle != null){
				if(this.StreamToHandle is IO::FileStream){
					prms.Callbacks.Read = this.ReadCallbackNative;
					prms.Callbacks.Tell = this.TellCallbackNative;
					prms.Callbacks.Seek = this.SeekCallbackNative;
				}else{
					prms.Callbacks.Read = this.ReadCallback;
					prms.Callbacks.Tell = this.TellCallback;
					prms.Callbacks.Seek = this.SeekCallback;
				}
			}else{
				prms.Callbacks.Read = null;
				prms.Callbacks.Tell = null;
				prms.Callbacks.Seek = null;
			}
		}

		private uint ReadCallback(IntPtr handle, IntPtr buffer, uint size){
			var b = new byte[size];
			var n = this.StreamToHandle.Read(b, 0, (int)size);
			Marshal.Copy(b, 0, buffer, n);
			return (uint)n;
		}

		private uint TellCallback(IntPtr handle){
			var len = (uint)this.StreamToHandle.Position;
			return len;
		}

		private uint SeekCallback(IntPtr handle, int offset, SeekOrigin origin){
			switch(origin){
				case SeekOrigin.Begin: return (uint)this.StreamToHandle.Seek(offset, IO::SeekOrigin.Begin);
				case SeekOrigin.Current: return (uint)this.StreamToHandle.Seek(offset, IO::SeekOrigin.Current);
				case SeekOrigin.End: return (uint)this.StreamToHandle.Seek(offset, IO::SeekOrigin.End);
				default: throw new ArgumentException("origin");
			}
		}

		private uint ReadCallbackNative(IntPtr handle, IntPtr buffer, uint size){
			uint n;
			NativeMethods.ReadFile(handle, buffer, size, out n, IntPtr.Zero);
			return (uint)n;
		}

		private uint TellCallbackNative(IntPtr handle){
			var len = NativeMethods.SetFilePointer(handle, 0, IntPtr.Zero, SeekOrigin.Current);
			return len;
		}

		private uint SeekCallbackNative(IntPtr handle, int offset, SeekOrigin origin){
			var n = NativeMethods.SetFilePointer(handle, offset, IntPtr.Zero, origin);
			return n;
		}

		#region Callbacks

		public event ProgressEventHandler ProgressChanged;
		public event CancelEventHandler WantCancel;

		internal Gfl.ProgressCallback GetProgressCallback(object sender){
			if(this.ProgressChanged != null){
				return new Gfl.ProgressCallback(delegate(int percentage, IntPtr args){
					var eh = this.ProgressChanged;
					if(eh != null){
						eh(sender, new ProgressEventArgs(percentage));
					}
				});
			}else{
				return null;
			}
		}

		internal Gfl.WantCancelCallback GetWantCancelCallback(object sender){
			if(this.WantCancel != null){
				return new Gfl.WantCancelCallback(delegate(IntPtr args){
					var eh = this.WantCancel;
					if(eh != null){
						var e = new CancelEventArgs();
						eh(sender, e);
						return e.Cancel;
					}else{
						return false;
					}
				});
			}else{
				return null;
			}
		}

		#endregion
	}
}
