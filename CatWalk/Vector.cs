/*
	$Id: T32Vector.cs 230 2011-06-23 09:36:13Z catwalkagogo@gmail.com $
*/
using System;

namespace CatWalk{
	public struct Vector<T> : IEquatable<Vector<T>>{
		public static readonly Vector<T> Empty = new Vector<T>();

		public T X{get; private set;}
		public T Y{get; private set;}

		public Vector(T x, T y) : this(){
			this.X = x;
			this.Y = y;
		}

		public bool Equals(Vector<T> poT){
			return this.X.Equals(poT.X) && this.Y.Equals(poT.Y);
		}

		public override bool Equals(object obj) {
			if(obj == null){
				return false;
			}else if(obj is Vector<T>){
				return this.Equals((Vector<T>)obj);
			}else{
				return false;
			}
		}

			public static bool operator==(Vector<T> a, Vector<T> b){
				return a.Equals(b);
			}

			public static bool operator!=(Vector<T> a, Vector<T> b){
				return !a.Equals(b);
			}

		public override int GetHashCode() {
			return this.X.GetHashCode() ^ this.Y.GetHashCode();
		}

		#region operator

		public static Vector<T> operator +(Vector<T> a, Vector<T> b) {
			var calculator = Calculators.GetFor<T>();
			return new Vector<T>(
				calculator.Add(a.X, b.X),
				calculator.Add(a.Y, b.Y));
		}

		public static Vector<T> operator -(Vector<T> a, Vector<T> b) {
			var calculator = Calculators.GetFor<T>();
			return new Vector<T>(
				calculator.Subtract(a.X, b.X),
				calculator.Subtract(a.Y, b.Y));
		}

		public static Vector<T> operator *(Vector<T> a, Vector<T> b) {
			var calculator = Calculators.GetFor<T>();
			return new Vector<T>(
				calculator.Multiply(a.X, b.X),
				calculator.Multiply(a.Y, b.Y));
		}

		public static Vector<T> operator /(Vector<T> a, Vector<T> b) {
			var calculator = Calculators.GetFor<T>();
			return new Vector<T>(
				calculator.Divide(a.X, b.X),
				calculator.Divide(a.Y, b.Y));
		}

		public static Vector<T> operator %(Vector<T> a, Vector<T> b) {
			var calculator = Calculators.GetFor<T>();
			return new Vector<T>(
				calculator.Mod(a.X, b.X),
				calculator.Mod(a.Y, b.Y));
		}

		#endregion
	}
}
