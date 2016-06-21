using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CatWalk.Collections {
	public static class Extensions {
		#region readonly

		public static IReadOnlyObservableCollection<TValue> AsReadOnlyCollection<TValue>(this IReadOnlyCollection<TValue> source) {
				return new WrappedReadOnlyObservableCollection<TValue>(source);
		}

		public static IReadOnlyObservableList<TValue> AsReadOnlyList<TValue>(this IReadOnlyList<TValue> source) {
				return new WrappedReadOnlyObservableList<TValue>((IReadOnlyList<TValue>)source);
		}


		#endregion
	}
}
