using System;

// Token: 0x02000379 RID: 889
public class OnPositionMemorySetAIEvent : BaseAIEvent
{
	// Token: 0x06001FEC RID: 8172 RVA: 0x000D5D77 File Offset: 0x000D3F77
	public OnPositionMemorySetAIEvent() : base(AIEventType.OnPositionMemorySet)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001FED RID: 8173 RVA: 0x000D5D88 File Offset: 0x000D3F88
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = false;
		if (memory.Position.GetTimeSinceSet(5) <= 0.5f)
		{
			base.Result = !base.Inverted;
			return;
		}
		base.Result = base.Inverted;
	}
}
