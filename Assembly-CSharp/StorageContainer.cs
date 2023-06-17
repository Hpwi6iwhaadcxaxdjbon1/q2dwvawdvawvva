using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000DB RID: 219
public class StorageContainer : global::DecayEntity, IItemContainerEntity, IIdealSlotEntity, ILootableEntity, LootPanel.IHasLootPanel, IContainerSounds, global::PlayerInventory.ICanMoveFrom
{
	// Token: 0x04000C05 RID: 3077
	[Header("Storage Container")]
	public static readonly Translate.Phrase LockedMessage = new Translate.Phrase("storage.locked", "Can't loot right now");

	// Token: 0x04000C06 RID: 3078
	public static readonly Translate.Phrase InUseMessage = new Translate.Phrase("storage.in_use", "Already in use");

	// Token: 0x04000C07 RID: 3079
	public int inventorySlots = 6;

	// Token: 0x04000C08 RID: 3080
	public bool dropsLoot = true;

	// Token: 0x04000C09 RID: 3081
	public float dropLootDestroyPercent;

	// Token: 0x04000C0A RID: 3082
	public bool dropFloats;

	// Token: 0x04000C0B RID: 3083
	public bool isLootable = true;

	// Token: 0x04000C0C RID: 3084
	public bool isLockable = true;

	// Token: 0x04000C0D RID: 3085
	public bool isMonitorable;

	// Token: 0x04000C0E RID: 3086
	public string panelName = "generic";

	// Token: 0x04000C0F RID: 3087
	public Translate.Phrase panelTitle = new Translate.Phrase("loot", "Loot");

	// Token: 0x04000C10 RID: 3088
	public global::ItemContainer.ContentsType allowedContents;

	// Token: 0x04000C11 RID: 3089
	public ItemDefinition allowedItem;

	// Token: 0x04000C12 RID: 3090
	public ItemDefinition allowedItem2;

	// Token: 0x04000C13 RID: 3091
	public int maxStackSize;

	// Token: 0x04000C14 RID: 3092
	public bool needsBuildingPrivilegeToUse;

	// Token: 0x04000C15 RID: 3093
	public bool mustBeMountedToUse;

	// Token: 0x04000C16 RID: 3094
	public SoundDefinition openSound;

	// Token: 0x04000C17 RID: 3095
	public SoundDefinition closeSound;

	// Token: 0x04000C18 RID: 3096
	[Header("Item Dropping")]
	public Vector3 dropPosition;

	// Token: 0x04000C19 RID: 3097
	public Vector3 dropVelocity = Vector3.forward;

	// Token: 0x04000C1A RID: 3098
	public ItemCategory onlyAcceptCategory = ItemCategory.All;

	// Token: 0x04000C1B RID: 3099
	public bool onlyOneUser;

