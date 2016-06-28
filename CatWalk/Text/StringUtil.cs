using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Globalization;

namespace CatWalk.Text{
	public static class StringUtil {
		#region Sequential Number

		public static string[] ExpandSequentialNumbers(this string src){
			List<string> retStrs = new List<string>();
			string[] tmpStrArray;
			Regex rexBracket = new Regex(@"(?<!\\)\[.*?(?<!\\)\]");
			Regex rexNumbers = new Regex(@"^(\d+)-(\d+)");
			Regex rexHexNumbers = new Regex(@"^0x([0-9a-f]+)-0x([0-9a-f]+)", RegexOptions.IgnoreCase);
			Regex rexHexNumbersUpper = new Regex(@"^0x([0-9A-F]+)-0x([0-9A-F]+)", RegexOptions.IgnoreCase);
			Regex rexChars = new Regex(@"^(.)-(.)");
			Match match;
			retStrs.Add(src);
			while(rexBracket.IsMatch(retStrs[0])){
				tmpStrArray = retStrs.ToArray();
				retStrs.Clear();
				foreach(string srci in tmpStrArray){
					match = rexBracket.Match(srci);
					if(match != Match.Empty){
						string left = srci.Substring(0, match.Index);
						string middle = srci.Substring(match.Index + 1, match.Length - 2);
						string right = srci.Substring(match.Index + match.Length);
						string source = middle;
						while(!(String.IsNullOrEmpty(source))){
							// dec-dec
							match = rexNumbers.Match(source);
							if(match != Match.Empty){
								source = source.Substring(match.Index + match.Length);
								string strStart = match.Groups[1].Value;
								string strEnd = match.Groups[2].Value;
								int numLength = strStart.Length;
								int start = Int32.Parse(strStart);
								int end = Int32.Parse(strEnd);
								if(start > end){
									retStrs.Add(left + source + right);
								}else{
									while(start <= end){
										retStrs.Add(left + start.ToString().PadLeft(numLength, '0') + right);
										start++;
									}
								}
								continue;
							}
							
							// hex-hex
							bool isUpper = false;
							match = rexHexNumbersUpper.Match(source);
							if(match != Match.Empty){
								isUpper = true;
							}else{
								match = rexHexNumbers.Match(source);
							}
							if(match != Match.Empty){
								source = source.Substring(match.Index + match.Length);
								string strStart = match.Groups[1].Value;
								string strEnd = match.Groups[2].Value;
								int numLength = strStart.Length;
								int start = Convert.ToInt32(strStart, 16);
								int end = Convert.ToInt32(strEnd, 16);
								if(start > end){
									retStrs.Add(left + source + right);
								}else{
									while(start <= end){
										string hexNumber;
										if(isUpper){
											hexNumber = Convert.ToString(start, 16).ToUpper();
										}else{
											hexNumber = Convert.ToString(start, 16).ToLower();
										}
										retStrs.Add(left + hexNumber.PadLeft(numLength, '0') + right);
										start++;
									}
								}
								continue;
							}
							
							// char-char
							match = rexChars.Match(source);
							if(match != Match.Empty){
								source = source.Substring(match.Index + match.Length);
								char charStart = match.Groups[1].Value[0];
								char charEnd = match.Groups[2].Value[0];
								if(charStart > charEnd){
									retStrs.Add(left + source + right);
								}else{
									while(charStart <= charEnd){
										retStrs.Add(left + charStart + right);
										charStart++;
									}
								}
								continue;
							}
							
							foreach(string strj in source.Split(new char[]{','})){
								source = null;
								retStrs.Add(left + strj + right);
							}
						} // while : sourceがnullか空でない
					} // if : 括弧が含まれる
				} // foreach : 置換文字列分
			} // while : 括弧がある
			for(int i = 0; i < retStrs.Count; i++){
				retStrs[i] = retStrs[i].Replace("\\[", "[").Replace("\\]", "]");
			}
			return retStrs.ToArray();
		}

		#endregion

		#region FileSize

