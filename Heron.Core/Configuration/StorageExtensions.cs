using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CatWalk.Heron.Configuration {
	public static class StorageExtensions {
		public static Task<T> GetAsync<T>(this IStorage storage, string key) {
			return storage.GetAsync(key, default(T), CancellationToken.None);
		}

		public static Task<T> GetAsync<T>(this IStorage storage, string key, T def) {
			return storage.GetAsync(key, def, CancellationToken.None);
		}

		public static Task<T> GetAsync<T>(this IStorage storage, string key, T def, CancellationToken token) {
			storage.ThrowIfNull(nameof(storage));
			return Task.Run<T>(() => {
				return storage.Get(key, def);
			}, token);
		}

		public static Task<IReadOnlyDictionary<string, object>> GetAllAsync(this IStorage storage, string[] keys) {
			return storage.GetAllAsync(keys, CancellationToken.None);
		}

		public static async Task<IReadOnlyDictionary<string, object>> GetAllAsync(this IStorage storage, string[] keys, CancellationToken token) {
			storage.ThrowIfNull(nameof(storage));
			keys.ThrowIfNull(nameof(keys));

			var tasks = keys.Select(key => new KeyValuePair<string, Task<object>>(key, storage.GetAsync<object>(key, null))).ToArray();
			Task.WaitAll(tasks.Select(p => p.Value).ToArray());

			return new ReadOnlyDictionary<string, object>(tasks.ToDictionary(p => p.Key, p => p.Value.Result));
		}

		public static Task SetAsync<T>(this IStorage storage, string key, T value) {
			return storage.SetAsync(key, value, CancellationToken.None);
		}

		public static Task SetAsync<T>(this IStorage storage, string key, T value, CancellationToken token) {
			storage.ThrowIfNull(nameof(storage));
			return Task.Run(() => {
				storage[key] = value;
			}, token);
		}
	}
}
