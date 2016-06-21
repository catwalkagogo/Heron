/*
	$Id: User.cs 325 2014-01-08 16:21:52Z catwalkagogo@gmail.com $
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
	public class User : IEquatable<User> {
		public TwitterApi TwitterApi{get; private set;}
	
		#region Data

		public ulong Id{get; private set;}
		public string Name{get; private set;}
		public string ScreenName{get; private set;}
		public string Location{get; private set;}
		public string Description{get; private set;}
		public string ProfileImageUrl{get; private set;}
		public string Url{get; private set;}
		public bool Protected{get; private set;}
		public int FollowersCount{get; private set;}
		public int ProfileBackgroundColor{get; private set;}
		public int ProfileTextColor{get; private set;}
		public int ProfileLinkColor{get; private set;}
		public int ProfileSidebarFillColor{get; private set;}
		public int ProfileSidebarBorderColor{get; private set;}
		public int FriendsCount{get; private set;}
		public DateTime CreatedAt{get; private set;}
		public int FavouritesCount{get; private set;}
		public int UtcOffset{get; private set;}
		public string TimeZone{get; private set;}
		public string ProfileBackgroundImageUrl{get; private set;}
		public bool ProfileBackgroundTile{get; private set;}
		public string Notifications{get; private set;}
		public bool GeoEnabled{get; private set;}
		public bool Verified{get; private set;}
		public bool Following{get; private set;}
		public int StatusesCount{get; private set;}
		public int ListedCount{get; private set;}
		public string Lang{get; private set;}
		
		public User(XElement element){
			if(element == null){
				throw new ArgumentNullException("element");
			}
			DateTime dt;
			bool b;
			int n;
			
			this.Id =  (ulong)element.Element("id");
			this.Name = (string)element.Element("name");
			this.ScreenName = (string)element.Element("screen_name");
			this.Location = (string)element.Element("location");
			this.Description = (string)element.Element("description");
			this.ProfileImageUrl = (string)element.Element("profile_image_url");
			this.Url = (string)element.Element("url");
			if(Boolean.TryParse((string)element.Element("protected"), out b)){
				this.Protected = b;
			}
			if(Int32.TryParse((string)element.Element("followers_count"), out n)){
				this.FollowersCount = n;
			}
			
			this.ProfileBackgroundColor = Convert.ToInt32((string)element.Element("profile_background_color"), 16);
			this.ProfileTextColor = Convert.ToInt32((string)element.Element("profile_text_color"), 16);
			this.ProfileLinkColor = Convert.ToInt32((string)element.Element("profile_link_color"), 16);
			this.ProfileSidebarFillColor = Convert.ToInt32((string)element.Element("profile_sidebar_fill_color"), 16);
			this.ProfileSidebarBorderColor = Convert.ToInt32((string)element.Element("profile_sidebar_border_color"), 16);
			this.FriendsCount = (int)element.Element("friends_count");
			if(TwitterApi.TryParseDateTime((string)element.Element("created_at"), out dt)){
				this.CreatedAt = dt;
			}
			if(Int32.TryParse((string)element.Element("favourites_count"), out n)){
				this.FavouritesCount = n;
			}
			if(Int32.TryParse((string)element.Element("utc_offset"), out n)){
				this.UtcOffset = n;
			}
			this.TimeZone = (string)element.Element("time_zone");
			this.ProfileBackgroundImageUrl = (string)element.Element("profile_background_image_url");
			if(Boolean.TryParse((string)element.Element("profile_background_tile"), out b)){
				this.ProfileBackgroundTile = b;
			}
			this.Notifications = (string)element.Element("notifications");
			if(Boolean.TryParse((string)element.Element("geo_enabled"), out b)){
				this.GeoEnabled = b;
			}
			if(Boolean.TryParse((string)element.Element("verified"), out b)){
				this.Verified = b;
			}
			if(Boolean.TryParse((string)element.Element("following"), out b)){
				this.Following = b;
			}
			if(Int32.TryParse((string)element.Element("statuses_count"), out n)){
				this.StatusesCount = n;
			}
			this.ListedCount = (int)element.Element("listed_count");
			this.Lang = (string)element.Element("lang");
		}

		public static User FromId(ulong id){
			return FromId(id, CancellationToken.None);
		}
		public static User FromId(ulong id, CancellationToken token){
			var req = TwitterApi.Default.ShowUser(id);
			using(Stream stream = req.GetStream(token))
			using(StreamReader reader = new StreamReader(stream, Encoding.UTF8)){
				var xml = XElement.Load(reader);
				return new User(xml);
			}
		}

		public static User FromName(string screenName){
			return FromName(screenName, CancellationToken.None);
		}
		public static User FromName(string screenName, CancellationToken token){
			var req = TwitterApi.Default.ShowUser(screenName);
			using(Stream stream = req.GetStream(token))
			using(StreamReader reader = new StreamReader(stream, Encoding.UTF8)){
				var xml = XElement.Load(reader);
				return new User(xml);
			}
		}

		#endregion

		#region API

		public Timeline GetTimeline(int count, int page, ulong sinceId, ulong maxId, bool trimUser, bool includeRts){
			return this.GetTimeline(count, page, sinceId, maxId, trimUser, includeRts, CancellationToken.None);
		}

		public Timeline GetTimeline(int count, int page, ulong sinceId, ulong maxId, bool trimUser, bool includeRts, CancellationToken token){
			var req = TwitterApi.GetUserTimeline(this.Id, count, page, sinceId, maxId, trimUser, includeRts);
			return new UserTimeline(
				Seq.Using(
					() => req.GetStream(token),
					stream => XmlUtility.FromStream(stream)
				).Select(elm => new Status(elm)),
				this.Id.ToString(),
				trimUser,
				includeRts); 
		}

		public static Timeline GetTimeline(string screenName, int count, int page, ulong sinceId, ulong maxId, bool trimUser, bool includeRts){
			return GetTimeline(screenName, count, page, sinceId, maxId, trimUser, includeRts, CancellationToken.None);
		}
		public static Timeline GetTimeline(string screenName, int count, int page, ulong sinceId, ulong maxId, bool trimUser, bool includeRts, CancellationToken token){
			var req = TwitterApi.Default.GetUserTimeline(screenName, count, page, sinceId, maxId, trimUser, includeRts);
			return new UserTimeline(
				Seq.Using(
					() => req.GetStream(token),
					stream => XmlUtility.FromStream(stream)
				).Select(elm => new Status(elm)),
				screenName,
				trimUser,
				includeRts); 
		}

		#endregion

		#region IEquatable<User> Members

		public override bool Equals(object obj){
			if(obj is User){
				return this.Equals((User)obj);
			}else{
				return false;
			}
		}
		
		public static bool operator ==(User a, User b){
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
		
		public static bool operator !=(User a, User b){
			return !(a == b);
		}
		
		public override int GetHashCode(){
			return this.Id.GetHashCode();
		}

		public bool Equals(User other) {
			if(other != null){
				return this.Id.Equals(other.Id);
			}else{
				return false;
			}
		}

		#endregion
	}
}