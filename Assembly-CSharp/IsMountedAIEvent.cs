using System;

// Token: 0x02000378 RID: 888
public class IsMountedAIEvent : BaseAIEvent
{
	// Token: 0x06001FEA RID: 8170 RVA: 0x000D5CD4 File Offset: 0x000D3ED4
	public IsMountedAIEvent() : base(AIEventType.IsMounted)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001FEB RID: 8171 RVA: 0x000D5CE8 File Offset: 0x000D3EE8
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		IAIMounted iaimounted = memory.Entity.Get(base.InputEntityMemorySlot) as IAIMounted;
		base.Result = false;
		if (iaimounted == null)
		{
			return;
		}
		if (base.Inverted && !iaimounted.IsMounted())
		{
			base.Result = true;
		}
		if (!base.Inverted && iaimounted.IsMounted())
		{
			base.Result = true;
		}
		if (base.Result && base.ShouldSetOutputEntityMemory)
		{
			memory.Entity.Set(memory.Entity.Get(base.InputEntityMemorySlot), base.OutputEntityMemorySlot);
		}
	}
}
