using System;
using ConVar;
using UnityEngine;

// Token: 0x020001FC RID: 508
public class ScarecrowBrain : BaseAIBrain
{
	// Token: 0x06001A94 RID: 6804 RVA: 0x000BEEA7 File Offset: 0x000BD0A7
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new BaseAIBrain.BaseIdleState());
		base.AddState(new ScarecrowBrain.ChaseState());
		base.AddState(new ScarecrowBrain.AttackState());
		base.AddState(new ScarecrowBrain.RoamState());
		base.AddState(new BaseAIBrain.BaseFleeState());
	}

	// Token: 0x06001A95 RID: 6805 RVA: 0x000BCBCC File Offset: 0x000BADCC
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(this.GetBaseEntity());
	}

	// Token: 0x06001A96 RID: 6806 RVA: 0x000BCC07 File Offset: 0x000BAE07
	public override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x02000C5C RID: 3164
	public class AttackState : BaseAIBrain.BasicAIState
	{
		// Token: 0x040042CF RID: 17103
		private IAIAttack attack;

		// Token: 0x040042D0 RID: 17104
		private float originalStoppingDistance;

		// Token: 0x06004EB6 RID: 20150 RVA: 0x0019EC12 File Offset: 0x0019CE12
		public AttackState() : base(AIState.Attack)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004EB7 RID: 20151 RVA: 0x001A4208 File Offset: 0x001A2408
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			entity.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
			this.originalStoppingDistance = brain.Navigator.StoppingDistance;
			brain.Navigator.Agent.stoppingDistance = 1f;
			brain.Navigator.StoppingDistance = 1f;
			base.StateEnter(brain, entity);
			this.attack = (entity as IAIAttack);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				Vector3 aimDirection = ScarecrowBrain.AttackState.GetAimDirection(brain.Navigator.transform.position, baseEntity.transform.position);
				brain.Navigator.SetFacingDirectionOverride(aimDirection);
				if (this.attack.CanAttack(baseEntity))
				{
					this.StartAttacking(baseEntity);
				}
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x06004EB8 RID: 20152 RVA: 0x001A42FC File Offset: 0x001A24FC
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			entity.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
			brain.Navigator.Agent.stoppingDistance = this.originalStoppingDistance;
			brain.Navigator.StoppingDistance = this.originalStoppingDistance;
			brain.Navigator.ClearFacingDirectionOverride();
			brain.Navigator.Stop();
			this.StopAttacking();
		}

		// Token: 0x06004EB9 RID: 20153 RVA: 0x001A4362 File Offset: 0x001A2562
		private void StopAttacking()
		{
			this.attack.StopAttacking();
		}

		// Token: 0x06004EBA RID: 20154 RVA: 0x001A4370 File Offset: 0x001A2570
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (this.attack == null)
			{
				return StateStatus.Error;
			}
			if (baseEntity == null)
			{
				brain.Navigator.ClearFacingDirectionOverride();
				this.StopAttacking();
				return StateStatus.Finished;
			}
			if (brain.Senses.ignoreSafeZonePlayers)
			{
				BasePlayer basePlayer = baseEntity as BasePlayer;
				if (basePlayer != null && basePlayer.InSafeZone())
				{
					return StateStatus.Error;
				}
			}
			Vector3Ex.Direction2D(baseEntity.transform.position, entity.transform.position);
			Vector3 position = baseEntity.transform.position;
			if (!brain.Navigator.SetDestination(position, BaseNavigator.NavigationSpeed.Fast, 0.2f, 0f))
			{
				return StateStatus.Error;
			}
			Vector3 aimDirection = ScarecrowBrain.AttackState.GetAimDirection(brain.Navigator.transform.position, baseEntity.transform.position);
			brain.Navigator.SetFacingDirectionOverride(aimDirection);
			if (this.attack.CanAttack(baseEntity))
			{
				this.StartAttacking(baseEntity);
			}
			else
			{
				this.StopAttacking();
			}
			return StateStatus.Running;
		}

		// Token: 0x06004EBB RID: 20155 RVA: 0x0019EDFD File Offset: 0x0019CFFD
		private static Vector3 GetAimDirection(Vector3 from, Vector3 target)
		{
			return Vector3Ex.Direction2D(target, from);
		}

		// Token: 0x06004EBC RID: 20156 RVA: 0x001A4483 File Offset: 0x001A2683
		private void StartAttacking(BaseEntity entity)
		{
			this.attack.StartAttacking(entity);
		}
	}

	// Token: 0x02000C5D RID: 3165
	public class ChaseState : BaseAIBrain.BasicAIState
	{
		// Token: 0x040042D1 RID: 17105
		private float throwDelayTime;

		// Token: 0x040042D2 RID: 17106
		private bool useBeanCan;

		// Token: 0x06004EBD RID: 20157 RVA: 0x0019EE1F File Offset: 0x0019D01F
		public ChaseState() : base(AIState.Chase)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004EBE RID: 20158 RVA: 0x001A4494 File Offset: 0x001A2694
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			entity.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
			this.throwDelayTime = UnityEngine.Time.time + UnityEngine.Random.Range(0.2f, 0.5f);
			this.useBeanCan = ((float)UnityEngine.Random.Range(0, 100) <= 20f);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x06004EBF RID: 20159 RVA: 0x001A4537 File Offset: 0x001A2737
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			entity.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
			this.Stop();
		}

		// Token: 0x06004EC0 RID: 20160 RVA: 0x0019EEA1 File Offset: 0x0019D0A1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004EC1 RID: 20161 RVA: 0x001A4558 File Offset: 0x001A2758
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.Stop();
				return StateStatus.Error;
			}
			if (this.useBeanCan && UnityEngine.Time.time >= this.throwDelayTime && AI.npc_use_thrown_weapons && Halloween.scarecrows_throw_beancans && UnityEngine.Time.time >= ScarecrowNPC.NextBeanCanAllowedTime && (brain.GetBrainBaseEntity() as ScarecrowNPC).TryUseThrownWeapon(baseEntity, 10f))
			{
				brain.Navigator.Stop();
				return StateStatus.Running;
			}
			if ((brain.GetBrainBaseEntity() as BasePlayer).modelState.aiming)
			{
				return StateStatus.Running;
			}
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0.25f, 0f))
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

	// Token: 0x02000C5E RID: 3166
	public class RoamState : BaseAIBrain.BasicAIState
	{
		// Token: 0x040042D3 RID: 17107
		private StateStatus status = StateStatus.Error;

		// Token: 0x06004EC2 RID: 20162 RVA: 0x001A4643 File Offset: 0x001A2843
		public RoamState() : base(AIState.Roam)
		{
		}

		// Token: 0x06004EC3 RID: 20163 RVA: 0x001A4653 File Offset: 0x001A2853
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004EC4 RID: 20164 RVA: 0x001A4664 File Offset: 0x001A2864
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			ScarecrowNPC scarecrowNPC = entity as ScarecrowNPC;
			if (scarecrowNPC == null)
			{
				return;
			}
			Vector3 vector = brain.Events.Memory.Position.Get(4);
			Vector3 pos;
			if (scarecrowNPC.RoamAroundHomePoint)
			{
				pos = brain.PathFinder.GetBestRoamPositionFromAnchor(brain.Navigator, vector, vector, 1f, brain.Navigator.BestRoamPointMaxDistance);
			}
			else
			{
				pos = brain.PathFinder.GetBestRoamPosition(brain.Navigator, brain.Events.Memory.Position.Get(4), 10f, brain.Navigator.BestRoamPointMaxDistance);
			}
			if (brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Slow, 0f, 0f))
			{
				this.status = StateStatus.Running;
				return;
			}
			this.status = StateStatus.Error;
		}

		// Token: 0x06004EC5 RID: 20165 RVA: 0x0019EEA1 File Offset: 0x0019D0A1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004EC6 RID: 20166 RVA: 0x001A4741 File Offset: 0x001A2941
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
