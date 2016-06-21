/*
	$Id: PriorityQueue.cs 33 2010-06-11 05:47:26Z catwalkagogo@gmail.com $
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
	[Serializable]
	public class PriorityQueue<T> : Heap<PriorityQueue<T>.PriorityItem>{
		#region コンストラクタ
		
		public PriorityQueue() : base(new PriorityComparer()){
		}
		
		public PriorityQueue(bool isMaxHeap) : base(new PriorityComparer(), isMaxHeap){
		}
		
		public PriorityQueue(int capacity) : base(capacity, new PriorityComparer()){
		}
		
		public PriorityQueue(int capacity, bool isMaxHeap) : base(capacity, new PriorityComparer(), isMaxHeap){
		}
		
		#endregion
		
		#region クラス
		
		public class PriorityItem{
			public int Priority{get; private set;}
			public T Value{get; set;}
			
			public PriorityItem(int priority, T value){
				this.Priority = priority;
				this.Value = value;
			}
		}
		
		private class PriorityComparer : IComparer<PriorityItem>{
			public int Compare(PriorityItem x, PriorityItem y){
				return y.Priority.CompareTo(x);
			}
		}
		
		#endregion
	}
}