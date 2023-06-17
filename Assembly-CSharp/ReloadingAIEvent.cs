using System;

// Token: 0x0200037C RID: 892
public class ReloadingAIEvent : BaseAIEvent
{
	// Token: 0x06001FF7 RID: 8183 RVA: 0x000D6031 File Offset: 0x000D4231
	public ReloadingAIEvent() : base(AIEventType.Reloading)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001FF8 RID: 8184 RVA: 0x000D6044 File Offset: 0x000D4244
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		BaseEntity baseEntity = memory.Entity.Get(base.InputEntityMemorySlot);
		base.Result = false;
		NPCPlayer npcplayer = baseEntity as NPCPlayer;
		if (npcplayer == null)
		{
			return;
		}
		bool flag = npcplayer.IsReloading();
		base.Result = (base.Inverted ? (!flag) : flag);
	}
}
