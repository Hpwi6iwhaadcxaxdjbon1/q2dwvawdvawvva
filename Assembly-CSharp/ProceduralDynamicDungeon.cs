using System;
using System.Collections;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000196 RID: 406
public class ProceduralDynamicDungeon : global::BaseEntity
{
	// Token: 0x04001101 RID: 4353
	public int gridResolution = 6;

	// Token: 0x04001102 RID: 4354
	public float gridSpacing = 12f;

	// Token: 0x04001103 RID: 4355
	public bool[] grid;

	// Token: 0x04001104 RID: 4356
	public List<GameObjectRef> cellPrefabReferences = new List<GameObjectRef>();

	// Token: 0x04001105 RID: 4357
	public List<ProceduralDungeonCell> spawnedCells = new List<ProceduralDungeonCell>();

	// Token: 0x04001106 RID: 4358
	public EnvironmentVolume envVolume;

	// Token: 0x04001107 RID: 4359
	public MonumentNavMesh monumentNavMesh;

	// Token: 0x04001108 RID: 4360
	public GameObjectRef exitPortalPrefab;

	// Token: 0x04001109 RID: 4361
	private EntityRef<BasePortal> exitPortal;

	// Token: 0x0400110A RID: 4362
	public TriggerRadiation exitRadiation;

	// Token: 0x0400110B RID: 4363
	public uint seed;

	// Token: 0x0400110C RID: 4364
	public uint baseseed;

	// Token: 0x0400110D RID: 4365
	public Vector3 mapOffset = Vector3.zero;

	// Token: 0x0400110E RID: 4366
	public static readonly List<ProceduralDynamicDungeon> dungeons = new List<ProceduralDynamicDungeon>();

	// Token: 0x0400110F RID: 4367
	public ProceduralDungeonCell entranceHack;

	// Token: 0x06001817 RID: 6167 RVA: 0x000B4F22 File Offset: 0x000B3122
	public override void InitShared()
	{
		base.InitShared();
		ProceduralDynamicDungeon.dungeons.Add(this);
	}

