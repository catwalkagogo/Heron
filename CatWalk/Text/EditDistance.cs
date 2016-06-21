/*
 *	$Id$
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.Text {
	public static class EditDistance {
		public static int GetEditDistanceTo(this string str1, string str2){
			var d = new int[str1.Length + 1, str2.Length + 1];
			var cost = 0;
			for(var i = 0; i <= str1.Length; i++){
				d[i, 0] = i;
			}
			for(var i = 0; i <= str2.Length; i++){
				d[0, i] = i;
			}
			for(var i = 1; i <= str1.Length; i++){
				for(var j = 1; j <= str2.Length; j++){
					if(str1[i].Equals(str2[j])){
						cost = 0;
					}else{
						cost = 1;
					}
					var c1 = d[i - 1, j] + 1;	// ins
					var c2 = d[i, j - 1] + 1;	// del
					var c3 = d[i - 1, j - 1] + cost; // replace
					if(c1 < c2){
						if(c1 < c3){
							d[i, j] = c1;
						}else{
							d[i, j] = c3;
						}
					}else{
						if(c2 < c3){
							d[i, j] = c2;
						}else{
							d[i, j] = c3;
						}
					}
				}
			}
			return d[str1.Length, str2.Length];
		}
	}
}
