/*
	$Id: Token.cs 259 2011-07-24 06:19:28Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace CatWalk.Net.OAuth{
	/// <summary>
	/// Inter face of token.
	/// </summary>
#if !SILVERLIGHT
	[Serializable]
#endif
	public abstract class Token{
		private string tokenValue;
		private string tokenSecret;
				
		public Token(string tokenValue, string tokenSecret){
			this.tokenValue = tokenValue;
			this.tokenSecret = tokenSecret;
		}
		
		public string TokenValue{
			get{
				return this.tokenValue;
			}
		}
		public string TokenSecret{
			get{
				return this.tokenSecret;
			}
		}

    }
}
