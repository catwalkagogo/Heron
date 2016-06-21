using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CatWalk.IOSystem;
using CatWalk.Net.Twitter;

namespace CatWalk.IOSystem.Twitter {
	public class StatusSystemEntry : TerminalSystemEntry, IComparable<StatusSystemEntry>{
		public Status Status{get; private set;}

		public StatusSystemEntry(ISystemEntry parent, Status status) : base(parent, status.Id.ToString()){
			this.Status = status;
		}

		public bool CanDelete(Account account){
			if(account == null){
				throw new ArgumentNullException("account");
			}
			return account != null && account.User == this.Status.User;
		}

		public void Delete(Account account){
			this.Delete(account, CancellationToken.None);
		}

		public void Delete(Account account, CancellationToken token){
		}

		public void Favorite(Account account){
			this.Favorite(account, CancellationToken.None);
		}

		public void Favorite(Account account, CancellationToken token){
		}

		public void Retweet(Account account){
			this.Retweet(account, CancellationToken.None);
		}

		public void Retweet(Account account, CancellationToken token){
		}

		public void Reply(Account account, string status){
			this.Reply(account, status, CancellationToken.None);
		}
		public void Reply(Account account, string status, CancellationToken token){
		}

		public override bool IsExists() {
			return true;
		}

		public int CompareTo(StatusSystemEntry other) {
			return this.Status.CompareTo(other.Status);
		}
	}
}
