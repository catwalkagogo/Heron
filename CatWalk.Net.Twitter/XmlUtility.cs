using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.IO;

namespace CatWalk.Net.Twitter {
	internal static class XmlUtility {
		private static readonly Regex TagRegex = new Regex(@"<\s*([^?!].*?)\s*>", RegexOptions.Compiled | RegexOptions.Multiline);

		public static IEnumerable<XElement> FromStream(Stream stream){
			const int block = 4096 * 4;
			var sb = new StringBuilder(block);
			var buffer = new char[block];
			using(var reader = new StreamReader(stream, Encoding.UTF8)){
				bool isRoot = true;
				int length;
				int offset = 0;
				int nextOffset = 0;
				int openCount = 0;
				while((length = reader.Read(buffer, 0, block)) > 0){
					sb.Append(buffer, 0, length);
					int removedCount = 0;
					foreach(var match in TagRegex.Matches(sb.ToString(), offset).Cast<Match>()){
						if(match.Success){
							nextOffset = match.Index + match.Length;
							var tagName = match.Groups[1].Value;
							if(tagName[0] == '/'){	// End Tag
								openCount--;
							}else if(tagName[tagName.Length - 1] != '/'){
								openCount++;
							}
							if(openCount == 1){
								if(isRoot){
									removedCount += match.Index + match.Length - removedCount;
									isRoot = false;
								}else{
									var xml = sb.ToString().Substring(removedCount, match.Index + match.Length - removedCount);
									removedCount += match.Index + match.Length - removedCount;
									yield return XElement.Parse(xml);
								}
							}else if(openCount < 1){
								yield break;
							}
						}
					}
					if(removedCount > 0){
						sb.Remove(0, removedCount);
					}
					offset = nextOffset - removedCount;
				}
			}
		}
	}
}
