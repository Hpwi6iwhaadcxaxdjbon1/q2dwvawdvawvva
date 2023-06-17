using System;
using ConVar;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000F0 RID: 240
public class Workbench : StorageContainer
{
	// Token: 0x04000D6A RID: 3434
	public const int blueprintSlot = 0;

	// Token: 0x04000D6B RID: 3435
	public const int experimentSlot = 1;

	// Token: 0x04000D6C RID: 3436
	public bool Static;

	// Token: 0x04000D6D RID: 3437
	public int Workbenchlevel;

	// Token: 0x04000D6E RID: 3438
	public LootSpawn experimentalItems;

	// Token: 0x04000D6F RID: 3439
	public GameObjectRef experimentStartEffect;

	// Token: 0x04000D70 RID: 3440
	public GameObjectRef experimentSuccessEffect;

	// Token: 0x04000D71 RID: 3441
	public ItemDefinition experimentResource;

	// Token: 0x04000D72 RID: 3442
	public TechTreeData techTree;

	// Token: 0x04000D73 RID: 3443
	public bool supportsIndustrialCrafter;

	// Token: 0x04000D74 RID: 3444
	public static ItemDefinition blueprintBaseDef;

	// Token: 0x04000D75 RID: 3445
	private ItemDefinition pendingBlueprint;

	// Token: 0x04000D76 RID: 3446
	private bool creatingBlueprint;

