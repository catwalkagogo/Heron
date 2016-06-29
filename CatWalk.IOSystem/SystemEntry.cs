/*
	$Id: SystemEntry.cs 316 2013-12-26 10:16:12Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace CatWalk.IOSystem {
	public abstract class SystemEntry : ISystemEntry{
		public const char DirectorySeperatorChar = '/';

		/// <summary>
		/// コンストラクタ。parentがnullの場合はルートフォルダになります。
		/// </summary>
		/// <param name="parent">親のISystemDirectory</param>
		/// <param name="id">同階層内で一意な識別子</param>
		public SystemEntry(ISystemEntry parent, string name){
			if(name == null){
				throw new ArgumentNullException("name");
			}
			this.Parent = parent;
			this.Name = name;
			this.DisplayName = name;
		}

		#region Implemented

		/// <summary>
		/// 同階層内で一意な識別子
		/// </summary>
		public string Name{get; private set;}

		public string Path{
			get{
				if(this.Parent != null){
					return this.Parent.ConcatPath(this.Name);
				}else{
					return this.Name;
				}
			}
		}

		/// <summary>
		/// 表示名
		/// </summary>
		public virtual string DisplayName{get; private set;}

		/// <summary>
		/// 親のISystemDirectory
		/// </summary>
		public ISystemEntry Parent{get; private set;}

		/// <summary>
		/// 表示パス。
		/// 親のISystemDirectoryのConcatDisplayPath関数によりDisplayNameを連結してDisplayPathに設定します。
		/// </summary>
		public string DisplayPath{
			get{
				if(this.Parent == null){
					return this.DisplayName;
				}else{
					return this.Parent.ConcatDisplayPath(this.DisplayName);
				}
			}
		}

		/// <summary>
		/// このエントリの実体が存在するかどうか。
		/// 既定では親のISystemDirectoryのContains関数を呼び出します。
		/// </summary>
		public virtual bool IsExists() {
			return (this.Parent != null) ? this.Parent.Contains(this.Name) : true;
		}

		public virtual bool IsExists(CancellationToken token){
			return (this.Parent != null) ? this.Parent.Contains(this.Name, token) : true;
		}

		public virtual bool IsExists(CancellationToken token, IProgress<double> progress) {
			return (this.Parent != null) ? this.Parent.Contains(this.Name, token, progress) : true;
		}

		#endregion

		#region Equals

		public override bool Equals(object obj) {
			var entry = obj as ISystemEntry;
			if(entry != null) {
				return this.Equals(entry);
			} else {
				return base.Equals(obj);
			}
		}

		public bool Equals(ISystemEntry entry) {
			return this.Path.Equals(entry);
		}

		public override int GetHashCode() {
			return this.Path.GetHashCode();
		}

		public static bool operator ==(SystemEntry a, SystemEntry b) {
			if(System.Object.ReferenceEquals(a, b)) {
				return true;
			}

			if(((object)a == null) || ((object)b == null)) {
				return false;
			}

			return a.Equals(b);
		}

		public static bool operator !=(SystemEntry a, SystemEntry b) {
			return !(a == b);
		}

		#endregion

		#region Directory Members

		protected void ThrowIfNotDirectory() {
			if(!this.IsDirectory){
				throw new InvalidOperationException("This entry is not a directory.");
			}
		}

		protected virtual StringComparison StringComparison {
			get {
				return StringComparison.Ordinal;
			}
		}

		public abstract bool IsDirectory { get; }

		public IEnumerable<ISystemEntry> GetChildren() {
			this.ThrowIfNotDirectory();
			return this.GetChildren(CancellationToken.None, null);
		}
		public IEnumerable<ISystemEntry> GetChildren(CancellationToken token) {
			this.ThrowIfNotDirectory();
			return this.GetChildren(token, null);
		}
		public abstract IEnumerable<ISystemEntry> GetChildren(CancellationToken token, IProgress<double> progress);

		public ISystemEntry GetChild(string name) {
			this.ThrowIfNotDirectory();
			return this.GetChild(name, CancellationToken.None, null);
		}
		public ISystemEntry GetChild(string name, CancellationToken token) {
			this.ThrowIfNotDirectory();
			return this.GetChild(name, token, null);
		}
		public virtual ISystemEntry GetChild(string name, CancellationToken token, IProgress<double> progress) {
			this.ThrowIfNotDirectory();
			return this.GetChildren(token, progress).OfType<ISystemEntry>().FirstOrDefault(entry => entry.Name.Equals(name, this.StringComparison));
		}

		public bool Contains(string name) {
			this.ThrowIfNotDirectory();
			return this.Contains(name, CancellationToken.None, null);
		}
		public bool Contains(string name, CancellationToken token) {
			this.ThrowIfNotDirectory();
			return this.Contains(name, token, null);
		}
		public virtual bool Contains(string name, CancellationToken token, IProgress<double> progress) {
			this.ThrowIfNotDirectory();
			return this.GetChildren(token, progress).Any(entry => entry.Name.Equals(name, this.StringComparison));
		}

		public virtual string ConcatPath(string name) {
			this.ThrowIfNotDirectory();
			return this.Path + DirectorySeperatorChar + name;
		}

		public virtual string ConcatDisplayPath(string name) {
			this.ThrowIfNotDirectory();
			return this.DisplayPath + DirectorySeperatorChar + name;
		}

		#endregion
	}
}
