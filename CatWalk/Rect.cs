/*
	$Id: Int32Rect.cs 316 2013-12-26 10:16:12Z catwalkagogo@gmail.com $
*/
using System;
using System.Runtime.Serialization;

namespace CatWalk{
	[DataContract]
	public struct Rect<T> : IEquatable<Rect<T>> where T : IComparable<T>{
		[DataMember]
		public T X{get; private set;}
		[DataMember]
		public T Y{get; private set;}
		[DataMember]
		public T Width{get; private set;}
		[DataMember]
		public T Height{get; private set;}

		public T Left{get{return this.X;}}
		public T Top{get{return this.Y;}}
		public T Right{
			get {
				var c = Calculators.GetFor<T>();
				return c.Add(this.X, this.Width);
			}
		}
		public T Bottom{
			get {
				var c = Calculators.GetFor<T>();
				return c.Add(this.Y, this.Height);
			}
		}

		public Rect(T x, T y, T width, T height) : this(){
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
		}

		#region Equals

		public bool Equals(Rect<T> rect){
			return this.X.Equals(rect.X) && this.Y.Equals(rect.Y) && this.Width.Equals(rect.Width) && this.Height.Equals(this.Height);
		}

		public override bool Equals(object obj) {
			if(obj == null){
				return false;
			}else if(obj is Rect<T>){
				return this.Equals((Rect<T>)obj);
			}else{
				return false;
			}
		}

		public override int GetHashCode() {
			return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Width.GetHashCode() ^ this.Height.GetHashCode();
		}

		#endregion

		#region Operators

		public static bool operator ==(Rect<T> a, Rect<T> b) {
			return a.Equals(b);
		}

		public static bool operator !=(Rect<T> a, Rect<T> b) {
			return !a.Equals(b);
		}
		
		public static Rect<T> operator +(Rect<T> a, Vector<T> v) {
			var calculator = Calculators.GetFor<T>();
			return new Rect<T>(calculator.Add(a.Left, v.X), calculator.Add(a.Top, v.Y), a.Width, a.Height);
		}

		public static Rect<T> operator +(Vector<T> v, Rect<T> a) {
			var calculator = Calculators.GetFor<T>();
			return new Rect<T>(calculator.Add(a.Left, v.X), calculator.Add(a.Top, v.Y), a.Width, a.Height);
		}

		#endregion

		#region Contains
		
		public bool Contains(Point<T> pt) {
			return this.ContainsInternal(pt.X, pt.Y);
		}
		
		public bool Contains(T x, T y) {
			return this.ContainsInternal(x, y);
		}

		public bool Contains(Rect<T> rect) {
			var c = Calculators.GetFor<T>();
			return this.X.CompareTo(rect.X) <= 0 &&
					this.Y.CompareTo(rect.Y) <= 0 &&
					c.Add(this.X, this.Width).CompareTo(c.Add(rect.X, rect.Width)) >= 0 &&
					c.Add(this.Y, this.Height).CompareTo(c.Add(rect.Y, rect.Height)) >= 0;
		} 

		#endregion

		#region Intersect

		public bool IsIntersect(Rect<T> rect) {
			return !(this.Left.CompareTo(rect.Right) > 0 || this.Right.CompareTo(rect.Left) < 0 || this.Top.CompareTo(rect.Bottom) > 0 || this.Bottom.CompareTo(rect.Top) < 0);
		}

		public Rect<T> Intersect(Rect<T> b){
			var a = this;

			var c = Calculators.GetFor<T>();

			var x1 = c.Max(a.X, b.X);
			var x2 = c.Min(c.Add(a.X, a.Width), c.Add(b.X, b.Width));
			var y1 = c.Max(a.Y, b.Y);
			var y2 = c.Min(c.Add(a.Y, a.Height), c.Add(b.Y, b.Height));

			if (x2.CompareTo(x1) >= 0 && y2.CompareTo(y1) >= 0) {

				return new Rect<T>(x1, y1, c.Subtract(x2, x1), c.Subtract(y2, y1));
			}
			return Empty;
		}

		#endregion

		#region Property
		public T Area {
			get {
				var c = Calculators.GetFor<T>();
				return c.Multiply(this.Width, this.Height);
			}
		}

		public static readonly Rect<T> Empty = new Rect<T>();

		public bool IsEmpty {
			get {
				return Empty.Equals(this);
			}
		}

		#endregion	

		public Rect<T> Union(Rect<T> b) {
			var a = this;
			var c = Calculators.GetFor<T>();

			var x1 = c.Min(a.X, b.X);
			var x2 = c.Max(c.Add(a.X, a.Width), c.Add(b.X, b.Width));
			var y1 = c.Min(a.Y, b.Y);
			var y2 = c.Max(c.Add(a.Y, a.Height), c.Add(b.Y, b.Height));

			return new Rect<T>(x1, y1, c.Subtract(x2, x1), c.Subtract(y2, y1));
		}

		private bool ContainsInternal(T x, T y) {
			var c = Calculators.GetFor<T>();

			return ((x.CompareTo(this.X) >= 0) && (c.Subtract(x, this.Width).CompareTo(this.X) <= 0) &&
					(y.CompareTo(this.Y) >= 0) && (c.Subtract(y, this.Height).CompareTo(this.Y) <=  0));
		}
	}
}
