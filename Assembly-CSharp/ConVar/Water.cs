using System;

namespace ConVar
{
	// Token: 0x02000AE7 RID: 2791
	[ConsoleSystem.Factory("water")]
	public class Water : ConsoleSystem
	{
		// Token: 0x04003C43 RID: 15427
		[ClientVar(Saved = true)]
		public static int quality = 1;

		// Token: 0x04003C44 RID: 15428
		public static int MaxQuality = 2;

		// Token: 0x04003C45 RID: 15429
		public static int MinQuality = 0;

		// Token: 0x04003C46 RID: 15430
		[ClientVar(Saved = true)]
		public static int reflections = 1;

		// Token: 0x04003C47 RID: 15431
		public static int MaxReflections = 2;

		// Token: 0x04003C48 RID: 15432
		public static int MinReflections = 0;
	}
}
