/*
	$Id: RomanToJapanese.cs 130 2010-12-18 05:34:57Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CatWalk.Collections;

namespace CatWalk.Text{
	/*
	public class RomanToJapanese{
		private static CharComparerIgnoreCase charComaprer = new CharComparerIgnoreCase();
		private PrefixDictionary<string[]> dictionary = new PrefixDictionary<string[]>(charComaprer);
		private IDictionary<string, string> romanToHiraDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		private IDictionary<char, char> zenToHanDictionary = new Dictionary<char, char>(charComaprer);
		private IDictionary<char, char> hiraToKataDictionary = new Dictionary<char, char>(charComaprer);
		private IDictionary<char, char> kataToHiraDictionary = new Dictionary<char, char>(charComaprer);
		private IDictionary<char, char> hanToZenDictionary = new Dictionary<char, char>(charComaprer);
		
		public RomanToJapanese(string dict, string romanToHiraDict, string zenToHanDict, string hiraToKataDict){
			this.ReadDictionary(dict);
			this.ReadRomanToHiraDictionary(romanToHiraDict);
			this.ReadZenToHanDictionary(zenToHanDict);
			this.ReadHiraToKataDictionary(hiraToKataDict);
		}
		
		public void ReadDictionary(string dict){
			this.dictionary.Clear();
			
			string path = Path.GetFullPath(dict);
			foreach(string line in File.ReadAllLines(path)){
				string[] strs = line.Split(new char[]{' ', '\t'}).Where((str) => !(String.IsNullOrEmpty(str))).ToArray();
				if(strs.Length > 1){
					string key = strs[0];
					this.dictionary[key] = strs.Where((str, idx) => (idx > 0)).ToArray();
				}
			}
		}
		
		public void ReadRomanToHiraDictionary(string path){
			this.romanToHiraDictionary.Clear();
			
			path = Path.GetFullPath(path);
			foreach(string line in File.ReadAllLines(path)){
				string[] strs = line.Split(new char[]{' ', '\t'}).Where((str) => !(String.IsNullOrEmpty(str))).ToArray();
				if(strs.Length > 1){
					this.romanToHiraDictionary[strs[0]] = strs[1];
				}
			}
		}
		
		public void ReadZenToHanDictionary(string path){
			this.zenToHanDictionary.Clear();
			
			path = Path.GetFullPath(path);
			foreach(string line in File.ReadAllLines(path)){
				string[] strs = line.Split(new char[]{' ', '\t'}).Where((str) => !(String.IsNullOrEmpty(str))).ToArray();
				if(strs.Length > 1){
					this.zenToHanDictionary[strs[0][0]] = strs[1][0];
					this.hanToZenDictionary[strs[1][0]] = strs[0][0];
				}
			}
		}
		
		public void ReadHiraToKataDictionary(string path){
			this.hiraToKataDictionary.Clear();
			
			path = Path.GetFullPath(path);
			foreach(string line in File.ReadAllLines(path)){
				string[] strs = line.Split(new char[]{' ', '\t'}).Where((str) => !(String.IsNullOrEmpty(str))).ToArray();
				if(strs.Length > 1){
					this.hiraToKataDictionary[strs[0][0]] = strs[1][0];
					this.kataToHiraDictionary[strs[1][0]] = strs[0][0];
				}
			}
		}
		
		public string[] GetWords(string word){
			string[] words;
			this.dictionary.TryGetValue(word, out words);
			return words;
		}
		
		public string RomanToHira(string roman){
			StringBuilder sb = new StringBuilder(roman.Length);
			for(int s = 0, i = 1; i <= roman.Length; i++){
				string hira;
				if(this.romanToHiraDictionary.TryGetValue(roman.Substring(s, i - s), out hira)){
					sb.Append(hira);
					s = i;
				}
			}
			return sb.ToString();
		}
		
		public string GetPattern(string query){
			List<string> patterns = new List<string>();
			foreach(string roman in this.Tokenize(query)){
				Console.WriteLine(roman);
				patterns.Add(this.GetPatternFromRoman(roman));
			}
			if(patterns.Count > 1){
				return "(" + String.Join(")(", patterns.ToArray()) + ")";
			}else if(patterns.Count == 1){
				return patterns[0];
			}else{
				return "";
			}
		}
		
		private string GetPatternFromRoman(string roman){
			string hira = this.RomanToHira(roman);
			if(!(String.IsNullOrEmpty(hira))){
				StringBuilder pattern = new StringBuilder();
				foreach(string query in new string[]{hira, roman}){
					string[] words = this.GetWords(query);
					if(words != null){
						PrefixDictionary<object> dic = new PrefixDictionary<object>();
						foreach(string word in words){
							dic[word] = null;
						}
						
						pattern.Append(this.GetPattern(dic.Root));
						pattern.Append("|");
					}
				}
				
				for(int i = 0; i < roman.Length; i++){
					bool blacket = false;
					char zen = this.GetZen(roman[i]);
					if(zen != Char.MinValue){
						pattern.Append("[").Append(zen);
						blacket = true;
					}
					pattern.Append(roman[i]);
					if(blacket){
						pattern.Append("]");
					}
				}
				pattern.Append("|");
				
				for(int i = 0; i < hira.Length; i++){
					bool blacket = false;
					char kata = this.GetKata(hira[i]);
					if(kata != Char.MinValue){
						pattern.Append("[").Append(kata);
						blacket = true;
						char han = this.GetHan(kata);
						if(han != Char.MinValue){
							pattern.Append(han);
						}
					}
					pattern.Append(hira[i]);
					if(blacket){
						pattern.Append("]");
					}
				}
				
				return pattern.ToString();
			}
			return "";
		}
		
		private IEnumerable<string> Tokenize(string str){
			int s = 0;
			int i = 1;
			for(; i < str.Length; i++){
				char c = str[i];
				if(('A' <= c) && (c <= 'Z')){
					yield return str.Substring(s, i -  s);
					s = i;
				}
			}
			yield return str.Substring(s, i - s);
		}
		
		private string GetPattern(PrefixTreeNode<object> node){
			List<string> subPatterns = new List<string>();
			List<string> chars = new List<string>();
			foreach(PrefixTreeNode<object> child in node.Children){
				if(child.IsAcceptState && (child.Children.Count == 0)){
					char han;
					if(this.zenToHanDictionary.TryGetValue(child.Char, out han)){
						chars.Add(Regex.Escape(String.Concat(child.Char.ToString(), han.ToString())));
					}else{
						chars.Add(Regex.Escape(child.Char.ToString()));
					}
				}else{
					subPatterns.Add(this.GetPattern(child));
				}
			}
			if(node.Parent == null){
				if(chars.Count > 1){
					if(subPatterns.Count > 0){
						return String.Concat(new string[]{"[", String.Join("", chars.ToArray()), "]|", String.Join("|", subPatterns.ToArray())});
					}else{
						return String.Concat(new string[]{"[", String.Join("", chars.ToArray()), "]"});
					}
				}else if(chars.Count == 1){
					if(subPatterns.Count > 0){
						return String.Concat(new string[]{chars[0].ToString(), "|", String.Join("|", subPatterns.ToArray())});
					}else{
						return String.Concat(new string[]{chars[0].ToString(), "]"});
					}
				}else{
					if(subPatterns.Count > 0){
						return String.Join("|", subPatterns.ToArray());
					}else{
						return "";
					}
				}
			}else{
				char han;
				string nodeChars;
				if(this.zenToHanDictionary.TryGetValue(node.Char, out han)){
					nodeChars = String.Concat(new string[]{"[", Regex.Escape(String.Concat(node.Char.ToString(), han.ToString())), "]"});
				}else{
					nodeChars = node.Char.ToString();
				}
				if(chars.Count > 1){
					if(subPatterns.Count > 1){
						return String.Concat(new string[]{nodeChars, "([", String.Join("", chars.ToArray()), "]|", String.Join("|", subPatterns.ToArray()), ")"});
					}else{
						return String.Concat(new string[]{nodeChars, "[", String.Join("", chars.ToArray()), "]"});
					}
				}else if(chars.Count == 1){
					string charsStr = (chars[0].Length == 1) ? chars[0] : "[" + chars[0] + "]";
					if(subPatterns.Count > 1){
						return String.Concat(new string[]{nodeChars, "(", charsStr, "|", String.Join("|", subPatterns.ToArray()), ")"});
					}else{
						return String.Concat(new string[]{nodeChars, charsStr});
					}
				}else{
					if(subPatterns.Count > 1){
						return String.Concat(new string[]{nodeChars, "(", String.Join("|", subPatterns.ToArray()), ")"});
					}else if(subPatterns.Count == 1){
						return String.Concat(new string[]{nodeChars, subPatterns[0]});
					}else{
						return node.Char.ToString();
					}
				}
			}
		}
		
		public char GetKata(char hira){
			char kata;
			if(this.hiraToKataDictionary.TryGetValue(hira, out kata)){
				return kata;
			}else{
				return Char.MinValue;
			}
		}
		
		public char GetHan(char zen){
			char han;
			if(this.zenToHanDictionary.TryGetValue(zen, out han)){
				return han;
			}else{
				return Char.MinValue;
			}
		}
		
		public char GetZen(char han){
			char zen;
			if(this.hanToZenDictionary.TryGetValue(han, out zen)){
				return zen;
			}else{
				return Char.MinValue;
			}
		}
		
		
		private class CharComparerIgnoreCase : IComparer<char>, IEqualityComparer<char>{
			public int Compare(char x, char y){
				const int toSmall = ('a' - 'A');
				bool xIsLarge = ('A' <= x) && (x <= 'Z');
				bool yIsLarge = ('A' <= y) && (y <= 'Z');
				
				if(xIsLarge && !(yIsLarge)){
					int sx = x + toSmall;
					// x‚ª‘å•¶Žš‚Ì‚Æ‚«
					if(sx > y){
						return 1;
					}else if(sx < y){
						return -1;
					}else{
						return 0;
					}
				}else if(!(xIsLarge) && yIsLarge){
					int sy = y + toSmall;
					// y‚ª‘å•¶Žš‚ÌŽž
					if(x > sy){
						return 1;
					}else if(x < sy){
						return -1;
					}else{
						return 0;
					}
				}else{
					if(x > y){
						return 1;
					}else if(x < y){
						return -1;
					}else{
						return 0;
					}
				}
			}
			public bool Equals(char x, char y){
				return (this.Compare(x, y) == 0);
			}
			
			public int GetHashCode(char x){
				return x.GetHashCode();
			}
		}
	}
	*/
}