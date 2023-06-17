using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005E5 RID: 1509
public class ItemModContainer : ItemMod
{
	// Token: 0x040024C3 RID: 9411
	public int capacity = 6;

	// Token: 0x040024C4 RID: 9412
	public int maxStackSize;

	// Token: 0x040024C5 RID: 9413
	[InspectorFlags]
	public ItemContainer.Flag containerFlags;

	// Token: 0x040024C6 RID: 9414
	public ItemContainer.ContentsType onlyAllowedContents = ItemContainer.ContentsType.Generic;

	// Token: 0x040024C7 RID: 9415
	public ItemDefinition onlyAllowedItemType;

	// Token: 0x040024C8 RID: 9416
	public List<ItemSlot> availableSlots = new List<ItemSlot>();

	// Token: 0x040024C9 RID: 9417
	public ItemDefinition[] validItemWhitelist = new ItemDefinition[0];

	// Token: 0x040024CA RID: 9418
	public bool openInDeployed = true;

	// Token: 0x040024CB RID: 9419
	public bool openInInventory = true;

	// Token: 0x040024CC RID: 9420
	public List<ItemAmount> defaultContents = new List<ItemAmount>();

	// Token: 0x06002D36 RID: 11574 RVA: 0x00110868 File Offset: 0x0010EA68
	public override void OnItemCreated(Item item)
	{
		if (!item.isServer)
		{
			return;
		}
		if (this.capacity <= 0)
		{
			return;
		}
		if (item.contents != null)
		{
			if (this.validItemWhitelist != null && this.validItemWhitelist.Length != 0)
			{
				item.contents.canAcceptItem = new Func<Item, int, bool>(this.CanAcceptItem);
			}
			return;
		}
		item.contents = new ItemContainer();
		item.contents.flags = this.containerFlags;
		item.contents.allowedContents = ((this.onlyAllowedContents == (ItemContainer.ContentsType)0) ? ItemContainer.ContentsType.Generic : this.onlyAllowedContents);
		this.SetAllowedItems(item.contents);
		item.contents.availableSlots = this.availableSlots;
		if ((this.validItemWhitelist != null && this.validItemWhitelist.Length != 0) || this.ForceAcceptItemCheck)
		{
			item.contents.canAcceptItem = new Func<Item, int, bool>(this.CanAcceptItem);
		}
		item.contents.ServerInitialize(item, this.capacity);
		item.contents.maxStackSize = this.maxStackSize;
		item.contents.GiveUID();
	}

	// Token: 0x06002D37 RID: 11575 RVA: 0x0011096E File Offset: 0x0010EB6E
	protected virtual void SetAllowedItems(ItemContainer container)
	{
		container.SetOnlyAllowedItem(this.onlyAllowedItemType);
	}

	// Token: 0x170003BF RID: 959
	// (get) Token: 0x06002D38 RID: 11576 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected virtual bool ForceAcceptItemCheck
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002D39 RID: 11577 RVA: 0x0011097C File Offset: 0x0010EB7C
	protected virtual bool CanAcceptItem(Item item, int count)
	{
		ItemDefinition[] array = this.validItemWhitelist;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].itemid == item.info.itemid)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002D3A RID: 11578 RVA: 0x001109B8 File Offset: 0x0010EBB8
	public override void OnVirginItem(Item item)
	{
		base.OnVirginItem(item);
		foreach (ItemAmount itemAmount in this.defaultContents)
		{
			Item item2 = ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, 0UL);
			if (item2 != null)
			{
				item2.MoveToContainer(item.contents, -1, true, false, null, true);
			}
		}
	}

	// Token: 0x06002D3B RID: 11579 RVA: 0x00110A38 File Offset: 0x0010EC38
	public override void CollectedForCrafting(Item item, BasePlayer crafter)
	{
		if (item.contents == null)
		{
			return;
		}
		for (int i = item.contents.itemList.Count - 1; i >= 0; i--)
		{
			Item item2 = item.contents.itemList[i];
			if (!item2.MoveToContainer(crafter.inventory.containerMain, -1, true, false, null, true))
			{
				item2.Drop(crafter.GetDropPosition(), crafter.GetDropVelocity(), default(Quaternion));
			}
		}
	}
}
