using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000BE RID: 190
public class ResearchTable : StorageContainer
{
	// Token: 0x04000AC1 RID: 2753
	[NonSerialized]
	public float researchFinishedTime;

	// Token: 0x04000AC2 RID: 2754
	public float researchCostFraction = 1f;

	// Token: 0x04000AC3 RID: 2755
	public float researchDuration = 10f;

	// Token: 0x04000AC4 RID: 2756
	public int requiredPaper = 10;

	// Token: 0x04000AC5 RID: 2757
	public GameObjectRef researchStartEffect;

	// Token: 0x04000AC6 RID: 2758
	public GameObjectRef researchFailEffect;

	// Token: 0x04000AC7 RID: 2759
	public GameObjectRef researchSuccessEffect;

	// Token: 0x04000AC8 RID: 2760
	public ItemDefinition researchResource;

	// Token: 0x04000AC9 RID: 2761
	private global::BasePlayer user;

	// Token: 0x06001123 RID: 4387 RVA: 0x0008D218 File Offset: 0x0008B418
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ResearchTable.OnRpcMessage", 0))
		{
			if (rpc == 3177710095U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoResearch ");
				}
				using (TimeWarning.New("DoResearch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3177710095U, "DoResearch", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.DoResearch(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in DoResearch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001124 RID: 4388 RVA: 0x0008D380 File Offset: 0x0008B580
	public override void ResetState()
	{
		base.ResetState();
		this.researchFinishedTime = 0f;
	}

	// Token: 0x06001125 RID: 4389 RVA: 0x0008D394 File Offset: 0x0008B594
	public override int GetIdealSlot(global::BasePlayer player, global::Item item)
	{
		if (item.info.shortname == "scrap")
		{
			global::Item slot = base.inventory.GetSlot(1);
			if (slot == null)
			{
				return 1;
			}
			if (slot.amount < item.MaxStackable())
			{
				return 1;
			}
		}
		return base.GetIdealSlot(player, item);
	}

	// Token: 0x06001126 RID: 4390 RVA: 0x0002A4EC File Offset: 0x000286EC
	public bool IsResearching()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06001127 RID: 4391 RVA: 0x0008D3E2 File Offset: 0x0008B5E2
	public int RarityMultiplier(Rarity rarity)
	{
		if (rarity == Rarity.Common)
		{
			return 20;
		}
		if (rarity == Rarity.Uncommon)
		{
			return 15;
		}
		if (rarity == Rarity.Rare)
		{
			return 10;
		}
		return 5;
	}

	// Token: 0x06001128 RID: 4392 RVA: 0x0008D3FC File Offset: 0x0008B5FC
	public int GetBlueprintStacksize(global::Item sourceItem)
	{
		int result = this.RarityMultiplier(sourceItem.info.rarity);
		if (sourceItem.info.category == ItemCategory.Ammunition)
		{
			result = Mathf.FloorToInt((float)sourceItem.MaxStackable() / (float)sourceItem.info.Blueprint.amountToCreate) * 2;
		}
		return result;
	}

	// Token: 0x06001129 RID: 4393 RVA: 0x0008D44B File Offset: 0x0008B64B
	public int ScrapForResearch(global::Item item)
	{
		return this.ScrapForResearch(item.info);
	}

	// Token: 0x0600112A RID: 4394 RVA: 0x0008D45C File Offset: 0x0008B65C
	public int ScrapForResearch(ItemDefinition info)
	{
		if (info.isRedirectOf != null)
		{
			return this.ScrapForResearch(info.isRedirectOf);
		}
		int result = 0;
		if (info.rarity == Rarity.Common)
		{
			result = 20;
		}
		if (info.rarity == Rarity.Uncommon)
		{
			result = 75;
		}
		if (info.rarity == Rarity.Rare)
		{
			result = 125;
		}
		if (info.rarity == Rarity.VeryRare || info.rarity == Rarity.None)
		{
			result = 500;
		}
		ItemBlueprint itemBlueprint = ItemManager.FindBlueprint(info);
		if (itemBlueprint != null && itemBlueprint.defaultBlueprint)
		{
			return ConVar.Server.defaultBlueprintResearchCost;
		}
		return result;
	}

	// Token: 0x0600112B RID: 4395 RVA: 0x0008D4E0 File Offset: 0x0008B6E0
	public static int ScrapForResearch(ItemDefinition info, global::ResearchTable.ResearchType type)
	{
		int num = 0;
		if (info.rarity == Rarity.Common)
		{
			num = 20;
		}
		if (info.rarity == Rarity.Uncommon)
		{
			num = 75;
		}
		if (info.rarity == Rarity.Rare)
		{
			num = 125;
		}
		if (info.rarity == Rarity.VeryRare || info.rarity == Rarity.None)
		{
			num = 500;
		}
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode != null)
		{
			BaseGameMode.ResearchCostResult scrapCostForResearch = activeGameMode.GetScrapCostForResearch(info, type);
			if (scrapCostForResearch.Scale != null)
			{
				num = Mathf.RoundToInt((float)num * scrapCostForResearch.Scale.Value);
			}
			else if (scrapCostForResearch.Amount != null)
			{
				num = scrapCostForResearch.Amount.Value;
			}
		}
		return num;
	}

	// Token: 0x0600112C RID: 4396 RVA: 0x0008D584 File Offset: 0x0008B784
	public bool IsItemResearchable(global::Item item)
	{
		ItemBlueprint itemBlueprint = ItemManager.FindBlueprint((item.info.isRedirectOf != null) ? item.info.isRedirectOf : item.info);
		return (itemBlueprint != null && itemBlueprint.defaultBlueprint) || (!(itemBlueprint == null) && itemBlueprint.isResearchable);
	}

	// Token: 0x0600112D RID: 4397 RVA: 0x0008D5E4 File Offset: 0x0008B7E4
	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.canAcceptItem = new Func<global::Item, int, bool>(this.ItemFilter);
	}

	// Token: 0x0600112E RID: 4398 RVA: 0x0008D604 File Offset: 0x0008B804
	public override bool ItemFilter(global::Item item, int targetSlot)
	{
		return (targetSlot != 1 || !(item.info != this.researchResource)) && base.ItemFilter(item, targetSlot);
	}

	// Token: 0x0600112F RID: 4399 RVA: 0x0008D627 File Offset: 0x0008B827
	public global::Item GetTargetItem()
	{
		return base.inventory.GetSlot(0);
	}

	// Token: 0x06001130 RID: 4400 RVA: 0x0008D638 File Offset: 0x0008B838
	public global::Item GetScrapItem()
	{
		global::Item slot = base.inventory.GetSlot(1);
		if (slot == null || slot.info != this.researchResource)
		{
			return null;
		}
		return slot;
	}

	// Token: 0x06001131 RID: 4401 RVA: 0x0008D66B File Offset: 0x0008B86B
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (base.HasFlag(global::BaseEntity.Flags.On))
		{
			base.Invoke(new Action(this.ResearchAttemptFinished), this.researchDuration);
		}
		base.inventory.SetLocked(false);
	}

