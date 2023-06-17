using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020001B6 RID: 438
public class FishLookup : PrefabAttribute
{
	// Token: 0x04001197 RID: 4503
	public ItemModFishable FallbackFish;

	// Token: 0x04001198 RID: 4504
	private static ItemModFishable[] AvailableFish;

	// Token: 0x04001199 RID: 4505
	public static ItemDefinition[] BaitItems;

	// Token: 0x0400119A RID: 4506
	private static TimeSince lastShuffle;

	// Token: 0x0400119B RID: 4507
	public const int ALL_FISH_COUNT = 9;

	// Token: 0x0400119C RID: 4508
	public const string ALL_FISH_ACHIEVEMENT_NAME = "PRO_ANGLER";

	// Token: 0x060018E9 RID: 6377 RVA: 0x000B841C File Offset: 0x000B661C
	public static void LoadFish()
	{
		if (FishLookup.AvailableFish != null)
		{
			if (FishLookup.lastShuffle > 5f)
			{
				FishLookup.AvailableFish.Shuffle((uint)UnityEngine.Random.Range(0, 10000));
			}
			return;
		}
		List<ItemModFishable> list = Pool.GetList<ItemModFishable>();
		List<ItemDefinition> list2 = Pool.GetList<ItemDefinition>();
		foreach (ItemDefinition itemDefinition in ItemManager.itemList)
		{
			ItemModFishable item;
			if (itemDefinition.TryGetComponent<ItemModFishable>(out item))
			{
				list.Add(item);
			}
			ItemModCompostable itemModCompostable;
			if (itemDefinition.TryGetComponent<ItemModCompostable>(out itemModCompostable) && itemModCompostable.BaitValue > 0f)
			{
				list2.Add(itemDefinition);
			}
		}
		FishLookup.AvailableFish = list.ToArray();
		FishLookup.BaitItems = list2.ToArray();
		Pool.FreeList<ItemModFishable>(ref list);
		Pool.FreeList<ItemDefinition>(ref list2);
	}

	// Token: 0x060018EA RID: 6378 RVA: 0x000B84F8 File Offset: 0x000B66F8
	public ItemDefinition GetFish(Vector3 worldPos, WaterBody bodyType, ItemDefinition lure, out ItemModFishable fishable, ItemModFishable ignoreFish)
	{
		FishLookup.LoadFish();
		ItemModCompostable itemModCompostable;
		float num = lure.TryGetComponent<ItemModCompostable>(out itemModCompostable) ? itemModCompostable.BaitValue : 0f;
		WaterBody.FishingTag fishingTag = (bodyType != null) ? bodyType.FishingType : WaterBody.FishingTag.Ocean;
		float num2 = WaterLevel.GetOverallWaterDepth(worldPos, true, null, true);
		if (worldPos.y < -10f)
		{
			num2 = 10f;
		}
		int num3 = UnityEngine.Random.Range(0, FishLookup.AvailableFish.Length);
		for (int i = 0; i < FishLookup.AvailableFish.Length; i++)
		{
			num3++;
			if (num3 >= FishLookup.AvailableFish.Length)
			{
				num3 = 0;
			}
			ItemModFishable itemModFishable = FishLookup.AvailableFish[num3];
			if (itemModFishable.CanBeFished && itemModFishable.MinimumBaitLevel <= num && (itemModFishable.MaximumBaitLevel <= 0f || num <= itemModFishable.MaximumBaitLevel) && !(itemModFishable == ignoreFish) && (itemModFishable.RequiredTag == (WaterBody.FishingTag)(-1) || (itemModFishable.RequiredTag & fishingTag) != (WaterBody.FishingTag)0) && ((fishingTag & WaterBody.FishingTag.Ocean) != WaterBody.FishingTag.Ocean || ((itemModFishable.MinimumWaterDepth <= 0f || num2 >= itemModFishable.MinimumWaterDepth) && (itemModFishable.MaximumWaterDepth <= 0f || num2 <= itemModFishable.MaximumWaterDepth))) && UnityEngine.Random.Range(0f, 1f) - num * 3f * 0.01f <= itemModFishable.Chance)
			{
				fishable = itemModFishable;
				return itemModFishable.GetComponent<ItemDefinition>();
			}
		}
		fishable = this.FallbackFish;
		return this.FallbackFish.GetComponent<ItemDefinition>();
	}

	// Token: 0x060018EB RID: 6379 RVA: 0x000B866C File Offset: 0x000B686C
	public void CheckCatchAllAchievement(BasePlayer player)
	{
		FishLookup.LoadFish();
		int num = 0;
		foreach (ItemModFishable itemModFishable in FishLookup.AvailableFish)
		{
			if (!string.IsNullOrEmpty(itemModFishable.SteamStatName) && player.stats.steam.Get(itemModFishable.SteamStatName) > 0)
			{
				num++;
			}
		}
		if (num == 9)
		{
			player.GiveAchievement("PRO_ANGLER");
		}
	}

	// Token: 0x060018EC RID: 6380 RVA: 0x000B86D2 File Offset: 0x000B68D2
	protected override Type GetIndexedType()
	{
		return typeof(FishLookup);
	}
}
