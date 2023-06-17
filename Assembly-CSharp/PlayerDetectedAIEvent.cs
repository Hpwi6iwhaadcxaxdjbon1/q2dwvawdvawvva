using System;
using ProtoBuf;

// Token: 0x0200037B RID: 891
public class PlayerDetectedAIEvent : BaseAIEvent
{
	// Token: 0x170002A1 RID: 673
	// (get) Token: 0x06001FF1 RID: 8177 RVA: 0x000D5F38 File Offset: 0x000D4138
	// (set) Token: 0x06001FF2 RID: 8178 RVA: 0x000D5F40 File Offset: 0x000D4140
	public float Range { get; set; }

	// Token: 0x06001FF3 RID: 8179 RVA: 0x000D5F49 File Offset: 0x000D4149
	public PlayerDetectedAIEvent() : base(AIEventType.PlayerDetected)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Slow;
	}

	// Token: 0x06001FF4 RID: 8180 RVA: 0x000D5F5C File Offset: 0x000D415C
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		PlayerDetectedAIEventData playerDetectedData = data.playerDetectedData;
		this.Range = playerDetectedData.range;
	}

	// Token: 0x06001FF5 RID: 8181 RVA: 0x000D5F84 File Offset: 0x000D4184
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.playerDetectedData = new PlayerDetectedAIEventData();
		aieventData.playerDetectedData.range = this.Range;
		return aieventData;
	}

	// Token: 0x06001FF6 RID: 8182 RVA: 0x000D5FA8 File Offset: 0x000D41A8
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = false;
		global::BaseEntity nearestPlayer = senses.GetNearestPlayer(this.Range);
		if (base.Inverted)
		{
			if (nearestPlayer == null && base.ShouldSetOutputEntityMemory)
			{
				memory.Entity.Remove(base.OutputEntityMemorySlot);
			}
			base.Result = (nearestPlayer == null);
			return;
		}
		if (nearestPlayer != null && base.ShouldSetOutputEntityMemory)
		{
			memory.Entity.Set(nearestPlayer, base.OutputEntityMemorySlot);
		}
		base.Result = (nearestPlayer != null);
	}
}
