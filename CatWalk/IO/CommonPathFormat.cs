using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.IO {
	public abstract class CommonPathFormat : IFilePathFormat{
		public string TrimEndSeparator(string path) {
			var sep = this.DirectorySeparator;
			while(path.EndsWith(sep, this.StringComparison)) {
				path = path.Substring(0, path.Length - sep.Length);
			}
			return path;
		}

		public bool EndsWithDirectorySeparator(string path) {
			path.ThrowIfNull("path");
			return path.EndsWith(this.DirectorySeparator, this.StringComparison);
		}

		public virtual string NormalizePath(string path, FilePathKind pathKind) {
			foreach(var sep in this.AltDirectorySeparators) {
				path = path.Replace(sep, this.DirectorySeparator);
			}
			if(pathKind == FilePathKind.Relative) {
				return this.PackRelativePath(this.TrimEndSeparator(path));
			} else {
				return this.TrimEndSeparator(path);
			}
		}

		public abstract IReadOnlyList<string> NormalizeFragments(IReadOnlyList<string> fragments, FilePathKind pathKind);
		
		private string PackRelativePath(string relativePath) {
			return String.Join(
				this.DirectorySeparator,
				FilePath.Resolve(
					relativePath.Split(
						new string[]{this.DirectorySeparator},
						StringSplitOptions.RemoveEmptyEntries
					)
				)
			);
		}

		public abstract string DirectorySeparator { get; }

		public abstract IEnumerable<string> AltDirectorySeparators { get; }

		private static readonly IReadOnlyCollection<char> _InvalidFileNameChars = 
			new char[] { '\"', '<', '>', '|', '\0', (Char)1, (Char)2, (Char)3, (Char)4, (Char)5, (Char)6, (Char)7, (Char)8, (Char)9, (Char)10, (Char)11, (Char)12, (Char)13, (Char)14, (Char)15, (Char)16, (Char)17, (Char)18, (Char)19, (Char)20, (Char)21, (Char)22, (Char)23, (Char)24, (Char)25, (Char)26, (Char)27, (Char)28, (Char)29, (Char)30, (Char)31, ':', '*', '?', '\\', '/' }.AsReadOnly();
		public IReadOnlyCollection<char> InvalidFileNameChars {
			get {
				return _InvalidFileNameChars;
			}
		}

		public abstract bool Parse(string path, FilePathKind pathKind, out string[] fragments);

		public abstract string GetPath(FilePath path);

		public abstract string GetFullPath(FilePath path);

		public FilePath PackRelativePath(FilePath relativePath) {
						if(!relativePath.IsValid){
				throw new ArgumentException("relativePath");
			}
			if(relativePath.PathKind != FilePathKind.Relative){
				throw new ArgumentException("relativePath");
			}

			return new FilePath(PackRelativePath(relativePath.Path), relativePath.Format);
		}

		public abstract IEqualityComparer<string> StringEqualityComparer {
			get;
		}

		public abstract StringComparison StringComparison { get; }

		public abstract int GetHashCode(FilePath path);

		public abstract FilePathKind DetectPathKind(string path);

		public virtual bool IsValid(IEnumerable<string> fragments) {
			var nameInvChars = this.InvalidFileNameChars;
			return !fragments.Any(name => name.ToCharArray().Any(c => nameInvChars.Contains(c)));
		}
	}
}
