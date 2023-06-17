using System;
using UnityEngine;

// Token: 0x020005E3 RID: 1507
public class ItemModConsumeChance : ItemModConsume
{
	// Token: 0x040024BF RID: 9407
	public float chanceForSecondaryConsume = 0.5f;

	// Token: 0x040024C0 RID: 9408
	public GameObjectRef secondaryConsumeEffect;

	// Token: 0x040024C1 RID: 9409
	public ItemModConsumable secondaryConsumable;

	// Token: 0x06002D2F RID: 11567 RVA: 0x001106DC File Offset: 0x0010E8DC
	private bool GetChance()
	{
		UnityEngine.Random.State state = UnityEngine.Random.state;
		UnityEngine.Random.InitState(Time.frameCount);
		bool result = UnityEngine.Random.Range(0f, 1f) <= this.chanceForSecondaryConsume;
		UnityEngine.Random.state = state;
		return result;
	}

	// Token: 0x06002D30 RID: 11568 RVA: 0x00110719 File Offset: 0x0010E919
	public override ItemModConsumable GetConsumable()
	{
		if (this.GetChance())
		{
			return this.secondaryConsumable;
		}
		return base.GetConsumable();
	}

	// Token: 0x06002D31 RID: 11569 RVA: 0x00110730 File Offset: 0x0010E930
	public override GameObjectRef GetConsumeEffect()
	{
		if (this.GetChance())
		{
			return this.secondaryConsumeEffect;
		}
		return base.GetConsumeEffect();
	}
}