		private const long    KBThreathold = 1000;
		private const long    MBThreathold = 1000 * 1000;
		private const long    GBThreathold = 1000 * 1000 * 1000;
		private const long    TBThreathold = Int64.MaxValue;
		private const double  KiBDouble    = 1024d;
		private const double  MiBDouble    = 1024d * 1024;
		private const double  GiBDouble    = 1024d * 1024 * 1024;
		private const double  KBDouble     = 1000d;
		private const double  MBDouble     = 1000d * 1000;
		private const double  GBDouble     = 1000d * 1000 * 1000;
		private const decimal KiBDecimal   = 1024m;
		private const decimal MiBDecimal   = 1024m * 1024;
		private const decimal GiBDecimal   = 1024m * 1024 * 1024;
		private const decimal TiBDecimal   = 1024m * 1024 * 1024 * 1024;
		private const decimal PiBDecimal   = 1024m * 1024 * 1024 * 1024 * 1024;
		private const decimal EiBDecimal   = 1024m * 1024 * 1024 * 1024 * 1024 * 1024;
		private const decimal ZiBDecimal   = 1024m * 1024 * 1024 * 1024 * 1024 * 1024 * 1024;
		private const decimal YiBDecimal   = 1024m * 1024 * 1024 * 1024 * 1024 * 1024 * 1024 * 1024;
		private const decimal KBDecimal    = 1000m;
		private const decimal MBDecimal    = 1000m * 1000;
		private const decimal GBDecimal    = 1000m * 1000 * 1000;
		private const decimal TBDecimal    = 1000m * 1000 * 1000 * 1000;
		private const decimal PBDecimal    = 1000m * 1000 * 1000 * 1000 * 1000;
		private const decimal EBDecimal    = 1000m * 1000 * 1000 * 1000 * 1000 * 1000;
		private const decimal ZBDecimal    = 1000m * 1000 * 1000 * 1000 * 1000 * 1000 * 1000;
		private const decimal YBDecimal    = 1000m * 1000 * 1000 * 1000 * 1000 * 1000 * 1000 * 1000;

		public static string GetFileSizeString(decimal size){
			return GetFileSizeString(size, FileSizeScale.Auto);
		}
		
		public static string GetFileSizeString(decimal size, FileSizeScale scale){
			if(scale == FileSizeScale.Auto){
				if(size < KBDecimal){
					scale = FileSizeScale.Bytes;
				}else if(size < MBDecimal){
					scale = FileSizeScale.KB;
				}else if(size < GBDecimal){
					scale = FileSizeScale.MB;
				}else if(size < TBDecimal){
					scale = FileSizeScale.GB;
				}else if(size < PBDecimal){
					scale = FileSizeScale.TB;
				}else if(size < EBDecimal){
					scale = FileSizeScale.PB;
				}else if(size < ZBDecimal){
					scale = FileSizeScale.EB;
				}else if(size < YBDecimal){
					scale = FileSizeScale.ZB;
				}else{
					scale = FileSizeScale.YB;
				}
			}else if(scale == FileSizeScale.AutoBinary){
				if(scale == FileSizeScale.AutoBinary){
					if(size < KBDecimal){
						scale = FileSizeScale.Bytes;
					}else if(size < MBDecimal){
						scale = FileSizeScale.KiB;
					}else if(size < GBDecimal){
						scale = FileSizeScale.MiB;
					}else if(size < TBDecimal){
						scale = FileSizeScale.GiB;
					}else if(size < PBDecimal){
						scale = FileSizeScale.TiB;
					}else if(size < EBDecimal){
						scale = FileSizeScale.PiB;
					}else if(size < ZBDecimal){
						scale = FileSizeScale.EiB;
					}else if(size < YBDecimal){
						scale = FileSizeScale.ZiB;
					}else{
						scale = FileSizeScale.YiB;
					}
				}
			}
			switch(scale){
				case FileSizeScale.Bytes:
					return size.ToString("N") + " B";
				case FileSizeScale.KB:
					return (size / KBDecimal).ToString("N") + " KB";
				case FileSizeScale.MB:
					return (size / MBDecimal).ToString("N") + " MB";
				case FileSizeScale.GB:
					return (size / GBDecimal).ToString("N") + " GB";
				case FileSizeScale.TB:
					return (size / TBDecimal).ToString("N") + " TB";
				case FileSizeScale.PB:
					return (size / PBDecimal).ToString("N") + " TB";
				case FileSizeScale.EB:
					return (size / EBDecimal).ToString("N") + " EB";
				case FileSizeScale.ZB:
					return (size / ZBDecimal).ToString("N") + " ZB";
				case FileSizeScale.YB:
					return (size / YBDecimal).ToString("N") + " YB";
				case FileSizeScale.KiB:
					return (size / KiBDecimal).ToString("N") + " KB";
				case FileSizeScale.MiB:
					return (size / MiBDecimal).ToString("N") + " MB";
				case FileSizeScale.GiB:
					return (size / GiBDecimal).ToString("N") + " GB";
				case FileSizeScale.TiB:
					return (size / TiBDecimal).ToString("N") + " TB";
				case FileSizeScale.PiB:
					return (size / PiBDecimal).ToString("N") + " TB";
				case FileSizeScale.EiB:
					return (size / EiBDecimal).ToString("N") + " EB";
				case FileSizeScale.ZiB:
					return (size / ZiBDecimal).ToString("N") + " ZB";
				case FileSizeScale.YiB:
					return (size / YiBDecimal).ToString("N") + " YB";
				default:
					throw new ArgumentException();
			}
		}

