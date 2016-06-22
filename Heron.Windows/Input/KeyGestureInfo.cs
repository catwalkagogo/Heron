using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;


namespace CatWalk.Heron.Windows.Input {
	[JsonObject("KeyGestureInfo")]
	public class KeyGestureInfo : IInputGestureInfo {
		[JsonProperty("Key")]
		[JsonConverter(typeof(StringEnumConverter))]
		public Key Key { get; private set; }

		[JsonProperty("Modifiers")]
		[DefaultValue(ModifierKeys.None)]
		[JsonConverter(typeof(StringEnumConverter))]
		public ModifierKeys Modifiers { get; private set; }

		public KeyGestureInfo() { }
		public KeyGestureInfo(Key key) : this(key, ModifierKeys.None) { }

		[JsonConstructor]
		public KeyGestureInfo(Key key, ModifierKeys modifiers) {
			this.Key = key;
			this.Modifiers = modifiers;
		}

		public InputGesture Gesture {
			get {
				return new KeyGesture(this.Key, this.Modifiers);
			}
		}

		public InputBinding GetBinding(ICommand command) {
			return new KeyBinding(command, this.Key, this.Modifiers);
		}

	}
}
