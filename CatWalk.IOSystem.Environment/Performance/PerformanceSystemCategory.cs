/*
	$Id: PerformanceSystemCategory.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace CatWalk.IOSystem.Environment{
	public class PerformanceSystemCategory : SystemEntry{
		public PerformanceCounterCategory CounterCategory{get; private set;}

		public PerformanceSystemCategory(ISystemEntry parent, string name, PerformanceCounterCategory category) : base(parent, name){
			if(category == null){
				throw new ArgumentNullException("category");
			}
			this.CounterCategory = category;
		}

		public override bool IsDirectory {
			get {
				return false;
			}
		}

		public override IEnumerable<ISystemEntry> GetChildren(CancellationToken token, IProgress<double> progress) {
			var instanceNames = this.CounterCategory.GetInstanceNames().WithCancellation(token).ToArray();
			return ((instanceNames.Length > 0) ? this.CounterCategory.GetCounters(instanceNames[0]) : this.CounterCategory.GetCounters())
				.WithCancellation(token)
				.Select(counter => new PerformanceSystemCounter(this, counter.CounterName, counter));
		}
	}
}
