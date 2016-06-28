/*
	$Id: Messenger.cs 195 2011-04-12 08:27:58Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Reactive.Linq;
using CatWalk;

namespace CatWalk.Mvvm {
	using TEntryKey = Type;
	using TEntryValue = Messenger.Entry;
	using TEntryList = LinkedList<Messenger.Entry>;
	using TDictionary = Dictionary<Type, LinkedList<Messenger.Entry>>;

	public class Messenger {
		private readonly object _Sync = new object();

		private static Messenger _Default;
		public static Messenger Default {
			get {
				return _Default ?? (_Default = new Messenger());
			}
		}

		private TDictionary _DerivedEntries;
		private TDictionary DerivedEntries {
			get {
				return this._DerivedEntries ?? (this._DerivedEntries = new TDictionary());
			}
		}

		private TDictionary _StrictEntries;
		private TDictionary StrictEntries {
			get {
				return this._StrictEntries ?? (this._StrictEntries = new TDictionary());
			}
		}

		private SynchronizationContext _SynchronizationContext;
		public SynchronizationContext SynchronizationContext {
			get {
				return this._SynchronizationContext;
			}
			set {
				value.ThrowIfNull("value");
				this._SynchronizationContext = value;
			}
		}

		public Messenger() : this(SynchronizeViewModel.DefaultSynchronizationContext.Default) {

		}

		public Messenger(SynchronizationContext invoke) {
			invoke.ThrowIfNull("invoke");
			this._SynchronizationContext = invoke;
		}

		#region Subscribe

		public IDisposable Subscribe<TMessage>(Func<TMessage, Task> action) {
			return this.Subscribe(action, null, false);
		}
		public IDisposable Subscribe<TMessage>(Func<TMessage, Task> action, object token) {
			return this.Subscribe(action, token, false);
		}
		public IDisposable Subscribe<TMessage>(Func<TMessage, Task> action, bool isReceiveDerivedMessages) {
			return this.Subscribe(action, null, isReceiveDerivedMessages);
		}
		public IDisposable Subscribe<TMessage>(Func<TMessage, Task> action, object token, bool isReceiveDerivedMessages) {
			return this.SubscribeInternal<TMessage>(action, token, isReceiveDerivedMessages, true, false);
		}
		private IDisposable SubscribeInternal<TMessage>(Delegate action, object token, bool isReceiveDerivedMessages, bool isTask, bool isPassToken) {
			action.ThrowIfNull("action");
			lock (this._Sync) {
				var messageType = typeof(TMessage);
				var entry = new TEntryValue(this, new WeakDelegate(action), token, isTask, isReceiveDerivedMessages, messageType, isPassToken);
				// get list
				var entries = (isReceiveDerivedMessages) ? this.DerivedEntries : this.StrictEntries;
				TEntryList list;
				var key = messageType;
				if (!entries.TryGetValue(key, out list)) {
					list = new TEntryList();
					entries.Add(key, list);
				}

				list.AddLast(entry);
				return entry;
			}
		}

		public IDisposable Subscribe<TMessage>(Action<TMessage> action) {
			return this.Subscribe(action, null, false);
		}
		public IDisposable Subscribe<TMessage>(Action<TMessage> action, object token) {
			return this.Subscribe(action, token, false);
		}
		public IDisposable Subscribe<TMessage>(Action<TMessage> action, bool isReceiveDerivedMessages) {
			return this.Subscribe(action, null, isReceiveDerivedMessages);
		}
		public IDisposable Subscribe<TMessage>(Action<TMessage> action, object token, bool isReceiveDerivedMessages) {
			return this.SubscribeInternal<TMessage>(action, token, isReceiveDerivedMessages, false, false);
		}

		public IDisposable Subscribe<TMessage>(Action<TMessage, object> action) {
			return this.Subscribe(action, null, false);
		}
		public IDisposable Subscribe<TMessage>(Action<TMessage, object> action, object token) {
			return this.Subscribe(action, token, false);
		}
		public IDisposable Subscribe<TMessage>(Action<TMessage, object> action, bool isReceiveDerivedMessages) {
			return this.Subscribe(action, null, isReceiveDerivedMessages);
		}
		public IDisposable Subscribe<TMessage>(Action<TMessage, object> action, object token, bool isReceiveDerivedMessages) {
			return this.SubscribeInternal<TMessage>(action, token, isReceiveDerivedMessages, false, true);
		}

		public IDisposable Subscribe<TMessage>(Func<TMessage, object, Task> action) {
			return this.Subscribe(action, null, false);
		}
		public IDisposable Subscribe<TMessage>(Func<TMessage, object, Task> action, object token) {
			return this.Subscribe(action, token, false);
		}
		public IDisposable Subscribe<TMessage>(Func<TMessage, object, Task> action, bool isReceiveDerivedMessages) {
			return this.Subscribe(action, null, isReceiveDerivedMessages);
		}
		public IDisposable Subscribe<TMessage>(Func<TMessage, object, Task> action, object token, bool isReceiveDerivedMessages) {
			return this.SubscribeInternal<TMessage>(action, token, isReceiveDerivedMessages, true, true);
		}

		#endregion

		#region Send

		public void Send<TMessage>(TMessage message) {
			this.Send(message, null);
		}

		public void Send<TMessage>(TMessage message, object token) {
			foreach (var list in this.FindEntries(typeof(TMessage))) {
				this.ProcessEntryList(list, token, (entry, d) => {
					this._SynchronizationContext.Send(new SendOrPostCallback(state => {
						d.DynamicInvoke(entry.GetParameters(message, token));
					}), null);
				});
			}
		}

		private IEnumerable<TEntryList> FindEntries(Type messageType) {
			// derived
			if (this._DerivedEntries != null) {
				var keysToDelete = new List<TEntryKey>();
				foreach (var pair in this._DerivedEntries
					.Where(pair => messageType.GetTypeInfo().IsSubclassOf(pair.Key))) {
					var list = pair.Value;
					yield return list;
					if (list.Count == 0) {
						keysToDelete.Add(pair.Key);
					}
				}
				foreach (var key in keysToDelete) {
					this._DerivedEntries.Remove(key);
				}
			}

			// strict
			if (this._StrictEntries != null) {
				var key = messageType;
				TEntryList list;
				if (this._StrictEntries.TryGetValue(key, out list)) {
					yield return list;
					if (list.Count == 0) {
						this._StrictEntries.Remove(key);
					}
				}
			}
		}

		private void ProcessEntryList(TEntryList list, object token, Action<Entry, Delegate> callback) {
			var node = list.First;
			while (node != null) {
				var next = node.Next;
				var entry = node.Value;
				var target = entry.Action.Target.Target;
				var d = entry.Action.Delegate;
				if (target != null) {
					if (entry.IsMatchToken(token)) {
						callback(entry, d);
					}
				} else {
					list.Remove(node);
				}
				node = next;
			}
		}

		#endregion

		#region Post

		public Task Post<TMessage>(TMessage message) {
			return this.Post(message, null);
		}

		public Task Post<TMessage>(TMessage message, object token) {
			return Task.Run(() => {
				var tasks = new List<Task>();
				foreach (var list in this.FindEntries(typeof(TMessage))) {
					this.ProcessEntryList(list, token, (entry, d) => {
						Task task = null;
						if (entry.IsTask) {
							this._SynchronizationContext.Send(state => {
								task = (Task)d.DynamicInvoke(entry.GetParameters(message, token));
							}, null);
						} else {
							task = Task.Run(() => {
								this._SynchronizationContext.Send(state => {
									d.DynamicInvoke(entry.GetParameters(message, token));
								}, null);
							});
						}
						if(task != null) {
							tasks.Add(task);
						}
					});
				}

				Task.WaitAll(tasks.ToArray());
			});
		}

		#endregion

		#region Entry

		internal class Entry : IDisposable {
			public Messenger Messenger { get; private set; }
			public WeakDelegate Action { get; private set; }
			public WeakReference Token { get; private set; }
			public bool IsTask { get; private set; }
			public bool IsReceiveDerivedMessages{ get; private set; }
			public Type MessageType { get; private set; }
			public bool IsPassToken { get; private set; }

			public Entry(Messenger messenger, WeakDelegate action, object token, bool isTask, bool isReceiveDerivedMessages, Type messageType, bool isPassToken) {
				this.Messenger = messenger;
				this.Action = action;
				this.Token = (token != null) ? new WeakReference(token) : null;
				this.IsTask = isTask;
				this.IsReceiveDerivedMessages = isReceiveDerivedMessages;
				this.MessageType = messageType;
				this.IsPassToken = isPassToken;
			}

			public object[] GetParameters(object message, object token) {
				return this.IsPassToken ? new object[] { message, token} : new object[] { message };
			}

			public bool IsMatchToken(object token) {
				return token == null || this.Token == null || (this.Token.IsAlive && this.Token.Target == token);
			}

			public void Dispose() {
				lock (this.Messenger._Sync) {
					var dict = this.IsReceiveDerivedMessages ? this.Messenger._DerivedEntries : this.Messenger._StrictEntries;
					TEntryList list;
					if(dict.TryGetValue(MessageType, out list)) {
						list.Remove(this);
					}
				}
			}
		}

		#endregion

	}

	public static class MessengerExtensions {
		public static IObservable<T> ToObservable<T>(this Messenger messenger, object token = null, bool isReceiveDerivedMessages = false) {
			return Observable.Create<T>(observer => {
				var sync = new object();
				return messenger.Subscribe<T>(m => {
					lock (sync) {
						observer.OnNext(m);
					}
				}, token, isReceiveDerivedMessages);
			});
		}

		public static IObservable<MessageTokenSet<T>> ToObservableWithToken<T>(this Messenger messenger, object token = null, bool isReceiveDerivedMessages = false) {
			return Observable.Create<MessageTokenSet<T>>(observer => {
				var sync = new object();
				return messenger.Subscribe<T>((m, t) => {
					lock (sync) {
						observer.OnNext(new MessageTokenSet<T>(m, t));
					}
				}, token, isReceiveDerivedMessages);
			});
		}
	}

	public struct MessageTokenSet<T> {
		public T Message { get; private set; }
		public object Token { get; private set; }
		public MessageTokenSet(T message, object token) {
			this.Message = message;
			this.Token = token;
		}
	}
}