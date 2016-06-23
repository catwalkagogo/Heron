/*
	$Id: Comparer.cs 316 2013-12-26 10:16:12Z catwalkagogo@gmail.com $
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace CatWalk.Collections{
	public class SelectEqualityComparer<T> : IEqualityComparer<T>{
		private Func<T, object> selector;
		public SelectEqualityComparer(Func<T, object> selector){
			if(selector == null){
				throw new ArgumentNullException("selector");
			}
			this.selector = selector;
		}
		
		public bool Equals(T x, T y){
			return this.selector(x) == this.selector(y);
		}
		
		public int GetHashCode(T obj){
			return this.selector(obj).GetHashCode();
		}
	}

	public class DefaultComparer : IComparer {
		private DefaultComparer() { }

		public int Compare(object x, object y) {
			var comparableX = (IComparable)x;
			return comparableX.CompareTo(y);
		}

		private static DefaultComparer _Default;
		public static DefaultComparer Default {
			get {
				return _Default ?? (_Default = new DefaultComparer());
			}
		}
	}

	public class ReversedComparer : IComparer {
		private IComparer comparer;

		public ReversedComparer(IComparer comparer) {
			if (comparer == null) {
				throw new ArgumentNullException("comparer");
			}
			this.comparer = comparer;
		}

		public int Compare(object x, object y) {
			return -(this.comparer.Compare(x, y));
		}

		public IComparer BaseComparer {
			get {
				return this.comparer;
			}
		}
	}

	public class ReversedComparer<T> : IComparer<T>{
		private IComparer<T> comparer;
		
		public ReversedComparer(IComparer<T> comparer){
			if(comparer == null){
				throw new ArgumentNullException("comparer");
			}
			this.comparer = comparer;
		}
		
		public int Compare(T x, T y){
			return -(this.comparer.Compare(x, y));
		}
		
		public IComparer<T> BaseComparer{
			get{
				return this.comparer;
			}
		}
	}
	
	public class LambdaComparer<T> : IComparer<T>, IComparer{
		private Func<T, T, int> compare;
		
		public LambdaComparer(Func<T, T, int> compare){
			if(compare == null){
				throw new ArgumentNullException("compare");
			}
			this.compare = compare;
		}
		
		public int Compare(object x, object y){
			return this.compare((T)x, (T)y);
		}
		
		public int Compare(T x, T y){
			return this.compare(x, y);
		}
	}
	
	public class KeyValuePairComparer<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>>, IComparer{
		private IComparer<TKey> comparer;
		public KeyValuePairComparer() : this(Comparer<TKey>.Default){
		}
		
		public KeyValuePairComparer(IComparer<TKey> comparer){
			if(comparer == null){
				throw new ArgumentNullException("comparer");
			}
			this.comparer = comparer;
		}
		
		public int Compare(object x, object y){
			return this.Compare((KeyValuePair<TKey, TValue>)x, (KeyValuePair<TKey, TValue>)y);
		}
		public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y){
			return this.comparer.Compare(x.Key, y.Key);
		}
	}
	
	public class CharIgnoreCaseComparer : IComparer<char>, IComparer{
		public int Compare(object x, object y){
			return this.Compare((char)x, (char)y);
		}
		
		public int Compare(char x, char y){
			const char toSmall = (char)('a' - 'A');
			bool xIsLarge = ('A' <= x) && (x <= 'Z');
			bool yIsLarge = ('A' <= y) && (y <= 'Z');
			if(xIsLarge){
				x = (char)(x + toSmall);
			}
			if(yIsLarge){
				y = (char)(y + toSmall);
			}
			return x.CompareTo(y);
		}

		private static WeakReference<CharIgnoreCaseComparer> comparer = null;
		public static CharIgnoreCaseComparer Comparer{
			get{
				CharIgnoreCaseComparer comparer2;
				if(comparer.TryGetTarget(out comparer2)) {
					return comparer2;
				} else {
					comparer2 = new CharIgnoreCaseComparer();
					comparer = new WeakReference<CharIgnoreCaseComparer>(comparer2);
					return comparer2;
				}
			}
		}
	}
	
	public class MergedComparer<T> : IComparer<T>, IComparer{
		IComparer<T> _Primary;
		IComparer<T> _Secondary;
		
		public MergedComparer(IComparer<T> primary, IComparer<T> secondary){
			primary.ThrowIfNull("primary");
			secondary.ThrowIfNull("secondary");
			this._Primary = primary;
			this._Secondary = secondary;
		}
		
		public int Compare(object x, object y){
			return this.Compare((T)x, (T)y);
		}
		
		public int Compare(T x, T y){
			var d = this._Primary.Compare(x, y);
			return (d == 0) ? this._Secondary.Compare(x, y) : d;
		}
	}
}