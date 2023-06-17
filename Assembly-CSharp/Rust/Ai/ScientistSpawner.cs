using System;
using ConVar;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B42 RID: 2882
	public class ScientistSpawner : SpawnGroup
	{
		// Token: 0x04003E48 RID: 15944
		[Header("Scientist Spawner")]
		public bool Mobile = true;

		// Token: 0x04003E49 RID: 15945
		public bool NeverMove;

		// Token: 0x04003E4A RID: 15946
		public bool SpawnHostile;

		// Token: 0x04003E4B RID: 15947
		public bool OnlyAggroMarkedTargets = true;

		// Token: 0x04003E4C RID: 15948
		public bool IsPeacekeeper = true;

		// Token: 0x04003E4D RID: 15949
		public bool IsBandit;

		// Token: 0x04003E4E RID: 15950
		public bool IsMilitaryTunnelLab;

		// Token: 0x04003E4F RID: 15951
		public WaypointSet Waypoints;

		// Token: 0x04003E50 RID: 15952
		public Transform[] LookAtInterestPointsStationary;

		// Token: 0x04003E51 RID: 15953
		public Vector2 RadioEffectRepeatRange = new Vector2(10f, 15f);

		// Token: 0x04003E52 RID: 15954
		public Model Model;

		// Token: 0x04003E53 RID: 15955
		[SerializeField]
		private AiLocationManager _mgr;

		// Token: 0x04003E54 RID: 15956
		private float _nextForcedRespawn = float.PositiveInfinity;

		// Token: 0x04003E55 RID: 15957
		private bool _lastSpawnCallHadAliveMembers;

		// Token: 0x04003E56 RID: 15958
		private bool _lastSpawnCallHadMaxAliveMembers;

		// Token: 0x060045DC RID: 17884 RVA: 0x00197E2C File Offset: 0x0019602C
		protected override void Spawn(int numToSpawn)
		{
			if (!AI.npc_enable)
			{
				return;
			}
			if (base.currentPopulation == this.maxPopulation)
			{
				this._lastSpawnCallHadMaxAliveMembers = true;
				this._lastSpawnCallHadAliveMembers = true;
				return;
			}
			if (this._lastSpawnCallHadMaxAliveMembers)
			{
				this._nextForcedRespawn = UnityEngine.Time.time + 2200f;
			}
			if (UnityEngine.Time.time < this._nextForcedRespawn)
			{
				if (base.currentPopulation == 0 && this._lastSpawnCallHadAliveMembers)
				{
					this._lastSpawnCallHadMaxAliveMembers = false;
					this._lastSpawnCallHadAliveMembers = false;
					return;
				}
				if (base.currentPopulation > 0)
				{
					this._lastSpawnCallHadMaxAliveMembers = false;
					this._lastSpawnCallHadAliveMembers = (base.currentPopulation > 0);
					return;
				}
			}
			this._lastSpawnCallHadMaxAliveMembers = false;
			this._lastSpawnCallHadAliveMembers = (base.currentPopulation > 0);
			base.Spawn(numToSpawn);
		}

		// Token: 0x060045DD RID: 17885 RVA: 0x000063A5 File Offset: 0x000045A5
		protected override void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
		{
		}

		// Token: 0x060045DE RID: 17886 RVA: 0x00197EE0 File Offset: 0x001960E0
		protected override void OnDrawGizmos()
		{
			base.OnDrawGizmos();
			if (this.LookAtInterestPointsStationary != null && this.LookAtInterestPointsStationary.Length != 0)
			{
				Gizmos.color = Color.magenta - new Color(0f, 0f, 0f, 0.5f);
				foreach (Transform transform in this.LookAtInterestPointsStationary)
				{
					if (transform != null)
					{
						Gizmos.DrawSphere(transform.position, 0.1f);
						Gizmos.DrawLine(base.transform.position, transform.position);
					}
				}
			}
		}
	}
}
