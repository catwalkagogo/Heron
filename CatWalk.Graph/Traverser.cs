/*
	$Id: Traverser.cs 316 2013-12-26 10:16:12Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.Graph {
	public static partial class Graph {
		#region Depth First

		public static IEnumerable<INode<T>> TraverseNodesDepthFirst<T>(this INode<T> root){
			var visited = new HashSet<INode<T>>();
			var stack = new Stack<INode<T>>();
			stack.Push(root);

			yield return root;
			visited.Add(root);

			while(stack.Count > 0){
			mainLoop:
				var node = stack.Peek();
				foreach(var link in node.Links.Where(link => !visited.Contains(link.To))){
					stack.Push(link.To);
					visited.Add(link.To);
					yield return link.To;
					goto mainLoop;
				}
				stack.Pop();
			}
		}

		public static IEnumerable<INodeLink<T>> TraverseLinksDepthFirst<T>(this INode<T> root){
			var visited = new HashSet<INodeLink<T>>();
			var stack = new Stack<INode<T>>();
			stack.Push(root);

			while(stack.Count > 0){
			mainLoop:
				var node = stack.Peek();
				foreach(var link in node.Links.Where(link => !visited.Contains(link))){
					stack.Push(link.To);
					visited.Add(link);
					yield return link;
					goto mainLoop;
				}
				stack.Pop();
			}
		}

		#endregion

		#region Preorder

		public static IEnumerable<INode<T>> TraverseNodesPreorder<T>(this INode<T> root){
			var visited = new HashSet<INode<T>>();
			var collection = new Queue<INode<T>>();
			collection.Enqueue(root);
			while(collection.Count > 0){
				var node = collection.Dequeue();
				yield return node;
				visited.Add(node);
				foreach(var link in node.Links.Where(link => !visited.Contains(link.To))){
					collection.Enqueue(link.To);
				}
			}
		}

		public static IEnumerable<INodeLink<T>> TraverseLinksPreorder<T>(this INode<T> root){
			var visited = new HashSet<INodeLink<T>>();
			var collection = new Queue<INode<T>>();
			collection.Enqueue(root);
			while(collection.Count > 0){
				var node = collection.Dequeue();
				foreach(var link in node.Links.Where(link => !visited.Contains(link))){
					yield return link;
					visited.Add(link);
					collection.Enqueue(link.To);
				}
			}
		}

		#endregion

		#region Postoreder

		public static IEnumerable<INode<T>> TraverseNodesPostorder<T>(this INode<T> root){
			var visited = new HashSet<INode<T>>();
			var stack = new Stack<INode<T>>();
			stack.Push(root);
			while(stack.Count > 0){
			mainLoop:
				var node = stack.Peek();
				visited.Add(node);
				foreach(var link in node.Links.Where(link => !visited.Contains(link.To))){
					stack.Push(link.To);
					goto mainLoop;
				}
				stack.Pop();
				yield return node;
			}
		}
		#endregion
		/*
		#region Walk

		public static void WalkParallel<T>(this INode<T> node, Action<INode<T>> action){
			action.ThrowIfNull("action");
			var visited = new HashSet<INode<T>>();
			WalkParallel(node, action, visited);
		}

		private static void WalkParallel<T>(INode<T> node, Action<INode<T>> action, HashSet<INode<T>> visited){
			IEnumerable<Action> tasks = null;
			lock(visited){
				visited.Add(node);
				tasks = node.Links.Where(link => !visited.Contains(link.To)).Select(link => {
					var node2 = link.To;
					return new Action(() => WalkParallel(node2, action, visited));
				});
			}
			Parallel.Invoke(tasks.Concat(Seq.Make(new Action(() => action(node)))).ToArray());
		}

		#endregion
		 * */
	}
}
