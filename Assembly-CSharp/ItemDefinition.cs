using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x020005CA RID: 1482
public class ItemDefinition : MonoBehaviour
{
	// Token: 0x0400244E RID: 9294
	[Header("Item")]
	[ReadOnly]
	public int itemid;

	// Token: 0x0400244F RID: 9295
	[Tooltip("The shortname should be unique. A hash will be generated from it to identify the item type. If this name changes at any point it will make all saves incompatible")]
	public string shortname;

	// Token: 0x04002450 RID: 9296
	[Header("Appearance")]
	public Translate.Phrase displayName;

	// Token: 0x04002451 RID: 9297
	public Translate.Phrase displayDescription;

	// Token: 0x04002452 RID: 9298
	public Sprite iconSprite;

	// Token: 0x04002453 RID: 9299
	public ItemCategory category;

	// Token: 0x04002454 RID: 9300
	public ItemSelectionPanel selectionPanel;

	// Token: 0x04002455 RID: 9301
	[Header("Containment")]
	public int maxDraggable;

	// Token: 0x04002456 RID: 9302
	public ItemContainer.ContentsType itemType = ItemContainer.ContentsType.Generic;

	// Token: 0x04002457 RID: 9303
	public ItemDefinition.AmountType amountType;

	// Token: 0x04002458 RID: 9304
	[InspectorFlags]
	public ItemSlot occupySlots = ItemSlot.None;

	// Token: 0x04002459 RID: 9305
	public int stackable;

	// Token: 0x0400245A RID: 9306
	public bool quickDespawn;

	// Token: 0x0400245B RID: 9307
	[Header("Spawn Tables")]
	[Tooltip("How rare this item is and how much it costs to research")]
	public Rarity rarity;

	// Token: 0x0400245C RID: 9308
	public Rarity despawnRarity;

	// Token: 0x0400245D RID: 9309
	public bool spawnAsBlueprint;

	// Token: 0x0400245E RID: 9310
	[Header("Sounds")]
	public SoundDefinition inventoryGrabSound;

	// Token: 0x0400245F RID: 9311
	public SoundDefinition inventoryDropSound;

	// Token: 0x04002460 RID: 9312
	public SoundDefinition physImpactSoundDef;

	// Token: 0x04002461 RID: 9313
	public ItemDefinition.Condition condition;

	// Token: 0x04002462 RID: 9314
	[Header("Misc")]
	public bool hidden;

	// Token: 0x04002463 RID: 9315
	[InspectorFlags]
	public ItemDefinition.Flag flags;

	// Token: 0x04002464 RID: 9316
	[Tooltip("User can craft this item on any server if they have this steam item")]
	public SteamInventoryItem steamItem;

	// Token: 0x04002465 RID: 9317
	[Tooltip("User can craft this item if they have this DLC purchased")]
	public SteamDLCItem steamDlc;

	// Token: 0x04002466 RID: 9318
	[Tooltip("Can only craft this item if the parent is craftable (tech tree)")]
	public ItemDefinition Parent;

	// Token: 0x04002467 RID: 9319
	public GameObjectRef worldModelPrefab;

	// Token: 0x04002468 RID: 9320
	public ItemDefinition isRedirectOf;

	// Token: 0x04002469 RID: 9321
	public ItemDefinition.RedirectVendingBehaviour redirectVendingBehaviour;

	// Token: 0x0400246A RID: 9322
	[NonSerialized]
	public ItemMod[] itemMods;

	// Token: 0x0400246B RID: 9323
	public BaseEntity.TraitFlag Traits;

	// Token: 0x0400246C RID: 9324
	[NonSerialized]
	public ItemSkinDirectory.Skin[] skins;

	// Token: 0x0400246D RID: 9325
	[NonSerialized]
	private IPlayerItemDefinition[] _skins2;

	// Token: 0x0400246E RID: 9326
	[Tooltip("Panel to show in the inventory menu when selected")]
	public GameObject panel;

