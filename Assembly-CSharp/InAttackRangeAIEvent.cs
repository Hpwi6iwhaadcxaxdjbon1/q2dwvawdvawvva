using System;

// Token: 0x02000373 RID: 883
public class InAttackRangeAIEvent : BaseAIEvent
{
	// Token: 0x06001FD8 RID: 8152 RVA: 0x000D5A07 File Offset: 0x000D3C07
	public InAttackRangeAIEvent() : base(AIEventType.InAttackRange)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001FD9 RID: 8153 RVA: 0x000D5A18 File Offset: 0x000D3C18
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		BaseEntity baseEntity = memory.Entity.Get(base.InputEntityMemorySlot);
		base.Result = false;
		if (baseEntity == null)
		{
			return;
		}
		IAIAttack iaiattack = base.Owner as IAIAttack;
		if (iaiattack == null)
		{
			return;
		}
		float num;
		bool flag = iaiattack.IsTargetInRange(baseEntity, out num);
		base.Result = (base.Inverted ? (!flag) : flag);
	}
}
