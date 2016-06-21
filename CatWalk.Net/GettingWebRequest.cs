using System;
using System.Net;
using System.Linq;
using System.IO;
using System.Threading;

namespace CatWalk.Net {
	public class GettingWebRequest{
		public WebRequest WebRequest{get; private set;}
		private WebResponse _Response;
		private Stream _ResponseStream;
		private bool _IsTimedout;
		protected Exception AsyncException{get; set;}
		private readonly object _SyncObject = new object();

		public GettingWebRequest(WebRequest req){
			if(req == null){
				throw new ArgumentNullException("req");
			}
			this.WebRequest = req;
			this._IsTimedout = false;
		}

		public virtual WebResponse GetResponse(){
			return this.WebRequest.GetResponse();
		}
		public virtual WebResponse GetResponse(CancellationToken token){
			if(token == CancellationToken.None){
				return this.GetResponse();
			}
			token.Register(this.WebRequest.Abort);

			IAsyncResult result;
			try{
				result = this.WebRequest.BeginGetResponse(this.GetResponseCallback, null);
			}catch(WebException ex){ // Aborted
				throw new OperationCanceledException("Operation cancelled", ex, token);
			}
			this.WaitAndTimeoutRequest(result);
			lock(this._SyncObject){
				if(this._Response == null){
					throw new OperationCanceledException("Operation cancelled", token);
				}
				return this._Response;
			}
		}

		public virtual Stream GetStream(){
			return this.WebRequest.GetResponse().GetResponseStream();
		}
		public virtual Stream GetStream(CancellationToken token){
			if(token == CancellationToken.None){
				return GetStream();
			}
			token.Register(this.WebRequest.Abort);
			
			IAsyncResult result;
			try{
				result = this.WebRequest.BeginGetResponse(this.GetStreamCallback, null);
			}catch(WebException ex){ // Aborted
				throw new OperationCanceledException("Operation cancelled", ex, token);
			}
			this.WaitAndTimeoutRequest(result);
			lock(this._SyncObject){
				if(this._ResponseStream == null){
					throw new OperationCanceledException("Operation cancelled", token);
				}
				return this._ResponseStream;
			}
		}

		private void GetResponseCallback(IAsyncResult async){
			lock(this._SyncObject){
				try{
					this._Response = this.WebRequest.EndGetResponse(async);
				}catch(Exception ex){
					this.AsyncException = ex;
				}
			}
		}

		private void GetStreamCallback(IAsyncResult async){
			lock(this._SyncObject){
				try{
						this._Response = this.WebRequest.EndGetResponse(async);
						this._ResponseStream = this._Response.GetResponseStream();
						if(this._ResponseStream == null){
							this.AsyncException = new WebException();
						}
				}catch(Exception ex){
					this.AsyncException = ex;
				}
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
				if(this.AsyncException is WebException){
					throw this.AsyncException;
				}else{
					throw new WebException("Request was failed", this.AsyncException);
				}
			}else if(this._IsTimedout){
				if(this.AsyncException is WebException){
					throw this.AsyncException;
				}else{
					throw new WebException("Request was timeout", WebExceptionStatus.Timeout);
				}
			}
		}
	}
}
