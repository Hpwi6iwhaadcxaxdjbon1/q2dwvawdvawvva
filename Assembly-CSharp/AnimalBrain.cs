using System;
using UnityEngine;

// Token: 0x020001E3 RID: 483
public class AnimalBrain : BaseAIBrain
{
	// Token: 0x04001257 RID: 4695
	public static int Count;

	// Token: 0x0600199A RID: 6554 RVA: 0x000BB85C File Offset: 0x000B9A5C
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new AnimalBrain.IdleState());
		base.AddState(new AnimalBrain.MoveTowardsState());
		base.AddState(new AnimalBrain.FleeState());
		base.AddState(new AnimalBrain.RoamState());
		base.AddState(new AnimalBrain.AttackState());
		base.AddState(new BaseAIBrain.BaseSleepState());
		base.AddState(new AnimalBrain.ChaseState());
		base.AddState(new BaseAIBrain.BaseCooldownState());
	}

	// Token: 0x0600199B RID: 6555 RVA: 0x000BB8C7 File Offset: 0x000B9AC7
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new BasePathFinder();
		AnimalBrain.Count++;
	}

	// Token: 0x0600199C RID: 6556 RVA: 0x000BB8F8 File Offset: 0x000B9AF8
	public override void OnDestroy()
	{
		base.OnDestroy();
		AnimalBrain.Count--;
	}

	// Token: 0x0600199D RID: 6557 RVA: 0x000BB90C File Offset: 0x000B9B0C
	public BaseAnimalNPC GetEntity()
	{
		return this.GetBaseEntity() as BaseAnimalNPC;
	}

	// Token: 0x02000C3E RID: 3134
	public class AttackState : BaseAIBrain.BasicAIState
	{
		// Token: 0x0400429F RID: 17055
		private IAIAttack attack;

		// Token: 0x06004E2D RID: 20013 RVA: 0x0019EC12 File Offset: 0x0019CE12
		public AttackState() : base(AIState.Attack)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004E2E RID: 20014 RVA: 0x001A2120 File Offset: 0x001A0320
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.attack = (entity as IAIAttack);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			BasePlayer basePlayer = baseEntity as BasePlayer;
			if (basePlayer != null && basePlayer.IsDead())
			{
				this.StopAttacking();
				return;
			}
			if (baseEntity != null && baseEntity.Health() > 0f)
			{
				BaseCombatEntity target = baseEntity as BaseCombatEntity;
				Vector3 aimDirection = AnimalBrain.AttackState.GetAimDirection(entity as BaseCombatEntity, target);
				brain.Navigator.SetFacingDirectionOverride(aimDirection);
				if (this.attack.CanAttack(baseEntity))
				{
					this.StartAttacking(baseEntity);
				}
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x06004E2F RID: 20015 RVA: 0x001A21EE File Offset: 0x001A03EE
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
			brain.Navigator.Stop();
			this.StopAttacking();
		}

		// Token: 0x06004E30 RID: 20016 RVA: 0x001A2214 File Offset: 0x001A0414
		private void StopAttacking()
		{
			this.attack.StopAttacking();
		}

		// Token: 0x06004E31 RID: 20017 RVA: 0x001A2224 File Offset: 0x001A0424
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
			if (baseEntity.Health() <= 0f)
			{
				this.StopAttacking();
				return StateStatus.Finished;
			}
			BasePlayer basePlayer = baseEntity as BasePlayer;
			if (basePlayer != null && basePlayer.IsDead())
			{
				this.StopAttacking();
				return StateStatus.Finished;
			}
			BaseVehicle baseVehicle = (basePlayer != null) ? basePlayer.GetMountedVehicle() : null;
			if (baseVehicle != null && baseVehicle is BaseModularVehicle)
			{
				this.StopAttacking();
				return StateStatus.Error;
			}
			if (brain.Senses.ignoreSafeZonePlayers && basePlayer != null && basePlayer.InSafeZone())
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0.25f, (baseEntity is BasePlayer && this.attack != null) ? this.attack.EngagementRange() : 0f))
			{
				return StateStatus.Error;
			}
			BaseCombatEntity target = baseEntity as BaseCombatEntity;
			Vector3 aimDirection = AnimalBrain.AttackState.GetAimDirection(entity as BaseCombatEntity, target);
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

		// Token: 0x06004E32 RID: 20018 RVA: 0x001A2388 File Offset: 0x001A0588
		private static Vector3 GetAimDirection(BaseCombatEntity from, BaseCombatEntity target)
		{
			if (!(from == null) && !(target == null))
			{
				return Vector3Ex.Direction2D(target.transform.position, from.transform.position);
			}
			if (!(from != null))
			{
				return Vector3.forward;
			}
			return from.transform.forward;
		}

		// Token: 0x06004E33 RID: 20019 RVA: 0x001A23DD File Offset: 0x001A05DD
		private void StartAttacking(BaseEntity entity)
		{
			this.attack.StartAttacking(entity);
		}
	}

	// Token: 0x02000C3F RID: 3135
	public class ChaseState : BaseAIBrain.BasicAIState
	{
		// Token: 0x040042A0 RID: 17056
		private IAIAttack attack;

		// Token: 0x06004E34 RID: 20020 RVA: 0x0019EE1F File Offset: 0x0019D01F
		public ChaseState() : base(AIState.Chase)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004E35 RID: 20021 RVA: 0x001A23EC File Offset: 0x001A05EC
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.attack = (entity as IAIAttack);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x06004E36 RID: 20022 RVA: 0x001A2459 File Offset: 0x001A0659
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004E37 RID: 20023 RVA: 0x0019EEA1 File Offset: 0x0019D0A1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004E38 RID: 20024 RVA: 0x001A246C File Offset: 0x001A066C
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.Stop();
				return StateStatus.Error;
			}
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0.25f, (baseEntity is BasePlayer && this.attack != null) ? this.attack.EngagementRange() : 0f))
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

	// Token: 0x02000C40 RID: 3136
	public class FleeState : BaseAIBrain.BasicAIState
	{
		// Token: 0x040042A1 RID: 17057
		private float nextInterval = 2f;

		// Token: 0x040042A2 RID: 17058
		private float stopFleeDistance;

		// Token: 0x06004E39 RID: 20025 RVA: 0x001A2507 File Offset: 0x001A0707
		public FleeState() : base(AIState.Flee)
		{
		}

		// Token: 0x06004E3A RID: 20026 RVA: 0x001A251C File Offset: 0x001A071C
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				this.stopFleeDistance = UnityEngine.Random.Range(80f, 100f) + Mathf.Clamp(Vector3Ex.Distance2D(brain.Navigator.transform.position, baseEntity.transform.position), 0f, 50f);
			}
			this.FleeFrom(brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot), entity);
		}

		// Token: 0x06004E3B RID: 20027 RVA: 0x001A25C8 File Offset: 0x001A07C8
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004E3C RID: 20028 RVA: 0x0019EEA1 File Offset: 0x0019D0A1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004E3D RID: 20029 RVA: 0x001A25D8 File Offset: 0x001A07D8
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				return StateStatus.Finished;
			}
			if (Vector3Ex.Distance2D(brain.Navigator.transform.position, baseEntity.transform.position) >= this.stopFleeDistance)
			{
				return StateStatus.Finished;
			}
			if ((brain.Navigator.UpdateIntervalElapsed(this.nextInterval) || !brain.Navigator.Moving) && !this.FleeFrom(baseEntity, entity))
			{
				return StateStatus.Error;
			}
			return StateStatus.Running;
		}

		// Token: 0x06004E3E RID: 20030 RVA: 0x001A2678 File Offset: 0x001A0878
		private bool FleeFrom(BaseEntity fleeFromEntity, BaseEntity thisEntity)
		{
			if (thisEntity == null || fleeFromEntity == null)
			{
				return false;
			}
			this.nextInterval = UnityEngine.Random.Range(3f, 6f);
			Vector3 pos;
			if (!this.brain.PathFinder.GetBestFleePosition(this.brain.Navigator, this.brain.Senses, fleeFromEntity, this.brain.Events.Memory.Position.Get(4), 50f, 100f, out pos))
			{
				return false;
			}
			bool flag = this.brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			if (!flag)
			{
				this.Stop();
			}
			return flag;
		}
	}

	// Token: 0x02000C41 RID: 3137
	public class IdleState : BaseAIBrain.BaseIdleState
	{
		// Token: 0x040042A3 RID: 17059
		private float nextTurnTime;

		// Token: 0x040042A4 RID: 17060
		private float minTurnTime = 10f;

		// Token: 0x040042A5 RID: 17061
		private float maxTurnTime = 20f;

		// Token: 0x040042A6 RID: 17062
		private int turnChance = 33;

		// Token: 0x06004E3F RID: 20031 RVA: 0x001A2725 File Offset: 0x001A0925
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.FaceNewDirection(entity);
		}

		// Token: 0x06004E40 RID: 20032 RVA: 0x001A2736 File Offset: 0x001A0936
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
		}

		// Token: 0x06004E41 RID: 20033 RVA: 0x001A274C File Offset: 0x001A094C
		private void FaceNewDirection(BaseEntity entity)
		{
			if (UnityEngine.Random.Range(0, 100) <= this.turnChance)
			{
				Vector3 position = entity.transform.position;
				Vector3 normalized = (BasePathFinder.GetPointOnCircle(position, 1f, UnityEngine.Random.Range(0f, 594f)) - position).normalized;
				this.brain.Navigator.SetFacingDirectionOverride(normalized);
			}
			this.nextTurnTime = Time.realtimeSinceStartup + UnityEngine.Random.Range(this.minTurnTime, this.maxTurnTime);
		}

		// Token: 0x06004E42 RID: 20034 RVA: 0x001A27CC File Offset: 0x001A09CC
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (Time.realtimeSinceStartup >= this.nextTurnTime)
			{
				this.FaceNewDirection(entity);
			}
			return StateStatus.Running;
		}
	}

	// Token: 0x02000C42 RID: 3138
	public class MoveTowardsState : BaseAIBrain.BaseMoveTorwardsState
	{
	}

	// Token: 0x02000C43 RID: 3139
	public class RoamState : BaseAIBrain.BasicAIState
	{
		// Token: 0x040042A7 RID: 17063
		private StateStatus status = StateStatus.Error;

		// Token: 0x06004E45 RID: 20037 RVA: 0x001A281B File Offset: 0x001A0A1B
		public RoamState() : base(AIState.Roam)
		{
		}

		// Token: 0x06004E46 RID: 20038 RVA: 0x001A282B File Offset: 0x001A0A2B
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004E47 RID: 20039 RVA: 0x001A283C File Offset: 0x001A0A3C
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			Vector3 vector;
			if (brain.InGroup() && !brain.IsGroupLeader)
			{
				vector = brain.Events.Memory.Position.Get(5);
				vector = BasePathFinder.GetPointOnCircle(vector, UnityEngine.Random.Range(2f, 7f), UnityEngine.Random.Range(0f, 359f));
			}
			else
			{
				vector = brain.PathFinder.GetBestRoamPosition(brain.Navigator, brain.Events.Memory.Position.Get(4), 20f, 100f);
			}
			if (brain.Navigator.SetDestination(vector, BaseNavigator.NavigationSpeed.Slow, 0f, 0f))
			{
				if (brain.InGroup() && brain.IsGroupLeader)
				{
					brain.SetGroupRoamRootPosition(vector);
				}
				this.status = StateStatus.Running;
				return;
			}
			this.status = StateStatus.Error;
		}

		// Token: 0x06004E48 RID: 20040 RVA: 0x0019EEA1 File Offset: 0x0019D0A1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004E49 RID: 20041 RVA: 0x001A2926 File Offset: 0x001A0B26
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
