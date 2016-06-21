using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron.ViewModel {
	public interface IJobManager{
		void Register(Job job);
		int Count { get; }
	}
}
