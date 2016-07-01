using System;
using System.IO;

namespace CatWalk.IOSystem.FileSystem {
	public interface IFileInformation {
		DateTime CreationTime { get; }
		DateTime LastAccessTime { get; }
		DateTime LastWriteTime { get; }
		bool Exists { get; }
		//long FileIndex { get; }
		long Length { get; }
		int LinkCount { get; }
		//int VolumeSerialNumber { get; }
	}
}
