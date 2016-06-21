using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using CatWalk.Collections;

namespace CatWalk.Mvvm {
	public abstract class DataErrorInfoViewModelBase : ViewModelBase, IDataErrorInfo{
		#region IDataErrorInfo

		private ObservableDictionary<string, string> _Errors;
		protected ObservableDictionary<string, string> Errors{
			get{
				if(this._Errors == null){
					this._Errors = new ObservableDictionary<string, string>();
				}
				return this._Errors;
			}
		}

		public void SetError(string propertyName, string message){
			this.Errors[propertyName] = message;
			this.OnPropertyChanged("HasError", "Error");
		}

		public void RemoveError(string propertyName){
			this.Errors.Remove(propertyName);
			this.OnPropertyChanged("HasError", "Error");
		}

		public void ClearErrors(){
			this.Errors.Clear();
			this.OnPropertyChanged("HasError", "Error");
		}

		public virtual string Error{
			get{
				return String.Join(
					Environment.NewLine,
					this._Errors.Where(err => !String.IsNullOrEmpty(err.Value)).Select(err => err.Value));
			}
		}

		public string this[string columnName] {
			get{
				return this.Errors[columnName];
			}
		}

		public bool HasError{
			get{
				return (this._Errors != null) && (this._Errors.Count > 0);
			}
		}

		#endregion
	}
}
