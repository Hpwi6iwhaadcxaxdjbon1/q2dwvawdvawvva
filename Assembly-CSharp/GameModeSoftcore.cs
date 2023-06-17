using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x02000514 RID: 1300
public class GameModeSoftcore : GameModeVanilla
{
	// Token: 0x0400216E RID: 8558
	public GameObjectRef reclaimManagerPrefab;

	// Token: 0x0400216F RID: 8559
	public GameObjectRef reclaimBackpackPrefab;

	// Token: 0x04002170 RID: 8560
	public static readonly Translate.Phrase ReclaimToast = new Translate.Phrase("softcore.reclaim", "You can reclaim some of your lost items by visiting the Outpost or Bandit Town.");

	// Token: 0x04002171 RID: 8561
	public ItemAmount[] startingGear;

	// Token: 0x04002172 RID: 8562
	[ServerVar]
	public static float reclaim_fraction_belt = 0.5f;

	// Token: 0x04002173 RID: 8563
	[ServerVar]
	public static float reclaim_fraction_wear = 0f;

	// Token: 0x04002174 RID: 8564
	[ServerVar]
	public static float reclaim_fraction_main = 0.5f;

	// Token: 0x0600296F RID: 10607 RVA: 0x000FE0AE File Offset: 0x000FC2AE
	protected override void OnCreated()
	{
		base.OnCreated();
		SingletonComponent<ServerMgr>.Instance.CreateImportantEntity<ReclaimManager>(this.reclaimManagerPrefab.resourcePath);
	}

