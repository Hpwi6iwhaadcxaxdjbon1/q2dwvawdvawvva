using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000607 RID: 1543
public class ItemManager
{
	// Token: 0x0400254E RID: 9550
	public static List<ItemDefinition> itemList;

	// Token: 0x0400254F RID: 9551
	public static Dictionary<int, ItemDefinition> itemDictionary;

	// Token: 0x04002550 RID: 9552
	public static Dictionary<string, ItemDefinition> itemDictionaryByName;

	// Token: 0x04002551 RID: 9553
	public static List<ItemBlueprint> bpList;

	// Token: 0x04002552 RID: 9554
	public static int[] defaultBlueprints;

	// Token: 0x04002553 RID: 9555
	public static ItemDefinition blueprintBaseDef;

	// Token: 0x04002554 RID: 9556
	private static List<ItemManager.ItemRemove> ItemRemoves = new List<ItemManager.ItemRemove>();

	// Token: 0x06002DA8 RID: 11688 RVA: 0x001127E0 File Offset: 0x001109E0
	public static void InvalidateWorkshopSkinCache()
	{
		if (ItemManager.itemList == null)
		{
			return;
		}
		foreach (ItemDefinition itemDefinition in ItemManager.itemList)
		{
			itemDefinition.InvalidateWorkshopSkinCache();
		}
	}

	// Token: 0x06002DA9 RID: 11689 RVA: 0x00112838 File Offset: 0x00110A38
	public static void Initialize()
	{
		if (ItemManager.itemList != null)
		{
			return;
		}
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		GameObject[] array = FileSystem.LoadAllFromBundle<GameObject>("items.preload.bundle", "l:ItemDefinition");
		if (array.Length == 0)
		{
			throw new Exception("items.preload.bundle has no items!");
		}
		if (stopwatch.Elapsed.TotalSeconds > 1.0)
		{
			UnityEngine.Debug.Log("Loading Items Took: " + (stopwatch.Elapsed.TotalMilliseconds / 1000.0).ToString() + " seconds");
		}
		List<ItemDefinition> list = (from x in array
		select x.GetComponent<ItemDefinition>() into x
		where x != null
		select x).ToList<ItemDefinition>();
		List<ItemBlueprint> list2 = (from x in array
		select x.GetComponent<ItemBlueprint>() into x
		where x != null && x.userCraftable
		select x).ToList<ItemBlueprint>();
		Dictionary<int, ItemDefinition> dictionary = new Dictionary<int, ItemDefinition>();
		Dictionary<string, ItemDefinition> dictionary2 = new Dictionary<string, ItemDefinition>(StringComparer.OrdinalIgnoreCase);
		foreach (ItemDefinition itemDefinition in list)
		{
			itemDefinition.Initialize(list);
			if (dictionary.ContainsKey(itemDefinition.itemid))
			{
				ItemDefinition itemDefinition2 = dictionary[itemDefinition.itemid];
				UnityEngine.Debug.LogWarning(string.Concat(new object[]
				{
					"Item ID duplicate ",
					itemDefinition.itemid,
					" (",
					itemDefinition.name,
					") - have you given your items unique shortnames?"
				}), itemDefinition.gameObject);
				UnityEngine.Debug.LogWarning("Other item is " + itemDefinition2.name, itemDefinition2);
			}
			else if (string.IsNullOrEmpty(itemDefinition.shortname))
			{
				UnityEngine.Debug.LogWarning(string.Format("{0} has a null short name! id: {1} {2}", itemDefinition, itemDefinition.itemid, itemDefinition.displayName.english));
			}
			else
			{
				dictionary.Add(itemDefinition.itemid, itemDefinition);
				dictionary2.Add(itemDefinition.shortname, itemDefinition);
			}
		}
		stopwatch.Stop();
		if (stopwatch.Elapsed.TotalSeconds > 1.0)
		{
			UnityEngine.Debug.Log(string.Concat(new string[]
			{
				"Building Items Took: ",
				(stopwatch.Elapsed.TotalMilliseconds / 1000.0).ToString(),
				" seconds / Items: ",
				list.Count.ToString(),
				" / Blueprints: ",
				list2.Count.ToString()
			}));
		}
		ItemManager.defaultBlueprints = (from x in list2
		where !x.NeedsSteamItem && !x.NeedsSteamDLC && x.defaultBlueprint
		select x.targetItem.itemid).ToArray<int>();
		ItemManager.itemList = list;
		ItemManager.bpList = list2;
		ItemManager.itemDictionary = dictionary;
		ItemManager.itemDictionaryByName = dictionary2;
		ItemManager.blueprintBaseDef = ItemManager.FindItemDefinition("blueprintbase");
	}

