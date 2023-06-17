using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000752 RID: 1874
public class ItemSkinDirectory : ScriptableObject
{
	// Token: 0x04002A7B RID: 10875
	private static ItemSkinDirectory _Instance;

	// Token: 0x04002A7C RID: 10876
	public ItemSkinDirectory.Skin[] skins;

	// Token: 0x1700044D RID: 1101
	// (get) Token: 0x0600346C RID: 13420 RVA: 0x00144A6C File Offset: 0x00142C6C
	public static ItemSkinDirectory Instance
	{
		get
		{
			if (ItemSkinDirectory._Instance == null)
			{
				ItemSkinDirectory._Instance = FileSystem.Load<ItemSkinDirectory>("assets/skins.asset", true);
				if (ItemSkinDirectory._Instance == null)
				{
					throw new Exception("Couldn't load assets/skins.asset");
				}
				if (ItemSkinDirectory._Instance.skins == null || ItemSkinDirectory._Instance.skins.Length == 0)
				{
					throw new Exception("Loaded assets/skins.asset but something is wrong");
				}
			}
			return ItemSkinDirectory._Instance;
		}
	}

	// Token: 0x0600346D RID: 13421 RVA: 0x00144AD8 File Offset: 0x00142CD8
	public static ItemSkinDirectory.Skin[] ForItem(ItemDefinition item)
	{
		return (from x in ItemSkinDirectory.Instance.skins
		where x.isSkin && x.itemid == item.itemid
		select x).ToArray<ItemSkinDirectory.Skin>();
	}

	// Token: 0x0600346E RID: 13422 RVA: 0x00144B14 File Offset: 0x00142D14
	public static ItemSkinDirectory.Skin FindByInventoryDefinitionId(int id)
	{
		return (from x in ItemSkinDirectory.Instance.skins
		where x.id == id
		select x).FirstOrDefault<ItemSkinDirectory.Skin>();
	}

	// Token: 0x02000E62 RID: 3682
	[Serializable]
	public struct Skin
	{
		// Token: 0x04004B4F RID: 19279
		public int id;

		// Token: 0x04004B50 RID: 19280
		public int itemid;

		// Token: 0x04004B51 RID: 19281
		public string name;

		// Token: 0x04004B52 RID: 19282
		public bool isSkin;

		// Token: 0x04004B53 RID: 19283
		private SteamInventoryItem _invItem;

		// Token: 0x170006F0 RID: 1776
		// (get) Token: 0x06005292 RID: 21138 RVA: 0x001B0840 File Offset: 0x001AEA40
		public SteamInventoryItem invItem
		{
			get
			{
				if (this._invItem == null && !string.IsNullOrEmpty(this.name))
				{
					this._invItem = FileSystem.Load<SteamInventoryItem>(this.name, true);
				}
				return this._invItem;
			}
		}
	}
}