	// Token: 0x06001332 RID: 4914 RVA: 0x0009A548 File Offset: 0x00098748
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("StorageContainer.OnRpcMessage", 0))
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

	// Token: 0x170001BA RID: 442
	// (get) Token: 0x06001333 RID: 4915 RVA: 0x0009A6B0 File Offset: 0x000988B0
	public Translate.Phrase LootPanelTitle
	{
		get
		{
			return this.panelTitle;
		}
	}

	// Token: 0x06001334 RID: 4916 RVA: 0x0009A6B8 File Offset: 0x000988B8
	public override void ResetState()
	{
		base.ResetState();
		if (base.isServer && this.inventory != null)
		{
			this.inventory.Clear();
			this.inventory = null;
		}
	}

	// Token: 0x06001335 RID: 4917 RVA: 0x0009A6E4 File Offset: 0x000988E4
	public virtual void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(this.dropPosition, Vector3.one * 0.1f);
		Gizmos.color = Color.white;
		Gizmos.DrawRay(this.dropPosition, this.dropVelocity);
	}

	// Token: 0x170001BB RID: 443
	// (get) Token: 0x06001336 RID: 4918 RVA: 0x0009A740 File Offset: 0x00098940
	// (set) Token: 0x06001337 RID: 4919 RVA: 0x0009A748 File Offset: 0x00098948
	public global::ItemContainer inventory { get; private set; }

	// Token: 0x170001BC RID: 444
	// (get) Token: 0x06001338 RID: 4920 RVA: 0x0005DA2D File Offset: 0x0005BC2D
	public Transform Transform
	{
		get
		{
			return base.transform;
		}
	}

	// Token: 0x170001BD RID: 445
	// (get) Token: 0x06001339 RID: 4921 RVA: 0x0009A751 File Offset: 0x00098951
	public bool DropsLoot
	{
		get
		{
			return this.dropsLoot;
		}
	}

	// Token: 0x170001BE RID: 446
	// (get) Token: 0x0600133A RID: 4922 RVA: 0x0009A759 File Offset: 0x00098959
	public bool DropFloats
	{
		get
		{
			return this.dropFloats;
		}
	}

	// Token: 0x170001BF RID: 447
	// (get) Token: 0x0600133B RID: 4923 RVA: 0x0009A761 File Offset: 0x00098961
	public float DestroyLootPercent
	{
		get
		{
			return this.dropLootDestroyPercent;
		}
	}

	// Token: 0x170001C0 RID: 448
	// (get) Token: 0x0600133C RID: 4924 RVA: 0x0009A769 File Offset: 0x00098969
	// (set) Token: 0x0600133D RID: 4925 RVA: 0x0009A771 File Offset: 0x00098971
	public ulong LastLootedBy { get; set; }

	// Token: 0x0600133E RID: 4926 RVA: 0x0009A77A File Offset: 0x0009897A
	public bool MoveAllInventoryItems(global::ItemContainer from)
	{
		return StorageContainer.MoveAllInventoryItems(from, this.inventory);
	}

	// Token: 0x0600133F RID: 4927 RVA: 0x0009A788 File Offset: 0x00098988
	public static bool MoveAllInventoryItems(global::ItemContainer source, global::ItemContainer dest)
	{
		bool flag = true;
		for (int i = 0; i < Mathf.Min(source.capacity, dest.capacity); i++)
		{
			global::Item slot = source.GetSlot(i);
			if (slot != null)
			{
				bool flag2 = slot.MoveToContainer(dest, -1, true, false, null, true);
				if (flag && !flag2)
				{
					flag = false;
				}
			}
		}
		return flag;
	}

	// Token: 0x06001340 RID: 4928 RVA: 0x0009A7D4 File Offset: 0x000989D4
	public virtual void ReceiveInventoryFromItem(global::Item item)
	{
		if (item.contents != null)
		{
			StorageContainer.MoveAllInventoryItems(item.contents, this.inventory);
		}
	}

	// Token: 0x06001341 RID: 4929 RVA: 0x0009A7F0 File Offset: 0x000989F0
	public override bool CanPickup(global::BasePlayer player)
	{
		bool flag = base.GetSlot(global::BaseEntity.Slot.Lock) != null;
		if (base.isClient)
		{
			return base.CanPickup(player) && !flag;
		}
		return (!this.pickup.requireEmptyInv || this.inventory == null || this.inventory.itemList.Count == 0) && base.CanPickup(player) && !flag;
	}

	// Token: 0x06001342 RID: 4930 RVA: 0x0009A85C File Offset: 0x00098A5C
	public override void OnPickedUp(global::Item createdItem, global::BasePlayer player)
	{
		base.OnPickedUp(createdItem, player);
		if (createdItem != null && createdItem.contents != null)
		{
			StorageContainer.MoveAllInventoryItems(this.inventory, createdItem.contents);
			return;
		}
		for (int i = 0; i < this.inventory.capacity; i++)
		{
			global::Item slot = this.inventory.GetSlot(i);
			if (slot != null)
			{
				slot.RemoveFromContainer();
				player.GiveItem(slot, global::BaseEntity.GiveItemReason.PickedUp);
			}
		}
	}

	// Token: 0x06001343 RID: 4931 RVA: 0x0009A8C3 File Offset: 0x00098AC3
	public override void ServerInit()
	{
		if (this.inventory == null)
		{
			this.CreateInventory(true);
			this.OnInventoryFirstCreated(this.inventory);
		}
		base.ServerInit();
	}

	// Token: 0x06001344 RID: 4932 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnInventoryFirstCreated(global::ItemContainer container)
	{
	}

	// Token: 0x06001345 RID: 4933 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnItemAddedOrRemoved(global::Item item, bool added)
	{
	}

	// Token: 0x06001346 RID: 4934 RVA: 0x0009A8E6 File Offset: 0x00098AE6
	public virtual bool ItemFilter(global::Item item, int targetSlot)
	{
		return this.onlyAcceptCategory == ItemCategory.All || item.info.category == this.onlyAcceptCategory;
	}

	// Token: 0x06001347 RID: 4935 RVA: 0x0009A908 File Offset: 0x00098B08
	public void CreateInventory(bool giveUID)
	{
		this.inventory = new global::ItemContainer();
		this.inventory.entityOwner = this;
		this.inventory.allowedContents = ((this.allowedContents == (global::ItemContainer.ContentsType)0) ? global::ItemContainer.ContentsType.Generic : this.allowedContents);
		this.inventory.SetOnlyAllowedItems(new ItemDefinition[]
		{
			this.allowedItem,
			this.allowedItem2
		});
		this.inventory.maxStackSize = this.maxStackSize;
		this.inventory.ServerInitialize(null, this.inventorySlots);
		if (giveUID)
		{
			this.inventory.GiveUID();
		}
		this.inventory.onDirty += this.OnInventoryDirty;
		this.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
		this.inventory.canAcceptItem = new Func<global::Item, int, bool>(this.ItemFilter);
	}

	// Token: 0x06001348 RID: 4936 RVA: 0x0009A9E4 File Offset: 0x00098BE4
	public override void PreServerLoad()
	{
		base.PreServerLoad();
		this.CreateInventory(false);
	}

	// Token: 0x06001349 RID: 4937 RVA: 0x0009A9F3 File Offset: 0x00098BF3
	protected virtual void OnInventoryDirty()
	{
		base.InvalidateNetworkCache();
	}

	// Token: 0x0600134A RID: 4938 RVA: 0x0009A9FB File Offset: 0x00098BFB
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.inventory != null && !this.inventory.uid.IsValid)
		{
			this.inventory.GiveUID();
		}
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
	}

	// Token: 0x0600134B RID: 4939 RVA: 0x0009AA32 File Offset: 0x00098C32
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (this.inventory != null)
		{
			this.inventory.Kill();
			this.inventory = null;
		}
	}

	// Token: 0x0600134C RID: 4940 RVA: 0x0009AA54 File Offset: 0x00098C54
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(global::BaseEntity.RPCMessage rpc)
	{
		if (!this.isLootable)
		{
			return;
		}
		global::BasePlayer player = rpc.player;
		if (!player || !player.CanInteract())
		{
			return;
		}
		this.PlayerOpenLoot(player, "", true);
	}

	// Token: 0x0600134D RID: 4941 RVA: 0x0009AA90 File Offset: 0x00098C90
	public virtual string GetPanelName()
	{
		return this.panelName;
	}

	// Token: 0x0600134E RID: 4942 RVA: 0x0009AA98 File Offset: 0x00098C98
	public virtual bool CanMoveFrom(global::BasePlayer player, global::Item item)
	{
		return !this.inventory.IsLocked();
	}

	// Token: 0x0600134F RID: 4943 RVA: 0x0009AAA8 File Offset: 0x00098CA8
	public virtual bool CanOpenLootPanel(global::BasePlayer player, string panelName)
	{
		if (!this.CanBeLooted(player))
		{
			return false;
		}
		BaseLock baseLock = base.GetSlot(global::BaseEntity.Slot.Lock) as BaseLock;
		if (baseLock != null && !baseLock.OnTryToOpen(player))
		{
			player.ChatMessage("It is locked...");
			return false;
		}
		return true;
	}

	// Token: 0x06001350 RID: 4944 RVA: 0x0009AAED File Offset: 0x00098CED
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return (!this.needsBuildingPrivilegeToUse || player.CanBuild()) && (!this.mustBeMountedToUse || player.isMounted) && base.CanBeLooted(player);
	}

	// Token: 0x06001351 RID: 4945 RVA: 0x0009AB1A File Offset: 0x00098D1A
	public virtual void AddContainers(PlayerLoot loot)
	{
		loot.AddContainer(this.inventory);
	}

	// Token: 0x06001352 RID: 4946 RVA: 0x0009AB28 File Offset: 0x00098D28
	public virtual bool PlayerOpenLoot(global::BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		if (base.IsLocked())
		{
			player.ShowToast(GameTip.Styles.Red_Normal, StorageContainer.LockedMessage, Array.Empty<string>());
			return false;
		}
		if (this.onlyOneUser && base.IsOpen())
		{
			player.ShowToast(GameTip.Styles.Red_Normal, StorageContainer.InUseMessage, Array.Empty<string>());
			return false;
		}
		if (panelToOpen == "")
		{
			panelToOpen = this.panelName;
		}
		if (!this.CanOpenLootPanel(player, panelToOpen))
		{
			return false;
		}
		if (player.inventory.loot.StartLootingEntity(this, doPositionChecks))
		{
			base.SetFlag(global::BaseEntity.Flags.Open, true, false, true);
			this.AddContainers(player.inventory.loot);
			player.inventory.loot.SendImmediate();
			player.ClientRPCPlayer<string>(null, player, "RPC_OpenLootPanel", panelToOpen);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			return true;
		}
		return false;
	}

	// Token: 0x06001353 RID: 4947 RVA: 0x0005DD1A File Offset: 0x0005BF1A
	public virtual void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001354 RID: 4948 RVA: 0x0009ABEC File Offset: 0x00098DEC
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

	// Token: 0x06001355 RID: 4949 RVA: 0x0009AC51 File Offset: 0x00098E51
	public override void OnKilled(HitInfo info)
	{
		this.DropItems((info != null) ? info.Initiator : null);
		base.OnKilled(info);
	}

	// Token: 0x06001356 RID: 4950 RVA: 0x0005DC1D File Offset: 0x0005BE1D
	public void DropItems(global::BaseEntity initiator = null)
	{
		StorageContainer.DropItems(this, initiator);
	}

	// Token: 0x06001357 RID: 4951 RVA: 0x0009AC6C File Offset: 0x00098E6C
	public static void DropItems(IItemContainerEntity containerEntity, global::BaseEntity initiator = null)
	{
		global::ItemContainer inventory = containerEntity.inventory;
		if (inventory == null || inventory.itemList == null || inventory.itemList.Count == 0)
		{
			return;
		}
		if (!containerEntity.DropsLoot)
		{
			return;
		}
		if (containerEntity.ShouldDropItemsIndividually() || (inventory.itemList.Count == 1 && !containerEntity.DropFloats))
		{
			if (initiator != null)
			{
				containerEntity.DropBonusItems(initiator, inventory);
			}
			DropUtil.DropItems(inventory, containerEntity.GetDropPosition());
			return;
		}
		string prefab = containerEntity.DropFloats ? "assets/prefabs/misc/item drop/item_drop_buoyant.prefab" : "assets/prefabs/misc/item drop/item_drop.prefab";
		inventory.Drop(prefab, containerEntity.GetDropPosition(), containerEntity.Transform.rotation, containerEntity.DestroyLootPercent) != null;
	}

	// Token: 0x06001358 RID: 4952 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void DropBonusItems(global::BaseEntity initiator, global::ItemContainer container)
	{
	}

	// Token: 0x06001359 RID: 4953 RVA: 0x0009AD18 File Offset: 0x00098F18
	public override Vector3 GetDropPosition()
	{
		return base.transform.localToWorldMatrix.MultiplyPoint(this.dropPosition);
	}

	// Token: 0x0600135A RID: 4954 RVA: 0x0009AD40 File Offset: 0x00098F40
	public override Vector3 GetDropVelocity()
	{
		return this.GetInheritedDropVelocity() + base.transform.localToWorldMatrix.MultiplyVector(this.dropPosition);
	}

	// Token: 0x0600135B RID: 4955 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool ShouldDropItemsIndividually()
	{
		return false;
	}

	// Token: 0x0600135C RID: 4956 RVA: 0x0009AD74 File Offset: 0x00098F74
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.storageBox != null)
		{
			if (this.inventory != null)
			{
				this.inventory.Load(info.msg.storageBox.contents);
				this.inventory.capacity = this.inventorySlots;
				return;
			}
			Debug.LogWarning("Storage container without inventory: " + this.ToString());
		}
	}

	// Token: 0x0600135D RID: 4957 RVA: 0x000445C9 File Offset: 0x000427C9
	public virtual int GetIdealSlot(global::BasePlayer player, global::Item item)
	{
		return -1;
	}

	// Token: 0x0600135E RID: 4958 RVA: 0x0009ADE0 File Offset: 0x00098FE0
	public virtual ItemContainerId GetIdealContainer(global::BasePlayer player, global::Item item, bool altMove)
	{
		return default(ItemContainerId);
	}

	// Token: 0x0600135F RID: 4959 RVA: 0x0009ADF6 File Offset: 0x00098FF6
	public override bool HasSlot(global::BaseEntity.Slot slot)
	{
		return (this.isLockable && slot == global::BaseEntity.Slot.Lock) || (this.isMonitorable && slot == global::BaseEntity.Slot.StorageMonitor) || base.HasSlot(slot);
	}

	// Token: 0x06001360 RID: 4960 RVA: 0x0009AE1A File Offset: 0x0009901A
	public bool OccupiedCheck(global::BasePlayer player = null)
	{
		return (player != null && player.inventory.loot.entitySource == this) || !this.onlyOneUser || !base.IsOpen();
	}

	// Token: 0x06001361 RID: 4961 RVA: 0x0009AE54 File Offset: 0x00099054
	protected bool HasAttachedStorageAdaptor()
	{
		if (this.children == null)
		{
			return false;
		}
		using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current is IndustrialStorageAdaptor)
				{
					return true;
				}
			}
		}
		return false;
	}
}