		#endregion

		#region ParseCommandline

		[DllImport("shell32.dll", EntryPoint = "CommandLineToArgvW", CharSet = CharSet.Unicode)]
		internal static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string commmandLine, out int argCount);

		public static string[] ParseCommandLineString(this string commmandLineString){
			IntPtr lparray = IntPtr.Zero;
			try{
				int count;
				lparray = CommandLineToArgvW(commmandLineString, out count);
				if(lparray != IntPtr.Zero){
					string[] args = new string[count];
					int offset = 0;
					for(int i = 0; i < count; i++){
						IntPtr lpstr = Marshal.ReadIntPtr(lparray, offset);
						args[i] = Marshal.PtrToStringUni(lpstr);
						offset += Marshal.SizeOf(lpstr);
					}
					return args;
				}
			}finally{
				if(lparray != IntPtr.Zero){
					Marshal.FreeHGlobal(lparray);
				}
			}
			return new string[0];
		}

		#endregion

		#region EditDistance

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

		#endregion

		#region Multi bytes Width

		#region FitTextWidth

		public static string FitTextWidth(this string str, int width){
			if(str == null){
				throw new ArgumentNullException("str");
			}
			if(width == 0){
				return String.Empty;
			}
			if(width < 0){
				throw new ArgumentOutOfRangeException("width");
			}
			int wid;
			str = WidthSubstringInternal(str, 0, width, CultureInfo.CurrentUICulture, out wid);
			if(wid < width){
				return str + (new String(' ', width - wid));
			}else{
				return str;
			}
		}

		#endregion

