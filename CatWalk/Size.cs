/*
	$Id: T32Size.cs 230 2011-06-23 09:36:13Z catwalkagogo@gmail.com $
*/
using System;

namespace CatWalk{
	public struct Size<T> : IEquatable<Size<T>>{
		public static readonly Size<T> Empty = new Size<T>();

		public T Width{get; private set;}
		public T Height{get; private set;}

		public Size(T width, T height) : this(){
			this.Width = width;
			this.Height = height;
		}

		public bool Equals(Size<T> size){
			return this.Width.Equals(size.Width) && this.Height.Equals(size.Height);
		}

		public override bool Equals(object obj) {
			if(obj == null){
				return false;
			}else if(obj is Size<T>) {
				return this.Equals((Size<T>)obj);
			}else{
				return false;
			}
		}

		public static bool operator==(Size<T> a, Size<T> b){
			return a.Equals(b);
		}

		public static bool operator!=(Size<T> a, Size<T> b){
			return !a.Equals(b);
		}

		public override int GetHashCode() {
			return this.Width.GetHashCode() ^ this.Height.GetHashCode();
		}
	}
}
