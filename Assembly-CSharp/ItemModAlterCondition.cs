using System;

// Token: 0x020005D2 RID: 1490
public class ItemModAlterCondition : ItemMod
{
	// Token: 0x04002498 RID: 9368
	public float conditionChange;

	// Token: 0x06002CFC RID: 11516 RVA: 0x0010FCBA File Offset: 0x0010DEBA
	public override void DoAction(Item item, BasePlayer player)
	{
		if (item.amount < 1)
		{
			return;
		}
		if (this.conditionChange < 0f)
		{
			item.LoseCondition(this.conditionChange * -1f);
			return;
		}
		item.RepairCondition(this.conditionChange);
	}
}
