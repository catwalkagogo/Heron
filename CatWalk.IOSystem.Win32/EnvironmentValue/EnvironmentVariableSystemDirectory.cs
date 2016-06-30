/*
	$Id: EnvironmentVariableSystemDirectory.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace CatWalk.IOSystem.Win32 {
	public class EnvironmentVariableSystemDirectory : SystemEntry{
		public EnvironmentVariableTarget EnvironmentVariableTarget{get; private set;}

		public EnvironmentVariableSystemDirectory(ISystemEntry parent, string name, EnvironmentVariableTarget target) : base(parent, name){
			this.EnvironmentVariableTarget = target;
		}

		public override bool IsDirectory {
			get {
				return true;
			}
		}

		#region ISystemDirectory Members

		public override IEnumerable<ISystemEntry> GetChildren(CancellationToken token, IProgress<double> progress) {
			return System.Environment.GetEnvironmentVariables(this.EnvironmentVariableTarget)
				.Cast<DictionaryEntry>()
				.Select(v => new EnvironmentVariableSystemEntry(this, (string)v.Key, this.EnvironmentVariableTarget, (string)v.Key));
		}

		#endregion
	}
}
