/*
	$Id: Format.cs 184 2011-03-25 16:04:17Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	[Serializable]
	public struct Format : IEquatable<Format>{
		internal int Index{get; private set;}
		public string Name{get; private set;}
		public string[] Extensions{get; private set;}
		public bool Readable{get; private set;}
		public bool Writable{get; private set;}
		public string Description{get; private set;}
		public string DefaultSuffix{
			get{
				return this.Extensions[0];
			}
		}

		private Format(int index) : this(){
			this.Index = index;
		}

		internal Format(ref Gfl.GflFormatInformation formatInfo) : this(){
			this.Index = formatInfo.Index;
			this.Name = formatInfo.Name;
			this.Readable = (formatInfo.Status & Gfl.Status.Read) > 0;
			this.Writable = (formatInfo.Status & Gfl.Status.Write) > 0;
			this.Description = formatInfo.Description;
			this.Extensions = formatInfo.Extension.Split(new char[]{'\0'}, formatInfo.NumberOfExtension);
		}

		public static readonly Format AnyFormats = new Format(-1);

		#region IEquatable

		public bool Equals(Format other){
			return this.Name.Equals(other.Name) && this.Extensions.Equals(other.Extensions) &&
				this.Readable.Equals(other.Readable) && this.Writable.Equals(other.Writable) &&
				this.Description.Equals(other.Description);
		}

		public override bool Equals(object obj){
			if(!(obj is Format)) {
				return false;
			}
			return this.Equals((Format)obj);
		}

		public override int GetHashCode(){
			return this.Name.GetHashCode() ^ this.DefaultSuffix.GetHashCode() ^ this.Readable.GetHashCode() ^ this.Writable.GetHashCode() ^ this.Extensions.GetHashCode();
		}

		public static bool operator ==(Format a, Format b){
			return a.Equals(b);
		}

		public static bool operator !=(Format a, Format b){
			return !a.Equals(b);
		}

		#endregion
	}
}