		#region Substring
		public static string WidthSubstring(this string str, int startPos){
			return WidthSubstring(str, startPos, CultureInfo.CurrentUICulture);
		}
		public static string WidthSubstring(this string str, int startPos, CultureInfo culture){
			if(str == null){
				throw new ArgumentNullException("str");
			}
			if(culture == null){
				throw new ArgumentNullException("culture");
			}
			var lengthOfAmb = GetLengthOfAmbiguous(culture);
			var outWidth = 0;
			var pos = 0;
			var clen = 1;
			UnicodeWidthClass cls;
			char c;
			char high = '\0';
			var pc = 0;
			while(pc < str.Length){
				c = str[pc];
				if(0xD800 <= c && c <=0xDBFF){
					if(high != '\0'){
						cls = UnicodeWidthClass.Nutral;
						high = c;
					}else{
						high = c;
						goto inc;
					}
				}else{
					if(high != '\0'){
						cls = GetWidthClass(Char.ConvertToUtf32(high, c));
						high = '\0';
					}else{
						cls = GetWidthClass(c);
					}
				}

				switch(cls){
					case UnicodeWidthClass.Nutral:
					case UnicodeWidthClass.Half:
					case UnicodeWidthClass.Narrow:
						outWidth++; break;
					case UnicodeWidthClass.Wide:
					case UnicodeWidthClass.Full:
						outWidth += 2; break;
					case UnicodeWidthClass.Ambiguous:
						outWidth += 1; break;
				}
				if(outWidth > startPos){
					return str.Substring(pos);
				}
			inc:
				pos += clen;
				pc++;
			} // end while
			return String.Empty;
		}
		public static string WidthSubstring(this string str, int startPos, int width){
			return WidthSubstring(str, startPos, width, CultureInfo.CurrentUICulture);
		}
		public static string WidthSubstring(this string str, int startPos, int width, out int outwidth){
			return WidthSubstringInternal(str, startPos, width, CultureInfo.CurrentUICulture, out outwidth);
		}
		public static string WidthSubstring(this string str, int startPos, int width, CultureInfo culture){
			int len;
			return WidthSubstringInternal(str, startPos, width, culture, out len);
		}
		public static string WidthSubstring(this string str, int startPos, int width, CultureInfo culture, out int outwidth){
			return WidthSubstringInternal(str, startPos, width, culture, out outwidth);
		}
		private static string WidthSubstringInternal(string str, int startPos, int width, CultureInfo culture, out int outWidth){
			if(str == null){
				throw new ArgumentNullException("str");
			}
			if(culture == null){
				throw new ArgumentNullException("culture");
			}
			if(startPos < 0 || str.Length <= startPos){
				throw new ArgumentOutOfRangeException("startPos");
			}
			var lengthOfAmbiguous = GetLengthOfAmbiguous(culture);
			var endPos = startPos + width;
			outWidth = 0;
			var pos = 0;
			var start = 0;
			var clen = 1;
			var started = false;
			UnicodeWidthClass cls;
			char c;
			char high = '\0';
			var pc = 0;
			while(pc < str.Length){
				c = str[pc];
				if(0xD800 <= c && c <=0xDBFF){
					if(high != '\0'){
						cls = UnicodeWidthClass.Nutral;
						high = c;
					}else{
						high = c;
						goto inc;
					}
				}else{
					if(high != '\0'){
						cls = GetWidthClass(Char.ConvertToUtf32(high, c));
						high = '\0';
					}else{
						cls = GetWidthClass(c);
					}
				}

				switch(cls){
					case UnicodeWidthClass.Nutral:
					case UnicodeWidthClass.Half:
					case UnicodeWidthClass.Narrow:
						outWidth++; break;
					case UnicodeWidthClass.Wide:
					case UnicodeWidthClass.Full:
						outWidth += 2; break;
					case UnicodeWidthClass.Ambiguous:
						outWidth += lengthOfAmbiguous; break;
				}
				if(started){
					if(outWidth == endPos){
						return str.Substring(start, pos + clen - start);
					}else if(outWidth > endPos){
						return str.Substring(start, pos - start);
					}
				}else if(outWidth >= startPos){
					start = pos;
					started = true;
				}
			inc:
				pos += clen;
				pc++;
			} // end for
			if(started){
				return str.Substring(start);
			}else{
				return str;
			}
		}
		#endregion

		#region IsZenkaku / IsHankaku

		public static bool IsZenkaku(this char c){
			return !c.IsHankaku();
		}

		public static bool IsHankaku(this char c){
			switch(GetWidthClass(c)){
				case UnicodeWidthClass.Nutral:
				case UnicodeWidthClass.Half:
				case UnicodeWidthClass.Narrow:
					return true;
				default:
					return false;
			}
		}

		#endregion

		#region GetWidth

		public static int GetWidth(this string str){
			return GetWidth(str, CultureInfo.CurrentUICulture);
		}
		public static int GetWidth(this string str, CultureInfo culture){
			if(str == null){
				throw new ArgumentNullException("str");
			}
			int lengthOfAmbiguous = GetLengthOfAmbiguous(culture);
			int length = 0;
			UnicodeWidthClass cls;
			char c;
			char high = '\0';
			var pc = 0;
			while(pc < str.Length){
				c = str[pc];

				// サロゲートペア
				if(0xD800 <= c && c <=0xDBFF){
					if(high != '\0'){
						cls = UnicodeWidthClass.Nutral;
						high = c;
					}else{
						high = c;
						goto inc;
					}
				}else{
					if(high != '\0'){
						cls = GetWidthClass(Char.ConvertToUtf32(high, c));
						high = '\0';
					}else{
						cls = GetWidthClass(c);
					}
				}

				switch(cls){
					case UnicodeWidthClass.Nutral:
					case UnicodeWidthClass.Half:
					case UnicodeWidthClass.Narrow:
						length++; break;
					case UnicodeWidthClass.Wide:
					case UnicodeWidthClass.Full:
						length += 2; break;
					case UnicodeWidthClass.Ambiguous:
						length += lengthOfAmbiguous; break;
				}
			inc:
				pc++;
			}
			return length;
		}

		public static IEnumerable<UnicodeWidthClass> GetWidthClasses(this string str){
			if(str == null){
				throw new ArgumentNullException("str");
			}
			return GetWidthClassesInternal(str);
		}

