using System;
using System.Runtime.InteropServices;

namespace Rust
{
	// Token: 0x02000B03 RID: 2819
	public static class Numlock
	{
		// Token: 0x04003CFF RID: 15615
		private const byte VK_NUMLOCK = 144;

		// Token: 0x04003D00 RID: 15616
		private const uint KEYEVENTF_EXTENDEDKEY = 1U;

		// Token: 0x04003D01 RID: 15617
		private const int KEYEVENTF_KEYUP = 2;

		// Token: 0x04003D02 RID: 15618
		private const int KEYEVENTF_KEYDOWN = 0;

		// Token: 0x060044E1 RID: 17633
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		private static extern short GetKeyState(int keyCode);

		// Token: 0x060044E2 RID: 17634
		[DllImport("user32.dll")]
		private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

		// Token: 0x17000638 RID: 1592
		// (get) Token: 0x060044E3 RID: 17635 RVA: 0x00194B27 File Offset: 0x00192D27
		public static bool IsOn
		{
			get
			{
				return ((ushort)Numlock.GetKeyState(144) & ushort.MaxValue) > 0;
			}
		}

		// Token: 0x060044E4 RID: 17636 RVA: 0x00194B3D File Offset: 0x00192D3D
		public static void TurnOn()
		{
			if (!Numlock.IsOn)
			{
				Numlock.keybd_event(144, 69, 1U, 0);
				Numlock.keybd_event(144, 69, 3U, 0);
			}
		}
	}
}
