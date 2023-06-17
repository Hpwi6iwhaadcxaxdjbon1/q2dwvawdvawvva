using System;
using UnityEngine;

// Token: 0x02000606 RID: 1542
[Serializable]
public class ItemAmountRanged : ItemAmount
{
	// Token: 0x0400254D RID: 9549
	public float maxAmount = -1f;

	// Token: 0x06002DA5 RID: 11685 RVA: 0x00112784 File Offset: 0x00110984
	public override void OnAfterDeserialize()
	{
		base.OnAfterDeserialize();
	}

	// Token: 0x06002DA6 RID: 11686 RVA: 0x0011278C File Offset: 0x0011098C
	public ItemAmountRanged(ItemDefinition item = null, float amt = 0f, float max = -1f) : base(item, amt)
	{
		this.maxAmount = max;
	}

	// Token: 0x06002DA7 RID: 11687 RVA: 0x001127A8 File Offset: 0x001109A8
	public override float GetAmount()
	{
		if (this.maxAmount > 0f && this.maxAmount > this.amount)
		{
			return UnityEngine.Random.Range(this.amount, this.maxAmount);
		}
		return this.amount;
	}
}
