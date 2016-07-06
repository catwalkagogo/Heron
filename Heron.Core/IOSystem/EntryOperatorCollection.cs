using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk.IOSystem;

namespace CatWalk.Heron.IOSystem {
	public class EntryOperatorCollection : ICollection<IEntryOperator>, IEntryOperator{
		private ICollection<IEntryOperator> _Collection = new LinkedList<IEntryOperator>();

		public int Count {
			get {
				return _Collection.Count;
			}
		}

		public bool IsReadOnly {
			get {
				return _Collection.IsReadOnly;
			}
		}

		private IEnumerable<T> Call<T>(Func<IEntryOperator, IEnumerable<T>> call) {
			return this.Call(call, CancellationToken.None);
		}
		private IEnumerable<T> Call<T>(Func<IEntryOperator, IEnumerable<T>> call, CancellationToken token) {
			var list = new List<T>();
			foreach(var op in this) {
				token.ThrowIfCancellationRequested();
				var ent = call(op);
				list.AddRange(ent);
				if(list.Count > 0) {
					break;
				}
			}
			return list;
		}

		private Task<IEntryOperationResult> CallTask<T>(Func<IEntryOperator, IEnumerable<T>> call, Func<IEntryOperator, Task<IEntryOperationResult>> callTask, CancellationToken token) {
			return new Task<IEntryOperationResult>(() => {
				foreach (var op in this) {
					token.ThrowIfCancellationRequested();
					var ent = call(op).ToArray();
					if (ent.Length > 0) {
						// task実行
						var task = callTask(op);
						task.Wait();
						return task.Result;
					}
				}

				return new EntryOperationResult(new ISystemEntry[0]);
			});
		}

		private IEnumerable<T> Call<T>(IEnumerable<T> source, Func<IEntryOperator, IEnumerable<T>, IEnumerable<T>> call) {
			return this.Call(source, call, CancellationToken.None);
		}

		private IEnumerable<T> Call<T>(IEnumerable<T> source, Func<IEntryOperator, IEnumerable<T>, IEnumerable<T>> call, CancellationToken token) {
			var set = new HashSet<T>(source);
			var list = new List<T>();
			foreach(var op in this) {
				token.ThrowIfCancellationRequested();
				var ent = call(op, set);
				set.ExceptWith(ent);
				list.AddRange(ent);
			}
			return list;
		}

		private Task<IEntryOperationResult> CallTask<T>(IEnumerable<T> source, Func<IEntryOperator, IEnumerable<T>, IEnumerable<T>> call, Func<IEntryOperator, IEnumerable<T>, IProgress<double>, Task<IEntryOperationResult>> callTask, CancellationToken token, IProgress<double> progress) {
			return new Task<IEntryOperationResult>(() => {
				var aggregateProgress = ProgressAggregators.GetDoubleProgressAggregator(progress);

				var set = new HashSet<T>(source);
				var tasks = new List<Task<IEntryOperationResult>>();
				foreach (var op in this) {
					token.ThrowIfCancellationRequested();
					var ent = call(op, set).ToArray();
					var task = callTask(op, ent, aggregateProgress.NewProgress());
					tasks.Add(task);

					set.ExceptWith(ent);
				}

				Task.WaitAll(tasks.ToArray(), token);
				token.ThrowIfCancellationRequested();

				var procEntries = tasks.SelectMany(t => t.Result.Entries);

				return new EntryOperationResult(procEntries);
			});
		}

		private Task Call(Func<IEntryOperator, bool> pred, Func<IEntryOperator, Task> call, CancellationToken token) {
			var op = this.Where(pred).FirstOrDefault();
			if (op != null) {
				return call(op);
			} else {
				return Task.Run<bool>(() => {
					return false;
				}, token);
			}
		}


		#region IEntryOperator Members

		public IEnumerable<ISystemEntry> CanCopyTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest) {
			return this.Call(entries, (op, set) => op.CanCopyTo(set, dest));
		}

