using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CatWalk.Collections;

namespace CatWalk.Heron.ViewModel {
	public class JobManager : ViewModelBase, IJobManager{
		private ObservableCollection<Job> _Jobs = new ObservableCollection<Job>();
		private ReadOnlyObservableCollection<Job> _ReadOnlyJobs;
		private ListCollectionView _View;

		public JobManager(){
			this._View = new ListCollectionView(this._Jobs);
			this._View.GroupDescriptions.Add(new PropertyGroupDescription("Status"));
			this._View.CustomSort = new JobComparer();
			this._ReadOnlyJobs = new ReadOnlyObservableCollection<Job>(this._Jobs);

			this._Jobs.CollectionChanged += this.job_StatusChanged;
		}

		#region Property

		public double TotalProgress {
			get {
				var running = this._Jobs.Where(job => job.Status == JobStatus.Running).ToArray();
				var count = running.Length;
				var sum = running.Sum(job => job.Progress);
				return sum / (double)count;
			}
		}

		public ReadOnlyObservableCollection<Job> Jobs {
			get {
				return this._ReadOnlyJobs;
			}
		}

		public int RunningCount {
			get {
				return this._Jobs.Count(job => job.Status == JobStatus.Running);
			}
		}

		public int PendingCount {
			get {
				return this._Jobs.Count(job => job.Status == JobStatus.Pending);
			}
		}

		public int CompletedCount {
			get {
				return this._Jobs.Count(job => job.Status == JobStatus.Completed);
			}
		}

		public int FailedCount {
			get {
				return this._Jobs.Count(job => job.Status == JobStatus.Failed);
			}
		}

		public int CancelledCount {
			get {
				return this._Jobs.Count(job => job.Status == JobStatus.Cancelled);
			}
		}


		#endregion

		#region IJobManager Members

		public void Register(Job job) {
			job.ThrowIfNull("job");
			this._Jobs.Add(job);

			job.StatusChanged += job_StatusChanged;
			job.ProgressChanged += job_ProgressChanged;

			this.OnPropertyChanged("Count");
		}

		void job_ProgressChanged(object sender, EventArgs e) {
			this.OnPropertyChanged("TotalProgress");
		}

		private void job_StatusChanged(object sender, EventArgs e) {
			this.OnPropertyChanged("RunningCount", "FailedCount", "CancelledCount", "CompletedCount", "PendingCount", "TotalProgress");
		}

		public int Count {
			get {
				return this._Jobs.Count;
			}
		}

		#endregion

		#region View

		public CollectionView JobsView {
			get {
				return this._View;
			}
		}

		private class JobComparer : IComparer<Job>, IComparer {

			#region IComparer<Job> Members

			public int Compare(Job x, Job y) {
				int d = GetKey(x.Status).CompareTo(GetKey(y.Status));
				if(d == 0) {
					return -GetDate(x).CompareTo(GetDate(y));
				} else {
					return d;
				}
			}

			private static DateTime GetDate(Job job) {
				if(job.CreatedTime < job.StartedTime) {
					if(job.StartedTime < job.FinishedTime) {
						return job.FinishedTime;
					} else {
						return job.StartedTime;
					}
				} else {
					return job.CreatedTime;
				}
			}

			private static int GetKey(JobStatus status) {
				switch(status) {
					case JobStatus.Completed: return 4;
					case JobStatus.Cancelled: return 3;
					case JobStatus.Failed: return 2;
					case JobStatus.Pending: return 1;
					case JobStatus.Running: return 0;
				}
				return Int32.MaxValue;
			}

			#endregion

			#region IComparer Members

			public int Compare(object x, object y) {
				return this.Compare((Job)x, (Job)y);
			}

			#endregion
		}

		#endregion

	}
}
