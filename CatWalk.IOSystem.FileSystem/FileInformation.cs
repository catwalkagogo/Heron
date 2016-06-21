/*
	$Id: FileInformation.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;

namespace CatWalk.IOSystem.FileSystem {
	using IO = System.IO;

	[StructLayout(LayoutKind.Sequential)]
	internal class FileInformation : IFileInformation {
		private const int ERROR_FILE_NOT_FOUND = 2;
		private ByHandleFileInformation _Info;
		private Exception _GetFileInformationByHandleException;

		public FileInformation(string file){
			//file = IO.Path.GetFullPath(file);
			Win32Exception ex;
			using(var hFile = OpenFile(file, out ex)){
				if(ex != null){
					this.Exists = (ex.ErrorCode != ERROR_FILE_NOT_FOUND);
				}else{
					this.Exists = true;
					if(!GetFileInformationByHandle(hFile, out this._Info)){
						this._GetFileInformationByHandleException = new FileLoadException("GetFileInformationByHandle faild", new Win32Exception(Marshal.GetLastWin32Error()));
					}
				}
			}
		}

		private void CheckInfo(){
			if(!this.Exists){
				throw new FileNotFoundException();
			}
			if(this._GetFileInformationByHandleException != null){
				throw this._GetFileInformationByHandleException;
			}
		}

		public bool Exists{get; private set;}
		public FileAttributes Attributes{get{ this.CheckInfo(); return this._Info.FileAttributes;}}
		public DateTime CreationTime{get{ this.CheckInfo(); return ToDateTime(this._Info.CreationTime);}}
		public DateTime LastWriteTime{get{ this.CheckInfo(); return ToDateTime(this._Info.LastWriteTime);}}
		public DateTime LastAccessTime{get{ this.CheckInfo(); return ToDateTime(this._Info.LastAccessTime);}}
		public int VolumeSerialNumber{get{ this.CheckInfo(); return this._Info.VolumeSerialNumber;}}
		public long Length{get{ this.CheckInfo(); return ToLong(this._Info.FileSizeHigh, this._Info.FileIndexLow);}}
		public int LinkCount{get{ this.CheckInfo(); return this._Info.NumberOfLinks;}}
		public long FileIndex{get{ this.CheckInfo(); return ToLong(this._Info.FileIndexHigh, this._Info.FileIndexLow);}}


		#region Structs

		private struct ByHandleFileInformation{
			public IO.FileAttributes FileAttributes;
			public FileTime CreationTime;
			public FileTime LastWriteTime;
			public FileTime LastAccessTime;
			public int VolumeSerialNumber;
			public int FileSizeHigh;
			public int FileSizeLow;
			public int NumberOfLinks;
			public int FileIndexHigh;
			public int FileIndexLow;
		}

		private struct FileTime{
			public int LowDateTime;
			public int HighDateTime;
		}

		#endregion

		#region Functions

		private static SafeFileHandle OpenFile(string file, out Win32Exception ex){
			var handle = CreateFileW(file, (FileAccess)0, FileShare.Delete | FileShare.Read, IntPtr.Zero, FileMode.Open, FileOptions.None, IntPtr.Zero);
			var eno = Marshal.GetLastWin32Error();
			if(handle == null || handle.IsInvalid){
				ex = new Win32Exception(eno);
				return null;
			}
			ex = null;
			return handle;
		}

		private static DateTime ToDateTime(FileTime fileTime){
			return (new DateTime(ToLong(fileTime.HighDateTime, fileTime.LowDateTime))).AddYears(1600);
		}

		private static long ToLong(int high, int low){
			return (((long)high) << 32) + low;
		}

		[DllImport("Kernel32.dll", SetLastError = true, ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetFileInformationByHandle(SafeFileHandle file, out ByHandleFileInformation fileInformation);

		[DllImport("Kernel32.dll", SetLastError = true, ExactSpelling = true)]
		private static extern SafeFileHandle CreateFileW(
			[In, MarshalAs(UnmanagedType.LPWStr)] string fileName,
			FileAccess desiredAccess,
			FileShare shareMode,
			IntPtr securityAttributes,
			FileMode createDisposition,
			FileOptions flagsAndAttributes,
			IntPtr templateFile);

		#endregion
	}
}

