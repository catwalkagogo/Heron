using System;
namespace CatWalk.Graph {
	public interface INodeLink<T> {
		INode<T> To { get; }
		INode<T> From { get; }
		int Distance { get; }
	}
}
