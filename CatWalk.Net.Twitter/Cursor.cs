/*
	$Id: Cursor.cs 222 2011-06-23 07:17:01Z catwalkagogo@gmail.com $
*/
using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Threading;

namespace CatWalk.Net.Twitter{
	public struct Cursor<T>{
		public long Previous{get; private set;}
		public long Next{get; private set;}
		private Func<long, CancellationToken, CursorResult<T>> _Getter;

		public Cursor(long previous, long next, Func<long, CursorResult<T>> func) : this(){
			this.Previous = previous;
			this.Next = next;
			this._Getter = (cursor, token) => func(cursor);
		}

		public Cursor(long previous, long next, Func<long, CancellationToken, CursorResult<T>> func) : this(){
			this.Previous = previous;
			this.Next = next;
			this._Getter = func;
		}

		public Cursor(XElement xml, Func<long, CursorResult<T>> func) : this(){
			this.Previous = (long)xml.Element("previours_cursor");
			this.Next = (long)xml.Element("next_cursor");
			this._Getter = (cursor, token) => func(cursor);
		}

		public Cursor(XElement xml, Func<long, CancellationToken, CursorResult<T>> func) : this(){
			this.Previous = (long)xml.Element("previours_cursor");
			this.Next = (long)xml.Element("next_cursor");
			this._Getter = func;
		}

		public CursorResult<T> GetNext(){
			return this._Getter(this.Next, CancellationToken.None);
		}

		public CursorResult<T> GetNext(CancellationToken token){
			return this._Getter(this.Next, token);
		}

		public CursorResult<T> GetPrevious(){
			return this._Getter(this.Previous, CancellationToken.None);
		}

		public CursorResult<T> GetPrevious(CancellationToken token){
			return this._Getter(this.Previous, token);
		}
	}

	public struct CursorResult<T>{
		public IEnumerable<T> Result{get; private set;}
		public Cursor<T> Cursor{get; private set;}

		public CursorResult(IEnumerable<T> result, Cursor<T> cursor) : this(){
			this.Result = result;
			this.Cursor = cursor;
		}
	}
}