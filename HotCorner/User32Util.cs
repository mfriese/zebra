using System;
using System.Runtime.InteropServices;

namespace HotCorner
{
    /// <summary>
    /// Contains interface to native system calls.
    /// </summary>
    public static class User32Util
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);

        /// <summary>
        /// Send a single key to the system. The key will be held down until sendkeyup method is
        /// called.
        /// </summary>
        /// <param name="keyCode">Representing the key to be held down.</param>
        /// <param name="release">true, if the key is released, else false.</param>
        internal static void SendInput(KeyCode keyCode, Boolean release)
        {
            INPUT[] inputs = new INPUT[] { MakeInput(keyCode, release) };
            User32Util.SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        /// <summary>
        /// Send a whole sequence of keys to the system.
        /// </summary>
        /// <param name="inputs">a sequence of key inputs.</param>
        internal static void SendInputSequence(params INPUT[] inputs)
        {
            User32Util.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        /// <summary>
        /// Create the input structure. Necessary to create either a single or a sequence of
        /// keystrokes.
        /// </summary>
        /// <param name="keyCode">Representing the key to be held down.</param>
        /// <param name="release">true, if the key is released, else false.</param>
        /// <returns>Native input structure wrapper.</returns>
        internal static INPUT MakeInput(KeyCode keyCode, Boolean release)
        {
            INPUT input = new INPUT
            {
                Type = 1
            };

            input.Data.Keyboard = new KEYBDINPUT
            {
                Vk = (ushort)keyCode,
                Scan = 0,
                Flags = (uint)(release ? 2 : 0),
                Time = 0,
                ExtraInfo = IntPtr.Zero
            };

            return input;
        }
    }
}