	// Token: 0x04002473 RID: 9331
	[NonSerialized]
	public ItemDefinition[] Children = new ItemDefinition[0];

	// Token: 0x170003B0 RID: 944
	// (get) Token: 0x06002CD2 RID: 11474 RVA: 0x0010F8FC File Offset: 0x0010DAFC
	public IPlayerItemDefinition[] skins2
	{
		get
		{
			if (this._skins2 != null)
			{
				return this._skins2;
			}
			if (PlatformService.Instance.IsValid && PlatformService.Instance.ItemDefinitions != null)
			{
				string prefabname = base.name;
				this._skins2 = (from x in PlatformService.Instance.ItemDefinitions
				where (x.ItemShortName == this.shortname || x.ItemShortName == prefabname) && x.WorkshopId > 0UL
				select x).ToArray<IPlayerItemDefinition>();
			}
			return this._skins2;
		}
	}

	// Token: 0x06002CD3 RID: 11475 RVA: 0x0010F975 File Offset: 0x0010DB75
	public void InvalidateWorkshopSkinCache()
	{
		this._skins2 = null;
	}

	// Token: 0x06002CD4 RID: 11476 RVA: 0x0010F980 File Offset: 0x0010DB80
	public static ulong FindSkin(int itemID, int skinID)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemID);
		if (itemDefinition == null)
		{
			return 0UL;
		}
		IPlayerItemDefinition itemDefinition2 = PlatformService.Instance.GetItemDefinition(skinID);
		if (itemDefinition2 != null)
		{
			ulong workshopDownload = itemDefinition2.WorkshopDownload;
			if (workshopDownload != 0UL)
			{
				string itemShortName = itemDefinition2.ItemShortName;
				if (itemShortName == itemDefinition.shortname || itemShortName == itemDefinition.name)
				{
					return workshopDownload;
				}
			}
		}
		for (int i = 0; i < itemDefinition.skins.Length; i++)
		{
			if (itemDefinition.skins[i].id == skinID)
			{
				return (ulong)((long)skinID);
			}
		}
		return 0UL;
	}

	// Token: 0x170003B1 RID: 945
	// (get) Token: 0x06002CD5 RID: 11477 RVA: 0x0010FA0F File Offset: 0x0010DC0F
	public ItemBlueprint Blueprint
	{
		get
		{
			return base.GetComponent<ItemBlueprint>();
		}
	}

	// Token: 0x170003B2 RID: 946
	// (get) Token: 0x06002CD6 RID: 11478 RVA: 0x0010FA17 File Offset: 0x0010DC17
	public int craftingStackable
	{
		get
		{
			return Mathf.Max(10, this.stackable);
		}
	}

	// Token: 0x06002CD7 RID: 11479 RVA: 0x0010FA26 File Offset: 0x0010DC26
	public bool HasFlag(ItemDefinition.Flag f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x06002CD8 RID: 11480 RVA: 0x0010FA34 File Offset: 0x0010DC34
	public void Initialize(List<ItemDefinition> itemList)
	{
		if (this.itemMods != null)
		{
			Debug.LogError("Item Definition Initializing twice: " + base.name);
		}
		this.skins = ItemSkinDirectory.ForItem(this);
		this.itemMods = base.GetComponentsInChildren<ItemMod>(true);
		ItemMod[] array = this.itemMods;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ModInit();
		}
		this.Children = (from x in itemList
		where x.Parent == this
		select x).ToArray<ItemDefinition>();
		this.ItemModWearable = base.GetComponent<ItemModWearable>();
		this.isHoldable = (base.GetComponent<ItemModEntity>() != null);
		this.isUsable = (base.GetComponent<ItemModEntity>() != null || base.GetComponent<ItemModConsume>() != null);
	}

	// Token: 0x170003B3 RID: 947
	// (get) Token: 0x06002CD9 RID: 11481 RVA: 0x0010FAF2 File Offset: 0x0010DCF2
	public bool isWearable
	{
		get
		{
			return this.ItemModWearable != null;
		}
	}

	// Token: 0x170003B4 RID: 948
	// (get) Token: 0x06002CDA RID: 11482 RVA: 0x0010FB00 File Offset: 0x0010DD00
	// (set) Token: 0x06002CDB RID: 11483 RVA: 0x0010FB08 File Offset: 0x0010DD08
	public ItemModWearable ItemModWearable { get; private set; }

	// Token: 0x170003B5 RID: 949
	// (get) Token: 0x06002CDC RID: 11484 RVA: 0x0010FB11 File Offset: 0x0010DD11
	// (set) Token: 0x06002CDD RID: 11485 RVA: 0x0010FB19 File Offset: 0x0010DD19
	public bool isHoldable { get; private set; }

	// Token: 0x170003B6 RID: 950
	// (get) Token: 0x06002CDE RID: 11486 RVA: 0x0010FB22 File Offset: 0x0010DD22
	// (set) Token: 0x06002CDF RID: 11487 RVA: 0x0010FB2A File Offset: 0x0010DD2A
	public bool isUsable { get; private set; }

	// Token: 0x170003B7 RID: 951
	// (get) Token: 0x06002CE0 RID: 11488 RVA: 0x0010FB33 File Offset: 0x0010DD33
	public bool HasSkins
	{
		get
		{
			return (this.skins2 != null && this.skins2.Length != 0) || (this.skins != null && this.skins.Length != 0);
		}
	}

	// Token: 0x170003B8 RID: 952
	// (get) Token: 0x06002CE1 RID: 11489 RVA: 0x0010FB5C File Offset: 0x0010DD5C
	// (set) Token: 0x06002CE2 RID: 11490 RVA: 0x0010FB64 File Offset: 0x0010DD64
	public bool CraftableWithSkin { get; private set; }

	// Token: 0x02000D77 RID: 3447
	[Serializable]
	public struct Condition
	{
		// Token: 0x04004793 RID: 18323
		public bool enabled;

		// Token: 0x04004794 RID: 18324
		[Tooltip("The maximum condition this item type can have, new items will start with this value")]
		public float max;

		// Token: 0x04004795 RID: 18325
		[Tooltip("If false then item will destroy when condition reaches 0")]
		public bool repairable;

		// Token: 0x04004796 RID: 18326
		[Tooltip("If true, never lose max condition when repaired")]
		public bool maintainMaxCondition;

		// Token: 0x04004797 RID: 18327
		public bool ovenCondition;

		// Token: 0x04004798 RID: 18328
		public ItemDefinition.Condition.WorldSpawnCondition foundCondition;

		// Token: 0x02000FCD RID: 4045
		[Serializable]
		public class WorldSpawnCondition
		{
			// Token: 0x040050E5 RID: 20709
			public float fractionMin = 1f;

			// Token: 0x040050E6 RID: 20710
			public float fractionMax = 1f;
		}
	}

	// Token: 0x02000D78 RID: 3448
	public enum RedirectVendingBehaviour
	{
		// Token: 0x0400479A RID: 18330
		NoListing,
		// Token: 0x0400479B RID: 18331
		ListAsUniqueItem
	}

	// Token: 0x02000D79 RID: 3449
	[Flags]
	public enum Flag
	{
		// Token: 0x0400479D RID: 18333
		NoDropping = 1,
		// Token: 0x0400479E RID: 18334
		NotStraightToBelt = 2
	}

	// Token: 0x02000D7A RID: 3450
	public enum AmountType
	{
		// Token: 0x040047A0 RID: 18336
		Count,
		// Token: 0x040047A1 RID: 18337
		Millilitre,
		// Token: 0x040047A2 RID: 18338
		Feet,
		// Token: 0x040047A3 RID: 18339
		Genetics,
		// Token: 0x040047A4 RID: 18340
		OxygenSeconds,
		// Token: 0x040047A5 RID: 18341
		Frequency,
		// Token: 0x040047A6 RID: 18342
		Generic,
		// Token: 0x040047A7 RID: 18343
		BagLimit
	}
}
