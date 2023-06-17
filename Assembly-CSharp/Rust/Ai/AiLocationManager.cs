using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	// Token: 0x02000B43 RID: 2883
	public class AiLocationManager : FacepunchBehaviour, IServerComponent
	{
		// Token: 0x04003E57 RID: 15959
		public static List<AiLocationManager> Managers = new List<AiLocationManager>();

		// Token: 0x04003E58 RID: 15960
		[SerializeField]
		public AiLocationSpawner MainSpawner;

		// Token: 0x04003E59 RID: 15961
		[SerializeField]
		public AiLocationSpawner.SquadSpawnerLocation LocationWhenMainSpawnerIsNull = AiLocationSpawner.SquadSpawnerLocation.None;

		// Token: 0x04003E5A RID: 15962
		public Transform CoverPointGroup;

		// Token: 0x04003E5B RID: 15963
		public Transform PatrolPointGroup;

		// Token: 0x04003E5C RID: 15964
		public CoverPointVolume DynamicCoverPointVolume;

		// Token: 0x04003E5D RID: 15965
		public bool SnapCoverPointsToGround;

		// Token: 0x04003E5E RID: 15966
		private List<PathInterestNode> patrolPoints;

		// Token: 0x17000661 RID: 1633
		// (get) Token: 0x060045E0 RID: 17888 RVA: 0x00197FB1 File Offset: 0x001961B1
		public AiLocationSpawner.SquadSpawnerLocation LocationType
		{
			get
			{
				if (this.MainSpawner != null)
				{
					return this.MainSpawner.Location;
				}
				return this.LocationWhenMainSpawnerIsNull;
			}
		}

		// Token: 0x060045E1 RID: 17889 RVA: 0x00197FD4 File Offset: 0x001961D4
		private void Awake()
		{
			AiLocationManager.Managers.Add(this);
			if (this.SnapCoverPointsToGround)
			{
				foreach (AICoverPoint aicoverPoint in this.CoverPointGroup.GetComponentsInChildren<AICoverPoint>())
				{
					NavMeshHit navMeshHit;
					if (NavMesh.SamplePosition(aicoverPoint.transform.position, out navMeshHit, 4f, -1))
					{
						aicoverPoint.transform.position = navMeshHit.position;
					}
				}
			}
		}

		// Token: 0x060045E2 RID: 17890 RVA: 0x0019803E File Offset: 0x0019623E
		private void OnDestroy()
		{
			AiLocationManager.Managers.Remove(this);
		}

		// Token: 0x060045E3 RID: 17891 RVA: 0x0019804C File Offset: 0x0019624C
		public PathInterestNode GetFirstPatrolPointInRange(Vector3 from, float minRange = 10f, float maxRange = 100f)
		{
			if (this.PatrolPointGroup == null)
			{
				return null;
			}
			if (this.patrolPoints == null)
			{
				this.patrolPoints = new List<PathInterestNode>(this.PatrolPointGroup.GetComponentsInChildren<PathInterestNode>());
			}
			if (this.patrolPoints.Count == 0)
			{
				return null;
			}
			float num = minRange * minRange;
			float num2 = maxRange * maxRange;
			foreach (PathInterestNode pathInterestNode in this.patrolPoints)
			{
				float sqrMagnitude = (pathInterestNode.transform.position - from).sqrMagnitude;
				if (sqrMagnitude >= num && sqrMagnitude <= num2)
				{
					return pathInterestNode;
				}
			}
			return null;
		}

		// Token: 0x060045E4 RID: 17892 RVA: 0x0019810C File Offset: 0x0019630C
		public PathInterestNode GetRandomPatrolPointInRange(Vector3 from, float minRange = 10f, float maxRange = 100f, PathInterestNode currentPatrolPoint = null)
		{
			if (this.PatrolPointGroup == null)
			{
				return null;
			}
			if (this.patrolPoints == null)
			{
				this.patrolPoints = new List<PathInterestNode>(this.PatrolPointGroup.GetComponentsInChildren<PathInterestNode>());
			}
			if (this.patrolPoints.Count == 0)
			{
				return null;
			}
			float num = minRange * minRange;
			float num2 = maxRange * maxRange;
			for (int i = 0; i < 20; i++)
			{
				PathInterestNode pathInterestNode = this.patrolPoints[UnityEngine.Random.Range(0, this.patrolPoints.Count)];
				if (UnityEngine.Time.time < pathInterestNode.NextVisitTime)
				{
					if (pathInterestNode == currentPatrolPoint)
					{
						return null;
					}
				}
				else
				{
					float sqrMagnitude = (pathInterestNode.transform.position - from).sqrMagnitude;
					if (sqrMagnitude >= num && sqrMagnitude <= num2)
					{
						pathInterestNode.NextVisitTime = UnityEngine.Time.time + AI.npc_patrol_point_cooldown;
						return pathInterestNode;
					}
				}
			}
			return null;
		}
	}
}
