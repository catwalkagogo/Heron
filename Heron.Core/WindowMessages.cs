using CatWalk.Heron.ViewModel.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CatWalk.Heron {
	public static class WindowMessages {
		/*
		public abstract class RestoreBoundsMessage : MessageBase{
			public Rect<double> Bounds { get; set; }

			public RestoreBoundsMessage() { }
		}

		public class RequestRestoreBoundsMessage : RestoreBoundsMessage {
			public RequestRestoreBoundsMessage() { }
		}

		public class SetRestoreBoundsMessage : RestoreBoundsMessage {

			public SetRestoreBoundsMessage(Rect<double> rect)
				{
				this.Bounds = rect;
			}
		}
		*/
		public class ClosingMessage : Messages.CancelMessage {
			public ClosingMessage() { }
		}

		public class CloseMessage : MessageBase {
			public CloseMessage(){}
		}
		
		public class MessageBoxMessage : MessageBase {
			public bool? Result { get; set; }
			public string Title { get; set; }
			public string Message { get; set; }
			/*public MessageBoxImage Image { get; set; }
			public MessageBoxButton Button{get;set;}
			public MessageBoxResult Default { get; set; }
			public MessageBoxOptions Options { get; set; }*/

			public MessageBoxMessage(){
			}
		}
		/*
		public abstract class DialogResultMessage : MessageBase {
			public Nullable<bool> DialogResult { get; set; }

			public DialogResultMessage() {
			}
		}
		
		public class RequestDialogResultMessage : DialogResultMessage {
			public RequestDialogResultMessage()
				{
			}
		}

		public class SetDialogResultMessage : DialogResultMessage {
			public SetDialogResultMessage(Nullable<bool> result)
				{
					this.DialogResult = result;
			}
		}
		*/
		public class ArrangeWindowsMessage : MessageBase {
			public ArrangeMode Mode {
				get;
				private set;
			}

			public ArrangeWindowsMessage(ArrangeMode mode)
				{
				this.Mode = mode;
			}
		}
		/*
		public class ActivatedMessage : MessageBase{
			public ActivatedMessage(){}
		}

		public class DeactivatedMessage : MessageBase {
			public DeactivatedMessage() {
			}
		}
		
		public class SetIsActiveMessage : MessageBase {
			public bool IsActive {
				get;
				private set;
			}

			public SetIsActiveMessage(bool active)
				{
					this.IsActive = active;
			}
		}

		public class RequestIsActiveMessage : MessageBase {
			public bool IsActive {
				get;
				set;
			}

			public RequestIsActiveMessage()
				{
			}
		}
		*/
		/// <summary>
		/// 文字列入力を要求する
		/// </summary>
		public class PromptMessage : Messages.CancelMessage {
			public string Message { get; set; }

			public PromptMessage() { }
		}

		public class RequestHandleMessage : MessageBase {
			public IntPtr Handle { get; set; }

			public RequestHandleMessage() { }
		}

		public class RequestMainWindow : MessageBase {
			public MainWindowViewModel MainWindow { get; set; }
		}
	}

	public enum ArrangeMode {
		Cascade,
		TileHorizontal,
		TileVertical,
		StackHorizontal,
		StackVertical,
	}
	
}
