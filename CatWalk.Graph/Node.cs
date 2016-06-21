/*
	$Id: Node.cs 316 2013-12-26 10:16:12Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.Graph {
	public class Node<T> : INode<T>{
		private IList<INodeLink<T>> links;
		public IList<INodeLink<T>> Links {
			get{
				return this.links ?? (this.links = new List<INodeLink<T>>());
			}
		}
		public T Value{get; set;}

		public Node(){
		}

		public Node(IList<INodeLink<T>> links) {
			this.links = links;
		}

		public Node(IList<INodeLink<T>> links, T value)
			: this(links) {
			this.Value = value;
		}

		public Node(T value){
			this.Value = value;
		}

		internal void AddLink(INode<T> to, int distance) {
			this.Links.Add(new NodeLink<T>(this, to, distance));
		}
	}

	public struct NodeLink<T> : INodeLink<T> {
		public INode<T> From { get; private set; }
		public INode<T> To { get; private set; }
		public int Distance { get; private set; }

		public NodeLink(INode<T> from, INode<T> to, int distance)
			: this() {
			this.From = from;
			this.To = to;
			this.Distance = distance;
		}
	}

}
