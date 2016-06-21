/*
	$Id: Prim.cs 316 2013-12-26 10:16:12Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace CatWalk.Graph{
	public static partial class Graph {
		public static IEnumerable<INodeLink<T>> GetMinimumSpanningTree<T, TLink>(this INode<T> root) {
			var visited = new Dictionary<INode<T>, bool>();
			var current = root;
			var nexts = new Dictionary<INode<T>, INodeLink<T>>();

			visited.Add(current, true);
			foreach(var link in current.Links){
				nexts.Add(link.To, link);
				visited[link.To] = false;
			}

			while(!visited.All(pair => pair.Value)){ // �S�ĖK��ς݂Ȃ�I��
				// �אڃ����N�����ԋ����̒Z�����̂�I��
				INodeLink<T> next = default(NodeLink<T>);
				int min = Int32.MaxValue;
				foreach(var link in nexts.Values){
					if(min > link.Distance){
						min = link.Distance;
						next = link;
					}
				}
				nexts.Remove(next.To);
				yield return next; // �S��؂ɒǉ�
				visited[next.To] = true; // �K��ς݂ɂ���

				// ���̌�⃊���N��nexts�֒ǉ�
				foreach(var link in next.To.Links){
					// �V�����o�������m�[�h��visited�ɒǉ�
					if(!visited.ContainsKey(link.To)){
						visited.Add(link.To, false);
					}
					if(!visited[link.To]){
						INodeLink<T> link2;
						if(nexts.TryGetValue(link.To, out link2)){
							// �����̒Z���ق������̃����N�ɂ���
							if(link.Distance < link2.Distance){
								nexts[link.To] = link;
							}
						}else{
							nexts.Add(link.To, link);
						}
					}
				}
			}
		}
	}
}