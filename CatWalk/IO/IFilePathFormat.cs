using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.IO {
	public interface IFilePathFormat {
		string NormalizePath(string path, FilePathKind pathKind);

		IReadOnlyList<string> NormalizeFragments(IReadOnlyList<string> fragments, FilePathKind pathKind);

		FilePathKind DetectPathKind(string path);

		bool Parse(string path, FilePathKind pathKind, out string[] fragments);

		bool IsValid(IEnumerable<string> fragments);

		string GetPath(FilePath path);

		string GetFullPath(FilePath path);

		FilePath PackRelativePath(FilePath relativePath);

		IEqualityComparer<string> StringEqualityComparer { get; }

		int GetHashCode(FilePath path);
	}
}
