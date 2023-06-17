using System;

namespace ConVar
{
	// Token: 0x02000ACE RID: 2766
	[ConsoleSystem.Factory("net")]
	public class Net : ConsoleSystem
	{
		// Token: 0x04003B84 RID: 15236
		[ServerVar]
		public static bool visdebug = false;

		// Token: 0x04003B85 RID: 15237
		[ClientVar]
		public static bool debug = false;

		// Token: 0x04003B86 RID: 15238
		[ServerVar]
		public static int visibilityRadiusFarOverride = -1;

		// Token: 0x04003B87 RID: 15239
		[ServerVar]
		public static int visibilityRadiusNearOverride = -1;
	}
}
