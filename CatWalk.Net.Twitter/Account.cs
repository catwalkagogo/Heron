/*
	$Id: Account.cs 259 2011-07-24 06:19:28Z cs6m7y@bma.biglobe.ne.jp $
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
using CatWalk.Net.OAuth;
using System.Threading;

namespace CatWalk.Net.Twitter{
	public class Account{
		public AccessToken AccessToken{get; set;}
		public User User{get; private set;}
		public AuthorizedTwitterApi TwitterApi{get; private set;}
		
		public Account(AuthorizedTwitterApi api, AccessToken accessToken){
			this.AccessToken = accessToken;
			this.TwitterApi = api;
		}
		
		public void VerifyCredential(){
			this.VerifyCredential(CancellationToken.None);
		}

		public void VerifyCredential(CancellationToken token){
			this.User = null;
			var req = TwitterApi.VerifyCredential(this.AccessToken);
			using(Stream stream = req.GetStream(token)){
				var xml = XElement.Load(stream);
				this.User = new User(xml);
			}
		}

		public bool IsVerified{
			get{
				return (this.User != null);
			}
		}

		#region Friends / Followers

		public IEnumerable<ulong> GetFriends(){
			return this.GetFriends(CancellationToken.None);
		}
		public IEnumerable<ulong> GetFriends(CancellationToken token){
			if(!this.IsVerified){
				throw new UnauthorizedAccessException();
			}
			var req = TwitterApi.GetFriends(this.AccessToken, this.User.Id);
			using(Stream stream = req.GetStream(token)){
				var xml = XDocument.Load(stream);
				var root = xml.Root;
				foreach(XElement user in root.Element("ids").Elements("id")){
					yield return UInt64.Parse(user.Value);
				}
			}
		}
		public ulong[] GetFriends(out Cursor<ulong> cursor){
			return this.GetFriends(CancellationToken.None, out cursor);
		}
		public ulong[] GetFriends(CancellationToken token, out Cursor<ulong> cursor){
			return this.GetFriends(token, -1, out cursor);
		}
		private ulong[] GetFriends(CancellationToken token, long cursorId, out Cursor<ulong> cursor){
			if(!this.IsVerified){
				throw new UnauthorizedAccessException();
			}
			var req = TwitterApi.GetFriends(this.AccessToken, this.User.Id, cursorId);
			var list = new List<ulong>();
			using(Stream stream = req.GetStream(token)){
				var xml = XDocument.Load(stream);
				var root = xml.Root;
				cursor = new Cursor<ulong>(root, this.GetFriendsCursor);
				foreach(XElement user in xml.Element("ids").Elements("id")){
					list.Add(UInt64.Parse(user.Value));
				}
			}
			return list.ToArray();
		}
		private CursorResult<ulong> GetFriendsCursor(long cursor, CancellationToken token){
			Cursor<ulong> outCursor;
			var result = this.GetFriends(token, cursor, out outCursor);
			return new CursorResult<ulong>(result, outCursor);
		}

		public IEnumerable<ulong> GetFollowers(){
			return this.GetFollowers(CancellationToken.None);
		}
		public IEnumerable<ulong> GetFollowers(CancellationToken token){
			if(!this.IsVerified){
				throw new UnauthorizedAccessException();
			}
			var req = TwitterApi.GetFollowers(this.AccessToken, this.User.Id);
			using(Stream stream = req.GetStream(token)){
				var xml = XDocument.Load(stream);
				var root = xml.Root;
				foreach(XElement user in root.Element("ids").Elements("id")){
					yield return UInt64.Parse(user.Value);
				}
			}
		}
		public ulong[] GetFollowers(out Cursor<ulong> cursor){
			return this.GetFollowers(CancellationToken.None, out cursor);
		}
		public ulong[] GetFollowers(CancellationToken token, out Cursor<ulong> cursor){
			return this.GetFollowers(-1, token, out cursor);
		}
		private ulong[] GetFollowers(long cursorId, CancellationToken token, out Cursor<ulong> cursor){
			if(!this.IsVerified){
				throw new UnauthorizedAccessException();
			}
			var req = TwitterApi.GetFollowers(this.AccessToken, this.User.Id, cursorId);
			var list = new List<ulong>();
			using(Stream stream = req.GetStream(token)){
				var xml = XDocument.Load(stream);
				var root = xml.Root;
				cursor = new Cursor<ulong>(root, this.GetFollowersCursor);
				foreach(XElement user in xml.Element("ids").Elements("id")){
					list.Add(UInt64.Parse(user.Value));
				}
			}
			return list.ToArray();
		}
		private CursorResult<ulong> GetFollowersCursor(long cursor, CancellationToken token){
			Cursor<ulong> outCursor;
			var result = this.GetFollowers(cursor, token, out outCursor);
			return new CursorResult<ulong>(result, outCursor);
		}

		#endregion

		#region Timeline

		public Timeline GetHomeTimeline(int count, int page, ulong sinceId, ulong maxId, bool trimUser, bool includeRts){
			return this.GetHomeTimeline(count, page, sinceId, maxId, trimUser, includeRts, CancellationToken.None);
		}

		public Timeline GetHomeTimeline(int count, int page, ulong sinceId, ulong maxId, bool trimUser, bool includeRts, CancellationToken token){
			if(!this.IsVerified){
				throw new UnauthorizedAccessException();
			}
			var req = this.TwitterApi.GetHomeTimeline(this.AccessToken, count, page, sinceId, maxId, trimUser);
			return new HomeTimeline(XmlUtility.FromStream(req.GetStream(token)).Select(elm => new Status(elm)), this, trimUser, includeRts);
	}

		#endregion

		#region Manipulation

		public void UpdateStatus(string status, ulong replyTo, string source){
			this.UpdateStatus(status, replyTo, source, CancellationToken.None);
		}
		public void UpdateStatus(string status, ulong replyToStatusId, string source, CancellationToken token){
			if(!this.IsVerified){
				throw new UnauthorizedAccessException();
			}
			PostingWebRequest data = TwitterApi.UpdateStatus(this.AccessToken, status, replyToStatusId, source);
			data.Post(token);
		}
		
		public void DestroyStatus(Status status){
			this.DestroyStatus(status.Id, CancellationToken.None);
		}
		public void DestroyStatus(Status status, CancellationToken token){
			this.DestroyStatus(status.Id, token);
		}
		public void DestroyStatus(ulong id){
			this.DestroyStatus(id, CancellationToken.None);
		}
		public void DestroyStatus(ulong id, CancellationToken token){
			if(!this.IsVerified){
				throw new UnauthorizedAccessException();
			}
			PostingWebRequest data = TwitterApi.DestroyStatus(this.AccessToken, id);
			data.Post(token);
		}
		
		public void CreateFavorite(Status status){
			this.CreateFavorite(status.Id, CancellationToken.None);
		}
		public void CreateFavorite(Status status, CancellationToken token){
			this.CreateFavorite(status.Id, token);
		}
		public void CreateFavorite(ulong id){
			this.CreateFavorite(id, CancellationToken.None);
		}
		public void CreateFavorite(ulong id, CancellationToken token){
			if(!this.IsVerified){
				throw new UnauthorizedAccessException();
			}
			PostingWebRequest data = TwitterApi.CreateFavorite(this.AccessToken, id);
			data.Post(token);
		}
		
		public void CreateBlock(User user){
			this.CreateBlock(user.Id, CancellationToken.None);
		}
		public void CreateBlock(User user, CancellationToken token){
			this.CreateBlock(user.Id, token);
		}
		public void CreateBlock(ulong id){
			this.CreateBlock(id, CancellationToken.None);
		}
		public void CreateBlock(ulong id, CancellationToken token){
			if(!this.IsVerified){
				throw new UnauthorizedAccessException();
			}
			PostingWebRequest data = TwitterApi.CreateBlock(this.AccessToken, id);
			data.Post(token);
		}
		
		public void DestroyFriendship(User user){
			this.DestroyFriendship(user.Id, CancellationToken.None);
		}
		public void DestroyFriendship(User user, CancellationToken token){
			this.DestroyFriendship(user.Id, token);
		}
		public void DestroyFriendship(ulong id){
			this.DestroyFriendship(id, CancellationToken.None);
		}
		public void DestroyFriendship(ulong id, CancellationToken token){
			if(!this.IsVerified){
				throw new UnauthorizedAccessException();
			}
			PostingWebRequest data = TwitterApi.DestroyFriendship(this.AccessToken, id);
			data.Post(token);
		}

		#endregion

        #region List

		public IEnumerable<TwitterList> GetLists(CancellationToken token){
			if(!this.IsVerified){
				throw new UnauthorizedAccessException();
			}
			var req = this.TwitterApi.GetLists(this.User.Id);
			using(Stream stream = req.GetStream(token)){
				var xml = XDocument.Load(stream);
				foreach(XElement list in xml.Root.Element("lists").Elements("list")){
					yield return new TwitterList(list);
				}
			}
		}

		public void DestroyList(TwitterList list, CancellationToken token){
			if(!this.IsVerified){
				throw new UnauthorizedAccessException();
			}
			var req = this.TwitterApi.DestroyList(list.Id, this.AccessToken);
			req.Post(token);
		}

        #endregion
    }
}