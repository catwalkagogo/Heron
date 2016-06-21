using System;

namespace CatWalk.Heron {
	public interface IJob : IProgress<double> {
		double Progress { get; }
		bool CanCancel { get; }
		JobStatus Status { get; set; }
	}

	public enum JobStatus {
		Pending,
		Running,
		Cancelled,
		Completed,
		Failed,
	}
}
