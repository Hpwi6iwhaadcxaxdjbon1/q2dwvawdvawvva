using System;

// Token: 0x020005E6 RID: 1510
public class ItemModContainerRestriction : ItemMod
{
	// Token: 0x040024CD RID: 9421
	[InspectorFlags]
	public ItemModContainerRestriction.SlotFlags slotFlags;

	// Token: 0x06002D3D RID: 11581 RVA: 0x00110B05 File Offset: 0x0010ED05
	public bool CanExistWith(ItemModContainerRestriction other)
	{
		return other == null || (this.slotFlags & other.slotFlags) == (ItemModContainerRestriction.SlotFlags)0;
	}

	// Token: 0x02000D7E RID: 3454
	[Flags]
	public enum SlotFlags
	{
		// Token: 0x040047B6 RID: 18358
		Map = 1
	}
}
