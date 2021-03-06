/*
	$Id: dijkstra.cs 316 2013-12-26 10:16:12Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CatWalk.Collections;

namespace CatWalk.Graph{
	public static partial class Graph {
		/*
		static void Main(string[] args){
			var nodes = Graph.ReadGraphFromFile<int>(args[0]);
			int i = 0;
			foreach(var node in nodes){
				node.Value = i;
				i++;
			}
			var t = DateTime.Now;
			foreach(var node in nodes){
				Console.WriteLine("SP from {0}", node.Value);
				foreach(var tup in GetShortestPath(node).Skip(1)){
					Console.WriteLine("{0} to {1}", tup.Item2.Value, tup.Item1.Value);
				}
			}
			Console.WriteLine("{0}", DateTime.Now - t);
		}
		*/
		
		public static IEnumerable<Route<T>> GetShortestPaths<T>(this INode<T> root){
			return GetShortestPaths<T>(root, root.TraverseNodesPreorder());
		}
		public static IEnumerable<Route<T>> GetShortestPaths<T>(INode<T> start, IEnumerable<INode<T>> nodes){
			var allNodes = new HashSet<INode<T>>(nodes);
			var routes = new Dictionary<INode<T>, WorkingRoute<T>>();

			routes[start] = new WorkingRoute<T>(0);
			foreach(var node in allNodes.Where(v => v != start)){
				routes[node] = new WorkingRoute<T>(Int32.MaxValue);
			}

			// 総て訪問済みになるまで
			while(allNodes.Count() > 0){
				// 未訪問で距離が最小のノードを検索
				INode<T> u = null;
				var min = new WorkingRoute<T>(Int32.MaxValue);
				foreach(var pair in routes){
					var node = pair.Key;
					var route = pair.Value;
					if(allNodes.Contains(node) && route.TotalDistance < min.TotalDistance){
						min = route;
						u = node;
					}
				}
				if(min.Links.Count > 0){
					yield return new Route<T>(min.TotalDistance, min.Links);
				}

				// 訪問済みにする
				allNodes.Remove(u);
				// ノードuからのリンクでdistancesより距離の近い物を登録
				var distU = routes[u];
				foreach(var link in u.Links){
					if(link.Distance < 0){
						throw new NegativeDistanceException();
					}
					// リンク先のノードのルートの総距離より、uまでの距離とuからの距離の和の方が短いとき
					var distTo = routes[link.To];
					var dist = distU.TotalDistance + link.Distance;
					if(distTo.TotalDistance > dist){
						// 経路を更新
						distTo.TotalDistance = dist;
						distTo.Links.Clear();
						distTo.Links.AddRange(distU.Links.Concat(new INodeLink<T>[]{link}));
					}
				}
			}
		}
	}

	public class NegativeDistanceException : Exception{
		public NegativeDistanceException(){}
		public NegativeDistanceException(string message) : base(message){}
		public NegativeDistanceException(string message, Exception ex) : base(message, ex){}
	}
}