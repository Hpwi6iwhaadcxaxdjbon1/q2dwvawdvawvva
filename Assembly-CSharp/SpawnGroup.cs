﻿using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using UnityEngine;

// Token: 0x02000576 RID: 1398
public class SpawnGroup : BaseMonoBehaviour, IServerComponent, ISpawnPointUser, ISpawnGroup
{
	// Token: 0x040022C5 RID: 8901
	[InspectorFlags]
	public MonumentTier Tier = (MonumentTier)(-1);

	// Token: 0x040022C6 RID: 8902
	public List<SpawnGroup.SpawnEntry> prefabs;

	// Token: 0x040022C7 RID: 8903
	public int maxPopulation = 5;

	// Token: 0x040022C8 RID: 8904
	public int numToSpawnPerTickMin = 1;

	// Token: 0x040022C9 RID: 8905
	public int numToSpawnPerTickMax = 2;

	// Token: 0x040022CA RID: 8906
	public float respawnDelayMin = 10f;

	// Token: 0x040022CB RID: 8907
	public float respawnDelayMax = 20f;

	// Token: 0x040022CC RID: 8908
	public bool wantsInitialSpawn = true;

	// Token: 0x040022CD RID: 8909
	public bool temporary;

	// Token: 0x040022CE RID: 8910
	public bool forceInitialSpawn;

	// Token: 0x040022CF RID: 8911
	public bool preventDuplicates;

	// Token: 0x040022D0 RID: 8912
	public bool isSpawnerActive = true;

	// Token: 0x040022D1 RID: 8913
	public BoxCollider setFreeIfMovedBeyond;

	// Token: 0x040022D2 RID: 8914
	public string category;

	// Token: 0x040022D3 RID: 8915
	[NonSerialized]
	public MonumentInfo Monument;

	// Token: 0x040022D4 RID: 8916
	protected bool fillOnSpawn;

	// Token: 0x040022D5 RID: 8917
	protected BaseSpawnPoint[] spawnPoints;

	// Token: 0x040022D6 RID: 8918
	private List<SpawnPointInstance> spawnInstances = new List<SpawnPointInstance>();

	// Token: 0x040022D7 RID: 8919
	private LocalClock spawnClock = new LocalClock();

	// Token: 0x17000394 RID: 916
	// (get) Token: 0x06002AC3 RID: 10947 RVA: 0x001040F5 File Offset: 0x001022F5
	public int currentPopulation
	{
		get
		{
			return this.spawnInstances.Count;
		}
	}

	// Token: 0x06002AC4 RID: 10948 RVA: 0x00104102 File Offset: 0x00102302
	public virtual bool WantsInitialSpawn()
	{
		return this.wantsInitialSpawn;
	}

	// Token: 0x06002AC5 RID: 10949 RVA: 0x0010410A File Offset: 0x0010230A
	public virtual bool WantsTimedSpawn()
	{
		return this.respawnDelayMax != float.PositiveInfinity;
	}

	// Token: 0x06002AC6 RID: 10950 RVA: 0x0010411C File Offset: 0x0010231C
	public float GetSpawnDelta()
	{
		return (this.respawnDelayMax + this.respawnDelayMin) * 0.5f / SpawnHandler.PlayerScale(ConVar.Spawn.player_scale);
	}

	// Token: 0x06002AC7 RID: 10951 RVA: 0x0010413C File Offset: 0x0010233C
	public float GetSpawnVariance()
	{
		return (this.respawnDelayMax - this.respawnDelayMin) * 0.5f / SpawnHandler.PlayerScale(ConVar.Spawn.player_scale);
	}

	// Token: 0x06002AC8 RID: 10952 RVA: 0x0010415C File Offset: 0x0010235C
	protected void Awake()
	{
		if (TerrainMeta.TopologyMap == null)
		{
			return;
		}
		int topology = TerrainMeta.TopologyMap.GetTopology(base.transform.position);
		int num = 469762048;
		int num2 = MonumentInfo.TierToMask(this.Tier);
		if (num2 != num && (num2 & topology) == 0)
		{
			return;
		}
		this.spawnPoints = base.GetComponentsInChildren<BaseSpawnPoint>();
		if (this.WantsTimedSpawn())
		{
			this.spawnClock.Add(this.GetSpawnDelta(), this.GetSpawnVariance(), new Action(this.Spawn));
		}
		if (!this.temporary && SingletonComponent<SpawnHandler>.Instance)
		{
			SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Add(this);
		}
		if (this.forceInitialSpawn)
		{
			base.Invoke(new Action(this.SpawnInitial), 1f);
		}
		this.Monument = this.FindMonument();
	}

