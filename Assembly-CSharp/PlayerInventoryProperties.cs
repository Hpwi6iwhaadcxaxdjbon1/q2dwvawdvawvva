using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000756 RID: 1878
[CreateAssetMenu(menuName = "Rust/Player Inventory Properties")]
public class PlayerInventoryProperties : ScriptableObject
{
	// Token: 0x04002A90 RID: 10896
	public string niceName;

	// Token: 0x04002A91 RID: 10897
	public int order = 100;

	// Token: 0x04002A92 RID: 10898
	public List<PlayerInventoryProperties.ItemAmountSkinned> belt;

	// Token: 0x04002A93 RID: 10899
	public List<PlayerInventoryProperties.ItemAmountSkinned> main;

	// Token: 0x04002A94 RID: 10900
	public List<PlayerInventoryProperties.ItemAmountSkinned> wear;

	// Token: 0x04002A95 RID: 10901
	public PlayerInventoryProperties giveBase;

	// Token: 0x04002A96 RID: 10902
	private static PlayerInventoryProperties[] allInventories;

	// Token: 0x0600347A RID: 13434 RVA: 0x00145144 File Offset: 0x00143344
	public void GiveToPlayer(BasePlayer player)
	{
		PlayerInventoryProperties.<>c__DisplayClass7_0 CS$<>8__locals1;
		CS$<>8__locals1.player = player;
		if (CS$<>8__locals1.player == null)
		{
			return;
		}
		CS$<>8__locals1.player.inventory.Strip();
		if (this.giveBase != null)
		{
			this.giveBase.GiveToPlayer(CS$<>8__locals1.player);
		}
		foreach (PlayerInventoryProperties.ItemAmountSkinned toCreate in this.belt)
		{
			PlayerInventoryProperties.<GiveToPlayer>g__CreateItem|7_0(toCreate, CS$<>8__locals1.player.inventory.containerBelt, ref CS$<>8__locals1);
		}
		foreach (PlayerInventoryProperties.ItemAmountSkinned toCreate2 in this.main)
		{
			PlayerInventoryProperties.<GiveToPlayer>g__CreateItem|7_0(toCreate2, CS$<>8__locals1.player.inventory.containerMain, ref CS$<>8__locals1);
		}
		foreach (PlayerInventoryProperties.ItemAmountSkinned toCreate3 in this.wear)
		{
			PlayerInventoryProperties.<GiveToPlayer>g__CreateItem|7_0(toCreate3, CS$<>8__locals1.player.inventory.containerWear, ref CS$<>8__locals1);
		}
	}

	// Token: 0x0600347B RID: 13435 RVA: 0x00145290 File Offset: 0x00143490
	public static PlayerInventoryProperties GetInventoryConfig(string name)
	{
		if (PlayerInventoryProperties.allInventories == null)
		{
			PlayerInventoryProperties.allInventories = FileSystem.LoadAll<PlayerInventoryProperties>("assets/content/properties/playerinventory", "");
			Debug.Log(string.Format("Found {0} inventories", PlayerInventoryProperties.allInventories.Length));
		}
		if (PlayerInventoryProperties.allInventories != null)
		{
			foreach (PlayerInventoryProperties playerInventoryProperties in PlayerInventoryProperties.allInventories)
			{
				if (playerInventoryProperties.niceName.ToLower() == name.ToLower())
				{
					return playerInventoryProperties;
				}
			}
		}
		return null;
	}

	// Token: 0x0600347E RID: 13438 RVA: 0x00145320 File Offset: 0x00143520
	[CompilerGenerated]
	internal static void <GiveToPlayer>g__CreateItem|7_0(PlayerInventoryProperties.ItemAmountSkinned toCreate, ItemContainer destination, ref PlayerInventoryProperties.<>c__DisplayClass7_0 A_2)
	{
		Item item;
		if (toCreate.blueprint)
		{
			item = ItemManager.Create(ItemManager.blueprintBaseDef, 1, 0UL);
			item.blueprintTarget = ((toCreate.itemDef.isRedirectOf != null) ? toCreate.itemDef.isRedirectOf.itemid : toCreate.itemDef.itemid);
		}
		else
		{
			item = ItemManager.Create(toCreate.itemDef, (int)toCreate.amount, toCreate.skinOverride);
		}
		A_2.player.inventory.GiveItem(item, destination, false);
	}

	// Token: 0x02000E69 RID: 3689
	[Serializable]
	public class ItemAmountSkinned : ItemAmount
	{
		// Token: 0x04004B63 RID: 19299
		public ulong skinOverride;

		// Token: 0x04004B64 RID: 19300
		public bool blueprint;

		// Token: 0x060052A1 RID: 21153 RVA: 0x001B0BF5 File Offset: 0x001AEDF5
		public ItemAmountSkinned() : base(null, 0f)
		{
		}
	}
}
