using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk.Heron.IOSystem;
using CatWalk.IOSystem;

namespace CatWalk.Heron.Configuration.IOSystem {
	public class ConfigurationEntryOperator : IEntryOperator {
		public IEnumerable<ISystemEntry> CanCopyTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest) {
			return new ISystemEntry[0];
		}

		public IEnumerable<ISystemEntry> CanCopyToClipboard(IEnumerable<ISystemEntry> entries) {
			return entries.OfType<ConfigurationEntry>();
		}

		public bool CanCreate(ISystemEntry parent) {
			return parent is ConfigurationDirectory;
		}

		public IEnumerable<ISystemEntry> CanDelete(IEnumerable<ISystemEntry> entries) {
			return entries.OfType<ConfigurationEntry>();
		}

		public IEnumerable<ISystemEntry> CanMoveTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest) {
			return new ISystemEntry[0];
		}

		public IEnumerable<ISystemEntry> CanMoveToClipboard(IEnumerable<ISystemEntry> entries) {
			return new ISystemEntry[0];
		}

		public IEnumerable<ISystemEntry> CanOpen(IEnumerable<ISystemEntry> entries) {
			return entries.OfType<ConfigurationEntry>();
		}

		public bool CanPasteTo(ISystemEntry dest) {
			return false;
		}

		public bool CanRename(ISystemEntry entry) {
			return entry is ConfigurationEntry;
		}

		public Task<IEntryOperationResult> CopyTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest, CancellationToken token, IProgress<double> progress) {
			throw new NotImplementedException();
		}

		public Task<IEntryOperationResult> CopyToClipboard(IEnumerable<ISystemEntry> entries, CancellationToken token, IProgress<double> progress) {
			throw new NotImplementedException();
		}

		public Task Create(ISystemEntry parent, string newName, CancellationToken token, IProgress<double> progress) {
			var dir = parent as ConfigurationDirectory;
			if(dir == null) {
				throw new ArgumentException("parent");
			}

			return dir.Storage.SetAsync<object>(newName, null, token);
		}

		public Task<IEntryOperationResult> Delete(IEnumerable<ISystemEntry> entries, bool canUndo, CancellationToken token, IProgress<double> progress) {
			return Task.Run<IEntryOperationResult>(() => {
				entries = this.CanDelete(entries).ToArray();
				foreach(var entry in entries.Cast<ConfigurationEntry>()) {
					entry.Configuration.Storage.Remove(entry.Name);
				}

				return new EntryOperationResult(entries);
			}, token);
		}

		public Task<IEntryOperationResult> MoveTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest, CancellationToken token, IProgress<double> progress) {
			throw new NotImplementedException();
		}

		public Task<IEntryOperationResult> MoveToClipboard(IEnumerable<ISystemEntry> entries, CancellationToken token, IProgress<double> progress) {
			throw new NotImplementedException();
		}

		public Task<IEntryOperationResult> Open(IEnumerable<ISystemEntry> entries, CancellationToken token, IProgress<double> progress) {
			throw new NotImplementedException();
		}

		public Task PasteTo(ISystemEntry dest, CancellationToken token, IProgress<double> progress) {
			throw new NotImplementedException();
		}

		public async Task Rename(ISystemEntry entry, string newName, CancellationToken token, IProgress<double> progress) {
			var confEntry = (ConfigurationEntry)entry;

			var data = await confEntry.GetValueAsync<object>(token);
			await confEntry.SetValueAsync(data, token);
		}
	}
}