	// Token: 0x06002DAA RID: 11690 RVA: 0x00112BA4 File Offset: 0x00110DA4
	public static global::Item CreateByName(string strName, int iAmount = 1, ulong skin = 0UL)
	{
		ItemDefinition itemDefinition = ItemManager.itemList.Find((ItemDefinition x) => x.shortname == strName);
		if (itemDefinition == null)
		{
			return null;
		}
		return ItemManager.CreateByItemID(itemDefinition.itemid, iAmount, skin);
	}

	// Token: 0x06002DAB RID: 11691 RVA: 0x00112BF0 File Offset: 0x00110DF0
	public static global::Item CreateByPartialName(string strName, int iAmount = 1, ulong skin = 0UL)
	{
		ItemDefinition itemDefinition = ItemManager.itemList.Find((ItemDefinition x) => x.shortname == strName);
		if (itemDefinition == null)
		{
			itemDefinition = ItemManager.itemList.Find((ItemDefinition x) => x.shortname.Contains(strName, CompareOptions.IgnoreCase));
		}
		if (itemDefinition == null)
		{
			return null;
		}
		return ItemManager.CreateByItemID(itemDefinition.itemid, iAmount, skin);
	}

	// Token: 0x06002DAC RID: 11692 RVA: 0x00112C5C File Offset: 0x00110E5C
	public static ItemDefinition FindDefinitionByPartialName(string strName, int iAmount = 1, ulong skin = 0UL)
	{
		ItemDefinition itemDefinition = ItemManager.itemList.Find((ItemDefinition x) => x.shortname == strName);
		if (itemDefinition == null)
		{
			itemDefinition = ItemManager.itemList.Find((ItemDefinition x) => x.shortname.Contains(strName, CompareOptions.IgnoreCase));
		}
		return itemDefinition;
	}

