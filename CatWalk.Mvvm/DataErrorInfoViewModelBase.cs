using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using CatWalk.Collections;
using System.Collections;

namespace CatWalk.Mvvm {
	public abstract class DataErrorInfoViewModelBase : ViewModelBase, INotifyDataErrorInfo {
		private Lazy<IDictionary<string, IEnumerable>> _Errors = new Lazy<IDictionary<string, IEnumerable>>(() => {
			return new Dictionary<string, IEnumerable>();
		});

		public bool HasErrors {
			get {
				return this._Errors.Value.Values
					.Where(errors => errors != null)
					.Any(errors => errors.GetEnumerator().MoveNext());
			}
		}

		protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs e) {
			var handler = this.ErrorsChanged;
			if (handler != null){
				handler(this, e);
			}
		}

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		public IEnumerable GetErrors(string propertyName) {
			IEnumerable errors;
			if(this._Errors.Value.TryGetValue(propertyName, out errors)) {
				return errors;
			}else {
				return new object[0];
			}
		}

		public void SetErrors(string propertyName, IEnumerable enumerable) {
			propertyName.ThrowIfNullOrEmpty("propertyName");

			this._Errors.Value[propertyName] = enumerable;
			this.OnErrorsChanged(new DataErrorsChangedEventArgs(propertyName));
			this.OnPropertyChanged("HasErrors");
		}
	}
}
