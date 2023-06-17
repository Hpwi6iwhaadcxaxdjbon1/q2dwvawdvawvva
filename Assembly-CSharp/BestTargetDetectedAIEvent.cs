using System;
using ProtoBuf;

// Token: 0x0200036D RID: 877
public class BestTargetDetectedAIEvent : BaseAIEvent
{
	// Token: 0x06001FC1 RID: 8129 RVA: 0x000D5709 File Offset: 0x000D3909
	public BestTargetDetectedAIEvent() : base(AIEventType.BestTargetDetected)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Normal;
	}

	// Token: 0x06001FC2 RID: 8130 RVA: 0x000D571A File Offset: 0x000D391A
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
	}

	// Token: 0x06001FC3 RID: 8131 RVA: 0x000D5724 File Offset: 0x000D3924
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		IAIAttack iaiattack = base.Owner as IAIAttack;
		if (iaiattack == null)
		{
			return;
		}
		global::BaseEntity bestTarget = iaiattack.GetBestTarget();
		if (base.Inverted)
		{
			if (bestTarget == null && base.ShouldSetOutputEntityMemory)
			{
				memory.Entity.Remove(base.OutputEntityMemorySlot);
			}
			base.Result = (bestTarget == null);
			return;
		}
		if (bestTarget != null && base.ShouldSetOutputEntityMemory)
		{
			memory.Entity.Set(bestTarget, base.OutputEntityMemorySlot);
		}
		base.Result = (bestTarget != null);
	}
}
