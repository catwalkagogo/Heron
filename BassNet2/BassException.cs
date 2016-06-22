/*
	$Id: BassException.cs 159 2011-03-07 07:14:41Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassNet2 {
	public class BassException : Exception{
		public BassErrorCode ErrorCode{get; private set;}
		
		public BassException(){
			this.ErrorCode = BassErrorCode.Unknown;
		}
		
		public BassException(BassErrorCode code){
			this.ErrorCode = code;
		}
		
		public BassException(string message) : base(message){
		}
		
		protected BassException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context){
		}
		
		public BassException(string message, Exception innerException) : base(message, innerException){
		}
	}
}
