using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron {
	public class ProgressAggregator<T> {
		private IProgress<T> _TargetProgress;
		private Func<IEnumerable<T>, T> _Aggregate;
		private IDictionary<object, T> _SourceProgresses = new Dictionary<object, T>();

		public ProgressAggregator(IProgress<T> targetProgress, Func<IEnumerable<T>, T> aggregate) {
			targetProgress.ThrowIfNull("targetProgress");
			aggregate.ThrowIfNull("aggregate");

			this._TargetProgress = targetProgress;
			this._Aggregate = aggregate;
		}

		public IProgress<T> NewProgress() {
			var key = new Object();
			var prog = new Progress<T>((p) => {
				this._SourceProgresses[key] = p;
			});

			this._SourceProgresses.Add(key, default(T));

			return prog;
		}

		private void OnProgress() {
			var progress = this._Aggregate(this._SourceProgresses.Values);
			this._TargetProgress.Report(progress);
		}
	}

	public static class ProgressAggregators {
		public static ProgressAggregator<double> GetDoubleProgressAggregator(IProgress<double> progress) {
			return new ProgressAggregator<double>(progress, progs => {
				var progsA = progs.ToArray();
				return Math.Max(0, Math.Min(progsA.Sum(), 1)) / progsA.Length;
			});
		}
	}
}