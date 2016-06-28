using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.IO {
	public static class FilePathFormats {
		private static Lazy<IFilePathFormat> _Windows = new Lazy<IFilePathFormat>(() => new WindowsPathFormat());
		public static IFilePathFormat Windows {
			get {
				return _Windows.Value;
			}
		}

		private static Lazy<IFilePathFormat> _Unix = new Lazy<IFilePathFormat>(() => new UnixPathFormat());
		public static IFilePathFormat Unix {
			get {
				return _Unix.Value;
			}
		}

		/*
		public static IFilePathFormat PlatformPathFormat {
			get {
				switch(Environment.OSVersion.Platform) {
					case PlatformID.Win32NT:
					case PlatformID.Win32S:
					case PlatformID.Win32Windows:
					case PlatformID.WinCE:
					case PlatformID.Xbox:
						return Windows;
					default:
						return Unix;
				}
			}
		}*/
	}
}
