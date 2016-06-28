using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk {
	public abstract class Calculator<T> : ICalculator<T> where T : IComparable<T> {
		public abstract T Add(T a, T b);
		public abstract T Divide(T a, T b);
		public abstract T Multiply(T a, T b);
		public abstract T Negate(T a);
		public abstract T Mod(T a, T b);

		public virtual T Subtract(T a, T b) {
			return this.Add(a, Negate(b));
		}

		public virtual T Min(T a, T b) {
			return a.CompareTo(b) < 0 ? a : b;
		}
		public virtual T Max(T a, T b) {
			return a.CompareTo(b) > 0 ? a : b;
		}
	}
}
