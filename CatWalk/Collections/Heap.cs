/*
	$Id: Heap.cs 330 2014-01-14 15:24:19Z catwalkagogo@gmail.com $
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
	public class Heap<T> : IEnumerable<T>, ICollection, IReadOnlyCollection<T>{
		#region フィールド
		
		private List<T> list;
		private IComparer<T> comparer;
		private bool isMaxHeap;
		
		#endregion
		
		#region コンストラクタ
		
		public Heap() : this(Comparer<T>.Default, false){
			this.list = new List<T>();
		}
		
		public Heap(IComparer<T> comparer) : this(comparer, false){
			this.list = new List<T>();
		}
		
		public Heap(bool isMaxHeap) : this(Comparer<T>.Default, isMaxHeap){
			this.list = new List<T>();
		}
		
		public Heap(IComparer<T> comparer, bool isMaxHeap){
			this.list = new List<T>();
			if(comparer == null){
				throw new ArgumentNullException();
			}
			this.comparer = comparer;
			this.isMaxHeap = isMaxHeap;
		}
		
		public Heap(int capacity) : this(Comparer<T>.Default, false){
			this.list = new List<T>(capacity);
		}
		
		public Heap(int capacity, IComparer<T> comparer) : this(comparer, false){
			this.list = new List<T>(capacity);
		}
		
		public Heap(int capacity, bool isMaxHeap) : this(Comparer<T>.Default, isMaxHeap){
			this.list = new List<T>(capacity);
		}
		
		public Heap(int capacity, IComparer<T> comparer, bool isMaxHeap) : this(comparer, isMaxHeap){
			this.list = new List<T>(capacity);
		}
		
		#endregion
		
		#region 関数
		
		public void Push(T item){
			int i = this.list.Count;
			this.list.Add(item);
			while(i != 0){
				int parentIndex = (i - 1) >> 2;
				T parent = this.list[parentIndex];
				if(this.Compare(parent, item) > 0){	// iが親より小さければ入れ替え
					this.Swap(i, parentIndex);
					i = parentIndex;
				}else{	// iが親より大きければ終了
					break;
				}
			}
		}
		
		public T Pop(){
			if(list.Count > 0){
				return this.PopImpl();
			}else{	// 要素が無いときはエラー
				throw new InvalidOperationException();
			}
		}
		
		public T PopOrDefault(){
			if(list.Count > 0){
				return this.PopImpl();
			}else{
				return default(T);
			}
		}
		
		private T PopImpl(){
			T item = this.list[0];
			int lastIndex = this.list.Count - 1;
			this.list[0] = this.list[lastIndex];
			this.list.RemoveAt(lastIndex);
			int i = 0;
			int count = this.list.Count;
			while(true){
				int left  = (i << 1) + 1;
				int right = (i << 1) + 2;
				int child;
				T childValue;
				if(left >= count){	// 子要素がない(終了)
					break;
				}else if(right >= count){	// 右の子要素がない
					child = left;
					childValue = this.list[left];
				}else{	// どちらもない場合は小さい方
					T leftValue = this.list[left];
					T rightValue = this.list[right];
					if(this.Compare(leftValue, rightValue) < 0){
						child = left;
						childValue = leftValue;
					}else{
						child = right;
						childValue = rightValue;
					}
				}
				if(this.Compare(this.list[i], childValue) > 0){	// iが子より大きければ入れ替え
					this.Swap(i, child);
					i = child;
				}else{	// iが子より小さければ終了
					break;
				}
			}
			return item;
		}
		
		public T Peek(){
			if(this.list.Count == 0){
				throw new InvalidOperationException();
			}
			return this.list[0];
		}
		
		public T PeekOrDefault(){
			if(this.list.Count == 0){
				return default(T);
			}
			return this.list[0];
		}
		
		public void Clear(){
			this.list.Clear();
		}
		
		public void CopyTo(T[] array, int index){
			this.list.CopyTo(array, index);
		}
		
		void ICollection.CopyTo(Array array, int index){
			((ICollection)this.list).CopyTo(array, index);
		}
		
		public T[] ToArray(){
			return this.list.ToArray();
		}
		
		public void TrimExcess(){
			this.list.TrimExcess();
		}
		
		private void Swap(int x, int y){
			T temp = this.list[y];
			this.list[y] = this.list[x];
			this.list[x] = temp;
		}
		
		private int Compare(T x, T y){
			if(this.isMaxHeap){
				return this.comparer.Compare(y, x);
			}else{
				return this.comparer.Compare(x, y);
			}
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return this.list.GetEnumerator();
		}
		
		public IEnumerator<T> GetEnumerator(){
			return this.list.GetEnumerator();
		}
		
		#endregion
		
		#region プロパティ
		
		public int Count{
			get{
				return this.list.Count;
			}
		}
		
		public bool IsMaxHeap{
			get{
				return this.isMaxHeap;
			}
		}
		
		bool ICollection.IsSynchronized{
			get{
				return ((ICollection)this.list).IsSynchronized;
			}
		}
		
		object ICollection.SyncRoot{
			get{
				return ((ICollection)this.list).SyncRoot;
			}
		}
		
		#endregion
	}
}