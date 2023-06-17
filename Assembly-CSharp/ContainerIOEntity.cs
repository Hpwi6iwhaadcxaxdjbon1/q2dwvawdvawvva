using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000061 RID: 97
public class ContainerIOEntity : global::IOEntity, IItemContainerEntity, IIdealSlotEntity, ILootableEntity, LootPanel.IHasLootPanel, IContainerSounds
{
	// Token: 0x040006AA RID: 1706
	public ItemDefinition onlyAllowedItem;

	// Token: 0x040006AB RID: 1707
	public global::ItemContainer.ContentsType allowedContents = global::ItemContainer.ContentsType.Generic;

	// Token: 0x040006AC RID: 1708
	public int maxStackSize = 1;

	// Token: 0x040006AD RID: 1709
	public int numSlots;

	// Token: 0x040006AE RID: 1710
	public string lootPanelName = "generic";

	// Token: 0x040006AF RID: 1711
	public Translate.Phrase panelTitle = new Translate.Phrase("loot", "Loot");

	// Token: 0x040006B0 RID: 1712
	public bool needsBuildingPrivilegeToUse;

	// Token: 0x040006B1 RID: 1713
	public bool isLootable = true;

	// Token: 0x040006B2 RID: 1714
	public bool dropsLoot;

	// Token: 0x040006B3 RID: 1715
	public bool dropFloats;

	// Token: 0x040006B4 RID: 1716
	public bool onlyOneUser;

	// Token: 0x040006B5 RID: 1717
	public SoundDefinition openSound;

	// Token: 0x040006B6 RID: 1718
	public SoundDefinition closeSound;

