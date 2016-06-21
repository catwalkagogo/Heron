using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CatWalk.Collections;
using CatWalk.IOSystem;
using CatWalk.Net.Twitter;

namespace CatWalk.IOSystem.Twitter {
	public class TimelineSystemDirectory : SystemEntry, IPartialSystemEntry{
		public Account Account{get; private set;}
		private Timeline _NewestTimeline;
		private Timeline _OldestTimeline;
		private Func<CancellationToken, Timeline> _SeedTimeline;
		private WrappedReadOnlyObservableList<StatusSystemEntry> _StatusReadOnlyList;
		private ObservableSortedSkipList<StatusSystemEntry> _StatusList;

		public TimelineSystemDirectory(ISystemEntry parent, string name, Func<CancellationToken, Timeline> timeline) : base(parent, name){
			this._SeedTimeline = timeline;
			this._StatusReadOnlyList = new WrappedReadOnlyObservableList<StatusSystemEntry>(this._StatusList);
		}

		public TimelineSystemDirectory(ISystemEntry parent, string name, Account account, Func<CancellationToken, Timeline> timeline) : base(parent, name){
			this._SeedTimeline = timeline;
		}

		public override bool IsDirectory {
			get {
				return true;
			}
		}

		private void InitListIfFirst(CancellationToken token){
			if(this._SeedTimeline != null){
				var timeline = this._SeedTimeline(token);
				this._StatusList = new ObservableSortedSkipList<StatusSystemEntry>(
					timeline.Select(status => new StatusSystemEntry(this, status)),
					new ReversedComparer<StatusSystemEntry>(Comparer<StatusSystemEntry>.Default),
					false);
				this._OldestTimeline = this._NewestTimeline = timeline;
				this._StatusReadOnlyList = new WrappedReadOnlyObservableList<StatusSystemEntry>(this._StatusList);
				this._SeedTimeline = null;
			}
		}

		public void GetNextChildren(int count){
			this.GetNextChildren(count, CancellationToken.None);
		}
		public void GetNextChildren(int count, CancellationToken token){
			this.InitListIfFirst(token);
			this._NewestTimeline = this._NewestTimeline.GetNewer(count);
			foreach(var status in this._NewestTimeline){
				this._StatusList.Add(new StatusSystemEntry(this, status));
			}
		}

		public void GetPreviousChildren(int count){
			this.GetPreviousChildren(count, CancellationToken.None);
		}
		public void GetPreviousChildren(int count, CancellationToken token){
			this.InitListIfFirst(token);
			this._OldestTimeline = this._OldestTimeline.GetOlder(count);
			foreach(var status in this._OldestTimeline){
				this._StatusList.Add(new StatusSystemEntry(this, status));
			}
		}

		public bool CanUpdateStatus(){
			return this.Account != null;
		}

		public void UpdateStatus(string status){
			if(!this.CanUpdateStatus()){
				throw new NotSupportedException();
			}
		}

		public void UpdateStatus(string status, Status replyTo, string source){
			this.UpdateStatus(status, replyTo, source, CancellationToken.None);
		}

		public void UpdateStatus(string status, Status replyTo, string source, CancellationToken token){
			if(!this.CanUpdateStatus()){
				throw new NotSupportedException();
			}
			this.Account.UpdateStatus(status, replyTo.Id, source, token);
		}

		public override IEnumerable<ISystemEntry> GetChildren(CancellationToken token, IProgress<double> progress) {
			this.InitListIfFirst(token);
			return this._StatusReadOnlyList;
		}

		public override bool IsExists() {
			return true;
		}
	}
}
