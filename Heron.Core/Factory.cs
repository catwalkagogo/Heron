using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron {
	public class Factory<TKey, TValue> {
		private IDictionary<TKey, Delegate> _Creaters = new Dictionary<TKey, Delegate>();

		public void Register(TKey key, Delegate d) {
			key.ThrowIfNull("key");
			d.ThrowIfNull("d");
			this._Creaters.Add(key, d);
		}

		public TValue Create(TKey key, params object[] args) {
			key.ThrowIfNull("key");
			Delegate d;
			TValue v;
			if(this._Creaters.TryGetValue(key, out d)) {
				var prms = Seq.Make((object)key).Concat(args.EmptyIfNull()).ToArray();
				v = (TValue)d.DynamicInvoke(prms);
			} else {
				v = default(TValue);
			}
			this.OnCreated(new FactoryValueCreatedEventArgs<TKey, TValue>(key, v, args));
			return v;
		}

		public event EventHandler Created;

		protected virtual void OnCreated(FactoryValueCreatedEventArgs<TKey, TValue> e) {
			var handler = this.Created;
			if(handler != null) {
				handler(this, e);
			}
		}
	}

	public class FactoryValueCreatedEventArgs<TKey, TValue> : EventArgs {
		public TKey Key { get; private set; }
		public TValue Value { get; private set; }
		public IReadOnlyList<object> Parameters { get; private set; }

		public FactoryValueCreatedEventArgs(TKey key, TValue value, object[] parameters) {
			this.Key = key;
			this.Value = value;
			this.Parameters = parameters;
		}
	}
}
