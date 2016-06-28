/*
	$Id: T32Point.cs 230 2011-06-23 09:36:13Z catwalkagogo@gmail.com $
*/
using System;
using System.Net;

namespace CatWalk{
	public struct Point<T> : IEquatable<Point<T>>{
		public static readonly Point<T> Empty = new Point<T>();

		public T X{get; private set;}
		public T Y{get; private set;}

		public Point(T x, T y) : this(){
			this.X = x;
			this.Y = y;
		}

		public bool Equals(Point<T> point){
			return this.X.Equals(point.X) && this.Y.Equals(point.Y);
		}

		public override bool Equals(object obj) {
			if(obj == null){
				return false;
			}else if(obj is Point<T>){
				return this.Equals((Point<T>)obj);
			}else{
				return false;
			}
		}

			public static bool operator==(Point<T> a, Point<T> b){
				return a.Equals(b);
			}

			public static bool operator!=(Point<T> a, Point<T> b){
				return !a.Equals(b);
			}

		public override int GetHashCode() {
			return this.X.GetHashCode() ^ this.Y.GetHashCode();
		}

		#region operators

		public static Point<T> operator+(Point<T> a, Point<T> b){
			var calculator = Calculators.GetFor<T>();
			return new Point<T>(
				calculator.Add(a.X, b.X),
				calculator.Add(a.Y, b.Y));
		}

		public static Point<T> operator-(Point<T> a, Point<T> b){
			var calculator = Calculators.GetFor<T>();
			return new Point<T>(
				calculator.Subtract(a.X, b.X),
				calculator.Subtract(a.Y, b.Y));
		}

		public static Point<T> operator*(Point<T> a, Point<T> b){
			var calculator = Calculators.GetFor<T>();
			return new Point<T>(
				calculator.Multiply(a.X, b.X),
				calculator.Multiply(a.Y, b.Y));
		}

		public static Point<T> operator/(Point<T> a, Point<T> b){
			var calculator = Calculators.GetFor<T>();
			return new Point<T>(
				calculator.Divide(a.X, b.X),
				calculator.Divide(a.Y, b.Y));
		}

		public static Point<T> operator%(Point<T> a, Point<T> b){
			var calculator = Calculators.GetFor<T>();
			return new Point<T>(
				calculator.Mod(a.X, b.X),
				calculator.Mod(a.Y, b.Y));
		}

		#endregion

		#region operators with vector

		public static Point<T> operator +(Point<T> a, Vector<T> b) {
			var calculator = Calculators.GetFor<T>();
			return new Point<T>(
				calculator.Add(a.X, b.X),
				calculator.Add(a.Y, b.Y));
		}

		public static Point<T> operator -(Point<T> a, Vector<T> b) {
			var calculator = Calculators.GetFor<T>();
			return new Point<T>(
				calculator.Subtract(a.X, b.X),
				calculator.Subtract(a.Y, b.Y));
		}

		public static Point<T> operator *(Point<T> a, Vector<T> b) {
			var calculator = Calculators.GetFor<T>();
			return new Point<T>(
				calculator.Multiply(a.X, b.X),
				calculator.Multiply(a.Y, b.Y));
		}

		public static Point<T> operator /(Point<T> a, Vector<T> b) {
			var calculator = Calculators.GetFor<T>();
			return new Point<T>(
				calculator.Divide(a.X, b.X),
				calculator.Divide(a.Y, b.Y));
		}

		public static Point<T> operator %(Point<T> a, Vector<T> b) {
			var calculator = Calculators.GetFor<T>();
			return new Point<T>(
				calculator.Mod(a.X, b.X),
				calculator.Mod(a.Y, b.Y));
		}

		#endregion
	}
}
