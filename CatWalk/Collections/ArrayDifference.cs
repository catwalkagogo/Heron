/*
	$Id: ArrayDifference.cs 281 2011-08-05 10:07:00Z catwalkagogo@gmail.com $
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace CatWalk.Collections{
	[Serializable]
	public struct ArrayDifference<T>{
		//public T[] Source{get; private set;}
		//public T[] Destination{get; private set;}
		public IList<T> RemovedItems{get; private set;}
		public IList<T> AddedItems{get; private set;}
		public IList<T> RemainItems{get; private set;}

		public ArrayDifference(IEnumerable<T> source, IEnumerable<T> destination, EqualityComparer<T> eqComp) : this(){
			source.ThrowIfNull("source");
			destination.ThrowIfNull("destination");
			eqComp.ThrowIfNull("comparer");
			
			var dstSet = new HashSet<T>(destination, eqComp);
			var removedItems = new List<T>();
			var remainItems = new List<T>();
			foreach(var srcItem in source){
				if(!dstSet.Remove(srcItem)){
					remainItems.Add(srcItem);
				}else{
					removedItems.Add(srcItem);
				}
			}
			var addedItems = dstSet.ToArray();
			this.RemainItems = remainItems;
			this.AddedItems = addedItems;
			this.RemovedItems = removedItems;
		}
	
		internal ArrayDifference(IList<T> removed, IList<T> added, IList<T> remain) : this(){
			this.RemovedItems = removed;
			this.AddedItems = added;
			this.RemainItems = remain;
		}
	}

	public static class ArrayDifferenceExtension{
		public static ArrayDifference<T> ToDiff<T>(this IEnumerable<T> source, IEnumerable<T> destination){
			return ToDiff(source, destination, EqualityComparer<T>.Default);
		}

		public static ArrayDifference<T> ToDiff<T>(this IEnumerable<T> source, IEnumerable<T> destination, EqualityComparer<T> eqComp){
			return new ArrayDifference<T>(source, destination, eqComp);
		}
	}
}