	// Token: 0x0600151C RID: 5404 RVA: 0x000A74C8 File Offset: 0x000A56C8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Workbench.OnRpcMessage", 0))
		{
			if (rpc == 2308794761U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_BeginExperiment ");
				}
				using (TimeWarning.New("RPC_BeginExperiment", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2308794761U, "RPC_BeginExperiment", this, player, 3f))
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
							this.RPC_BeginExperiment(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_BeginExperiment");
					}
				}
				return true;
			}
			if (rpc == 4127240744U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_TechTreeUnlock ");
				}
				using (TimeWarning.New("RPC_TechTreeUnlock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(4127240744U, "RPC_TechTreeUnlock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_TechTreeUnlock(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_TechTreeUnlock");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600151D RID: 5405 RVA: 0x000A77C8 File Offset: 0x000A59C8
	public int GetScrapForExperiment()
	{
		if (this.Workbenchlevel == 1)
		{
			return 75;
		}
		if (this.Workbenchlevel == 2)
		{
			return 300;
		}
		if (this.Workbenchlevel == 3)
		{
			return 1000;
		}
		Debug.LogWarning("GetScrapForExperiment fucked up big time.");
		return 0;
	}

	// Token: 0x0600151E RID: 5406 RVA: 0x0002A4EC File Offset: 0x000286EC
	public bool IsWorking()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x0600151F RID: 5407 RVA: 0x000A77FF File Offset: 0x000A59FF
	public override bool CanPickup(global::BasePlayer player)
	{
		return this.children.Count == 0 && base.CanPickup(player);
	}

	// Token: 0x06001520 RID: 5408 RVA: 0x000A7818 File Offset: 0x000A5A18
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_TechTreeUnlock(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		int num = msg.read.Int32();
		TechTreeData.NodeInstance byID = this.techTree.GetByID(num);
		if (byID == null)
		{
			Debug.Log("Node for unlock not found :" + num);
			return;
		}
		if (this.techTree.PlayerCanUnlock(player, byID))
		{
			if (byID.IsGroup())
			{
				foreach (int id in byID.outputs)
				{
					TechTreeData.NodeInstance byID2 = this.techTree.GetByID(id);
					if (byID2 != null && byID2.itemDef != null)
					{
						player.blueprints.Unlock(byID2.itemDef);
						Analytics.Azure.OnBlueprintLearned(player, byID2.itemDef, "techtree", this);
					}
				}
				Debug.Log("Player unlocked group :" + byID.groupName);
				return;
			}
			if (byID.itemDef != null)
			{
				int num2 = global::ResearchTable.ScrapForResearch(byID.itemDef, global::ResearchTable.ResearchType.TechTree);
				int itemid = ItemManager.FindItemDefinition("scrap").itemid;
				if (player.inventory.GetAmount(itemid) >= num2)
				{
					player.inventory.Take(null, itemid, num2);
					player.blueprints.Unlock(byID.itemDef);
					Analytics.Azure.OnBlueprintLearned(player, byID.itemDef, "techtree", this);
				}
			}
		}
	}

	// Token: 0x06001521 RID: 5409 RVA: 0x000A798C File Offset: 0x000A5B8C
	public static ItemDefinition GetBlueprintTemplate()
	{
		if (Workbench.blueprintBaseDef == null)
		{
			Workbench.blueprintBaseDef = ItemManager.FindItemDefinition("blueprintbase");
		}
		return Workbench.blueprintBaseDef;
	}

	// Token: 0x06001522 RID: 5410 RVA: 0x000A79B0 File Offset: 0x000A5BB0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_BeginExperiment(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (this.IsWorking())
		{
			return;
		}
		PersistantPlayer persistantPlayerInfo = player.PersistantPlayerInfo;
		int num = UnityEngine.Random.Range(0, this.experimentalItems.subSpawn.Length);
		for (int i = 0; i < this.experimentalItems.subSpawn.Length; i++)
		{
			int num2 = i + num;
			if (num2 >= this.experimentalItems.subSpawn.Length)
			{
				num2 -= this.experimentalItems.subSpawn.Length;
			}
			ItemDefinition itemDef = this.experimentalItems.subSpawn[num2].category.items[0].itemDef;
			if (itemDef.Blueprint && !itemDef.Blueprint.defaultBlueprint && itemDef.Blueprint.userCraftable && itemDef.Blueprint.isResearchable && !itemDef.Blueprint.NeedsSteamItem && !itemDef.Blueprint.NeedsSteamDLC && !persistantPlayerInfo.unlockedItems.Contains(itemDef.itemid))
			{
				this.pendingBlueprint = itemDef;
				break;
			}
		}
		if (this.pendingBlueprint == null)
		{
			player.ChatMessage("You have already unlocked everything for this workbench tier.");
			return;
		}
		global::Item slot = base.inventory.GetSlot(0);
		if (slot != null)
		{
			if (!slot.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true))
			{
				slot.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
			}
			player.inventory.loot.SendImmediate();
		}
		if (this.experimentStartEffect.isValid)
		{
			Effect.server.Run(this.experimentStartEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		}
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.inventory.SetLocked(true);
		base.CancelInvoke(new Action(this.ExperimentComplete));
		base.Invoke(new Action(this.ExperimentComplete), 5f);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001523 RID: 5411 RVA: 0x000A7BB4 File Offset: 0x000A5DB4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
	}

	// Token: 0x06001524 RID: 5412 RVA: 0x000A7BBD File Offset: 0x000A5DBD
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		base.CancelInvoke(new Action(this.ExperimentComplete));
	}

	// Token: 0x06001525 RID: 5413 RVA: 0x000A7BD8 File Offset: 0x000A5DD8
	public int GetAvailableExperimentResources()
	{
		global::Item experimentResourceItem = this.GetExperimentResourceItem();
		if (experimentResourceItem == null || experimentResourceItem.info != this.experimentResource)
		{
			return 0;
		}
		return experimentResourceItem.amount;
	}

	// Token: 0x06001526 RID: 5414 RVA: 0x000A7C0A File Offset: 0x000A5E0A
	public global::Item GetExperimentResourceItem()
	{
		return base.inventory.GetSlot(1);
	}

	// Token: 0x06001527 RID: 5415 RVA: 0x000A7C18 File Offset: 0x000A5E18
	public void ExperimentComplete()
	{
		global::Item experimentResourceItem = this.GetExperimentResourceItem();
		int scrapForExperiment = this.GetScrapForExperiment();
		if (this.pendingBlueprint == null)
		{
			Debug.LogWarning("Pending blueprint was null!");
		}
		if (experimentResourceItem != null && experimentResourceItem.amount >= scrapForExperiment && this.pendingBlueprint != null)
		{
			experimentResourceItem.UseItem(scrapForExperiment);
			global::Item item = ItemManager.Create(Workbench.GetBlueprintTemplate(), 1, 0UL);
			item.blueprintTarget = this.pendingBlueprint.itemid;
			this.creatingBlueprint = true;
			if (!item.MoveToContainer(base.inventory, 0, true, false, null, true))
			{
				item.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
			}
			this.creatingBlueprint = false;
			if (this.experimentSuccessEffect.isValid)
			{
				Effect.server.Run(this.experimentSuccessEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
			}
		}
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		this.pendingBlueprint = null;
		base.inventory.SetLocked(false);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001528 RID: 5416 RVA: 0x000A7D20 File Offset: 0x000A5F20
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		if (base.inventory != null)
		{
			base.inventory.SetLocked(false);
		}
	}

	// Token: 0x06001529 RID: 5417 RVA: 0x0008D5E4 File Offset: 0x0008B7E4
	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.canAcceptItem = new Func<global::Item, int, bool>(this.ItemFilter);
	}

	// Token: 0x0600152A RID: 5418 RVA: 0x000A7D46 File Offset: 0x000A5F46
	public override bool ItemFilter(global::Item item, int targetSlot)
	{
		return (targetSlot == 1 && item.info == this.experimentResource) || (targetSlot == 0 && this.creatingBlueprint);
	}

	// Token: 0x0600152B RID: 5419 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool SupportsChildDeployables()
	{
		return true;
	}
}
