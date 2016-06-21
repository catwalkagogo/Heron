/*
	$Id: Graph.cs 316 2013-12-26 10:16:12Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CatWalk.Graph{
	public static partial class Graph{
		public static Node<T>[] ReadGraphFromFile<T>(string file){
			return ReadGraphFromFile<T>(file, Int32.MaxValue);
		}

		public static Node<T>[] ReadGraphFromFile<T>(string file, int infinity){
			var lines = File.ReadAllLines(file, Encoding.UTF8);
			var n = Int32.Parse(lines[0]);
			Node<T>[] nodes = new Node<T>[n];
			for(int i = 0; i < n; i++){
				var elms = lines[i + 1].Split(new char[]{' '}, n, StringSplitOptions.RemoveEmptyEntries);
				for(int j = 0; j < elms.Length; j++){
					try{
					int d = Int32.Parse(elms[j]);
					if(nodes[i] == null){
						nodes[i] = new Node<T>();
					}
					if(nodes[j] == null){
						nodes[j] = new Node<T>();
					}
					if(d != 0 && d < infinity){
						nodes[i].AddLink(nodes[j], d);
					}
					}catch{
						Console.WriteLine("{0}", elms[j]);
					}
				}
			}
			return nodes.Where(node => node != null).ToArray();
		}

		public static int[,] ReadMatrixFromFile<T>(string file){
			var lines = File.ReadAllLines(file, Encoding.UTF8);
			var n = Int32.Parse(lines[0]);
			int[,] matrix = new int[n,n];
			for(int i = 0; i < n; i++){
				var elms = lines[i + 1].Split(new char[]{' ', '\t'}, n, StringSplitOptions.RemoveEmptyEntries);
				for(int j = 0; j < elms.Length; j++){
					int d = Int32.Parse(elms[j]);
					matrix[i,j] = d;
				}
			}
			return matrix;
		}

		public static int[,] GetGraphMatrix<T>(Node<T> root){
			var trMap = new Dictionary<Tuple<int, int>, int>(); // from, to, distance
			var nodeDic = new Dictionary<INode<T>, int>();

			foreach(var link in root.TraverseLinksPreorder()){
				int a, b;
				if(!nodeDic.TryGetValue(link.From, out a)){
					a = nodeDic.Count;
					nodeDic.Add(link.From, a);
				}
				if(!nodeDic.TryGetValue(link.To, out b)){
					b = nodeDic.Count;
					nodeDic.Add(link.To, b);
				}
				trMap.Add(new Tuple<int, int>(a, b), link.Distance);
			}

			return MakeGraphMatrix(trMap, nodeDic.Count);
		}

		public static int[,] GetGraphMatrix<T>(Node<T>[] nodes){
			var trMap = new Dictionary<Tuple<int, int>, int>();
			var nodeDic = new Dictionary<INode<T>, int>();

			foreach(var node in nodes){
				nodeDic.Add(node, nodeDic.Count);
			}
			foreach(var node in nodes){
				foreach(var link in node.Links){
					int a, b;
					if(!nodeDic.TryGetValue(node, out a)){
						a = nodeDic.Count;
						nodeDic.Add(node, a);
					}
					if(!nodeDic.TryGetValue(link.To, out b)){
						b = nodeDic.Count;
						nodeDic.Add(link.To, b);
					}
					trMap.Add(new Tuple<int, int>(a, b), link.Distance);
				}
			}
			return MakeGraphMatrix(trMap, nodeDic.Count);
		}

		private static int[,] MakeGraphMatrix(IDictionary<Tuple<int, int>, int> trMap, int n){
			var matrix = new int[n, n];
			for(int i = 0; i < n; i++){
				for(int j = 0; j < n; j++){
					int d;
					if(trMap.TryGetValue(new Tuple<int, int>(i, j), out d)){
						matrix[i, j] = d;
					}else{
						matrix[i, j] = Int32.MaxValue;
					}
				}
			}
			return matrix;
		}
	}
}