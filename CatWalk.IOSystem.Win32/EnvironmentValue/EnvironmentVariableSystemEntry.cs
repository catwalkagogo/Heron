/*
	$Id: EnvironmentVariableSystemEntry.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IOSystem.Win32 {
	public class EnvironmentVariableSystemEntry : TerminalSystemEntry{
		public EnvironmentVariableTarget EnvironmentVariableTarget{get; private set;}
		public string VariableName{get; private set;}

		public EnvironmentVariableSystemEntry(ISystemEntry parent, string name, EnvironmentVariableTarget target, string varName) : base(parent, name){
			this.EnvironmentVariableTarget = target;
			this.VariableName = varName;
		}

		public string Value{
			get{
				return System.Environment.GetEnvironmentVariable(this.VariableName, this.EnvironmentVariableTarget);
			}
		}
	}
}