	// Token: 0x06001132 RID: 4402 RVA: 0x0008D6A0 File Offset: 0x0008B8A0
	public override bool PlayerOpenLoot(global::BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		this.user = player;
		return base.PlayerOpenLoot(player, "", true);
	}

	// Token: 0x06001133 RID: 4403 RVA: 0x0008D6B6 File Offset: 0x0008B8B6
	public override void PlayerStoppedLooting(global::BasePlayer player)
	{
		this.user = null;
		base.PlayerStoppedLooting(player);
	}

	// Token: 0x06001134 RID: 4404 RVA: 0x0008D6C8 File Offset: 0x0008B8C8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void DoResearch(global::BaseEntity.RPCMessage msg)
	{
		if (this.IsResearching())
		{
			return;
		}
		global::BasePlayer player = msg.player;
		global::Item targetItem = this.GetTargetItem();
		if (targetItem == null)
		{
			return;
		}
		if (targetItem.amount > 1)
		{
			return;
		}
		if (!this.IsItemResearchable(targetItem))
		{
			return;
		}
		targetItem.CollectedForCrafting(player);
		this.researchFinishedTime = UnityEngine.Time.realtimeSinceStartup + this.researchDuration;
		base.Invoke(new Action(this.ResearchAttemptFinished), this.researchDuration);
		base.inventory.SetLocked(true);
		int scrapCost = this.ScrapForResearch(targetItem);
		Analytics.Azure.OnResearchStarted(player, this, targetItem, scrapCost);
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		player.inventory.loot.SendImmediate();
		if (this.researchStartEffect.isValid)
		{
			Effect.server.Run(this.researchStartEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		}
		msg.player.GiveAchievement("RESEARCH_ITEM");
	}

	// Token: 0x06001135 RID: 4405 RVA: 0x0008D7B0 File Offset: 0x0008B9B0
	public void ResearchAttemptFinished()
	{
		global::Item targetItem = this.GetTargetItem();
		global::Item scrapItem = this.GetScrapItem();
		if (targetItem != null && scrapItem != null)
		{
			int num = this.ScrapForResearch(targetItem);
			if (scrapItem.amount >= num)
			{
				if (scrapItem.amount == num)
				{
					base.inventory.Remove(scrapItem);
					scrapItem.RemoveFromContainer();
					scrapItem.Remove(0f);
				}
				else
				{
					scrapItem.UseItem(num);
				}
				base.inventory.Remove(targetItem);
				targetItem.Remove(0f);
				global::Item item = ItemManager.Create(ItemManager.blueprintBaseDef, 1, 0UL);
				item.blueprintTarget = ((targetItem.info.isRedirectOf != null) ? targetItem.info.isRedirectOf.itemid : targetItem.info.itemid);
				if (!item.MoveToContainer(base.inventory, 0, true, false, null, true))
				{
					item.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
				}
				if (this.researchSuccessEffect.isValid)
				{
					Effect.server.Run(this.researchSuccessEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
				}
			}
		}
		base.SendNetworkUpdateImmediate(false);
		if (this.user != null)
		{
			this.user.inventory.loot.SendImmediate();
		}
		this.EndResearch();
	}

	// Token: 0x06001136 RID: 4406 RVA: 0x000063A5 File Offset: 0x000045A5
	public void CancelResearch()
	{
	}

	// Token: 0x06001137 RID: 4407 RVA: 0x0008D904 File Offset: 0x0008BB04
	public void EndResearch()
	{
		base.inventory.SetLocked(false);
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		this.researchFinishedTime = 0f;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		if (this.user != null)
		{
			this.user.inventory.loot.SendImmediate();
		}
	}

	// Token: 0x06001138 RID: 4408 RVA: 0x0008D95C File Offset: 0x0008BB5C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.researchTable = Facepunch.Pool.Get<ProtoBuf.ResearchTable>();
		info.msg.researchTable.researchTimeLeft = this.researchFinishedTime - UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x06001139 RID: 4409 RVA: 0x0008D991 File Offset: 0x0008BB91
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.researchTable != null)
		{
			this.researchFinishedTime = UnityEngine.Time.realtimeSinceStartup + info.msg.researchTable.researchTimeLeft;
		}
	}

	// Token: 0x02000BF5 RID: 3061
	public enum ResearchType
	{
		// Token: 0x04004171 RID: 16753
		ResearchTable,
		// Token: 0x04004172 RID: 16754
		TechTree
	}
}
