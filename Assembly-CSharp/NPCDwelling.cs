using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000180 RID: 384
public class NPCDwelling : BaseEntity
{
	// Token: 0x0400108C RID: 4236
	public NPCSpawner npcSpawner;

	// Token: 0x0400108D RID: 4237
	public float NPCSpawnChance = 1f;

	// Token: 0x0400108E RID: 4238
	public SpawnGroup[] spawnGroups;

	// Token: 0x0400108F RID: 4239
	public AIMovePoint[] movePoints;

	// Token: 0x04001090 RID: 4240
	public AICoverPoint[] coverPoints;

	// Token: 0x060017AD RID: 6061 RVA: 0x000B30B0 File Offset: 0x000B12B0
	public override void ServerInit()
	{
		base.ServerInit();
		this.UpdateInformationZone(false);
		if (this.npcSpawner != null && UnityEngine.Random.Range(0f, 1f) <= this.NPCSpawnChance)
		{
			this.npcSpawner.SpawnInitial();
		}
		SpawnGroup[] array = this.spawnGroups;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SpawnInitial();
		}
	}

	// Token: 0x060017AE RID: 6062 RVA: 0x000B3117 File Offset: 0x000B1317
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			this.CleanupSpawned();
		}
		base.DestroyShared();
		if (base.isServer)
		{
			this.UpdateInformationZone(true);
		}
	}

	// Token: 0x060017AF RID: 6063 RVA: 0x000B313C File Offset: 0x000B133C
	public bool ValidateAIPoint(Vector3 pos)
	{
		base.gameObject.SetActive(false);
		bool result = !GamePhysics.CheckSphere(pos + Vector3.up * 0.6f, 0.5f, 65537, QueryTriggerInteraction.UseGlobal);
		base.gameObject.SetActive(true);
		return result;
	}

	// Token: 0x060017B0 RID: 6064 RVA: 0x000B318C File Offset: 0x000B138C
	public void UpdateInformationZone(bool remove)
	{
		AIInformationZone forPoint = AIInformationZone.GetForPoint(base.transform.position, true);
		if (forPoint == null)
		{
			return;
		}
		if (remove)
		{
			forPoint.RemoveDynamicAIPoints(this.movePoints, this.coverPoints);
			return;
		}
		forPoint.AddDynamicAIPoints(this.movePoints, this.coverPoints, new Func<Vector3, bool>(this.ValidateAIPoint));
	}

	// Token: 0x060017B1 RID: 6065 RVA: 0x000B31E9 File Offset: 0x000B13E9
	public void CheckDespawn()
	{
		if (this.PlayersNearby())
		{
			return;
		}
		if (this.npcSpawner && this.npcSpawner.currentPopulation > 0)
		{
			return;
		}
		this.CleanupSpawned();
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060017B2 RID: 6066 RVA: 0x000B3220 File Offset: 0x000B1420
	public void CleanupSpawned()
	{
		if (this.spawnGroups != null)
		{
			SpawnGroup[] array = this.spawnGroups;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Clear();
			}
		}
		if (this.npcSpawner)
		{
			this.npcSpawner.Clear();
		}
	}

	// Token: 0x060017B3 RID: 6067 RVA: 0x000B326C File Offset: 0x000B146C
	public bool PlayersNearby()
	{
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Vis.Entities<BasePlayer>(base.transform.position, this.TimeoutPlayerCheckRadius(), list, 131072, QueryTriggerInteraction.Collide);
		bool result = false;
		foreach (BasePlayer basePlayer in list)
		{
			if (!basePlayer.IsSleeping() && basePlayer.IsAlive())
			{
				result = true;
				break;
			}
		}
		Pool.FreeList<BasePlayer>(ref list);
		return result;
	}

	// Token: 0x060017B4 RID: 6068 RVA: 0x0004DAAE File Offset: 0x0004BCAE
	public virtual float TimeoutPlayerCheckRadius()
	{
		return 10f;
	}
}
