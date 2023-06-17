using System;
using UnityEngine;

// Token: 0x02000198 RID: 408
public class HorseSpawner : VehicleSpawner
{
	// Token: 0x04001113 RID: 4371
	public float respawnDelay = 10f;

	// Token: 0x04001114 RID: 4372
	public float respawnDelayVariance = 5f;

	// Token: 0x04001115 RID: 4373
	public bool spawnForSale = true;

	// Token: 0x0600183C RID: 6204 RVA: 0x000B5B70 File Offset: 0x000B3D70
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.RespawnHorse), UnityEngine.Random.Range(0f, 4f), this.respawnDelay, this.respawnDelayVariance);
	}

	// Token: 0x0600183D RID: 6205 RVA: 0x000B5BA5 File Offset: 0x000B3DA5
	public override int GetOccupyLayer()
	{
		return 2048;
	}

	// Token: 0x0600183E RID: 6206 RVA: 0x000B5BAC File Offset: 0x000B3DAC
	public void RespawnHorse()
	{
		if (base.GetVehicleOccupying() != null)
		{
			return;
		}
		BaseVehicle baseVehicle = base.SpawnVehicle(this.objectsToSpawn[0].prefabToSpawn.resourcePath, null);
		if (this.spawnForSale)
		{
			RidableHorse ridableHorse = baseVehicle as RidableHorse;
			if (ridableHorse != null)
			{
				ridableHorse.SetForSale();
			}
		}
	}

	// Token: 0x17000205 RID: 517
	// (get) Token: 0x0600183F RID: 6207 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected override bool LogAnalytics
	{
		get
		{
			return false;
		}
	}
}
