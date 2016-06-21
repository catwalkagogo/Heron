using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using CatWalk.Net.OAuth;

namespace CatWalk.Net.Twitter {
	using Parameter = KeyValuePair<string, string>;

	public class AuthorizedTwitterApi : TwitterApi{
		public Consumer Consumer{get; private set;}
		public AuthorizedTwitterApi(Consumer consumer){
			this.Consumer = consumer;
		}

		#region OAuth

		public RequestToken ObtainUnauthorizedRequestToken(){
			return Consumer.ObtainUnauthorizedRequestToken("https://api.twitter.com/oauth/request_token", "http://twitter.com/");
		}

		public RequestToken ObtainUnauthorizedRequestToken(CancellationToken token){
			return Consumer.ObtainUnauthorizedRequestToken("https://api.twitter.com/oauth/request_token", "http://twitter.com/", token);
		}
		
		public AccessToken GetAccessToken(RequestToken requestToken, string verifier){
			return Consumer.GetAccessToken(
				verifier, requestToken, "https://api.twitter.com/oauth/access_token", "http://twitter.com/");
		}

		public AccessToken GetAccessToken(RequestToken requestToken, string verifier, CancellationToken token){
			return Consumer.GetAccessToken(
				verifier, requestToken, "https://api.twitter.com/oauth/access_token", "http://twitter.com/", token);
		}

		public string BuildUserAuthorizationURL(RequestToken reqToken){
			return Consumer.BuildUserAuthorizationURL("https://api.twitter.com/oauth/authorize", reqToken);
		}
		
		public GettingWebRequest VerifyCredential(AccessToken token){
			const string url = "https://api.twitter.com/1/account/verify_credentials.xml";
			return Get(url, new Parameter[0], token);
		}

		public static string AuthorizationUri{
			get{
				return "https://api.twitter.com/oauth/access_token";
			}
		}

		#endregion

		#region Timeline

		public GettingWebRequest GetHomeTimeline(AccessToken token, int count, int page, ulong sinceId, ulong maxId, bool trimUser){
			const string url = "http://api.twitter.com/1/statuses/home_timeline.xml";
			List<Parameter> prms = new List<Parameter>();
			if(count > 0){
				prms.Add(new Parameter("count", count.ToString()));
			}
			if(page > 0){
				prms.Add(new Parameter("page", page.ToString()));
			}
			if(sinceId > 0){
				prms.Add(new Parameter("since_id", sinceId.ToString()));
			}
			if(maxId > 0){
				prms.Add(new Parameter("max_id", maxId.ToString()));
			}
			if(trimUser){
				prms.Add(new Parameter("trim_user", "1"));
			}
			return Get(url, prms.ToArray(), token);
		}

		#endregion

		#region Favorite / Block / Follow

		public PostingWebRequest DestroyStatus(AccessToken token, ulong id){
			const string url = "http://api.twitter.com/1/statuses/destroy";
			Parameter[] prms = new Parameter[1]{new Parameter("id", id.ToString())};
			return Post(url + "/" + id.ToString() + ".xml", prms, token);
		}
		
		public PostingWebRequest CreateFavorite(AccessToken token, ulong id){
			const string url = "http://api.twitter.com/1/favorites/create";
			Parameter[] prms = new Parameter[1]{new Parameter("id", id.ToString())};
			return Post(url + "/" + id.ToString() + ".xml", prms, token);
		}
		
		public PostingWebRequest CreateBlock(AccessToken token, ulong id){
			const string url = "http://api.twitter.com/1/blocks/create";
			Parameter[] prms = new Parameter[1]{new Parameter("id", id.ToString())};
			return Post(url + "/" + id.ToString() + ".xml", prms, token);
		}
		
		public PostingWebRequest DestroyBlock(AccessToken token, ulong id){
			const string url = "http://api.twitter.com/1/blocks/destroy";
			Parameter[] prms = new Parameter[1]{new Parameter("id", id.ToString())};
			return Post(url + "/" + id.ToString() + ".xml", prms, token);
		}
		
		public PostingWebRequest CreateFriendship(AccessToken token, ulong id){
			const string url = "http://api.twitter.com/1/friendships/create";
			Parameter[] prms = new Parameter[1]{new Parameter("id", id.ToString())};
			return Post(url + "/" + id.ToString() + ".xml", prms, token);
		}
		
