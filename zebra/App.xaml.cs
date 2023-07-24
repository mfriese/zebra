using GlobalLowLevelHooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HotCorner
{
	public partial class App : Application
	{
		protected static List<TimedInput> releasedKeys = new List<TimedInput>();

		protected static KeyboardHook keyHook = null;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			keyHook = new KeyboardHook();

			keyHook.KeyUp += KeyHook_KeyUp;

			keyHook.Install();
		}

		private async void KeyHook_KeyUp(KeyboardHook.VKeys key)
		{
			releasedKeys.Add(new TimedInput()
			{
				Key = key,
				Time = DateTime.Now
			});

			if (releasedKeys.
				Where(rr => rr.Time.AddSeconds(1) > DateTime.Now).
				Count(rr => rr.Key == KeyboardHook.VKeys.ESCAPE) >= 3)
			{
				releasedKeys.Clear();

				INPUT[] sequence = {
					User32Util.MakeInput(KeyCode.ALT, false),
					User32Util.MakeInput(KeyCode.F4, false),
					User32Util.MakeInput(KeyCode.F4, true),
					User32Util.MakeInput(KeyCode.ALT, true)
				};

				User32Util.SendInputSequence(sequence);
				return;
			}

			while (releasedKeys.FirstOrDefault().Time.AddSeconds(5) < DateTime.Now)
			{
				releasedKeys.RemoveAt(0);
			}
			
			await Task.CompletedTask;
		}

		public struct TimedInput
		{
			public DateTime Time
			{
				get; set;
			}
			public KeyboardHook.VKeys Key
			{
				get; set;
			}
		}

		//public void ShowDesktop()
		//{
		//    INPUT[] sequence = {
		//        User32Util.MakeInput(KeyCode.LWIN, false),
		//        User32Util.MakeInput(KeyCode.KEY_D, false),
		//        User32Util.MakeInput(KeyCode.KEY_D, true),
		//        User32Util.MakeInput(KeyCode.LWIN, true)
		//    };

		//    User32Util.SendInputSequence(sequence);
		//}

		//private void ArrangeWindows()
		//{
		//    INPUT[] sequence = {
		//        User32Util.MakeInput(KeyCode.LWIN, false),
		//        User32Util.MakeInput(KeyCode.TAB, false),
		//        User32Util.MakeInput(KeyCode.TAB, true),
		//        User32Util.MakeInput(KeyCode.LWIN, true)
		//    };

		//    User32Util.SendInputSequence(sequence);
		//}
	}
}
