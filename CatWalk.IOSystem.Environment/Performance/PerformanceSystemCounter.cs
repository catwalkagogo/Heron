/*
	$Id: PerformanceSystemCounter.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CatWalk.IOSystem.Environment{
	public class PerformanceSystemCounter : TerminalSystemEntry{
		public PerformanceCounter Counter{get; private set;}

		public PerformanceSystemCounter(ISystemEntry parent, string name, PerformanceCounter counter) : base(parent, name){
			this.Counter = counter;
		}

		public long RawValue{
			get{
				return this.Counter.RawValue;
			}
		}

		public string Description{
			get{
				return this.Counter.CounterHelp;
			}
		}
	}
}
