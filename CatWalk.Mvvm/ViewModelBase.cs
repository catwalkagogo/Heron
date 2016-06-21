/*
	$Id: ViewModelBase.cs 190 2011-03-30 10:44:37Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace CatWalk.Mvvm{
	public abstract class ViewModelBase : INotifyPropertyChanged/*, INotifyPropertyChanging*/{
		protected ViewModelBase(){
		}

		#region StaticPropertyChanged
		/*
		public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

		protected static void OnStaticPropertyChanged(params string[] names) {
			foreach(var name in names) {
				OnStaticPropertyChanged(new PropertyChangedEventArgs(name));
			}
		}

		protected static void OnStaticPropertyChanged(PropertyChangedEventArgs e) {
			var eh = StaticPropertyChanged;
			if(eh != null) {
				eh(null, e);
			}
		}
		*/
		#endregion

		#region INotifyPropertyChanging
		/*
		public event PropertyChangingEventHandler PropertyChanging;
		[Conditional("DEBUG")]
		protected void OnPropertyChanging(params string[] names){
			CheckPropertyName(names);
			foreach(var name in names){
				this.OnPropertyChanging(new PropertyChangingEventArgs(name));
			}
		}
		[Conditional("DEBUG")]
		protected virtual void OnPropertyChanging(PropertyChangingEventArgs e){
			var eh = this.PropertyChanging;
			if(eh != null){
				eh(this, e);
			}
		}
		*/
		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(params string[] names){
			CheckPropertyName(names);
			foreach(var name in names){
				this.OnPropertyChanged(new PropertyChangedEventArgs(name));
			}
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e){
			var eh = this.PropertyChanged;
			if(eh != null){
				eh(this, e);
			}
		}

#if DEBUG
		protected IDictionary<string, System.Reflection.PropertyInfo> PropertyNameDictionary = null;
#endif

		[Conditional("DEBUG")]
		private void CheckPropertyName(params string[] names){
			if(this.PropertyNameDictionary == null) {
				this.PropertyNameDictionary = this.GetType().GetProperties().ToDictionary(prop => {
					var prms = prop.GetIndexParameters();
					if (prms == null || prms.Length == 0) {
						return prop.Name;
					} else {
						return prop.Name + "[]";
					}
				});
			}

			foreach (var name in names){
				if(!this.PropertyNameDictionary.ContainsKey(name)){
					throw new ArgumentException(name);
				}
			}
		}

		#endregion
	}
}
