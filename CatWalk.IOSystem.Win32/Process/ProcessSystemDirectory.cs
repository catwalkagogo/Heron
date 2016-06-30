/*
	$Id: ProcessSystemDirectory.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace CatWalk.IOSystem.Win32 {
	public class ProcessSystemDirectory : SystemEntry{
		public string MachineName{get; private set;}
		public bool EnumAllProcesses{get; private set;}

		public ProcessSystemDirectory(ISystemEntry parent, string name) : this(parent, name, null, false){
		}

		public ProcessSystemDirectory(ISystemEntry parent, string name, bool enumAllProcesses) : this(parent, name, null, enumAllProcesses){
		}

		public ProcessSystemDirectory(ISystemEntry parent, string name, string machineName) : this(parent, machineName, false){
		}

		public ProcessSystemDirectory(ISystemEntry parent, string name, string machineName, bool enumAllProcesses) : base(parent, name){
			this.MachineName = machineName;
			this.EnumAllProcesses = enumAllProcesses;
		}

		public override bool IsDirectory {
			get {
				return true;
			}
		}

		#region ISystemDirectory Members

		public override IEnumerable<ISystemEntry> GetChildren(CancellationToken token, IProgress<double> progress) {
			if(this.EnumAllProcesses){
				return ((String.IsNullOrEmpty(this.MachineName)) ? Process.GetProcesses() : Process.GetProcesses(this.MachineName))
					.WithCancellation(token)
					.Select(proc => new ProcessSystemEntry(this, proc.Id.ToString(), proc));
			}else{
				return ((String.IsNullOrEmpty(this.MachineName)) ? Process.GetProcesses() : Process.GetProcesses(this.MachineName))
					.Select(proc => new ProcessSystemEntry(this, proc.Id.ToString(), proc))
					.WithCancellation(token)
					.Where(entry => entry.ParentProcess == null);
			}
		}

		#endregion
	}
}
