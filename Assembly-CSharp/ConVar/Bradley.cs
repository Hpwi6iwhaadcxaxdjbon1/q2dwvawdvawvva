using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AA8 RID: 2728
	[ConsoleSystem.Factory("bradley")]
	public class Bradley : ConsoleSystem
	{
		// Token: 0x04003B06 RID: 15110
		[ServerVar]
		public static float respawnDelayMinutes = 60f;

		// Token: 0x04003B07 RID: 15111
		[ServerVar]
		public static float respawnDelayVariance = 1f;

		// Token: 0x04003B08 RID: 15112
		[ServerVar]
		public static bool enabled = true;

		// Token: 0x0600416A RID: 16746 RVA: 0x0018410C File Offset: 0x0018230C
		[ServerVar]
		public static void quickrespawn(ConsoleSystem.Arg arg)
		{
			if (!arg.Player())
			{
				return;
			}
			BradleySpawner singleton = BradleySpawner.singleton;
			if (singleton == null)
			{
				Debug.LogWarning("No Spawner");
				return;
			}
			if (singleton.spawned)
			{
				singleton.spawned.Kill(BaseNetworkable.DestroyMode.None);
			}
			singleton.spawned = null;
			singleton.DoRespawn();
		}
	}
}
