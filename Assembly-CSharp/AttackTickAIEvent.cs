using System;

// Token: 0x0200036A RID: 874
public class AttackTickAIEvent : BaseAIEvent
{
	// Token: 0x06001F9B RID: 8091 RVA: 0x000D519C File Offset: 0x000D339C
	public AttackTickAIEvent() : base(AIEventType.AttackTick)
	{
		base.Rate = BaseAIEvent.ExecuteRate.VeryFast;
	}

	// Token: 0x06001F9C RID: 8092 RVA: 0x000D51B0 File Offset: 0x000D33B0
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		IAIAttack iaiattack = base.Owner as IAIAttack;
		if (iaiattack == null)
		{
			return;
		}
		BaseEntity baseEntity = memory.Entity.Get(base.InputEntityMemorySlot);
		iaiattack.AttackTick(this.deltaTime, baseEntity, senses.Memory.IsLOS(baseEntity));
		base.Result = !base.Inverted;
	}
}
