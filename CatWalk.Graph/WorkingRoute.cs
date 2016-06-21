using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.Graph {
	internal class WorkingRoute<T>{
		public int TotalDistance { get; set; }
		public List<INodeLink<T>> Links { get; private set; }

		public WorkingRoute(int distance) {
			this.TotalDistance = distance;
			this.Links = new List<INodeLink<T>>();
		}

		public WorkingRoute(int distance, IEnumerable<INodeLink<T>> links) {
			this.TotalDistance = distance;
			this.Links = new List<INodeLink<T>>(links);
		}
	}
}
