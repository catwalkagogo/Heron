using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk.Collections;

namespace CatWalk.IOSystem.FileSystem.Win32 {
	using IO = System.IO;
	public class Win32FileSystemWatcher : IIOSystemWatcher{
		private Lazy<IO::FileSystemWatcher> _Watcher;
		private IFileSystemEntry _Target;
		private Task _NotifyTask;
		private object _SyncObject = new Object();
		private const int DelayTime = 1000;
		private Queue<IO::FileSystemEventArgs> _EventQueue = new Queue<IO.FileSystemEventArgs>();
		private CancellationTokenSource _TokenSource = new CancellationTokenSource();

		public Win32FileSystemWatcher(IFileSystemEntry dir) {
			this._Target = dir;
			this._Watcher = new Lazy<IO.FileSystemWatcher>(this.WatcherFactory);
		}

		private IO::FileSystemWatcher WatcherFactory() {
			var watcher = new IO::FileSystemWatcher(this._Target.FileSystemPath.FullPath);
			watcher.IncludeSubdirectories = false;
			watcher.InternalBufferSize = 1024 * 8;
			watcher.Created += _Watcher_Created;
			watcher.Changed += _Watcher_Changed;
			watcher.Renamed += _Watcher_Renamed;
			watcher.Deleted += _Watcher_Deleted;
			return watcher;
		}

		#region Notify

		private void EnqueueEvent(IO::FileSystemEventArgs e) {
			lock(this._EventQueue) {
				this._EventQueue.Enqueue(e);
				if(this._NotifyTask == null || this._NotifyTask.IsCanceled || this._NotifyTask.IsCompleted || this._NotifyTask.IsFaulted) {
					// not started
					this._NotifyTask = Task
						.Delay(DelayTime, this._TokenSource.Token)
						.ContinueWith(this.NotifyTaskProcess, this._TokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
				}
			}
		}

		private struct NotifyItem : IEquatable<NotifyItem>{
			public int Index { get; private set; }
			public string Path { get; private set; }
			public NotifyItem(int index, string path) : this(){
				this.Index = index;
				this.Path = path;
			}

			public override bool Equals(object obj) {
				if(obj is NotifyItem){
					return this.Path.Equals((NotifyItem)obj);
				}else{
					return base.Equals(obj);
				}
			}

			public bool Equals(NotifyItem item) {
				return this.Path.Equals(item.Path);
			}

			public override int GetHashCode() {
				return this.Path.GetHashCode();
			}
		}

		private void NotifyTaskProcess(Task task) {
			var idx = 0;
			var addItems = new HashSet<NotifyItem>();
			var removeItems = new HashSet<NotifyItem>();
			var replaceItems = new Dictionary<string, NotifyItem>();
			lock(this._EventQueue) {
				while(this._EventQueue.Count > 0) {
					var ev = this._EventQueue.Dequeue();
					var item = new NotifyItem(idx, ev.FullPath);
					switch(ev.ChangeType) {
						case IO.WatcherChangeTypes.Changed:
							if(!addItems.Contains(item)) {
								if(!removeItems.Contains(item)) {
									replaceItems.Add(ev.FullPath, item);
								}
							}
							break;
						case IO.WatcherChangeTypes.Created:
							addItems.Add(item);
							removeItems.Remove(item);
							break;
						case IO.WatcherChangeTypes.Deleted:
							addItems.Remove(item);
							replaceItems.Remove(ev.FullPath);
							removeItems.Add(item);
							break;
						case IO.WatcherChangeTypes.Renamed:
							var rev = (IO::RenamedEventArgs)ev;
							var oldItem = new NotifyItem(idx, rev.OldFullPath);
							addItems.Remove(oldItem);
							removeItems.Remove(oldItem);
							replaceItems.Add(rev.OldFullPath, item);
							break;
					}
					idx++;
				}

				var events = new SortedDictionary<int, NotifyCollectionChangedEventArgs>();
				foreach(var item in addItems) {
					events.Add(item.Index, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this.CreateEntry(item.Path)));
				}
				foreach(var pair in replaceItems) {
					events.Add(pair.Value.Index, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, this.CreateEntry(pair.Key), this.CreateEntry(pair.Value.Path)));
				}
				foreach(var item in removeItems) {
					events.Add(item.Index, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, this.CreateEntry(item.Path)));
				}

				lock(this._SyncObject) {
					if(this.IsEnabled) {
						var handler = this.CollectionChanged;
						foreach(var ev in events.Values) {
							handler(this, ev);
						}
					}
				}

			}
		}

		#endregion

		private Win32FileSystemEntry CreateEntry(string path) {
			return new Win32FileSystemEntry(this._Target, IO::Path.GetFileName(path), path);
		}

		private void _Watcher_Deleted(object sender, IO.FileSystemEventArgs e) {
			this.EnqueueEvent(e);
		}

		private void _Watcher_Renamed(object sender, IO.RenamedEventArgs e) {
			this.EnqueueEvent(e);
		}

		private void _Watcher_Changed(object sender, IO.FileSystemEventArgs e) {
			this.EnqueueEvent(e);
		}

		private void _Watcher_Created(object sender, IO.FileSystemEventArgs e) {
			this.EnqueueEvent(e);
		}

		public bool IsEnabled {
			get {
				return this._Watcher.Value.EnableRaisingEvents;
			}
			set {
				lock(this._SyncObject) {
					this._Watcher.Value.EnableRaisingEvents = value;
					this._TokenSource.Cancel();
					this._TokenSource = new CancellationTokenSource();
				}
			}
		}

		public ISystemEntry Target {
			get {
				return this._Target;
			}
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}
