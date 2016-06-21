/*
	OAuthLib by 7k8m http://oauthlib.codeplex.com
	
	$Id: Consumer.cs 259 2011-07-24 06:19:28Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Threading;

namespace CatWalk.Net.OAuth {
	using Parameter = KeyValuePair<string, string>;

	public class Consumer {
		#region Data

		private readonly string _consumerKey;
		private readonly string _consumerSecret;

		public int Timeout{get; set;}

		#endregion

		#region Constructor

		/// <summary>
		/// Construct new Consumer instance.
		/// </summary>
		/// <param name="consumerKey">Key of consumer</param>
		/// <param name="consumerSecret">Secret of consumer</param>
		public Consumer(
			string consumerKey,
			string consumerSecret
			) {
			_consumerKey = consumerKey;
			_consumerSecret = consumerSecret;
			this.Timeout = 100 * 1000;
		}

		#endregion

		#region ObtainUnauthorizedRequestToken

		/// <summary>
		/// Obtain unauthorized request token from service provider
		/// </summary>
		/// <param name="requestTokenUrl">Request token URL</param>
		/// <param name="realm">Realm for obtaining request token</param>
		/// <returns>Obtained request token</returns>
		public RequestToken ObtainUnauthorizedRequestToken(string requestTokenUrl, string realm) {
			return ObtainUnauthorizedRequestToken(requestTokenUrl, null, realm, CancellationToken.None);
		}

		public RequestToken ObtainUnauthorizedRequestToken(string requestTokenUrl, string realm, CancellationToken token){
			return ObtainUnauthorizedRequestToken(requestTokenUrl, null, realm, token);
		}

		/// <summary>
		/// Obtain unauthorized request token from service provider
		/// </summary>
		/// <param name="requestTokenUrl">Request token URL</param>
		/// <param name="callbackURL">An absolute URL to which the Service Provider will redirect the User back when the Obtaining User Authorization step is completed.</param>
		/// <param name="realm">Realm for obtaining request token</param>
		/// <returns>Obtained request token</returns>
		public RequestToken ObtainUnauthorizedRequestToken(string requestTokenUrl, string callbackURL, string realm){
			return ObtainUnauthorizedRequestToken(requestTokenUrl, callbackURL, realm, CancellationToken.None);
		}

		public RequestToken ObtainUnauthorizedRequestToken(string requestTokenUrl, string callbackURL, string realm, CancellationToken token){
			var req = RequestUnauthorizedRequestToken(requestTokenUrl, callbackURL, realm);
			return RequestToken.FromRequest(req, token);
		}

		public GettingWebRequest RequestUnauthorizedRequestToken(string requestTokenUrl, string callbackURL, string realm){
			string oauth_consumer_key = _consumerKey;
			string oauth_signature_method = "HMAC-SHA1";
			string oauth_timestamp =
				((DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks) / (1000 * 10000)).ToString();
			string oauth_nonce =
				Guid.NewGuid().ToString();
			string oauth_callback =
				(callbackURL != null && callbackURL.Length > 0 ?
					callbackURL :
					"oob"
				);

			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(requestTokenUrl);
#if !SILVERLIGHT
			req.Timeout = this.Timeout;
#endif
			req.Method = WebRequestMethods.Http.Post;

			string oauth_signature =
				CreateHMACSHA1Signature(
					req.Method,
					requestTokenUrl,
					new Parameter[]{
						new Parameter("oauth_consumer_key",oauth_consumer_key),
						new Parameter ("oauth_signature_method",oauth_signature_method ),
						new Parameter ("oauth_timestamp",oauth_timestamp),
						new Parameter ("oauth_nonce",oauth_nonce ),
						new Parameter ("oauth_callback",oauth_callback)
					},
					_consumerSecret,
					null
				);

			req.Headers.Add(
				"Authorization: OAuth " +
				"realm=\"" + realm + "\"," +
				"oauth_consumer_key=\"" + Uri.EscapeDataString(oauth_consumer_key) + "\"," +
				"oauth_signature_method=\"" + Uri.EscapeDataString(oauth_signature_method) + "\"," +
				"oauth_signature=\"" + Uri.EscapeDataString(oauth_signature) + "\"," +
				"oauth_timestamp=\"" + Uri.EscapeDataString(oauth_timestamp) + "\"," +
				"oauth_nonce=\"" + Uri.EscapeDataString(oauth_nonce) + "\"," +
				"oauth_callback=\"" + Uri.EscapeDataString(oauth_callback) + "\""
			);

			return new GettingWebRequest(req);
		}

		#endregion

		#region RequestAccessToken

		/// <summary>
		/// Request access token responding to authenticated request token.
		/// </summary>
		/// <param name="verifier">Verifier string for authenticaed request token</param>
		/// <param name="requestToken">Authenticated request token</param>
		/// <param name="accessTokenUrl">Access token URL</param>
		/// <param name="realm">Realm for requesting access token</param>
		/// <returns>Responding access token</returns>
		public AccessToken GetAccessToken(string verifier, RequestToken requestToken, string accessTokenUrl, string realm) {
			return this.GetAccessToken(verifier, requestToken, accessTokenUrl, realm, CancellationToken.None);
		}

		public AccessToken GetAccessToken(string verifier, RequestToken requestToken, string accessTokenUrl, string realm, CancellationToken token) {
			return AccessToken.FromRequest(RequestAccessToken(verifier, requestToken, accessTokenUrl, realm), token);
		}

		public GettingWebRequest RequestAccessToken(string verifier, RequestToken requestToken, string accessTokenUrl, string realm) {
			string oauth_consumer_key = _consumerKey;
			string oauth_token = requestToken.TokenValue;
			string oauth_signature_method = "HMAC-SHA1";
			string oauth_timestamp =
				((DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks) / (1000 * 10000)).ToString();
			string oauth_nonce =
				Guid.NewGuid().ToString();

			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(accessTokenUrl);
#if !SILVERLIGHT
			req.Timeout = this.Timeout;
#endif
			req.Method = WebRequestMethods.Http.Post;

			string oauth_signature =
				CreateHMACSHA1Signature(
					req.Method,
					accessTokenUrl,
					new Parameter[]{
						new Parameter("oauth_consumer_key",oauth_consumer_key),
						new Parameter("oauth_token",oauth_token ),
						new Parameter ("oauth_signature_method",oauth_signature_method ),
						new Parameter ("oauth_timestamp",oauth_timestamp),
						new Parameter ("oauth_nonce",oauth_nonce ),
						new Parameter ("oauth_verifier",verifier ),
					},
					_consumerSecret,
					requestToken.TokenSecret
				);

			req.Headers.Add(
				"Authorization: OAuth " +
				"realm=\"" + realm + "\"," +
				"oauth_consumer_key=\"" + Uri.EscapeDataString(oauth_consumer_key) + "\"," +
				"oauth_token=\"" + Uri.EscapeDataString(oauth_token) + "\"," +
				"oauth_signature_method=\"" + Uri.EscapeDataString(oauth_signature_method) + "\"," +
				"oauth_signature=\"" + Uri.EscapeDataString(oauth_signature) + "\"," +
				"oauth_timestamp=\"" + Uri.EscapeDataString(oauth_timestamp) + "\"," +
				"oauth_nonce=\"" + Uri.EscapeDataString(oauth_nonce) + "\"," +
				"oauth_verifier=\"" + Uri.EscapeDataString(verifier) + "\""
			);

			return new GettingWebRequest(req);
		}

		#endregion

		#region AccessProtectedResource

		/// <summary>
		/// Access protected resource with access token
		/// </summary>
		/// <param name="accessToken">Access token</param>
		/// <param name="urlString">URL string for accessing protected resource</param>
		/// <param name="method">HTTP method to access</param>
		/// <param name="authorizationRealm">realm for accessing protected resource</param>
		/// <param name="queryParameters">Query parameter to be sent</param>
		/// <returns>HttpWebResponse from protected resource</returns>
		public void AccessProtectedResource(
			AccessToken accessToken,
			HttpWebRequest req,
			string urlString,
			string authorizationRealm,
			Parameter[] queryParameters
		) {
			AccessProtectedResource(
				accessToken,
				req,
				urlString,
				authorizationRealm,
				queryParameters,
				null);
		}

		/// <summary>
		/// Access protected resource with access token
		/// </summary>
		/// <param name="accessToken">Access token</param>
		/// <param name="urlString">URL string for accessing protected resource</param>
		/// <param name="method">HTTP method to access</param>
		/// <param name="authorizationRealm">realm for accessing protected resource</param>
		/// <param name="queryParameters">Query parameter to be sent</param>
		/// <param name="additionalParameters">Parameters added to Authorization header</param>
		/// <returns>HttpWebResponse from protected resource</returns>
		public void AccessProtectedResource(
			AccessToken accessToken,
			HttpWebRequest req,
			string urlString,
			string authorizationRealm,
			Parameter[] queryParameters,
			Parameter[] additionalParameters) {

			if(additionalParameters == null)
				additionalParameters = new Parameter[0];

			if(queryParameters == null)
				queryParameters = new Parameter[0];

			if(accessToken == null)
				accessToken = new AccessToken("", "");

			string oauth_consumer_key = _consumerKey;
			string oauth_token = accessToken.TokenValue;
			string oauth_signature_method = "HMAC-SHA1";
			string oauth_timestamp =
				((DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks) / (1000 * 10000)).ToString();
			string oauth_nonce =
				Guid.NewGuid().ToString();
#if !SILVERLIGHT
			req.Timeout = this.Timeout;

			//Twitter service does not accept expect100continue
			req.ServicePoint.Expect100Continue = false;
#endif
			string oauth_signature =
				CreateHMACSHA1Signature(
					req.Method,
					urlString,
					(new Parameter[]{
						new Parameter("oauth_consumer_key",oauth_consumer_key),
						new Parameter("oauth_token",oauth_token ),
						new Parameter ("oauth_signature_method",oauth_signature_method ),
						new Parameter ("oauth_timestamp",oauth_timestamp),
						new Parameter ("oauth_nonce",oauth_nonce )
					}).Concat(additionalParameters).Concat(queryParameters),
					_consumerSecret,
					accessToken.TokenSecret
				);

			req.Headers.Add(
				"Authorization: OAuth " +
				"realm=\"" + authorizationRealm + "\"," +
				"oauth_consumer_key=\"" + Uri.EscapeDataString(oauth_consumer_key) + "\"," +
				"oauth_token=\"" + Uri.EscapeDataString(oauth_token) + "\"," +
				"oauth_signature_method=\"" + Uri.EscapeDataString(oauth_signature_method) + "\"," +
				"oauth_signature=\"" + Uri.EscapeDataString(oauth_signature) + "\"," +
				"oauth_timestamp=\"" + Uri.EscapeDataString(oauth_timestamp) + "\"," +
				"oauth_nonce=\"" + Uri.EscapeDataString(oauth_nonce) + "\"" +
				(additionalParameters.Length > 0 ? "," + additionalParameters.EncodeQuery("\"") : "")
			);
		}

		#endregion

		#region CreateHMACSHA1Signature

		private static string CreateHMACSHA1Signature(
			string method,
			string url,
			IEnumerable<Parameter> parameters,
			string consumerSecret) {
			return CreateHMACSHA1Signature(method, url, parameters, consumerSecret, null);
		}

		private static string CreateHMACSHA1Signature(
			string method,
			string url,
			IEnumerable<Parameter> parameters,
			string consumerSecret,
			string tokenSecret) {

			if(consumerSecret == null)
				throw new NullReferenceException();

			if(tokenSecret == null)
				tokenSecret = "";

			method = method.ToUpper();

			url = url.ToLower();
			Uri uri = new Uri(url);
			url =
				uri.Scheme + "://" +
				uri.Host +
				((uri.Scheme.Equals("http") && uri.Port == 80 ||
							uri.Scheme.Equals("https") && uri.Port == 443) ?
							"" :
							uri.Port.ToString()
				) +
				uri.AbsolutePath;

			string concatenatedParameter = parameters.EncodeQuery(true);

			HMACSHA1 alg = new HMACSHA1
				(
					Encode(
						Uri.EscapeDataString(consumerSecret) + "&" +
						Uri.EscapeDataString(tokenSecret)
					)
				);

			return
				System.Convert.ToBase64String(
					alg.ComputeHash(
						Encode(
							Uri.EscapeDataString(method) + "&" +
							Uri.EscapeDataString(url) + "&" +
							Uri.EscapeDataString(concatenatedParameter)
						)
					)
				);

		}

		#endregion

		#region BuildUserAuthorizationURL

		/// <summary>
		/// Build user authorization URL to authorize request token
		/// </summary>
		/// <param name="userAuthorizationUrl">User authorization URL served by Service Provider</param>
		/// <param name="requestToken">Request token</param>
		/// <returns>user authorization URL to authorize request token</returns>
		public static string BuildUserAuthorizationURL(
			string userAuthorizationUrl,
			RequestToken requestToken
			) {

			Uri uri = new Uri(userAuthorizationUrl);

			return
				uri.OriginalString +
				(uri.Query != null && uri.Query.Length > 0 ?
				"&" : "?") +
				"oauth_token=" + Uri.EscapeDataString(requestToken.TokenValue);

		}

		#endregion

		#region Encode / Decode

		private static byte[] Encode(string val) {
			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms, Encoding.ASCII);

			sw.Write(val);
			sw.Flush();

			return ms.ToArray();

		}

		private static string Decode(byte[] val) {
			MemoryStream ms = new MemoryStream(val);
			StreamReader sr = new StreamReader(ms, Encoding.ASCII);
			return sr.ReadToEnd();

		}

		#endregion

		#region RequestAccessToken XOuth

		public AccessToken RequestAccessToken(string url, string username, string password, string realm){
			return this.RequestAccessToken(url, username, password, realm, "client_auth");
		}

		public AccessToken RequestAccessToken(string url, string username, string password, string realm, string mode){
			Parameter[] resp;
			return this.RequestAccessToken(url, username, password, realm, "client_auth", out resp);
		}

		public AccessToken RequestAccessToken(string url, string username, string password, string realm, string mode, out Parameter[] responseParameters){
			string oauth_consumer_key = _consumerKey;
			string oauth_signature_method = "HMAC-SHA1";
			string oauth_timestamp =
				((DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks) / (1000 * 10000)).ToString();
			string oauth_nonce =
				Guid.NewGuid().ToString();
	
			Parameter[] urlPrms = new Parameter[]{
				new Parameter ("x_auth_username", username),
				new Parameter ("x_auth_password", password),
				new Parameter ("x_auth_mode", mode),
			};
			var query = urlPrms.EncodeQuery();
			byte[] data = Encoding.ASCII.GetBytes(query);

			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
			req.Method = WebRequestMethods.Http.Post;
			req.ContentType = "application/x-www-form-urlencoded";
			req.ContentLength = query.Length;
#if !SILVERLIGHT
			req.Timeout = this.Timeout;
#endif
			Parameter[] prms = new Parameter[]{
				new Parameter ("oauth_consumer_key",oauth_consumer_key),
				new Parameter ("oauth_nonce",oauth_nonce ),
				new Parameter ("oauth_signature_method",oauth_signature_method ),
				new Parameter ("oauth_timestamp",oauth_timestamp),
				new Parameter ("oauth_version", "1.0"),
			};

			string oauth_signature =
				CreateHMACSHA1Signature(
					req.Method,
					url,
					prms.Concat(urlPrms),
					_consumerSecret
				);

			var headerString = String.Join("\",",
				prms.Concat(new Parameter[]{new Parameter("oauth_signature", oauth_signature)})
				.Select(prm => prm.Key + "=\"" + Uri.EscapeDataString(prm.Value))) + "\"";
			req.Headers.Add("Authorization: OAuth realm=\"" + realm + "\"," + headerString);

			using(Stream stream = req.GetRequestStream()){
				stream.Write(data, 0, data.Length);
			}

			HttpWebResponse resp = null;
			try {
				resp = (HttpWebResponse)req.GetResponse();
				StreamReader sr = new StreamReader(resp.GetResponseStream());

				responseParameters =
					NetUtility.ParseQueryString(sr.ReadToEnd()).ToArray();

				string accessToken = null;
				string accessTokenSecret = null;
				foreach(Parameter param in responseParameters) {
					if(param.Key == "oauth_token")
						accessToken = param.Value;

					if(param.Key == "oauth_token_secret")
						accessTokenSecret = param.Value;
				}

				if(accessToken == null || accessTokenSecret == null)
					throw new InvalidOperationException();

				return new AccessToken(accessToken, accessTokenSecret);

			} finally {
				if(resp != null)
					resp.Close();
			}
		}

		#endregion
	}
}

