/*
	$Id: FilePath.cs 315 2013-12-11 07:59:06Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IO{
	using IO = System.IO;

	public struct FilePath : IEquatable<FilePath>{
		public FilePathKind PathKind{get; private set;}

		/// <summary>
		/// 未パースのパス
		/// </summary>
		public string RawPath { get; private set; }
		private readonly IReadOnlyList<string> _Fragments;

		/// <summary>
		/// パスの階層
		/// </summary>
		public IReadOnlyList<string> Fragments {
			get {
				return this._Fragments;
			}
		}
		/// <summary>
		/// 与えられたパスが正当かどうか
		/// </summary>
		public bool IsValid{get; private set;}

		/// <summary>
		/// パスのフォーマット(Windows/Unix)
		/// </summary>
		public IFilePathFormat Format { get; private set; }

		#region Constructor

		/*public FilePath(string path) : this(path, FilePathFormats.PlatformPathFormat){
		}*/

		public FilePath(string path, IFilePathFormat format) : this(path, null, format) {
		}
		/*
		public FilePath(string path, FilePathKind? pathKind) : this(path, pathKind, FilePathFormats.PlatformPathFormat){
		}
		*/
		public FilePath(string path, FilePathKind? pathKind, IFilePathFormat format) : this(){
			if(path == null) {
				throw new ArgumentNullException("path");
			}
			if(pathKind.HasValue && !Enum.IsDefined(typeof(FilePathKind), pathKind.Value)) {
				throw new ArgumentException("pathKind");
			}
			if(format == null) {
				throw new ArgumentNullException("format");
			}
			this.Format = format;
			this.RawPath = path;
			this.PathKind = !pathKind.HasValue ? format.DetectPathKind(path) : pathKind.Value;

			string[] fragments;
			this.IsValid = format.Parse(path, this.PathKind, out fragments);

			if(this.IsValid) {
				this._Fragments = format.NormalizeFragments(fragments, this.PathKind);
			} else {
				this._Fragments = new string[0];
			}
		}

		public FilePath(IEnumerable<string> fragments, FilePathKind pathKind, IFilePathFormat format)
			: this() {
			if(fragments == null) {
				throw new ArgumentNullException("fragments");
			}
			if(!Enum.IsDefined(typeof(FilePathKind), pathKind)) {
				throw new ArgumentException("pathKind");
			}
			if(format == null) {
				throw new ArgumentNullException("format");
			}
			this.Format = format;
			this.RawPath = null;
			this.PathKind = pathKind;
			this.IsValid = format.IsValid(fragments);

			if(this.IsValid) {
				this._Fragments = format.NormalizeFragments(fragments.ToArray(), this.PathKind);
			}
		}

		#endregion

		#region Normalize
		/*
		public static string Normalize(string path, FilePathKind pathKind) {
			return Normalize(path, pathKind, FilePathFormats.PlatformPathFormat);
		}
		*/
		/// <summary>
		/// パス文字列を正規化する
		/// 区切り文字をformatに合わせ、末尾の区切り文字を削除する
		/// Windowsの場合はドライブ文字を大文字にする
		/// </summary>
		/// <param name="path"></param>
		/// <param name="pathKind"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string Normalize(string path, FilePathKind pathKind, IFilePathFormat format){
			return format.NormalizePath(path, pathKind);
		}

		#endregion

		#region Properties
		public bool IsAbsolute {
			get {
				return this.PathKind == FilePathKind.Absolute;
			}
		}

		public bool IsRelative {
			get {
				return this.PathKind == FilePathKind.Relative;
			}
		}

		/// <summary>
		/// パス
		/// 絶対パスでWindowsの場合はボリューム区切り文字以降(C:\path→\path)
		/// </summary>
		public string Path {
			get {
				return this.Format.GetPath(this);
			}
		}

		/// <summary>
		/// フルパスを取得する。
		/// </summary>
		public string FullPath{
			get{
				return this.Format.GetFullPath(this);
			}
		}

		/// <summary>
		/// ファイル名を取得する。パスがドライブパスや、ディレクトリパス(区切り文字で終わっている)の場合は空文字を返す。
		/// </summary>
		public string FileName{
			get{
				this.ThrowIfInvalid();

				if(this._Fragments.Count > 0) {
					return this._Fragments[this._Fragments.Count - 1];
				} else {
					return "";
				}
			}
		}

		public string Extension{
			get{
				this.ThrowIfInvalid();

				var name = this.FileName;
				var idx = name.LastIndexOf('.');
				if(idx >= 0){
					return name.Substring(idx + 1);
				}else{
					return String.Empty;
				}
			}
		}

		public string FileNameWithoutExtension {
			get {
				var name = this.FileName;
				return name.Substring(0, name.Length - this.Extension.Length);
			}
		}

		public IEnumerable<FilePath> FragmentPaths {
			get {
				var list = new List<string>();
				foreach(var frag in this.Fragments) {
					list.Add(frag);
					yield return new FilePath(list, this.PathKind, this.Format);
				}
			}
		}

		public FilePath Parent {
			get {
				if(this._Fragments.Count == 0) {
					throw new InvalidOperationException("root path");
				}
				return new FilePath(this._Fragments.Take(this._Fragments.Count - 1), this.PathKind, this.Format);
			}
		}

		#endregion

		#region private Functions

		private void ThrowIfInvalid(){
			if(!this.IsValid){
				throw new InvalidOperationException("This path is invalid");
			}
		}

		#endregion

		#region GetFullPath

		public FilePath GetFullPath(string basePath){
			return this.GetFullPath(new FilePath(basePath, FilePathKind.Absolute, this.Format));
		}

		/// <summary>
		/// Get full path from base path.
		/// </summary>
		/// <param name="basePath">Base absolute path</param>
		/// <exception cref="System.InvalidOperationException">this path kind is not relative</exception>
		/// <exception cref="System.UriFormatException">The base path is not absolute.</exception>
		/// <returns>full path</returns>
		public FilePath GetFullPath(FilePath basePath){
			this.ThrowIfInvalid();
			if(this.PathKind != FilePathKind.Relative){
				throw new InvalidOperationException("Path kind is not relative");
			}
			if(basePath.PathKind != FilePathKind.Absolute){
				throw new ArgumentException("Given base path is not absolute", "basePath");
			}
			if(!basePath.IsValid){
				throw new ArgumentException("Given path is invalid.", "basePath");
			}

			return basePath.Resolve(this);
		}

		#endregion

		#region Resolve

		public FilePath Resolve(string relativePath){
			return this.Resolve(new FilePath(relativePath, FilePathKind.Relative, this.Format));
		}

		/// <summary>
		/// 指定した相対パスを結合する
		/// </summary>
		/// <param name="relativePath"></param>
		/// <exception cref="System.InvalidOperationException">this path is not valid.</exception>
		/// <exception cref="System.ArgumentException">given path is not relative</exception>
		/// <returns>二つのパスを結合したパス。</returns>
		public FilePath Resolve(FilePath relativePath){
			this.ThrowIfInvalid();

			if(relativePath.PathKind != FilePathKind.Relative) {
				throw new ArgumentException("Given path is not relative.", "relativePath");
			}
			if(!relativePath.IsValid) {
				throw new ArgumentException("Given path is invalid.", "relativePath");
			}

			var baseNames = this.Fragments;
			var destNames = relativePath.Fragments;
			var outNames = new List<string>(baseNames.Count + destNames.Count);
			foreach(var names in new IReadOnlyList<string>[] { baseNames, destNames }) {
				ResolveInternal(names, outNames);
			}
			return new FilePath(outNames, this.PathKind, this.Format);
		}

		private static void ResolveInternal(IEnumerable<string> names, List<string> outNames) {
			var names2 = names.ToArray();
			foreach(var name in names2) {
				//Console.WriteLine(name);
				if(name == "..") {
					if(outNames.Count > 0 && outNames[outNames.Count - 1] != "..") {
						outNames.RemoveAt(outNames.Count - 1);
					} else {
						outNames.Add("..");
					}
				} else if(name != ".") {
					outNames.Add(name);
				}
				//Console.WriteLine(String.Join("/", outNames));
			}
		}

		public static IReadOnlyList<string> Resolve(params IEnumerable<string>[] fragmentsList) {
			var outNames = new List<string>();
			foreach(var fragments in fragmentsList){
				ResolveInternal(fragments, outNames);
			}
			return outNames.AsReadOnly();
		}

		#endregion

		#region GetCommonRoot
		/*
		public static FilePath GetCommonRoot(params string[] paths){
			return GetCommonRoot(FilePathFormats.PlatformPathFormat, paths);
		}

		public static FilePath GetCommonRoot(IEnumerable<string> paths){
			return GetCommonRoot(FilePathFormats.PlatformPathFormat, paths);
		}
		*/
		public static FilePath GetCommonRoot(IFilePathFormat format, params string[] paths) {
			return GetCommonRoot(format, paths.Select(path => new FilePath(path, format)));
		}

		public static FilePath GetCommonRoot(IFilePathFormat format, IEnumerable<string> paths) {
			return GetCommonRoot(format, paths.Select(path => new FilePath(path, format)));
		}

		public static FilePath GetCommonRoot(IFilePathFormat format, IEnumerable<FilePath> paths) {
			return GetCommonRoot(format, paths.ToArray());
		}

		/// <summary>
		/// 指定したパスの配列に共通するパスを取得する。
		/// </summary>
		/// <param name="paths"></param>
		/// <exception cref="System.ArgumentException">指定したパスの配列の要素に相対パスが含まれているか、配列が空の時。</exception>
		/// <returns></returns>
		public static FilePath GetCommonRoot(IEnumerable<FilePath> paths){
			paths.ThrowIfNull("paths");
			var formats = paths.Select(path => path.Format).Distinct().ToArray();
			if(formats.Length > 1) {
				throw new ArgumentException("mixed diferent path formats");
			}
			return GetCommonRoot(formats[0], paths.ToArray());
		}

		public static FilePath GetCommonRoot(params FilePath[] paths) {
			paths.ThrowIfNull("paths");
			var formats = paths.Select(path => path.Format).Distinct().ToArray();
			if (formats.Length > 1) {
				throw new ArgumentException("mixed diferent path formats");
			}
			return GetCommonRoot(formats[0], paths);
		}

		/// <summary>
		/// 指定したパスの配列に共通するパスを取得する。
		/// </summary>
		/// <param name="paths"></param>
		/// <exception cref="System.ArgumentException">指定したパスの配列の要素に相対パスが含まれているか、配列が空の時。</exception>
		/// <returns></returns>
		public static FilePath GetCommonRoot(IFilePathFormat format, params FilePath[] paths){
			if(paths.Any(path => path.PathKind != FilePathKind.Absolute || !path.IsValid)) {
				throw new ArgumentException("One or more paths are relative or invalid.", "paths");
			}
			if(paths.Length == 0) {
				throw new ArgumentException("paths parameter is empty", "paths");
			}else if(paths.Length == 1) {
				return paths[0];
			} else {
				IEnumerable<string> fragments = paths[0].Fragments;
				var cmp = format.StringEqualityComparer;
				foreach(var path in paths.Skip(0)) {
					var fragments2 = path.Fragments;
					fragments = fragments
						.TakeWhile((frg, i) => cmp.Equals(frg, fragments2[i]));
				}
				return new FilePath(fragments, FilePathKind.Absolute, format);
			}
		}

		#endregion

		#region GetRelativePathTo

		public FilePath GetRelativePathTo(string dest){
			return this.GetRelativePathTo(new FilePath(dest, FilePathKind.Absolute, this.Format));
		}

		/// <summary>
		/// 指定した絶対パスへの相対パスを取得する
		/// </summary>
		/// <param name="dest"></param>
		/// <exception cref="System.InvalidOperationException">this path kind is not absolute</exception>
		/// <exception cref="System.ArgumentException">dest path kind is not absolute.</exception>
		/// <returns>相対パス</returns>
		public FilePath GetRelativePathTo(FilePath dest){
			this.ThrowIfInvalid();

			if(this.PathKind != FilePathKind.Absolute){
				throw new InvalidOperationException("This path kind is not absolute");
			}
			if(dest.PathKind != FilePathKind.Absolute){
				throw new ArgumentException("Given dest path is not absolute", "dest");
			}
			if(!dest.IsValid){
				throw new ArgumentException("Given dest path is invalid.", "dest");
			}

			var common = GetCommonRoot(this, dest).Fragments;
			var fromRoute = this.Fragments.Skip(common.Count);
			var destRoute = dest.Fragments.Skip(common.Count);
			return new FilePath(
				Enumerable.Repeat(@"..", fromRoute.Count()).Concat(destRoute),
				FilePathKind.Relative,
				this.Format
			);
		}

		#endregion

		#region PackRelativePath

		public static FilePath PackRelativePath(FilePath relativePath) {
			return PackRelativePath(relativePath, relativePath.Format);
		}

		/// <summary>
		/// 相対パスの余分を削除します
		/// </summary>
		/// <param name="relativePath"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static FilePath PackRelativePath(FilePath relativePath, IFilePathFormat format){
			if(format == null) {
				throw new ArgumentNullException("format");
			}

			return format.PackRelativePath(relativePath);
		}

		#endregion

		#region ConvertFormat

		public FilePath ConvertFormat(IFilePathFormat format) {
			if(format == null) {
				throw new ArgumentNullException("format");
			}

			if(this.Format == format) {
				return this;
			} else {
				return new FilePath(this.Fragments, this.PathKind, format);
			}
		}

		#endregion

		#region IEquatable<FilePath> Members

		public bool Equals(FilePath other) {
			return
				this.IsValid.Equals(other.IsValid) &&
				this.PathKind.Equals(other.PathKind) &&
				this.Format.Equals(other.Format) &&
				this._Fragments.SequenceEqual(other._Fragments, this.Format.StringEqualityComparer);
		}

		public override int GetHashCode() {
			return this.Format.GetHashCode(this);
		}

		public override bool Equals(object obj) {
			if(!(obj is FilePath)) {
				return false;
			}
			return this.Equals((FilePath)obj);
		}

		#endregion

		#region Operators

		public static bool operator ==(FilePath a, FilePath b){
			return a.Equals(b);
		}

		public static bool operator !=(FilePath a, FilePath b){
			return !a.Equals(b);
		}

		#endregion
	}

	public enum FilePathKind{
		Absolute = 0,
		Relative = 1,
	}
}
