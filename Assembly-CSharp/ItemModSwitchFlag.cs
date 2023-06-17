using System;

// Token: 0x020005FE RID: 1534
public class ItemModSwitchFlag : ItemMod
{
	// Token: 0x0400252B RID: 9515
	public Item.Flag flag;

	// Token: 0x0400252C RID: 9516
	public bool state;

	// Token: 0x06002D86 RID: 11654 RVA: 0x00112086 File Offset: 0x00110286
	public override void DoAction(Item item, BasePlayer player)
	{
		if (item.amount < 1)
		{
			return;
		}
		if (item.HasFlag(this.flag) == this.state)
		{
			return;
		}
		item.SetFlag(this.flag, this.state);
		item.MarkDirty();
	}
}
