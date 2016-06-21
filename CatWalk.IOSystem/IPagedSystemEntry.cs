/*
	$Id: IPagedSystemDirectory.cs 299 2011-11-25 09:58:57Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatWalk.IOSystem {
	public interface IPagedSystemEntry : ISystemEntry{
		void MoveNextPage();
		void MovePreviousPage();
		void ResetPage();
		int Page{get;set;}
		int PageCount{get;}
	}
}
