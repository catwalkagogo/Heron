/*
	$Id: PerformanceSystemCategoryDirectory.cs 217 2011-06-21 14:16:53Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace CatWalk.IOSystem.Win32{
	public class PerformanceSystemCategoryDirectory : SystemEntry{
		public string MachineName{get; private set;}

		public PerformanceSystemCategoryDirectory(ISystemEntry parent, string name) : this(parent, name, null){
		}

		public PerformanceSystemCategoryDirectory(ISystemEntry parent, string name, string machineName) : base(parent, name){
			this.MachineName = machineName;
		}

		public override bool IsDirectory {
			get {
				return true;
			}
		}

		public override IEnumerable<ISystemEntry> GetChildren(CancellationToken token, IProgress<double> progress) {
			return ((String.IsNullOrEmpty(this.MachineName)) ? PerformanceCounterCategory.GetCategories() :
				PerformanceCounterCategory.GetCategories(this.MachineName))
					.WithCancellation(token)
					.Select(cat => new PerformanceSystemCategory(this, cat.CategoryName, cat));
		}
	}
}
