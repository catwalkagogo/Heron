using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;

namespace CatWalk.Windows.Input {
	[TypeConverter(typeof(MultiKeyGestureConverter))]
	public class MultiKeyGesture : KeyGesture {
		private readonly IReadOnlyList<Key> _Keys;
		private int _CurrentKeyIndex;
		private bool _IsWaitingNextKey = false;
		// private DateTime _lastKeyPress;
		// private static readonly TimeSpan _maximumDelayBetweenKeyPresses = TimeSpan.FromSeconds(1);

		public MultiKeyGesture(IEnumerable<Key> keys, ModifierKeys modifiers)
			: this(keys, modifiers, string.Empty) {
		}

		public MultiKeyGesture(IEnumerable<Key> keys, ModifierKeys modifiers, string displayString)
			: base(Key.None, modifiers, displayString) {
			keys.ThrowIfNull("keys");
			this._Keys = Array.AsReadOnly(keys.ToArray());


			if(_Keys.Count == 0) {
				throw new ArgumentException("At least one key must be specified.", "keys");
			}
		}

		public IReadOnlyList<Key> Keys {
			get {
				return _Keys;
			}
		}

		public override bool Matches(object targetElement, InputEventArgs inputEventArgs) {
			this._IsWaitingNextKey = false;
			var args = inputEventArgs as KeyEventArgs;

			if((args == null) || !IsDefinedKey(args.Key)) {
				return false;
			}
			/*
			 * Timeout
			if(_currentKeyIndex != 0 && ((DateTime.Now - _lastKeyPress) > _maximumDelayBetweenKeyPresses)) {
				//took too long to press next key so reset
				_currentKeyIndex = 0;
				return false;
			}
			*/
			//the modifier only needs to be held down for the first keystroke, but you could also require that the modifier be held down for every keystroke
			if(_CurrentKeyIndex == 0 && Modifiers != Keyboard.Modifiers) {
				//wrong modifiers
				_CurrentKeyIndex = 0;
				return false;
			}

			if(_Keys[_CurrentKeyIndex] != args.Key) {
				//wrong key
				_CurrentKeyIndex = 0;
				return false;
			}

			++_CurrentKeyIndex;

			if(_CurrentKeyIndex != _Keys.Count) {
				//still matching
				//_lastKeyPress = DateTime.Now;
				inputEventArgs.Handled = true;
				this._IsWaitingNextKey = true;
				return false;
			}

			//match complete
			_CurrentKeyIndex = 0;
			return true;
		}

		private static bool IsDefinedKey(Key key) {
			return ((key >= Key.None) && (key <= Key.OemClear));
		}

		public bool IsWaitingNextKey {
			get {
				return this._IsWaitingNextKey;
			}
		}
	}
}