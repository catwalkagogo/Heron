/*
	$Id: ReadOnlyObservableList.cs 222 2011-06-23 07:17:01Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace CatWalk.Collections {
	public class WrappedReadOnlyObservableList<T> : WrappedReadOnlyObservableCollection<T>, IReadOnlyObservableList<T>{
		protected IReadOnlyList<T> List{
			get{
				return (IReadOnlyList<T>)this.Collection;
			}
		}

		public WrappedReadOnlyObservableList() : this(new ObservableCollection<T>()){
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="list">list must impliment INotifyCollectionChanged and INotifyPropertyChanged</param>
		public WrappedReadOnlyObservableList(IReadOnlyList<T> collection) : base(collection) {
		}

		#region IReadOnlyList<T> Members

		public T this[int index] {
			get {
				return this.List[index];
			}
		}

		#endregion
	}
}
