using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using CatWalk.Heron.IOSystem;
using CatWalk.Heron.ViewModel.IOSystem;

namespace CatWalk.Heron.FileSystem.Win32 {
	using IO = System.IO;
	/*
	public class FileSystemEntryFilter : EntryFilter{
		private string _PathPattern = null;
		private Lazy<Regex> _PathRegex = null;
		private Range<long>? _SizeRange = null;
		private int _CreatedTimeSpan = -1;
		private int _LastWriteTimeSpan = -1;
		private Range<DateTime>? _CreatedTimeRange = null;
		private Range<DateTime>? _LastWriteTimeRange = null;
		private IO.FileAttributes attributes = IO.FileAttributes.Archive
											 | IO.FileAttributes.Compressed
											 | IO.FileAttributes.Device
											 | IO.FileAttributes.Directory
											 | IO.FileAttributes.Encrypted
											 | IO.FileAttributes.Hidden
											 | IO.FileAttributes.Normal
											 | IO.FileAttributes.NotContentIndexed
											 | IO.FileAttributes.Offline
											 | IO.FileAttributes.ReadOnly
											 | IO.FileAttributes.ReparsePoint
											 | IO.FileAttributes.SparseFile
											 | IO.FileAttributes.System
											 | IO.FileAttributes.Temporary;

		public string PathPattern {
			get {
				return this._PathPattern;
			}
			set {
				this._PathPattern = value;
				if(!(String.IsNullOrEmpty(this._PathPattern))) {
					this._PathRegex = new Lazy<Regex>(() => new Regex(this._PathPattern, RegexOptions.IgnoreCase));
				}
			}
		}

		public Range<long>? SizeRange {
			get {
				return this._SizeRange;
			}
			set {
				this._SizeRange = value;
			}
		}

		public int CreatedTimeSpan {
			get {
				return this._CreatedTimeSpan;
			}
			set {
				this._CreatedTimeSpan = value;
			}
		}

		public int LastWriteTimeSpan {
			get {
				return this._LastWriteTimeSpan;
			}
			set {
				this._LastWriteTimeSpan = value;
			}
		}

		public Range<DateTime>? CreatedTimeRange {
			get {
				return this._CreatedTimeRange;
			}
			set {
				this._CreatedTimeRange = value;
			}
		}

		public Range<DateTime>? LastWriteTimeRange {
			get {
				return this._LastWriteTimeRange;
			}
			set {
				this._LastWriteTimeRange = value;
			}
		}

		public IO.FileAttributes Attributes {
			get {
				return this.attributes;
			}
			set {
				this.attributes = value;
			}
		}

		protected override bool FilterEntry(SystemEntryViewModel entry) {
			var fse = entry.Entry as IFileSystemEntry;
			if(fse != null) {
				bool retVal = true;
				if(!(String.IsNullOrEmpty(this._PathPattern))) {
					retVal &= this._PathRegex.Value.IsMatch(fse.FileSystemPath.FullPath);
					if(!retVal) {
						return retVal;
					}
				}
				if(this._SizeRange != null) {
					retVal &= this._SizeRange.Value.Contains((long)entry.Columns[typeof(FileSystemProvider.FileSizeColumn)].Value);
					if(!retVal) {
						return retVal;
					}
				}
				if(this._CreatedTimeSpan >= 0) {
					retVal &= ((DateTime.Now - ((DateTime)entry.Columns[typeof(FileSystemProvider.CreationTimeColumn)].Value)).TotalSeconds <= this._CreatedTimeSpan);
					if(!retVal) {
						return retVal;
					}
				}
				if(this._LastWriteTimeSpan >= 0) {
					retVal &= ((DateTime.Now - ((DateTime)entry.Columns[typeof(FileSystemProvider.LastWriteTimeColumn)].Value)).TotalSeconds <= this._LastWriteTimeSpan);
					if(!retVal) {
						return retVal;
					}
				}
				if(this._CreatedTimeRange != null) {
					retVal &= this._CreatedTimeRange.Value.Contains((DateTime)entry.Columns[typeof(FileSystemProvider.CreationTimeColumn)].Value);
					if(!retVal) {
						return retVal;
					}
				}
				if(this._LastWriteTimeRange != null) {
					retVal &= this._LastWriteTimeRange.Value.Contains((DateTime)entry.Columns[typeof(FileSystemProvider.LastWriteTimeColumn)].Value);
					if(!retVal) {
						return retVal;
					}
				}
				retVal &= (((IO::FileAttributes)entry.Columns[typeof(FileSystemProvider.AttributesColumn)].Value & this.attributes) > 0);
				return retVal;
			} else {
				return false;
			}
		}
		
	}
	*/
}
