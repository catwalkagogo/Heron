/*
	$Id: RegistrySystemEntry.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace CatWalk.IOSystem.Win32 {
	public class RegistrySystemEntry : TerminalSystemEntry{
		public string EntryName{get; private set;}

		public RegistrySystemEntry(ISystemEntry parent, string name, string entryName) : base(parent, name){
			this.EntryName = entryName;
		}
	}
}
