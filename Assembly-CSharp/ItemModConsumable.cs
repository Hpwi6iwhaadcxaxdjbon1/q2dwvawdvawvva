using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005E1 RID: 1505
public class ItemModConsumable : MonoBehaviour
{
	// Token: 0x040024B6 RID: 9398
	public int amountToConsume = 1;

	// Token: 0x040024B7 RID: 9399
	public float conditionFractionToLose;

	// Token: 0x040024B8 RID: 9400
	public string achievementWhenEaten;

	// Token: 0x040024B9 RID: 9401
	public List<ItemModConsumable.ConsumableEffect> effects = new List<ItemModConsumable.ConsumableEffect>();

	// Token: 0x040024BA RID: 9402
	public List<ModifierDefintion> modifiers = new List<ModifierDefintion>();

	// Token: 0x06002D28 RID: 11560 RVA: 0x00110360 File Offset: 0x0010E560
	public float GetIfType(MetabolismAttribute.Type typeToPick)
	{
		for (int i = 0; i < this.effects.Count; i++)
		{
			if (this.effects[i].type == typeToPick)
			{
				return this.effects[i].amount;
			}
		}
		return 0f;
	}

	// Token: 0x02000D7D RID: 3453
	[Serializable]
	public class ConsumableEffect
	{
		// Token: 0x040047B1 RID: 18353
		public MetabolismAttribute.Type type;

		// Token: 0x040047B2 RID: 18354
		public float amount;

		// Token: 0x040047B3 RID: 18355
		public float time;

		// Token: 0x040047B4 RID: 18356
		public float onlyIfHealthLessThan = 1f;
	}
}