	// Token: 0x06002DAD RID: 11693 RVA: 0x00112CB0 File Offset: 0x00110EB0
	public static global::Item CreateByItemID(int itemID, int iAmount = 1, ulong skin = 0UL)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemID);
		if (itemDefinition == null)
		{
			return null;
		}
		return ItemManager.Create(itemDefinition, iAmount, skin);
	}

	// Token: 0x06002DAE RID: 11694 RVA: 0x00112CD8 File Offset: 0x00110ED8
	public static global::Item Create(ItemDefinition template, int iAmount = 1, ulong skin = 0UL)
	{
		ItemManager.TrySkinChangeItem(ref template, ref skin);
		if (template == null)
		{
			UnityEngine.Debug.LogWarning("Creating invalid/missing item!");
			return null;
		}
		global::Item item = new global::Item();
		item.isServer = true;
		if (iAmount <= 0)
		{
			UnityEngine.Debug.LogError("Creating item with less than 1 amount! (" + template.displayName.english + ")");
			return null;
		}
		item.info = template;
		item.amount = iAmount;
		item.skin = skin;
		item.Initialize(template);
		return item;
	}

	// Token: 0x06002DAF RID: 11695 RVA: 0x00112D54 File Offset: 0x00110F54
	private static void TrySkinChangeItem(ref ItemDefinition template, ref ulong skinId)
	{
		if (skinId == 0UL)
		{
			return;
		}
		ItemSkinDirectory.Skin skin = ItemSkinDirectory.FindByInventoryDefinitionId((int)skinId);
		if (skin.id == 0)
		{
			return;
		}
		ItemSkin itemSkin = skin.invItem as ItemSkin;
		if (itemSkin == null)
		{
			return;
		}
		if (itemSkin.Redirect == null)
		{
			return;
		}
		template = itemSkin.Redirect;
		skinId = 0UL;
	}

	// Token: 0x06002DB0 RID: 11696 RVA: 0x00112DAC File Offset: 0x00110FAC
	public static global::Item Load(ProtoBuf.Item load, global::Item created, bool isServer)
	{
		if (created == null)
		{
			created = new global::Item();
		}
		created.isServer = isServer;
		created.Load(load);
		if (created.info == null)
		{
			UnityEngine.Debug.LogWarning("Item loading failed - item is invalid");
			return null;
		}
		if (created.info == ItemManager.blueprintBaseDef && created.blueprintTargetDef == null)
		{
			UnityEngine.Debug.LogWarning("Blueprint item loading failed - invalid item target");
			return null;
		}
		return created;
	}

	// Token: 0x06002DB1 RID: 11697 RVA: 0x00112E18 File Offset: 0x00111018
	public static ItemDefinition FindItemDefinition(int itemID)
	{
		ItemManager.Initialize();
		ItemDefinition result;
		if (ItemManager.itemDictionary.TryGetValue(itemID, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06002DB2 RID: 11698 RVA: 0x00112E3C File Offset: 0x0011103C
	public static ItemDefinition FindItemDefinition(string shortName)
	{
		ItemManager.Initialize();
		ItemDefinition result;
		if (ItemManager.itemDictionaryByName.TryGetValue(shortName, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06002DB3 RID: 11699 RVA: 0x00112E60 File Offset: 0x00111060
	public static ItemBlueprint FindBlueprint(ItemDefinition item)
	{
		return item.GetComponent<ItemBlueprint>();
	}

	// Token: 0x06002DB4 RID: 11700 RVA: 0x00112E68 File Offset: 0x00111068
	public static List<ItemDefinition> GetItemDefinitions()
	{
		ItemManager.Initialize();
		return ItemManager.itemList;
	}

	// Token: 0x06002DB5 RID: 11701 RVA: 0x00112E74 File Offset: 0x00111074
	public static List<ItemBlueprint> GetBlueprints()
	{
		ItemManager.Initialize();
		return ItemManager.bpList;
	}

	// Token: 0x06002DB6 RID: 11702 RVA: 0x00112E80 File Offset: 0x00111080
	public static void DoRemoves()
	{
		using (TimeWarning.New("DoRemoves", 0))
		{
			for (int i = 0; i < ItemManager.ItemRemoves.Count; i++)
			{
				if (ItemManager.ItemRemoves[i].time <= Time.time)
				{
					global::Item item = ItemManager.ItemRemoves[i].item;
					ItemManager.ItemRemoves.RemoveAt(i--);
					item.DoRemove();
				}
			}
		}
	}

	// Token: 0x06002DB7 RID: 11703 RVA: 0x00112F08 File Offset: 0x00111108
	public static void Heartbeat()
	{
		ItemManager.DoRemoves();
	}

	// Token: 0x06002DB8 RID: 11704 RVA: 0x00112F10 File Offset: 0x00111110
	public static void RemoveItem(global::Item item, float fTime = 0f)
	{
		Assert.IsTrue(item.isServer, "RemoveItem: Removing a client item!");
		ItemManager.ItemRemove item2 = default(ItemManager.ItemRemove);
		item2.item = item;
		item2.time = Time.time + fTime;
		ItemManager.ItemRemoves.Add(item2);
	}

	// Token: 0x02000D82 RID: 3458
	private struct ItemRemove
	{
		// Token: 0x040047BF RID: 18367
		public global::Item item;

		// Token: 0x040047C0 RID: 18368
		public float time;
	}
}
