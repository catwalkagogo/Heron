/*
	$Id: FloydWarshall.cs 316 2013-12-26 10:16:12Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatWalk;

namespace CatWalk.Graph {
	public static class FloydWarshall {

		public static IEnumerable<Route<T>> GetAllShortestPath<T>(INode<T> root){
			return GetAllShortestPath<T>(root.TraverseNodesPreorder());
		}
		public static IEnumerable<Route<T>> GetAllShortestPath<T>(IEnumerable<INode<T>> nodes){
			var path = new Dictionary<Tuple<INode<T>, INode<T>>, WorkingRoute<T>>();
			var allNodes = nodes.ToArray();
			foreach(var v in allNodes){
				foreach(var link in v.Links){
					var key = new Tuple<INode<T>, INode<T>>(link.From, link.To);
					WorkingRoute<T> route;
					if(path.TryGetValue(key, out route)){
						if(route.TotalDistance > link.Distance){
							path[key] = new WorkingRoute<T>(link.Distance, new INodeLink<T>[]{link});
						}
					}else{
						path[key] = new WorkingRoute<T>(link.Distance, new INodeLink<T>[]{link});
					}
				}
			}
			foreach(var vk in allNodes){
				foreach(var vi in allNodes){
					var ik = new Tuple<INode<T>, INode<T>>(vk, vi);
					foreach(var vj in allNodes){
						var ij = new Tuple<INode<T>, INode<T>>(vi, vj);
						var kj = new Tuple<INode<T>, INode<T>>(vk, vj);
						WorkingRoute<T> rik;
						WorkingRoute<T> rkj;
						if(path.TryGetValue(ik, out rik) && path.TryGetValue(kj, out rkj)){
							WorkingRoute<T> rij;
							var dist = rik.TotalDistance + rkj.TotalDistance;
							if(path.TryGetValue(ij, out rij)){
								if(rij.TotalDistance > dist){
									if(vi == vj && dist < 0){
										throw new NegativeCycleException();
									}
									path[ij] = new WorkingRoute<T>(dist, rik.Links.Concat(rkj.Links));
								}
							}else{
								if(vi == vj && dist < 0){
									throw new NegativeCycleException();
								}
								path[ij] = new WorkingRoute<T>(dist, rik.Links.Concat(rkj.Links));
							}
						}
					}
				}
			}
			return path.Values.Select(route => new Route<T>(route.TotalDistance, route.Links));
		}
	}

	public class NegativeCycleException : NegativeDistanceException{
		public NegativeCycleException(){}
		public NegativeCycleException(string message) : base(message){}
		public NegativeCycleException(string message, Exception ex) : base(message, ex){}
	}
}
