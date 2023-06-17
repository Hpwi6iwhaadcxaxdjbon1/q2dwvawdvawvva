using System;

// Token: 0x0200037E RID: 894
public class StateFinishedAIEvent : BaseAIEvent
{
	// Token: 0x06001FFB RID: 8187 RVA: 0x000D60D6 File Offset: 0x000D42D6
	public StateFinishedAIEvent() : base(AIEventType.StateFinished)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001FFC RID: 8188 RVA: 0x000D60E6 File Offset: 0x000D42E6
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		if (stateStatus == StateStatus.Finished)
		{
			base.Result = !base.Inverted;
		}
	}
}
