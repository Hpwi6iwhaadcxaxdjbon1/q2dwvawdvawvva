using System;

namespace ConVar
{
	// Token: 0x02000AE0 RID: 2784
	[ConsoleSystem.Factory("terrain")]
	public class Terrain : ConsoleSystem
	{
		// Token: 0x04003C2F RID: 15407
		[ClientVar(Saved = true)]
		public static float quality = 100f;
	}
}
