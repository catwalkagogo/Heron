using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk {
	public interface ICalculator<T> {
		T Divide(T a, T b);
		T Multiply(T a, T b);
		T Add(T a, T b);
		T Negate(T a);
		T Subtract(T a, T b);
		T Mod(T a, T b);
		T Min(T a, T b);
		T Max(T a, T b);
	}
}
