using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AEB RID: 2795
	[ConsoleSystem.Factory("xmas")]
	public class XMas : ConsoleSystem
	{
		// Token: 0x04003C4D RID: 15437
		private const string path = "assets/prefabs/misc/xmas/xmasrefill.prefab";

		// Token: 0x04003C4E RID: 15438
		[ServerVar]
		public static bool enabled = false;

		// Token: 0x04003C4F RID: 15439
		[ServerVar]
		public static float spawnRange = 40f;

		// Token: 0x04003C50 RID: 15440
		[ServerVar]
		public static int spawnAttempts = 5;

		// Token: 0x04003C51 RID: 15441
		[ServerVar]
		public static int giftsPerPlayer = 2;

		// Token: 0x06004379 RID: 17273 RVA: 0x0018E4B4 File Offset: 0x0018C6B4
		[ServerVar]
		public static void refill(ConsoleSystem.Arg arg)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/misc/xmas/xmasrefill.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.Spawn();
			}
		}
	}
}
