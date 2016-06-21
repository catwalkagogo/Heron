using System;
using System.Threading;
using CatWalk.IOSystem;

namespace CatWalk.Heron.IOSystem {
	public interface IColumnDefinition {
		string DisplayName {
			get;
		}
		object GetValue(ISystemEntry entry);
		object GetValue(ISystemEntry entry, bool noCache);
		object GetValue(ISystemEntry entry, bool noCache, CancellationToken token);
		string Name {
			get;
		}
	}

	public interface IColumnDefinition<out T> : IColumnDefinition{
		new T GetValue(ISystemEntry entry);
		new T GetValue(ISystemEntry entry, bool noCache);
		new T GetValue(ISystemEntry entry, bool noCache, CancellationToken token);
	}
}
