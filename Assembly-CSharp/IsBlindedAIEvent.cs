using System;

// Token: 0x02000376 RID: 886
public class IsBlindedAIEvent : BaseAIEvent
{
	// Token: 0x06001FE6 RID: 8166 RVA: 0x000D5C1C File Offset: 0x000D3E1C
	public IsBlindedAIEvent() : base(AIEventType.IsBlinded)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001FE7 RID: 8167 RVA: 0x000D5C30 File Offset: 0x000D3E30
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		bool flag = senses.brain.Blinded();
		if (base.Inverted)
		{
			base.Result = !flag;
			return;
		}
		base.Result = flag;
	}
}
