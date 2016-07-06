/*
	$Id: ISystemEntry.cs 315 2013-12-11 07:59:06Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace CatWalk.IOSystem {
	public interface ISystemEntry : IEquatable<ISystemEntry>{
		string Name{get;}
		string Path{get;}
		/// <summary>
		/// 表示用のパス
		/// ローカライズ済みのパスや物理パスなど
		/// </summary>
		string DisplayPath { get; }

		/// <summary>
		/// 表示用の名前
		/// ローカライズ済みの名前など
		/// </summary>
		string DisplayName{get;}
		ISystemEntry Parent{get;}
		bool IsExists();
		bool IsExists(CancellationToken token);
		bool IsExists(CancellationToken token, IProgress<double> progress);

		bool IsDirectory { get; }

		IEnumerable<ISystemEntry> GetChildren();
		IEnumerable<ISystemEntry> GetChildren(CancellationToken token);
		IEnumerable<ISystemEntry> GetChildren(CancellationToken token, IProgress<double> progress);

		/// <summary>
		/// このISystemDirectoryが持つ指定した識別子のISystemDirectoryを返す
		/// </summary>
		/// <param name="name">識別子</param>
		/// <returns>一致したISystemDirectory。見つからない場合はnull</returns>
		ISystemEntry GetChild(string name);

		ISystemEntry GetChild(string name, CancellationToken token);

		ISystemEntry GetChild(string name, CancellationToken token, IProgress<double> progress);


		/// <summary>
		/// 指定した識別子のSystemEntryを含むかどうか
		/// </summary>
		/// <returns></returns>
		bool Contains(string name);

		bool Contains(string name, CancellationToken token);

		bool Contains(string name, CancellationToken token, IProgress<double> progress);
		/*
		/// <summary>
		/// 表示パスを連結する
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		string ConcatDisplayPath(string name);
		*/
		/// <summary>
		/// パスを連結する。
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		string ConcatPath(string name);


	}
}
