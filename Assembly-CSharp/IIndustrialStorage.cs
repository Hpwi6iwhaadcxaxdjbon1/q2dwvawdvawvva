using System;

// Token: 0x020004D4 RID: 1236
public interface IIndustrialStorage
{
	// Token: 0x1700036A RID: 874
	// (get) Token: 0x06002821 RID: 10273
	ItemContainer Container { get; }

	// Token: 0x06002822 RID: 10274
	Vector2i InputSlotRange(int slotIndex);

	// Token: 0x06002823 RID: 10275
	Vector2i OutputSlotRange(int slotIndex);

	// Token: 0x06002824 RID: 10276
	void OnStorageItemTransferBegin();

	// Token: 0x06002825 RID: 10277
	void OnStorageItemTransferEnd();

	// Token: 0x1700036B RID: 875
	// (get) Token: 0x06002826 RID: 10278
	BaseEntity IndustrialEntity { get; }
}
