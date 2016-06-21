using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections;
using System.Windows;
using System.Windows.Input;
using CatWalk.Collections;

namespace CatWalk.Windows{
	public partial class CollectionEditDialog : Window{
		private WrappedObservableList<object> collection = new WrappedObservableList<object>(new SkipList<object>());
		
		public CollectionEditDialog(){
			this.InitializeComponent();
			this.listBox.ItemsSource = this.collection;
			if(this.listBox.HasItems){
				this.listBox.SelectedIndex = 0;
			}
		}
		
		private void SwapItem(int x, int y){
			var temp = this.collection[x];
			this.collection[x] = this.collection[y];
			this.collection[y] = temp;
		}
		
		#region コマンド
		
		private void Cancel_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}
		
		private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e){
			this.DialogResult = false;
			this.Close();
		}
		
		private void OK_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = true;
		}
		
		private void OK_Executed(object sender, ExecutedRoutedEventArgs e){
			if(this.Collection != null){
				this.Collection.Clear();
				foreach(var item in this.collection){
					this.Collection.Add(item);
				}
			}
			this.DialogResult = true;
			this.Close();
		}
		
		private void MoveItemUp_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = (this.listBox.SelectedIndex > 0);
		}
		
		private void MoveItemUp_Executed(object sender, ExecutedRoutedEventArgs e){
			var idx = this.listBox.SelectedIndex;
			var idx2 = idx - 1;
			if(idx2 >= 0){
				this.SwapItem(idx, idx2);
				this.listBox.SelectedIndex = idx2;
			}
		}
		
		private void MoveItemDown_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = (0 <= this.listBox.SelectedIndex) && (this.listBox.SelectedIndex < (this.listBox.Items.Count - 1));
		}
		
		private void MoveItemDown_Executed(object sender, ExecutedRoutedEventArgs e){
			var idx = this.listBox.SelectedIndex;
			var idx2 = idx + 1;
			if(idx2 < this.listBox.Items.Count){
				this.SwapItem(idx, idx2);
				this.listBox.SelectedIndex = idx2;
			}
		}
		
		private void RemoveItem_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = (0 <= this.listBox.SelectedIndex);
		}
		
		private void RemoveItem_Executed(object sender, ExecutedRoutedEventArgs e){
			var idx = this.listBox.SelectedIndex;
			this.collection.RemoveAt(idx);
			var max = (this.collection.Count - 1);
			this.listBox.SelectedIndex = (idx > max) ? max : idx;
		}
		
		private void AddItem_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = (this.AddItem != null) && (this.Collection != null);
		}
		
		private void AddItem_Executed(object sender, ExecutedRoutedEventArgs e){
			var e2 = new ItemEventArgs();
			this.AddItem(this, e2);
			if(!e2.Cancel){
				this.collection.Insert(this.listBox.SelectedIndex + 1, e2.Item);
				if(this.listBox.SelectedIndex < 0){
					this.listBox.SelectedIndex = 0;
				}
			}
		}
		
		private void EditItem_CanExecute(object sender, CanExecuteRoutedEventArgs e){
			e.CanExecute = (this.EditItem != null) && (this.listBox.SelectedIndex >= 0);
		}
		
		private void EditItem_Executed(object sender, ExecutedRoutedEventArgs e){
			int idx = this.listBox.SelectedIndex;
			var e2 = new ItemEventArgs(this.collection[idx]);
			this.EditItem(this, e2);
			if(!e2.Cancel){
				this.collection[idx] = e2.Item;
				this.listBox.SelectedIndex = idx;
			}
		}
		
		#endregion
		
		#region プロパティ
		
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(string), typeof(CollectionEditDialog), new PropertyMetadata(""));
		public string Message{
			get{
				return (string)this.GetValue(MessageProperty);
			}
			set{
				this.SetValue(MessageProperty, value);
			}
		}
		
		public static readonly DependencyProperty CollectionProperty = DependencyProperty.Register("Collection", typeof(IList), typeof(CollectionEditDialog),
			new PropertyMetadata(null, CollectionPropertyChanged));
		public IList Collection{
			get{
				return (IList)this.GetValue(CollectionProperty);
			}
			set{
				this.SetValue(CollectionProperty, value);
				this.collection.Clear();
				if(value != null){
					foreach(var item in value){
						this.collection.Add(item);
					}
				}
			}
		}
		
		public static void CollectionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e){
			var self = (CollectionEditDialog)sender;
			if(self.listBox.HasItems){
				self.listBox.SelectedIndex = 0;
			}
		}

		public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(CollectionEditDialog));
		public DataTemplate ItemTemplate{
			get{
				return (DataTemplate)this.GetValue(ItemTemplateProperty);
			}
			set{
				this.SetValue(ItemTemplateProperty, value);
			}
		}
		
		#endregion
		
		#region イベント
		
		public event ItemEventHandler AddItem;
		
		public event ItemEventHandler EditItem;
		
		#endregion
	}
	
	public delegate void ItemEventHandler(object sender, ItemEventArgs e);
	
	public class ItemEventArgs : CancelEventArgs{
		public object Item{get; set;}
		
		public ItemEventArgs(){
		}
		
		public ItemEventArgs(object item){
			this.Item = item;
		}
	}
	
	public static class CollectionEditCommands{
		public static readonly RoutedUICommand AddItem = new RoutedUICommand();
		public static readonly RoutedUICommand EditItem = new RoutedUICommand();
		public static readonly RoutedUICommand MoveItemUp = new RoutedUICommand();
		public static readonly RoutedUICommand MoveItemDown = new RoutedUICommand();
		public static readonly RoutedUICommand RemoveItem = new RoutedUICommand();
	}
}