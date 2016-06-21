using System;
using System.Collections.Generic;
using System.Threading;
using CatWalk.IOSystem;
using CatWalk.Heron.ViewModel.IOSystem;

namespace CatWalk.Heron.IOSystem {
	public interface ISystemProvider {
		string DisplayName { get; }
		IEnumerable<IColumnDefinition> GetColumnDefinitions(ISystemEntry entry);
		object GetEntryIcon(ISystemEntry entry, Int32Size size, System.Threading.CancellationToken token);
		/// <summary>
		/// ルートとなるISystemEntryの一覧を取得する
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		IEnumerable<CatWalk.IOSystem.ISystemEntry> GetRootEntries(ISystemEntry parent);
		string Name { get; }
		bool TryParsePath(ISystemEntry root, string path, out ISystemEntry entry);
		/// <summary>
		/// SystemEntryViewModelに紐づくViewModelを取得する
		/// </summary>
		/// <param name="parent">親ViewModel</param>
		/// <param name="entry">SystemEntryViewModel</param>
		/// <param name="previous">前のViewModel</param>
		/// <returns></returns>
		object GetViewModel(object parent, SystemEntryViewModel entry, object previous);
		IEnumerable<EntryGroupDescription> GetGroupings(ISystemEntry entry);
	}
}
