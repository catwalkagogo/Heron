using System;
using System.Collections.Generic;
using System.Threading;
using CatWalk.IOSystem;
using CatWalk.Heron.ViewModel.IOSystem;

namespace CatWalk.Heron.IOSystem {
	internal interface ISystemProvider {
		/// <summary>
		/// プロバイダ表示名を取得する
		/// </summary>
		string DisplayName { get; }

		/// <summary>
		/// カラム定義を取得する
		/// </summary>
		/// <param name="entry"></param>
		/// <returns></returns>
		IEnumerable<IColumnDefinition> GetColumnDefinitions(ISystemEntry entry);

		/// <summary>
		/// システムエントリーのアイコンを取得する
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="size"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		object GetEntryIcon(ISystemEntry entry, Size<int> size, System.Threading.CancellationToken token);

		/// <summary>
		/// ルートとなるISystemEntryの一覧を取得する
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		IEnumerable<CatWalk.IOSystem.ISystemEntry> GetRootEntries(ISystemEntry parent);

		/// <summary>
		/// プロバイダ名
		/// </summary>
		string Name { get; }

		ParsePathResult ParsePath(ISystemEntry root, string path);
		/// <summary>
		/// SystemEntryViewModelに紐づくViewModelを取得する
		/// </summary>
		/// <param name="parent">親ViewModel</param>
		/// <param name="entry">SystemEntryViewModel</param>
		/// <param name="previous">前のViewModel</param>
		/// <returns></returns>
		object GetViewModel(object parent, SystemEntryViewModel entry, object previous);

		/// <summary>
		/// 利用可能なグルーピングを取得する
		/// </summary>
		/// <param name="entry"></param>
		/// <returns></returns>
		IEnumerable<IGroupDefinition> GetGroupings(ISystemEntry entry);

		IEnumerable<OrderDefinition> GetOrderDefinitions(SystemEntryViewModel entry);
	}

	public class ParsePathResult {
		public bool Success { get; private set; }
		public ISystemEntry Entry { get; private set; }
		public bool TerminatedByDirectorySeparator { get; private set; }

		public ParsePathResult(bool success, ISystemEntry entry, bool terminatedByDirectorySeparator) {
			this.Success = success;
			this.Entry = entry;
			this.TerminatedByDirectorySeparator = terminatedByDirectorySeparator;
		}
	}
}
