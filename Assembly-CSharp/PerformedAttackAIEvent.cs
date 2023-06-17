using System;
using UnityEngine;

// Token: 0x0200037A RID: 890
public class PerformedAttackAIEvent : BaseAIEvent
{
	// Token: 0x04001937 RID: 6455
	protected float lastExecuteTime = float.NegativeInfinity;

	// Token: 0x04001938 RID: 6456
	private BaseCombatEntity combatEntity;

	// Token: 0x06001FEE RID: 8174 RVA: 0x000D5DC0 File Offset: 0x000D3FC0
	public PerformedAttackAIEvent() : base(AIEventType.PerformedAttack)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001FEF RID: 8175 RVA: 0x000D5DDB File Offset: 0x000D3FDB
	public override void Reset()
	{
		base.Reset();
		this.lastExecuteTime = Time.time;
	}

	// Token: 0x06001FF0 RID: 8176 RVA: 0x000D5DF0 File Offset: 0x000D3FF0
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = false;
		this.combatEntity = (memory.Entity.Get(base.InputEntityMemorySlot) as BaseCombatEntity);
		float num = this.lastExecuteTime;
		this.lastExecuteTime = Time.time;
		if (this.combatEntity == null)
		{
			return;
		}
		if (this.combatEntity.lastDealtDamageTime < num)
		{
			base.Result = base.Inverted;
			return;
		}
		if (this.combatEntity.lastDealtDamageTo == null)
		{
			return;
		}
		if (this.combatEntity.lastDealtDamageTo == this.combatEntity)
		{
			return;
		}
		BasePlayer basePlayer = this.combatEntity as BasePlayer;
		if (basePlayer != null)
		{
			if (basePlayer == memory.Entity.Get(5) && basePlayer.lastDealtDamageTo == base.Owner)
			{
				return;
			}
			if (basePlayer == memory.Entity.Get(5) && (basePlayer.lastDealtDamageTo.gameObject.layer == 21 || basePlayer.lastDealtDamageTo.gameObject.layer == 8))
			{
				return;
			}
		}
		if (base.ShouldSetOutputEntityMemory)
		{
			memory.Entity.Set(this.combatEntity.lastDealtDamageTo, base.OutputEntityMemorySlot);
		}
		base.Result = !base.Inverted;
	}
}
