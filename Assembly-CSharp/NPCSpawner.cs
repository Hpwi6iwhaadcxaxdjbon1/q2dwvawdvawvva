using System;
using ConVar;
using UnityEngine;

// Token: 0x02000572 RID: 1394
public class NPCSpawner : SpawnGroup
{
	// Token: 0x040022B3 RID: 8883
	public int AdditionalLOSBlockingLayer;

	// Token: 0x040022B4 RID: 8884
	public MonumentNavMesh monumentNavMesh;

	// Token: 0x040022B5 RID: 8885
	public bool shouldFillOnSpawn;

	// Token: 0x040022B6 RID: 8886
	[Header("InfoZone Config")]
	public AIInformationZone VirtualInfoZone;

	// Token: 0x040022B7 RID: 8887
	[Header("Navigator Config")]
	public AIMovePointPath Path;

	// Token: 0x040022B8 RID: 8888
	public BasePath AStarGraph;

	// Token: 0x040022B9 RID: 8889
	[Header("Human Stat Replacements")]
	public bool UseStatModifiers;

	// Token: 0x040022BA RID: 8890
	public float SenseRange = 30f;

	// Token: 0x040022BB RID: 8891
	public bool CheckLOS = true;

	// Token: 0x040022BC RID: 8892
	public float TargetLostRange = 50f;

	// Token: 0x040022BD RID: 8893
	public float AttackRangeMultiplier = 1f;

	// Token: 0x040022BE RID: 8894
	public float ListenRange = 10f;

	// Token: 0x040022BF RID: 8895
	public float CanUseHealingItemsChance;

	// Token: 0x040022C0 RID: 8896
	[Header("Loadout Replacements")]
	public PlayerInventoryProperties[] Loadouts;

	// Token: 0x06002AB3 RID: 10931 RVA: 0x00103D36 File Offset: 0x00101F36
	public override void SpawnInitial()
	{
		this.fillOnSpawn = this.shouldFillOnSpawn;
		if (this.WaitingForNavMesh())
		{
			base.Invoke(new Action(this.LateSpawn), 10f);
			return;
		}
		base.SpawnInitial();
	}

	// Token: 0x06002AB4 RID: 10932 RVA: 0x00103D6A File Offset: 0x00101F6A
	public bool WaitingForNavMesh()
	{
		if (this.monumentNavMesh != null)
		{
			return this.monumentNavMesh.IsBuilding;
		}
		return !DungeonNavmesh.NavReady() || !AI.move;
	}

	// Token: 0x06002AB5 RID: 10933 RVA: 0x00103D97 File Offset: 0x00101F97
	public void LateSpawn()
	{
		if (!this.WaitingForNavMesh())
		{
			this.SpawnInitial();
			Debug.Log("Navmesh complete, spawning");
			return;
		}
		base.Invoke(new Action(this.LateSpawn), 5f);
	}

	// Token: 0x06002AB6 RID: 10934 RVA: 0x00103DCC File Offset: 0x00101FCC
	protected override void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
	{
		base.PostSpawnProcess(entity, spawnPoint);
		BaseNavigator component = entity.GetComponent<BaseNavigator>();
		HumanNPC humanNPC;
		if (this.AdditionalLOSBlockingLayer != 0 && entity != null && (humanNPC = (entity as HumanNPC)) != null)
		{
			humanNPC.AdditionalLosBlockingLayer = this.AdditionalLOSBlockingLayer;
		}
		HumanNPC humanNPC2 = entity as HumanNPC;
		if (humanNPC2 != null)
		{
			if (this.Loadouts != null && this.Loadouts.Length != 0)
			{
				humanNPC2.EquipLoadout(this.Loadouts);
			}
			this.ModifyHumanBrainStats(humanNPC2.Brain);
		}
		if (this.VirtualInfoZone != null)
		{
			if (this.VirtualInfoZone.Virtual)
			{
				NPCPlayer npcplayer = entity as NPCPlayer;
				if (npcplayer != null)
				{
					npcplayer.VirtualInfoZone = this.VirtualInfoZone;
					if (humanNPC2 != null)
					{
						humanNPC2.VirtualInfoZone.RegisterSleepableEntity(humanNPC2.Brain);
					}
				}
			}
			else
			{
				Debug.LogError("NPCSpawner trying to set a virtual info zone without the Virtual property!");
			}
		}
		if (component != null)
		{
			component.Path = this.Path;
			component.AStarGraph = this.AStarGraph;
		}
	}

	// Token: 0x06002AB7 RID: 10935 RVA: 0x00103EC8 File Offset: 0x001020C8
	private void ModifyHumanBrainStats(BaseAIBrain brain)
	{
		if (!this.UseStatModifiers)
		{
			return;
		}
		if (brain == null)
		{
			return;
		}
		brain.SenseRange = this.SenseRange;
		brain.TargetLostRange *= this.TargetLostRange;
		brain.AttackRangeMultiplier = this.AttackRangeMultiplier;
		brain.ListenRange = this.ListenRange;
		brain.CheckLOS = this.CheckLOS;
		if (this.CanUseHealingItemsChance > 0f)
		{
			brain.CanUseHealingItems = (UnityEngine.Random.Range(0f, 1f) <= this.CanUseHealingItemsChance);
		}
	}
}
