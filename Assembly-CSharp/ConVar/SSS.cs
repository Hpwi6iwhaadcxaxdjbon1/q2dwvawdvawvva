using System;

namespace ConVar
{
	// Token: 0x02000ADB RID: 2779
	[ConsoleSystem.Factory("SSS")]
	public class SSS : ConsoleSystem
	{
		// Token: 0x04003C23 RID: 15395
		[ClientVar(Saved = true)]
		public static bool enabled = true;

		// Token: 0x04003C24 RID: 15396
		[ClientVar(Saved = true)]
		public static int quality = 0;

		// Token: 0x04003C25 RID: 15397
		[ClientVar(Saved = true)]
		public static bool halfres = true;

		// Token: 0x04003C26 RID: 15398
		[ClientVar(Saved = true)]
		public static float scale = 1f;
	}
}
