using System;
using System.Net;
using System.Linq;
using System.IO;
using System.Threading;

namespace CatWalk.Net {
	public class PostingWebRequest : GettingWebRequest{
		private byte[] _RequestData;
		private Stream _RequestStream;

		public PostingWebRequest(WebRequest req, Stream data) : base(req){
			if(data == null){
				throw new ArgumentNullException("data");
			}
			this._RequestStream = data;
		}
	
		public PostingWebRequest(WebRequest req, byte[] data) : base(req){
			if(data == null){
				throw new ArgumentNullException("data");
			}
			this._RequestData = data;
		}

		public override Stream GetStream() {
			// Not posted yet
			if(!(this._RequestData == null && this._RequestStream == null)){
				throw new InvalidOperationException();
			}
			return base.GetStream();
		}

		public override Stream GetStream(CancellationToken token) {
			// Not posted yet
			if(!(this._RequestData == null && this._RequestStream == null)){
				throw new InvalidOperationException();
			}
			return base.GetStream(token);
		}

		public virtual void Post(){
			// Posted
			if(this._RequestData == null && this._RequestStream == null){
				throw new InvalidOperationException();
			}
			if(this._RequestData != null){
				using(Stream stream = this.WebRequest.GetRequestStream()){
					stream.Write(this._RequestData, 0, this._RequestData.Length);
				}
				this._RequestData = null;
			}else{
				try{
					using(Stream stream = this.WebRequest.GetRequestStream()){
						const int block = 1024 * 64;
						var buffer = new byte[block];
						int length;
						while((length = this._RequestStream.Read(buffer, 0, block)) > 0){
							stream.Write(buffer, 0, length);
						}
					}
				}finally{
					this._RequestStream.Dispose();
				}
				this._RequestStream = null;
			}
		}

		public virtual void Post(CancellationToken token){
			this.Post(DummyCallback, token);
		}

		public virtual void Post(WebRequestProgressEventHandler progressCallback, CancellationToken token){
			if(token == CancellationToken.None){
				Post();
			}
			// Posted
			if(this._RequestData == null && this._RequestStream == null){
				throw new InvalidOperationException();
			}
			if(progressCallback == null){
				throw new ArgumentNullException("progressCallback");
			}
			token.Register(this.WebRequest.Abort);
			var result = this.WebRequest.BeginGetRequestStream(this.PostCallback, progressCallback);
			this.WaitAndTimeoutRequest(result);
		}

		private void PostCallback(IAsyncResult async){
			var progressCallback = (WebRequestProgressEventHandler)async.AsyncState;
			try{
				var inStream = (this._RequestData != null) ? new MemoryStream(this._RequestData) : this._RequestStream;
				var inLength = (inStream.CanSeek) ? inStream.Length : -1;
				int block = (this._RequestData != null) ? this._RequestData.Length : 1024 * 64;
				try{
					using(Stream stream = this.WebRequest.EndGetRequestStream(async)){
						var buffer = new byte[block];
						long current = 0;
						int length;
						while((length = inStream.Read(buffer, 0, block)) > 0){
							stream.Write(this._RequestData, 0, length);
							current += length;
							progressCallback(this, new WebRequestProgressEventArgs(current, inLength));
						}
					}
				}finally{
					inStream.Dispose();
				}
			}catch(Exception ex){
				this.AsyncException = ex;
			}finally{
				this._RequestStream = null;
				this._RequestData = null;
			}
		}

		private static void DummyCallback(object sender, EventArgs e){}
	}

	public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);

	public class ProgressEventArgs : EventArgs{
		public double Progress{get; private set;}

		public ProgressEventArgs(double progress){
			this.Progress = progress;
		}
	}

	public delegate void WebRequestProgressEventHandler(object sender, WebRequestProgressEventArgs e);

	public class WebRequestProgressEventArgs : ProgressEventArgs{
		public long ContentLength{get; private set;}
		public long CurrentLength{get; private set;}

		public WebRequestProgressEventArgs(long current, long length) : base((double)current / (double)length){
			this.ContentLength = length;
			this.CurrentLength = current;
		}
	}

}
