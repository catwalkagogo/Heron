using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk.Heron.ViewModel {
	public class OutputMessage : ViewModelBase{
		private string _Message;
		public string Message {
			get {
				return this._Message;
			}
			set {
				this._Message = value;
				this.OnPropertyChanged(nameof(Message));
			}
		}

		private MessagePriority _Priority;
		public MessagePriority Priority {
			get {
				return this._Priority;
			}
			set {
				this._Priority = value;
				this.OnPropertyChanged(nameof(Priority));
			}
		}

		public OutputMessage() { }
		public OutputMessage(string message, MessagePriority priority) {
			this.Message = message;
			this.Priority = priority;
		}
	}

	public enum MessagePriority {
		Debug = 0,
		Info = 1,
		Success = 2,
		Warning = 3,
		Error = 4,
	}
}
