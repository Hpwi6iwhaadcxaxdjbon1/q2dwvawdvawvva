using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000375 RID: 885
public class InRangeOfHomeAIEvent : BaseAIEvent
{
	// Token: 0x170002A0 RID: 672
	// (get) Token: 0x06001FE0 RID: 8160 RVA: 0x000D5B55 File Offset: 0x000D3D55
	// (set) Token: 0x06001FE1 RID: 8161 RVA: 0x000D5B5D File Offset: 0x000D3D5D
	public float Range { get; set; }

	// Token: 0x06001FE2 RID: 8162 RVA: 0x000D5B66 File Offset: 0x000D3D66
	public InRangeOfHomeAIEvent() : base(AIEventType.InRangeOfHome)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001FE3 RID: 8163 RVA: 0x000D5B78 File Offset: 0x000D3D78
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		InRangeOfHomeAIEventData inRangeOfHomeData = data.inRangeOfHomeData;
		this.Range = inRangeOfHomeData.range;
	}

	// Token: 0x06001FE4 RID: 8164 RVA: 0x000D5BA0 File Offset: 0x000D3DA0
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.inRangeOfHomeData = new InRangeOfHomeAIEventData();
		aieventData.inRangeOfHomeData.range = this.Range;
		return aieventData;
	}

	// Token: 0x06001FE5 RID: 8165 RVA: 0x000D5BC4 File Offset: 0x000D3DC4
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		Vector3 b = memory.Position.Get(4);
		base.Result = false;
		bool flag = Vector3Ex.Distance2D(base.Owner.transform.position, b) <= this.Range;
		base.Result = (base.Inverted ? (!flag) : flag);
	}
}
