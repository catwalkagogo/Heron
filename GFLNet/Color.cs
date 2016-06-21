/*
	$Id: Color.cs 182 2011-03-23 12:46:00Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	[Serializable]
	public struct Color : IEquatable<Color>{
		public int Red{get; private set;}
		public int Green{get; private set;}
		public int Blue{get; private set;}
		public int Alpha{get; private set;}

		public Color(int r, int g, int b): this(r, g, b, 0){}
		public Color(int r, int g, int b, int a) : this(){
			if(r < 0 || 255 < r){
				throw new ArgumentOutOfRangeException("r");
			}
			if(g < 0 || 255 < g){
				throw new ArgumentOutOfRangeException("g");
			}
			if(b < 0 || 255 < b){
				throw new ArgumentOutOfRangeException("b");
			}
			if(a < 0 || 255 < a){
				throw new ArgumentOutOfRangeException("a");
			}
			this.Red = r;
			this.Green = g;
			this.Blue = b;
			this.Alpha = a;
		}

		internal Gfl.GflColor ToGflColor(){
			var color = new Gfl.GflColor();
			color.Alpha = (ushort)this.Alpha;
			color.Red = (ushort)this.Red;
			color.Green = (ushort)this.Green;
			color.Blue = (ushort)this.Blue;
			return color;
		}

		#region IEquatable

		public bool Equals(Color other){
			return this.Alpha.Equals(other.Alpha) && this.Red.Equals(other.Red) && this.Green.Equals(other.Green) && this.Blue.Equals(other.Blue);
		}

		public override bool Equals(object obj){
			if(!(obj is Color)) {
				return false;
			}
			return this.Equals((Color)obj);
		}

		public override int GetHashCode(){
			return this.Alpha.GetHashCode() ^ this.Red.GetHashCode() ^ this.Green.GetHashCode() ^ this.Blue.GetHashCode();
		}

		public static bool operator ==(Color a, Color b){
			return a.Equals(b);
		}

		public static bool operator !=(Color a, Color b){
			return !a.Equals(b);
		}

		#endregion
	}
}
