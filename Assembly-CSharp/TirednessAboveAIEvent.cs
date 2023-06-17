using System;
using ProtoBuf;

// Token: 0x02000388 RID: 904
public class TirednessAboveAIEvent : BaseAIEvent
{
	// Token: 0x170002A8 RID: 680
	// (get) Token: 0x06002023 RID: 8227 RVA: 0x000D65BD File Offset: 0x000D47BD
	// (set) Token: 0x06002024 RID: 8228 RVA: 0x000D65C5 File Offset: 0x000D47C5
	public float Value { get; private set; }

	// Token: 0x06002025 RID: 8229 RVA: 0x000D65CE File Offset: 0x000D47CE
	public TirednessAboveAIEvent() : base(AIEventType.TirednessAbove)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Slow;
	}

	// Token: 0x06002026 RID: 8230 RVA: 0x000D65E0 File Offset: 0x000D47E0
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		TirednessAboveAIEventData tirednessAboveData = data.tirednessAboveData;
		this.Value = tirednessAboveData.value;
	}

	// Token: 0x06002027 RID: 8231 RVA: 0x000D6608 File Offset: 0x000D4808
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.tirednessAboveData = new TirednessAboveAIEventData();
		aieventData.tirednessAboveData.value = this.Value;
		return aieventData;
	}

	// Token: 0x06002028 RID: 8232 RVA: 0x000D662C File Offset: 0x000D482C
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		IAITirednessAbove iaitirednessAbove = base.Owner as IAITirednessAbove;
		if (iaitirednessAbove == null)
		{
			return;
		}
		bool flag = iaitirednessAbove.IsTirednessAbove(this.Value);
		if (base.Inverted)
		{
			base.Result = !flag;
			return;
		}
		base.Result = flag;
	}
}
