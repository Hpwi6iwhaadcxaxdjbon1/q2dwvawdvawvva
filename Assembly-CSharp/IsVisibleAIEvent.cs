using System;

// Token: 0x02000377 RID: 887
public class IsVisibleAIEvent : BaseAIEvent
{
	// Token: 0x06001FE8 RID: 8168 RVA: 0x000D5C63 File Offset: 0x000D3E63
	public IsVisibleAIEvent() : base(AIEventType.IsVisible)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001FE9 RID: 8169 RVA: 0x000D5C74 File Offset: 0x000D3E74
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = false;
		BaseEntity baseEntity = memory.Entity.Get(base.InputEntityMemorySlot);
		if (baseEntity == null)
		{
			return;
		}
		if (!(base.Owner is IAIAttack))
		{
			return;
		}
		bool flag = senses.Memory.IsLOS(baseEntity);
		base.Result = (base.Inverted ? (!flag) : flag);
	}
}
