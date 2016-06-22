/*
	$Id: PlayChannel.cs 159 2011-03-07 07:14:41Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassNet2.Channels{
	public abstract class PlayChannel : Channel{
		public PlayChannel(IntPtr handle) : base(handle){
		}
		
		public void Seek(long bytes){
			if(this.Disposed){
				throw new ObjectDisposedException("");
			}
			if(Bass.SetChannelPosition(this.Handle, bytes, Bass.PositionMode.Byte)){
				return;
			}else{
				throw new BassException(Bass.GetErrorCode());
			}
		}
		
		public void Seek(double seconds){
			if(this.Disposed){
				throw new ObjectDisposedException("");
			}
			long bytes = Bass.ChannelSeconds2Bytes(this.Handle, seconds);
			if(bytes == -1){
				throw new BassException(Bass.GetErrorCode());
			}else{
				this.Seek(bytes);
			}
		}
		
		public BassChannelInfo GetChannelInfo(){
			if(this.Disposed){
				throw new ObjectDisposedException("");
			}
			Bass.ChannelInfo info;
			if(Bass.GetChannelInfo(this.Handle, out info)){
				return new BassChannelInfo(info);
			}else{
				throw new BassException(Bass.GetErrorCode());
			}
		}
	}
}
