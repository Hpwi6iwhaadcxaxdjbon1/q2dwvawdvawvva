using System;
using UnityEngine;

// Token: 0x020001FA RID: 506
public class FrankensteinBrain : PetBrain
{
	// Token: 0x040012D0 RID: 4816
	[ServerVar]
	public static float MoveTowardsRate = 1f;

	// Token: 0x06001A8B RID: 6795 RVA: 0x000BEE04 File Offset: 0x000BD004
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new BaseAIBrain.BaseIdleState());
		base.AddState(new FrankensteinBrain.MoveTorwardsState());
		base.AddState(new BaseAIBrain.BaseChaseState());
		base.AddState(new BaseAIBrain.BaseAttackState());
		base.AddState(new FrankensteinBrain.MoveToPointState());
	}

	// Token: 0x06001A8C RID: 6796 RVA: 0x000BEE43 File Offset: 0x000BD043
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(this.GetBaseEntity());
	}

	// Token: 0x06001A8D RID: 6797 RVA: 0x000BEE7E File Offset: 0x000BD07E
	public FrankensteinPet GetEntity()
	{
		return this.GetBaseEntity() as FrankensteinPet;
	}

	// Token: 0x06001A8E RID: 6798 RVA: 0x000BEE8B File Offset: 0x000BD08B
	public override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x02000C5A RID: 3162
	public class MoveToPointState : BaseAIBrain.BasicAIState
	{
		// Token: 0x040042CE RID: 17102
		private float originalStopDistance;

		// Token: 0x06004EAD RID: 20141 RVA: 0x001A40A7 File Offset: 0x001A22A7
		public MoveToPointState() : base(AIState.MoveToPoint)
		{
		}

		// Token: 0x06004EAE RID: 20142 RVA: 0x001A40B4 File Offset: 0x001A22B4
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			BaseNavigator navigator = brain.Navigator;
			this.originalStopDistance = navigator.StoppingDistance;
			navigator.StoppingDistance = 0.5f;
		}

		// Token: 0x06004EAF RID: 20143 RVA: 0x001A40E7 File Offset: 0x001A22E7
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.StoppingDistance = this.originalStopDistance;
			this.Stop();
		}

		// Token: 0x06004EB0 RID: 20144 RVA: 0x0019EEA1 File Offset: 0x0019D0A1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004EB1 RID: 20145 RVA: 0x001A4108 File Offset: 0x001A2308
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			Vector3 pos = brain.Events.Memory.Position.Get(6);
			if (!brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Normal, FrankensteinBrain.MoveTowardsRate, 0f))
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.Moving)
			{
				brain.LoadDefaultAIDesign();
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}
	}

	// Token: 0x02000C5B RID: 3163
	public class MoveTorwardsState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004EB2 RID: 20146 RVA: 0x0019F454 File Offset: 0x0019D654
		public MoveTorwardsState() : base(AIState.MoveTowards)
		{
		}

		// Token: 0x06004EB3 RID: 20147 RVA: 0x001A4175 File Offset: 0x001A2375
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004EB4 RID: 20148 RVA: 0x0019EEA1 File Offset: 0x0019D0A1
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004EB5 RID: 20149 RVA: 0x001A4188 File Offset: 0x001A2388
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.Stop();
				return StateStatus.Error;
			}
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Normal, FrankensteinBrain.MoveTowardsRate, 0f))
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
}