	// Token: 0x06001818 RID: 6168 RVA: 0x000B4F38 File Offset: 0x000B3138
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		foreach (ProceduralDungeonCell proceduralDungeonCell in this.spawnedCells)
		{
			EntityFlag_Toggle[] componentsInChildren = proceduralDungeonCell.GetComponentsInChildren<EntityFlag_Toggle>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].DoUpdate(this);
			}
		}
	}

	// Token: 0x06001819 RID: 6169 RVA: 0x000B4FA8 File Offset: 0x000B31A8
	public global::BaseEntity GetExitPortal(bool serverSide)
	{
		return this.exitPortal.Get(serverSide);
	}

	// Token: 0x0600181A RID: 6170 RVA: 0x000B4FB6 File Offset: 0x000B31B6
	public override void DestroyShared()
	{
		ProceduralDynamicDungeon.dungeons.Remove(this);
		this.RetireAllCells();
		base.DestroyShared();
	}

	// Token: 0x0600181B RID: 6171 RVA: 0x000B4FD0 File Offset: 0x000B31D0
	public bool ContainsAnyPlayers()
	{
		Bounds bounds = new Bounds(base.transform.position, new Vector3((float)this.gridResolution * this.gridSpacing, 20f, (float)this.gridResolution * this.gridSpacing));
		for (int i = 0; i < global::BasePlayer.activePlayerList.Count; i++)
		{
			global::BasePlayer basePlayer = global::BasePlayer.activePlayerList[i];
			if (bounds.Contains(basePlayer.transform.position))
			{
				return true;
			}
		}
		for (int j = 0; j < global::BasePlayer.sleepingPlayerList.Count; j++)
		{
			global::BasePlayer basePlayer2 = global::BasePlayer.sleepingPlayerList[j];
			if (bounds.Contains(basePlayer2.transform.position))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600181C RID: 6172 RVA: 0x000B5088 File Offset: 0x000B3288
	public void KillPlayers()
	{
		Bounds bounds = new Bounds(base.transform.position, new Vector3((float)this.gridResolution * this.gridSpacing, 20f, (float)this.gridResolution * this.gridSpacing));
		for (int i = 0; i < global::BasePlayer.activePlayerList.Count; i++)
		{
			global::BasePlayer basePlayer = global::BasePlayer.activePlayerList[i];
			if (bounds.Contains(basePlayer.transform.position))
			{
				basePlayer.Hurt(10000f, DamageType.Suicide, null, false);
			}
		}
		for (int j = 0; j < global::BasePlayer.sleepingPlayerList.Count; j++)
		{
			global::BasePlayer basePlayer2 = global::BasePlayer.sleepingPlayerList[j];
			if (bounds.Contains(basePlayer2.transform.position))
			{
				basePlayer2.Hurt(10000f, DamageType.Suicide, null, false);
			}
		}
	}

	// Token: 0x0600181D RID: 6173 RVA: 0x000B5156 File Offset: 0x000B3356
	internal override void DoServerDestroy()
	{
		this.KillPlayers();
		if (this.exitPortal.IsValid(true))
		{
			this.exitPortal.Get(true).Kill(global::BaseNetworkable.DestroyMode.None);
		}
		base.DoServerDestroy();
	}

	// Token: 0x0600181E RID: 6174 RVA: 0x000B5184 File Offset: 0x000B3384
	public override void ServerInit()
	{
		if (!Rust.Application.isLoadingSave)
		{
			this.baseseed = (this.seed = (uint)UnityEngine.Random.Range(0, 12345567));
			Debug.Log("Spawning dungeon with seed :" + (int)this.seed);
		}
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.DoGeneration();
			BasePortal component = GameManager.server.CreateEntity(this.exitPortalPrefab.resourcePath, this.entranceHack.exitPointHack.position, this.entranceHack.exitPointHack.rotation, true).GetComponent<BasePortal>();
			component.Spawn();
			this.exitPortal.Set(component);
		}
	}

	// Token: 0x0600181F RID: 6175 RVA: 0x000B5230 File Offset: 0x000B3430
	public void DoGeneration()
	{
		this.GenerateGrid();
		this.CreateAIZ();
		if (base.isServer)
		{
			Debug.Log("Server DoGeneration,calling routine update nav mesh");
			base.StartCoroutine(this.UpdateNavMesh());
		}
		base.Invoke(new Action(this.InitSpawnGroups), 1f);
	}

	// Token: 0x06001820 RID: 6176 RVA: 0x000B5280 File Offset: 0x000B3480
	private void CreateAIZ()
	{
		AIInformationZone aiinformationZone = base.gameObject.AddComponent<AIInformationZone>();
		aiinformationZone.UseCalculatedCoverDistances = false;
		aiinformationZone.bounds.extents = new Vector3((float)this.gridResolution * this.gridSpacing * 0.75f, 10f, (float)this.gridResolution * this.gridSpacing * 0.75f);
		aiinformationZone.Init();
	}

	// Token: 0x06001821 RID: 6177 RVA: 0x000B52E1 File Offset: 0x000B34E1
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.DoGeneration();
	}

	// Token: 0x06001822 RID: 6178 RVA: 0x000B52EF File Offset: 0x000B34EF
	public IEnumerator UpdateNavMesh()
	{
		Debug.Log("Dungeon Building navmesh");
		yield return base.StartCoroutine(this.monumentNavMesh.UpdateNavMeshAndWait());
		Debug.Log("Dungeon done!");
		yield break;
	}

	// Token: 0x06001823 RID: 6179 RVA: 0x000B5300 File Offset: 0x000B3500
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.proceduralDungeon == null)
		{
			info.msg.proceduralDungeon = Pool.Get<ProceduralDungeon>();
		}
		info.msg.proceduralDungeon.seed = this.baseseed;
		info.msg.proceduralDungeon.exitPortalID = this.exitPortal.uid;
		info.msg.proceduralDungeon.mapOffset = this.mapOffset;
	}

	// Token: 0x06001824 RID: 6180 RVA: 0x000B5378 File Offset: 0x000B3578
	public BasePortal GetExitPortal()
	{
		return this.exitPortal.Get(true);
	}

	// Token: 0x06001825 RID: 6181 RVA: 0x000B5388 File Offset: 0x000B3588
	public void InitSpawnGroups()
	{
		foreach (ProceduralDungeonCell proceduralDungeonCell in this.spawnedCells)
		{
			if (!(this.entranceHack != null) || Vector3.Distance(this.entranceHack.transform.position, proceduralDungeonCell.transform.position) >= 20f)
			{
				SpawnGroup[] spawnGroups = proceduralDungeonCell.spawnGroups;
				for (int i = 0; i < spawnGroups.Length; i++)
				{
					spawnGroups[i].Spawn();
				}
			}
		}
	}

	// Token: 0x06001826 RID: 6182 RVA: 0x000B5428 File Offset: 0x000B3628
	public void CleanupSpawnGroups()
	{
		foreach (ProceduralDungeonCell proceduralDungeonCell in this.spawnedCells)
		{
			SpawnGroup[] spawnGroups = proceduralDungeonCell.spawnGroups;
			for (int i = 0; i < spawnGroups.Length; i++)
			{
				spawnGroups[i].Clear();
			}
		}
	}

	// Token: 0x06001827 RID: 6183 RVA: 0x000B5490 File Offset: 0x000B3690
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.proceduralDungeon != null)
		{
			this.baseseed = (this.seed = info.msg.proceduralDungeon.seed);
			this.exitPortal.uid = info.msg.proceduralDungeon.exitPortalID;
			this.mapOffset = info.msg.proceduralDungeon.mapOffset;
		}
	}

	// Token: 0x06001828 RID: 6184 RVA: 0x000B5504 File Offset: 0x000B3704
	[ContextMenu("Test Grid")]
	[ExecuteInEditMode]
	public void GenerateGrid()
	{
		Vector3 a = base.transform.position - new Vector3((float)this.gridResolution * this.gridSpacing * 0.5f, 0f, (float)this.gridResolution * this.gridSpacing * 0.5f);
		this.RetireAllCells();
		this.grid = new bool[this.gridResolution * this.gridResolution];
		for (int i = 0; i < this.grid.Length; i++)
		{
			this.grid[i] = ((SeedRandom.Range(ref this.seed, 0, 2) == 0) ? true : false);
		}
		this.SetEntrance(3, 0);
		for (int j = 0; j < this.gridResolution; j++)
		{
			for (int k = 0; k < this.gridResolution; k++)
			{
				if (this.GetGridState(j, k) && !this.HasPathToEntrance(j, k))
				{
					this.SetGridState(j, k, false);
				}
			}
		}
		for (int l = 0; l < this.gridResolution; l++)
		{
			for (int m = 0; m < this.gridResolution; m++)
			{
				if (this.GetGridState(l, m))
				{
					bool gridState = this.GetGridState(l, m + 1);
					bool gridState2 = this.GetGridState(l, m - 1);
					bool gridState3 = this.GetGridState(l - 1, m);
					bool gridState4 = this.GetGridState(l + 1, m);
					bool flag = this.IsEntrance(l, m);
					GameObjectRef gameObjectRef = null;
					ProceduralDungeonCell x = null;
					if (x == null)
					{
						foreach (GameObjectRef gameObjectRef2 in this.cellPrefabReferences)
						{
							ProceduralDungeonCell component = gameObjectRef2.Get().GetComponent<ProceduralDungeonCell>();
							if (component.north == gridState && component.south == gridState2 && component.west == gridState3 && component.east == gridState4 && component.entrance == flag)
							{
								x = component;
								gameObjectRef = gameObjectRef2;
								break;
							}
						}
					}
					if (x != null)
					{
						ProceduralDungeonCell proceduralDungeonCell = this.CellInstantiate(gameObjectRef.resourcePath);
						proceduralDungeonCell.transform.position = a + new Vector3((float)l * this.gridSpacing, 0f, (float)m * this.gridSpacing);
						this.spawnedCells.Add(proceduralDungeonCell);
						proceduralDungeonCell.transform.SetParent(base.transform);
						if (proceduralDungeonCell.entrance && this.entranceHack == null)
						{
							this.entranceHack = proceduralDungeonCell;
						}
					}
				}
			}
		}
	}

	// Token: 0x06001829 RID: 6185 RVA: 0x000B579C File Offset: 0x000B399C
	public ProceduralDungeonCell CellInstantiate(string path)
	{
		if (base.isServer)
		{
			return GameManager.server.CreatePrefab(path, true).GetComponent<ProceduralDungeonCell>();
		}
		return null;
	}

	// Token: 0x0600182A RID: 6186 RVA: 0x000B57B9 File Offset: 0x000B39B9
	public void RetireCell(GameObject cell)
	{
		if (cell == null)
		{
			return;
		}
		if (base.isServer)
		{
			GameManager.server.Retire(cell);
		}
	}

	// Token: 0x0600182B RID: 6187 RVA: 0x000B57D8 File Offset: 0x000B39D8
	public void RetireAllCells()
	{
		if (base.isServer)
		{
			this.CleanupSpawnGroups();
		}
		for (int i = this.spawnedCells.Count - 1; i >= 0; i--)
		{
			ProceduralDungeonCell proceduralDungeonCell = this.spawnedCells[i];
			if (proceduralDungeonCell)
			{
				this.RetireCell(proceduralDungeonCell.gameObject);
			}
		}
		this.spawnedCells.Clear();
	}

	// Token: 0x0600182C RID: 6188 RVA: 0x000B5838 File Offset: 0x000B3A38
	public bool CanSeeEntrance(int x, int y, ref List<int> checkedCells)
	{
		int gridIndex = this.GetGridIndex(x, y);
		if (checkedCells.Contains(gridIndex))
		{
			return false;
		}
		checkedCells.Add(gridIndex);
		if (!this.GetGridState(x, y))
		{
			return false;
		}
		if (this.IsEntrance(x, y))
		{
			return true;
		}
		bool flag = this.CanSeeEntrance(x, y + 1, ref checkedCells);
		bool flag2 = this.CanSeeEntrance(x, y - 1, ref checkedCells);
		bool flag3 = this.CanSeeEntrance(x - 1, y, ref checkedCells);
		bool flag4 = this.CanSeeEntrance(x + 1, y, ref checkedCells);
		return flag || flag4 || flag3 || flag2;
	}

	// Token: 0x0600182D RID: 6189 RVA: 0x000B58B0 File Offset: 0x000B3AB0
	public bool HasPathToEntrance(int x, int y)
	{
		List<int> list = new List<int>();
		bool result = this.CanSeeEntrance(x, y, ref list);
		list.Clear();
		return result;
	}

	// Token: 0x0600182E RID: 6190 RVA: 0x000B58D3 File Offset: 0x000B3AD3
	public bool CanFindEntrance(int x, int y)
	{
		new List<int>();
		this.GetGridState(x, y + 1);
		this.GetGridState(x, y - 1);
		this.GetGridState(x - 1, y);
		this.GetGridState(x + 1, y);
		return true;
	}

	// Token: 0x0600182F RID: 6191 RVA: 0x000B5908 File Offset: 0x000B3B08
	public bool IsEntrance(int x, int y)
	{
		return this.GetGridIndex(x, y) == this.GetEntranceIndex();
	}

	// Token: 0x06001830 RID: 6192 RVA: 0x000B591A File Offset: 0x000B3B1A
	public int GetEntranceIndex()
	{
		return this.GetGridIndex(3, 0);
	}

	// Token: 0x06001831 RID: 6193 RVA: 0x000B5924 File Offset: 0x000B3B24
	public void SetEntrance(int x, int y)
	{
		this.grid[this.GetGridIndex(x, y)] = true;
		this.grid[this.GetGridIndex(x, y + 1)] = true;
		this.grid[this.GetGridIndex(x - 1, y)] = false;
		this.grid[this.GetGridIndex(x + 1, y)] = false;
		this.grid[this.GetGridIndex(x, y + 2)] = true;
		this.grid[this.GetGridIndex(x + 1, y + 2)] = ((SeedRandom.Range(ref this.seed, 0, 1) == 1) ? true : false);
		this.grid[this.GetGridIndex(x + 2, y + 2)] = ((SeedRandom.Range(ref this.seed, 0, 1) == 1) ? true : false);
		this.grid[this.GetGridIndex(x, y + 3)] = true;
		this.grid[this.GetGridIndex(x, y + 4)] = true;
		this.grid[this.GetGridIndex(x - 1, y + 4)] = ((SeedRandom.Range(ref this.seed, 0, 1) == 1) ? true : false);
		this.grid[this.GetGridIndex(x - 2, y + 4)] = ((SeedRandom.Range(ref this.seed, 0, 1) == 1) ? true : false);
	}

	// Token: 0x06001832 RID: 6194 RVA: 0x000B5A4C File Offset: 0x000B3C4C
	public void SetGridState(int x, int y, bool state)
	{
		int gridIndex = this.GetGridIndex(x, y);
		this.grid[gridIndex] = state;
	}

	// Token: 0x06001833 RID: 6195 RVA: 0x000B5A6C File Offset: 0x000B3C6C
	public bool GetGridState(int x, int y)
	{
		return this.GetGridIndex(x, y) < this.grid.Length && x >= 0 && x < this.gridResolution && y >= 0 && y < this.gridResolution && this.grid[this.GetGridIndex(x, y)];
	}

	// Token: 0x06001834 RID: 6196 RVA: 0x000B5ABA File Offset: 0x000B3CBA
	public int GetGridX(int index)
	{
		return index % this.gridResolution;
	}

	// Token: 0x06001835 RID: 6197 RVA: 0x000B5AC4 File Offset: 0x000B3CC4
	public int GetGridY(int index)
	{
		return Mathf.FloorToInt((float)index / (float)this.gridResolution);
	}

	// Token: 0x06001836 RID: 6198 RVA: 0x000B5AD5 File Offset: 0x000B3CD5
	public int GetGridIndex(int x, int y)
	{
		return y * this.gridResolution + x;
	}
}
