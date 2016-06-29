﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk.IOSystem;
using CatWalk.ComponentModel;

namespace CatWalk.Heron.ViewModel.IOSystem {
	public interface IOrderDefinition {
		string Name { get; }
		string DisplayName { get; }
		IComparer<SystemEntryViewModel> GetComparer(ListSortDirection order);
	}
}