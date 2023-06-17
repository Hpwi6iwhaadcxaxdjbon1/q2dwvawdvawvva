using System;
using UnityEngine;

// Token: 0x02000380 RID: 896
public class TargetLostAIEvent : BaseAIEvent
{
	// Token: 0x170002A3 RID: 675
	// (get) Token: 0x06002003 RID: 8195 RVA: 0x000D6206 File Offset: 0x000D4406
	// (set) Token: 0x06002004 RID: 8196 RVA: 0x000D620E File Offset: 0x000D440E
	public float Range { get; set; }

	// Token: 0x06002005 RID: 8197 RVA: 0x000D6217 File Offset: 0x000D4417
	public TargetLostAIEvent() : base(AIEventType.TargetLost)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06002006 RID: 8198 RVA: 0x000D6228 File Offset: 0x000D4428
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		BaseEntity baseEntity = memory.Entity.Get(base.InputEntityMemorySlot);
		if (baseEntity == null)
		{
			base.Result = !base.Inverted;
			return;
		}
		if (Vector3.Distance(baseEntity.transform.position, base.Owner.transform.position) > senses.TargetLostRange)
		{
			base.Result = !base.Inverted;
			return;
		}
		BasePlayer basePlayer = baseEntity as BasePlayer;
		if (baseEntity.Health() <= 0f || (basePlayer != null && basePlayer.IsDead()))
		{
			base.Result = !base.Inverted;
			return;
		}
		if (senses.ignoreSafeZonePlayers && basePlayer != null && basePlayer.InSafeZone())
		{
			base.Result = !base.Inverted;
			return;
		}
	}
}
