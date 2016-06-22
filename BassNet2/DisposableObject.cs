/*
	$Id: DisposableObject.cs 205 2011-04-22 16:11:08Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;

namespace BassNet2{
	public abstract class DisposableObject : IDisposable{
		public void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(bool disposing){
		}
		
		~DisposableObject(){
			this.Dispose(false);
		}
	}
}