	// Token: 0x060009FC RID: 2556 RVA: 0x0005D8AC File Offset: 0x0005BAAC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ContainerIOEntity.OnRpcMessage", 0))
		{
			if (rpc == 331989034U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenLoot ");
				}
				using (TimeWarning.New("RPC_OpenLoot", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(331989034U, "RPC_OpenLoot", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenLoot(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_OpenLoot");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170000FF RID: 255
	// (get) Token: 0x060009FD RID: 2557 RVA: 0x0005DA14 File Offset: 0x0005BC14
	public Translate.Phrase LootPanelTitle
	{
		get
		{
			return this.panelTitle;
		}
	}

	// Token: 0x17000100 RID: 256
	// (get) Token: 0x060009FE RID: 2558 RVA: 0x0005DA1C File Offset: 0x0005BC1C
	// (set) Token: 0x060009FF RID: 2559 RVA: 0x0005DA24 File Offset: 0x0005BC24
	public global::ItemContainer inventory { get; private set; }

	// Token: 0x17000101 RID: 257
	// (get) Token: 0x06000A00 RID: 2560 RVA: 0x0005DA2D File Offset: 0x0005BC2D
	public Transform Transform
	{
		get
		{
			return base.transform;
		}
	}

	// Token: 0x17000102 RID: 258
	// (get) Token: 0x06000A01 RID: 2561 RVA: 0x0005DA35 File Offset: 0x0005BC35
	public bool DropsLoot
	{
		get
		{
			return this.dropsLoot;
		}
	}

	// Token: 0x17000103 RID: 259
	// (get) Token: 0x06000A02 RID: 2562 RVA: 0x0005DA3D File Offset: 0x0005BC3D
	public bool DropFloats
	{
		get
		{
			return this.dropFloats;
		}
	}

	// Token: 0x17000104 RID: 260
	// (get) Token: 0x06000A03 RID: 2563 RVA: 0x00029CA8 File Offset: 0x00027EA8
	public float DestroyLootPercent
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x17000105 RID: 261
	// (get) Token: 0x06000A04 RID: 2564 RVA: 0x0005DA45 File Offset: 0x0005BC45
	// (set) Token: 0x06000A05 RID: 2565 RVA: 0x0005DA4D File Offset: 0x0005BC4D
	public ulong LastLootedBy { get; set; }

	// Token: 0x06000A06 RID: 2566 RVA: 0x0005DA56 File Offset: 0x0005BC56
	public override bool CanPickup(global::BasePlayer player)
	{
		return (!this.pickup.requireEmptyInv || this.inventory == null || this.inventory.itemList.Count == 0) && base.CanPickup(player);
	}

	// Token: 0x06000A07 RID: 2567 RVA: 0x0005DA88 File Offset: 0x0005BC88
	public override void ServerInit()
	{
		if (this.inventory == null)
		{
			this.CreateInventory(true);
			this.OnInventoryFirstCreated(this.inventory);
		}
		base.ServerInit();
	}

	// Token: 0x06000A08 RID: 2568 RVA: 0x0005DAAB File Offset: 0x0005BCAB
	public override void PreServerLoad()
	{
		base.PreServerLoad();
		this.CreateInventory(false);
	}

	// Token: 0x06000A09 RID: 2569 RVA: 0x0005DABA File Offset: 0x0005BCBA
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.inventory != null && !this.inventory.uid.IsValid)
		{
			this.inventory.GiveUID();
		}
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x0005DAF4 File Offset: 0x0005BCF4
	public void CreateInventory(bool giveUID)
	{
		this.inventory = new global::ItemContainer();
		this.inventory.entityOwner = this;
		this.inventory.allowedContents = ((this.allowedContents == (global::ItemContainer.ContentsType)0) ? global::ItemContainer.ContentsType.Generic : this.allowedContents);
		this.inventory.SetOnlyAllowedItem(this.onlyAllowedItem);
		this.inventory.maxStackSize = this.maxStackSize;
		this.inventory.ServerInitialize(null, this.numSlots);
		if (giveUID)
		{
			this.inventory.GiveUID();
		}
		this.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
		this.inventory.onDirty += this.OnInventoryDirty;
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x0005DBA8 File Offset: 0x0005BDA8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			if (this.inventory != null)
			{
				info.msg.storageBox = Facepunch.Pool.Get<StorageBox>();
				info.msg.storageBox.contents = this.inventory.Save();
				return;
			}
			Debug.LogWarning("Storage container without inventory: " + this.ToString());
		}
	}

	// Token: 0x06000A0C RID: 2572 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnInventoryFirstCreated(global::ItemContainer container)
	{
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnItemAddedOrRemoved(global::Item item, bool added)
	{
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnInventoryDirty()
	{
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x0005DC0D File Offset: 0x0005BE0D
	public override void OnKilled(HitInfo info)
	{
		this.DropItems(null);
		base.OnKilled(info);
	}

	// Token: 0x06000A10 RID: 2576 RVA: 0x0005DC1D File Offset: 0x0005BE1D
	public void DropItems(global::BaseEntity initiator = null)
	{
		StorageContainer.DropItems(this, initiator);
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x0005DC28 File Offset: 0x0005BE28
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(global::BaseEntity.RPCMessage rpc)
	{
		if (this.inventory == null)
		{
			return;
		}
		global::BasePlayer player = rpc.player;
		if (!player || !player.CanInteract())
		{
			return;
		}
		this.PlayerOpenLoot(player, this.lootPanelName, true);
	}

	// Token: 0x06000A12 RID: 2578 RVA: 0x0005DC68 File Offset: 0x0005BE68
	public virtual bool PlayerOpenLoot(global::BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		if (this.needsBuildingPrivilegeToUse && !player.CanBuild())
		{
			return false;
		}
		if (this.onlyOneUser && base.IsOpen())
		{
			player.ChatMessage("Already in use");
			return false;
		}
		if (panelToOpen == "")
		{
			panelToOpen = this.lootPanelName;
		}
		if (player.inventory.loot.StartLootingEntity(this, doPositionChecks))
		{
			base.SetFlag(global::BaseEntity.Flags.Open, true, false, true);
			player.inventory.loot.AddContainer(this.inventory);
			player.inventory.loot.SendImmediate();
			player.ClientRPCPlayer<string>(null, player, "RPC_OpenLootPanel", this.lootPanelName);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			return true;
		}
		return false;
	}

	// Token: 0x06000A13 RID: 2579 RVA: 0x0005DD1A File Offset: 0x0005BF1A
	public virtual void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x00007A3C File Offset: 0x00005C3C
	public bool ShouldDropItemsIndividually()
	{
		return false;
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x000445C9 File Offset: 0x000427C9
	public virtual int GetIdealSlot(global::BasePlayer player, global::Item item)
	{
		return -1;
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x0005DD30 File Offset: 0x0005BF30
	public virtual ItemContainerId GetIdealContainer(global::BasePlayer player, global::Item item, bool altMove)
	{
		return default(ItemContainerId);
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void DropBonusItems(global::BaseEntity initiator, global::ItemContainer container)
	{
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x0005DD46 File Offset: 0x0005BF46
	public bool OccupiedCheck(global::BasePlayer player = null)
	{
		return (player != null && player.inventory.loot.entitySource == this) || !this.onlyOneUser || !base.IsOpen();
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x0005DD80 File Offset: 0x0005BF80
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.storageBox != null)
		{
			if (this.inventory != null)
			{
				this.inventory.Load(info.msg.storageBox.contents);
				this.inventory.capacity = this.numSlots;
				return;
			}
			Debug.LogWarning("Storage container without inventory: " + this.ToString());
		}
	}
}
