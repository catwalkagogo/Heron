using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using CatWalk.IOSystem;
using CatWalk.ComponentModel;

namespace CatWalk.Heron.IOSystem {
	public interface IColumnDefinition{
		string DisplayName {
			get;
		}
		object GetValue(ISystemEntry entry);
		object GetValue(ISystemEntry entry, bool noCache);
		object GetValue(ISystemEntry entry, bool noCache, CancellationToken token);
		string Name {
			get;
		}

		bool CanSort { get; }
		/// <summary>
		/// 取得した値に対するComaprerを取得する
		/// </summary>
		/// <param name="order"></param>
		/// <returns></returns>
		IComparer GetComparer(ListSortDirection order);
	}

	public interface IColumnDefinition<T> : IColumnDefinition {
		new T GetValue(ISystemEntry entry);
		new T GetValue(ISystemEntry entry, bool noCache);
		new T GetValue(ISystemEntry entry, bool noCache, CancellationToken token);

		new IComparer<T> GetComparer(ListSortDirection order);
	}
}
