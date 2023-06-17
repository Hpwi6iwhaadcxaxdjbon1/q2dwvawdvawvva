using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C9 RID: 201
public class ShopFront : StorageContainer
{
	// Token: 0x04000B42 RID: 2882
	public float maxUseAngle = 27f;

	// Token: 0x04000B43 RID: 2883
	public BasePlayer vendorPlayer;

	// Token: 0x04000B44 RID: 2884
	public BasePlayer customerPlayer;

	// Token: 0x04000B45 RID: 2885
	public GameObjectRef transactionCompleteEffect;

	// Token: 0x04000B46 RID: 2886
	public ItemContainer customerInventory;

	// Token: 0x04000B47 RID: 2887
	private bool swappingItems;

	// Token: 0x060011F1 RID: 4593 RVA: 0x00091618 File Offset: 0x0008F818
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ShopFront.OnRpcMessage", 0))
		{
			if (rpc == 1159607245U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AcceptClicked ");
				}
				using (TimeWarning.New("AcceptClicked", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1159607245U, "AcceptClicked", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.AcceptClicked(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in AcceptClicked");
					}
				}
				return true;
			}
			if (rpc == 3168107540U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - CancelClicked ");
				}
				using (TimeWarning.New("CancelClicked", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(3168107540U, "CancelClicked", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CancelClicked(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in CancelClicked");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700019C RID: 412
	// (get) Token: 0x060011F2 RID: 4594 RVA: 0x00091918 File Offset: 0x0008FB18
	private float AngleDotProduct
	{
		get
		{
			return 1f - this.maxUseAngle / 90f;
		}
	}

	// Token: 0x1700019D RID: 413
	// (get) Token: 0x060011F3 RID: 4595 RVA: 0x000380E4 File Offset: 0x000362E4
	public ItemContainer vendorInventory
	{
		get
		{
			return base.inventory;
		}
	}

	// Token: 0x060011F4 RID: 4596 RVA: 0x00007A3C File Offset: 0x00005C3C
	public bool TradeLocked()
	{
		return false;
	}

	// Token: 0x060011F5 RID: 4597 RVA: 0x0009192C File Offset: 0x0008FB2C
	public bool IsTradingPlayer(BasePlayer player)
	{
		return player != null && (this.IsPlayerCustomer(player) || this.IsPlayerVendor(player));
	}

	// Token: 0x060011F6 RID: 4598 RVA: 0x0009194B File Offset: 0x0008FB4B
	public bool IsPlayerCustomer(BasePlayer player)
	{
		return player == this.customerPlayer;
	}

	// Token: 0x060011F7 RID: 4599 RVA: 0x00091959 File Offset: 0x0008FB59
	public bool IsPlayerVendor(BasePlayer player)
	{
		return player == this.vendorPlayer;
	}

	// Token: 0x060011F8 RID: 4600 RVA: 0x00091968 File Offset: 0x0008FB68
	public bool PlayerInVendorPos(BasePlayer player)
	{
		return Vector3.Dot(base.transform.right, (player.transform.position - base.transform.position).normalized) <= -this.AngleDotProduct;
	}

	// Token: 0x060011F9 RID: 4601 RVA: 0x000919B4 File Offset: 0x0008FBB4
	public bool PlayerInCustomerPos(BasePlayer player)
	{
		return Vector3.Dot(base.transform.right, (player.transform.position - base.transform.position).normalized) >= this.AngleDotProduct;
	}

	// Token: 0x060011FA RID: 4602 RVA: 0x000919FF File Offset: 0x0008FBFF
	public bool LootEligable(BasePlayer player)
	{
		return !(player == null) && ((this.PlayerInVendorPos(player) && this.vendorPlayer == null) || (this.PlayerInCustomerPos(player) && this.customerPlayer == null));
	}

	// Token: 0x060011FB RID: 4603 RVA: 0x00091A40 File Offset: 0x0008FC40
	public void ResetTrade()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		this.vendorInventory.SetLocked(false);
		this.customerInventory.SetLocked(false);
		base.CancelInvoke(new Action(this.CompleteTrade));
	}

	// Token: 0x060011FC RID: 4604 RVA: 0x00091AA4 File Offset: 0x0008FCA4
	public void CompleteTrade()
	{
		if (this.vendorPlayer != null && this.customerPlayer != null && base.HasFlag(BaseEntity.Flags.Reserved1) && base.HasFlag(BaseEntity.Flags.Reserved2))
		{
			try
			{
				this.swappingItems = true;
				for (int i = this.vendorInventory.capacity - 1; i >= 0; i--)
				{
					Item slot = this.vendorInventory.GetSlot(i);
					Item slot2 = this.customerInventory.GetSlot(i);
					if (this.customerPlayer && slot != null)
					{
						this.customerPlayer.GiveItem(slot, BaseEntity.GiveItemReason.Generic);
					}
					if (this.vendorPlayer && slot2 != null)
					{
						this.vendorPlayer.GiveItem(slot2, BaseEntity.GiveItemReason.Generic);
					}
				}
			}
			finally
			{
				this.swappingItems = false;
			}
			Effect.server.Run(this.transactionCompleteEffect.resourcePath, this, 0U, new Vector3(0f, 1f, 0f), Vector3.zero, null, false);
		}
		this.ResetTrade();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060011FD RID: 4605 RVA: 0x00091BBC File Offset: 0x0008FDBC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void AcceptClicked(BaseEntity.RPCMessage msg)
	{
		if (!this.IsTradingPlayer(msg.player))
		{
			return;
		}
		if (this.vendorPlayer == null || this.customerPlayer == null)
		{
			return;
		}
		if (this.IsPlayerVendor(msg.player))
		{
			base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
			this.vendorInventory.SetLocked(true);
		}
		else if (this.IsPlayerCustomer(msg.player))
		{
			base.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
			this.customerInventory.SetLocked(true);
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved1) && base.HasFlag(BaseEntity.Flags.Reserved2))
		{
			base.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
			base.Invoke(new Action(this.CompleteTrade), 2f);
		}
	}

	// Token: 0x060011FE RID: 4606 RVA: 0x00091C86 File Offset: 0x0008FE86
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void CancelClicked(BaseEntity.RPCMessage msg)
	{
		if (!this.IsTradingPlayer(msg.player))
		{
			return;
		}
		this.vendorPlayer;
		this.customerPlayer;
		this.ResetTrade();
	}

	// Token: 0x060011FF RID: 4607 RVA: 0x00091CB5 File Offset: 0x0008FEB5
	public override void PreServerLoad()
	{
		base.PreServerLoad();
	}

	// Token: 0x06001200 RID: 4608 RVA: 0x00091CC0 File Offset: 0x0008FEC0
	public override void ServerInit()
	{
		base.ServerInit();
		ItemContainer vendorInventory = this.vendorInventory;
		vendorInventory.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(vendorInventory.canAcceptItem, new Func<Item, int, bool>(this.CanAcceptVendorItem));
		if (this.customerInventory == null)
		{
			this.customerInventory = new ItemContainer();
			this.customerInventory.allowedContents = ((this.allowedContents == (ItemContainer.ContentsType)0) ? ItemContainer.ContentsType.Generic : this.allowedContents);
			this.customerInventory.SetOnlyAllowedItem(this.allowedItem);
			this.customerInventory.entityOwner = this;
			this.customerInventory.maxStackSize = this.maxStackSize;
			this.customerInventory.ServerInitialize(null, this.inventorySlots);
			this.customerInventory.GiveUID();
			this.customerInventory.onDirty += this.OnInventoryDirty;
			this.customerInventory.onItemAddedRemoved = new Action<Item, bool>(this.OnItemAddedOrRemoved);
			ItemContainer itemContainer = this.customerInventory;
			itemContainer.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(itemContainer.canAcceptItem, new Func<Item, int, bool>(this.CanAcceptCustomerItem));
			this.OnInventoryFirstCreated(this.customerInventory);
		}
	}

	// Token: 0x06001201 RID: 4609 RVA: 0x00091DDA File Offset: 0x0008FFDA
	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		this.ResetTrade();
	}

	// Token: 0x06001202 RID: 4610 RVA: 0x00091DEC File Offset: 0x0008FFEC
	private bool CanAcceptVendorItem(Item item, int targetSlot)
	{
		return this.swappingItems || (this.vendorPlayer != null && item.GetOwnerPlayer() == this.vendorPlayer) || this.vendorInventory.itemList.Contains(item);
	}

	// Token: 0x06001203 RID: 4611 RVA: 0x00091E38 File Offset: 0x00090038
	private bool CanAcceptCustomerItem(Item item, int targetSlot)
	{
		return this.swappingItems || (this.customerPlayer != null && item.GetOwnerPlayer() == this.customerPlayer) || this.customerInventory.itemList.Contains(item);
	}

	// Token: 0x06001204 RID: 4612 RVA: 0x00091E84 File Offset: 0x00090084
	public override bool CanMoveFrom(BasePlayer player, Item item)
	{
		if (this.TradeLocked())
		{
			return false;
		}
		if (this.IsTradingPlayer(player))
		{
			if (this.IsPlayerCustomer(player) && this.customerInventory.itemList.Contains(item) && !this.customerInventory.IsLocked())
			{
				return true;
			}
			if (this.IsPlayerVendor(player) && this.vendorInventory.itemList.Contains(item) && !this.vendorInventory.IsLocked())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001205 RID: 4613 RVA: 0x00091EFB File Offset: 0x000900FB
	public override bool CanOpenLootPanel(BasePlayer player, string panelName)
	{
		return base.CanOpenLootPanel(player, panelName) && this.LootEligable(player);
	}

	// Token: 0x06001206 RID: 4614 RVA: 0x00091F10 File Offset: 0x00090110
	public void ReturnPlayerItems(BasePlayer player)
	{
		if (this.IsTradingPlayer(player))
		{
			ItemContainer itemContainer = null;
			if (this.IsPlayerVendor(player))
			{
				itemContainer = this.vendorInventory;
			}
			else if (this.IsPlayerCustomer(player))
			{
				itemContainer = this.customerInventory;
			}
			if (itemContainer != null)
			{
				for (int i = itemContainer.itemList.Count - 1; i >= 0; i--)
				{
					Item item = itemContainer.itemList[i];
					player.GiveItem(item, BaseEntity.GiveItemReason.Generic);
				}
			}
		}
	}

	// Token: 0x06001207 RID: 4615 RVA: 0x00091F7C File Offset: 0x0009017C
	public override void PlayerStoppedLooting(BasePlayer player)
	{
		if (!this.IsTradingPlayer(player))
		{
			return;
		}
		this.ReturnPlayerItems(player);
		if (player == this.vendorPlayer)
		{
			this.vendorPlayer = null;
		}
		if (player == this.customerPlayer)
		{
			this.customerPlayer = null;
		}
		this.UpdatePlayers();
		this.ResetTrade();
		base.PlayerStoppedLooting(player);
	}

	// Token: 0x06001208 RID: 4616 RVA: 0x00091FD8 File Offset: 0x000901D8
	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		bool flag = base.PlayerOpenLoot(player, panelToOpen, true);
		if (flag)
		{
			player.inventory.loot.AddContainer(this.customerInventory);
			player.inventory.loot.SendImmediate();
		}
		if (this.PlayerInVendorPos(player) && this.vendorPlayer == null)
		{
			this.vendorPlayer = player;
		}
		else
		{
			if (!this.PlayerInCustomerPos(player) || !(this.customerPlayer == null))
			{
				return false;
			}
			this.customerPlayer = player;
		}
		this.ResetTrade();
		this.UpdatePlayers();
		return flag;
	}

	// Token: 0x06001209 RID: 4617 RVA: 0x00092068 File Offset: 0x00090268
	public void UpdatePlayers()
	{
		base.ClientRPC<NetworkableId, NetworkableId>(null, "CLIENT_ReceivePlayers", (this.vendorPlayer == null) ? default(NetworkableId) : this.vendorPlayer.net.ID, (this.customerPlayer == null) ? default(NetworkableId) : this.customerPlayer.net.ID);
	}

	// Token: 0x02000BF9 RID: 3065
	public static class ShopFrontFlags
	{
		// Token: 0x04004177 RID: 16759
		public const BaseEntity.Flags VendorAccepted = BaseEntity.Flags.Reserved1;

		// Token: 0x04004178 RID: 16760
		public const BaseEntity.Flags CustomerAccepted = BaseEntity.Flags.Reserved2;

		// Token: 0x04004179 RID: 16761
		public const BaseEntity.Flags Exchanging = BaseEntity.Flags.Reserved3;
	}
}
