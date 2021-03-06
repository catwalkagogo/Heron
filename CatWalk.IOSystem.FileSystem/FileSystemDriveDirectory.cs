﻿/*
	$Id: FileSystemDrives.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace CatWalk.IOSystem.FileSystem {
	public class FileSystemDriveDirectory : SystemEntry{
		public FileSystemDriveDirectory(ISystemEntry parent, string name) : base(parent, name){
		}

		public override bool IsDirectory {
			get {
				return true;
			}
		}

		public override IEnumerable<ISystemEntry> GetChildren(CancellationToken token, IProgress<double> progress) {
			return DriveInfo.GetDrives().WithCancellation(token).Select(drive => new FileSystemDrive(this, drive.Name, drive.Name[0]));
		}
	}
}
