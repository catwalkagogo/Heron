/*
	OAuthLib by 7k8m http://oauthlib.codeplex.com
	
	$Id: AccessToken.cs 259 2011-07-24 06:19:28Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Threading;

namespace CatWalk.Net.OAuth {
	using Parameter = KeyValuePair<string, string>;
	
	/// <summary>
	/// Stands for access token
	/// </summary>
#if !SILVERLIGHT
	[Serializable]
#endif
	[TypeConverter(typeof(AccessTokenConverter))]
	public class AccessToken : Token {
		public AccessToken(String tokenValue, String tokenSecret) : base(tokenValue, tokenSecret) {
		}

		public static AccessToken FromRequest(GettingWebRequest req, CancellationToken token){
			using(var res = req.GetResponse(token)){
				return FromResponse(res);
			}
		}

		public static AccessToken FromResponse(WebResponse resp){
			StreamReader sr = new StreamReader(resp.GetResponseStream());

			string accessToken = null;
			string accessTokenSecret = null;
			foreach(Parameter param in NetUtility.ParseQueryString(sr.ReadToEnd())) {
				if(param.Key == "oauth_token")
					accessToken = param.Value;

				if(param.Key == "oauth_token_secret")
					accessTokenSecret = param.Value;
			}

			if(accessToken == null || accessTokenSecret == null)
				throw new InvalidOperationException();

			return new AccessToken(accessToken, accessTokenSecret);
		}
	}

	public class AccessTokenConverter : TypeConverter{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType.Equals(typeof(string)) || sourceType.Equals(typeof(AccessToken));
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType.Equals(typeof(string)) || destinationType.Equals(typeof(AccessToken));
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(value is string){
				return ConvertFromStringInternal(value as string);
			}else if(value is AccessToken){
				return ConvertToString(value as AccessToken);
			}else{
				return value;
			}
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType.Equals(value.GetType())){
				return value;
			}else if(value is string){
				if(destinationType.Equals(typeof(AccessToken))){
					return ConvertFromStringInternal(value as string);
				}
			}else if(value is AccessToken){
				if(destinationType.Equals(typeof(string))){
					return ConvertToString(value as AccessToken);
				}
			}
			return null;
		}

		private static string ConvertToString(AccessToken token){
			var text = token.TokenValue + "&" + token.TokenSecret;
			return Protect(text, optionalEntropy);
		}

		private static object ConvertFromStringInternal(string text){
			var tokens = Unprotect(text, optionalEntropy).Split('&');
			return new AccessToken(tokens[0], tokens[1]);
		}

		private static readonly byte[] optionalEntropy = new byte[]{0x01, 0xba, 0x33, 0x12, 0x9a};

		private static string Protect(string plain, byte[] entropy){
			if(plain == null){
				return "";
			}else{
#if !SILVERLIGHT
				var data = Encoding.UTF8.GetBytes(plain);
				return Convert.ToBase64String(ProtectedData.Protect(data, optionalEntropy, DataProtectionScope.CurrentUser));
#else
				var password = Encoding.UTF8.GetString(entropy, 0, entropy.Length);
				using(Aes aes = new AesManaged()) {
					Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, entropy);
					aes.Key = deriveBytes.GetBytes(128 / 8);
					aes.IV = aes.Key;
					using(MemoryStream encryptionStream = new MemoryStream()) {
						using(CryptoStream encrypt = new CryptoStream(encryptionStream, aes.CreateEncryptor(), CryptoStreamMode.Write)) {
							byte[] utfD1 = UTF8Encoding.UTF8.GetBytes(plain);
							encrypt.Write(utfD1, 0, utfD1.Length);
							encrypt.FlushFinalBlock();
						}
						return Convert.ToBase64String(encryptionStream.ToArray());
					}
				}
#endif
			}
		}
		
		private static string Unprotect(string encrypted, byte[] entropy){
			if(encrypted == null){
				return "";
			}else{
#if !SILVERLIGHT
				var data = Convert.FromBase64String(encrypted);
				return Encoding.UTF8.GetString(ProtectedData.Unprotect(data, optionalEntropy, DataProtectionScope.CurrentUser));
#else
				var password = Encoding.UTF8.GetString(entropy, 0, entropy.Length);

				using(Aes aes = new AesManaged()) {
					Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, entropy);
					aes.Key = deriveBytes.GetBytes(128 / 8);
					aes.IV = aes.Key;

					using(MemoryStream decryptionStream = new MemoryStream()) {
						using(CryptoStream decrypt = new CryptoStream(decryptionStream, aes.CreateDecryptor(), CryptoStreamMode.Write)) {
							byte[] encryptedData = Convert.FromBase64String(encrypted);
							decrypt.Write(encryptedData, 0, encryptedData.Length);
							decrypt.Flush();
						}
						byte[] decryptedData = decryptionStream.ToArray();
						return UTF8Encoding.UTF8.GetString(decryptedData, 0, decryptedData.Length);
					}
				}
#endif
			}
		}
	}
}
