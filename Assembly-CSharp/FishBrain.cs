using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001E8 RID: 488
public class FishBrain : BaseAIBrain
{
	// Token: 0x0400125C RID: 4700
	public static int Count;

	// Token: 0x060019B7 RID: 6583 RVA: 0x000BBA60 File Offset: 0x000B9C60
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new FishBrain.IdleState());
		base.AddState(new FishBrain.RoamState());
		base.AddState(new BaseAIBrain.BaseFleeState());
		base.AddState(new BaseAIBrain.BaseChaseState());
		base.AddState(new BaseAIBrain.BaseMoveTorwardsState());
		base.AddState(new BaseAIBrain.BaseAttackState());
		base.AddState(new BaseAIBrain.BaseCooldownState());
	}

	// Token: 0x060019B8 RID: 6584 RVA: 0x000BBAC0 File Offset: 0x000B9CC0
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new UnderwaterPathFinder();
		((UnderwaterPathFinder)base.PathFinder).Init(this.GetBaseEntity());
		FishBrain.Count++;
	}

	// Token: 0x060019B9 RID: 6585 RVA: 0x000BBB12 File Offset: 0x000B9D12
	public override void OnDestroy()
	{
		base.OnDestroy();
		FishBrain.Count--;
	}

	// Token: 0x02000C44 RID: 3140
	public class IdleState : BaseAIBrain.BaseIdleState
	{
		// Token: 0x040042A8 RID: 17064
		private StateStatus status = StateStatus.Error;

		// Token: 0x040042A9 RID: 17065
		private List<Vector3> idlePoints;

		// Token: 0x040042AA RID: 17066
		private int currentPointIndex;

		// Token: 0x040042AB RID: 17067
		private Vector3 idleRootPos;

		// Token: 0x06004E4A RID: 20042 RVA: 0x001A2952 File Offset: 0x001A0B52
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004E4B RID: 20043 RVA: 0x001A2964 File Offset: 0x001A0B64
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.idleRootPos = brain.Navigator.transform.position;
			this.GenerateIdlePoints(20f, 0f);
			this.currentPointIndex = 0;
			this.status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			if (brain.Navigator.SetDestination(this.idleRootPos + this.idlePoints[0], BaseNavigator.NavigationSpeed.Normal, 0f, 0f))
			{
				this.status = StateStatus.Running;
				return;
			}
			this.status = StateStatus.Error;
		}

		// Token: 0x06004E4C RID: 20044 RVA: 0x0019EEA1 File Offset: 0x0019D0A1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004E4D RID: 20045 RVA: 0x001A29F4 File Offset: 0x001A0BF4
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (Vector3.Distance(brain.Navigator.transform.position, this.idleRootPos + this.idlePoints[this.currentPointIndex]) < 4f)
			{
				this.currentPointIndex++;
			}
			if (this.currentPointIndex >= this.idlePoints.Count)
			{
				this.currentPointIndex = 0;
			}
			if (brain.Navigator.SetDestination(this.idleRootPos + this.idlePoints[this.currentPointIndex], BaseNavigator.NavigationSpeed.Normal, 0f, 0f))
			{
				this.status = StateStatus.Running;
			}
			else
			{
				this.status = StateStatus.Error;
			}
			return this.status;
		}

		// Token: 0x06004E4E RID: 20046 RVA: 0x001A2AB8 File Offset: 0x001A0CB8
		private void GenerateIdlePoints(float radius, float heightOffset)
		{
			if (this.idlePoints != null)
			{
				return;
			}
			this.idlePoints = new List<Vector3>();
			float num = 0f;
			int num2 = 32;
			float height = TerrainMeta.WaterMap.GetHeight(this.brain.Navigator.transform.position);
			float height2 = TerrainMeta.HeightMap.GetHeight(this.brain.Navigator.transform.position);
			for (int i = 0; i < num2; i++)
			{
				num += 360f / (float)num2;
				Vector3 pointOnCircle = BasePathFinder.GetPointOnCircle(Vector3.zero, radius, num);
				pointOnCircle.y += UnityEngine.Random.Range(-heightOffset, heightOffset);
				pointOnCircle.y = Mathf.Clamp(pointOnCircle.y, height2, height);
				this.idlePoints.Add(pointOnCircle);
			}
		}
	}

	// Token: 0x02000C45 RID: 3141
	public class RoamState : BaseAIBrain.BasicAIState
	{
		// Token: 0x040042AC RID: 17068
		private StateStatus status = StateStatus.Error;

		// Token: 0x06004E50 RID: 20048 RVA: 0x001A2B8F File Offset: 0x001A0D8F
		public RoamState() : base(AIState.Roam)
		{
		}

		// Token: 0x06004E51 RID: 20049 RVA: 0x001A2B9F File Offset: 0x001A0D9F
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004E52 RID: 20050 RVA: 0x001A2BB0 File Offset: 0x001A0DB0
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			Vector3 fallbackPos = brain.Events.Memory.Position.Get(4);
			Vector3 bestRoamPosition = brain.PathFinder.GetBestRoamPosition(brain.Navigator, fallbackPos, 5f, brain.Navigator.MaxRoamDistanceFromHome);
			if (brain.Navigator.SetDestination(bestRoamPosition, BaseNavigator.NavigationSpeed.Normal, 0f, 0f))
			{
				this.status = StateStatus.Running;
				return;
			}
			this.status = StateStatus.Error;
		}

		// Token: 0x06004E53 RID: 20051 RVA: 0x0019EEA1 File Offset: 0x0019D0A1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004E54 RID: 20052 RVA: 0x001A2C37 File Offset: 0x001A0E37
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (this.status == StateStatus.Error)
			{
				return this.status;
			}
			if (brain.Navigator.Moving)
			{
				return StateStatus.Running;
			}
			return StateStatus.Finished;
		}
	}
}
