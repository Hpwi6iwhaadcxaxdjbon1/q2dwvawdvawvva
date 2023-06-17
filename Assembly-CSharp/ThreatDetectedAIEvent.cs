using System;
using ProtoBuf;

// Token: 0x02000381 RID: 897
public class ThreatDetectedAIEvent : BaseAIEvent
{
	// Token: 0x170002A4 RID: 676
	// (get) Token: 0x06002007 RID: 8199 RVA: 0x000D6302 File Offset: 0x000D4502
	// (set) Token: 0x06002008 RID: 8200 RVA: 0x000D630A File Offset: 0x000D450A
	public float Range { get; set; }

	// Token: 0x06002009 RID: 8201 RVA: 0x000D6313 File Offset: 0x000D4513
	public ThreatDetectedAIEvent() : base(AIEventType.ThreatDetected)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Slow;
	}

	// Token: 0x0600200A RID: 8202 RVA: 0x000D6324 File Offset: 0x000D4524
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		ThreatDetectedAIEventData threatDetectedData = data.threatDetectedData;
		this.Range = threatDetectedData.range;
	}

	// Token: 0x0600200B RID: 8203 RVA: 0x000D634C File Offset: 0x000D454C
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.threatDetectedData = new ThreatDetectedAIEventData();
		aieventData.threatDetectedData.range = this.Range;
		return aieventData;
	}

	// Token: 0x0600200C RID: 8204 RVA: 0x000D6370 File Offset: 0x000D4570
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		global::BaseEntity nearestThreat = senses.GetNearestThreat(this.Range);
		if (base.Inverted)
		{
			if (nearestThreat == null && base.ShouldSetOutputEntityMemory)
			{
				memory.Entity.Remove(base.OutputEntityMemorySlot);
			}
			base.Result = (nearestThreat == null);
			return;
		}
		if (nearestThreat != null && base.ShouldSetOutputEntityMemory)
		{
			memory.Entity.Set(nearestThreat, base.OutputEntityMemorySlot);
		}
		base.Result = (nearestThreat != null);
	}
}
