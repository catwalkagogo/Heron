/*
	$Id: Channel.cs 159 2011-03-07 07:14:41Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassNet2.Channels{
	public abstract class Channel : IDisposable{
		public IntPtr Handle{get; private set;}
		
		public Channel(IntPtr handle){
			this.Handle = handle;
		}
		
		public void Play(){
			this.Play(false);
		}
		
		public void Play(bool restart){
			if(this.disposed){
				throw new ObjectDisposedException("");
			}
			if(Bass.PlayChannel(this.Handle, restart)){
				return;
			}else{
				throw new BassException(Bass.GetErrorCode());
			}
		}
		
		public void Pause(){
			if(this.disposed){
				throw new ObjectDisposedException("");
			}
			if(Bass.PauseChannel(this.Handle)){
				return;
			}else{
				throw new BassException(Bass.GetErrorCode());
			}
		}
		
		public void Stop(){
			if(this.disposed){
				throw new ObjectDisposedException("");
			}
			if(Bass.StopChannel(this.Handle)){
				return;
			}else{
				throw new BassException(Bass.GetErrorCode());
			}
		}
		
		public ChannelState State{
			get{
				if(this.disposed){
					throw new ObjectDisposedException("");
				}
				return Bass.GetChannelState(this.Handle);
			}
		}
		
		public double BytesToSeconds(long bytes){
			if(this.disposed){
				throw new ObjectDisposedException("");
			}
			double seconds = Bass.ChannelBytes2Seconds(this.Handle, bytes);
			if(seconds < 0){
				throw new BassException(Bass.GetErrorCode());
			}else{
				return seconds;
			}
		}
		
		public long SecondsToBytes(double seconds){
			if(this.disposed){
				throw new ObjectDisposedException("");
			}
			long bytes = Bass.ChannelSeconds2Bytes(this.Handle, seconds);
			if(bytes < 0){
				throw new BassException(Bass.GetErrorCode());
			}else{
				return bytes;
			}
		}
		
		public long PositionInByte{
			get{
				if(this.disposed){
					throw new ObjectDisposedException("");
				}
				long pos = Bass.GetChannelPosition(this.Handle, Bass.PositionMode.Byte);
				if(pos != -1){
					return pos;
				}else{
					throw new BassException(Bass.GetErrorCode());
				}
			}
		}
		
		public double PositionInSecond{
			get{
				if(this.disposed){
					throw new ObjectDisposedException("");
				}
				double seconds = Bass.ChannelBytes2Seconds(this.Handle, this.PositionInByte);
				if(seconds < 0){
					throw new BassException(Bass.GetErrorCode());
				}else{
					return seconds;
				}
			}
		}
		
		public long Bytes{
			get{
				if(this.disposed){
					throw new ObjectDisposedException("");
				}
				long bytes = Bass.GetChannelLength(this.Handle, Bass.PositionMode.Byte);
				if(bytes == -1){
					throw new BassException(Bass.GetErrorCode());
				}else{
					return bytes;
				}
			}
		}
		
		public double Seconds{
			get{
				if(this.disposed){
					throw new ObjectDisposedException("");
				}
				double seconds = Bass.ChannelBytes2Seconds(this.Handle, this.Bytes);
				if(seconds < 0){
					throw new BassException(Bass.GetErrorCode());
				}else{
					return seconds;
				}
			}
		}
		
		
		#region IDisposable
		
		protected bool Disposed{
			get{
				return this.disposed;
			}
		}
		
		private bool disposed = false;
		public void Dispose(){
			try{
				this.Dispose(true);
			}finally{
				if(!this.disposed){
					Bass.FreeStream(this.Handle);
					this.disposed = true;
					GC.SuppressFinalize(this);
				}
			}
		}
		
		protected virtual void Dispose(bool disposing){
		}
		
		~Channel(){
			this.Dispose(false);
		}
		
		#endregion

	}

}
