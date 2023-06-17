using System;
using Facepunch.Rust;
using Rust;
using UnityEngine;

// Token: 0x02000415 RID: 1045
public class LootContainer : StorageContainer
{
	// Token: 0x04001B4E RID: 6990
	public bool destroyOnEmpty = true;

	// Token: 0x04001B4F RID: 6991
	public LootSpawn lootDefinition;

	// Token: 0x04001B50 RID: 6992
	public int maxDefinitionsToSpawn;

	// Token: 0x04001B51 RID: 6993
	public float minSecondsBetweenRefresh = 3600f;

	// Token: 0x04001B52 RID: 6994
	public float maxSecondsBetweenRefresh = 7200f;

	// Token: 0x04001B53 RID: 6995
	public bool initialLootSpawn = true;

	// Token: 0x04001B54 RID: 6996
	public float xpLootedScale = 1f;

	// Token: 0x04001B55 RID: 6997
	public float xpDestroyedScale = 1f;

	// Token: 0x04001B56 RID: 6998
	public bool BlockPlayerItemInput;

	// Token: 0x04001B57 RID: 6999
	public int scrapAmount;

	// Token: 0x04001B58 RID: 7000
	public string deathStat = "";

	// Token: 0x04001B59 RID: 7001
	public LootContainer.LootSpawnSlot[] LootSpawnSlots;

	// Token: 0x04001B5A RID: 7002
	public LootContainer.spawnType SpawnType;

	// Token: 0x04001B5B RID: 7003
	public bool FirstLooted;

	// Token: 0x04001B5C RID: 7004
	private static ItemDefinition scrapDef;

	// Token: 0x170002F4 RID: 756
	// (get) Token: 0x06002342 RID: 9026 RVA: 0x000E13B3 File Offset: 0x000DF5B3
	public bool shouldRefreshContents
	{
		get
		{
			return this.minSecondsBetweenRefresh > 0f && this.maxSecondsBetweenRefresh > 0f;
		}
	}

	// Token: 0x06002343 RID: 9027 RVA: 0x000E13D1 File Offset: 0x000DF5D1
	public override void ResetState()
	{
		this.FirstLooted = false;
		base.ResetState();
	}

