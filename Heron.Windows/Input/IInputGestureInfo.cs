using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CatWalk.Heron.Windows.Input {
	public interface IInputGestureInfo {
		InputGesture Gesture { get; }
		InputBinding GetBinding(ICommand command);
	}
}
