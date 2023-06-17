using System;
using UnityEngine;

// Token: 0x020001EE RID: 494
public class GingerbreadBrain : BaseAIBrain
{
	// Token: 0x060019F4 RID: 6644 RVA: 0x000BCB8D File Offset: 0x000BAD8D
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new BaseAIBrain.BaseIdleState());
		base.AddState(new BaseAIBrain.BaseChaseState());
		base.AddState(new GingerbreadBrain.AttackState());
		base.AddState(new GingerbreadBrain.RoamState());
		base.AddState(new BaseAIBrain.BaseFleeState());
	}

	// Token: 0x060019F5 RID: 6645 RVA: 0x000BCBCC File Offset: 0x000BADCC
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(this.GetBaseEntity());
	}

	// Token: 0x060019F6 RID: 6646 RVA: 0x000BCC07 File Offset: 0x000BAE07
	public override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x02000C49 RID: 3145
	public class AttackState : BaseAIBrain.BasicAIState
	{
		// Token: 0x040042B0 RID: 17072
		private IAIAttack attack;

		// Token: 0x040042B1 RID: 17073
		private float originalStoppingDistance;

		// Token: 0x06004E69 RID: 20073 RVA: 0x0019EC12 File Offset: 0x0019CE12
		public AttackState() : base(AIState.Attack)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004E6A RID: 20074 RVA: 0x001A2F2C File Offset: 0x001A112C
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
				Vector3 aimDirection = GingerbreadBrain.AttackState.GetAimDirection(brain.Navigator.transform.position, baseEntity.transform.position);
				brain.Navigator.SetFacingDirectionOverride(aimDirection);
				if (this.attack.CanAttack(baseEntity))
				{
					this.StartAttacking(baseEntity);
				}
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x06004E6B RID: 20075 RVA: 0x001A3020 File Offset: 0x001A1220
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

		// Token: 0x06004E6C RID: 20076 RVA: 0x001A3086 File Offset: 0x001A1286
		private void StopAttacking()
		{
			this.attack.StopAttacking();
		}

		// Token: 0x06004E6D RID: 20077 RVA: 0x001A3094 File Offset: 0x001A1294
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
			Vector3 aimDirection = GingerbreadBrain.AttackState.GetAimDirection(brain.Navigator.transform.position, baseEntity.transform.position);
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

		// Token: 0x06004E6E RID: 20078 RVA: 0x0019EDFD File Offset: 0x0019CFFD
		private static Vector3 GetAimDirection(Vector3 from, Vector3 target)
		{
			return Vector3Ex.Direction2D(target, from);
		}

		// Token: 0x06004E6F RID: 20079 RVA: 0x001A31A7 File Offset: 0x001A13A7
		private void StartAttacking(BaseEntity entity)
		{
			this.attack.StartAttacking(entity);
		}
	}

	// Token: 0x02000C4A RID: 3146
	public class RoamState : BaseAIBrain.BasicAIState
	{
		// Token: 0x040042B2 RID: 17074
		private StateStatus status = StateStatus.Error;

		// Token: 0x06004E70 RID: 20080 RVA: 0x001A31B6 File Offset: 0x001A13B6
		public RoamState() : base(AIState.Roam)
		{
		}

		// Token: 0x06004E71 RID: 20081 RVA: 0x001A31C6 File Offset: 0x001A13C6
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004E72 RID: 20082 RVA: 0x001A31D8 File Offset: 0x001A13D8
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

		// Token: 0x06004E73 RID: 20083 RVA: 0x0019EEA1 File Offset: 0x0019D0A1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004E74 RID: 20084 RVA: 0x001A32B5 File Offset: 0x001A14B5
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
