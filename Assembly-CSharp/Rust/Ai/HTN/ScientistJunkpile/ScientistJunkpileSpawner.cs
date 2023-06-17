using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile
{
	// Token: 0x02000B46 RID: 2886
	public class ScientistJunkpileSpawner : MonoBehaviour, IServerComponent, ISpawnGroup
	{
		// Token: 0x04003E69 RID: 15977
		public GameObjectRef ScientistPrefab;

		// Token: 0x04003E6A RID: 15978
		[NonSerialized]
		public List<BaseCombatEntity> Spawned = new List<BaseCombatEntity>();

		// Token: 0x04003E6B RID: 15979
		[NonSerialized]
		public BaseSpawnPoint[] SpawnPoints;

		// Token: 0x04003E6C RID: 15980
		public int MaxPopulation = 1;

		// Token: 0x04003E6D RID: 15981
		public bool InitialSpawn;

		// Token: 0x04003E6E RID: 15982
		public float MinRespawnTimeMinutes = 120f;

		// Token: 0x04003E6F RID: 15983
		public float MaxRespawnTimeMinutes = 120f;

		// Token: 0x04003E70 RID: 15984
		public float MovementRadius = -1f;

		// Token: 0x04003E71 RID: 15985
		public bool ReducedLongRangeAccuracy;

		// Token: 0x04003E72 RID: 15986
		public ScientistJunkpileSpawner.JunkpileType SpawnType;

		// Token: 0x04003E73 RID: 15987
		[Range(0f, 1f)]
		public float SpawnBaseChance = 1f;

		// Token: 0x04003E74 RID: 15988
		private float nextRespawnTime;

		// Token: 0x04003E75 RID: 15989
		private bool pendingRespawn;

		// Token: 0x17000664 RID: 1636
		// (get) Token: 0x060045F0 RID: 17904 RVA: 0x001985F3 File Offset: 0x001967F3
		public int currentPopulation
		{
			get
			{
				return this.Spawned.Count;
			}
		}

		// Token: 0x060045F1 RID: 17905 RVA: 0x00198600 File Offset: 0x00196800
		private void Awake()
		{
			this.SpawnPoints = base.GetComponentsInChildren<BaseSpawnPoint>();
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Add(this);
			}
		}

		// Token: 0x060045F2 RID: 17906 RVA: 0x0019862A File Offset: 0x0019682A
		protected void OnDestroy()
		{
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Remove(this);
				return;
			}
			Debug.LogWarning(base.GetType().Name + ": SpawnHandler instance not found.");
		}

		// Token: 0x060045F3 RID: 17907 RVA: 0x00198664 File Offset: 0x00196864
		public void Fill()
		{
			this.DoRespawn();
		}

		// Token: 0x060045F4 RID: 17908 RVA: 0x0019866C File Offset: 0x0019686C
		public void Clear()
		{
			if (this.Spawned == null)
			{
				return;
			}
			foreach (BaseCombatEntity baseCombatEntity in this.Spawned)
			{
				if (!(baseCombatEntity == null) && !(baseCombatEntity.gameObject == null) && !(baseCombatEntity.transform == null))
				{
					BaseEntity baseEntity = baseCombatEntity.gameObject.ToBaseEntity();
					if (baseEntity)
					{
						baseEntity.Kill(BaseNetworkable.DestroyMode.None);
					}
				}
			}
			this.Spawned.Clear();
		}

		// Token: 0x060045F5 RID: 17909 RVA: 0x0019870C File Offset: 0x0019690C
		public void SpawnInitial()
		{
			this.nextRespawnTime = UnityEngine.Time.time + UnityEngine.Random.Range(3f, 4f);
			this.pendingRespawn = true;
		}

		// Token: 0x060045F6 RID: 17910 RVA: 0x00198730 File Offset: 0x00196930
		public void SpawnRepeating()
		{
			this.CheckIfRespawnNeeded();
		}

		// Token: 0x060045F7 RID: 17911 RVA: 0x00198738 File Offset: 0x00196938
		public void CheckIfRespawnNeeded()
		{
			if (!this.pendingRespawn)
			{
				if (this.Spawned == null || this.Spawned.Count == 0 || this.IsAllSpawnedDead())
				{
					this.ScheduleRespawn();
					return;
				}
			}
			else if ((this.Spawned == null || this.Spawned.Count == 0 || this.IsAllSpawnedDead()) && UnityEngine.Time.time >= this.nextRespawnTime)
			{
				this.DoRespawn();
			}
		}

		// Token: 0x060045F8 RID: 17912 RVA: 0x001987A4 File Offset: 0x001969A4
		private bool IsAllSpawnedDead()
		{
			for (int i = 0; i < this.Spawned.Count; i++)
			{
				BaseCombatEntity baseCombatEntity = this.Spawned[i];
				if (!(baseCombatEntity == null) && !(baseCombatEntity.transform == null) && !baseCombatEntity.IsDestroyed && !baseCombatEntity.IsDead())
				{
					return false;
				}
				this.Spawned.RemoveAt(i);
				i--;
			}
			return true;
		}

		// Token: 0x060045F9 RID: 17913 RVA: 0x00198810 File Offset: 0x00196A10
		public void ScheduleRespawn()
		{
			this.nextRespawnTime = UnityEngine.Time.time + UnityEngine.Random.Range(this.MinRespawnTimeMinutes, this.MaxRespawnTimeMinutes) * 60f;
			this.pendingRespawn = true;
		}

		// Token: 0x060045FA RID: 17914 RVA: 0x0019883C File Offset: 0x00196A3C
		public void DoRespawn()
		{
			if (!Application.isLoading && !Application.isLoadingSave)
			{
				this.SpawnScientist();
			}
			this.pendingRespawn = false;
		}

		// Token: 0x060045FB RID: 17915 RVA: 0x0019885C File Offset: 0x00196A5C
		public void SpawnScientist()
		{
			if (!AI.npc_enable)
			{
				return;
			}
			if (this.Spawned == null || this.Spawned.Count >= this.MaxPopulation)
			{
				return;
			}
			float num = this.SpawnBaseChance;
			ScientistJunkpileSpawner.JunkpileType spawnType = this.SpawnType;
			if (spawnType != ScientistJunkpileSpawner.JunkpileType.A)
			{
				if (spawnType == ScientistJunkpileSpawner.JunkpileType.G)
				{
					num = AI.npc_junkpile_g_spawn_chance;
				}
			}
			else
			{
				num = AI.npc_junkpile_a_spawn_chance;
			}
			if (UnityEngine.Random.value > num)
			{
				return;
			}
			int num2 = this.MaxPopulation - this.Spawned.Count;
			for (int i = 0; i < num2; i++)
			{
				Vector3 pos;
				Quaternion rot;
				if (!(this.GetSpawnPoint(out pos, out rot) == null))
				{
					BaseEntity baseEntity = GameManager.server.CreateEntity(this.ScientistPrefab.resourcePath, pos, rot, false);
					if (!(baseEntity != null))
					{
						return;
					}
					baseEntity.enableSaving = false;
					baseEntity.gameObject.AwakeFromInstantiate();
					baseEntity.Spawn();
					this.Spawned.Add((BaseCombatEntity)baseEntity);
				}
			}
		}

		// Token: 0x060045FC RID: 17916 RVA: 0x00198944 File Offset: 0x00196B44
		private BaseSpawnPoint GetSpawnPoint(out Vector3 pos, out Quaternion rot)
		{
			BaseSpawnPoint baseSpawnPoint = null;
			pos = Vector3.zero;
			rot = Quaternion.identity;
			int num = UnityEngine.Random.Range(0, this.SpawnPoints.Length);
			for (int i = 0; i < this.SpawnPoints.Length; i++)
			{
				baseSpawnPoint = this.SpawnPoints[(num + i) % this.SpawnPoints.Length];
				if (baseSpawnPoint && baseSpawnPoint.gameObject.activeSelf)
				{
					break;
				}
			}
			if (baseSpawnPoint)
			{
				baseSpawnPoint.GetLocation(out pos, out rot);
			}
			return baseSpawnPoint;
		}

		// Token: 0x02000FA2 RID: 4002
		public enum JunkpileType
		{
			// Token: 0x04005087 RID: 20615
			A,
			// Token: 0x04005088 RID: 20616
			B,
			// Token: 0x04005089 RID: 20617
			C,
			// Token: 0x0400508A RID: 20618
			D,
			// Token: 0x0400508B RID: 20619
			E,
			// Token: 0x0400508C RID: 20620
			F,
			// Token: 0x0400508D RID: 20621
			G
		}
	}
}
