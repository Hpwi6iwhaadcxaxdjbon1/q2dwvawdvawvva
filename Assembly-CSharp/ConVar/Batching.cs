using System;

namespace ConVar
{
	// Token: 0x02000AA7 RID: 2727
	[ConsoleSystem.Factory("batching")]
	public class Batching : ConsoleSystem
	{
		// Token: 0x04003B00 RID: 15104
		[ClientVar]
		public static bool renderers = true;

		// Token: 0x04003B01 RID: 15105
		[ClientVar]
		public static bool renderer_threading = true;

		// Token: 0x04003B02 RID: 15106
		[ClientVar]
		public static int renderer_capacity = 30000;

		// Token: 0x04003B03 RID: 15107
		[ClientVar]
		public static int renderer_vertices = 1000;

		// Token: 0x04003B04 RID: 15108
		[ClientVar]
		public static int renderer_submeshes = 1;

		// Token: 0x04003B05 RID: 15109
		[ServerVar]
		[ClientVar]
		public static int verbose = 0;
	}
}
