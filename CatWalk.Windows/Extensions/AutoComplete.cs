/*
	$Id: AutoComplete.cs 137 2010-12-19 10:01:30Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using CatWalk.Collections;

namespace CatWalk.Windows.Extensions{
	public static class AutoComplete{
		#region 添付プロパティ
		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(AutoComplete), new UIPropertyMetadata(false, IsEnabledChanged));
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static bool GetIsEnabled(DependencyObject obj){
			return (bool)obj.GetValue(IsEnabledProperty);
		}
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static void SetIsEnabled(DependencyObject obj, bool value){
			obj.SetValue(IsEnabledProperty, value);
		}
		
		public static readonly DependencyProperty CandidatesListBoxProperty =
			DependencyProperty.RegisterAttached("CandidatesListBox", typeof(ListBox), typeof(AutoComplete));
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static ListBox GetCandidatesListBox(DependencyObject obj){
			return (ListBox)obj.GetValue(CandidatesListBoxProperty);
		}
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static void SetCandidatesListBox(DependencyObject obj, ListBox value){
			obj.SetValue(CandidatesListBoxProperty, value);
		}
		
		public static readonly DependencyProperty PopupProperty =
			DependencyProperty.RegisterAttached("Popup", typeof(Popup), typeof(AutoComplete));
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static Popup GetPopup(DependencyObject obj){
			return (Popup)obj.GetValue(PopupProperty);
		}
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static void SetPopup(DependencyObject obj, Popup value){
			obj.SetValue(PopupProperty, value);
		}
		
		public static readonly DependencyProperty PopupOffsetProperty =
			DependencyProperty.RegisterAttached("PopupOffset", typeof(Vector), typeof(AutoComplete));
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static Vector GetPopupOffset(DependencyObject obj){
			return (Vector)obj.GetValue(PopupOffsetProperty);
		}
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static void SetPopupOffset(DependencyObject obj, Vector value){
			obj.SetValue(PopupOffsetProperty, value);
		}
		
		public static readonly DependencyProperty IsInsertAutomaticallyProperty =
			DependencyProperty.RegisterAttached("IsInsertAutomatically", typeof(bool), typeof(AutoComplete), new PropertyMetadata(true));
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static bool GetIsInsertAutomatically(DependencyObject obj){
			return (bool)obj.GetValue(IsInsertAutomaticallyProperty);
		}
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static void SetIsInsertAutomatically(DependencyObject obj, bool value){
			obj.SetValue(IsInsertAutomaticallyProperty, value);
		}
		
		public static readonly DependencyProperty CandidatesProperty =
			DependencyProperty.RegisterAttached("Candidates", typeof(IDictionary<string, KeyValuePair<string, object>[]>), typeof(AutoComplete));
		
		public static IDictionary<string, KeyValuePair<string, object>[]> GetCandidates(DependencyObject obj){
			return (IDictionary<string, KeyValuePair<string, object>[]>)obj.GetValue(CandidatesProperty);
		}
		
		public static readonly DependencyProperty TokenPatternProperty =
			DependencyProperty.RegisterAttached("TokenPattern", typeof(string), typeof(AutoComplete));
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static string GetTokenPattern(DependencyObject obj){
			return (string)obj.GetValue(TokenPatternProperty);
		}
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static void SetTokenPattern(DependencyObject obj, string value){
			obj.SetValue(TokenPatternProperty, value);
		}
		
		public static readonly DependencyProperty StringComparisonProperty =
			DependencyProperty.RegisterAttached("StringComparison", typeof(StringComparison), typeof(AutoComplete), new PropertyMetadata(StringComparison.OrdinalIgnoreCase));
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static StringComparison GetStringComparison(DependencyObject obj){
			return (StringComparison)obj.GetValue(StringComparisonProperty);
		}
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static void SetStringComparison(DependencyObject obj, StringComparison value){
			obj.SetValue(StringComparisonProperty, value);
		}

		private static readonly DependencyProperty AutoCompleteStateProperty =
			DependencyProperty.RegisterAttached("States", typeof(AutoCompleteState), typeof(AutoComplete));
		
		private static AutoCompleteState GetState(DependencyObject obj){
			return (AutoCompleteState)obj.GetValue(AutoCompleteStateProperty);
		}
		
		private static void SetState(DependencyObject obj, AutoCompleteState value){
			obj.SetValue(AutoCompleteStateProperty, value);
		}
		
		private class AutoCompleteState{
			public string ProcessingWord{get; set;}
			public TextBox TextBox{get; set;}
		}
		
		public static readonly DependencyProperty ReplaceWordTypesProperty =
			DependencyProperty.RegisterAttached("ReplaceWordTypes", typeof(Type[]), typeof(AutoComplete));
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static Type[] GetReplaceWordTypes(DependencyObject obj){
			return (Type[])obj.GetValue(ReplaceWordTypesProperty);
		}
		
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static void SetReplaceWordTypes(DependencyObject obj, Type[] value){
			obj.SetValue(ReplaceWordTypesProperty, value);
		}
		
		public static readonly DependencyProperty QueryCandidatesProperty =
			DependencyProperty.RegisterAttached("QueryCandidates", typeof(QueryCandidatesEventHandler), typeof(AutoComplete));
		public static void AddQueryCandidates(DependencyObject obj, QueryCandidatesEventHandler handler){
			var ev = (QueryCandidatesEventHandler)obj.GetValue(QueryCandidatesProperty);
			if(ev != null){
				ev += handler;
			}else{
				ev = handler;
			}
			obj.SetValue(QueryCandidatesProperty, ev);
		}
		
		public static void RemoveQueryCandidates(DependencyObject obj, QueryCandidatesEventHandler handler){
			var ev = (QueryCandidatesEventHandler)obj.GetValue(QueryCandidatesProperty);
			if(ev != null){
				ev -= handler;
			}
			obj.SetValue(QueryCandidatesProperty, ev);
		}
		
		#endregion
		
		#region イベント処理

		private static void IsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e){
			TextBox textBox = (TextBox)sender;
			
			bool newValue = (bool)e.NewValue;
			bool oldValue = (bool)e.OldValue;
			if(newValue != oldValue){
				if(oldValue){
					textBox.TextChanged -= TextBox_TextChanged;
					textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
					textBox.SetValue(CandidatesProperty, null);
					//textBox.LostFocus -= TextBox_LostFocus;
					SetState(textBox, null);
				}else{
					textBox.TextChanged += TextBox_TextChanged;
					textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
					textBox.SetValue(CandidatesProperty, new PrefixDictionary<KeyValuePair<string, object>[]>(CharIgnoreCaseComparer.Comparer));
					//textBox.LostFocus += TextBox_LostFocus;
					SetState(textBox, new AutoCompleteState());
					if(String.IsNullOrEmpty(GetTokenPattern(textBox))){
						SetTokenPattern(textBox, " ");
					}
				}
			}
		}
		
		private static void TextBox_TextChanged(object sender, TextChangedEventArgs e){
			var textBox = (TextBox)sender;
			var listBox = GetCandidatesListBox(textBox);
			if(listBox != null){
				//var dict = (PrefixDictionary<object>)GetCandidates(textBox);
				var text = textBox.Text;
				var popup = GetPopup(textBox);
				
				if(!String.IsNullOrEmpty(text)){
					var state = GetState(textBox);
					var tokenPattern = GetTokenPattern(textBox);
					var caretIndex = textBox.CaretIndex;
					var startIndex = GetStartIndex(text, caretIndex, tokenPattern);
					var queryWord = text.Substring(startIndex, caretIndex - startIndex);
					
					//MessageBox.Show("Count:" + e.Changes.Count + "\nAddedLength:" + e.Changes.First().AddedLength + "\nRemovedLength" + e.Changes.First().RemovedLength);
					if(
						//!String.IsNullOrEmpty(queryWord) &&
						//(queryWord != state.ProcessingWord) &&
						!(
							(e.Changes.Count == 1) &&
							(e.Changes.First().AddedLength == 0) &&
							(e.Changes.First().RemovedLength > 0)
						)
					){
						//MessageBox.Show(queryWord);
						RefreshListAsync(textBox, listBox, queryWord, delegate{
							if(popup != null){
								if(state.ProcessingWord == queryWord){
									if((listBox.Items.Count > 1) || 
										((listBox.Items.Count == 1) && 
											!queryWord.Equals(((KeyValuePair<string, object>)listBox.Items[0]).Key,
											StringComparison.OrdinalIgnoreCase))){
										OpenPopup(textBox, popup, startIndex);
									}else{
										popup.IsOpen = false;
										ClearListBox(listBox);
									}
								}
							}
						});
					}else{
						textBox.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate{
							if(state.ProcessingWord == text){
								if(popup != null){
									popup.IsOpen = false;
								}
								ClearListBox(listBox);
							}
						}));
					}
				}else{
					if(popup != null){
						popup.IsOpen = false;
					}
					ClearListBox(listBox);
				}
			}
		}
		
		private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e){
			var textBox = (TextBox)sender;
			var listBox = GetCandidatesListBox(textBox);
			var popup = GetPopup(textBox);
			if(popup.IsOpen){
				switch(e.Key){
					case Key.Up:{
						if(listBox.SelectedIndex > 0){
							listBox.SelectedIndex--;
							listBox.ScrollIntoView(listBox.SelectedItem);
							e.Handled = true;
						}
						break;
					}
					case Key.Down:{
						if(listBox.SelectedIndex < listBox.Items.Count){
							listBox.SelectedIndex++;
							listBox.ScrollIntoView(listBox.SelectedItem);
							e.Handled = true;
						}
						break;
					}
					case Key.Tab:{
						string text = textBox.Text;
						if(!String.IsNullOrEmpty(text) && (listBox.SelectedValue != null)){
							var item = (KeyValuePair<string, object>)listBox.SelectedValue;
							InsertWord(textBox, item, false);
							ClearListBox(listBox);
							if(popup != null){
								popup.IsOpen = false;
							}
							e.Handled = true;
						}
						break;
					}
					case Key.Left:
					case Key.Right:
					case Key.PageUp:
					case Key.PageDown:{
						ClearListBox(listBox);
						if(popup != null){
							popup.IsOpen = false;
						}
						break;
					}
					case Key.Enter:
					case Key.Escape:{
						popup.IsOpen = false;
						e.Handled = true;
						break;
					}
				}
			}else{
				switch(e.Key){
					case Key.Down:{
						if(!popup.IsOpen){
							var state = GetState(textBox);
							var text = textBox.Text;
							var tokenPattern = GetTokenPattern(textBox);
							var caretIndex = textBox.CaretIndex;
							var startIndex = GetStartIndex(text, caretIndex, tokenPattern);
							var queryWord = text.Substring(startIndex, caretIndex - startIndex);
							//MessageBox.Show(queryWord);
							RefreshListAsync(textBox, listBox, queryWord, delegate{
								if(popup != null){
									if(state.ProcessingWord == queryWord){
										if(listBox.Items.Count > 0){
											OpenPopup(textBox, popup, 0);
										}else{
											popup.IsOpen = false;
											ClearListBox(listBox);
										}
									}
								}
							});
							e.Handled = true;
						}
						break;
					}
				}
			}
		}

		private static void TextBox_LostFocus(object sender, EventArgs e){
			var textBox = (TextBox)sender;
			textBox.Dispatcher.BeginInvoke(new Action(delegate{
				//Thread.Sleep(1000);
				var popup = GetPopup(textBox);
				var listBox = GetCandidatesListBox(textBox);
				if((popup != null) && (listBox != null) && (!listBox.IsFocused || !popup.IsFocused)){
					popup.IsOpen = false;
				}
			}), DispatcherPriority.Background, null);
		}

		private static void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e){
			var listBox = (ListBox)sender;
			var state = GetState(listBox);
			var textBox = state.TextBox;
			if(listBox.SelectedValue != null){
				var item = (KeyValuePair<string, object>)listBox.SelectedValue;
				InsertWord(textBox, item, true);
			}
		}

		#endregion
		
		#region 関数
		
		private static void InsertWord(TextBox textBox, KeyValuePair<string, object> item, bool isIns){
			var text = textBox.Text;
			var word = item.Key;
			int caretIndex = textBox.CaretIndex;
			int startIndex = GetStartIndex(text, caretIndex, GetTokenPattern(textBox));
			string left = text.Substring(0, startIndex);
			string right = text.Substring(textBox.SelectionStart + textBox.SelectionLength);
			
			var types = GetReplaceWordTypes(textBox);
			// 候補ワードに置き換える
			if(!isIns && (types != null) && Array.IndexOf(types, item.Value.GetType()) != -1){
				textBox.TextChanged -= TextBox_TextChanged;
				textBox.Text = left + word + right;
				textBox.TextChanged += TextBox_TextChanged;
				if(isIns){
					textBox.SelectionStart = caretIndex;
					textBox.SelectionLength = word.Length;
				}
			}else{ // 挿入する。
				string inputed = text.Substring(startIndex, caretIndex - startIndex);
				if(word.Length > inputed.Length){
					string insert = word.Substring(inputed.Length);
					textBox.TextChanged -= TextBox_TextChanged;
					textBox.Text = left + inputed + insert + right;
					textBox.TextChanged += TextBox_TextChanged;
					if(isIns){
						textBox.SelectionStart = caretIndex;
						textBox.SelectionLength = insert.Length;
					}
				}
			}
			if(!isIns){
				int inputedLength = caretIndex - startIndex;
				textBox.CaretIndex = caretIndex + (word.Length - inputedLength);
			}
		}

		private static int GetStartIndex(string text, int index, string tokenPattern){
			if((index < 0) || (text.Length < index)){
				throw new ArgumentOutOfRangeException();
			}
			try{
				var regex = new Regex(tokenPattern, RegexOptions.RightToLeft);
				var match = regex.Match(text, index);
				if(match.Success){
					return match.Index + match.Length;
				}else{
					return 0;
				}
			}catch{
				return 0;
			}
		}
		
		public static void RefreshList(TextBox textBox){
			string text = textBox.Text;
			if(text == null){
				text = "";
			}
			int caretIndex = textBox.CaretIndex;
			int startIndex = GetStartIndex(text, caretIndex, GetTokenPattern(textBox));
			var listBox = GetCandidatesListBox(textBox);
			string word = text.Substring(startIndex, caretIndex);
			RefreshList(textBox, listBox, word);
		}
		
		private static void RefreshList(TextBox textBox, ListBox listBox, string word){
			ManualResetEvent wait = new ManualResetEvent(false);
			wait.Reset();
			RefreshListAsync(textBox, listBox, word, delegate{
				wait.Set();
			});
			wait.WaitOne();
		}
		
		private static void RefreshListAsync(TextBox textBox, ListBox listBox, string word, Action callback){
			var dict = (PrefixDictionary<KeyValuePair<string, object>[]>)textBox.GetValue(CandidatesProperty);
			var state = GetState(textBox);
			state.ProcessingWord = word;
			var comparison = GetStringComparison(textBox);
			ThreadPool.QueueUserWorkItem(new WaitCallback(delegate{
				RefreshListAsync(textBox, listBox, word, dict, callback, comparison);
			}));
		}
		
		private static void RefreshListAsync(TextBox textBox, ListBox listBox, string word, PrefixDictionary<KeyValuePair<string, object>[]> dict, Action callback, StringComparison comparison){
			var matches = dict.Search(word, false).Select(found => found.Value).SelectMany(found => found).ToList();
			listBox.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate{
				var ev = (QueryCandidatesEventHandler)textBox.GetValue(QueryCandidatesProperty);
				if(ev != null){
					var e = new QueryCandidatesEventArgs(word);
					foreach(var del in ev.GetInvocationList()){
						del.DynamicInvoke(new object[]{textBox, e});
						if(e.Candidates != null){
							matches.AddRange(e.Candidates);
						}
					}
				}
			}));
			listBox.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate{
				bool isIns = GetIsInsertAutomatically(textBox);
				listBox.ItemsSource = null;
				listBox.ItemsSource = matches.ToArray();
				SetState(listBox, new AutoCompleteState(){TextBox = textBox});
				listBox.SelectionChanged -= ListBox_SelectionChanged;
				if(isIns){
					listBox.SelectionChanged += ListBox_SelectionChanged;
				}
				if(listBox.Items.Count > 0){
					listBox.SelectedIndex = 0;
					listBox.ScrollIntoView(listBox.SelectedItem);
				}
				if(!isIns){
					listBox.SelectionChanged += ListBox_SelectionChanged;
				}
				if(callback != null){
					callback();
				}
			}));
		}
		
		private static void OpenPopup(TextBox textBox, Popup popup, int index){
			popup.PlacementTarget = textBox;
			var rect = textBox.GetRectFromCharacterIndex(Math.Max(0, index));
			rect.Offset(GetPopupOffset(textBox));
			popup.PlacementRectangle = rect;
			popup.IsOpen = true;
		}

		public static void AddCondidates(TextBox textBox, IDictionary<string, object> dict){
			var dict2 = AutoComplete.GetCandidates(textBox);
			foreach(var grp in dict.GroupBy(pair => pair.Key, pair => pair, StringComparer.OrdinalIgnoreCase)){
				dict2.Add(grp.Key, grp.ToArray());
			}
		}
		
		public static void AddCondidates(TextBox textBox, string[] words){
			var dict2 = AutoComplete.GetCandidates(textBox);
			foreach(var grp in words.GroupBy(word => word, word => word, StringComparer.OrdinalIgnoreCase)){
				dict2.Add(grp.Key, grp.Select(word => new KeyValuePair<string, object>(word, null)).ToArray());
			}
		}
		
		private static void ClearListBox(ListBox listBox){
			listBox.ItemsSource = null;
			listBox.SelectionChanged -= ListBox_SelectionChanged;
			SetState(listBox, null);
		}

		#endregion 
		
		#region ハンドラ
		
		private static QueryCandidatesEventHandler queryDirectoryCandidatesHandler = null;
		public static QueryCandidatesEventHandler QueryDirectoryCandidatesHandler{
			get{
				if(queryDirectoryCandidatesHandler == null){
					queryDirectoryCandidatesHandler = new QueryCandidatesEventHandler(QueryDirectoryCandidates);
				}
				return queryDirectoryCandidatesHandler;
			}
		}
		
		private static void QueryDirectoryCandidates(object sender, QueryCandidatesEventArgs e){
			string path = e.Query;
			if(!String.IsNullOrEmpty(path)){
				var idx = path.LastIndexOf(Path.DirectorySeparatorChar.ToString());
				if(idx > 0){
					var dir = path.Substring(0, idx + 1);
					var name = path.Substring(idx + 1);
					try{
						var dirs = Directory.GetDirectories(dir).Where(file => Path.GetFileName(file).StartsWith(name, StringComparison.OrdinalIgnoreCase));
						e.Candidates = dirs.Select(d => new KeyValuePair<string, object>(d, d)).ToArray();
					}catch{
						e.Candidates = new KeyValuePair<string, object>[0];
					}
				}
			}
		}
		
		public static QueryCandidatesEventHandler GetQueryFilesCandidatesHandler(string mask){
			return new QueryCandidatesEventHandler(delegate(object sender, QueryCandidatesEventArgs e){
				string path = e.Query;
				if(!String.IsNullOrEmpty(path)){
					var idx = path.LastIndexOf(Path.DirectorySeparatorChar.ToString());
					if(idx > 0){
						var dir = path.Substring(0, idx + 1);
						var name = path.Substring(idx + 1);
						try{
							var files = Directory.GetFiles(dir, mask).Where(file => Path.GetFileName(file).StartsWith(name, StringComparison.OrdinalIgnoreCase));;
							e.Candidates = files.Select(d => new KeyValuePair<string, object>(d, d)).ToArray();
						}catch{
							e.Candidates = new KeyValuePair<string, object>[0];
						}
					}
				}
			});
		}
		
		#endregion
	}
	
	public delegate void QueryCandidatesEventHandler(object sender, QueryCandidatesEventArgs e);
	
	public class QueryCandidatesEventArgs : EventArgs{
		public KeyValuePair<string, object>[] Candidates{get; set;}
		public string Query{get; private set;}
		
		public QueryCandidatesEventArgs(string query){
			this.Query = query;
		}
	}
}