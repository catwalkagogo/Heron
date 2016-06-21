using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using CatWalk.IOSystem;

namespace CatWalk.Heron.IOSystem {
	/// <summary>
	/// entryに紐づくカラムの定義
	/// 値の取得を行う
	/// </summary>
	public abstract class ColumnDefinition : IColumnDefinition{
		public string Name {
			get {
				return this.GetType().FullName;
			}
		}
		public virtual string DisplayName {
			get {
				return this.GetType().Name;
			}
		}
		public object GetValue(ISystemEntry entry) {
			return this.GetValue(entry, false, CancellationToken.None);
		}

		public object GetValue(ISystemEntry entry, bool noCache) {
			return this.GetValue(entry, noCache, CancellationToken.None);
		}

		public object GetValue(ISystemEntry entry, bool noCache, CancellationToken token){
			return this.GetValueImpl(entry, noCache, token);
		}

		protected abstract object GetValueImpl(ISystemEntry entry, bool noCache, CancellationToken token);

		/*
		#region Equals

		public override bool Equals(object obj) {
			var def = obj as ColumnDefinition;
			if(def != null) {
				return this.Equals(def);
			} else {
				return this.Equals(obj);
			}
		}

		public override int GetHashCode() {
			return this.Name.GetHashCode();
		}

		public bool Equals(ColumnDefinition other) {
			if(other == null) {
				return false;
			} else {
				return this.Name.Equals(other);
			}
		}

		#endregion
		*/
		#region Builtins
		private static NameColumnDefinition _NameColumn = new NameColumnDefinition();
		public static ColumnDefinition<string> NameColumn {
			get {
				return _NameColumn;
			}
		}

		private static DisplayNameColumnDefinition _DisplayNameColumn = new DisplayNameColumnDefinition();
		public static ColumnDefinition<string> DisplayNameColumn {
			get {
				return _DisplayNameColumn;
			}
		}

		private class NameColumnDefinition : ColumnDefinition<string> {

			public override string DisplayName {
				get {
					return "Name";
				}
			}

			protected override object GetValueImpl(ISystemEntry entry, bool noCache, CancellationToken token) {
				return entry.Name;
			}
		}

		private class DisplayNameColumnDefinition : ColumnDefinition<string> {

			public override string DisplayName {
				get {
					return "DisplayName";
				}
			}

			protected override object GetValueImpl(ISystemEntry entry, bool noCache, CancellationToken token) {
				return entry.DisplayName;
			}
		}
		#endregion
	}

	public abstract class ColumnDefinition<T> : ColumnDefinition, IColumnDefinition<T> {

		#region IColumnDefinition<T> Members

		public new T GetValue(ISystemEntry entry) {
			return (T)base.GetValue(entry);
		}

		public new T GetValue(ISystemEntry entry, bool noCache) {
			return (T)base.GetValue(entry, noCache);
		}

		public new T GetValue(ISystemEntry entry, bool noCache, CancellationToken token) {
			return (T)base.GetValue(entry, noCache, token);
		}

		#endregion
	}
}
