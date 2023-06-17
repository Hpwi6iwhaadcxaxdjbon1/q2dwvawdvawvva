using System;
using ProtoBuf;

// Token: 0x0200038A RID: 906
public class AggressionTimerAIEvent : BaseAIEvent
{
	// Token: 0x170002AC RID: 684
	// (get) Token: 0x06002032 RID: 8242 RVA: 0x000D67FF File Offset: 0x000D49FF
	// (set) Token: 0x06002033 RID: 8243 RVA: 0x000D6807 File Offset: 0x000D4A07
	public float Value { get; private set; }

	// Token: 0x06002034 RID: 8244 RVA: 0x000D6810 File Offset: 0x000D4A10
	public AggressionTimerAIEvent() : base(AIEventType.AggressionTimer)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06002035 RID: 8245 RVA: 0x000D6824 File Offset: 0x000D4A24
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		AggressionTimerAIEventData aggressionTimerData = data.aggressionTimerData;
		this.Value = aggressionTimerData.value;
	}

	// Token: 0x06002036 RID: 8246 RVA: 0x000D684C File Offset: 0x000D4A4C
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.aggressionTimerData = new AggressionTimerAIEventData();
		aieventData.aggressionTimerData.value = this.Value;
		return aieventData;
	}

	// Token: 0x06002037 RID: 8247 RVA: 0x000D6870 File Offset: 0x000D4A70
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		if (base.Inverted)
		{
			base.Result = (senses.TimeInAgressiveState < this.Value);
			return;
		}
		base.Result = (senses.TimeInAgressiveState >= this.Value);
	}
}
