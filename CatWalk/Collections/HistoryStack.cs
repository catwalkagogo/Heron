/*
	$Id: History.cs 303 2010-01-16 02:53:09Z catwalk $
*/

using System;
using System.Collections.Generic;

namespace CatWalk.Collections{
	public interface IHistoryStack<T>{
		void Add(T item);
		void Clear();
		T Forward();
		T Back();
		bool CanGoForward{get;}
		bool CanGoBack{get;}
		T Current{get;}
	}
	
	public class HistoryStack<T> : IHistoryStack<T>{
		private LinkedList<T> list = new LinkedList<T>();
		private LinkedListNode<T> current = null;
		private int limitCount = 0;
		
		public HistoryStack(){
		}
		
		public HistoryStack(int limitCount){
			this.limitCount = limitCount;
		}
		
		public void Add(T item){
			if(this.list.Count == 0){
				this.list.AddFirst(item);
				this.current = this.list.First;
			}else{
				if(this.current != null){
					while(this.current.Next != null){
						this.list.Remove(this.current.Next);
					}
				}
				this.list.AddAfter(this.current, item);
				this.current = this.current.Next;
			}
			if((this.limitCount > 0) && (this.list.Count > this.limitCount)){
				this.list.Remove(this.list.First);
			}
		}
		
		public void Insert(T item){
			if(this.list.Count == 0){
				this.list.AddFirst(item);
				this.current = this.list.First;
			}else{
				this.list.AddAfter(this.current, item);
				this.current = this.current.Next;
			}
			if((this.limitCount > 0) && (this.list.Count > this.limitCount)){
				this.list.Remove(this.list.First);
			}
		}
		
		public void Clear(){
			this.list.Clear();
			this.current = null;
		}
		
		public T Forward(){
			if(this.CanGoForward){
				this.current = this.current.Next;
				return this.current.Value;
			}else{
				return default(T);
			}
		}
		
		public T Back(){
			if(this.CanGoBack){
				this.current = this.current.Previous;
				return this.current.Value;
			}else{
				return default(T);
			}
		}
		
		public T Current{
			get{
				return this.current.Value;
			}
			set{
				if(this.current == null){
					this.list.AddFirst(value);
					this.current = this.list.First;
				}else{
					this.current.Value = value;
				}
			}
		}
		
		public T[] ForwardHistory{
			get{
				if(this.current != null){
					List<T> list = new List<T>();
					LinkedListNode<T> node = this.current.Next;
					while(node != null){
						list.Add(node.Value);
						node = node.Next;
					}
					return list.ToArray();
				}else{
					return new T[0];
				}
			}
			set{
				if(this.current == null){
					this.list.AddFirst(default(T));
					this.current = this.list.First;
				}
				LinkedListNode<T> node = this.current;
				foreach(T item in value){
					this.list.AddAfter(node, item);
					node = node.Next;
					if((this.limitCount > 0) && (this.list.Count == this.limitCount)){
						break;
					}
				}
			}
		}
		
		public T[] BackHistory{
			get{
				if(this.current != null){
					List<T> list = new List<T>();
					LinkedListNode<T> node = this.current.Previous;
					while(node != null){
						list.Add(node.Value);
						node = node.Previous;
					}
					return list.ToArray();
				}else{
					return new T[0];
				}
			}
			set{
				if(this.current == null){
					this.list.AddFirst(default(T));
					this.current = this.list.First;
				}
				LinkedListNode<T> node = this.current;
				foreach(T item in value){
					this.list.AddBefore(node, item);
					node = node.Previous;
					if((this.limitCount > 0) && (this.list.Count == this.limitCount)){
						break;
					}
				}
			}
		}
		
		public bool CanGoForward{
			get{
				return ((this.current != null) || (this.current.Next != null));
			}
		}
		
		public bool CanGoBack{
			get{
				return ((this.current != null) || (this.current.Previous != null));
			}
		}
		
		public int LimitCount{
			get{
				return this.limitCount;
			}
			set{
				this.limitCount = value;
			}
		}
	}
}