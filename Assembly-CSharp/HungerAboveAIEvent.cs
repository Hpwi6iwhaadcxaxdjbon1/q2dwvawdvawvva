using System;
using ProtoBuf;

// Token: 0x02000371 RID: 881
public class HungerAboveAIEvent : BaseAIEvent
{
	// Token: 0x1700029E RID: 670
	// (get) Token: 0x06001FD1 RID: 8145 RVA: 0x000D594E File Offset: 0x000D3B4E
	// (set) Token: 0x06001FD2 RID: 8146 RVA: 0x000D5956 File Offset: 0x000D3B56
	public float Value { get; private set; }

	// Token: 0x06001FD3 RID: 8147 RVA: 0x000D595F File Offset: 0x000D3B5F
	public HungerAboveAIEvent() : base(AIEventType.HungerAbove)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Slow;
	}

	// Token: 0x06001FD4 RID: 8148 RVA: 0x000D5970 File Offset: 0x000D3B70
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		HungerAboveAIEventData hungerAboveData = data.hungerAboveData;
		this.Value = hungerAboveData.value;
	}

	// Token: 0x06001FD5 RID: 8149 RVA: 0x000D5998 File Offset: 0x000D3B98
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.hungerAboveData = new HungerAboveAIEventData();
		aieventData.hungerAboveData.value = this.Value;
		return aieventData;
	}

	// Token: 0x06001FD6 RID: 8150 RVA: 0x000D59BC File Offset: 0x000D3BBC
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		IAIHungerAbove iaihungerAbove = base.Owner as IAIHungerAbove;
		if (iaihungerAbove == null)
		{
			base.Result = false;
			return;
		}
		bool flag = iaihungerAbove.IsHungerAbove(this.Value);
		if (base.Inverted)
		{
			base.Result = !flag;
			return;
		}
		base.Result = flag;
	}
}
