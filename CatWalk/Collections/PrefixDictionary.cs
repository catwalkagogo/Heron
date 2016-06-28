/*
	$Id: PrefixDictionary.cs 222 2011-06-23 07:17:01Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace CatWalk.Collections{
	public class PrefixDictionary<T> : IDictionary<string, T>{
		#region フィールド
		
		private PrefixTreeNode root = new PrefixTreeNode();
		private IComparer<char> comparer = null;
		private int count = 0;

		#endregion
		
		#region コンストラクタ
		
		public PrefixDictionary(){
			this.Initialize(Comparer<char>.Default, null);}
		public PrefixDictionary(IDictionary<string, T> dict){
			dict.ThrowIfNull("dict");this.Initialize(Comparer<char>.Default, dict);}
		public PrefixDictionary(IComparer<char> comparer){
			this.Initialize(comparer, null);}
		public PrefixDictionary(IComparer<char> comparer, IDictionary<string, T> dict){
			dict.ThrowIfNull("dict");this.Initialize(comparer, dict);}

		private void Initialize(IComparer<char> comparer, IDictionary<string, T> dict){
			comparer.ThrowIfNull("comparer");
			this.comparer = comparer;
			if(dict != null){
				foreach(var pair in dict){
					this.Add(pair.Key, pair.Value);
				}
			}
		}
		
		#endregion
		
		#region ロジック
		
		/// <summary>
		/// keyに一致するノードを検索する。
		/// </summary>
		/// <param name="node">ルート</param>
		/// <param name="key">キー</param>
		/// <param name="index">キーの文字位置</param>
		/// <returns>見つかったノード</returns>
		private IEnumerable<PrefixTreeNode> FindNodes(PrefixTreeNode node, string key, int index){
			if(index == key.Length){
				yield return node;
			}else{
				var c = key[index];
				if(node.Children != null){
					foreach(var child in node.Children){
						var d = this.comparer.Compare(c, child.Char);
						if(d < 0){ // c < node.Char
							break;
						}else if(d == 0){
							foreach(var found in this.FindNodes(child, key, index + 1)){
								yield return found;
							}
						}
					}
				}
			}
		}
		
		protected static void CheckKey(string key){
			if(key == null){
				throw new ArgumentNullException();
			}
		}
		
		#endregion
		
		#region 関数
		
		public IEnumerable<KeyValuePair<string,T>> Search(string key){
			return this.Search(key, false);
		}

		public IEnumerable<KeyValuePair<string,T>> Search(string key, bool includeEmptyEntry){
			CheckKey(key);
			foreach(var found in this.FindNodes(this.root, key, 0)){
				if(includeEmptyEntry || found.Entry.HasValue){
					yield return found.Entry.Value;
				}
				foreach(var sub in found.SubNodes){
					if(includeEmptyEntry || sub.Entry.HasValue){
						yield return sub.Entry.Value;
					}
				}
			}
		}
		
		public void Add(KeyValuePair<string, T> pair){
			this.Add(pair.Key, pair.Value);
		}
		
		public void Add(string key, T value){
			CheckKey(key);
			if(key.Length == 0){
				this.root.Entry = new KeyValuePair<string, T>(key, value);
				this.count++;
				return;
			}
			
			PrefixTreeNode node = this.root;
			int index = 0;
			int lastIndex = key.Length - 1;
			while(true){
				//Debug.WriteLine("Add({0}): loop index={1}", key, index);
				PrefixTreeNode found = null;
				LinkedListNode<PrefixTreeNode> right = null;
				LinkedListNode<PrefixTreeNode> llNode = (node.Children != null) ? node.Children.First : null;
				while(llNode != null){
					PrefixTreeNode child = llNode.Value;
					int d = this.comparer.Compare(key[index], child.Char);
					//Debug.WriteLine("Add({0}): find child.Char={1}, d={2}", key, child.Char, d);
					if(d == 0){
						found = child;
						break;
					}else if(d < 0){
						right = llNode;
						break;
					}
					llNode = llNode.Next;
				}
				
				if(found != null){
					if(index == lastIndex){
						//Debug.WriteLine("Add({0}): set found={1}", key, found.Key);
						if(found.Entry.HasValue){
							throw new ArgumentException("key");
						}
						found.Entry = new KeyValuePair<string, T>(key, value);
						this.count++;
						break;
					}else{
						//Debug.WriteLine("Add({0}): node=found={1}", key, found.Key);
						node = found;
					}
				}else{
					PrefixTreeNode newNode = new PrefixTreeNode(node, key[index]);
					//Debug.WriteLine("Add({0}): create newNode={1}", key, newNode.Key);
					if(node.Children == null){
						node.Children = new LinkedList<PrefixTreeNode>();
					}
					if(right != null){
						node.Children.AddBefore(right, newNode);
					}else{
						node.Children.AddLast(newNode);
					}
					if(index == lastIndex){
						//Debug.WriteLine("Add({0}): set newNode={1}", key, newNode.Key);
						newNode.Entry = new KeyValuePair<string, T>(key, value);
						this.count++;
						break;
					}else{
						//Debug.WriteLine("Add({0}): node=newNode={1}", key, newNode.Key);
						node = newNode;
					}
				}
				index++;
			}
		}
		
		public bool Remove(KeyValuePair<string, T> pair){
			return this.Remove(pair.Key);
		}
		
		public bool Remove(string key){
			CheckKey(key);
			var founds = this.FindNodes(this.root, key, 0).ToArray();
			foreach(var node in founds){
				if((node.Children != null) && (node.Children.Count > 0)){
					if(node.Entry.HasValue){
						node.Entry = null;
					}
				}else{
					node.Parent.Children.Remove(node);
					if(node.Parent.Children.Count == 0){
						node.Parent.Children = null;
					}
				}
				this.count--;
			}
			return (founds.Length > 0);
		}
		
		public bool Contains(KeyValuePair<string, T> pair){
			T item;
			if(this.TryGetValue(pair.Key, out item)){
				return item.Equals(pair.Value);
			}else{
				return false;
			}
		}
		
		public bool ContainsKey(string key){
			var found = this.FindNodes(this.root, key, 0).FirstOrDefault();
			if(found != null){
				return found.Entry.HasValue;
			}else{
				return false;
			}
		}
		
		public bool TryGetValue(string key, out T item){
			var found = this.FindNodes(this.root, key, 0).FirstOrDefault();
			if(found != null){
				if(found.Entry.HasValue){
					item = found.Entry.Value.Value;
					return true;
				}
			}
			item = default(T);
			return false;
		}
		
		public void Clear(){
			if(this.root.Children != null){
				this.root.Children.Clear();
			}
			this.root.Entry = null;
			this.count = 0;
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return this.GetEnumerator();
		}
		
		public IEnumerator<KeyValuePair<string,T>> GetEnumerator(){
			foreach(PrefixTreeNode node in this.Nodes){
				if(node.Entry.HasValue){
					yield return node.Entry.Value;
				}
			}
		}
		
		public void CopyTo(KeyValuePair<string,T>[] array, int arrayIndex){
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
			foreach(PrefixTreeNode node in this.Nodes){
				if(node.Entry.HasValue){
					array[i] = node.Entry.Value;
					i++;
				}
			}
		}
		
		#endregion
		
		#region プロパティ
		
		public int Count{
			get{
				return this.count;
			}
		}
		
		public T this[string key]{
			get{
				CheckKey(key);
				var found = this.FindNodes(this.root, key, 0).FirstOrDefault();
				if(found != null){
					if(found.Entry.HasValue){
						return found.Entry.Value.Value;
					}
				}
				throw new KeyNotFoundException();
			}
			set{
				CheckKey(key);
				var found = this.FindNodes(this.root, key, 0).FirstOrDefault();
				if(found != null){
					found.Entry = new KeyValuePair<string, T>(key, value);
				}else{
					this.Add(key, value);
				}
			}
		}
		
		public ICollection<string> Keys{
			get{
				var list = new Collection<string>();
				foreach(PrefixTreeNode node in this.Nodes){
					if(node.Entry.HasValue){
						list.Add(node.Entry.Value.Key);
					}
				}
				return list;
			}
		}
		
		public ICollection<T> Values{
			get{
				var list = new Collection<T>();
				foreach(PrefixTreeNode node in this.Nodes){
					if(node.Entry.HasValue){
						list.Add(node.Entry.Value.Value);
					}
				}
				return list;
			}
		}
		
		public bool IsReadOnly{
			get{
				return false;
			}
		}
		
		private IEnumerable<PrefixTreeNode> Nodes{
			get{
				if(this.root.Entry.HasValue){
					yield return this.root;
				}
				foreach(var node in this.root.SubNodes){
					yield return node;
				}
			}
		}
		
		#endregion

		#region 内部クラス

		private class PrefixTreeNode{
			#region フィールド

			public char Char{get; private set;}
			public PrefixTreeNode Parent{get; private set;}
			public LinkedList<PrefixTreeNode> Children{get; set;}
			private KeyValuePair<string, T>? entry;

			#endregion

			#region コンストラクタ

			public PrefixTreeNode(){
			}

			public PrefixTreeNode(PrefixTreeNode parent, char c){
				//if(parent == null){
				//	throw new ArgumentNullException();
				//}
				this.Parent = parent;
				this.Char = c;
				//this.Children = new LinkedList<PrefixTreeNode>();
			}

			public PrefixTreeNode(PrefixTreeNode parent, string key, T value){
				this.Parent = parent;
				this.entry = new KeyValuePair<string,T>(key, value);
				this.Char = key[key.Length - 1];
			}

			#endregion

			#region 関数

			/// <summary>
			/// キー文字列を取得する。
			/// </summary>
			public string GetKey(){
				PrefixTreeNode[] nodes =this.PrefixNodes.ToArray();
				if(nodes.Length > 0){
					char[] chars = new char[nodes.Length + 1];
					for(int i = nodes.Length - 1, j = 0; j < nodes.Length; i--, j++){
						chars[j] = nodes[i].Char;
					}
					chars[nodes.Length] = this.Char;
					return new String(chars);
				} else{
					if(this.Char == '\0'){
						return String.Empty;
					} else{
						return this.Char.ToString();
					}
				}
			}


			#endregion

			#region プロパティ

			/// <summary>
			/// プレフィックスとなる上位ノードを取得する。
			/// 自分自身を含まない。
			/// </summary>
			public IEnumerable<PrefixTreeNode> PrefixNodes{
				get{
					PrefixTreeNode node = this;
					PrefixTreeNode parent = this.Parent;
					while((parent != null) && (parent.Parent != null)){
						yield return parent;
						parent = parent.Parent;
					}
				}
			}

			/// <summary>
			/// 同じ長さで同じプレフィックスを持つノードを取得する。
			/// 自分自身を含む。
			/// </summary>
			public IEnumerable<PrefixTreeNode> ColumnNodes{
				get{
					PrefixTreeNode parent = this.Parent;
					if(parent != null){
						foreach(PrefixTreeNode node in parent.Children){
							yield return node;
						}
					}
				}
			}

			/// <summary>
			/// 自分をプレフィックスとするノードを取得する。
			/// 自分自身を含まない。
			/// </summary>
			public IEnumerable<PrefixTreeNode> SubNodes{
				get{
					if(this.Children != null){
						foreach(PrefixTreeNode node in this.Children){
							yield return node;
							foreach(PrefixTreeNode node2 in node.SubNodes){
								yield return node2;
							}
						}
					}
				}
			}

			public KeyValuePair<string, T>? Entry{
				get{
					return this.entry;
				}
				set{
					this.entry = value;
				}
			}

			#endregion
		}
		#endregion
	}
	
	interface ISplitter<TSrc, TDst>{
		TDst[] Split(TSrc src);
		TSrc Join(TDst[] src);
	}
	
	public class StringSplitter : ISplitter<string, char>{
		public char[] Split(string src){
			return src.ToCharArray();
		}
		
		public string Join(char[] src){
			return new String(src);
		}
	}
}