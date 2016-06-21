/*
	$Id: RegistrySystemKey.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.Threading;

namespace CatWalk.IOSystem.Win32 {
	public class RegistrySystemKey : SystemEntry{
		public RegistrySystemKey ParentRegistry{get; private set;}
		public string KeyName{get; private set;}
		public string RegistryPath {get; private set;}

		public RegistrySystemKey(RegistrySystemKey parent, string name, string keyName) : base(parent, name){
			this.ParentRegistry = parent;
			this.KeyName = keyName;
			this._RegistryKey = new Lazy<RegistryKey>(this.GetRegistryKey);
			this.RegistryPath = this.ParentRegistry.ConcatRegistryPath(keyName);
		}

		public RegistrySystemKey(ISystemEntry parent, string name, RegistryHive hive) : base(parent, RegistryUtility.GetHiveName(hive)){
			this.ParentRegistry = null;
			this._RegistryKey = new Lazy<RegistryKey>(() => RegistryUtility.GetRegistryKey(hive));
			this.KeyName = this.RegistryPath = RegistryUtility.GetHiveName(hive);
		}

		public override bool IsDirectory {
			get {
				return true;
			}
		}

		private RegistryKey GetRegistryKey(){
			if(this.ParentRegistry == null){
				return null;
			}else{
				if(this.ParentRegistry.RegistryKey ==  null){
					return null;
				}else{
					return this.ParentRegistry.RegistryKey.OpenSubKey(
						this.Name,
						RegistryKeyPermissionCheck.ReadSubTree,
						RegistryRights.EnumerateSubKeys | RegistryRights.QueryValues | RegistryRights.ReadKey);
				}
			}
		}

		#region ISystemDirectory Members

		public override IEnumerable<ISystemEntry> GetChildren(CancellationToken token, IProgress<double> progress){
			if(this.RegistryKey == null){
				return new RegistrySystemEntry[0];
			}else{
				return
					Seq.Make(
						this.RegistryKey.GetSubKeyNames()
							.Select(name => new RegistrySystemKey(this, name, name) as ISystemEntry),
						this.RegistryKey.GetValueNames()
						.Select(name => new RegistrySystemEntry(this, name, name) as ISystemEntry))
					.Aggregate((x, y) => x.Concat(y));
			}
		}

		public override bool Contains(string name) {
			return this.GetChildren().Any(entry => entry.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		public override ISystemEntry GetChildDirectory(string name){
			return this.GetChildren()
				.Where(entry => entry.IsDirectory)
				.FirstOrDefault(entry => entry.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		#endregion

		#region IDisposable Members

		~RegistrySystemKey(){
			if(this._RegistryKey.IsValueCreated){
				this._RegistryKey.Value.Close();
			}
		}

		#endregion

		#region IRegistrySystemKey Members

		private readonly Lazy<RegistryKey> _RegistryKey;
		public RegistryKey RegistryKey{
			get{
				return this._RegistryKey.Value;
			}
		}

		public string ConcatRegistryPath(string name){
			return this.RegistryPath + "\\" + name;
		}

		#endregion
	}
}