	// Token: 0x06002970 RID: 10608 RVA: 0x000FE0CC File Offset: 0x000FC2CC
	public void AddFractionOfContainer(ItemContainer from, ref List<Item> to, float fraction = 1f, bool takeLastItem = false)
	{
		if (from.itemList.Count == 0)
		{
			return;
		}
		fraction = Mathf.Clamp01(fraction);
		int count = from.itemList.Count;
		float num = Mathf.Ceil((float)count * fraction);
		if (count == 1 && num == 1f && !takeLastItem)
		{
			return;
		}
		List<int> list = Facepunch.Pool.GetList<int>();
		for (int i = 0; i < from.capacity; i++)
		{
			if (from.GetSlot(i) != null)
			{
				list.Add(i);
			}
		}
		if (list.Count == 0)
		{
			Facepunch.Pool.FreeList<int>(ref list);
			return;
		}
		int num2 = 0;
		while ((float)num2 < num)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			Item item = from.GetSlot(list[index]);
			if (item.MaxStackable() > 1)
			{
				foreach (Item item2 in from.itemList)
				{
					if (item2.info == item.info && item2.amount < item.amount && !to.Contains(item2))
					{
						item = item2;
						for (int j = 0; j < list.Count; j++)
						{
							if (list[j] == item2.position)
							{
								index = j;
							}
						}
					}
				}
			}
			to.Add(item);
			list.RemoveAt(index);
			num2++;
		}
		Facepunch.Pool.FreeList<int>(ref list);
	}

	// Token: 0x06002971 RID: 10609 RVA: 0x000FE240 File Offset: 0x000FC440
	public List<Item> RemoveItemsFrom(ItemContainer itemContainer, ItemAmount[] types)
	{
		List<Item> list = Facepunch.Pool.GetList<Item>();
		foreach (ItemAmount itemAmount in types)
		{
			int num = 0;
			while ((float)num < itemAmount.amount)
			{
				Item item = itemContainer.FindItemByItemID(itemAmount.itemDef.itemid);
				if (item != null)
				{
					item.RemoveFromContainer();
					list.Add(item);
				}
				num++;
			}
		}
		return list;
	}

	// Token: 0x06002972 RID: 10610 RVA: 0x000FE2A4 File Offset: 0x000FC4A4
	public void ReturnItemsTo(ref List<Item> source, ItemContainer itemContainer)
	{
		foreach (Item item in source)
		{
			item.MoveToContainer(itemContainer, -1, true, false, null, true);
		}
		Facepunch.Pool.FreeList<Item>(ref source);
	}

	// Token: 0x06002973 RID: 10611 RVA: 0x000FE300 File Offset: 0x000FC500
	public override void OnPlayerDeath(BasePlayer instigator, BasePlayer victim, HitInfo deathInfo = null)
	{
		if (victim != null && !victim.IsNpc)
		{
			this.SetInventoryLocked(victim, false);
			int newID = 0;
			if (ReclaimManager.instance == null)
			{
				Debug.LogError("No reclaim manage for softcore");
				return;
			}
			List<Item> list = Facepunch.Pool.GetList<Item>();
			List<Item> list2 = this.RemoveItemsFrom(victim.inventory.containerBelt, this.startingGear);
			this.AddFractionOfContainer(victim.inventory.containerBelt, ref list, GameModeSoftcore.reclaim_fraction_belt, false);
			this.AddFractionOfContainer(victim.inventory.containerWear, ref list, GameModeSoftcore.reclaim_fraction_wear, false);
			this.AddFractionOfContainer(victim.inventory.containerMain, ref list, GameModeSoftcore.reclaim_fraction_main, false);
			if (list.Count > 0)
			{
				newID = ReclaimManager.instance.AddPlayerReclaim(victim.userID, list, (instigator == null) ? 0UL : instigator.userID, (instigator == null) ? "" : instigator.displayName, -1);
			}
			this.ReturnItemsTo(ref list2, victim.inventory.containerBelt);
			if (list.Count > 0)
			{
				Vector3 pos = victim.transform.position + Vector3.up * 0.25f;
				Quaternion rot = Quaternion.Euler(0f, victim.transform.eulerAngles.y, 0f);
				ReclaimBackpack component = GameManager.server.CreateEntity(this.reclaimBackpackPrefab.resourcePath, pos, rot, true).GetComponent<ReclaimBackpack>();
				component.InitForPlayer(victim.userID, newID);
				component.Spawn();
			}
			Facepunch.Pool.FreeList<Item>(ref list);
		}
		base.OnPlayerDeath(instigator, victim, deathInfo);
	}

	// Token: 0x06002974 RID: 10612 RVA: 0x000FE491 File Offset: 0x000FC691
	public override void OnPlayerRespawn(BasePlayer player)
	{
		base.OnPlayerRespawn(player);
		player.ShowToast(GameTip.Styles.Blue_Long, GameModeSoftcore.ReclaimToast, Array.Empty<string>());
	}

	// Token: 0x06002975 RID: 10613 RVA: 0x00030E8D File Offset: 0x0002F08D
	public override SleepingBag[] FindSleepingBagsForPlayer(ulong playerID, bool ignoreTimers)
	{
		return SleepingBag.FindForPlayer(playerID, ignoreTimers);
	}

	// Token: 0x06002976 RID: 10614 RVA: 0x000FE4AC File Offset: 0x000FC6AC
	public override float CorpseRemovalTime(BaseCorpse corpse)
	{
		foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
		{
			if (monumentInfo != null && monumentInfo.IsSafeZone && monumentInfo.Bounds.Contains(corpse.transform.position))
			{
				return 30f;
			}
		}
		return Server.corpsedespawn;
	}

	// Token: 0x06002977 RID: 10615 RVA: 0x000FE534 File Offset: 0x000FC734
	public void SetInventoryLocked(BasePlayer player, bool wantsLocked)
	{
		player.inventory.containerMain.SetLocked(wantsLocked);
		player.inventory.containerBelt.SetLocked(wantsLocked);
		player.inventory.containerWear.SetLocked(wantsLocked);
	}

	// Token: 0x06002978 RID: 10616 RVA: 0x000FE569 File Offset: 0x000FC769
	public override void OnPlayerWounded(BasePlayer instigator, BasePlayer victim, HitInfo info)
	{
		base.OnPlayerWounded(instigator, victim, info);
		this.SetInventoryLocked(victim, true);
	}

	// Token: 0x06002979 RID: 10617 RVA: 0x000FE57C File Offset: 0x000FC77C
	public override void OnPlayerRevived(BasePlayer instigator, BasePlayer victim)
	{
		this.SetInventoryLocked(victim, false);
		base.OnPlayerRevived(instigator, victim);
	}

	// Token: 0x0600297A RID: 10618 RVA: 0x000FE58E File Offset: 0x000FC78E
	public override bool CanMoveItemsFrom(PlayerInventory inv, BaseEntity source, Item item)
	{
		if (item.parent != null && item.parent.HasFlag(ItemContainer.Flag.IsPlayer))
		{
			return !item.parent.IsLocked();
		}
		return base.CanMoveItemsFrom(inv, source, item);
	}
}