	// Token: 0x06002AC9 RID: 10953 RVA: 0x00103A2F File Offset: 0x00101C2F
	protected void OnDestroy()
	{
		if (SingletonComponent<SpawnHandler>.Instance)
		{
			SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Remove(this);
			return;
		}
		Debug.LogWarning(base.GetType().Name + ": SpawnHandler instance not found.");
	}

	// Token: 0x06002ACA RID: 10954 RVA: 0x00104230 File Offset: 0x00102430
	public void Fill()
	{
		if (this.isSpawnerActive)
		{
			this.Spawn(this.maxPopulation);
		}
	}

	// Token: 0x06002ACB RID: 10955 RVA: 0x00104248 File Offset: 0x00102448
	public void Clear()
	{
		for (int i = this.spawnInstances.Count - 1; i >= 0; i--)
		{
			SpawnPointInstance spawnPointInstance = this.spawnInstances[i];
			BaseEntity baseEntity = spawnPointInstance.gameObject.ToBaseEntity();
			if (this.setFreeIfMovedBeyond != null && !this.setFreeIfMovedBeyond.bounds.Contains(baseEntity.transform.position))
			{
				spawnPointInstance.Retire();
			}
			else if (baseEntity)
			{
				baseEntity.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
		this.spawnInstances.Clear();
	}

	// Token: 0x06002ACC RID: 10956 RVA: 0x001042D8 File Offset: 0x001024D8
	public bool HasSpawned(uint prefabID)
	{
		foreach (SpawnPointInstance spawnPointInstance in this.spawnInstances)
		{
			BaseEntity baseEntity = spawnPointInstance.gameObject.ToBaseEntity();
			if (baseEntity && baseEntity.prefabID == prefabID)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002ACD RID: 10957 RVA: 0x00104348 File Offset: 0x00102548
	public virtual void SpawnInitial()
	{
		if (!this.wantsInitialSpawn)
		{
			return;
		}
		if (this.isSpawnerActive)
		{
			if (this.fillOnSpawn)
			{
				this.Spawn(this.maxPopulation);
				return;
			}
			this.Spawn();
		}
	}

	// Token: 0x06002ACE RID: 10958 RVA: 0x00104378 File Offset: 0x00102578
	public void SpawnRepeating()
	{
		for (int i = 0; i < this.spawnClock.events.Count; i++)
		{
			LocalClock.TimedEvent timedEvent = this.spawnClock.events[i];
			if (UnityEngine.Time.time > timedEvent.time)
			{
				timedEvent.delta = this.GetSpawnDelta();
				timedEvent.variance = this.GetSpawnVariance();
				this.spawnClock.events[i] = timedEvent;
			}
		}
		this.spawnClock.Tick();
	}

	// Token: 0x06002ACF RID: 10959 RVA: 0x001043F6 File Offset: 0x001025F6
	public void ObjectSpawned(SpawnPointInstance instance)
	{
		this.spawnInstances.Add(instance);
	}

	// Token: 0x06002AD0 RID: 10960 RVA: 0x00104404 File Offset: 0x00102604
	public void ObjectRetired(SpawnPointInstance instance)
	{
		this.spawnInstances.Remove(instance);
	}

	// Token: 0x06002AD1 RID: 10961 RVA: 0x00104413 File Offset: 0x00102613
	public void DelayedSpawn()
	{
		base.Invoke(new Action(this.Spawn), 1f);
	}

	// Token: 0x06002AD2 RID: 10962 RVA: 0x0010442C File Offset: 0x0010262C
	public void Spawn()
	{
		if (this.isSpawnerActive)
		{
			this.Spawn(UnityEngine.Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1));
		}
	}

	// Token: 0x06002AD3 RID: 10963 RVA: 0x00104450 File Offset: 0x00102650
	protected virtual void Spawn(int numToSpawn)
	{
		numToSpawn = Mathf.Min(numToSpawn, this.maxPopulation - this.currentPopulation);
		for (int i = 0; i < numToSpawn; i++)
		{
			GameObjectRef prefab = this.GetPrefab();
			if (prefab != null && !string.IsNullOrEmpty(prefab.guid))
			{
				Vector3 pos;
				Quaternion rot;
				BaseSpawnPoint spawnPoint = this.GetSpawnPoint(prefab, out pos, out rot);
				if (spawnPoint)
				{
					BaseEntity baseEntity = GameManager.server.CreateEntity(prefab.resourcePath, pos, rot, false);
					if (baseEntity)
					{
						if (baseEntity.enableSaving && !(spawnPoint is SpaceCheckingSpawnPoint))
						{
							baseEntity.enableSaving = false;
						}
						baseEntity.gameObject.AwakeFromInstantiate();
						baseEntity.Spawn();
						this.PostSpawnProcess(baseEntity, spawnPoint);
						SpawnPointInstance spawnPointInstance = baseEntity.gameObject.AddComponent<SpawnPointInstance>();
						spawnPointInstance.parentSpawnPointUser = this;
						spawnPointInstance.parentSpawnPoint = spawnPoint;
						spawnPointInstance.Notify();
					}
				}
			}
		}
	}

	// Token: 0x06002AD4 RID: 10964 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
	{
	}

	// Token: 0x06002AD5 RID: 10965 RVA: 0x0010452C File Offset: 0x0010272C
	protected GameObjectRef GetPrefab()
	{
		float num = (float)this.prefabs.Sum(delegate(SpawnGroup.SpawnEntry x)
		{
			if (!this.preventDuplicates || !this.HasSpawned(x.prefab.resourceID))
			{
				return x.weight;
			}
			return 0;
		});
		if (num == 0f)
		{
			return null;
		}
		float num2 = UnityEngine.Random.Range(0f, num);
		foreach (SpawnGroup.SpawnEntry spawnEntry in this.prefabs)
		{
			int num3 = (this.preventDuplicates && this.HasSpawned(spawnEntry.prefab.resourceID)) ? 0 : spawnEntry.weight;
			if ((num2 -= (float)num3) <= 0f)
			{
				return spawnEntry.prefab;
			}
		}
		return this.prefabs[this.prefabs.Count - 1].prefab;
	}

	// Token: 0x06002AD6 RID: 10966 RVA: 0x00104608 File Offset: 0x00102808
	protected virtual BaseSpawnPoint GetSpawnPoint(GameObjectRef prefabRef, out Vector3 pos, out Quaternion rot)
	{
		BaseSpawnPoint baseSpawnPoint = null;
		pos = Vector3.zero;
		rot = Quaternion.identity;
		int num = UnityEngine.Random.Range(0, this.spawnPoints.Length);
		for (int i = 0; i < this.spawnPoints.Length; i++)
		{
			BaseSpawnPoint baseSpawnPoint2 = this.spawnPoints[(num + i) % this.spawnPoints.Length];
			if (!(baseSpawnPoint2 == null) && baseSpawnPoint2.IsAvailableTo(prefabRef) && !baseSpawnPoint2.HasPlayersIntersecting())
			{
				baseSpawnPoint = baseSpawnPoint2;
				break;
			}
		}
		if (baseSpawnPoint)
		{
			baseSpawnPoint.GetLocation(out pos, out rot);
		}
		return baseSpawnPoint;
	}

	// Token: 0x06002AD7 RID: 10967 RVA: 0x00104692 File Offset: 0x00102892
	private MonumentInfo FindMonument()
	{
		return base.GetComponentInParent<MonumentInfo>();
	}

	// Token: 0x06002AD8 RID: 10968 RVA: 0x0010469A File Offset: 0x0010289A
	protected virtual void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 1f, 0f, 1f);
		Gizmos.DrawSphere(base.transform.position, 0.25f);
	}

	// Token: 0x02000D51 RID: 3409
	[Serializable]
	public class SpawnEntry
	{
		// Token: 0x040046F7 RID: 18167
		public GameObjectRef prefab;

		// Token: 0x040046F8 RID: 18168
		public int weight = 1;

		// Token: 0x040046F9 RID: 18169
		public bool mobile;
	}
}
