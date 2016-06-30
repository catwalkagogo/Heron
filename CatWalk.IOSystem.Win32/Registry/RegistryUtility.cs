/*
	$Id: RegistryUtility.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace CatWalk.IOSystem.Win32{
	internal static class RegistryUtility {
		public static string GetHiveName(RegistryHive hive){
			switch(hive){
				case RegistryHive.ClassesRoot: return "HKEY_CLASSES_ROOT";
				case RegistryHive.CurrentConfig: return "HKEY_CURRENT_CONFIG";
				case RegistryHive.CurrentUser: return "HKEY_CURRENT_USER";
				//case RegistryHive.DynData: return "HKEY_DYNAMIC_DATA";
				case RegistryHive.LocalMachine: return "HKEY_LOCAL_MACHINE";
				case RegistryHive.PerformanceData: return "HKEY_PERFORMANCE_DATA";
				case RegistryHive.Users: return "HKEY_USERS";
				default: throw new ArgumentOutOfRangeException(nameof(hive));
			}
		}

		public static RegistryHive GetHive(string name){
			switch(name.ToUpper()){
				case "HKEY_CLASSES_ROOT": return RegistryHive.ClassesRoot;
				case "HKEY_CURRENT_CONFIG": return RegistryHive.CurrentConfig;
				case "HKEY_CURRENT_USER": return RegistryHive.CurrentUser;
				//case "HKEY_DYNAMIC_DATA": return RegistryHive.DynData;
				case "HKEY_LOCAL_MACHINE": return RegistryHive.LocalMachine;
				case "HKEY_PERFORMANCE_DATA": return RegistryHive.PerformanceData;
				case "HKEY_USERS": return RegistryHive.Users;
				default: throw new ArgumentException(nameof(name));
			}
		}

		public static RegistryKey GetRegistryKey(RegistryHive hive){
			switch(hive){
				case RegistryHive.ClassesRoot: return Registry.ClassesRoot;
				case RegistryHive.CurrentConfig: return Registry.CurrentConfig;
				case RegistryHive.CurrentUser: return Registry.CurrentUser;
				//case RegistryHive.DynData: return Registry.DynData;
				case RegistryHive.LocalMachine: return Registry.LocalMachine;
				case RegistryHive.PerformanceData: return Registry.PerformanceData;
				case RegistryHive.Users: return Registry.Users;
				default: throw new ArgumentOutOfRangeException(nameof(hive));
			}
		}
	}
}
