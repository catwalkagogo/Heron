using System;
namespace CatWalk.Graph {
	public interface INode<T> {
		System.Collections.Generic.IList<INodeLink<T>> Links { get; }
	}
}
