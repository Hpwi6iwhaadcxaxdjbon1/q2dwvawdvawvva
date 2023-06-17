using System;

// Token: 0x020003F7 RID: 1015
public interface IIdealSlotEntity
{
	// Token: 0x060022C8 RID: 8904
	int GetIdealSlot(BasePlayer player, Item item);

	// Token: 0x060022C9 RID: 8905
	ItemContainerId GetIdealContainer(BasePlayer player, Item item, bool altMove);
}
