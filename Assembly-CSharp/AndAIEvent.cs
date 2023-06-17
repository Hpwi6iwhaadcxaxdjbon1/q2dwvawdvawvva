using System;

// Token: 0x02000369 RID: 873
public class AndAIEvent : BaseAIEvent
{
	// Token: 0x06001F99 RID: 8089 RVA: 0x000D5182 File Offset: 0x000D3382
	public AndAIEvent() : base(AIEventType.And)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Normal;
	}

	// Token: 0x06001F9A RID: 8090 RVA: 0x000D5193 File Offset: 0x000D3393
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = false;
	}
}
