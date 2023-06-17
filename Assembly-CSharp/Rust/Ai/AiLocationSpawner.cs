using System;
using ConVar;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B44 RID: 2884
	public class AiLocationSpawner : SpawnGroup
	{
		// Token: 0x04003E5F RID: 15967
		public AiLocationSpawner.SquadSpawnerLocation Location;

		// Token: 0x04003E60 RID: 15968
		public AiLocationManager Manager;

		// Token: 0x04003E61 RID: 15969
		public JunkPile Junkpile;

		// Token: 0x04003E62 RID: 15970
		public bool IsMainSpawner = true;

		// Token: 0x04003E63 RID: 15971
		public float chance = 1f;

		// Token: 0x04003E64 RID: 15972
		private int defaultMaxPopulation;

		// Token: 0x04003E65 RID: 15973
		private int defaultNumToSpawnPerTickMax;

		// Token: 0x04003E66 RID: 15974
		private int defaultNumToSpawnPerTickMin;

		// Token: 0x060045E7 RID: 17895 RVA: 0x001981F8 File Offset: 0x001963F8
		public override void SpawnInitial()
		{
			if (this.IsMainSpawner)
			{
				if (this.Location == AiLocationSpawner.SquadSpawnerLocation.MilitaryTunnels)
				{
					this.maxPopulation = AI.npc_max_population_military_tunnels;
					this.numToSpawnPerTickMax = AI.npc_spawn_per_tick_max_military_tunnels;
					this.numToSpawnPerTickMin = AI.npc_spawn_per_tick_min_military_tunnels;
					this.respawnDelayMax = AI.npc_respawn_delay_max_military_tunnels;
					this.respawnDelayMin = AI.npc_respawn_delay_min_military_tunnels;
				}
				else
				{
					this.defaultMaxPopulation = this.maxPopulation;
					this.defaultNumToSpawnPerTickMax = this.numToSpawnPerTickMax;
					this.defaultNumToSpawnPerTickMin = this.numToSpawnPerTickMin;
				}
			}
			else
			{
				this.defaultMaxPopulation = this.maxPopulation;
				this.defaultNumToSpawnPerTickMax = this.numToSpawnPerTickMax;
				this.defaultNumToSpawnPerTickMin = this.numToSpawnPerTickMin;
			}
			base.SpawnInitial();
		}

		// Token: 0x060045E8 RID: 17896 RVA: 0x001982A0 File Offset: 0x001964A0
		protected override void Spawn(int numToSpawn)
		{
			if (!AI.npc_enable)
			{
				this.maxPopulation = 0;
				this.numToSpawnPerTickMax = 0;
				this.numToSpawnPerTickMin = 0;
				return;
			}
			if (numToSpawn == 0)
			{
				if (this.IsMainSpawner)
				{
					if (this.Location == AiLocationSpawner.SquadSpawnerLocation.MilitaryTunnels)
					{
						this.maxPopulation = AI.npc_max_population_military_tunnels;
						this.numToSpawnPerTickMax = AI.npc_spawn_per_tick_max_military_tunnels;
						this.numToSpawnPerTickMin = AI.npc_spawn_per_tick_min_military_tunnels;
						numToSpawn = UnityEngine.Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1);
					}
					else
					{
						this.maxPopulation = this.defaultMaxPopulation;
						this.numToSpawnPerTickMax = this.defaultNumToSpawnPerTickMax;
						this.numToSpawnPerTickMin = this.defaultNumToSpawnPerTickMin;
						numToSpawn = UnityEngine.Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1);
					}
				}
				else
				{
					this.maxPopulation = this.defaultMaxPopulation;
					this.numToSpawnPerTickMax = this.defaultNumToSpawnPerTickMax;
					this.numToSpawnPerTickMin = this.defaultNumToSpawnPerTickMin;
					numToSpawn = UnityEngine.Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1);
				}
			}
			float num = this.chance;
			AiLocationSpawner.SquadSpawnerLocation location = this.Location;
			if (location != AiLocationSpawner.SquadSpawnerLocation.JunkpileA)
			{
				if (location == AiLocationSpawner.SquadSpawnerLocation.JunkpileG)
				{
					num = AI.npc_junkpile_g_spawn_chance;
				}
			}
			else
			{
				num = AI.npc_junkpile_a_spawn_chance;
			}
			if (numToSpawn == 0 || UnityEngine.Random.value > num)
			{
				return;
			}
			numToSpawn = Mathf.Min(numToSpawn, this.maxPopulation - base.currentPopulation);
			for (int i = 0; i < numToSpawn; i++)
			{
				GameObjectRef prefab = base.GetPrefab();
				Vector3 pos;
				Quaternion rot;
				BaseSpawnPoint spawnPoint = this.GetSpawnPoint(prefab, out pos, out rot);
				if (spawnPoint)
				{
					BaseEntity baseEntity = GameManager.server.CreateEntity(prefab.resourcePath, pos, rot, true);
					if (baseEntity)
					{
						baseEntity.Spawn();
						SpawnPointInstance spawnPointInstance = baseEntity.gameObject.AddComponent<SpawnPointInstance>();
						spawnPointInstance.parentSpawnPointUser = this;
						spawnPointInstance.parentSpawnPoint = spawnPoint;
						spawnPointInstance.Notify();
					}
				}
			}
		}

		// Token: 0x060045E9 RID: 17897 RVA: 0x00198447 File Offset: 0x00196647
		protected override BaseSpawnPoint GetSpawnPoint(GameObjectRef prefabRef, out Vector3 pos, out Quaternion rot)
		{
			return base.GetSpawnPoint(prefabRef, out pos, out rot);
		}

		// Token: 0x02000F9F RID: 3999
		public enum SquadSpawnerLocation
		{
			// Token: 0x04005077 RID: 20599
			MilitaryTunnels,
			// Token: 0x04005078 RID: 20600
			JunkpileA,
			// Token: 0x04005079 RID: 20601
			JunkpileG,
			// Token: 0x0400507A RID: 20602
			CH47,
			// Token: 0x0400507B RID: 20603
			None,
			// Token: 0x0400507C RID: 20604
			Compound,
			// Token: 0x0400507D RID: 20605
			BanditTown,
			// Token: 0x0400507E RID: 20606
			CargoShip
		}
	}
}
