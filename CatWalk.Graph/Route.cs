using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.Graph {
	public class Route<T>{
		public int TotalDistance { get; private set;}
		public IEnumerable<INodeLink<T>> Links { get; private set; }

		public Route(int distance, IEnumerable<INodeLink<T>> links) {
			this.TotalDistance = distance;
			this.Links = links;
		}
	}
}
