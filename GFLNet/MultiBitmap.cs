/*
	$Id: MultiBitmap.cs 279 2011-08-04 10:24:25Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	using IO = System.IO;

	public class MultiBitmap : IDisposable, IEnumerable<Bitmap>{
		private Bitmap[] _Frames;
		private Gfl _Gfl;
		private string _Path;
		private IO::Stream _Stream;
		public FileInformation FileInformation{get; private set;}

		internal MultiBitmap(Gfl gfl, string path, FileInformation info){
			this._Path = IO.Path.GetFullPath(path);
			this._Frames = new Bitmap[info.ImageCount];
			this._Gfl = gfl;
			this.FileInformation = info;
			this.LoadParameters = this._Gfl.GetDefaultLoadParameters();
			this.LoadParameters.Format = info.Format;
		}

		internal MultiBitmap(Gfl gfl, IO::Stream stream, FileInformation info){
			this._Stream = stream;
			this._Frames = new Bitmap[info.ImageCount];
			this._Gfl = gfl;
			this.FileInformation = info;
			this.LoadParameters = this._Gfl.GetDefaultLoadParameters();
			this.LoadParameters.Format = info.Format;
		}

		public void LoadAllFrames(){
			if(this.FileInformation == null){
				this.LoadFrame(0);
			}
			for(var i = 0; i < this._Frames.Length; i++){
				if(this._Frames[i] == null){
					this.LoadFrame(i);
				}
			}
		}

		public void LoadFrame(int index){
			try{
				this.OnFrameLoading(EventArgs.Empty);
				FileInformation info;
				Bitmap bitmap = null;
				if(this._Path != null){
					bitmap = this._Gfl.LoadBitmap(this._Path, index, this.LoadParameters, out info, this);
				}else{
					this._Stream.Seek(0, IO::SeekOrigin.Begin);
					bitmap = this._Gfl.LoadBitmap(this._Stream, index, this.LoadParameters, out info, this);
				}
				this.FileInformation = info;
				this._Frames[index] = bitmap;
				this.OnFrameLoaded(new FrameLoadedEventArgs(bitmap));
			}catch(Exception ex){
				this.OnFrameLoadFailed(new FrameLoadFailedEventArgs(ex));
				throw ex;
			}
		}

		public Bitmap this[int index]{
			get{
				if(index < 0 || index >= this._Frames.Length){
					throw new ArgumentOutOfRangeException();
				}
				if(this._Frames[index] == null){
					this.LoadFrame(index);
				}
				return this._Frames[index];
			}
		}

		private LoadParameters _LoadParameters;
		public LoadParameters LoadParameters{
			get{
				return this._LoadParameters;
			}
			set{
				if(value == null){
					throw new ArgumentNullException();
				}
				this._LoadParameters = value;
			}
		}

		public int FrameCount{
			get{
				return this._Frames.Length;
			}
		}

		public Bitmap GetThumbnail(int width, int height){
			try{
				this.OnFrameLoading(EventArgs.Empty);
				if(this._Path != null){
					FileInformation info;
					var bmp = this._Gfl.LoadThumbnail(this._Path, width, height, this.LoadParameters, out info, this);
					this.FileInformation = info;
					this.OnFrameLoaded(new FrameLoadedEventArgs(bmp));
					return bmp;
				}else{
					FileInformation info;
					var bmp = this._Gfl.LoadThumbnail(this._Stream, width, height, this.LoadParameters, out info, this);
					this.FileInformation = info;
					this.OnFrameLoaded(new FrameLoadedEventArgs(bmp));
					return bmp;
				}
			}catch(Exception ex){
				this.OnFrameLoadFailed(new FrameLoadFailedEventArgs(ex));
			}
			return null;
		}

		#region event

		public event EventHandler FrameLoading;
		protected virtual void OnFrameLoading(EventArgs e){
			var eh = this.FrameLoading;
			if(eh != null){
				eh(this, e);
			}
		}

		public event FrameLoadedEventHandler FrameLoaded;

		protected virtual void OnFrameLoaded(FrameLoadedEventArgs e){
			var eh = this.FrameLoaded;
			if(eh != null){
				eh(this, e);
			}
		}

		public event FrameLoadFailedEventHandler FrameLoadFailed;
		protected virtual void OnFrameLoadFailed(FrameLoadFailedEventArgs e){
			var eh = this.FrameLoadFailed;
			if(eh != null){
				eh(this, e);
			}
		}

		#endregion

		#region IDisposable

		public void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~MultiBitmap(){
			this.Dispose(false);
		}
		
		private bool disposed = false;
		protected virtual void Dispose(bool disposing){
			if(!(this.disposed)){
				foreach(var bitmap in this._Frames.Where(bmp => bmp != null)){
					bitmap.Dispose();
				}
				this.disposed = true;
			}
		}

		#endregion

		#region IEnumerable<Bitmap> Members

		public IEnumerator<Bitmap> GetEnumerator() {
			for(var i = 0; i < this._Frames.Length; i++){
				if(this._Frames[i] == null){
					this.LoadFrame(i);
				}
				yield return this._Frames[i];
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		#endregion
	}

	public delegate void FrameLoadedEventHandler(object sender, FrameLoadedEventArgs e);

	public class FrameLoadedEventArgs : EventArgs{
		public Bitmap Frame{get; private set;}

		public FrameLoadedEventArgs(Bitmap frame){
			this.Frame = frame;
		}
	}

	public delegate void FrameLoadFailedEventHandler(object sender, FrameLoadFailedEventArgs e);

	public class FrameLoadFailedEventArgs : EventArgs{
		public Exception Exception{get; private set;}
		public FrameLoadFailedEventArgs(Exception ex){
			this.Exception = ex;
		}
	}
}
