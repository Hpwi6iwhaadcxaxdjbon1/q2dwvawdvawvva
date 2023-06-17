using System;

// Token: 0x0200037D RID: 893
public class StateErrorAIEvent : BaseAIEvent
{
	// Token: 0x06001FF9 RID: 8185 RVA: 0x000D6095 File Offset: 0x000D4295
	public StateErrorAIEvent() : base(AIEventType.StateError)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001FFA RID: 8186 RVA: 0x000D60A5 File Offset: 0x000D42A5
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		if (stateStatus == StateStatus.Error)
		{
			base.Result = !base.Inverted;
			return;
		}
		if (stateStatus == StateStatus.Running)
		{
			base.Result = base.Inverted;
		}
	}
}