		public Task<IEntryOperationResult> CopyTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest, CancellationToken token, IProgress<double> progress) {
			return this.CallTask(entries, (op, set) => op.CanCopyTo(set, dest), (op, set, prog) => op.CopyTo(set, dest, token, prog), token, progress);
		}

		public IEnumerable<ISystemEntry> CanMoveTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest) {
			return this.Call(entries, (op, set) => op.CanMoveTo(set, dest));
		}

		public Task<IEntryOperationResult> MoveTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest, System.Threading.CancellationToken token, IProgress<double> progress) {
			return this.CallTask(entries, (op, set) => op.CanMoveTo(set, dest), (op, set, prog) => op.MoveTo(set, dest, token, prog), token, progress);
		}

		public IEnumerable<ISystemEntry> CanDelete(IEnumerable<ISystemEntry> entries) {
			return this.Call(entries, (op, set) => op.CanDelete(set));
		}

		public Task<IEntryOperationResult> Delete(IEnumerable<ISystemEntry> entries, bool canUndo, System.Threading.CancellationToken token, IProgress<double> progress) {
			return this.CallTask(entries, (op, set) => op.CanDelete(set), (op, set, prog) => op.Delete(set, canUndo, token, prog), token, progress);
		}

		public bool CanRename(ISystemEntry entry) {
			return this.Any(op => op.CanRename(entry));
		}

		public Task Rename(ISystemEntry entry, string newName, System.Threading.CancellationToken token, IProgress<double> progress) {
			// (op => op.CanRename(entry), op => op.Rename(entry, newName, token, progress), token);
			return this.Call(op => op.CanRename(entry), op => Create(entry, newName, token, progress), token);
			
		}

		public bool CanCreate(ISystemEntry parent) {
			return this.Any(op => op.CanCreate(parent));
		}

		public Task Create(ISystemEntry parent, string newName, CancellationToken token, IProgress<double> progress) {
			return this.Call(op => op.CanCreate(parent), op => Create(parent, newName, token, progress), token);
		}

		public IEnumerable<ISystemEntry> CanOpen(IEnumerable<ISystemEntry> entries) {
			return this.Call(entries, (op, set) => op.CanOpen(set));
		}

		public Task<IEntryOperationResult> Open(IEnumerable<ISystemEntry> entries, CancellationToken token, IProgress<double> progress) {
			return this.CallTask(entries, (op, set) => op.CanOpen(set), (op, set, prog) => op.Open(set, token, prog), token, progress);
		}

		public IEnumerable<ISystemEntry> CanCopyToClipboard(IEnumerable<ISystemEntry> entries) {
			return this.Call(entries, (op, set) => op.CanCopyToClipboard(entries));
		}

		public Task<IEntryOperationResult> CopyToClipboard(IEnumerable<ISystemEntry> entries, CancellationToken token, IProgress<double> progress) {
			return this.CallTask(entries, (op, set) => op.CanCopyToClipboard(entries), (op, set, prog) => op.CopyToClipboard(entries, token, prog), token, progress);
		}

		public IEnumerable<ISystemEntry> CanMoveToClipboard(IEnumerable<ISystemEntry> entries) {
			return this.Call(entries, (op, set) => op.CanMoveToClipboard(entries));
		}

		public Task<IEntryOperationResult> MoveToClipboard(IEnumerable<ISystemEntry> entries, CancellationToken token, IProgress<double> progress) {
			return this.CallTask(entries, (op, set) => op.CanMoveToClipboard(entries), (op, set, prog) => op.MoveToClipboard(entries, token, prog), token, progress);
		}

		public bool CanPasteTo(ISystemEntry dest) {
			return this.Any(op => this.CanPasteTo(dest));
		}

		public Task PasteTo(ISystemEntry dest, CancellationToken token, IProgress<double> progress) {
			return this.Call(op => this.CanPasteTo(dest), op => this.PasteTo(dest, token, progress), token);
		}

		#endregion

		#region ICollection

		public void Add(IEntryOperator item) {
			_Collection.Add(item);
		}

		public void Clear() {
			_Collection.Clear();
		}

		public bool Contains(IEntryOperator item) {
			return _Collection.Contains(item);
		}

		public void CopyTo(IEntryOperator[] array, int arrayIndex) {
			_Collection.CopyTo(array, arrayIndex);
		}

		public bool Remove(IEntryOperator item) {
			return _Collection.Remove(item);
		}

		public IEnumerator<IEntryOperator> GetEnumerator() {
			return _Collection.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return _Collection.GetEnumerator();
		}

		#endregion
	}
}
