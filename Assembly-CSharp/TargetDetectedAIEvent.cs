using System;
using ProtoBuf;

// Token: 0x0200037F RID: 895
public class TargetDetectedAIEvent : BaseAIEvent
{
	// Token: 0x170002A2 RID: 674
	// (get) Token: 0x06001FFD RID: 8189 RVA: 0x000D6107 File Offset: 0x000D4307
	// (set) Token: 0x06001FFE RID: 8190 RVA: 0x000D610F File Offset: 0x000D430F
	public float Range { get; set; }

	// Token: 0x06001FFF RID: 8191 RVA: 0x000D6118 File Offset: 0x000D4318
	public TargetDetectedAIEvent() : base(AIEventType.TargetDetected)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Slow;
	}

	// Token: 0x06002000 RID: 8192 RVA: 0x000D612C File Offset: 0x000D432C
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		TargetDetectedAIEventData targetDetectedData = data.targetDetectedData;
		this.Range = targetDetectedData.range;
	}

	// Token: 0x06002001 RID: 8193 RVA: 0x000D6154 File Offset: 0x000D4354
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.targetDetectedData = new TargetDetectedAIEventData();
		aieventData.targetDetectedData.range = this.Range;
		return aieventData;
	}

	// Token: 0x06002002 RID: 8194 RVA: 0x000D6178 File Offset: 0x000D4378
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		global::BaseEntity nearestTarget = senses.GetNearestTarget(this.Range);
		if (base.Inverted)
		{
			if (nearestTarget == null && base.ShouldSetOutputEntityMemory)
			{
				memory.Entity.Remove(base.OutputEntityMemorySlot);
			}
			base.Result = (nearestTarget == null);
			return;
		}
		if (nearestTarget != null && base.ShouldSetOutputEntityMemory)
		{
			memory.Entity.Set(nearestTarget, base.OutputEntityMemorySlot);
		}
		base.Result = (nearestTarget != null);
	}
}
