using System;
using UnityEngine;

// Token: 0x0200036B RID: 875
public class AttackedAIEvent : BaseAIEvent
{
	// Token: 0x04001923 RID: 6435
	protected float lastExecuteTime = float.NegativeInfinity;

	// Token: 0x04001924 RID: 6436
	private BaseCombatEntity combatEntity;

	// Token: 0x06001F9D RID: 8093 RVA: 0x000D5213 File Offset: 0x000D3413
	public AttackedAIEvent() : base(AIEventType.Attacked)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001F9E RID: 8094 RVA: 0x000D522E File Offset: 0x000D342E
	public override void Reset()
	{
		base.Reset();
		this.lastExecuteTime = Time.time;
	}

	// Token: 0x06001F9F RID: 8095 RVA: 0x000D5244 File Offset: 0x000D3444
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		this.combatEntity = (memory.Entity.Get(base.InputEntityMemorySlot) as BaseCombatEntity);
		float num = this.lastExecuteTime;
		this.lastExecuteTime = Time.time;
		if (this.combatEntity == null)
		{
			return;
		}
		if (this.combatEntity.lastAttackedTime >= num)
		{
			if (this.combatEntity.lastAttacker == null)
			{
				return;
			}
			if (this.combatEntity.lastAttacker == this.combatEntity)
			{
				return;
			}
			BasePlayer basePlayer = this.combatEntity.lastAttacker as BasePlayer;
			if (basePlayer != null && basePlayer == memory.Entity.Get(5) && basePlayer.lastDealtDamageTo == base.Owner)
			{
				return;
			}
			if (base.ShouldSetOutputEntityMemory)
			{
				memory.Entity.Set(this.combatEntity.lastAttacker, base.OutputEntityMemorySlot);
			}
			base.Result = !base.Inverted;
		}
	}
}