		private static IEnumerable<UnicodeWidthClass> GetWidthClassesInternal(string str){
			char high = '\0';
			foreach(var c in str){
				if(0xD800 <= c && c <=0xDBFF){
					if(high != '\0'){
						yield return UnicodeWidthClass.Nutral;
					}
					high = c;
				}else{
					if(high != '\0'){
						yield return GetWidthClass(Char.ConvertToUtf32(high, c));
						high = '\0';
					}else{
						yield return GetWidthClass(c);
					}
				}
			}
		}

		#endregion

		#region GetWidthClass

		public static UnicodeWidthClass GetWidthClass(int c){
			if((0x3000<=c&&c<=0x3001)||(0xFF01<=c&&c<=0xFF61)||(0xFFE0<=c&&c<=0xFFE8)){
				return UnicodeWidthClass.Full;
			}else
			if((0x20A9<=c&&c<=0x20AA)||(0xFF61<=c&&c<=0xFFE0)||(0xFFE8<=c&&c<=0xFFF9)){
				return UnicodeWidthClass.Half;
			}else
			if((0x0020<=c&&c<=0x007F)||(0x00A2<=c&&c<=0x00A4)||(0x00A5<=c&&c<=0x00A7)||(0x00AC<=c&&c<=0x00AD)||
			   (0x00AF<=c&&c<=0x00B0)||(0x27E6<=c&&c<=0x27EE)||(0x2985<=c&&c<=0x2987)){
				return UnicodeWidthClass.Narrow;
			}else
			if((0x1100<=c&&c<=0x1160)||(0x11A3<=c&&c<=0x11A8)||(0x11FA<=c&&c<=0x1200)||(0x2329<=c&&c<=0x232B)||
			   (0x2E80<=c&&c<=0x3000)||(0x3001<=c&&c<=0x303F)||(0x3041<=c&&c<=0x3248)||(0x3250<=c&&c<=0x4DC0)||
			   (0x4E00<=c&&c<=0xA4D0)||(0xA960<=c&&c<=0xA980)||(0xAC00<=c&&c<=0xDB7F)||(0xF900<=c&&c<=0xFB00)||
			   (0xFE10<=c&&c<=0xFE20)||(0xFE30<=c&&c<=0xFE70)||(0x1B000<=c&&c<=0x1D000)||(0x1F200<=c&&c<=0x1F300)||
			   (0x20000<=c&&c<=0xE0001)){return UnicodeWidthClass.Wide;
			}else
			if((0x00A1<=c&&c<=0x00A2)||(0x00A4<=c&&c<=0x00A5)||(0x00A7<=c&&c<=0x00A9)||(0x00AA<=c&&c<=0x00AB)||
			   (0x00AD<=c&&c<=0x00AF)||(0x00B0<=c&&c<=0x00B5)||(0x00B6<=c&&c<=0x00BB)||(0x00BC<=c&&c<=0x00C0)||
			   (0x00C6<=c&&c<=0x00C7)||(0x00D0<=c&&c<=0x00D1)||(0x00D7<=c&&c<=0x00D9)||(0x00DE<=c&&c<=0x00E2)||
			   (0x00E6<=c&&c<=0x00E7)||(0x00E8<=c&&c<=0x00EB)||(0x00EC<=c&&c<=0x00EE)||(0x00F0<=c&&c<=0x00F1)||
			   (0x00F2<=c&&c<=0x00F4)||(0x00F7<=c&&c<=0x00FB)||(0x00FC<=c&&c<=0x00FD)||(0x00FE<=c&&c<=0x00FF)||
			   (0x0101<=c&&c<=0x0102)||(0x0111<=c&&c<=0x0112)||(0x0113<=c&&c<=0x0114)||(0x011B<=c&&c<=0x011C)||
			   (0x0126<=c&&c<=0x0128)||(0x012B<=c&&c<=0x012C)||(0x0131<=c&&c<=0x0134)||(0x0138<=c&&c<=0x0139)||
			   (0x013F<=c&&c<=0x0143)||(0x0144<=c&&c<=0x0145)||(0x0148<=c&&c<=0x014C)||(0x014D<=c&&c<=0x014E)||
			   (0x0152<=c&&c<=0x0154)||(0x0166<=c&&c<=0x0168)||(0x016B<=c&&c<=0x016C)||(0x01CE<=c&&c<=0x01CF)||
			   (0x01D0<=c&&c<=0x01D1)||(0x01D2<=c&&c<=0x01D3)||(0x01D4<=c&&c<=0x01D5)||(0x01D6<=c&&c<=0x01D7)||
			   (0x01D8<=c&&c<=0x01D9)||(0x01DA<=c&&c<=0x01DB)||(0x01DC<=c&&c<=0x01DD)||(0x0251<=c&&c<=0x0252)||
			   (0x0261<=c&&c<=0x0262)||(0x02C4<=c&&c<=0x02C5)||(0x02C7<=c&&c<=0x02C8)||(0x02C9<=c&&c<=0x02CC)||
			   (0x02CD<=c&&c<=0x02CE)||(0x02D0<=c&&c<=0x02D1)||(0x02D8<=c&&c<=0x02DC)||(0x02DD<=c&&c<=0x02DE)||
			   (0x02DF<=c&&c<=0x02E0)||(0x0300<=c&&c<=0x0370)||(0x0391<=c&&c<=0x03AA)||(0x03B1<=c&&c<=0x03C2)||
			   (0x03C3<=c&&c<=0x03CA)||(0x0401<=c&&c<=0x0402)||(0x0410<=c&&c<=0x0450)||(0x0451<=c&&c<=0x0452)||
			   (0x2010<=c&&c<=0x2011)||(0x2013<=c&&c<=0x2017)||(0x2018<=c&&c<=0x201A)||(0x201C<=c&&c<=0x201E)||
			   (0x2020<=c&&c<=0x2023)||(0x2024<=c&&c<=0x2028)||(0x2030<=c&&c<=0x2031)||(0x2032<=c&&c<=0x2034)||
			   (0x2035<=c&&c<=0x2036)||(0x203B<=c&&c<=0x203C)||(0x203E<=c&&c<=0x203F)||(0x2074<=c&&c<=0x2075)||
			   (0x207F<=c&&c<=0x2080)||(0x2081<=c&&c<=0x2085)||(0x20AC<=c&&c<=0x20AD)||(0x2103<=c&&c<=0x2104)||
			   (0x2105<=c&&c<=0x2106)||(0x2109<=c&&c<=0x210A)||(0x2113<=c&&c<=0x2114)||(0x2116<=c&&c<=0x2117)||
			   (0x2121<=c&&c<=0x2123)||(0x2126<=c&&c<=0x2127)||(0x212B<=c&&c<=0x212C)||(0x2153<=c&&c<=0x2155)||
			   (0x215B<=c&&c<=0x215F)||(0x2160<=c&&c<=0x216C)||(0x2170<=c&&c<=0x217A)||(0x2189<=c&&c<=0x219A)||
			   (0x21B8<=c&&c<=0x21BA)||(0x21D2<=c&&c<=0x21D3)||(0x21D4<=c&&c<=0x21D5)||(0x21E7<=c&&c<=0x21E8)||
			   (0x2200<=c&&c<=0x2201)||(0x2202<=c&&c<=0x2204)||(0x2207<=c&&c<=0x2209)||(0x220B<=c&&c<=0x220C)||
			   (0x220F<=c&&c<=0x2210)||(0x2211<=c&&c<=0x2212)||(0x2215<=c&&c<=0x2216)||(0x221A<=c&&c<=0x221B)||
			   (0x221D<=c&&c<=0x2221)||(0x2223<=c&&c<=0x2224)||(0x2225<=c&&c<=0x2226)||(0x2227<=c&&c<=0x222D)||
			   (0x222E<=c&&c<=0x222F)||(0x2234<=c&&c<=0x2238)||(0x223C<=c&&c<=0x223E)||(0x2248<=c&&c<=0x2249)||
			   (0x224C<=c&&c<=0x224D)||(0x2252<=c&&c<=0x2253)||(0x2260<=c&&c<=0x2262)||(0x2264<=c&&c<=0x2268)||
			   (0x226A<=c&&c<=0x226C)||(0x226E<=c&&c<=0x2270)||(0x2282<=c&&c<=0x2284)||(0x2286<=c&&c<=0x2288)||
			   (0x2295<=c&&c<=0x2296)||(0x2299<=c&&c<=0x229A)||(0x22A5<=c&&c<=0x22A6)||(0x22BF<=c&&c<=0x22C0)||
			   (0x2312<=c&&c<=0x2313)||(0x2460<=c&&c<=0x24EA)||(0x24EB<=c&&c<=0x254C)||(0x2550<=c&&c<=0x2574)||
			   (0x2580<=c&&c<=0x2590)||(0x2592<=c&&c<=0x2596)||(0x25A0<=c&&c<=0x25A2)||(0x25A3<=c&&c<=0x25AA)||
			   (0x25B2<=c&&c<=0x25B4)||(0x25B6<=c&&c<=0x25B8)||(0x25BC<=c&&c<=0x25BE)||(0x25C0<=c&&c<=0x25C2)||
			   (0x25C6<=c&&c<=0x25C9)||(0x25CB<=c&&c<=0x25CC)||(0x25CE<=c&&c<=0x25D2)||(0x25E2<=c&&c<=0x25E6)||
			   (0x25EF<=c&&c<=0x25F0)||(0x2605<=c&&c<=0x2607)||(0x2609<=c&&c<=0x260A)||(0x260E<=c&&c<=0x2610)||
			   (0x2614<=c&&c<=0x2616)||(0x261C<=c&&c<=0x261D)||(0x261E<=c&&c<=0x261F)||(0x2640<=c&&c<=0x2641)||
			   (0x2642<=c&&c<=0x2643)||(0x2660<=c&&c<=0x2662)||(0x2663<=c&&c<=0x2666)||(0x2667<=c&&c<=0x266B)||
			   (0x266C<=c&&c<=0x266E)||(0x266F<=c&&c<=0x2670)||(0x269E<=c&&c<=0x26A0)||(0x26BE<=c&&c<=0x26C0)||
			   (0x26C4<=c&&c<=0x26CE)||(0x26CF<=c&&c<=0x26E2)||(0x26E3<=c&&c<=0x26E4)||(0x26E8<=c&&c<=0x2701)||
			   (0x273D<=c&&c<=0x273E)||(0x2757<=c&&c<=0x2758)||(0x2776<=c&&c<=0x2780)||(0x2B55<=c&&c<=0x2C00)||
			   (0x3248<=c&&c<=0x3250)||(0xE000<=c&&c<=0xF900)||(0xFE00<=c&&c<=0xFE10)||(0xFFFD<=c&&c<=0x10000)||
			   (0x1F100<=c&&c<=0x1F12E)||(0x1F130<=c&&c<=0x1F1E6)||(0xE0100<=c&&c<=0x10FFFD)){
				return UnicodeWidthClass.Ambiguous;
			}else{
				return UnicodeWidthClass.Nutral;
			}
		}

