/*
	$Id: TwitterList.cs 253 2011-07-19 10:33:14Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading;
using System.IO;
using System.Net;

namespace CatWalk.Net.Twitter {
	public class TwitterList {
		public ulong Id{get; private set;}
		public string Name{get; private set;}
		public string FullName{get; private set;}
		public string Slug{get; private set;}
		public string Description{get; private set;}
		public int SubscriberCount{get; private set;}
		public int MemberCount{get; private set;}
		public string Uri{get; private set;}
		public bool Following{get; private set;}
		public string Mode{get; private set;}
		public User User{get; private set;}

		public TwitterList(XElement elm){
			if(elm == null){
				throw new ArgumentNullException("elm");
			}

			this.Id = (ulong)elm.Element("id");
			this.Name = (string)elm.Element("name");
			this.FullName = (string)elm.Element("full_name");
			this.Slug = (string)elm.Element("slug");
			this.Description = (string)elm.Element("description");
			this.SubscriberCount = (int)elm.Element("subscriber_count");
			this.MemberCount = (int)elm.Element("member_count");
			this.Uri = (string)elm.Element("uri");
			this.Following = (bool)elm.Element("following");
			this.Mode = (string)elm.Element("mode");
			this.User = new User(elm.Element("user"));
		}

		#region API

		public IEnumerable<Status> GetTimeline(int count, int page, ulong sinceId, ulong maxId, bool trimUser, CancellationToken token){
			var req = TwitterApi.Default.GetListStatuses(this.Id, sinceId, maxId, count, page, trimUser);
			using(Stream stream = req.GetStream(token)){
				foreach(XElement status in XmlUtility.FromStream(stream)){
					yield return new Status(status);
				}
			}
		}

		#endregion
	}
}
