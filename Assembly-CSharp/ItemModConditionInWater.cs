using System;

// Token: 0x020005DF RID: 1503
public class ItemModConditionInWater : ItemMod
{
	// Token: 0x040024B4 RID: 9396
	public bool requiredState;

	// Token: 0x06002D24 RID: 11556 RVA: 0x00110300 File Offset: 0x0010E500
	public override bool Passes(Item item)
	{
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		return !(ownerPlayer == null) && ownerPlayer.IsHeadUnderwater() == this.requiredState;
	}
}
