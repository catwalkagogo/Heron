/*
	$Id: Status.cs 259 2011-07-24 06:19:28Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace CatWalk.Net.Twitter{
	public class Status : IEquatable<Status>, IComparable<Status> {
		#region Data

		public DateTime CreatedAt{get; private set;}
		public ulong Id{get; private set;}
		public string Text{get; private set;}
		public string Source{get; private set;}
		public ulong InReplyToStatusId{get; private set;}
		public ulong InReplyToUserId{get; private set;}
		public string InReplyToScreenName{get; private set;}
		public bool Favorited{get; private set;}
		public bool Trancated{get; private set;}
		public int RetweetCount{get; private set;}
		public bool Retweeted{get; private set;}
		public User User{get; private set;}
		public ulong UserId{get; private set;}
		
		public Status(XElement element){
			if(element == null){
				throw new ArgumentNullException("element");
			}
			DateTime dt;
			bool b;
			ulong dec;
			
			//status.Id = (ulong)element.Element("id");
			this.Id = (ulong)element.Element("id");
			if(TwitterApi.TryParseDateTime((string)element.Element("created_at"), out dt)){
				this.CreatedAt = dt;
			}
			this.Text = System.Net.WebUtility.HtmlDecode((string)element.Element("text"));
			this.Source = (string)element.Element("source");
			if(Boolean.TryParse((string)element.Element("truncated"), out b)){
				this.Trancated = b;
			}
			if(UInt64.TryParse((string)element.Element("in_reply_to_status_id"), out dec)){
				this.InReplyToStatusId = dec;
			}
			if(UInt64.TryParse((string)element.Element("in_reply_to_user_id"), out dec)){
				this.InReplyToUserId = dec;
			}
			this.InReplyToScreenName = (string)element.Element("in_reply_to_screen_name");
			if(Boolean.TryParse((string)element.Element("favorited"), out b)){
				this.Favorited = b;
			}
			XElement elm = element.Element("retweet_count");
			if(elm != null){
				this.RetweetCount = Int32.Parse(elm.Value.TrimEnd('+'));
			}
			elm = element.Element("retweeted");
			if(elm != null){
				this.Retweeted = (bool)elm;
			}

			// for trim_user
			var userelm = element.Element("user");
			if(userelm.Element("description") != null){
				this.User = new User(userelm);
			}else{
				this.UserId = (ulong)userelm.Element("id");
			}
		}

		#endregion

		#region API

		public static Status FromId(ulong id){
			return FromId(id, false, false, CancellationToken.None);
		}
		public static Status FromId(ulong id, bool trimUser){
			return FromId(id, trimUser, false, CancellationToken.None);
		}
		public static Status FromId(ulong id, bool trimUser, bool includeEntities){
			return FromId(id, trimUser, includeEntities, CancellationToken.None);
		}
		public static Status FromId(ulong id, bool trimUser, bool includeEntities, CancellationToken token){
			var req =  TwitterApi.Default.ShowStatus(id, trimUser, includeEntities);
			using(Stream stream = req.GetStream(token))
			using(StreamReader reader = new StreamReader(stream, Encoding.UTF8)){
				var xml = XElement.Load(stream);
				return new Status(xml);
			}
		}

		public Status GetReplyStatus(){
			return this.GetReplyStatus(false, false, CancellationToken.None);
		}
		public Status GetReplyStatus(bool trimUser){
			return this.GetReplyStatus(trimUser, false, CancellationToken.None);
		}
		public Status GetReplyStatus(bool trimUser, bool includeEntities){
			return this.GetReplyStatus(trimUser, includeEntities, CancellationToken.None);
		}
		public Status GetReplyStatus(bool trimUser, bool includeEntities, CancellationToken token){
			return FromId(this.InReplyToStatusId, trimUser, includeEntities, token);
		}

		public User GetReplyUser(){
			return User.FromId(this.InReplyToUserId);
		}

		public User GetReplyUser(CancellationToken token){
			return User.FromId(this.InReplyToUserId, token);
		}

		public IEnumerable<Status> GetRetweets(bool trimUser, bool includeEntities, CancellationToken token){
			throw new NotImplementedException();
		}

		#endregion

		#region IEquatable<Status> Members

		public override bool Equals(object obj){
			if(obj is Status){
				return this.Equals((Status)obj);
			}else{
				return false;
			}
		}
		
		public static bool operator ==(Status a, Status b){
			if(Object.ReferenceEquals(a, null)){
				if(Object.ReferenceEquals(b, null)){
					return true;
				}else{
					return false;
				}
			}else{
				return a.Equals(b);
			}
		}
		
		public static bool operator !=(Status a, Status b){
			return !(a == b);
		}
		
		public override int GetHashCode(){
			return this.Id.GetHashCode();
		}

		public bool Equals(Status other) {
			if(other != null){
				return this.Id.Equals(other.Id);
			}else{
				return false;
			}
		}

		#endregion

		#region IComparable<Status>

		public int CompareTo(Status other) {
			return this.Id.CompareTo(other.Id);
		}

		#endregion
	}
}