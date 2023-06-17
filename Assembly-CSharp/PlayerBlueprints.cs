using System;
using Facepunch;
using ProtoBuf;

// Token: 0x02000444 RID: 1092
public class PlayerBlueprints : EntityComponent<global::BasePlayer>
{
	// Token: 0x04001CAA RID: 7338
	public SteamInventory steamInventory;

	// Token: 0x06002479 RID: 9337 RVA: 0x000E7EC0 File Offset: 0x000E60C0
	internal void Reset()
	{
		PersistantPlayer persistantPlayerInfo = base.baseEntity.PersistantPlayerInfo;
		if (persistantPlayerInfo.unlockedItems != null)
		{
			persistantPlayerInfo.unlockedItems.Clear();
		}
		else
		{
			persistantPlayerInfo.unlockedItems = Pool.GetList<int>();
		}
		base.baseEntity.PersistantPlayerInfo = persistantPlayerInfo;
		base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600247A RID: 9338 RVA: 0x000E7F14 File Offset: 0x000E6114
	internal void UnlockAll()
	{
		PersistantPlayer persistantPlayerInfo = base.baseEntity.PersistantPlayerInfo;
		foreach (ItemBlueprint itemBlueprint in ItemManager.bpList)
		{
			if (itemBlueprint.userCraftable && !itemBlueprint.defaultBlueprint && !persistantPlayerInfo.unlockedItems.Contains(itemBlueprint.targetItem.itemid))
			{
				persistantPlayerInfo.unlockedItems.Add(itemBlueprint.targetItem.itemid);
			}
		}
		base.baseEntity.PersistantPlayerInfo = persistantPlayerInfo;
		base.baseEntity.SendNetworkUpdateImmediate(false);
		base.baseEntity.ClientRPCPlayer<int>(null, base.baseEntity, "UnlockedBlueprint", 0);
	}

	// Token: 0x0600247B RID: 9339 RVA: 0x000E7FDC File Offset: 0x000E61DC
	public bool IsUnlocked(ItemDefinition itemDef)
	{
		PersistantPlayer persistantPlayerInfo = base.baseEntity.PersistantPlayerInfo;
		return persistantPlayerInfo.unlockedItems != null && persistantPlayerInfo.unlockedItems.Contains(itemDef.itemid);
	}

	// Token: 0x0600247C RID: 9340 RVA: 0x000E8010 File Offset: 0x000E6210
	public void Unlock(ItemDefinition itemDef)
	{
		PersistantPlayer persistantPlayerInfo = base.baseEntity.PersistantPlayerInfo;
		if (!persistantPlayerInfo.unlockedItems.Contains(itemDef.itemid))
		{
			persistantPlayerInfo.unlockedItems.Add(itemDef.itemid);
			base.baseEntity.PersistantPlayerInfo = persistantPlayerInfo;
			base.baseEntity.SendNetworkUpdateImmediate(false);
			base.baseEntity.ClientRPCPlayer<int>(null, base.baseEntity, "UnlockedBlueprint", itemDef.itemid);
			base.baseEntity.stats.Add("blueprint_studied", 1, (Stats)5);
		}
	}

	// Token: 0x0600247D RID: 9341 RVA: 0x000E809C File Offset: 0x000E629C
	public bool HasUnlocked(ItemDefinition targetItem)
	{
		if (targetItem.Blueprint)
		{
			if (targetItem.Blueprint.NeedsSteamItem)
			{
				if (targetItem.steamItem != null && !this.steamInventory.HasItem(targetItem.steamItem.id))
				{
					return false;
				}
				if (base.baseEntity.UnlockAllSkins)
				{
					return true;
				}
				if (targetItem.steamItem == null)
				{
					bool flag = false;
					foreach (ItemSkinDirectory.Skin skin in targetItem.skins)
					{
						if (this.steamInventory.HasItem(skin.id))
						{
							flag = true;
							break;
						}
					}
					if (!flag && targetItem.skins2 != null)
					{
						foreach (IPlayerItemDefinition playerItemDefinition in targetItem.skins2)
						{
							if (this.steamInventory.HasItem(playerItemDefinition.DefinitionId))
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						return false;
					}
				}
				return true;
			}
			else if (targetItem.Blueprint.NeedsSteamDLC)
			{
				if (base.baseEntity.UnlockAllSkins)
				{
					return true;
				}
				if (targetItem.steamDlc != null && targetItem.steamDlc.HasLicense(base.baseEntity.userID))
				{
					return true;
				}
			}
		}
		int[] defaultBlueprints = ItemManager.defaultBlueprints;
		for (int i = 0; i < defaultBlueprints.Length; i++)
		{
			if (defaultBlueprints[i] == targetItem.itemid)
			{
				return true;
			}
		}
		return base.baseEntity.isServer && this.IsUnlocked(targetItem);
	}

	// Token: 0x0600247E RID: 9342 RVA: 0x000E820C File Offset: 0x000E640C
	public bool CanCraft(int itemid, int skinItemId, ulong playerId)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemid);
		return !(itemDefinition == null) && (skinItemId == 0 || base.baseEntity.UnlockAllSkins || this.CheckSkinOwnership(skinItemId, playerId)) && base.baseEntity.currentCraftLevel >= (float)itemDefinition.Blueprint.workbenchLevelRequired && this.HasUnlocked(itemDefinition);
	}

	// Token: 0x0600247F RID: 9343 RVA: 0x000E8270 File Offset: 0x000E6470
	public bool CheckSkinOwnership(int skinItemId, ulong playerId)
	{
		ItemSkinDirectory.Skin skin = ItemSkinDirectory.FindByInventoryDefinitionId(skinItemId);
		return (skin.invItem != null && skin.invItem.HasUnlocked(playerId)) || this.steamInventory.HasItem(skinItemId);
	}
}
