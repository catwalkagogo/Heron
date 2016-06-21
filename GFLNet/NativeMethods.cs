using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace GflNet {
	internal static class NativeMethods{
		[DllImport("kernel32")]
		public static extern bool ReadFile(IntPtr handle, IntPtr buffer, uint nbytestoread, out uint lpnbytestoread, IntPtr overlap);
		[DllImport("kernel32")]
		public static extern uint SetFilePointer(IntPtr handle, int offset, IntPtr offsetHigh, SeekOrigin origin);
	}
}
