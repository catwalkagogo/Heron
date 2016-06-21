/*
	$Id: Diff.cs 142 2010-12-19 11:39:25Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.Text{
	public static class Diff{
		public static string GetLongestCommonSubsequence(this string s1, string s2){
			return ReadLCSFromBacktrack(GetLongestCommonSubsequenceMatrix(s1, s2), s1, s2, s1.Length - 1, s2.Length - 1).ToString();
		}

		private static StringBuilder ReadLCSFromBacktrack(int[,] backtrack, string string1, string string2, int s1position, int s2posision){
			if((s1position <= 0) || (s2posision <= 0)){
				return new StringBuilder();
			}else if(string1[s1position] == string2[s2posision]){
				return ReadLCSFromBacktrack(backtrack, string1, string2, s1position - 1, s2posision - 1).Append(string1[s1position]);
			}else{
				if(backtrack[s1position, s2posision - 1] >= backtrack[s1position - 1, s2posision]){
					return ReadLCSFromBacktrack(backtrack, string1, string2, s1position, s2posision - 1);
				}else{
					return ReadLCSFromBacktrack(backtrack, string1, string2, s1position - 1, s2posision);
				}
			}
		}

		public static int[,] GetLongestCommonSubsequenceMatrix(this string s1, string s2){
			int[,] lcsMatrix = new int[s1.Length, s2.Length];
			char letter1, letter2;

			for(int i = 0; i < s1.Length; i++){
				for(int j = 0; j < s2.Length; j++){
					letter1 = s1[i];
					letter2 = s2[j];

					if(letter1 == letter2){
						if((i == 0) || (j == 0))
							lcsMatrix[i, j] = 1;
						else
							lcsMatrix[i, j] = 1 + lcsMatrix[i - 1, j - 1];
					}else{
						if((i == 0) && (j == 0))
							lcsMatrix[i, j] = 0;
						else if((i == 0) && !(j == 0))
							lcsMatrix[i, j] = Math.Max(0, lcsMatrix[i, j - 1]);
						else if(!(i == 0) && (j == 0))
							lcsMatrix[i, j] = Math.Max(lcsMatrix[i - 1, j], 0);
						else if(!(i == 0) && !(j == 0))
							lcsMatrix[i, j] = Math.Max(lcsMatrix[i - 1, j], lcsMatrix[i, j - 1]);
					}
				}
			}
			return lcsMatrix;
		}
	}
}