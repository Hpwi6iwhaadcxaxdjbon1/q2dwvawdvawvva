using System;
using ProtoBuf;

// Token: 0x0200036F RID: 879
public class HealthBelowAIEvent : BaseAIEvent
{
	// Token: 0x04001933 RID: 6451
	private BaseCombatEntity combatEntity;

	// Token: 0x1700029D RID: 669
	// (get) Token: 0x06001FCA RID: 8138 RVA: 0x000D586E File Offset: 0x000D3A6E
	// (set) Token: 0x06001FCB RID: 8139 RVA: 0x000D5876 File Offset: 0x000D3A76
	public float HealthFraction { get; set; }

	// Token: 0x06001FCC RID: 8140 RVA: 0x000D587F File Offset: 0x000D3A7F
	public HealthBelowAIEvent() : base(AIEventType.HealthBelow)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001FCD RID: 8141 RVA: 0x000D5890 File Offset: 0x000D3A90
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		HealthBelowAIEventData healthBelowData = data.healthBelowData;
		this.HealthFraction = healthBelowData.healthFraction;
	}

	// Token: 0x06001FCE RID: 8142 RVA: 0x000D58B8 File Offset: 0x000D3AB8
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.healthBelowData = new HealthBelowAIEventData();
		aieventData.healthBelowData.healthFraction = this.HealthFraction;
		return aieventData;
	}

	// Token: 0x06001FCF RID: 8143 RVA: 0x000D58DC File Offset: 0x000D3ADC
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		this.combatEntity = (memory.Entity.Get(base.InputEntityMemorySlot) as BaseCombatEntity);
		if (this.combatEntity == null)
		{
			return;
		}
		bool flag = this.combatEntity.healthFraction < this.HealthFraction;
		if (base.Inverted)
		{
			base.Result = !flag;
			return;
		}
		base.Result = flag;
	}
}
