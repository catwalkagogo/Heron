/*
	$Id: AStar.cs 316 2013-12-26 10:16:12Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CatWalk;
using CatWalk.Collections;

namespace CatWalk.Graph {
	public static partial class Graph {
		public static Route<T> GetShortestPath<T>(this INode<T> start, INode<T> goal, Func<INode<T>, double> gstar, Func<INode<T>, double> hstar){
			var open = new Dictionary<INode<T>, Data<T>>();
			var close = new HashSet<INode<T>>();
			open.Add(start, new Data<T>(gstar(start) + hstar(start)));
			int openCount = 1;

			while(open.Count > 0){
				// Find least fs node data.
				var np = open.OrderBy(p => p.Value.Fs).First();
				var n = np.Key;
				var nd = np.Value;

				if(n == goal){
					var stack = new Stack<INodeLink<T>>();
					var data = nd;
					var distance = 0;
					while(data.ParentLink.From != null){
						distance += data.ParentLink.Distance;
						stack.Push(data.ParentLink);
						data = data.ParentData;
					}
					return new Route<T>(distance, stack.ToArray());
				}else{
					open.Remove(n);
					close.Add(n);
				}

				foreach(var link in n.Links){
					var m = link.To;
					if(close.Contains(m)){
						continue;
					}

					var fdm = gstar(n) + hstar(m) + link.Distance;
					Data<T> md;
					if(open.TryGetValue(m, out md)){
						var fsm = gstar(m) + hstar(m);
						if(fdm < fsm){
							md.ParentLink = link;
							md.ParentData = nd;
							md.Fs = fdm;
						}
					}else{
						open.Add(m, new Data<T>(fdm){ParentLink = link, ParentData=nd});
						openCount++;
					}
				}
			}
			return null;
		}

		private class Data<T>{
			public INodeLink<T> ParentLink{get; set;}
			public Data<T> ParentData{get; set;}
			public double Fs{get; set;}
			public Data(double fs){
				this.Fs = fs;
			}
		}
	}
}
