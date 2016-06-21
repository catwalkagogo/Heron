/*
	$Id: TwitterApi.cs 259 2011-07-24 06:19:28Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using CatWalk.Net.OAuth;

namespace CatWalk.Net.Twitter{
	using Parameter = KeyValuePair<string, string>;

	/// <summary>
	/// TwitterAPIのプリミティブな関数群。
	/// </summary>
	public class TwitterApi{
		private static WeakReference _Default;
		static TwitterApi(){
		}

		public static TwitterApi Default{
			get{
				var target = (_Default != null) ? _Default.Target : null;
				if(target == null){
					var api = new TwitterApi();
					_Default = new WeakReference(api);
					return api;
				}else{
					return (TwitterApi)target;
				}
			}
		}
		
		protected TwitterApi(){
			ServicePointManager.Expect100Continue = false;
			this.Timeout = 100 * 1000;
		}

		#region 通信プロパティ

		public int Timeout{get; set;}

		#endregion

		#region Tweets

		public GettingWebRequest GetUserTimeline(ulong id, int count, int page, ulong sinceId, ulong maxId, bool trimUser, bool includeRts){
			return GetUserTimeline(id.ToString(), count, page, sinceId, maxId, trimUser, includeRts);
		}

		public GettingWebRequest GetUserTimeline(string id, int count, int page, ulong sinceId, ulong maxId, bool trimUser, bool includeRts){
			const string url = "http://api.twitter.com/1/statuses/user_timeline.xml";
			List<Parameter> prms = new List<Parameter>();
			if(!String.IsNullOrEmpty(id)){
				prms.Add(new Parameter("id", id));
			}
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
			if(includeRts){
				prms.Add(new Parameter("include_rts", "1"));
			}
			return Get(url, prms.ToArray());
		}

		public GettingWebRequest ShowStatus(ulong id, bool trimUser, bool includeEntities){
			const string url = "http://api.twitter.com/1/statuses/show/";
			var prms = new List<Parameter>();
			if(trimUser){
				prms.Add(new Parameter("trim_user", "1"));
			}
			if(includeEntities){
				prms.Add(new Parameter("include_entities", "1"));
			}
			return Get(url + id.ToString() + ".xml", prms.ToArray());
		}

		#endregion

		#region User
		
		public GettingWebRequest ShowUser(ulong id){
			return ShowUser(id.ToString());
		}
		
		public GettingWebRequest ShowUser(string name){
			const string url = "http://api.twitter.com/1/users/show.xml";
			return Get(url, new Parameter[]{new Parameter("id", name)});
		}

		#endregion

		#region List

		public GettingWebRequest GetLists(ulong id){
			const string url = "http://api.twitter.com/1/lists.xml";
			return Get(url, new Parameter[]{
				new Parameter("user_id", id.ToString())});
		}

		public GettingWebRequest GetLists(ulong id, long cursor){
			const string url = "http://api.twitter.com/1/lists.xml";
			return Get(url, new Parameter[]{
				new Parameter("user_id", id.ToString()),
				new Parameter("cursor", cursor.ToString())});
		}

		public GettingWebRequest GetListStatuses(ulong id, ulong sinceId, ulong maxId, int perPage, int page, bool trimUser){
			const string url = "http://api.twitter.com/1/lists/statuses.xml";
			List<Parameter> prms = new List<Parameter>();
			if(perPage > 0){
				prms.Add(new Parameter("per_page", perPage.ToString()));
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
			return Get(url, prms.ToArray());
		}

		#endregion

		#region 通信

		protected GettingWebRequest Get(string url, Parameter[] prms){
			HttpWebRequest req = GetWebRequest(url + ((prms.Length > 0) ? ("?" + prms.EncodeQuery()) : ""), prms);
			req.Method = WebRequestMethods.Http.Get;
			
			return new GettingWebRequest(req);
		}
		
		protected PostingWebRequest Post(string url, Parameter[] prms){
			string query = prms.EncodeQuery();
			byte[] data = Encoding.ASCII.GetBytes(query);
			
			HttpWebRequest req = GetWebRequest(url, prms);
			req.Method = WebRequestMethods.Http.Post;
			req.ContentType = "application/x-www-form-urlencoded";
			req.ContentLength = query.Length;
			
			return new PostingWebRequest(req, data);
		}
				
		protected HttpWebRequest GetWebRequest(string url, Parameter[] prms){
			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
			req.Timeout = this.Timeout;
			req.KeepAlive = true;
			return req;
		}

		#endregion
		
		#region その他
		/*
		private string BuildUrl(string url, Parameter[] prms){
			return (prms.Length > 0) ? url + "?" + BuildQuery(prms) : url;
		}
		
		private string BuildQuery(Parameter[] prms){
			var query = from prm in prms
			            select prm.Name + "=" + prm.Value;
			return String.Join("&", query.ToArray());
		}
		*/
		public static bool TryParseDateTime(string s, out DateTime result){
			const string format = @"ddd MMM dd HH':'mm':'ss zz'00' yyyy";
			return DateTime.TryParseExact(
				s,
				format,
				System.Globalization.DateTimeFormatInfo.InvariantInfo,
				System.Globalization.DateTimeStyles.AllowWhiteSpaces,
				out result);
		}
		/*
		public bool TryParseColor(string s, out Color color){
			try{
				int n = Convert.ToInt32(s, 16);
				byte r = (byte)((n & 0xff0000) >> 16);
				byte g = (byte)((n & 0x00ff00) >> 8);
				byte b = (byte)((b & 0x0000ff));
				color = Color.FromRgb(r, g, b);
				return true;
			}catch{
				return false;
			}
		}
		*/
		
		public string GetErrorMessage(WebException ex){
			if(ex.Status == WebExceptionStatus.ProtocolError){
				HttpWebResponse req = ex.Response as HttpWebResponse;
				if(req != null){
					switch(req.StatusCode){
						case HttpStatusCode.BadRequest:
							return "400: リクエストが不正か、APIの使用制限を超えています。";
						case HttpStatusCode.Unauthorized:
							return "401: アカウントの認証を失敗しました。OAuth認証をやり直してください。";
						case HttpStatusCode.Forbidden:
							return "403: サーバーからアクセスが禁止されています。更新制限を超えている可能性があります。";
						case HttpStatusCode.BadGateway:
							return "501: Twitterのサーバーがダウンしているか、アップデート中です。";
						case HttpStatusCode.ServiceUnavailable:
							return "503: Twitterのサービスが使用できない状態にあります。しばらく後でやり直してください。";
						default:
							return String.Format("{0}: {1}", (int)req.StatusCode, ex.Message);
					}
				}
			}
			return ex.Message;
		}
		
		#endregion
	}
}