		#endregion

		private static int GetLengthOfAmbiguous(CultureInfo culture){
			if(culture.TwoLetterISOLanguageName == "ja" || culture.TwoLetterISOLanguageName == "zh" || culture.TwoLetterISOLanguageName == "vi"){
				return 2;
			}else{
				return 1;
			}
		}

		#endregion

		#region Encrypt
		/*
		private static readonly byte[] optionalEntropy = new byte[]{0xff, 0x4f, 0xef, 0x54, 0x01};

		public static string Protect(this string plain){
			if(plain == null){
				return "";
			}else{
				var data = Encoding.UTF8.GetBytes(plain);
				try{
					return Convert.ToBase64String(ProtectedData.Protect(data, optionalEntropy, DataProtectionScope.CurrentUser));
				}catch{
					return plain;
				}
			}
		}
		
		public static string Unprotect(this string encrypted){
			if(encrypted == null){
				return "";
			}else{
				var data = Convert.FromBase64String(encrypted);
				try{
					return Encoding.UTF8.GetString(ProtectedData.Unprotect(data, optionalEntropy, DataProtectionScope.CurrentUser));
				}catch{
					return encrypted;
				}
			}
		}
		*/
		#endregion
	}

	public enum FileSizeScale{
		Auto,
		AutoBinary,
		Bytes,
		KB,
		KiB,
		MB,
		MiB,
		GB,
		GiB,
		TB,
		TiB,
		PB,
		PiB,
		EB,
		EiB,
		ZB,
		ZiB,
		YB,
		YiB
	}

	public enum UnicodeWidthClass{
		Nutral = 0,
		Narrow = 1,
		Half = 2,
		Wide = 3,
		Full = 4,
		Ambiguous = 5,
	}
}
