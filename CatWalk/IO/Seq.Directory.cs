using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CatWalk.IO{
	using IO = System.IO;

	public static partial class Seq{
		#region Directory

		public static IEnumerable<string> EnumerateDirs(string path, IO::SearchOption option) {
			return EnumerateDirs(IO::Path.GetFullPath(path), option, CancellationToken.None);
		}
		public static IEnumerable<string> EnumerateDirs(string path, IO::SearchOption option, CancellationToken token) {
			return EnumerateDirs(IO::Path.GetFullPath(path), option, token, null);
		}
		public static IEnumerable<string> EnumerateDirs(string path, IO::SearchOption option, CancellationToken token, IProgress<double> progress) {
			return EnumerateFileSystemEntries(IO::Path.GetFullPath(path), option, token, progress, false, true, 0, 1);
		}

		public static IEnumerable<string> EnumerateFiles(string path, IO::SearchOption option) {
			return EnumerateFiles(IO::Path.GetFullPath(path), option, CancellationToken.None);
		}
		public static IEnumerable<string> EnumerateFiles(string path, IO::SearchOption option, CancellationToken token) {
			return EnumerateFiles(IO::Path.GetFullPath(path), option, token, null);
		}
		public static IEnumerable<string> EnumerateFiles(string path, IO::SearchOption option, CancellationToken token, IProgress<double> progress) {
			return EnumerateFileSystemEntries(IO::Path.GetFullPath(path), option, token, progress, true, false, 0, 1);
		}

		public static IEnumerable<string> EnumerateFileSystemEntries(string path, IO::SearchOption option) {
			return EnumerateFileSystemEntries(IO::Path.GetFullPath(path), option, CancellationToken.None);
		}
		public static IEnumerable<string> EnumerateFileSystemEntries(string path, IO::SearchOption option, CancellationToken token) {
			return EnumerateFileSystemEntries(IO::Path.GetFullPath(path), option, token, null);
		}
		public static IEnumerable<string> EnumerateFileSystemEntries(string path, IO::SearchOption option, CancellationToken token, IProgress<double> progress) {
			return EnumerateFileSystemEntries(IO::Path.GetFullPath(path), option, token, progress, true, true, 0, 1);
		}

		private static IEnumerable<string> EnumerateFileSystemEntries(string path, IO::SearchOption option, CancellationToken token, IProgress<double> iprogress, bool isEnumFiles, bool isEnumDirs, double progress, double step) {
			if(iprogress != null) {
				iprogress.Report(progress);
			}
			if(isEnumFiles){
				IEnumerable<string> files = null;
				try{
					files = IO::Directory.EnumerateFiles(path);
				}catch(IO::IOException){
				}catch(UnauthorizedAccessException){
				}
				if(files != null){
					foreach(var file in files) {
						yield return file;
					}
				}
			}
			if(option == IO::SearchOption.AllDirectories){
				string[] dirs = null;
				try{
					dirs = IO::Directory.EnumerateDirectories(path).ToArray();
				}catch(IO::IOException){
				}catch(UnauthorizedAccessException){
				}
				if(dirs != null){
					if(isEnumDirs){
						foreach(var dir in dirs) {
							yield return dir;
						}
					}
					var stepE = step / dirs.Length;
					for(int i = 0; i < dirs.Length; i++){
						var prog = progress + (step * i * stepE);
						foreach(var subfiles in EnumerateFileSystemEntries(dirs[i], option, token, iprogress, isEnumFiles, isEnumDirs, prog, stepE)){
							yield return subfiles;
						}
					}
				}
			}else if(isEnumDirs){
				IEnumerable<string> dirsQ = null;
				try{
					dirsQ = IO::Directory.EnumerateDirectories(path);
				}catch(IO::IOException){
				}catch(UnauthorizedAccessException){
				}
				if(dirsQ != null){
					if(iprogress != null){
						iprogress.Report(progress + step);
					}
					foreach(var dir in dirsQ) {
						yield return dir;
					}
				}
			}

		}

		#endregion
	}
}
