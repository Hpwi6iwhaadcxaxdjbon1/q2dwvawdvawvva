using System;
using ConVar;
using UnityEngine;

// Token: 0x02000571 RID: 1393
public class JunkpileNPCSpawner : NPCSpawner
{
	// Token: 0x040022B2 RID: 8882
	[Header("Junkpile NPC Spawner")]
	public bool UseSpawnChance;

	// Token: 0x06002AB1 RID: 10929 RVA: 0x00103D10 File Offset: 0x00101F10
	protected override void Spawn(int numToSpawn)
	{
		if (this.UseSpawnChance && UnityEngine.Random.value > AI.npc_junkpilespawn_chance)
		{
			return;
		}
		base.Spawn(numToSpawn);
	}
}
