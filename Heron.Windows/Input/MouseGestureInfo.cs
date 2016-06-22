using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Windows.Input;

namespace CatWalk.Heron.Windows.Input {
	public class MouseGestureInfo : IInputGestureInfo {
		[JsonProperty("MouseAction")]
		[JsonConverter(typeof(StringEnumConverter))]
		public MouseAction MouseAction { get; private set; }
		[JsonProperty("Modifiers")]
		[JsonConverter(typeof(StringEnumConverter))]
		public ModifierKeys Modifiers { get; private set; }

		public MouseGestureInfo() { }
		public MouseGestureInfo(MouseAction action) : this(action, ModifierKeys.None) { }

		[JsonConstructor]
		public MouseGestureInfo(MouseAction action, ModifierKeys modifiers) {
			this.MouseAction = action;
			this.Modifiers = modifiers;
		}

		public InputGesture Gesture {
			get {
				return new MouseGesture(this.MouseAction, this.Modifiers);
			}
		}

		public InputBinding GetBinding(ICommand command) {
			return new MouseBinding(command, new MouseGesture(this.MouseAction, this.Modifiers));
		}

	}
}
