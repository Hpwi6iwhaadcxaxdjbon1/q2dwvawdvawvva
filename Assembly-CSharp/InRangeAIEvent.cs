using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000374 RID: 884
public class InRangeAIEvent : BaseAIEvent
{
	// Token: 0x1700029F RID: 671
	// (get) Token: 0x06001FDA RID: 8154 RVA: 0x000D5A77 File Offset: 0x000D3C77
	// (set) Token: 0x06001FDB RID: 8155 RVA: 0x000D5A7F File Offset: 0x000D3C7F
	public float Range { get; set; }

	// Token: 0x06001FDC RID: 8156 RVA: 0x000D5A88 File Offset: 0x000D3C88
	public InRangeAIEvent() : base(AIEventType.InRange)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001FDD RID: 8157 RVA: 0x000D5A98 File Offset: 0x000D3C98
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		InRangeAIEventData inRangeData = data.inRangeData;
		this.Range = inRangeData.range;
	}

	// Token: 0x06001FDE RID: 8158 RVA: 0x000D5AC0 File Offset: 0x000D3CC0
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.inRangeData = new InRangeAIEventData();
		aieventData.inRangeData.range = this.Range;
		return aieventData;
	}

	// Token: 0x06001FDF RID: 8159 RVA: 0x000D5AE4 File Offset: 0x000D3CE4
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		global::BaseEntity baseEntity = memory.Entity.Get(base.InputEntityMemorySlot);
		base.Result = false;
		if (baseEntity == null)
		{
			return;
		}
		bool flag = Vector3Ex.Distance2D(base.Owner.transform.position, baseEntity.transform.position) <= this.Range;
		base.Result = (base.Inverted ? (!flag) : flag);
	}
}
