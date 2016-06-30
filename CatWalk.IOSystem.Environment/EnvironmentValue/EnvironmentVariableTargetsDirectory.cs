/*
	$Id: EnvironmentVariableTargetsDirectory.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace CatWalk.IOSystem.Environment{
	public class EnvironmentVariableTargetDirectory : SystemEntry{
		public EnvironmentVariableTargetDirectory(ISystemEntry parent, string name) : base(parent, name){
		}

		public override bool IsDirectory {
			get {
				return true;
			}
		}

		#region ISystemDirectory Members

		public override IEnumerable<ISystemEntry> GetChildren(CancellationToken token, IProgress<double> progress) {
			return Enum.GetValues(typeof(EnvironmentVariableTarget))
				.Cast<EnvironmentVariableTarget>()
				.WithCancellation(token)
				.Select(target => new EnvironmentVariableSystemDirectory(this, target.ToString(), target));
		}

		public override ISystemEntry GetChild(string name, CancellationToken token, IProgress<double> progress) {
			return this.GetChildren().Cast<EnvironmentVariableSystemDirectory>().FirstOrDefault(entry => entry.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		public override bool Contains(string name, CancellationToken token, IProgress<double> progress) {
			EnvironmentVariableTarget target;
			if(Enum.TryParse<EnvironmentVariableTarget>(name, out target)){
				return true;
			}else{
				return false;
			}
		}

		#endregion
	}
}
