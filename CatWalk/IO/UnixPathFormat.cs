using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.IO {
	public sealed class UnixPathFormat : CommonPathFormat{
		internal UnixPathFormat() { }

		public override IReadOnlyList<string> NormalizeFragments(IReadOnlyList<string> fragments, FilePathKind pathKind) {
			var fragmentsArray = fragments.ToArray();

			if(pathKind == FilePathKind.Relative) {
				fragmentsArray = FilePath.Resolve(fragmentsArray).ToArray();
			}

			return fragmentsArray.AsReadOnly();
		}

		public override string DirectorySeparator {
			get {
				return "/";
			}
		}

		public override IEnumerable<string> AltDirectorySeparators {
			get {
				return new string[]{"\\"};
			}
		}

		public override bool Parse(string path, FilePathKind pathKind, out string[] fragments) {
			if(!Enum.IsDefined(typeof(FilePathKind), pathKind)) {
				throw new ArgumentException("pathKind");
			}
			fragments = null;

			// 絶対パスの場合、スラッシュで始まるか
			if(pathKind == FilePathKind.Absolute) {
				if(!path.StartsWith(this.DirectorySeparator)) {
					return false;
				}
			} else {
				if(path.StartsWith(this.DirectorySeparator)) {
					return false;
				}
			}

			fragments = FilePath.Resolve(
				path.Split(
					this.AltDirectorySeparators
						.Concat(new string[] { this.DirectorySeparator })
						.ToArray(),
					StringSplitOptions.RemoveEmptyEntries
				)
			).ToArray();

			return this.IsValid(fragments);
		}

		public override string GetPath(FilePath path) {
			return String.Join(this.DirectorySeparator, path.Fragments);
		}

		public override string GetFullPath(FilePath path) {
			if(path.IsAbsolute) {
				return this.DirectorySeparator + String.Join(this.DirectorySeparator, path.Fragments);
			} else {
				return null;
			}
		}

		public override IEqualityComparer<string> StringEqualityComparer {
			get {
				return StringComparer.Ordinal;
			}
		}

		public override int GetHashCode(FilePath path) {
			return
				path.Path.GetHashCode() ^
				path.PathKind.GetHashCode() ^
				path.IsValid.GetHashCode() ^
				typeof(UnixPathFormat).GetHashCode();
		}

		public override FilePathKind DetectPathKind(string path) {
			if(path == null){
				throw new ArgumentNullException("path");
			}

			if(path.StartsWith("/")) {
				return FilePathKind.Absolute;
			} else {
				return FilePathKind.Relative;
			}
		}
	}
}