		public PostingWebRequest DestroyFriendship(AccessToken token, ulong id){
			const string url = "http://api.twitter.com/1/friendships/destroy";
			Parameter[] prms = new Parameter[1]{new Parameter("id", id.ToString())};
			return Post(url + "/" + id.ToString() + ".xml", prms, token);
		}

		#endregion

		#region Tweets

		public PostingWebRequest UpdateStatus(AccessToken token, string status, ulong replyTo, string source){
			const string url = "http://api.twitter.com/1/statuses/update.xml";
			List<Parameter> prms = new List<Parameter>();
			prms.Add(new Parameter("status", status));
			prms.Add(new Parameter("source", source));
			if(replyTo > 0){
				prms.Add(new Parameter("in_reply_to_status_id", replyTo.ToString()));
			}
			return Post(url, prms.ToArray(), token);
		}

		public PostingWebRequest Retweet(AccessToken token, ulong id){
			const string url = "http://api.twitter.com/1/statuses/retweet/";
			return Post(url + id.ToString() + ".xml", new Parameter[0], token);
		}

		#endregion

		#region User

		public GettingWebRequest SearchUsers(AccessToken token, string searchWord, int count, int page){
			const string url = "http://api.twitter.com/1/users/search.xml";
			List<Parameter> prms = new List<Parameter>();
			prms.Add(new Parameter("q", searchWord));
			if(count > 0){
				prms.Add(new Parameter("per_page", count.ToString()));
			}
			if(page > 0){
				prms.Add(new Parameter("page", page.ToString()));
			}
			return Get(url, prms.ToArray(), token);
		}

		public GettingWebRequest GetFollowers(AccessToken token, ulong user_id){
			const string url = "http://api.twitter.com/1/followers/ids.xml";
			return Get(url, new Parameter[]{new Parameter("user_id", user_id.ToString())}, token);
		}
		
		public GettingWebRequest GetFollowers(AccessToken token, ulong user_id, long cursor){
			const string url = "http://api.twitter.com/1/followers/ids.xml";
			return Get(url, new Parameter[]{new Parameter("user_id", user_id.ToString()), new Parameter("cursor", cursor.ToString())}, token);
		}
		
		public GettingWebRequest GetFriends(AccessToken token, ulong user_id){
			const string url = "http://api.twitter.com/1/friends/ids.xml";
			return Get(url, new Parameter[]{new Parameter("user_id", user_id.ToString())}, token);
		}
		
		public GettingWebRequest GetFriends(AccessToken token, ulong user_id, long cursor){
			const string url = "http://api.twitter.com/1/friends/ids.xml";
			return Get(url, new Parameter[]{new Parameter("user_id", user_id.ToString()), new Parameter("cursor", cursor.ToString())}, token);
		}
		
		public GettingWebRequest GetBlocks(AccessToken token, int page){
			const string url = "http://api.twitter.com/1/blocks/blocking.xml";
			return Get(url, new Parameter[]{new Parameter("page", page.ToString())}, token);
		}

		#endregion

		#region List

		public PostingWebRequest DestroyList(ulong id, AccessToken token){
			const string url = "http://api.twitter.com/1/lists/destroy.xml";
			return Post(url, new Parameter[]{new Parameter("list_id", id.ToString())}, token);
		}

		#endregion

		#region Network

		protected GettingWebRequest Get(string url, Parameter[] prms, AccessToken token){
			HttpWebRequest req = GetWebRequest(url + ((prms.Length > 0) ? ("?" + prms.EncodeQuery()) : ""), prms);
			req.Method = WebRequestMethods.Http.Get;
			
			Consumer.AccessProtectedResource(token, req, url, "http://twitter.com/", prms);
			return new GettingWebRequest(req);
		}
		
		protected PostingWebRequest Post(string url, Parameter[] prms, AccessToken token){
			string query = prms.EncodeQuery();
#if !SILVERLIGHT
			byte[] data = Encoding.ASCII.GetBytes(query);
#else
			byte[] data = StringToAscii(query);
#endif
			
			HttpWebRequest req = GetWebRequest(url, prms);
			req.Method = WebRequestMethods.Http.Post;
			req.ContentType = "application/x-www-form-urlencoded";
			req.ContentLength = query.Length;
			
			Consumer.AccessProtectedResource(token, req, url, "http://twitter.com/", prms);
			return new PostingWebRequest(req, data);
		}

		#endregion
	}
}
