using System;
using UnityEngine;

// Token: 0x020003F6 RID: 1014
public interface IItemContainerEntity : IIdealSlotEntity, ILootableEntity
{
	// Token: 0x170002E7 RID: 743
	// (get) Token: 0x060022BE RID: 8894
	ItemContainer inventory { get; }

	// Token: 0x170002E8 RID: 744
	// (get) Token: 0x060022BF RID: 8895
	Transform Transform { get; }

	// Token: 0x170002E9 RID: 745
	// (get) Token: 0x060022C0 RID: 8896
	bool DropsLoot { get; }

	// Token: 0x170002EA RID: 746
	// (get) Token: 0x060022C1 RID: 8897
	float DestroyLootPercent { get; }

	// Token: 0x170002EB RID: 747
	// (get) Token: 0x060022C2 RID: 8898
	bool DropFloats { get; }

	// Token: 0x060022C3 RID: 8899
	void DropItems(BaseEntity initiator = null);

	// Token: 0x060022C4 RID: 8900
	bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true);

	// Token: 0x060022C5 RID: 8901
	bool ShouldDropItemsIndividually();

	// Token: 0x060022C6 RID: 8902
	void DropBonusItems(BaseEntity initiator, ItemContainer container);

	// Token: 0x060022C7 RID: 8903
	Vector3 GetDropPosition();
}
