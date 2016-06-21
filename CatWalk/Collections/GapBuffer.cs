/*
	$Id: GapBuffer.cs 17 2010-02-13 06:06:34Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace CatWalk.Collections{
	public class GapBuffer<T> : IList<T>{
		#region フィールド
		
		T[] buffer;
		int gapIndex;
		int gapLength;
		
		#endregion 
		
		#region コンストラクタ
		
		public GapBuffer() : this(0){
		}
		
		public GapBuffer(int capacity){
			this.InitializeArray(capacity);
		}
		
		#endregion
		
		#region ロジック
		
		private void InitializeArray(int capacity){
			this.buffer = new T[capacity];
			this.gapIndex = 0;
			this.gapLength = this.buffer.Length;
		}
		
		private void Increase(){
			if(this.buffer.Length == 0){
				this.Resize(4);
			}else{
				this.Resize(this.buffer.Length * 2);
			}
		}
		
		private void Resize(int capacity){
			T[] oldBuffer = this.buffer;
			int oldGapLength = this.gapLength;
			
			this.buffer = new T[capacity];
			this.gapLength += this.buffer.Length - oldBuffer.Length;
			Array.Copy(oldBuffer, 0, this.buffer, 0, this.gapIndex);
			Array.Copy(oldBuffer, this.gapIndex + oldGapLength, this.buffer, this.gapIndex + this.gapLength, oldBuffer.Length - this.gapIndex - oldGapLength);
		}
		
		/// <summary>
		/// Gapの位置移動
		/// </summary>
		private void SetGapIndexInternal(int index){
			if(index < this.gapIndex){
				int num = this.gapIndex - index;
				for(int i = 0; i < num; i++){
					this.gapIndex--;
					this.Swap(this.gapIndex, this.gapIndex + this.gapLength);
				}
			}else if(this.gapIndex < index){
				int num = index - this.gapIndex;
				for(int i = 0; i < num; i++){
					this.Swap(this.gapIndex, this.gapIndex + this.gapLength);
					this.gapIndex++;
				}
			}
		}
		
		private void Swap(int x, int y){
			T temp = this.buffer[x];
			this.buffer[x] = this.buffer[y];
			this.buffer[y] = temp;
		}
		
		/// <summary>
		/// indexから対応するバッファ上の位置を得る。
		/// </summary>
		private int GetBufferIndex(int index){
			return (index < this.gapIndex) ? index : (index + this.gapLength);
		}
		
		private void CheckIndex(int index){
			if((index < 0) || ((this.buffer.Length - this.gapLength) <= index)){
				throw new ArgumentOutOfRangeException();
			}
		}
		
		#endregion
		
		#region 関数
		
		public void Add(T item){
			this.Insert(this.Count, item);
		}
		
		public void AddRange(IEnumerable<T> col){
			// gap足りないとき増やす
			if(this.gapLength == 0){
				this.Increase();
			}
			// gap移動
			this.SetGapIndexInternal(this.Count);
			foreach(T item in col){
				if(this.gapLength == 0){
					this.Increase();
				}
				this.buffer[this.gapIndex] = item;
				this.gapIndex++;
				this.gapLength--;
			}
		}
		
		public void Insert(int index, T item){
			if((index < 0) || ((this.buffer.Length - this.gapLength) < index)){
				throw new ArgumentOutOfRangeException();
			}
			
			// gap足りないとき増やす
			if(this.gapLength == 0){
				this.Increase();
			}
			// gap移動
			this.SetGapIndexInternal(index);
			this.buffer[this.gapIndex] = item;
			this.gapIndex++;
			this.gapLength--;
		}
		
		public void RemoveAt(int index){
			this.CheckIndex(index);
			if(this.gapLength == 0){
				this.Increase();
			}
			int bufIndex = this.GetBufferIndex(index);
			if((this.gapIndex + this.gapLength) == bufIndex){
				this.buffer[bufIndex] = default(T);
				this.gapLength++;
			}else{
				this.SetGapIndexInternal(index);
				this.buffer[bufIndex] = default(T);
				this.gapLength++;
			}
		}
		
		public void Clear(){
			this.gapIndex = 0;
			this.gapLength = this.buffer.Length;
			for(int i = 0; i < this.buffer.Length; i++){
				this.buffer[i] = default(T);
			}
		}
		
		public bool Contains(T item){
			foreach(T item2 in this){
				if(item.Equals(item2)){
					return true;
				}
			}
			return false;
		}
		
		public int IndexOf(T item){
			int i = 0;
			foreach(T item2 in this){
				if(item.Equals(item2)){
					return i;
				}
				i++;
			}
			return -1;
		}
		
		public bool Remove(T item){
			int idx = this.IndexOf(item);
			if(idx != -1){
				this.RemoveAt(idx);
				return true;
			}else{
				return false;
			}
		}
		
		public void CopyTo(T[] array, int arrayIndex){
			if(array == null){
				throw new ArgumentNullException();
			}
			if(arrayIndex < 0){
				throw new ArgumentOutOfRangeException();
			}
			if((array.Rank > 1) || (array.Length <= arrayIndex) || (this.Count > (array.Length - arrayIndex))){
				throw new ArgumentException();
			}
			int i = arrayIndex;
			foreach(T item in this){
				array[i] = item;
				i++;
			}
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return this.GetEnumerator();
		}
		
		public IEnumerator<T> GetEnumerator(){
			int count = this.Count;
			for(int i = 0; i < count; i++){
				yield return this.buffer[this.GetBufferIndex(i)];
			}
		}
		
		public T[] ToArray(){
			if(this.gapLength == 0){
				this.Increase();
			}
			this.SetGapIndexInternal(this.Count);
			
			T[] array = new T[this.Count];
			Array.Copy(this.buffer, 0, array, 0, this.Count);
			return array;
		}
		
		public void TrimExcess(){
			if(this.gapLength == 0){
				this.Increase();
			}
			this.SetGapIndexInternal(this.Count);
			
			Array.Resize<T>(ref this.buffer, this.Count);
		}
		
		#endregion
		
		#region プロパティ
		
		public int Count{
			get{
				return this.buffer.Length - this.gapLength;
			}
		}
		
		public int Capacity{
			get{
				return this.buffer.Length;
			}
		}
		
		public T this[int index]{
			get{
				this.CheckIndex(index);
				return this.buffer[this.GetBufferIndex(index)];
			}
			set{
				this.CheckIndex(index);
				this.buffer[this.GetBufferIndex(index)] = value;
			}
		}
		
		public bool IsReadOnly{
			get{
				return false;
			}
		}
		
		public int GapIndex{
			get{
				return this.gapIndex;
			}
			set{
				if((value < 0) || ((this.buffer.Length - this.gapLength) < value)){
					throw new ArgumentOutOfRangeException();
				}
				if(this.gapLength == 0){
					this.Increase();
				}
				this.SetGapIndexInternal(value);
			}
		}
		
		public int GapLength{
			get{
				return this.gapLength;
			}
		}
		
		#endregion
	}
}