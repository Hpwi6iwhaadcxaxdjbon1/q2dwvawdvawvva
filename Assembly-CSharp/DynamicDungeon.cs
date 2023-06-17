using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000192 RID: 402
public class DynamicDungeon : BaseEntity, IMissionEntityListener
{
	// Token: 0x040010D8 RID: 4312
	public Transform exitEntitySpawn;

	// Token: 0x040010D9 RID: 4313
	public GameObjectRef exitEntity;

	// Token: 0x040010DA RID: 4314
	public string exitString;

	// Token: 0x040010DB RID: 4315
	public MonumentNavMesh monumentNavMesh;

	// Token: 0x040010DC RID: 4316
	private static List<DynamicDungeon> _dungeons = new List<DynamicDungeon>();

	// Token: 0x040010DD RID: 4317
	public GameObjectRef portalPrefab;

	// Token: 0x040010DE RID: 4318
	public Transform portalSpawnPoint;

	// Token: 0x040010DF RID: 4319
	public BasePortal exitPortal;

	// Token: 0x040010E0 RID: 4320
	public GameObjectRef doorPrefab;

	// Token: 0x040010E1 RID: 4321
	public Transform doorSpawnPoint;

	// Token: 0x040010E2 RID: 4322
	public Door doorInstance;

	// Token: 0x040010E3 RID: 4323
	public static Vector3 nextDungeonPos = Vector3.zero;

	// Token: 0x040010E4 RID: 4324
	public static Vector3 dungeonStartPoint = Vector3.zero;

	// Token: 0x040010E5 RID: 4325
	public static float dungeonSpacing = 50f;

	// Token: 0x040010E6 RID: 4326
	public SpawnGroup[] spawnGroups;

	// Token: 0x040010E7 RID: 4327
	public bool AutoMergeAIZones = true;

	// Token: 0x060017F2 RID: 6130 RVA: 0x000B4388 File Offset: 0x000B2588
	public static void AddDungeon(DynamicDungeon newDungeon)
	{
		DynamicDungeon._dungeons.Add(newDungeon);
		Vector3 position = newDungeon.transform.position;
		if (position.y >= DynamicDungeon.nextDungeonPos.y)
		{
			DynamicDungeon.nextDungeonPos = position + Vector3.up * DynamicDungeon.dungeonSpacing;
		}
	}

	// Token: 0x060017F3 RID: 6131 RVA: 0x000B43D8 File Offset: 0x000B25D8
	public static void RemoveDungeon(DynamicDungeon dungeon)
	{
		Vector3 position = dungeon.transform.position;
		if (DynamicDungeon._dungeons.Contains(dungeon))
		{
			DynamicDungeon._dungeons.Remove(dungeon);
		}
		DynamicDungeon.nextDungeonPos = position;
	}

	// Token: 0x060017F4 RID: 6132 RVA: 0x000B4403 File Offset: 0x000B2603
	public static Vector3 GetNextDungeonPoint()
	{
		if (DynamicDungeon.nextDungeonPos == Vector3.zero)
		{
			DynamicDungeon.nextDungeonPos = Vector3.one * 700f;
		}
		return DynamicDungeon.nextDungeonPos;
	}

	// Token: 0x060017F5 RID: 6133 RVA: 0x000B442F File Offset: 0x000B262F
	public IEnumerator UpdateNavMesh()
	{
		Debug.Log("Dungeon Building navmesh");
		yield return base.StartCoroutine(this.monumentNavMesh.UpdateNavMeshAndWait());
		Debug.Log("Dunngeon done!");
		yield break;
	}

	// Token: 0x060017F6 RID: 6134 RVA: 0x000B4440 File Offset: 0x000B2640
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			SpawnGroup[] array = this.spawnGroups;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Clear();
			}
			if (this.exitPortal != null)
			{
				this.exitPortal.Kill(BaseNetworkable.DestroyMode.None);
			}
			DynamicDungeon.RemoveDungeon(this);
		}
		base.DestroyShared();
	}

	// Token: 0x060017F7 RID: 6135 RVA: 0x000B4498 File Offset: 0x000B2698
	public override void ServerInit()
	{
		base.ServerInit();
		DynamicDungeon.AddDungeon(this);
		if (this.portalPrefab.isValid)
		{
			this.exitPortal = GameManager.server.CreateEntity(this.portalPrefab.resourcePath, this.portalSpawnPoint.position, this.portalSpawnPoint.rotation, true).GetComponent<BasePortal>();
			this.exitPortal.SetParent(this, true, false);
			this.exitPortal.Spawn();
		}
		if (this.doorPrefab.isValid)
		{
			this.doorInstance = GameManager.server.CreateEntity(this.doorPrefab.resourcePath, this.doorSpawnPoint.position, this.doorSpawnPoint.rotation, true).GetComponent<Door>();
			this.doorInstance.SetParent(this, true, false);
			this.doorInstance.Spawn();
		}
		this.MergeAIZones();
		base.StartCoroutine(this.UpdateNavMesh());
	}

	// Token: 0x060017F8 RID: 6136 RVA: 0x000B4580 File Offset: 0x000B2780
	private void MergeAIZones()
	{
		if (!this.AutoMergeAIZones)
		{
			return;
		}
		List<AIInformationZone> list = base.GetComponentsInChildren<AIInformationZone>().ToList<AIInformationZone>();
		foreach (AIInformationZone aiinformationZone in list)
		{
			aiinformationZone.AddInitialPoints();
		}
		GameObject gameObject = new GameObject("AIZ");
		gameObject.transform.position = base.transform.position;
		AIInformationZone.Merge(list, gameObject).ShouldSleepAI = false;
		gameObject.transform.SetParent(base.transform);
	}

	// Token: 0x060017F9 RID: 6137 RVA: 0x000B4620 File Offset: 0x000B2820
	public void MissionStarted(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
		foreach (MissionEntity missionEntity in instance.createdEntities)
		{
			BunkerEntrance component = missionEntity.GetComponent<BunkerEntrance>();
			if (component != null)
			{
				BasePortal portalInstance = component.portalInstance;
				if (portalInstance)
				{
					portalInstance.targetPortal = this.exitPortal;
					this.exitPortal.targetPortal = portalInstance;
					Debug.Log("Dungeon portal linked...");
				}
			}
		}
	}

	// Token: 0x060017FA RID: 6138 RVA: 0x000063A5 File Offset: 0x000045A5
	public void MissionEnded(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
	}
}
