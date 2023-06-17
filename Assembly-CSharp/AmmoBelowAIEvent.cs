using System;
using ProtoBuf;

// Token: 0x02000368 RID: 872
public class AmmoBelowAIEvent : BaseAIEvent
{
	// Token: 0x1700028E RID: 654
	// (get) Token: 0x06001F93 RID: 8083 RVA: 0x000D50C0 File Offset: 0x000D32C0
	// (set) Token: 0x06001F94 RID: 8084 RVA: 0x000D50C8 File Offset: 0x000D32C8
	public float Value { get; private set; }

	// Token: 0x06001F95 RID: 8085 RVA: 0x000D50D1 File Offset: 0x000D32D1
	public AmmoBelowAIEvent() : base(AIEventType.AmmoBelow)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Normal;
	}

	// Token: 0x06001F96 RID: 8086 RVA: 0x000D50E4 File Offset: 0x000D32E4
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		AmmoBelowAIEventData ammoBelowData = data.ammoBelowData;
		this.Value = ammoBelowData.value;
	}

	// Token: 0x06001F97 RID: 8087 RVA: 0x000D510C File Offset: 0x000D330C
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.ammoBelowData = new AmmoBelowAIEventData();
		aieventData.ammoBelowData.value = this.Value;
		return aieventData;
	}

	// Token: 0x06001F98 RID: 8088 RVA: 0x000D5130 File Offset: 0x000D3330
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		IAIAttack iaiattack = base.Owner as IAIAttack;
		if (iaiattack == null)
		{
			return;
		}
		bool flag = iaiattack.GetAmmoFraction() < this.Value;
		if (base.Inverted)
		{
			base.Result = !flag;
			return;
		}
		base.Result = flag;
	}
}
