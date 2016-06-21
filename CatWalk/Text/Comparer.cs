/*
	$Id: Comparer.cs 239 2011-06-28 09:41:29Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CatWalk.Text{
public class LogicalStringComparer : StringComparer{
	public StringComparer StringComparer{get; set;}
	
	public LogicalStringComparer(){
		this.StringComparer = StringComparer.Ordinal;
	}
	
	public LogicalStringComparer(StringComparer comparer){
		comparer.ThrowIfNull("comparer");
		this.StringComparer = comparer;
	}
	
	public override int Compare(string x, string y){
		// 片方、もしくは両方がnull、もしくは空のとき
		var xIsNullOrEmpty = String.IsNullOrEmpty(x);
		var yIsNullOrEmpty = String.IsNullOrEmpty(y);
		if(xIsNullOrEmpty && yIsNullOrEmpty){
			return 0;
		}else if(xIsNullOrEmpty){
			return -1;
		}else if(yIsNullOrEmpty){
			return 1;
		}

		using(IEnumerator<string> xs = Split(x))
		using(IEnumerator<string> ys = Split(y)){
			while(xs.MoveNext() && ys.MoveNext()){
				string xe = xs.Current;
				string ye = ys.Current;
				// 数値同士の比較
				if(IsNumber(xe[0]) && IsNumber(ye[0])){
					// とりあえず長さで比較
					if(xe.Length > ye.Length){
						return 1;
					}else if(xe.Length < ye.Length){
						return -1;
					}
					// 同じ長さの場合は普通に比較すればよい。
					int d = xe.CompareTo(ye);
					if(d != 0){
						return d;
					}
				}else{ // 数値同士でないときは普通に比較。
					int d = this.StringComparer.Compare(xe, ye);
					if(d != 0){
						return d;
					}
				}
			}
		}
		// 片方のシーケンスがオワタ時は長さで比較
		return x.Length.CompareTo(y.Length);
	}
	
	public override bool Equals(string x, string y){
		return x.Equals(y);
	}
	
	public override int GetHashCode(string str){
		return this.StringComparer.GetHashCode(str);
	}
	
	/// <summary>
	/// 数値と文字列の部分を分割して交互に返す。
	/// </summary>
	private static IEnumerator<string> Split(string str){
		if(str == null){
			yield break;
		}
		if(str.Length == 0){
			yield break;
		}
	
		bool lastIsNum = IsNumber(str[0]);
		int left = 0;
		for(int i = 1; i < str.Length; i++){
			bool isNum = IsNumber(str[i]);
			if(isNum != lastIsNum){
				yield return str.Substring(left, i - left);
				left = i;
				lastIsNum = isNum;
			}
		}
		yield return str.Substring(left);
	}

	private static bool IsNumber(char c){
		return ('0' <= c) && (c <= '9');
	}
}
	
	public class UnsafeLogicalStringComparer : StringComparer{
		private static UnsafeLogicalStringComparer comparer = null;
		public static UnsafeLogicalStringComparer Comparer{
			get{
				if(comparer == null){
					comparer = new UnsafeLogicalStringComparer();
				}
				return comparer;
			}
		}
		
		public override int Compare(string x, string y){
			return CompareStatic(x, y);
		}
		
		private unsafe static int CompareStatic(string x, string y){
			fixed(char* fpx = x)
			fixed(char* fpy = y){
				char* px = fpx;
				char* py = fpy;
				int d;
				
				// 共通部分読み飛ばし
				if(x.Length > y.Length){
					while(CompareChar(px, py) == 0){
						if(*px == '\0'){
							break;
						}
						px++;
						py++;
					}
				}else{
					while(CompareChar(px, py) == 0){
						if (*py == '\0'){
							break;
						}
						px++;
						py++;
					}
				}
				
				char* pnx = px + 1;
				char* pny = py + 1;
				bool xIsDec = ('0' <= *px) && (*px <= '9');
				bool yIsDec = ('0' <= *py) && (*py <= '9');
				
				while(true){
					// チェック
					if(*px == '\0'){
						if(*py == '\0'){
							return 0;
						}else{
							return -1;
						}
					}else{
						if(*py == '\0'){
							return 1;
						}
					}
					
					// 次のトークンまでを調べる
					while(*pnx != '\0'){
						if(xIsDec){
							if(('0' > *pnx) || (*pnx > '9')){	// 数値でないとき
								break;
							}
						}else{
							if(('0' <= *pnx) && (*pnx <= '9')){	// 数値のとき
								break;
							}
						}
						pnx++;
					}
					
					while(*pny != '\0'){
						if(yIsDec){
							if(('0' > *pny) || (*pny > '9')){	// 数値でないとき
								break;
							}
						}else{
							if(('0' <= *pny) && (*pny <= '9')){	// 数値のとき
								break;
							}
						}
						pny++;
					}
					
					// 数値比較
					long lenx = pnx - px;
					long leny = pny - py;
					if(xIsDec && yIsDec){
						if(lenx > leny){
							return 1;
						}else if(lenx < leny){
							return -1;
						}else{
							for(; px != pnx; px++, py++){
								if(*px > *py){
									return 1;
								}else if(*px < *py){
									return -1;
								}
							}
						}
					}else{	// 文字列比較
						if(lenx < leny){
							for(; px != pnx; px++, py++){
								d = CompareChar(px, py);
								if(d != 0){
									return d;
								}
							}
						}else{
							for(; py != pny; px++, py++){
								d = CompareChar(px, py);
								if(d != 0){
									return d;
								}
							}
						}
					}
					
					xIsDec = !xIsDec;
					yIsDec = !yIsDec;
				}
			}
		}
		
		public unsafe static int CompareChar(char* px, char* py){
			const int toSmall = ('a' - 'A');
			bool xIsLarge = ('A' <= *px) && (*px <= 'Z');
			bool yIsLarge = ('A' <= *py) && (*py <= 'Z');
			
			if(xIsLarge && !(yIsLarge)){
				int sx = (*px) + toSmall;
				// xが大文字のとき
				if(sx > *py){
					return 1;
				}else if(sx < *py){
					return -1;
				}else{
					return 0;
				}
			}else if(!(xIsLarge) && yIsLarge){
				int sy = (*py) + toSmall;
				// yが大文字の時
				if(*px > sy){
					return 1;
				}else if(*px < sy){
					return -1;
				}else{
					return 0;
				}
			}else{
				if(*px > *py){
					return 1;
				}else if(*px < *py){
					return -1;
				}else{
					return 0;
				}
			}
		}
		
		public override bool Equals(string x, string y){
			return x.Equals(y);
		}
		
		public override int GetHashCode(string str){
			return str.GetHashCode();
		}
	}
	/*
	public class ShellLogicalStringComparer : StringComparer{
		[DllImport("Shlwapi.dll", EntryPoint = "StrCmpLogicalW", CharSet = CharSet.Unicode)]
		public static extern int CompareStringLogical(string x, string y);
	
		private static ShellLogicalStringComparer comparer = null;
		public static ShellLogicalStringComparer Comparer{
			get{
				if(comparer == null){
					comparer = new ShellLogicalStringComparer();
				}
				return comparer;
			}
		}
		
		public override int Compare(string x, string y){
			return CompareStringLogical(x, y);
		}
		
		public override bool Equals(string x, string y){
			return x.Equals(y);
		}
		
		public override int GetHashCode(string str){
			return str.GetHashCode();
		}
	}
	 * */
}
