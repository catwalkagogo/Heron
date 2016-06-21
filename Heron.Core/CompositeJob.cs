using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.ViewModel;
/*
namespace CatWalk.Heron {
	public class CompositeJob : ViewModelBase, IJob {
		private ICollection<IJob> _Jobs;

		public CompositeJob(IEnumerable<IJob> jobs) {
			jobs.ThrowIfNull("jobs");
			this._Jobs = jobs.ToArray();

		}

		public bool CanCancel {
			get {
				return this._Jobs.Where(job => job.CanCancel).FirstOrDefault() != null;
			}
		}

		public double Progress {
			get {
				return this._Jobs.Sum(job => job.Progress) / this._Jobs.Count;
			}
		}

		public JobStatus Status {
			get {
				var isFinished = this._Jobs.All(job => job.Status == JobStatus.Completed);
				var isFailed = this._Jobs.Any(job => job.Status == JobStatus.Failed);
				var isRunning = this._Jobs.Any(job => job.Status == JobStatus.Running);
				var isCanceled = this._Jobs.Any(job => job.Status == JobStatus.Cancelled);
				if (isCanceled) {
					return JobStatus.Cancelled;
				} else if (isRunning) {
					return JobStatus.Running;
				}else if (isFailed) {
					return JobStatus.Failed;
				}else if (isFinished) {
					return JobStatus.Completed;
				} else {
					return JobStatus.Pending;
				}
			}
			set {
				// Do Nothing
				throw new InvalidOperationException();
			}
		}

		public void Report(double value) {
			// Do Nothing
			throw new InvalidOperationException();
		}
	}
}
*/