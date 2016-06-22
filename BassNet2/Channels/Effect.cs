/*
	$Id: Effect.cs 159 2011-03-07 07:14:41Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassNet2.Channels{
	public abstract class Effect : IDisposable{
		protected IntPtr Handle{get; private set;}
		public Channel Channel{get; private set;}
		
		internal Effect(Channel channel, Bass.EffectType type, int priority){
			if(channel == null){
				throw new ArgumentNullException();
			}
			if(!Enum.IsDefined(typeof(Bass.EffectType), type)){
				throw new ArgumentException();
			}
			this.Handle = Bass.SetChannelEffect(channel.Handle, type, priority);
			this.Channel = channel;
		}
		
		#region IDisposable
		
		private bool disposed = false;
		public void Dispose(){
			try{
				this.Dispose(true);
			}finally{
				if(!this.disposed){
					Bass.RemoveChannelEffect(this.Channel.Handle, this.Handle);
					this.disposed = true;
					GC.SuppressFinalize(this);
				}
			}
		}
		
		protected virtual void Dispose(bool disposing){
		}
		
		~Effect(){
			this.Dispose(false);
		}
		
		#endregion
	}
}
