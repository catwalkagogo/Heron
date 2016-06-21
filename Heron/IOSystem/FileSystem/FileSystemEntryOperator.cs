using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk.IOSystem;
using CatWalk.Win32.Shell;
using CatWalk.IOSystem.FileSystem;
using System.Windows;
using System.Collections.Specialized;

namespace CatWalk.Heron.IOSystem.FileSystem {
	public class FileSystemEntryOperator : IEntryOperator {
		public IEnumerable<ISystemEntry> CanCopyTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest) {
			return entries.Where(entry => dest is IFileSystemEntry || entry is IFileSystemEntry);
		}

		public IEnumerable<ISystemEntry> CanCopyToClipboard(IEnumerable<ISystemEntry> entries) {
			return entries.Where(entry => entry is IFileSystemEntry);
		}

		public IEnumerable<ISystemEntry> CanCreate(ISystemEntry parent) {
			return Seq.Make(parent).Where(entry => entry is IFileSystemEntry);
		}

		public IEnumerable<ISystemEntry> CanDelete(IEnumerable<ISystemEntry> entries) {
			return entries.Where(entry => entry is FileSystemEntry);
		}

		public IEnumerable<ISystemEntry> CanMoveTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest) {
			return entries.Where(entry => dest is IFileSystemEntry || entry is IFileSystemEntry);
		}

		public IEnumerable<ISystemEntry> CanMoveToClipboard(IEnumerable<ISystemEntry> entries) {
			return entries.Where(entry => entry is IFileSystemEntry);
		}

		public IEnumerable<ISystemEntry> CanOpen(IEnumerable<ISystemEntry> entries) {
			return entries.Where(entry => entry is IFileSystemEntry);
		}

		public IEnumerable<ISystemEntry> CanPasteTo(ISystemEntry dest) {
			return Seq.Make(dest).Where(entry => entry is IFileSystemEntry && Clipboard.ContainsFileDropList());
		}

		public IEnumerable<ISystemEntry> CanRename(ISystemEntry entry) {
			return Seq.Make(entry).Where(e => e is FileSystemEntry);
		}

		private Task<IEntryOperationResult> CreateFileOperationTask(IEnumerable<ISystemEntry> entries, CancellationToken token, IProgress<double> progress, Action<FileOperation> op) {
			var tcs = new TaskCompletionSource<IEntryOperationResult>();

			token.ThrowIfCancellationRequested();

			var ops = new FileOperation();
			ops.ProgressSink.Completed += (s, e) => {
				if (ops.IsOperationAborted) {
					tcs.SetCanceled();
				} else {
					tcs.SetResult(new EntryOperationResult(entries));
				}
			};
			ops.ProgressSink.ProgressChanged += (s, e) => {
				if (progress != null) {
					progress.Report(e.Progress);
				}
			};

			return tcs.Task;
		}

		public Task<IEntryOperationResult> CopyTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest, CancellationToken token, IProgress<double> progress) {
			entries = this.CanCopyTo(entries, dest).ToArray();
			var files = entries.Cast<IFileSystemEntry>().Select(fse => fse.FileSystemPath.FullPath).ToArray();
			var destPath = ((IFileSystemEntry)dest).FileSystemPath.FullPath;

			return this.CreateFileOperationTask(entries, token, progress, (ops) => {
				ops.Copy(files, destPath);
			});
		}

		public Task<IEntryOperationResult> MoveTo(IEnumerable<ISystemEntry> entries, ISystemEntry dest, CancellationToken token, IProgress<double> progress) {
			entries = this.CanMoveTo(entries, dest).ToArray();
			var files = entries.Cast<IFileSystemEntry>().Select(fse => fse.FileSystemPath.FullPath).ToArray();
			var destPath = ((IFileSystemEntry)dest).FileSystemPath.FullPath;

			return this.CreateFileOperationTask(entries, token, progress, (ops) => {
				ops.Move(files, destPath);
			});
		}

		public Task<IEntryOperationResult> Delete(IEnumerable<ISystemEntry> entries, bool canUndo, CancellationToken token, IProgress<double> progress) {
			entries = this.CanDelete(entries).ToArray();
			var files = entries.Cast<IFileSystemEntry>().Select(fse => fse.FileSystemPath.FullPath).ToArray();

			return this.CreateFileOperationTask(entries, token, progress, (ops) => {
				ops.Delete(files);
			});
		}

		public Task<IEntryOperationResult> Rename(ISystemEntry entry, string newName, CancellationToken token, IProgress<double> progress) {
			var entries = this.CanRename(entry).ToArray();
			var files = entries.Cast<IFileSystemEntry>().Select(fse => fse.FileSystemPath.FullPath).ToArray();

			return this.CreateFileOperationTask(entries, token, progress, (ops) => {
				ops.Rename(files[0], newName);
			});
		}

		public Task<IEntryOperationResult> Create(ISystemEntry parent, string newName, CancellationToken token, IProgress<double> progress) {
			var entries = this.CanCreate(parent).Cast<IFileSystemEntry>().ToArray();
			var parentEntry = entries.FirstOrDefault();

			if (parentEntry != null) {
				return this.CreateFileOperationTask(entries, token, progress, (ops) => {
					ops.Create(parentEntry.FileSystemPath.FullPath, newName, System.IO.FileAttributes.Normal);
				});
			}else {
				throw new ArgumentException("parent");
			}
		}

		public Task<IEntryOperationResult> Open(IEnumerable<ISystemEntry> entries, CancellationToken token, IProgress<double> progress) {
			entries = this.CanOpen(entries).ToArray();
			var files = entries.Cast<IFileSystemEntry>().Select(fse => fse.FileSystemPath.FullPath).ToArray();

			return Task.Run<IEntryOperationResult>(() => {
				FileOperations.ExecuteDefaultAction(IntPtr.Zero, files);

				return new EntryOperationResult(entries);
			});
		}

		public Task<IEntryOperationResult> CopyToClipboard(IEnumerable<ISystemEntry> entries, CancellationToken token, IProgress<double> progress) {
			entries = this.CanCopyToClipboard(entries).ToArray();
			var files = entries.Cast<IFileSystemEntry>().Select(fse => fse.FileSystemPath.FullPath).ToArray();

			return Task.Run<IEntryOperationResult>(() => {
				var data = new DataObject();
				var fileList = new StringCollection();
				fileList.AddRange(files);
				data.SetFileDropList(fileList);
				data.SetData("Preferred DropEffect", DragDropEffects.Copy);


				return new EntryOperationResult(entries);
			});
		}

		public Task<IEntryOperationResult> MoveToClipboard(IEnumerable<ISystemEntry> entries, CancellationToken token, IProgress<double> progress) {
			entries = this.CanMoveToClipboard(entries).ToArray();
			var files = entries.Cast<IFileSystemEntry>().Select(fse => fse.FileSystemPath.FullPath).ToArray();

			return Task.Run<IEntryOperationResult>(() => {
				var data = new DataObject();
				var fileList = new StringCollection();
				fileList.AddRange(files);
				data.SetFileDropList(fileList);
				data.SetData("Preferred DropEffect", DragDropEffects.Move);


				return new EntryOperationResult(entries);
			});
		}

		public Task<IEntryOperationResult> PasteTo(ISystemEntry dest, CancellationToken token, IProgress<double> progress) {
			var entries = this.CanPasteTo(dest).ToArray();
			var files = entries.Cast<IFileSystemEntry>().Select(fse => fse.FileSystemPath.FullPath).ToArray();
			var destPath = files.FirstOrDefault();

			if(destPath == null) {
				throw new ArgumentException("dest");
			}

			return Task.Run<IEntryOperationResult>(() => {
				var fileList = Clipboard.GetFileDropList().Cast<string>().ToArray();
				var dropEffect = (DragDropEffects)Clipboard.GetData("Preferred DropEffect");

				Action<FileOperation> proc = null;
				if((dropEffect & DragDropEffects.Copy) > 0) {
					proc = op => {
						op.Copy(fileList, destPath);
					};
				}else if((dropEffect & DragDropEffects.Move) > 0) {
					proc = op => {
						op.Move(fileList, destPath);
					};
				}
				if (proc != null) {
					this.CreateFileOperationTask(entries, token, progress, proc).Wait();
				}

				return new EntryOperationResult(entries);
			});
		}
	}
}
