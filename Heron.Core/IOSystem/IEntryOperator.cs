using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using CatWalk.IOSystem;

namespace CatWalk.Heron.IOSystem {
	public interface IEntryOperator {
		/// <summary>
		/// コピー可能なエントリーを列挙する
		/// </summary>
		/// <param name="entries"></param>
		/// <param name="dest"></param>
		/// <returns></returns>
		IEnumerable<ISystemEntry> CanCopyTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest);
		Task<IEntryOperationResult> CopyTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest, CancellationToken token, IProgress<double> progress);
		
		IEnumerable<ISystemEntry> CanMoveTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest);
		Task<IEntryOperationResult> MoveTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest, CancellationToken token, IProgress<double> progress);
		
		IEnumerable<ISystemEntry> CanDelete(IEnumerable<ISystemEntry> entries);
		Task<IEntryOperationResult> Delete(IEnumerable<ISystemEntry> entries, bool canUndo, CancellationToken token, IProgress<double> progress);
		
		bool CanRename(ISystemEntry entry);
		Task Rename(ISystemEntry entry, string newName, CancellationToken token, IProgress<double> progress);
		
		bool CanCreate(ISystemEntry parent);
		Task Create(ISystemEntry parent, string newName, CancellationToken token, IProgress<double> progress);

		IEnumerable<ISystemEntry> CanOpen(IEnumerable<ISystemEntry> entries);
		Task<IEntryOperationResult> Open(IEnumerable<ISystemEntry> entries, CancellationToken token, IProgress<double> progress);

		IEnumerable<ISystemEntry> CanCopyToClipboard(IEnumerable<ISystemEntry> entries);
		Task<IEntryOperationResult> CopyToClipboard(IEnumerable<ISystemEntry> entries, CancellationToken token, IProgress<double> progress);

		IEnumerable<ISystemEntry> CanMoveToClipboard(IEnumerable<ISystemEntry> entries);
		Task<IEntryOperationResult> MoveToClipboard(IEnumerable<ISystemEntry> entries, CancellationToken token, IProgress<double> progress);

		bool CanPasteTo(ISystemEntry dest);
		Task PasteTo(ISystemEntry dest, CancellationToken token, IProgress<double> progress);
	}

	public interface IEntryOperationResult {
		IReadOnlyCollection<ISystemEntry> Entries { get; }

	}

	public class EntryOperationResult : IEntryOperationResult{
		public IReadOnlyCollection<ISystemEntry> Entries { get; private set; }

		public EntryOperationResult(IEnumerable<ISystemEntry> entries) {
			entries.ThrowIfNull("entries");
			this.Entries = entries.ToList().AsReadOnly();
		}

	}
}
