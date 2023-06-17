using System;
using UnityEngine;

// Token: 0x02000761 RID: 1889
public class SteamInventoryItem : ScriptableObject
{
	// Token: 0x04002AD4 RID: 10964
	public int id;

	// Token: 0x04002AD5 RID: 10965
	public Sprite icon;

	// Token: 0x04002AD6 RID: 10966
	public Translate.Phrase displayName;

	// Token: 0x04002AD7 RID: 10967
	public Translate.Phrase displayDescription;

	// Token: 0x04002AD8 RID: 10968
	[Header("Steam Inventory")]
	public SteamInventoryItem.Category category;

	// Token: 0x04002AD9 RID: 10969
	public SteamInventoryItem.SubCategory subcategory;

	// Token: 0x04002ADA RID: 10970
	public SteamInventoryCategory steamCategory;

	// Token: 0x04002ADB RID: 10971
	public bool isLimitedTimeOffer = true;

	// Token: 0x04002ADC RID: 10972
	[Tooltip("Stop this item being broken down into cloth etc")]
	public bool PreventBreakingDown;

	// Token: 0x04002ADD RID: 10973
	[Header("Meta")]
	public string itemname;

	// Token: 0x04002ADE RID: 10974
	public ulong workshopID;

	// Token: 0x04002ADF RID: 10975
	public SteamDLCItem DlcItem;

	// Token: 0x04002AE0 RID: 10976
	[Tooltip("Does nothing currently")]
	public bool forceCraftableItemDesc;

	// Token: 0x17000451 RID: 1105
	// (get) Token: 0x060034A1 RID: 13473 RVA: 0x00145B90 File Offset: 0x00143D90
	public ItemDefinition itemDefinition
	{
		get
		{
			return ItemManager.FindItemDefinition(this.itemname);
		}
	}

	// Token: 0x060034A2 RID: 13474 RVA: 0x00145B9D File Offset: 0x00143D9D
	public virtual bool HasUnlocked(ulong playerId)
	{
		return this.DlcItem != null && this.DlcItem.HasLicense(playerId);
	}

	// Token: 0x02000E6F RID: 3695
	public enum Category
	{
		// Token: 0x04004BA4 RID: 19364
		None,
		// Token: 0x04004BA5 RID: 19365
		Clothing,
		// Token: 0x04004BA6 RID: 19366
		Weapon,
		// Token: 0x04004BA7 RID: 19367
		Decoration,
		// Token: 0x04004BA8 RID: 19368
		Crate,
		// Token: 0x04004BA9 RID: 19369
		Resource
	}

	// Token: 0x02000E70 RID: 3696
	public enum SubCategory
	{
		// Token: 0x04004BAB RID: 19371
		None,
		// Token: 0x04004BAC RID: 19372
		Shirt,
		// Token: 0x04004BAD RID: 19373
		Pants,
		// Token: 0x04004BAE RID: 19374
		Jacket,
		// Token: 0x04004BAF RID: 19375
		Hat,
		// Token: 0x04004BB0 RID: 19376
		Mask,
		// Token: 0x04004BB1 RID: 19377
		Footwear,
		// Token: 0x04004BB2 RID: 19378
		Weapon,
		// Token: 0x04004BB3 RID: 19379
		Misc,
		// Token: 0x04004BB4 RID: 19380
		Crate,
		// Token: 0x04004BB5 RID: 19381
		Resource,
		// Token: 0x04004BB6 RID: 19382
		CrateUncraftable
	}
}
