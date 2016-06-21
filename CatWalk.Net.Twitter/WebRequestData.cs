using System;
using System.Net;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CatWalk.Net.Twitter {
	public class GettingWebRequest{
		public WebRequest WebRequest{get; private set;}
		private Stream _ResponseStream;
		private bool _IsTimedout;
		protected Exception AsyncException{get; set;}

		public GettingWebRequest(WebRequest req){
			if(req == null){
				throw new ArgumentNullException("req");
			}
			this.WebRequest = req;
			this._IsTimedout = false;
		}

		public virtual Stream Get(){
			using(var response = this.WebRequest.GetResponse()){
				return response.GetResponseStream();
			}
		}
		public virtual Stream Get(CancellationToken token){
			token.Register(this.WebRequest.Abort);
			var result = this.WebRequest.BeginGetResponse(this.GetCallback, null);
			this.WaitAndTimeoutRequest(result);
			return this._ResponseStream;
		}

		private void GetCallback(IAsyncResult async){
			try{
				this._ResponseStream = this.WebRequest.EndGetResponse(async).GetResponseStream();
			}catch(Exception ex){
				this.AsyncException = ex;
			}
		}

		private void TimeoutCallback(object state, bool timedOut){
			if(timedOut){
				this.WebRequest.Abort();
				this._IsTimedout = true;
			}
		}

		protected void WaitAndTimeoutRequest(IAsyncResult async){
			ThreadPool.RegisterWaitForSingleObject(async.AsyncWaitHandle, TimeoutCallback, null, this.WebRequest.Timeout, true);
			async.AsyncWaitHandle.WaitOne();
			if(this.AsyncException != null){
				throw new WebException("Request was failed", this.AsyncException);
			}
			if(this._IsTimedout){
				if(this.AsyncException is WebException){
					throw this.AsyncException;
				}else{
					throw new WebException("Request was timeout", WebExceptionStatus.Timeout);
				}
			}
		}
	}

	public class PostingWebRequest : GettingWebRequest{
		private byte[] _RequestData;

		public PostingWebRequest(WebRequest req, byte[] data) : base(req){
			if(data == null){
				throw new ArgumentNullException("data");
			}
			this._RequestData = data;
		}

		public override Stream Get() {
			// Not posted yet
			if(this._RequestData != null){
				throw new InvalidOperationException();
			}
			return base.Get();
		}

		public override Stream Get(CancellationToken token) {
			// Not posted yet
			if(this._RequestData != null){
				throw new InvalidOperationException();
			}
			return base.Get(token);
		}

		public virtual void Post(){
			if(this._RequestData == null){
				throw new InvalidOperationException();
			}
			using(Stream stream = this.WebRequest.GetRequestStream()){
				stream.Write(this._RequestData, 0, this._RequestData.Length);
			}
			this._RequestData = null;
		}

		public virtual void Post(CancellationToken token){
			this.Post(DummyCallback, token);
		}

		public virtual void Post(WebRequestProgressEventHandler progressCallback, CancellationToken token){
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
				const int blockLength = 1024;
				using(Stream stream = this.WebRequest.EndGetRequestStream(async)){
					int length = this._RequestData.Length;
					for(int offset = 0; offset < this._RequestData.Length; offset += blockLength){
						stream.Write(this._RequestData, offset, Math.Min(blockLength, this._RequestData.Length - offset));
						progressCallback(this, new WebRequestProgressEventArgs(offset, length));
					}
				}
			}catch(Exception ex){
				this.AsyncException = ex;
			}finally{
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
		public int ContentLength{get; private set;}
		public int CurrentLength{get; private set;}

		public WebRequestProgressEventArgs(int current, int length) : base((double)current / (double)length){
			this.ContentLength = length;
			this.CurrentLength = current;
		}
	}
}
