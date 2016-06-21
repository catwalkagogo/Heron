using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
//using CatWalk.Collections;

namespace CatWalk.Net.Twitter {
	/*
	public class TimelineCollection : ReadOnlyObservableList<Status>{
		private Timeline _NewestTimeline;
		private Timeline _OldestTimeline;

		public TimelineCollection(Timeline timeline) : base(new SortedSkipList<Status>(timeline.Statuses)){
		}

		public void RetrieveOlder(int count, CancellationToken token){
			var timeline = this._OldestTimeline.GetOlder(count, token);
			foreach(var status in timeline.Statuses){
				this.List.Add(status);
			}
			this._OldestTimeline = timeline;
		}

		public void RetrieveNewer(int count, CancellationToken token){
			var timeline = this._NewestTimeline.GetNewer(count, token);
			foreach(var status in timeline.Statuses){
				this.List.Add(status);
			}
			this._NewestTimeline = timeline;
		}
	}
	*/
	public abstract class Timeline : IEnumerable<Status>{
		private IEnumerable<Status> _Source;
		private List<Status> _SourceCache;

		public Timeline(IEnumerable<Status> source){
			if(source == null){
				throw new ArgumentNullException("source");
			}
			this._Source = this.CachingEnum(source);
		}

		private IEnumerable<Status> CachingEnum(IEnumerable<Status> source){
			if(this._SourceCache == null){
				this._SourceCache = new List<Status>();
				var filters = this.Filters;
				if(filters != null){
					foreach(var filter in filters.GetInvocationList()){
						source = (IEnumerable<Status>)filter.DynamicInvoke(source);
					}
				}
				foreach(var status in source){
					this._SourceCache.Add(status);
					yield return status;
				}
			}else{
				foreach(var status in this._SourceCache){
					yield return status;
				}
			}
		}

		public event Func<IEnumerable<Status>, IEnumerable<Status>> Filters;

		public abstract Timeline GetOlder(int count);
		public abstract Timeline GetOlder(int count, CancellationToken token);
		public abstract Timeline GetNewer(int count);
		public abstract Timeline GetNewer(int count, CancellationToken token);

		#region IEnumerable<Status> Members

		public IEnumerator<Status> GetEnumerator() {
			return this._Source.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this._Source.GetEnumerator();
		}

		#endregion
	}

	public abstract class IdTimeline : Timeline{
		public ulong MaxId{get; private set;}
		public ulong MinId{get; private set;}

		public IdTimeline(IEnumerable<Status> source) : base(source){
			this.MaxId = UInt64.MinValue;
			this.MinId = UInt64.MaxValue;
			this.Filters += this.Filter;
		}

		private IEnumerable<Status> Filter(IEnumerable<Status> source){
			foreach(var status in source){
				if(status.Id > this.MaxId){
					this.MaxId = status.Id;
				}
				if(status.Id < this.MinId){
					this.MinId = status.Id;
				}
				yield return status;
			}
		}
	}
	
	public class UserTimeline : IdTimeline{
		public string Id{get; private set;}
		public bool IsTrimUser{get; private set;}
		public bool IsIncludeRts{get; private set;}

		public UserTimeline(IEnumerable<Status> source, string id, bool isTrimUser, bool isIncludeRts) : base(source){
			this.Id = id;
			this.IsTrimUser = isTrimUser;
			this.IsIncludeRts = isIncludeRts;
		}

		public override Timeline GetOlder(int count) {
			return User.GetTimeline(this.Id, count, 0, 0, this.MinId - 1, this.IsTrimUser, this.IsIncludeRts);
		}

		public override Timeline GetOlder(int count, CancellationToken token) {
			return User.GetTimeline(this.Id, count, 0, 0, this.MinId - 1, this.IsTrimUser, this.IsIncludeRts, token);
		}

		public override Timeline GetNewer(int count) {
			return User.GetTimeline(this.Id, count, 0, this.MaxId + 1, 0, this.IsTrimUser, this.IsIncludeRts);
		}

		public override Timeline GetNewer(int count, CancellationToken token) {
			return User.GetTimeline(this.Id, count, 0, this.MaxId + 1, 0, this.IsTrimUser, this.IsIncludeRts, token);
		}
	}
	
	public class HomeTimeline : IdTimeline{
		public Account Account{get; private set;}
		public bool IsTrimUser{get; set;}
		public bool IsIncludeRts{get; set;}

		public HomeTimeline(IEnumerable<Status> source, Account account, bool isTrimUser, bool isIncludeRts) : base(source){
			this.Account = account;
			this.IsTrimUser = isTrimUser;
			this.IsIncludeRts = isIncludeRts;
		}

		public override Timeline GetOlder(int count) {
			return this.Account.GetHomeTimeline(count, 0, 0, this.MinId - 1, this.IsTrimUser, this.IsIncludeRts);
		}

		public override Timeline GetOlder(int count, CancellationToken token) {
			return this.Account.GetHomeTimeline(count, 0, 0, this.MinId - 1, this.IsTrimUser, this.IsIncludeRts, token);
		}

		public override Timeline GetNewer(int count) {
			return this.Account.GetHomeTimeline(count, 0, this.MaxId + 1, 0, this.IsTrimUser, this.IsIncludeRts);
		}

		public override Timeline GetNewer(int count, CancellationToken token) {
			return this.Account.GetHomeTimeline(count, 0, this.MaxId + 1, 0, this.IsTrimUser, this.IsIncludeRts, token);
		}
	}
	/*
	public class PublicTimeline : IdTimeline{
		public override Timeline GetOlder(int count) {
			throw new NotImplementedException();
		}

		public override Timeline GetOlder(int count, CancellationToken token) {
			throw new NotImplementedException();
		}

		public override Timeline GetNewer(int count) {
			throw new NotImplementedException();
		}

		public override Timeline GetNewer(int count, CancellationToken token) {
			throw new NotImplementedException();
		}
	}
	 * */
}
