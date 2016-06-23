using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CatWalk.Heron {
	public static class WindowMessages {

		public abstract class RestoreBoundsMessage : MessageBase{
			public Rect Bounds { get; set; }

			public RestoreBoundsMessage(object sender) : base(sender) { }
		}

		public class RequestRestoreBoundsMessage : RestoreBoundsMessage {
			public RequestRestoreBoundsMessage(object sender) : base(sender) { }
		}

		public class SetRestoreBoundsMessage : RestoreBoundsMessage {

			public SetRestoreBoundsMessage(object sender, Rect rect)
				: base(sender) {
				this.Bounds = rect;
			}
		}

		public class ClosingMessage : Messages.CancelMessage {
			public ClosingMessage(object sender) : base(sender) { }
		}

		public class CloseMessage : MessageBase {
			public CloseMessage(object sender) : base(sender){}
		}
		
		public class MessageBoxMessage : MessageBase {
			public MessageBoxResult Result { get; set; }
			public string Title { get; set; }
			public string Message { get; set; }
			public MessageBoxImage Image { get; set; }
			public MessageBoxButton Button{get;set;}
			public MessageBoxResult Default { get; set; }
			public MessageBoxOptions Options { get; set; }

			public MessageBoxMessage(object sender) : base(sender){
			}
		}
		
		public abstract class DialogResultMessage : MessageBase {
			public Nullable<bool> DialogResult { get; set; }

			public DialogResultMessage(object sender) : base(sender) {
			}
		}

		public class RequestDialogResultMessage : DialogResultMessage {
			public RequestDialogResultMessage(object sender)
				: base(sender) {
			}
		}

		public class SetDialogResultMessage : DialogResultMessage {
			public SetDialogResultMessage(object sender, Nullable<bool> result)
				: base(sender) {
					this.DialogResult = result;
			}
		}

		public class ArrangeWindowsMessage : MessageBase {
			public ArrangeMode Mode {
				get;
				private set;
			}

			public ArrangeWindowsMessage(object sender, ArrangeMode mode)
				: base(sender) {
				this.Mode = mode;
			}
		}

		public class ActivatedMessage : MessageBase{
			public ActivatedMessage(object sender) : base(sender){}
		}

		public class DeactivatedMessage : MessageBase {
			public DeactivatedMessage(object sender) : base(sender) {
			}
		}

		public class SetIsActiveMessage : MessageBase {
			public bool IsActive {
				get;
				private set;
			}

			public SetIsActiveMessage(object sender, bool active)
				: base(sender) {
					this.IsActive = active;
			}
		}

		public class RequestIsActiveMessage : MessageBase {
			public bool IsActive {
				get;
				set;
			}

			public RequestIsActiveMessage(object sender)
				: base(sender) {
			}
		}

		/// <summary>
		/// 文字列入力を要求する
		/// </summary>
		public class PromptMessage : Messages.CancelMessage {
			public string Message { get; set; }

			public PromptMessage(object sender) : base(sender) { }
		}

		public class RequestHandleMessage : MessageBase {
			public IntPtr Handle { get; set; }

			public RequestHandleMessage(object sender) : base(sender) { }
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
