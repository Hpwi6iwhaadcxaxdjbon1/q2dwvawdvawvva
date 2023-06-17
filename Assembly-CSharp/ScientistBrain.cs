using System;
using UnityEngine;

// Token: 0x020001F5 RID: 501
public class ScientistBrain : BaseAIBrain
{
	// Token: 0x040012B4 RID: 4788
	public static int Count;

	// Token: 0x06001A44 RID: 6724 RVA: 0x000BDEA0 File Offset: 0x000BC0A0
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new ScientistBrain.IdleState());
		base.AddState(new ScientistBrain.RoamState());
		base.AddState(new ScientistBrain.ChaseState());
		base.AddState(new ScientistBrain.CombatState());
		base.AddState(new ScientistBrain.TakeCoverState());
		base.AddState(new ScientistBrain.CoverState());
		base.AddState(new ScientistBrain.MountedState());
		base.AddState(new ScientistBrain.DismountedState());
		base.AddState(new BaseAIBrain.BaseFollowPathState());
		base.AddState(new BaseAIBrain.BaseNavigateHomeState());
		base.AddState(new ScientistBrain.CombatStationaryState());
		base.AddState(new BaseAIBrain.BaseMoveTorwardsState());
		base.AddState(new ScientistBrain.MoveToVector3State());
		base.AddState(new ScientistBrain.BlindedState());
	}

	// Token: 0x06001A45 RID: 6725 RVA: 0x000BDF50 File Offset: 0x000BC150
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(this.GetBaseEntity());
		ScientistBrain.Count++;
	}

	// Token: 0x06001A46 RID: 6726 RVA: 0x000BDFA2 File Offset: 0x000BC1A2
	public override void OnDestroy()
	{
		base.OnDestroy();
		ScientistBrain.Count--;
	}

	// Token: 0x06001A47 RID: 6727 RVA: 0x000BDFB6 File Offset: 0x000BC1B6
	public HumanNPC GetEntity()
	{
		return this.GetBaseEntity() as HumanNPC;
	}

	// Token: 0x06001A48 RID: 6728 RVA: 0x000BDFC4 File Offset: 0x000BC1C4
	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		if (base.CurrentState != null)
		{
			AIState stateType = base.CurrentState.StateType;
			if (stateType <= AIState.Patrol)
			{
				if (stateType - AIState.Idle > 1 && stateType != AIState.Patrol)
				{
					goto IL_46;
				}
			}
			else if (stateType != AIState.FollowPath && stateType != AIState.Cooldown)
			{
				goto IL_46;
			}
			this.GetEntity().SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
			return;
			IL_46:
			this.GetEntity().SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, false);
		}
	}

	// Token: 0x02000C4D RID: 3149
	public class BlindedState : BaseAIBrain.BaseBlindedState
	{
		// Token: 0x06004E7B RID: 20091 RVA: 0x001A33C8 File Offset: 0x001A15C8
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			HumanNPC humanNPC = entity as HumanNPC;
			humanNPC.SetDucked(false);
			humanNPC.Server_StartGesture(235662700U);
			brain.Navigator.SetDestination(brain.PathFinder.GetRandomPositionAround(entity.transform.position, 1f, 2.5f), BaseNavigator.NavigationSpeed.Slowest, 0f, 0f);
		}

		// Token: 0x06004E7C RID: 20092 RVA: 0x001A342B File Offset: 0x001A162B
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
			if (entity.ToPlayer() != null)
			{
				entity.ToPlayer().Server_CancelGesture();
			}
		}
	}

	// Token: 0x02000C4E RID: 3150
	public class ChaseState : BaseAIBrain.BasicAIState
	{
		// Token: 0x040042BC RID: 17084
		private StateStatus status = StateStatus.Error;

		// Token: 0x040042BD RID: 17085
		private float nextPositionUpdateTime;

		// Token: 0x06004E7E RID: 20094 RVA: 0x001A3461 File Offset: 0x001A1661
		public ChaseState() : base(AIState.Chase)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004E7F RID: 20095 RVA: 0x001A3478 File Offset: 0x001A1678
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004E80 RID: 20096 RVA: 0x001A3488 File Offset: 0x001A1688
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			this.status = StateStatus.Running;
			this.nextPositionUpdateTime = 0f;
		}

		// Token: 0x06004E81 RID: 20097 RVA: 0x001A34B4 File Offset: 0x001A16B4
		private void Stop()
		{
			this.brain.Navigator.Stop();
			this.brain.Navigator.ClearFacingDirectionOverride();
		}

		// Token: 0x06004E82 RID: 20098 RVA: 0x001A34D8 File Offset: 0x001A16D8
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (this.status == StateStatus.Error)
			{
				return this.status;
			}
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				return StateStatus.Error;
			}
			float num = Vector3.Distance(baseEntity.transform.position, entity.transform.position);
			if (brain.Senses.Memory.IsLOS(baseEntity) || num <= 10f || base.TimeInState <= 5f)
			{
				brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
			else
			{
				brain.Navigator.ClearFacingDirectionOverride();
			}
			if (num <= 10f)
			{
				brain.Navigator.SetCurrentSpeed(BaseNavigator.NavigationSpeed.Normal);
			}
			else
			{
				brain.Navigator.SetCurrentSpeed(BaseNavigator.NavigationSpeed.Fast);
			}
			if (Time.time > this.nextPositionUpdateTime)
			{
				this.nextPositionUpdateTime = Time.time + UnityEngine.Random.Range(0.5f, 1f);
				Vector3 pos = entity.transform.position;
				AIInformationZone informationZone = (entity as HumanNPC).GetInformationZone(baseEntity.transform.position);
				bool flag = false;
				if (informationZone != null)
				{
					AIMovePoint bestMovePointNear = informationZone.GetBestMovePointNear(baseEntity.transform.position, entity.transform.position, 0f, brain.Navigator.BestMovementPointMaxDistance, true, entity, true);
					if (bestMovePointNear)
					{
						bestMovePointNear.SetUsedBy(entity, 5f);
						pos = brain.PathFinder.GetRandomPositionAround(bestMovePointNear.transform.position, 0f, bestMovePointNear.radius - 0.3f);
						flag = true;
					}
				}
				if (!flag)
				{
					return StateStatus.Error;
				}
				if (num < 10f)
				{
					brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Normal, 0f, 0f);
				}
				else
				{
					brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
				}
			}
			if (brain.Navigator.Moving)
			{
				return StateStatus.Running;
			}
			return StateStatus.Finished;
		}
	}

	// Token: 0x02000C4F RID: 3151
	public class CombatState : BaseAIBrain.BasicAIState
	{
		// Token: 0x040042BE RID: 17086
		private float nextActionTime;

		// Token: 0x040042BF RID: 17087
		private Vector3 combatStartPosition;

		// Token: 0x06004E83 RID: 20099 RVA: 0x001A36C8 File Offset: 0x001A18C8
		public CombatState() : base(AIState.Combat)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004E84 RID: 20100 RVA: 0x001A36D8 File Offset: 0x001A18D8
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.combatStartPosition = entity.transform.position;
			this.FaceTarget();
		}

		// Token: 0x06004E85 RID: 20101 RVA: 0x001A36F9 File Offset: 0x001A18F9
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			(entity as HumanNPC).SetDucked(false);
			brain.Navigator.ClearFacingDirectionOverride();
		}

		// Token: 0x06004E86 RID: 20102 RVA: 0x001A371C File Offset: 0x001A191C
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			HumanNPC humanNPC = entity as HumanNPC;
			this.FaceTarget();
			if (Time.time > this.nextActionTime)
			{
				if (UnityEngine.Random.Range(0, 3) == 1)
				{
					this.nextActionTime = Time.time + UnityEngine.Random.Range(1f, 2f);
					humanNPC.SetDucked(true);
					brain.Navigator.Stop();
				}
				else
				{
					this.nextActionTime = Time.time + UnityEngine.Random.Range(2f, 3f);
					humanNPC.SetDucked(false);
					brain.Navigator.SetDestination(brain.PathFinder.GetRandomPositionAround(this.combatStartPosition, 1f, 2f), BaseNavigator.NavigationSpeed.Normal, 0f, 0f);
				}
			}
			return StateStatus.Running;
		}

		// Token: 0x06004E87 RID: 20103 RVA: 0x001A37E0 File Offset: 0x001A19E0
		private void FaceTarget()
		{
			BaseEntity baseEntity = this.brain.Events.Memory.Entity.Get(this.brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.brain.Navigator.ClearFacingDirectionOverride();
				return;
			}
			this.brain.Navigator.SetFacingDirectionEntity(baseEntity);
		}
	}

	// Token: 0x02000C50 RID: 3152
	public class CombatStationaryState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004E88 RID: 20104 RVA: 0x001A3843 File Offset: 0x001A1A43
		public CombatStationaryState() : base(AIState.CombatStationary)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004E89 RID: 20105 RVA: 0x001A2736 File Offset: 0x001A0936
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
		}

		// Token: 0x06004E8A RID: 20106 RVA: 0x001A3854 File Offset: 0x001A1A54
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
			else
			{
				brain.Navigator.ClearFacingDirectionOverride();
			}
			return StateStatus.Running;
		}
	}

	// Token: 0x02000C51 RID: 3153
	public class CoverState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004E8B RID: 20107 RVA: 0x001A38AF File Offset: 0x001A1AAF
		public CoverState() : base(AIState.Cover)
		{
		}

		// Token: 0x06004E8C RID: 20108 RVA: 0x001A38B8 File Offset: 0x001A1AB8
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			HumanNPC humanNPC = entity as HumanNPC;
			humanNPC.SetDucked(true);
			AIPoint aipoint = brain.Events.Memory.AIPoint.Get(4);
			if (aipoint != null)
			{
				aipoint.SetUsedBy(entity);
			}
			if (humanNPC.healthFraction <= brain.HealBelowHealthFraction && UnityEngine.Random.Range(0f, 1f) <= brain.HealChance)
			{
				Item item = humanNPC.FindHealingItem();
				if (item != null)
				{
					BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
					if (baseEntity == null || (!brain.Senses.Memory.IsLOS(baseEntity) && Vector3.Distance(entity.transform.position, baseEntity.transform.position) >= 5f))
					{
						humanNPC.UseHealingItem(item);
					}
				}
			}
		}

		// Token: 0x06004E8D RID: 20109 RVA: 0x001A399C File Offset: 0x001A1B9C
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			(entity as HumanNPC).SetDucked(false);
			brain.Navigator.ClearFacingDirectionOverride();
			AIPoint aipoint = brain.Events.Memory.AIPoint.Get(4);
			if (aipoint != null)
			{
				aipoint.ClearIfUsedBy(entity);
			}
		}

		// Token: 0x06004E8E RID: 20110 RVA: 0x001A39F0 File Offset: 0x001A1BF0
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			HumanNPC humanNPC = entity as HumanNPC;
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			float num = humanNPC.AmmoFractionRemaining();
			if (num == 0f || (baseEntity != null && !brain.Senses.Memory.IsLOS(baseEntity) && num < 0.25f))
			{
				humanNPC.AttemptReload();
			}
			if (baseEntity != null)
			{
				brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
			return StateStatus.Running;
		}
	}

	// Token: 0x02000C52 RID: 3154
	public class DismountedState : BaseAIBrain.BaseDismountedState
	{
		// Token: 0x040042C0 RID: 17088
		private StateStatus status = StateStatus.Error;

		// Token: 0x06004E8F RID: 20111 RVA: 0x001A3A88 File Offset: 0x001A1C88
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			AIInformationZone informationZone = (entity as HumanNPC).GetInformationZone(entity.transform.position);
			if (informationZone == null)
			{
				return;
			}
			AICoverPoint bestCoverPoint = informationZone.GetBestCoverPoint(entity.transform.position, entity.transform.position, 25f, 50f, entity, true);
			if (bestCoverPoint)
			{
				bestCoverPoint.SetUsedBy(entity, 10f);
			}
			Vector3 pos = (bestCoverPoint == null) ? entity.transform.position : bestCoverPoint.transform.position;
			if (brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Fast, 0f, 0f))
			{
				this.status = StateStatus.Running;
			}
		}

		// Token: 0x06004E90 RID: 20112 RVA: 0x001A3B4D File Offset: 0x001A1D4D
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

	// Token: 0x02000C53 RID: 3155
	public class IdleState : BaseAIBrain.BaseIdleState
	{
	}

	// Token: 0x02000C54 RID: 3156
	public class MountedState : BaseAIBrain.BaseMountedState
	{
	}

	// Token: 0x02000C55 RID: 3157
	public class MoveToVector3State : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004E94 RID: 20116 RVA: 0x001A3B98 File Offset: 0x001A1D98
		public MoveToVector3State() : base(AIState.MoveToVector3)
		{
		}

		// Token: 0x06004E95 RID: 20117 RVA: 0x001A3BA2 File Offset: 0x001A1DA2
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004E96 RID: 20118 RVA: 0x001A34B4 File Offset: 0x001A16B4
		private void Stop()
		{
			this.brain.Navigator.Stop();
			this.brain.Navigator.ClearFacingDirectionOverride();
		}

		// Token: 0x06004E97 RID: 20119 RVA: 0x001A3BB4 File Offset: 0x001A1DB4
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			Vector3 pos = brain.Events.Memory.Position.Get(7);
			if (!brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Fast, 0.5f, 0f))
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}
	}

	// Token: 0x02000C56 RID: 3158
	public class RoamState : BaseAIBrain.BaseRoamState
	{
		// Token: 0x040042C1 RID: 17089
		private StateStatus status = StateStatus.Error;

		// Token: 0x040042C2 RID: 17090
		private AIMovePoint roamPoint;

		// Token: 0x06004E98 RID: 20120 RVA: 0x001A3C0D File Offset: 0x001A1E0D
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
			this.ClearRoamPointUsage(entity);
		}

		// Token: 0x06004E99 RID: 20121 RVA: 0x001A3C24 File Offset: 0x001A1E24
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			this.ClearRoamPointUsage(entity);
			if (brain.PathFinder == null)
			{
				return;
			}
			this.status = StateStatus.Error;
			this.roamPoint = brain.PathFinder.GetBestRoamPoint(this.GetRoamAnchorPosition(), entity.transform.position, (entity as HumanNPC).eyes.BodyForward(), brain.Navigator.MaxRoamDistanceFromHome, brain.Navigator.BestRoamPointMaxDistance);
			if (this.roamPoint != null)
			{
				if (brain.Navigator.SetDestination(this.roamPoint.transform.position, BaseNavigator.NavigationSpeed.Slow, 0f, 0f))
				{
					this.roamPoint.SetUsedBy(entity);
					this.status = StateStatus.Running;
					return;
				}
				this.roamPoint.SetUsedBy(entity, 600f);
			}
		}

		// Token: 0x06004E9A RID: 20122 RVA: 0x001A3CFA File Offset: 0x001A1EFA
		private void ClearRoamPointUsage(BaseEntity entity)
		{
			if (this.roamPoint != null)
			{
				this.roamPoint.ClearIfUsedBy(entity);
				this.roamPoint = null;
			}
		}

		// Token: 0x06004E9B RID: 20123 RVA: 0x0019EEA1 File Offset: 0x0019D0A1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004E9C RID: 20124 RVA: 0x001A3D1D File Offset: 0x001A1F1D
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			if (this.status == StateStatus.Error)
			{
				return this.status;
			}
			if (brain.Navigator.Moving)
			{
				return StateStatus.Running;
			}
			this.PickGoodLookDirection();
			return StateStatus.Finished;
		}

		// Token: 0x06004E9D RID: 20125 RVA: 0x000063A5 File Offset: 0x000045A5
		private void PickGoodLookDirection()
		{
		}
	}

	// Token: 0x02000C57 RID: 3159
	public class TakeCoverState : BaseAIBrain.BasicAIState
	{
		// Token: 0x040042C3 RID: 17091
		private StateStatus status = StateStatus.Error;

		// Token: 0x040042C4 RID: 17092
		private BaseEntity coverFromEntity;

		// Token: 0x06004E9F RID: 20127 RVA: 0x001A3D54 File Offset: 0x001A1F54
		public TakeCoverState() : base(AIState.TakeCover)
		{
		}

		// Token: 0x06004EA0 RID: 20128 RVA: 0x001A3D65 File Offset: 0x001A1F65
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Running;
			if (!this.StartMovingToCover(entity as HumanNPC))
			{
				this.status = StateStatus.Error;
			}
		}

		// Token: 0x06004EA1 RID: 20129 RVA: 0x001A3D8B File Offset: 0x001A1F8B
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
			this.ClearCoverPointUsage(entity);
		}

		// Token: 0x06004EA2 RID: 20130 RVA: 0x001A3DA8 File Offset: 0x001A1FA8
		private void ClearCoverPointUsage(BaseEntity entity)
		{
			AIPoint aipoint = this.brain.Events.Memory.AIPoint.Get(4);
			if (aipoint != null)
			{
				aipoint.ClearIfUsedBy(entity);
			}
		}

		// Token: 0x06004EA3 RID: 20131 RVA: 0x001A3DE4 File Offset: 0x001A1FE4
		private bool StartMovingToCover(HumanNPC entity)
		{
			this.coverFromEntity = this.brain.Events.Memory.Entity.Get(this.brain.Events.CurrentInputMemorySlot);
			Vector3 hideFromPosition = this.coverFromEntity ? this.coverFromEntity.transform.position : (entity.transform.position + entity.LastAttackedDir * 30f);
			AIInformationZone informationZone = entity.GetInformationZone(entity.transform.position);
			if (informationZone == null)
			{
				return false;
			}
			float minRange = (entity.SecondsSinceAttacked < 2f) ? 2f : 0f;
			float bestCoverPointMaxDistance = this.brain.Navigator.BestCoverPointMaxDistance;
			AICoverPoint bestCoverPoint = informationZone.GetBestCoverPoint(entity.transform.position, hideFromPosition, minRange, bestCoverPointMaxDistance, entity, true);
			if (bestCoverPoint == null)
			{
				return false;
			}
			Vector3 position = bestCoverPoint.transform.position;
			if (!this.brain.Navigator.SetDestination(position, BaseNavigator.NavigationSpeed.Normal, 0f, 0f))
			{
				return false;
			}
			this.FaceCoverFromEntity();
			this.brain.Events.Memory.AIPoint.Set(bestCoverPoint, 4);
			bestCoverPoint.SetUsedBy(entity);
			return true;
		}

		// Token: 0x06004EA4 RID: 20132 RVA: 0x001A3F28 File Offset: 0x001A2128
		public override void DrawGizmos()
		{
			base.DrawGizmos();
		}

		// Token: 0x06004EA5 RID: 20133 RVA: 0x001A3F30 File Offset: 0x001A2130
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			this.FaceCoverFromEntity();
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

		// Token: 0x06004EA6 RID: 20134 RVA: 0x001A3F64 File Offset: 0x001A2164
		private void FaceCoverFromEntity()
		{
			this.coverFromEntity = this.brain.Events.Memory.Entity.Get(this.brain.Events.CurrentInputMemorySlot);
			if (this.coverFromEntity == null)
			{
				return;
			}
			this.brain.Navigator.SetFacingDirectionEntity(this.coverFromEntity);
		}
	}
}
