using System;

namespace ConVar
{
	// Token: 0x02000AD8 RID: 2776
	[ConsoleSystem.Factory("sentry")]
	public class Sentry : ConsoleSystem
	{
		// Token: 0x04003B97 RID: 15255
		[ServerVar(Help = "target everyone regardless of authorization")]
		public static bool targetall = false;

		// Token: 0x04003B98 RID: 15256
		[ServerVar(Help = "how long until something is considered hostile after it attacked")]
		public static float hostileduration = 120f;
	}
}
