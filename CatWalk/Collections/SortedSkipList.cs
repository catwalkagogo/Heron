using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.Collections {
#if !SILVERLIGHT
	[Serializable]
#endif
	public class SortedSkipList<T> : SkipList<T>, ISet<T>{
		private IComparer<T> comparer;
		public bool IsAllowDuplicates{get; private set;}
		
		public SortedSkipList() : this(new T[0], Comparer<T>.Default, false){}
		public SortedSkipList(bool isAllowDuplicates) : this(new T[0], Comparer<T>.Default, isAllowDuplicates){}
		public SortedSkipList(IComparer<T> comparer) : this(new T[0], comparer, false){}
		public SortedSkipList(IComparer<T> comparer, bool isAllowDuplicates) : this(new T[0], comparer, isAllowDuplicates){}
		public SortedSkipList(IEnumerable<T> source) : this(source, Comparer<T>.Default, false){}
		public SortedSkipList(IEnumerable<T> source, bool isAllowDuplicates) : this(source, Comparer<T>.Default, isAllowDuplicates){}
		public SortedSkipList(IEnumerable<T> source, IComparer<T> comparer) : this(source, comparer, false){}
		public SortedSkipList(IEnumerable<T> source, IComparer<T> comparer, bool isAllowDuplicates){
			if(source == null){
				throw new ArgumentNullException("source");
			}
			this.comparer = comparer;
			this.IsAllowDuplicates = isAllowDuplicates;
			foreach(var item in source){
				this.Add(item);
			}
		}
		
		public override void Insert(int index, T item){
			throw new NotSupportedException();
		}
		
		protected virtual void BaseInsert(int index, T item){
			base.Insert(index, item);
		}
		
		public override void Add(T item){
			var idx = this.IndexOf(item);
			if(idx >= 0){
				this.BaseInsert(idx, item);
			}else{
				this.BaseInsert(~idx, item);
			}
		}
		
		public override int IndexOf(T item){
			SkipListNodeBase node = this.Head;
			var level = this.Head.Links.Count - 1;
			var index = 0;
			var link = node.Links[level];
			//var cost = 0;
			while(link.Node != this.Foot){
				//cost++;
				int d = this.comparer.Compare(item, ((SkipListNode)link.Node).Value);
				if(d < 0){ // item is smaller
					if(level > 0){
						level--;
						link = node.Links[level];
					}else{
						//Console.WriteLine("Found: Cost:" + cost);
						return ~index;
					}
				}else if(d > 0){ // item is bigger
					// find next
					node = link.Node;
					index += link.Distance;
					link = node.Links[level];
					
					// if next is footer find below
					while((level > 0) && (link.Node == this.Foot)){
						level--;
						link = node.Links[level];
					}
				}else{
					//node = link.Node;
					index += link.Distance;
					//link = node.Links[level];
					//Console.WriteLine("Found: Cost:" + cost);
					return index - 1;
				}
			}
			//Console.WriteLine("Found: Cost:" + cost);
			return ~index;
		}

		public override T this[int index] {
			get {
				return base[index];
			}
			set {
				throw new NotSupportedException();
			}
		}

		#region ISet<T>

		bool ISet<T>.Add(T item) {
			if(this.IsAllowDuplicates){
				throw new InvalidOperationException();
			}
			var count = this.Count;
			this.Add(item);
			return count != this.Count;
		}

		public void ExceptWith(IEnumerable<T> other) {
			if(this.IsAllowDuplicates){
				throw new InvalidOperationException();
			}
			foreach(var item in other){
				this.Remove(item);
			}
		}

		public void IntersectWith(IEnumerable<T> other) {
			if(this.IsAllowDuplicates){
				throw new InvalidOperationException();
			}
			var set = new HashSet<T>(other);
			foreach(var item in this.Where(v => !set.Contains(v))){
				this.Remove(item);
			}
		}

		public bool IsProperSubsetOf(IEnumerable<T> other) {
			if(this.IsAllowDuplicates){
				throw new InvalidOperationException();
			}
			var set = new HashSet<T>(other);
			return set.IsProperSupersetOf(this);
		}

		public bool IsProperSupersetOf(IEnumerable<T> other) {
			if(this.IsAllowDuplicates){
				throw new InvalidOperationException();
			}
			var set = new HashSet<T>(other);
			return set.IsProperSubsetOf(other);
		}

		public bool IsSubsetOf(IEnumerable<T> other) {
			if(this.IsAllowDuplicates){
				throw new InvalidOperationException();
			}
			var set = new HashSet<T>(this);
			return set.IsSubsetOf(other);
		}

		public bool IsSupersetOf(IEnumerable<T> other) {
			if(this.IsAllowDuplicates){
				throw new InvalidOperationException();
			}
			return other.All(v => this.Contains(v));
		}

		public bool Overlaps(IEnumerable<T> other) {
			if(this.IsAllowDuplicates){
				throw new InvalidOperationException();
			}
			return other.Any(v => this.Contains(v));
		}

		public bool SetEquals(IEnumerable<T> other) {
			if(this.IsAllowDuplicates){
				throw new InvalidOperationException();
			}
			var set = new HashSet<T>(other);
			return other.All(v => this.Contains(v)) && this.All(v => set.Contains(v));
		}

		public void SymmetricExceptWith(IEnumerable<T> other) {
			if(this.IsAllowDuplicates){
				throw new InvalidOperationException();
			}
			var set = new HashSet<T>(other);
			foreach(var item in set){
				if(this.Contains(item) && set.Contains(item)){
					this.Remove(item);
				}
			}
		}

		public void UnionWith(IEnumerable<T> other) {
			if(this.IsAllowDuplicates){
				throw new InvalidOperationException();
			}
			foreach(var item in other){
				this.Add(item);
			}
		}

		#endregion
	}

}
