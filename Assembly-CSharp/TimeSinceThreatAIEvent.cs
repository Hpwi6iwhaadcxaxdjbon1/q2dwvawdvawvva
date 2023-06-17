using System;
using ProtoBuf;

// Token: 0x02000382 RID: 898
public class TimeSinceThreatAIEvent : BaseAIEvent
{
	// Token: 0x170002A5 RID: 677
	// (get) Token: 0x0600200D RID: 8205 RVA: 0x000D63FE File Offset: 0x000D45FE
	// (set) Token: 0x0600200E RID: 8206 RVA: 0x000D6406 File Offset: 0x000D4606
	public float Value { get; private set; }

	// Token: 0x0600200F RID: 8207 RVA: 0x000D640F File Offset: 0x000D460F
	public TimeSinceThreatAIEvent() : base(AIEventType.TimeSinceThreat)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06002010 RID: 8208 RVA: 0x000D6420 File Offset: 0x000D4620
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		TimeSinceThreatAIEventData timeSinceThreatData = data.timeSinceThreatData;
		this.Value = timeSinceThreatData.value;
	}

	// Token: 0x06002011 RID: 8209 RVA: 0x000D6448 File Offset: 0x000D4648
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.timeSinceThreatData = new TimeSinceThreatAIEventData();
		aieventData.timeSinceThreatData.value = this.Value;
		return aieventData;
	}

	// Token: 0x06002012 RID: 8210 RVA: 0x000D646C File Offset: 0x000D466C
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		if (base.Inverted)
		{
			base.Result = (senses.TimeSinceThreat < this.Value);
			return;
		}
		base.Result = (senses.TimeSinceThreat >= this.Value);
	}
}
