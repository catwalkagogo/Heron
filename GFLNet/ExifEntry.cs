/*
	$Id: ExifEntry.cs 182 2011-03-23 12:46:00Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	[Serializable]
	public struct ExifEntry : IEquatable<ExifEntry>{
		public ExifEntryTypes Types{get; private set;}
		public int Tag{get; private set;}
		public string Name{get; private set;}
		public string Value{get; private set;}
		
		internal ExifEntry(Gfl.GflExifEntry entry) : this(){
			this.Types = entry.Types;
			this.Tag = (int)entry.Tag;
			this.Name = entry.Name;
			this.Value = entry.Value;
		}

		#region IEquatable

		public bool Equals(ExifEntry other){
			return this.Types.Equals(other.Types) && this.Tag.Equals(other.Tag) && this.Name.Equals(other.Name) && this.Value.Equals(other.Value);
		}

		public override bool Equals(object obj){
			if(!(obj is ExifEntry)) {
				return false;
			}
			return this.Equals((ExifEntry)obj);
		}

		public override int GetHashCode(){
			return this.Types.GetHashCode() ^ this.Tag.GetHashCode() ^ this.Name.GetHashCode() ^ this.Value.GetHashCode();
		}

		public static bool operator ==(ExifEntry a, ExifEntry b){
			return a.Equals(b);
		}

		public static bool operator !=(ExifEntry a, ExifEntry b){
			return !a.Equals(b);
		}

		#endregion
	}
}
