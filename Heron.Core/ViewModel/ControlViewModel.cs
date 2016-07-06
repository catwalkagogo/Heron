using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk;
using CatWalk.Collections;
using CatWalk.Mvvm;
using System.ComponentModel;
using System.Windows;
using System.Collections.Specialized;

namespace CatWalk.Heron.ViewModel {
	public class ControlViewModel : ViewModelBase, IHierarchicalViewModel<ControlViewModel>{
		public ControlViewModel() : this(null) {
		}

		public ControlViewModel(ControlViewModel parent) : base(){
			this.Children = new ControlViewModelCollection(this);
			this.Parent = parent;
		}

		public ControlViewModel(ControlViewModel parent, SynchronizationContext invoke) : base(invoke) {
			this.Children = new ControlViewModelCollection(this);
			this.Parent = parent;
		}

		protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
			if(e.PropertyName == "Ancestors") {
				foreach(var child in this.Children) {
					child.OnPropertyChanged("Ancestors");
				}
			}
			base.OnPropertyChanged(e);
		}

		private Lazy<IDictionary<object, object>> _Values = new Lazy<IDictionary<object, object>>(() => new Dictionary<object, object>());
		public void SetValue(object key, object value) {
			this._Values.Value[key] = value;
		}

		public object GetValue(object key) {
			return this._Values.Value[key];
		}


		#region Job

		public Job CreateJob(string message, Action<Job> action) {
			Job job = null;
			job = Job.Create(message, new Action(() => {
				action(job);
			}));
			this.RegisterJob(job);
			return job;
		}

		public Job CreateJob(string message, Func<Job, Task> taskFactory) {
			Job job = null;
			job = Job.Create(message, taskFactory(job));
			this.RegisterJob(job);
			return job;
		}

		/*
		public Job CreateJob(Action<Job> action){
			var job = new Job(action);
			this.AddJob(job);
			return job;
		}

		public Job CreateJob(Action<Job> action, TaskCreationOptions creationOptions) {
			var job = new Job(action, creationOptions);
			this.AddJob(job);
			return job;
		}

		public Job CreateJob(Action<Job, object> action, object state){
			var job = new Job(action, state);
			this.AddJob(job);
			return job;
		}

		public Job CreateJob(Action<Job, object> action, object state, TaskCreationOptions creationOptions) {
			var job = new Job(action, state, creationOptions);
			this.AddJob(job);
			return job;
		}

		public Job<TResult> CreateJob<TResult>(Func<Job<TResult>, TResult> action){
			var job = new Job<TResult>(action);
			this.AddJob(job);
			return job;
		}

		public Job<TResult> CreateJob<TResult>(Func<Job<TResult>, TResult> action, TaskCreationOptions creationOptions){
			var job = new Job<TResult>(action, creationOptions);
			this.AddJob(job);
			return job;
		}

		public Job<TResult> CreateJob<TResult>(Func<Job<TResult>, object, TResult> action, object state){
			var job = new Job<TResult>(action, state);
			this.AddJob(job);
			return job;
		}

		public Job<TResult> CreateJob<TResult>(Func<Job<TResult>, object, TResult> action, object state, TaskCreationOptions creationOptions){
			var job = new Job<TResult>(action, state, creationOptions);
			this.AddJob(job);
			return job;
		}
		*/
		public void RegisterJob(Job job) {
			job.ThrowIfNull("job");

			var site = Seq.Make(this).Concat(this.Ancestors).OfType<IJobManagerSite>().FirstOrDefault();
			if(site != null) {
				site.JobManager.Register(job);
			}
		}

		#endregion

		#region IHierarchicalViewModel<ControlViewModel> Members

		public IEnumerable<ControlViewModel> Ancestors {
			get {
				ControlViewModel vm = this.Parent;
				while(vm != null) {
					yield return vm;
					vm = vm.Parent;
				}
			}
		}

		private ControlViewModel _Parent;
		public ControlViewModel Parent {
			get {
				return this._Parent;
			}
			protected set {
				var parent = this._Parent;
				if(parent != null) {
					parent.Children.Remove(this);
				}
				this._Parent = value;
				if(value != null) {
					value.Children.Add(this);
				}
				this.OnPropertyChanged("Parent", "Ancestors");
			}
		}
		public ControlViewModelCollection Children { get; private set; }
		IEnumerable<ControlViewModel> IHierarchicalViewModel<ControlViewModel>.Children {
			get {
				return this.Children;
			}
		}

		public class ControlViewModelCollection : /*WrappedObservableCollection<ControlViewModel>*/ ObservableHashSet<ControlViewModel> {

			public ControlViewModel ViewModel { get; private set; }

			public ControlViewModelCollection(ControlViewModel vm) /*: base(() => new WeakLinkedList<ControlViewModel>())*/ {
				this.ViewModel = vm;
			}

			protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
				switch(e.Action) {
					case NotifyCollectionChangedAction.Add:
					case NotifyCollectionChangedAction.Move:
					case NotifyCollectionChangedAction.Remove:
					case NotifyCollectionChangedAction.Replace:
						if (e.OldItems != null) {
							e.OldItems.Cast<ControlViewModel>().ForEach(this.RemoveItem);
						}
						if (e.NewItems != null) {
							e.NewItems.Cast<ControlViewModel>().ForEach(this.AddItem);
						}
						break;
					case NotifyCollectionChangedAction.Reset:
						this.ForEach(this.RemoveItem);
						this.ForEach(this.AddItem);
						break;
				}

				base.OnCollectionChanged(e);
			}

			private void AddItem(ControlViewModel item) {
				var parent = item._Parent;
				if(parent != null) {
					parent.Children.Remove(item);
				}
				item._Parent = this.ViewModel;
			}

			private void RemoveItem(ControlViewModel item) {
				item._Parent = null;
			}
		}

		#endregion

		protected override void Dispose(bool disposing) {
			if (!this.IsDisposed) {
				if (this.Parent != null) {
					this.Parent.Children.Remove(this);
				}
			}

			base.Dispose(disposing);
		}
	}
}
