/*
	$Id: AutoUpdater.cs 154 2011-01-06 04:43:26Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Net;
using System.Xml.Linq;
using System.Threading;

namespace CatWalk.Net{
	/*
	<packages>
		<package>
			<version></version>
			<infoversion></infoversion>
			<installeruri></installeruri>
			<changelog></changelog>
			<state></state>
			<date></date>
		</package>
		...
	</packages>
	*/
	public class AutoUpdater{
		public Collection<Uri> PackageUris{get; private set;}
		public int Timeout{get; set;}
		public AutoUpdater(params Uri[] packageUris){
			this.PackageUris = new Collection<Uri>(packageUris);
			this.Timeout = 100 * 1000;
		}
		
		public IEnumerable<GettingWebRequest> RequestUpdates(){
			foreach(var uri in this.PackageUris){
				WebRequest req = WebRequest.Create(uri);
				req.Timeout = this.Timeout;
				yield return new GettingWebRequest(req);
			}
		}
		
		public IEnumerable<UpdatePackage> CheckUpdates(IEnumerable<GettingWebRequest> requests){
			return this.CheckUpdates(requests, CancellationToken.None);
		}

		public IEnumerable<UpdatePackage> CheckUpdates(IEnumerable<GettingWebRequest> requests, CancellationToken token){
			foreach(var req in requests){
				XDocument doc;
				using(Stream stream = req.GetStream(token))
				using(StreamReader reader = new StreamReader(stream, Encoding.UTF8)){
					doc = XDocument.Load(stream);
				}
				foreach(var package in doc.Root.Elements("package")){
					UpdatePackage updatePackage = null;
					try{
						updatePackage = new UpdatePackage(package);
					}catch{
					}
					if(updatePackage != null){
						yield return updatePackage;
					}
				}
			}
		}
		
		public IEnumerable<UpdatePackage> CheckUpdates(){
			return this.CheckUpdates(this.RequestUpdates(), CancellationToken.None);
		}

		public IEnumerable<UpdatePackage> CheckUpdates(CancellationToken token){
			return this.CheckUpdates(this.RequestUpdates(), token);
		}
	}
	
	public class UpdatePackage{
		public Version Version{get; private set;}
		public Version InformationalVersion{get; private set;}
		public Uri InstallerUri{get; private set;}
		public string ChangeLog{get; private set;}
		public PackageState State{get; private set;}
		public DateTime Date{get; private set;}
		
		public UpdatePackage(XElement elm){
			this.Version = new Version((string)elm.Element("version"));
			this.InformationalVersion = new Version((string)elm.Element("infoversion"));
			this.InstallerUri = new Uri((string)elm.Element("installeruri"));
			this.ChangeLog = (string)elm.Element("changelog");
			this.State = (elm.Element("state") != null) ? (PackageState)Enum.Parse(typeof(PackageState), (string)elm.Element("state"), false) : PackageState.Unknown;
			this.Date = (elm.Element("date") != null) ? DateTime.ParseExact(
				(string)elm.Element("date"),
				"yyyy-MM-dd",
				System.Globalization.DateTimeFormatInfo.InvariantInfo,
				System.Globalization.DateTimeStyles.AllowWhiteSpaces) : DateTime.MinValue;
		}
		
		public string DownloadInstaller(){
			var client = new WebClient();
			string file = Path.GetTempPath() + Path.GetFileName(this.InstallerUri.AbsolutePath);
			client.DownloadFile(this.InstallerUri, file);
			return file;
		}
		
		public string DownloadInstaller(CancellationToken token){
			var client = new WebClient();
			token.Register(client.CancelAsync);
			var waitHandle = new ManualResetEvent(false);
			client.DownloadFileCompleted += delegate{
				waitHandle.Set();
			};
			
			string file = Path.GetTempPath() + Path.GetFileName(this.InstallerUri.AbsolutePath);
			client.DownloadFileAsync(this.InstallerUri, file, file);
			waitHandle.WaitOne();

			return file;
		}

		public void DownloadInstallerAsync(DownloadProgressChangedEventHandler progress, AsyncCompletedEventHandler completed){
			var client = new WebClient();
			if(progress != null){
				client.DownloadProgressChanged += progress;
			}
			if(completed != null){
				client.DownloadFileCompleted += completed;
			}
			
			string file = Path.GetTempPath() + Path.GetFileName(this.InstallerUri.AbsolutePath);
			client.DownloadFileAsync(this.InstallerUri, file, file);
		}
	}
	
	public enum PackageState{
		Stable,
		Beta,
		Alpha,
		ReleaseCandidate,
		Unknown,
	}
}