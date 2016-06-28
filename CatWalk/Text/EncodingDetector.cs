/*
	$Id: EncodingDetector.cs 293 2011-10-25 08:20:20Z catwalkagogo@gmail.com $
*/
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace CatWalk.Text{
	/*
	class Program{
		static void Main(){
			var result = new Dictionary<string, IList<string>>();
			foreach(var file in Directory.GetFiles(Environment.CurrentDirectory, "*.*")){
				try{
					Console.WriteLine(file);
					using(var stream = File.Open(file, FileMode.Open, FileAccess.Read)){
						foreach(var enc in EncodingDetector.GetEncodings(stream)){
							string key = enc.EncodingName;
							if(!result.ContainsKey(key)){
								result.Add(key, new List<string>());
							}
							result[key].Add(file);
						}
					}
					Console.Write("\n");
				}catch(IOException){
				}
			}
			
			// 結果
			foreach(var item in result){
				Console.WriteLine("{0}", item.Key);
				foreach(var file in item.Value){
					Console.WriteLine(file);
				}
				Console.Write("\n");
			}
		}
	}
	*/
	
	public abstract class EncodingDetector{
		internal abstract void Check(byte[] data);
		internal abstract Encoding Encoding{get;}
		
		/// <summary>
		/// 文字列のEncodingを判定する。
		/// </summary>
		/// <param name="str"></param>
		/// <returns>Encodingの候補</returns>
		/// <remarks>
		/// ASCII、Iso2022Jp、Shift_JIS、EUC-JP、UTF7、UTF8、UTF16LE/BE、UTF32LE/BEから判定します。
		/// </remarks>
		public static IEnumerable<Encoding> GetEncodings(string str){
			return GetEncodings(Encoding.UTF8.GetBytes(str));
		}
		
		public static IEnumerable<Encoding> GetEncodings(Stream stream){
			return GetEncodings(stream, null);
		}

		/// <summary>
		/// 文字列のEncodingを判定する
		/// </summary>
		/// <param name="stream"></param>
		/// <returns>Encodingの候補</returns>
		/// <remarks>
		/// ASCII、Iso2022Jp、Shift_JIS、EUC-JP、UTF7、UTF8、UTF16LE/BE、UTF32LE/BEから判定します。
		/// </remarks>
		public static IEnumerable<Encoding> GetEncodings(Stream stream, CancellationTokenSource tokenSource){
			var sevenBit = new SevenBitDetector();
			//var iso2022jp = new Iso2022JpDetector();
			var unicodeBom = new UnicodeBomDetector();
			//var shiftJis = new ShiftJisDetector();
			//var eucJp = new EucJpDetector();
			var utf8 = new Utf8Detector();
			//var utf7 = new Utf7Detector();
			var utf16le = new Utf16LEDetector();
			var utf16be = new Utf16BEDetector();
			//var utf32le = new Utf32LEDetector();
			//var utf32be = new Utf32BEDetector();
			
			const int bufferSize = 1024;
			var buffer = new byte[bufferSize];
			int count;
			var candidates = new HashSet<EncodingDetector>(new EncodingDetector[]{
				sevenBit/*, iso2022jp*/, unicodeBom/*, shiftJis, eucJp*/, utf8/*, utf7*/, utf16le, utf16be/*, utf32be, utf32le*/
			});
			while((count = stream.Read(buffer, 0, bufferSize)) > 0){
				if(tokenSource.IsCancellationRequested){
					yield break;
				}

				// バッファから
				byte[] data;
				if(count < bufferSize){
					data = new byte[count];
					Array.Copy(buffer, 0, data, 0, count);
				}else{
					data = buffer;
				}
				
				// チェック
				if(unicodeBom != null){
					// BOMチェック
					unicodeBom.Check(data);
					var enc = unicodeBom.Encoding;
					if(enc != null){
						yield return enc;
						yield break;
					}else{
						candidates.Remove(unicodeBom);
						unicodeBom = null;
					}
				}
				foreach(var detector in candidates){
					detector.Check(data);
				}
				if(sevenBit != null && !sevenBit.IsValid){
					candidates.Remove(sevenBit);
					//candidates.Remove(utf7);
					//candidates.Remove(iso2022jp);
					sevenBit = null;
				}
				if(utf8 != null && !utf8.IsValid){
					candidates.Remove(utf8);
					//candidates.Remove(utf32be);
					//candidates.Remove(utf32le);
					utf8 = null;
				}
				/*
				candidates.OfType<NihongoCountEncodingDetector>()
				          .OrderBy(c => c.ErrorCount).GroupBy(c => c.ErrorCount)
				          .Skip(1).Flatten().ForEach(det => candidates.Remove(det));
				if(candidates.Count == 1){
					break;
				}
				*/
			}

			if(sevenBit != null){
				/*if(iso2022jp.EscapeSequenceCount > 0){
					yield return iso2022jp.Encoding;
					yield break;
				}else if(utf7.IsValid && (utf7.Base64Count > 0)){
					yield return utf7.Encoding;
					yield break;
				}*/
			}
			var nihongos = candidates.OfType<NihongoCountEncodingDetector>().ToArray();
			var codes = nihongos.OrderByDescending(c => c.AsciiCount).GroupBy(c => c.AsciiCount).First();
			codes = codes.OrderByDescending(c => (c.HiraCount + c.KataCount)).GroupBy(c => (c.HiraCount + c.KataCount)).First();
			codes = codes.OrderByDescending(c => (c.KanjiCount)).GroupBy(c => (c.KanjiCount)).First();
			codes = codes.OrderBy(c => c.ErrorCount).GroupBy(c => c.ErrorCount).First();
			if(codes.Where(c => (c.HiraCount + c.KataCount + c.KanjiCount) == 0).FirstOrDefault() != null){
				yield return Encoding.UTF8;
			}else{
				foreach(var enc in codes.Select(c => c.Encoding)){
					yield return enc;
				}
			}
		}
		
		/// <summary>
		/// 文字列のEncodingを判定する
		/// </summary>
		/// <param name="data"></param>
		/// <returns>Encodingの候補</returns>
		/// <remarks>
		/// ASCII、Iso2022Jp、Shift_JIS、EUC-JP、UTF7、UTF8、UTF16LE/BE、UTF32LE/BEから判定します。
		/// </remarks>
		public static IEnumerable<Encoding> GetEncodings(byte[] data){
			var sevenBit = new SevenBitDetector();
			//var iso2022jp = new Iso2022JpDetector();
			var unicodeBom = new UnicodeBomDetector();
			//var shiftJis = new ShiftJisDetector();
			//var eucJp = new EucJpDetector();
			var utf8 = new Utf8Detector();
			//var utf7 = new Utf7Detector();
			var utf16le = new Utf16LEDetector();
			var utf16be = new Utf16BEDetector();
			//var utf32le = new Utf32LEDetector();
			//var utf32be = new Utf32BEDetector();
			sevenBit.Check(data);
			//iso2022jp.Check(data);
			unicodeBom.Check(data);
			//shiftJis.Check(data);
			//eucJp.Check(data);
			utf8.Check(data);
			//utf7.Check(data);
			utf16le.Check(data);
			utf16be.Check(data);
			//utf32le.Check(data);
			//utf32be.Check(data);
			
			var enc = unicodeBom.Encoding;
			if(enc != null){
				yield return enc;
			}else{
				foreach(var enc2 in GetEncodingsAfterScan(sevenBit/*, iso2022jp, utf7, shiftJis, eucJp*/, utf8, utf16le, utf16be/*, utf32le, utf32be*/)){
					yield return enc2;
				}
			}
		}
		
		private static IEnumerable<Encoding> GetEncodingsAfterScan(SevenBitDetector sevenBit/*, Iso2022JpDetector iso2022jp, Utf7Detector utf7, ShiftJisDetector shiftJis, EucJpDetector eucJp*/, Utf8Detector utf8, Utf16LEDetector utf16le, Utf16BEDetector utf16be/*, Utf32LEDetector utf32le, Utf32BEDetector utf32be*/){
			if(sevenBit.IsValid){ // 7 bit
				/*if(iso2022jp.EscapeSequenceCount > 0){
					yield return iso2022jp.Encoding;
					yield break;
				}else if(utf7.IsValid && (utf7.Base64Count > 0)){
					yield return utf7.Encoding;
					yield break;
				}*/
			}
			var nihongos = (utf8.IsValid) ? new NihongoCountEncodingDetector[]{/*shiftJis, eucJp, */utf8, utf16le, utf16be/*, utf32le, utf32be*/} :
			                              new NihongoCountEncodingDetector[]{/*shiftJis, eucJp,*/ utf16le, utf16be};
			var codes = nihongos.OrderByDescending(c => c.AsciiCount).GroupBy(c => c.AsciiCount).First();
			codes = codes.OrderByDescending(c => (c.HiraCount + c.KataCount)).GroupBy(c => (c.HiraCount + c.KataCount)).First();
			codes = codes.OrderByDescending(c => (c.KanjiCount)).GroupBy(c => (c.KanjiCount)).First();
			codes = codes.OrderBy(c => c.ErrorCount).GroupBy(c => c.ErrorCount).First();
			if(codes.Where(c => (c.HiraCount + c.KataCount + c.KanjiCount) == 0).FirstOrDefault() != null){
				yield return Encoding.UTF8;
			}else{
				foreach(var enc in codes.Select(c => c.Encoding)){
					yield return enc;
				}
			}
		}
		private class SevenBitDetector : EncodingDetector{
			public bool IsValid{get; private set;}
			
			public SevenBitDetector(){
				this.IsValid = true;
			}
			
			internal override void Check(byte[] data){
				for(int i = 0; i < data.Length; i++){
					if(data[i] > 0x7f){
						this.IsValid = false;
						break;
					}
				}
			}
			
			private static Encoding encoding = null;
			internal override Encoding Encoding{
				get{
					return encoding ?? (encoding = Encoding.UTF8);
				}
			}
		}
		/*
		class Iso2022JpDetector : EncodingDetector{
			public int EscapeSequenceCount{get; private set;}
			public Iso2022JpDetector(){
				this.EscapeSequenceCount = 0;
			}
			
			internal override void Check(byte[] data){
				for(int i = 0; i < data.Length; i++){
					byte c1 = data[i];
					int i2 = i + 1;
					int i3 = i + 2;
					if((c1 == 0x1b) && (i3 < data.Length)){
						byte c2 = data[i2];
						byte c3 = data[i3];
						
						if(c2 == 0x24){
							if((c3 == 0x40) || (c3 == 0x42)){ // 旧JIS漢字開始 / 新JIS漢字開始
								this.EscapeSequenceCount++;
								i += 2;
							}else if(c3 == 0x28){
								int i4 = i + 3;
								if(i4 < data.Length){
									byte c4 = data[i4];
									if((c4 == 0x44) || (c4 == 0x4f) || (c4 == 0x50) || (c4 == 0x51)){
										this.EscapeSequenceCount++;
										i += 3;
									}
								}
							}
						}else if((c2 == 0x26) && (c3 == 0x40)){
							int i4 = i + 3;
							int i5 = i + 4;
							int i6 = i + 5;
							if(i6 < data.Length){
								byte c4 = data[i4];
								byte c5 = data[i5];
								byte c6 = data[i6];
								if((c4 == 0x1b) && (c5 == 0x24) && (c6 == 0x42)){ // JIS X 0208-1990
									this.EscapeSequenceCount++;
									i += 5;
								}
							}
						}else if((c2 == 0x28) && ((c3 == 0x42) || (c3 == 0x4a) || (c3 == 0x49) || (c3 == 0x48))){
							this.EscapeSequenceCount++;
							i += 2;
						}
					}
				}
			}
			
			private static Encoding encoding = null;
			internal override Encoding Encoding{
				get{
					return encoding ?? (encoding = Encoding.GetEncoding(50220));
				}
			}
		}
		*/
		class UnicodeBomDetector : EncodingDetector{
			public UnicodeBom Bom{get; private set;}
			internal override void Check(byte[] data){
				// BOMチェック
				if((data.Length > 2) && (data[0] == 0xEF) && (data[1] == 0xBB) && (data[2] == 0xBF)){
					this.Bom = UnicodeBom.UTF8;
				}else if((data.Length > 2) && (data[0] == 0x2B) && (data[1] == 0x2F) && ((data[2] == 0x38) || (data[2] == 0x39) || (data[2] == 0x2B) || (data[2] == 0x2F))){
					this.Bom = UnicodeBom.UTF7;
				}else if((data.Length > 3) && (data[0] == 0x00) && (data[1] == 0x00) && (data[2] == 0xFE) && (data[3] == 0xFF)){
					this.Bom = UnicodeBom.UTF32BE;
				}else if((data.Length > 3) && (data[0] == 0xFF) && (data[1] == 0xFE) && (data[2] == 0x00) && (data[3] == 0x00)){
					this.Bom = UnicodeBom.UTF32LE;
				}else if((data.Length > 1) && (data[0] == 0xFE) && (data[1] == 0xFF)){
					this.Bom = UnicodeBom.UTF16BE;
				}else if((data.Length > 1) && (data[0] == 0xFF) && (data[1] == 0xFE)){
					this.Bom = UnicodeBom.UTF16LE;
				}else{
					this.Bom = UnicodeBom.None;
				}
			}
			
			internal override Encoding Encoding{
				get{
					switch(this.Bom){
						case UnicodeBom.UTF8:{
							return new UTF8Encoding(true);
						}
						/*case UnicodeBom.UTF7:{
							return Encoding.UTF7;
						}*/
						case UnicodeBom.UTF16LE:{
							return new UnicodeEncoding(false, true);
						}
						case UnicodeBom.UTF16BE:{
							return new UnicodeEncoding(true, true);
						}
						/*case UnicodeBom.UTF32LE:{
							return new UTF32Encoding(false, true);
						}
						case UnicodeBom.UTF32BE:{
							return new UTF32Encoding(true, true);
						}*/
						default:{
							return null;
						}
					}
				}
			}
		}

		abstract class CountEncodingDetector : EncodingDetector{
			public int ErrorCount{get; protected set;}
			public int AsciiCount{get; protected set;}
			
			public CountEncodingDetector(){
				this.ErrorCount = this.AsciiCount = 0;
			}
		}

		abstract class NihongoCountEncodingDetector : CountEncodingDetector{
			public int HiraCount{get; protected set;}
			public int KataCount{get; protected set;}
			public int KanjiCount{get; protected set;}
			
			public NihongoCountEncodingDetector(){
				this.HiraCount = this.KataCount = this.KanjiCount = 0;
			}
		}
		/*
		class ShiftJisDetector : NihongoCountEncodingDetector{
			internal override void Check(byte[] data){
				for(int i = 0; i < data.Length; i++){
					byte c1 = data[i];
					if((0x01 <= c1) && (c1 <= 0x7f)){ // ASCII
						this.AsciiCount++;
						continue;
					}else if((0xa1 <= c1) && (c1 <= 0xdf)){ // 半角
						continue;
					}else{
						int i2 = i + 1;
						if(i2 < data.Length){
							byte c2 = data[i2];
							if((c1 == 0x82) && (0x9f <= c2) && (c2 <= 0xf1)){ // ひらがな
								this.HiraCount++;
								i++;
								continue;
							}else if((c1 == 0x83) && (0x40 <= c2) && (c2 <= 0x96)){ // かたかな
								this.KataCount++;
								i++;
								continue;
							}else if(((0x85 <= c1) && (c1 <= 0x87)) || ((0xeb <= c1) && (c1 <= 0xef))){ // 未使用領域(だいたい)
								this.ErrorCount++;
								i++;
								continue;
							}else if(((0x89 <= c1) && (c1 <= 0x9f)) || ((0xe0 <= c1) && (c1 <= 0xea))){ // 漢字
								this.KanjiCount++;
								i++;
								continue;
							}else if((((0x81 <= c1) && (c1 <= 0x9f)) || ((0xe0 <= c1) && (c1 <= 0xef))) &&
								(((0x40 <= c2) && (c2 <= 0x7e)) || ((0x80 <= c2) && (c2 <= 0xfc)))){ // 全角
								i++;
								continue;
							}
						}else{
							continue;
						}
					}
					this.ErrorCount++;
				}
			}
			
			private static Encoding encoding = null;
			internal override Encoding Encoding{
				get{
					return encoding ?? (encoding = Encoding.GetEncoding(932));
				}
			}
		}*/
		/*
		class EucJpDetector : NihongoCountEncodingDetector{
			internal override void Check(byte[] data){
				for(int i = 0; i < data.Length; i++){
					byte c1 = data[i];
					if((0x01 <= c1) && (c1 <= 0x7f)){ // ASCII
						this.AsciiCount++;
						continue;
					}else{
						int i2 = i + 1;
						if(i2 < data.Length){
							byte c2 = data[i2];
							if((0xa4 == c1) && (0xa1 <= c2) && (c2 <= 0xf3)){ // ひらがな
								i++;
								this.HiraCount++;
								continue;
							}else if((0xa5 == c1) && (0xa1 <= c2) && (c2 <= 0xf6)){ // カタカナ
								i++;
								this.KataCount++;
								continue;
							}else if((0xb0 <= c1) && (c1 <= 0xf3)){ // 漢字
								i++;
								this.KanjiCount++;
								continue;
							}else if(((0xa9 <= c1) && (c1 <= 0xaf)) || ((0xf5 <= c1) && (c1 <= 0xfe))){ // 未使用領域(だいたい)
								this.ErrorCount++;
								i++;
								continue;
							}else if((0xa1 <= c1) && (c1 <= 0xfe) && (0xa1 <= c1) && (c1 <= 0xfe)){ // 全角
								i++;
								continue;
							}else if((c1 == 0x8e) && (0xa1 <= c2) && (c2 <= 0xfe)){ // 半角
								i++;
								continue;
							}
						}else{
							continue;
						}
					}
					this.ErrorCount++;
				}
			}
			
			private static Encoding encoding = null;
			internal override Encoding Encoding{
				get{
					return encoding ?? (encoding = Encoding.GetEncoding(20932));
				}
			}
		}
		*/
		class Utf8Detector : NihongoCountEncodingDetector{
			public bool IsValid{get; private set;}
			
			public Utf8Detector(){
				this.IsValid = true;
			}
			
			internal override void Check(byte[] data){
				for(int i = 0; i < data.Length; i++){
					byte c1 = data[i];
					if((0x01 <= c1) && (c1 <= 0x7f)){ // ASCII
						this.AsciiCount++;
						continue;
					}else if((c1 == 0xfe) || (c1 == 0xff)){ // BOMコード
						this.IsValid = false;
						continue;
					}else{
						int i2 = i + 1;
						if(i2 < data.Length){
							byte c2 = data[i2];
							if((0xc0 <= c1) && (c1 <= 0xdf) && (0x80 <= c2) && (c2 <= 0xbf)){ // U+0080...U+07FF
								i++;
								continue;
							}else{
								int i3 = i2 + 1;
								if(i3 < data.Length){
									byte c3 = data[i3];
									if((0xe0 <= c1) && (c1 <= 0xef) && (0x80 <= c2) && (c2 <= 0xbf) &&
										(0x80 <= c3) && (c3 <= 0xbf)){ // U+0800...U+FFFF
										int code = ((c1 & 0xf) << 12) + ((c2 & 0x3f) << 6) + (c3 & 0x3f);
										if((0x3040 <= code) && (code <= 0x309f)){ // ひらがな
											this.HiraCount++;
										}else if((0x30a1 <= code) && (code <= 0x30fa)){ // カタカナ
											this.KataCount++;
										}else if((0x4e00 <= code) && (code <= 0x9fff)){ // CJK統合漢字
											this.KanjiCount++;
										}
										i += 2;
										continue;
									}else{
										int i4 = i3 + 1;
										if(i4 < data.Length){
											byte c4 = data[i4];
											if((0xf0 <= c1) && (c1 <= 0xf7) && (0x80 <= c2) && (c2 <= 0xbf) &&
												(0x80 <= c3) && (c3 <= 0xbf) && (0x80 <= c4) && (c4 <= 0xbf)){ // U+10000...U+1FFFF
												i += 3;
												continue;
											}else{
												int i5 = i4 + 1;
												if(i5 < data.Length){
													byte c5 = data[i5];
													if((0xf8 <= c1) && (c1 <= 0xfb) && (0x80 <= c2) && (c2 <= 0xbf) &&
														(0x80 <= c3) && (c3 <= 0xbf) && (0x80 <= c4) && (c4 <= 0xbf) &&
														(0x80 <= c5) && (c5 <= 0xbf)){ // U+200000...U+3FFFFF
														i += 4;
														continue;
													}else{
														int i6 = i5 + 1;
														if(i6 < data.Length){
															byte c6 = data[i6];
															if((0xfc <= c1) && (c1 <= 0xfd) && (0x80 <= c2) && (c2 <= 0xbf) &&
																(0x80 <= c3) && (c3 <= 0xbf) && (0x80 <= c4) && (c4 <= 0xbf) &&
																(0x80 <= c5) && (c5 <= 0xbf) && (0x80 <= c6) && (c6 <= 0xbf)){ // U+400000...U+7FFFFFFF
																i += 4;
																continue;
															}
														}else{
															continue;
														}
													}
												}else{
													continue;
												}
											}
										}else{
											continue;
										}
									}
								}else{
									continue;
								}
							}
						}else{
							continue;
						}
					}
					this.ErrorCount++;
				}
			}
			
			internal override Encoding Encoding{
				get{
					return Encoding.UTF8;
				}
			}
		}
		
		class Utf16LEDetector : NihongoCountEncodingDetector{
			internal override void Check(byte[] data){
				for(int i = 0; i < data.Length; i++){
					int i2 = i + 1;
					if(i2 < data.Length){
						int c1 = (data[i2] << 8) + data[i];
						if((c1 >> 10) == 0x36){
							int i3 = i2 + 1;
							int i4 = i3 + 1;
							if(i4 < data.Length){
								int c2 = (data[i4] << 8) + data[i3];
								if((c2 >> 10) == 0x36){
									i += 3;
									continue;
								}
							}else{
								continue;
							}
						}else{ // U+0x0000...U+FFFF
							if((0x0001 <= c1) && (c1 <= 0x007f)){
								this.AsciiCount++;
								continue;
							}else if((0x3040 <= c1) && (c1 <= 0x309f)){ // ひらがな
								this.HiraCount++;
							}else if((0x30a1 <= c1) && (c1 <= 0x30fa)){ // カタカナ
								this.KataCount++;
							}else if((0x4e00 <= c1) && (c1 <= 0x9fff)){ // CJK統合漢字
								this.KanjiCount++;
							}else if(c1 == 0x0000){
								this.ErrorCount++;
							}
							i++;
							continue;
						}
					}else{
						continue;
					}
					this.ErrorCount++;
				}
			}
			
			private static Encoding encoding = null;
			internal override Encoding Encoding{
				get{
					return encoding ?? (encoding = new UnicodeEncoding(false, false));
				}
			}
		}
		
		class Utf16BEDetector : NihongoCountEncodingDetector{
			internal override void Check(byte[] data){
				for(int i = 0; i < data.Length; i++){
					int i2 = i + 1;
					if(i2 < data.Length){
						int c1 = (data[i] << 8) + data[i2];
						if((c1 >> 10) == 0x36){
							int i3 = i2 + 1;
							int i4 = i3 + 1;
							if(i4 < data.Length){
								int c2 = (data[i3] << 8) + data[i4];
								if((c2 >> 10) == 0x36){
									i += 3;
									continue;
								}
							}
						}else{ // U+0x0000...U+FFFF
							if((0x0001 <= c1) && (c1 <= 0x007f)){
								this.AsciiCount++;
								continue;
							}else if((0x3040 <= c1) && (c1 <= 0x309f)){ // ひらがな
								this.HiraCount++;
							}else if((0x30a1 <= c1) && (c1 <= 0x30fa)){ // カタカナ
								this.KataCount++;
							}else if((0x4e00 <= c1) && (c1 <= 0x9fff)){ // CJK統合漢字
								this.KanjiCount++;
							}else if(c1 == 0x0000){ // NULはエラー扱い
								this.ErrorCount++;
							}
							i++;
							continue;
						}
					}
					this.ErrorCount++;
				}
			}
			
			private static Encoding encoding = null;
			internal override Encoding Encoding{
				get{
					return encoding ?? (encoding = new UnicodeEncoding(true, false));
				}
			}
		}
		/*
		class Utf32LEDetector : NihongoCountEncodingDetector{
			internal override void Check(byte[] data){
				for(int i = 0; i < data.Length; i += 4){
					int i2 = i + 1;
					int i3 = i2 + 1;
					int i4 = i3 + 1;
					if(i4 < data.Length){
						int code = (data[i4] << 24) + (data[i3] << 16) + (data[i2] << 8) + data[i];
						if((0x0001 <= code) && (code <= 0x007f)){
							this.AsciiCount++;
							continue;
						}else if((0x3040 <= code) && (code <= 0x309f)){ // ひらがな
							this.HiraCount++;
						}else if((0x30a1 <= code) && (code <= 0x30fa)){ // カタカナ
							this.KataCount++;
						}else if((0x4e00 <= code) && (code <= 0x9fff)){ // CJK統合漢字
							this.KanjiCount++;
						}else if((0x0001 <= code) && (code <= 0x10ffff)){ // NULはエラー扱い
							this.ErrorCount += 4;
						}else{
							this.ErrorCount += 1;
						}
					}
				}
			}
			
			private static Encoding encoding = null;
			internal override Encoding Encoding{
				get{
					return encoding ?? (encoding = new UTF32Encoding(false, false));
				}
			}
		}
		
		class Utf32BEDetector : NihongoCountEncodingDetector{
			internal override void Check(byte[] data){
				for(int i = 0; i < data.Length; i += 4){
					int i2 = i + 1;
					int i3 = i2 + 1;
					int i4 = i3 + 1;
					if(i4 < data.Length){
						int code = (data[i] << 24) + (data[i2] << 16) + (data[i3] << 8) + data[i4];
						if((0x0001 <= code) && (code <= 0x007f)){ // ASCII
							this.AsciiCount++;
							continue;
						}else if((0x3040 <= code) && (code <= 0x309f)){ // ひらがな
							this.HiraCount++;
						}else if((0x30a1 <= code) && (code <= 0x30fa)){ // カタカナ
							this.KataCount++;
						}else if((0x4e00 <= code) && (code <= 0x9fff)){ // CJK統合漢字
							this.KanjiCount++;
						}else if((0x0001 <= code) && (code <= 0x10ffff)){ // NULはエラー扱い
							this.ErrorCount += 4;
						}else{
							this.ErrorCount += 1;
						}
					}
				}
			}
			
			private static Encoding encoding = null;
			internal override Encoding Encoding{
				get{
					return encoding ?? (encoding = new UTF32Encoding(true, false));
				}
			}
		}
		
		class Utf7Detector : EncodingDetector{
			public bool IsValid{get; private set;}
			public int Base64Count{get; private set;}
			private bool isInBase64 = false;
			
			public Utf7Detector(){
				this.IsValid = true;
				this.Base64Count = 0;
			}
			
			internal override void Check(byte[] data){
				for(int i = 0; i < data.Length; i++){
					byte c1 = data[i];
					if((c1 == 0x3d) || (c1 == 0x5c) || (c1 == 0x7e)){ // = / ~
						this.IsValid = false;
						break;
					}else if(this.isInBase64){
						if(c1 == 0x2d){ // -
							this.isInBase64 = false;
							this.Base64Count++;
						}else if(!(((0x30 <= c1) && (c1 <= 0x39)) ||
									((0x41 <= c1) && (c1 <= 0x5a)) ||
									((0x61 <= c1) && (c1 <= 0x7a)) ||
									(c1 == 0x2b) || (c1 == 0x2f))){
							this.IsValid = false;
							break;
						}
					}else{
						if(c1 == 0x2b){ // +
							this.isInBase64 = true;
						}
					}
				}
			}
			
			internal override Encoding Encoding{
				get{
					return Encoding.UTF7;
				}
			}
		}
		*/
	}

	enum UnicodeBom{
		None,
		UTF7,
		UTF8,
		UTF16BE,
		UTF16LE,
		UTF32BE,
		UTF32LE,
	}
}