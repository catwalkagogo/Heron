using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.IO {
	public sealed class WindowsPathFormat : CommonPathFormat{

		public override IReadOnlyList<string> NormalizeFragments(IReadOnlyList<string> fragments, FilePathKind pathKind) {
			var fragmentsArray = fragments.ToArray();

			if(pathKind == FilePathKind.Absolute && fragmentsArray.Length > 0 && fragmentsArray[0].Length > 1) {
				// ドライブ文字を大文字に
				fragmentsArray[0] = fragmentsArray[0].ToUpper().TrimEnd(':');
			}

			if(pathKind == FilePathKind.Relative) {
				fragmentsArray = FilePath.Resolve(fragmentsArray).ToArray();
			}

			return Array.AsReadOnly(fragmentsArray);
		}

		public override string DirectorySeparator {
			get {
				return "\\";
			}
		}

		public override IEnumerable<string> AltDirectorySeparators {
			get {
				return new string[]{"/"};
			}
		}

		public override bool Parse(string path, FilePathKind pathKind, out string[] fragments) {
			if(!Enum.IsDefined(typeof(FilePathKind), pathKind)) {
				throw new ArgumentException("pathKind");
			}
			fragments = null;

			// 絶対パスの場合、ドライブ名をチェック
			if(pathKind == FilePathKind.Absolute) {
				if(!(path.Length > 0 && (('A' <= path[0] && path[0] <= 'Z') || ('a' <= path[0] && path[0] <= 'z')))) {
					return false;
				}
				if(!IsAbsolute(path)) {
					return false;
				}

				// ドライブ区切り除去
				var v = path.Split(new string[]{":"}, 2, StringSplitOptions.None);
				path = v[0] + v[1];
			} else {
				if(IsAbsolute(path)) {
					return false;
				}
			}

			fragments = FilePath.Resolve(
				path.Split(
					this.AltDirectorySeparators
						.Concat(new string[]{this.DirectorySeparator})
						.ToArray(),
					StringSplitOptions.RemoveEmptyEntries
				)
			).ToArray();

			return this.IsValid(fragments);
		}

		private static bool IsAbsolute(string path){
			return (path.Length > 1 && path[1] == ':');
		}

		public override string GetPath(FilePath path) {
			return
				(path.Fragments != null && path.Fragments.Count > 0) ?
					String.Join(this.DirectorySeparator, path.Fragments.Skip(1)) :
					"";
		}

		public override string GetFullPath(FilePath path) {
			if(!path.IsValid) {
				throw new ArgumentException("path");
			}

			if(path.PathKind == FilePathKind.Absolute){
				return path.Fragments[0] + ":\\" + this.GetPath(path);
			}else{
				return null;
			}
		}

		public override IEqualityComparer<string> StringEqualityComparer {
			get {
				return StringComparer.OrdinalIgnoreCase;
			}
		}

		public override int GetHashCode(FilePath path) {
			return
				path.Path.ToUpper().GetHashCode() ^
				path.PathKind.GetHashCode() ^
				path.IsValid.GetHashCode() ^
				typeof(WindowsPathFormat).GetHashCode();
		}

		public override FilePathKind DetectPathKind(string path) {
			if(path == null){
				throw new ArgumentNullException("path");
			}
			if(path.Length > 1 && path[1] == ':') {
				return FilePathKind.Absolute;
			} else {
				return FilePathKind.Relative;
			}
		}
	}
}