	// Token: 0x06002344 RID: 9028 RVA: 0x000E13E0 File Offset: 0x000DF5E0
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.initialLootSpawn)
		{
			this.SpawnLoot();
		}
		if (this.BlockPlayerItemInput && !Rust.Application.isLoadingSave && base.inventory != null)
		{
			base.inventory.SetFlag(ItemContainer.Flag.NoItemInput, true);
		}
		base.SetFlag(BaseEntity.Flags.Reserved6, PlayerInventory.IsBirthday(), false, true);
	}

	// Token: 0x06002345 RID: 9029 RVA: 0x000E143B File Offset: 0x000DF63B
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.BlockPlayerItemInput && base.inventory != null)
		{
			base.inventory.SetFlag(ItemContainer.Flag.NoItemInput, true);
		}
	}

	// Token: 0x06002346 RID: 9030 RVA: 0x000E1464 File Offset: 0x000DF664
	public virtual void SpawnLoot()
	{
		if (base.inventory == null)
		{
			Debug.Log("CONTACT DEVELOPERS! LootContainer::PopulateLoot has null inventory!!!");
			return;
		}
		base.inventory.Clear();
		ItemManager.DoRemoves();
		this.PopulateLoot();
		if (this.shouldRefreshContents)
		{
			base.Invoke(new Action(this.SpawnLoot), UnityEngine.Random.Range(this.minSecondsBetweenRefresh, this.maxSecondsBetweenRefresh));
		}
	}

	// Token: 0x06002347 RID: 9031 RVA: 0x000E14C6 File Offset: 0x000DF6C6
	public int ScoreForRarity(Rarity rarity)
	{
		switch (rarity)
		{
		case Rarity.Common:
			return 1;
		case Rarity.Uncommon:
			return 2;
		case Rarity.Rare:
			return 3;
		case Rarity.VeryRare:
			return 4;
		default:
			return 5000;
		}
	}

	// Token: 0x06002348 RID: 9032 RVA: 0x000E14F0 File Offset: 0x000DF6F0
	public virtual void PopulateLoot()
	{
		if (this.LootSpawnSlots.Length != 0)
		{
			foreach (LootContainer.LootSpawnSlot lootSpawnSlot in this.LootSpawnSlots)
			{
				for (int j = 0; j < lootSpawnSlot.numberToSpawn; j++)
				{
					if (UnityEngine.Random.Range(0f, 1f) <= lootSpawnSlot.probability)
					{
						lootSpawnSlot.definition.SpawnIntoContainer(base.inventory);
					}
				}
			}
		}
		else if (this.lootDefinition != null)
		{
			for (int k = 0; k < this.maxDefinitionsToSpawn; k++)
			{
				this.lootDefinition.SpawnIntoContainer(base.inventory);
			}
		}
		if (this.SpawnType == LootContainer.spawnType.ROADSIDE || this.SpawnType == LootContainer.spawnType.TOWN)
		{
			foreach (Item item in base.inventory.itemList)
			{
				if (item.hasCondition)
				{
					item.condition = UnityEngine.Random.Range(item.info.condition.foundCondition.fractionMin, item.info.condition.foundCondition.fractionMax) * item.info.condition.max;
				}
			}
		}
		this.GenerateScrap();
	}

	// Token: 0x06002349 RID: 9033 RVA: 0x000E1648 File Offset: 0x000DF848
	public void GenerateScrap()
	{
		if (this.scrapAmount <= 0)
		{
			return;
		}
		if (LootContainer.scrapDef == null)
		{
			LootContainer.scrapDef = ItemManager.FindItemDefinition("scrap");
		}
		int num = this.scrapAmount;
		if (num > 0)
		{
			Item item = ItemManager.Create(LootContainer.scrapDef, num, 0UL);
			if (!item.MoveToContainer(base.inventory, -1, true, false, null, true))
			{
				item.Drop(base.transform.position, this.GetInheritedDropVelocity(), default(Quaternion));
			}
		}
	}

	// Token: 0x0600234A RID: 9034 RVA: 0x000E16C8 File Offset: 0x000DF8C8
	public override void DropBonusItems(BaseEntity initiator, ItemContainer container)
	{
		base.DropBonusItems(initiator, container);
		if (initiator == null || container == null)
		{
			return;
		}
		BasePlayer basePlayer = initiator as BasePlayer;
		if (basePlayer == null)
		{
			return;
		}
		if (this.scrapAmount > 0 && LootContainer.scrapDef != null)
		{
			float num = (basePlayer.modifiers != null) ? (1f + basePlayer.modifiers.GetValue(Modifier.ModifierType.Scrap_Yield, 0f)) : 0f;
			if (num > 1f)
			{
				float num2 = basePlayer.modifiers.GetVariableValue(Modifier.ModifierType.Scrap_Yield, 0f);
				float num3 = Mathf.Max((float)this.scrapAmount * num - (float)this.scrapAmount, 0f);
				num2 += num3;
				int num4 = 0;
				if (num2 >= 1f)
				{
					num4 = (int)num2;
					num2 -= (float)num4;
				}
				basePlayer.modifiers.SetVariableValue(Modifier.ModifierType.Scrap_Yield, num2);
				if (num4 > 0)
				{
					Item item = ItemManager.Create(LootContainer.scrapDef, num4, 0UL);
					if (item != null)
					{
						(item.Drop(this.GetDropPosition() + new Vector3(0f, 0.5f, 0f), this.GetInheritedDropVelocity(), default(Quaternion)) as DroppedItem).DropReason = DroppedItem.DropReasonEnum.Loot;
					}
				}
			}
		}
	}

	// Token: 0x0600234B RID: 9035 RVA: 0x000E17FD File Offset: 0x000DF9FD
	public override bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		if (!this.FirstLooted)
		{
			this.FirstLooted = true;
			Analytics.Azure.OnFirstLooted(this, baseEntity);
		}
		return base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x0600234C RID: 9036 RVA: 0x000E181C File Offset: 0x000DFA1C
	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		if (this.destroyOnEmpty && (base.inventory == null || base.inventory.itemList == null || base.inventory.itemList.Count == 0))
		{
			base.Kill(BaseNetworkable.DestroyMode.Gib);
		}
	}

	// Token: 0x0600234D RID: 9037 RVA: 0x00029A3C File Offset: 0x00027C3C
	public void RemoveMe()
	{
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x0600234E RID: 9038 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ShouldDropItemsIndividually()
	{
		return true;
	}

	// Token: 0x0600234F RID: 9039 RVA: 0x000E185C File Offset: 0x000DFA5C
	public override void OnKilled(HitInfo info)
	{
		Analytics.Azure.OnLootContainerDestroyed(this, info.InitiatorPlayer, info.Weapon);
		base.OnKilled(info);
		if (info != null && info.InitiatorPlayer != null && !string.IsNullOrEmpty(this.deathStat))
		{
			info.InitiatorPlayer.stats.Add(this.deathStat, 1, Stats.Life);
		}
	}

	// Token: 0x06002350 RID: 9040 RVA: 0x000E18B8 File Offset: 0x000DFAB8
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
	}

	// Token: 0x06002351 RID: 9041 RVA: 0x0004D153 File Offset: 0x0004B353
	public override void InitShared()
	{
		base.InitShared();
	}

	// Token: 0x02000CD8 RID: 3288
	public enum spawnType
	{
		// Token: 0x04004521 RID: 17697
		GENERIC,
		// Token: 0x04004522 RID: 17698
		PLAYER,
		// Token: 0x04004523 RID: 17699
		TOWN,
		// Token: 0x04004524 RID: 17700
		AIRDROP,
		// Token: 0x04004525 RID: 17701
		CRASHSITE,
		// Token: 0x04004526 RID: 17702
		ROADSIDE
	}

	// Token: 0x02000CD9 RID: 3289
	[Serializable]
	public struct LootSpawnSlot
	{
		// Token: 0x04004527 RID: 17703
		public LootSpawn definition;

		// Token: 0x04004528 RID: 17704
		public int numberToSpawn;

		// Token: 0x04004529 RID: 17705
		public float probability;
	}
}
