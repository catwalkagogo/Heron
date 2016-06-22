/*
	$Id: Stream.cs 159 2011-03-07 07:14:41Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassNet2.Channels{
	public class Stream : PlayChannel{
		internal Stream(IntPtr handle) : base(handle){
		}
		
		public static Stream FromFile(string path){
			IntPtr handle = Bass.CreateStreamFromFile(false, path, 0, 0, Bass.Options.None);
			if(handle != IntPtr.Zero){
				return new Stream(handle);
			}else{
				throw new BassException(Bass.GetErrorCode());
			}
		}
		
		public void GetLevel(out int left, out int right){
			if(this.Disposed){
				throw new ObjectDisposedException("");
			}
			int level = Bass.GetChannelLevel(this.Handle);
			if(level != -1){
				left = level & 0xffff;
				right = level >> 16;
			}else{
				throw new BassException(Bass.GetErrorCode());
			}
		}
	}
}
