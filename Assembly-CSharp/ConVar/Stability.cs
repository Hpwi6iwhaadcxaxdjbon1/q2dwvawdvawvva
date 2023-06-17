using System;
using System.Linq;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000ADC RID: 2780
	[ConsoleSystem.Factory("stability")]
	public class Stability : ConsoleSystem
	{
		// Token: 0x04003C27 RID: 15399
		[ServerVar]
		public static int verbose = 0;

		// Token: 0x04003C28 RID: 15400
		[ServerVar]
		public static int strikes = 10;

		// Token: 0x04003C29 RID: 15401
		[ServerVar]
		public static float collapse = 0.05f;

		// Token: 0x04003C2A RID: 15402
		[ServerVar]
		public static float accuracy = 0.001f;

		// Token: 0x04003C2B RID: 15403
		[ServerVar]
		public static float stabilityqueue = 9f;

		// Token: 0x04003C2C RID: 15404
		[ServerVar]
		public static float surroundingsqueue = 3f;

		// Token: 0x06004308 RID: 17160 RVA: 0x0018CCD8 File Offset: 0x0018AED8
		[ServerVar]
		public static void refresh_stability(ConsoleSystem.Arg args)
		{
			StabilityEntity[] array = BaseNetworkable.serverEntities.OfType<StabilityEntity>().ToArray<StabilityEntity>();
			Debug.Log("Refreshing stability on " + array.Length + " entities...");
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UpdateStability();
			}
		}
	}
}
