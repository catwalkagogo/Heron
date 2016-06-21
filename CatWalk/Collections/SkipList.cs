/*
	$Id: SkipList.cs 289 2011-08-12 10:03:33Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace CatWalk.Collections{
#if !SILVERLIGHT
	[Serializable]
#endif
	public class SkipList<T> : IList<T>{
		#region フィールド
		
		private const int P = Int32.MaxValue / 2;
		
		private SkipListNodeHeader head;
		private SkipListNodeHeader foot;
		private int count = 0;
		private const int maxLevel = Int32.MaxValue;
		private static Random random = new Random();
		
		#endregion
		
		#region コンストラクタ
		
		public SkipList(){
			this.Initialize();
		}
		
		private void Initialize(){
			this.count = 0;
			this.head = new SkipListNodeHeader();
			this.foot = new SkipListNodeHeader();
			this.head.Links.Add(new SkipListNodeLink(this.foot, 1));
			this.foot.Links.Add(new SkipListNodeLink(null, 0));
		}

		public SkipList(IEnumerable<T> collection){
			this.Initialize();
			foreach(var item in collection){
				this.Add(item);
			}
		}
		
		#endregion
		
		#region ロジック
		
		protected virtual int GetRandomLevel(){
			int newLevel = 0;
			while(random.Next() < P){
				newLevel++;
			}
			return Math.Min(newLevel, maxLevel);
		}
		
		private void SetLevel(int level){
			// 上位レベルの構築
			int newLevelCount = level - this.head.Links.Count;
			while(newLevelCount > 0){
				this.head.Links.Add(new SkipListNodeLink(this.foot, this.count + 1));
				this.foot.Links.Add(new SkipListNodeLink(null, 0));
				newLevelCount--;
			}
		}
		
		protected SkipListNode GetNodeAt(int index){
			// Headerの分インクリメント
			index++;
			
			SkipListNodeBase node = this.head;
			int level = this.head.Links.Count - 1;
			int d = 0;
			while(true){
				int t = d + node.Links[level].Distance;
				if(t == index){
					return (SkipListNode)node.Links[level].Node;
				}else if(t < index){
					d = t;
					node = node.Links[level].Node;
				}else{
					level--;
				}
			}
		}
		
		#endregion
		
		#region 関数
		
		protected void ClearEmptyLevels(){
			for(int level = this.head.Links.Count - 1; level > 0; level--){
				if(this.head.Links[level].Node == this.foot){
					this.head.Links.RemoveAt(level);
				}else{
					break;
				}
			}
		}
		
		public virtual void Add(T value){
			this.Insert(this.count, value);
		}
		
		public virtual void Insert(int index, T value){
			if((index < 0) || (this.count < index)){
				throw new ArgumentOutOfRangeException("index");
			}
			index++;
			
			int newLevel = GetRandomLevel();
			this.SetLevel(newLevel);
			
			var newNode = new SkipListNode(value);
			newNode.Links = new SkipListNodeLink[newLevel + 1];
			for(int i = 0; i <= newLevel; i++){
				newNode.Links[i] = new SkipListNodeLink(null, 0);
			}
			
			SkipListNodeBase node = this.head;
			var nodeIndex = 0;
			var level = this.head.Links.Count - 1;
			//var cost = 0;
			while(level >= 0){
				// 挿入位置検索
				var link = node.Links[level];
				while(link.Node != null){
					var nextIndex = nodeIndex + link.Distance;
					if(nextIndex < index){
						nodeIndex = nextIndex;
						node = link.Node;
						link = node.Links[level];
						//cost++;
					}else{
						break;
					}
				}
				
				if(level > newLevel){
					node.Links[level].Distance++;
				}else{
					// currentの後ろに繋げる
					var next = node.Links[level].Node;
					newNode.Links[level].Node = next;
					node.Links[level].Node = newNode;
					if(level >= 0){
						newNode.Links[level].Distance = nodeIndex + node.Links[level].Distance - index + 1;
						node.Links[level].Distance = index - nodeIndex;
					}
				}
				level--;
			}
			
			this.count++;
			//Console.WriteLine("added node cost:" + cost + " level:" + this.head.Links.Count + " newlevel:" + newLevel + " count:" + this.Count);
			//this.DebugPrint("added node:" + cost + " level:" + this.head.Links.Count);
		}
		
		public virtual bool Contains(T value){
			return (this.IndexOf(value) >= 0);
		}
		
		public virtual bool Remove(T item){
			int index = this.IndexOf(item);
			if(index >= 0){
				this.RemoveAt(index);
				return true;
			}else{
				return false;
			}
		}
		
		public virtual void RemoveAt(int index){
			if(index < 0 || this.count <= index){
				throw new ArgumentOutOfRangeException("index");
			}
			
			index++;
			
			SkipListNodeBase node = this.head;
			var nodeIndex = 0;
			var level = this.head.Links.Count - 1;
			while(level >= 0){
				// 削除位置検索
				var link = node.Links[level];
				while(link.Node != null){
					var nextIndex = nodeIndex + link.Distance;
					if(nextIndex < index){
						nodeIndex = nextIndex;
						node = link.Node;
						link = node.Links[level];
					}else{
						break;
					}
				}
				
				link = node.Links[level];
				if((nodeIndex + link.Distance) == index){
					link.Distance += link.Node.Links[level].Distance - 1;
					link.Node = link.Node.Links[level].Node;
				}else{
					link.Distance--;
				}
				level--;
			}
			this.count--;
			this.ClearEmptyLevels();
			//Console.WriteLine("Removed level:" + this.Head.Links.Count + " count:" + this.Count);
			//this.DebugPrint("Removed level:" + this.Head.Count);
		}
		
		public virtual int IndexOf(T value){
			var index = 0;
			//var cost = 0;
			foreach(var node in this.Nodes){
				if(node.Value.Equals(value)){
					//Console.WriteLine("Found: Cost:" + cost);
					return index;
				}
				//cost++;
				index++;
			}
			//Console.WriteLine("Found: Cost:" + cost);
			return -1;
		}
		
		public virtual void Clear(){
			this.Initialize();
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return this.GetEnumerator();
		}
		
		public virtual IEnumerator<T> GetEnumerator(){
			foreach(var node in this.Nodes){
				yield return node.Value;
			}
		}
		
		public virtual void CopyTo(T[] array, int arrayIndex){
			if(array == null){
				throw new ArgumentNullException();
			}
			if(arrayIndex < 0){
				throw new ArgumentOutOfRangeException();
			}
			if((array.Rank > 1) || (array.Length <= arrayIndex) || (this.Count > (array.Length - arrayIndex))){
				throw new ArgumentException();
			}
			int i = arrayIndex;
			foreach(T value in this){
				array[i] = value;
				i++;
			}
		}
		
#if DEBUG
		public void DebugPrint(string m){
			Console.WriteLine(m);
			int level = this.head.Links.Count - 1;
			while(level >= 0){
				SkipListNodeBase node = this.head;
				while(node != null){
					if(node == this.head){
						Console.Write("head");
					}else if(node == this.foot){
						Console.Write("foot");
					}else{
						Console.Write("{0,4}", node.ToString());
					}
					if(node.Links[level].Distance > 1){
						Console.Write("-{0,2}{1}", node.Links[level].Distance, new String(' ', (node.Links[level].Distance - 1) * 4 + node.Links[level].Distance - 3));
					}else{
						Console.Write(" ");
					}
					node = node.Links[level].Node;
				}
				Console.Write("\n");
				level--;
			}
		}
#endif
		#endregion
		
		#region プロパティ
		
		public virtual T this[int index]{
			get{
				return this.GetNodeAt(index).Value;
			}
			set{
				this.GetNodeAt(index).Value = value;
			}
		}
		
		public int Count{
			get{
				return this.count;
			}
			protected set{
				this.count = value;
			}
		}
		
		public virtual bool IsReadOnly{
			get{
				return false;
			}
		}
		
		protected SkipListNodeHeader Head{
			get{
				return this.head;
			}
			set{
				this.head = value;
			}
		}

		protected SkipListNodeHeader Foot{
			get{
				return this.foot;
			}
			set{
				this.foot = value;
			}
		}

		protected IEnumerable<SkipListNode> Nodes{
			get{
				SkipListNodeBase node = this.head.Links[0].Node;
				while(node != this.foot){
					yield return (SkipListNode)node;
					node = node.Links[0].Node;
				}
			}
		}

		#endregion
		
		#region クラス
		
#if !SILVERLIGHT
		[Serializable]
#endif
		protected abstract class SkipListNodeBase{
			public IList<SkipListNodeLink> Links{get; set;}

			public SkipListNodeBase(){
			}
		}
	
		protected class SkipListNode : SkipListNodeBase{
			public T Value{get; set;}
			
			public SkipListNode(){
			}
			
			public SkipListNode(T value){
				this.Value = value;
			}
		}

		protected class SkipListNodeHeader : SkipListNodeBase{
			public SkipListNodeHeader(){
				this.Links = new List<SkipListNodeLink>();
			}
		}
		
#if !SILVERLIGHT
		[Serializable]
#endif
		protected class SkipListNodeLink{
			public SkipListNodeBase Node{get; set;}
			public int Distance{get; set;}
			
			public SkipListNodeLink(SkipListNodeBase node, int distance){
				this.Node = node;
				this.Distance = distance;
			}
		}
		
		#endregion
	}	
}