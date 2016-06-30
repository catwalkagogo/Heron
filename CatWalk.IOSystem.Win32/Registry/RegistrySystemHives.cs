/*
	$Id: RegistrySystemHives.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace CatWalk.IOSystem.Win32{
	public class RegistrySystemHiveDirectory : SystemEntry{
		public RegistrySystemHiveDirectory() : this(null, "Registry"){
		}

		public RegistrySystemHiveDirectory(ISystemEntry parent, string name) : base(parent, name){
		}

		public override bool IsDirectory {
			get {
				return true;
			}
		}

		public override IEnumerable<ISystemEntry> GetChildren(CancellationToken token, IProgress<double> progress) {
			return new RegistryHive[]{
				RegistryHive.ClassesRoot,
				RegistryHive.CurrentConfig,
				RegistryHive.CurrentUser,
				RegistryHive.LocalMachine,
				RegistryHive.PerformanceData,
				RegistryHive.Users,
			}
			.WithCancellation(token)
			.Select(hive => new RegistrySystemKey(this, RegistryUtility.GetHiveName(hive), hive));
		}

		public override ISystemEntry GetChild(string name, CancellationToken token, IProgress<double> progress) {
			return this.GetChildren().Where(entry => entry.IsDirectory).FirstOrDefault(key => key.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}
	}
}
