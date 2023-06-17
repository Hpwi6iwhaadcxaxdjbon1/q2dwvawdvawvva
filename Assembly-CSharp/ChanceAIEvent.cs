using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200036E RID: 878
public class ChanceAIEvent : BaseAIEvent
{
	// Token: 0x1700029C RID: 668
	// (get) Token: 0x06001FC4 RID: 8132 RVA: 0x000D57BC File Offset: 0x000D39BC
	// (set) Token: 0x06001FC5 RID: 8133 RVA: 0x000D57C4 File Offset: 0x000D39C4
	public float Chance { get; set; }

	// Token: 0x06001FC6 RID: 8134 RVA: 0x000D57CD File Offset: 0x000D39CD
	public ChanceAIEvent() : base(AIEventType.Chance)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001FC7 RID: 8135 RVA: 0x000D57DE File Offset: 0x000D39DE
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		this.Chance = data.chanceData.value;
	}

	// Token: 0x06001FC8 RID: 8136 RVA: 0x000D57F9 File Offset: 0x000D39F9
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.chanceData = new ChanceAIEventData();
		aieventData.chanceData.value = this.Chance;
		return aieventData;
	}

	// Token: 0x06001FC9 RID: 8137 RVA: 0x000D5820 File Offset: 0x000D3A20
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		bool flag = UnityEngine.Random.Range(0f, 1f) <= this.Chance;
		if (base.Inverted)
		{
			base.Result = !flag;
			return;
		}
		base.Result = flag;
	}
}
