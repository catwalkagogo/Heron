using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Collections;
using System.Windows;

namespace CatWalk.Windows {
	public static class ObservableCollectionUtility {
		public static ObservableCollectionWeakSynchronizer NotifyToCollectionWeak<T>(this IEnumerable<T> source, ICollection<T> dest) {
			return source.NotifyToCollectionWeak(dest, v => true, v => v);
		}
		public static ObservableCollectionWeakSynchronizer NotifyToCollectionWeak<T>(this IEnumerable<T> source, ICollection<T> dest, Func<T, bool> predicate) {
			return source.NotifyToCollectionWeak(dest, predicate, v => v);
		}
		public static ObservableCollectionWeakSynchronizer NotifyToCollectionWeak<TSource, TDest>(this IEnumerable<TSource> source, ICollection<TDest> dest, Func<TSource, TDest> selector) {
			return source.NotifyToCollectionWeak(dest, v => true, selector);
		}
		public static ObservableCollectionWeakSynchronizer NotifyToCollectionWeak<TSource, TDest>(this IEnumerable<TSource> source, ICollection<TDest> dest, Func<TSource, bool> predicate, Func<TSource, TDest> selector) {
			return new ObservableCollectionWeakSynchronizer(source, dest, v => predicate((TSource)v), v => selector((TSource)v));
		}

		public static ObservableCollectionWeakSynchronizer NotifyToCollectionWeak(this IEnumerable source, IEnumerable dest) {
			return source.NotifyToCollectionWeak(dest, v => true, v => v);
		}
		public static ObservableCollectionWeakSynchronizer NotifyToCollectionWeak(this IEnumerable source, IEnumerable dest, Func<object, bool> predicate) {
			return source.NotifyToCollectionWeak(dest, predicate, v => v);
		}
		public static ObservableCollectionWeakSynchronizer NotifyToCollectionWeak(this IEnumerable source, IEnumerable dest, Func<object, object> selector) {
			return source.NotifyToCollectionWeak(dest, v => true, selector);
		}
		public static ObservableCollectionWeakSynchronizer NotifyToCollectionWeak(this IEnumerable source, IEnumerable dest, Func<object, bool> predicate, Func<object, object> selector) {
			return new ObservableCollectionWeakSynchronizer(source, dest, predicate, selector);
		}
	}

	public class ObservableCollectionWeakSynchronizer : CollectionSynchronizer, IWeakEventListener{
		public ObservableCollectionWeakSynchronizer(IEnumerable source, IEnumerable dest, Func<object, bool> predicate, Func<object, object> selector)
			: base(source, dest, predicate, selector){

		}

		public override void Start() {
			CollectionChangedEventManager.AddListener(this.SourceNotifyCollectionChanged, this);
		}

		public override void Stop() {
			CollectionChangedEventManager.RemoveListener(this.SourceNotifyCollectionChanged, this);
		}


		#region IWeakEventListener Members

		public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			this.OnSourceCollectionChanged((NotifyCollectionChangedEventArgs)e);
			return true;
		}

		#endregion
	}
}
