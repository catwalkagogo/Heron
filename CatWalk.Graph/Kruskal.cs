/*
	$Id: Kruskal.cs 316 2013-12-26 10:16:12Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using CatWalk.Collections;
using System.Linq;

namespace CatWalk.Graph{
	public static partial class Graph {

		public static IEnumerable<INodeLink<T>> GetMinimumSpanningTree<T>(this IEnumerable<INode<T>> nodes){
			// ��ԒZ�������N�����o�����߂̃q�[�v
			var heap = new Heap<INodeLink<T>>(new LambdaComparer<INodeLink<T>>(delegate(INodeLink<T> x, INodeLink<T> y) {
				return x.Distance.CompareTo(y.Distance);
			}));
			// �S��؂̑傫���͑S�m�[�h�� - 1
			int len = -1;
			foreach(var node in nodes){
				len++;
				foreach(var link in node.Links){
					heap.Push(link);
				}
			}
			// �����؂̏W��
			var forest = new HashSet<HashSet<INode<T>>>();

			int count = 0;
			while(count < len){ // �S��؂̑傫���ɂȂ�܂�
				// ��ԋ����̒Z�������N�����o��
				var link = heap.Pop();
				// �����N�����܂ޖ؂����o��
				var tree1 = forest.FirstOrDefault(tree => tree.Contains(link.From));
				// �����N����܂ޖ؂����o��
				var tree2 = forest.FirstOrDefault(tree => tree.Contains(link.To));
				if(tree1 != tree2){ // �قȂ�ؓ��m�̂Ƃ�
					if(tree1 == null){ // �����N���̖؂�������Ȃ������Ƃ�
						yield return link; count++;
						tree2.Add(link.From); // �����N��̖؂Ƀ����N���̃m�[�h���܂߂�
					}else{
						if(tree2 == null){ // �����N��̖؂�������Ȃ������Ƃ�
							yield return link; count++;
							tree1.Add(link.To); // �����N���̖؂Ƀ����N��̃m�[�h���܂߂�
						}else{ // �������������Ƃ�
							yield return link; count++;
							// 2�̖؂���������
							foreach(var node in tree2){
								tree1.Add(node);
							}
							forest.Remove(tree2);
						}
					}
				}else if((tree1 == null) && (tree2 == null)){ // �ǂ����������Ȃ������Ƃ�
					yield return link; count++;
					// �V�����؂����B
					var tree = new HashSet<INode<T>>();
					tree.Add(link.From);
					tree.Add(link.To);
					forest.Add(tree);
				} // �����ؓ��m�̏ꍇ�͉������Ȃ�
			}
		}
	}
}