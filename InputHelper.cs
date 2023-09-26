using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

namespace NoClip {
	internal class InputHelper {
		private static bool IsListening = false;

		internal static bool IsBusy() {
			return IsListening;
		}

		internal static Key? GetPressedKey() {
			if (IsListening) {
				if (Keyboard.current != null) {
					foreach (KeyControl Key in Keyboard.current.allKeys) {
						if (Key.isPressed) {
							return Key.keyCode;
						}
					}
				}
			}

			return null;
		}

		internal static GamepadButton? GetPressedButton() {
			if (IsListening) {
				foreach (Gamepad Gamepad in Gamepad.all) {
					foreach (GamepadButton Button in Enum.GetValues(typeof(GamepadButton)))
						if (Gamepad[Button].isPressed) {
							return Button;
						}
				}
			}

			return null;
		}
	}
}
