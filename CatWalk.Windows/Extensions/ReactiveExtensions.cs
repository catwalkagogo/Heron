using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CatWalk.Windows.Extensions {
	public static class ReactiveExtensions {
		public static IObservable<T> ObserveProperty<T>(this DependencyObject dependencyObject, DependencyProperty property) {
			return Observable.Create<T>(o => {
				var des = DependencyPropertyDescriptor.FromProperty(property, dependencyObject.GetType());
				var eh = new EventHandler((s, e) => o.OnNext((T)des.GetValue(dependencyObject)));
				des.AddValueChanged(dependencyObject, eh);
				return () => des.RemoveValueChanged(dependencyObject, eh);
			});
		}
	}
}
