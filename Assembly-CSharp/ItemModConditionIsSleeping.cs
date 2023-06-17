using System;

// Token: 0x020005E0 RID: 1504
public class ItemModConditionIsSleeping : ItemMod
{
	// Token: 0x040024B5 RID: 9397
	public bool requiredState;

	// Token: 0x06002D26 RID: 11558 RVA: 0x00110330 File Offset: 0x0010E530
	public override bool Passes(Item item)
	{
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		return !(ownerPlayer == null) && ownerPlayer.IsSleeping() == this.requiredState;
	}
}
