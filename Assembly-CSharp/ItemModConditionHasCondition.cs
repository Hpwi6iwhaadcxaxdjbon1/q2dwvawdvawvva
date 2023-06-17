using System;
using UnityEngine;

// Token: 0x020005DC RID: 1500
public class ItemModConditionHasCondition : ItemMod
{
	// Token: 0x040024AD RID: 9389
	public float conditionTarget = 1f;

	// Token: 0x040024AE RID: 9390
	[Tooltip("If set to above 0 will check for fraction instead of raw value")]
	public float conditionFractionTarget = -1f;

	// Token: 0x040024AF RID: 9391
	public bool lessThan;

	// Token: 0x06002D1D RID: 11549 RVA: 0x001101BC File Offset: 0x0010E3BC
	public override bool Passes(Item item)
	{
		if (!item.hasCondition)
		{
			return false;
		}
		if (this.conditionFractionTarget > 0f)
		{
			return (!this.lessThan && item.conditionNormalized > this.conditionFractionTarget) || (this.lessThan && item.conditionNormalized < this.conditionFractionTarget);
		}
		return (!this.lessThan && item.condition >= this.conditionTarget) || (this.lessThan && item.condition < this.conditionTarget);
	}
}
