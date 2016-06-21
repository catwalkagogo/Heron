/*
	$Id: Exif.cs 182 2011-03-23 12:46:00Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace GflNet {
	[Serializable]
	public class Exif : ReadOnlyCollection<ExifEntry>{
		internal Exif(Gfl.GflExifData exif) : base(new List<ExifEntry>(exif.NumberOfItems)){
			for(int i = 0; i < exif.NumberOfItems; i++){
				this.Items.Add(new ExifEntry(exif.ItemList[i]));
			}
		}
	}
}
