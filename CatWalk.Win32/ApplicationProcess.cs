/*
	$Id: ApplicationProcess.cs 271 2011-07-29 00:17:59Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using System.Runtime.Serialization;

namespace CatWalk.Win32{
	/// <summary>
	/// 二重起動防止・プロセス間通信クラス
	/// </summary>
	public static class ApplicationProcess{
		private static Mutex mutex;
		private static bool isStarted;
		private static Lazy<IRemoteControler> controler = null;
		private static Lazy<IChannel> serverChannel = null;
		private static string id;
		private static IDictionary<string, Delegate> actions;
		
		private const string RemoteControlerUri = "controler";
		
		#region コンストラクタ
		
		static ApplicationProcess(){
			id = Environment.UserName + "@" + Uri.EscapeUriString(Assembly.GetEntryAssembly().Location.ToLower());
			mutex = new Mutex(false, id, out isStarted);
			isStarted = !isStarted;
			
			if(isStarted){
				controler = new Lazy<IRemoteControler>(() => GetRemoteControler());
				mutex.Close();
			}else{
				actions = new Dictionary<string, Delegate>();
				serverChannel = new Lazy<IChannel>(() => RegisterRemoteControler(typeof(RemoteControler)));
			}
		}

		#endregion
		
		#region 関数
		
		private static IRemoteControler GetRemoteControler(){
			IpcClientChannel clientChannel = new IpcClientChannel();
			ChannelServices.RegisterChannel(clientChannel, true);
			return (IRemoteControler)Activator.GetObject(typeof(IRemoteControler), "ipc://" + id + "/" + RemoteControlerUri);
		}
		
		private static IpcServerChannel RegisterRemoteControler(Type type){
			if(!serverChannel.IsValueCreated && !(isStarted)){
				// IServerChannelSinkProvider初期化
				BinaryServerFormatterSinkProvider sinkProvider = new BinaryServerFormatterSinkProvider();
				sinkProvider.TypeFilterLevel = TypeFilterLevel.Full;
				
				// IpcServerChannel初期化
				var srvChannel = new IpcServerChannel("ipc", id, sinkProvider);
				ChannelServices.RegisterChannel(srvChannel, true);
				
				// リモートオブジェクト登録
				RemotingConfiguration.RegisterWellKnownServiceType(type, RemoteControlerUri, WellKnownObjectMode.Singleton);

				return srvChannel;
			}else{
				throw new InvalidOperationException();
			}
		}
		
		/// <summary>
		/// プロセス間通信で<see cref="Actions"/>に登録した関数を実行。
		/// </summary>
		/// <param name="name">関数名</param>
		/// <seealso cref="Actions"/>
		public static void InvokeRemote(string name){
			if(controler == null){
				throw new InvalidOperationException();
			}
			controler.Value.Invoke(name);
		}
		
		/// <summary>
		/// プロセス間通信で<see cref="Actions"/>に登録した関数を実行。
		/// </summary>
		/// <param name="name">関数名</param>
		/// <param name="args">引数</param>
		/// <seealso cref="Actions"/>
		public static void InvokeRemote(string name, params object[] args){
			if(controler == null){
				throw new InvalidOperationException();
			}
			controler.Value.Invoke(name, args);
		}
		
		#endregion
		
		#region プロパティ
		
		/// <summary>
		/// 現在のプロセスが一つ目かどうかを取得。
		/// </summary>
		public static bool IsFirst{
			get{
				return !isStarted;
			}
		}
		
		private static object dummy;

		/// <summary>
		/// プロセス間通信で実行する関数。
		/// キーに呼び出しに使用する関数名、値に<see cref="System.Delegate"/>を指定する。
		/// </summary>
		public static IDictionary<string, Delegate> Actions{
			get{
				dummy = serverChannel.Value;
				return actions;
			}
		}
		
		#endregion
		
		#region クラス
		
		private interface IRemoteControler{
			void Invoke(string name);
			void Invoke(string name, object[] args);
		}
		
		private class RemoteControler : MarshalByRefObject, IRemoteControler{
			public void Invoke(string name){
				ApplicationProcess.actions[name].DynamicInvoke(null);
			}
			
			public void Invoke(string name, object[] args){
				ApplicationProcess.actions[name].DynamicInvoke(args);
			}
		}
		
		#endregion
	}
}