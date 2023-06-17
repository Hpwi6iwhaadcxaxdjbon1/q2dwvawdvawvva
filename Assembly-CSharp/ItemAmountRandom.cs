using System;
using UnityEngine;

// Token: 0x02000605 RID: 1541
[Serializable]
public class ItemAmountRandom
{
	// Token: 0x0400254B RID: 9547
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemDef;

	// Token: 0x0400254C RID: 9548
	public AnimationCurve amount = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x06002DA3 RID: 11683 RVA: 0x00112712 File Offset: 0x00110912
	public int RandomAmount()
	{
		return Mathf.RoundToInt(this.amount.Evaluate(UnityEngine.Random.Range(0f, 1f)));
	}
}
