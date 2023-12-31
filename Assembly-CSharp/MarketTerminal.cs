﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Network;
using ProtoBuf;
using Rust;
using Rust.UI;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000099 RID: 153
public class MarketTerminal : StorageContainer
{
	// Token: 0x0400090B RID: 2315
	private Action<global::BasePlayer, global::Item> _onCurrencyRemovedCached;

	// Token: 0x0400090C RID: 2316
	private Action<global::BasePlayer, global::Item> _onItemPurchasedCached;

	// Token: 0x0400090D RID: 2317
	private Action _checkForExpiredOrdersCached;

	// Token: 0x0400090E RID: 2318
	private bool _transactionActive;

	// Token: 0x0400090F RID: 2319
	private static readonly List<NetworkableId> _deliveryEligible = new List<NetworkableId>(128);

	// Token: 0x04000910 RID: 2320
	private static RealTimeSince _deliveryEligibleLastCalculated;

	// Token: 0x04000911 RID: 2321
	public const global::BaseEntity.Flags Flag_HasItems = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000912 RID: 2322
	public const global::BaseEntity.Flags Flag_InventoryFull = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000913 RID: 2323
	[Header("Market Terminal")]
	public GameObjectRef menuPrefab;

	// Token: 0x04000914 RID: 2324
	public ulong lockToCustomerDuration = 300UL;

	// Token: 0x04000915 RID: 2325
	public ulong orderTimeout = 180UL;

	// Token: 0x04000916 RID: 2326
	public ItemDefinition deliveryFeeCurrency;

	// Token: 0x04000917 RID: 2327
	public int deliveryFeeAmount;

	// Token: 0x04000918 RID: 2328
	public DeliveryDroneConfig config;

	// Token: 0x04000919 RID: 2329
	public RustText userLabel;

	// Token: 0x0400091A RID: 2330
	private ulong _customerSteamId;

	// Token: 0x0400091B RID: 2331
	private string _customerName;

	// Token: 0x0400091C RID: 2332
	private TimeUntil _timeUntilCustomerExpiry;

	// Token: 0x0400091D RID: 2333
	private EntityRef<Marketplace> _marketplace;

	// Token: 0x0400091E RID: 2334
	public List<ProtoBuf.MarketTerminal.PendingOrder> pendingOrders;

	// Token: 0x06000DF0 RID: 3568 RVA: 0x00075DA0 File Offset: 0x00073FA0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MarketTerminal.OnRpcMessage", 0))
		{
			if (rpc == 3793918752U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_Purchase ");
				}
				using (TimeWarning.New("Server_Purchase", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3793918752U, "Server_Purchase", this, player, 10UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3793918752U, "Server_Purchase", this, player, 3f))
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
							this.Server_Purchase(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_Purchase");
					}
				}
				return true;
			}
			if (rpc == 1382511247U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_TryOpenMarket ");
				}
				using (TimeWarning.New("Server_TryOpenMarket", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1382511247U, "Server_TryOpenMarket", this, player, 3UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1382511247U, "Server_TryOpenMarket", this, player, 3f))
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
							this.Server_TryOpenMarket(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in Server_TryOpenMarket");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000DF1 RID: 3569 RVA: 0x000760D8 File Offset: 0x000742D8
	public void Setup(Marketplace marketplace)
	{
		this._marketplace = new EntityRef<Marketplace>(marketplace.net.ID);
	}

	// Token: 0x06000DF2 RID: 3570 RVA: 0x000760F0 File Offset: 0x000742F0
	public override void ServerInit()
	{
		base.ServerInit();
		this._onCurrencyRemovedCached = new Action<global::BasePlayer, global::Item>(this.OnCurrencyRemoved);
		this._onItemPurchasedCached = new Action<global::BasePlayer, global::Item>(this.OnItemPurchased);
		this._checkForExpiredOrdersCached = new Action(this.CheckForExpiredOrders);
	}

	// Token: 0x06000DF3 RID: 3571 RVA: 0x00076130 File Offset: 0x00074330
	private void RegisterOrder(global::BasePlayer player, global::VendingMachine vendingMachine)
	{
		if (this.pendingOrders == null)
		{
			this.pendingOrders = Facepunch.Pool.GetList<ProtoBuf.MarketTerminal.PendingOrder>();
		}
		if (this.HasPendingOrderFor(vendingMachine.net.ID))
		{
			return;
		}
		Marketplace marketplace;
		if (!this._marketplace.TryGet(true, out marketplace))
		{
			Debug.LogError("Marketplace is not set", this);
			return;
		}
		NetworkableId droneId = marketplace.SendDrone(player, this, vendingMachine);
		if (!droneId.IsValid)
		{
			Debug.LogError("Failed to spawn delivery drone");
			return;
		}
		ProtoBuf.MarketTerminal.PendingOrder pendingOrder = Facepunch.Pool.Get<ProtoBuf.MarketTerminal.PendingOrder>();
		pendingOrder.vendingMachineId = vendingMachine.net.ID;
		pendingOrder.timeUntilExpiry = this.orderTimeout;
		pendingOrder.droneId = droneId;
		this.pendingOrders.Add(pendingOrder);
		this.CheckForExpiredOrders();
		this.UpdateHasItems(false);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000DF4 RID: 3572 RVA: 0x000761F0 File Offset: 0x000743F0
	public void CompleteOrder(NetworkableId vendingMachineId)
	{
		if (this.pendingOrders == null)
		{
			return;
		}
		int num = this.pendingOrders.FindIndexWith((ProtoBuf.MarketTerminal.PendingOrder o) => o.vendingMachineId, vendingMachineId);
		if (num < 0)
		{
			Debug.LogError("Completed market order that doesn't exist?");
			return;
		}
		this.pendingOrders[num].Dispose();
		this.pendingOrders.RemoveAt(num);
		this.CheckForExpiredOrders();
		this.UpdateHasItems(false);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000DF5 RID: 3573 RVA: 0x00076274 File Offset: 0x00074474
	private void CheckForExpiredOrders()
	{
		if (this.pendingOrders != null && this.pendingOrders.Count > 0)
		{
			bool flag = false;
			float? num = null;
			for (int i = 0; i < this.pendingOrders.Count; i++)
			{
				ProtoBuf.MarketTerminal.PendingOrder pendingOrder = this.pendingOrders[i];
				if (pendingOrder.timeUntilExpiry <= 0f)
				{
					EntityRef<global::DeliveryDrone> entityRef = new EntityRef<global::DeliveryDrone>(pendingOrder.droneId);
					global::DeliveryDrone deliveryDrone;
					if (entityRef.TryGet(true, out deliveryDrone))
					{
						Debug.LogError("Delivery timed out waiting for drone (too slow speed?)", this);
						deliveryDrone.Kill(global::BaseNetworkable.DestroyMode.None);
					}
					else
					{
						Debug.LogError("Delivery timed out waiting for drone, and the drone seems to have been destroyed?", this);
					}
					this.pendingOrders.RemoveAt(i);
					i--;
					flag = true;
				}
				else if (num == null || pendingOrder.timeUntilExpiry < num.Value)
				{
					num = new float?(pendingOrder.timeUntilExpiry);
				}
			}
			if (flag)
			{
				this.UpdateHasItems(false);
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
			if (num != null)
			{
				base.Invoke(this._checkForExpiredOrdersCached, num.Value);
				return;
			}
		}
		else
		{
			base.CancelInvoke(this._checkForExpiredOrdersCached);
		}
	}

	// Token: 0x06000DF6 RID: 3574 RVA: 0x00076398 File Offset: 0x00074598
	private void RestrictToPlayer(global::BasePlayer player)
	{
		if (this._customerSteamId == player.userID)
		{
			this._timeUntilCustomerExpiry = this.lockToCustomerDuration;
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			return;
		}
		if (this._customerSteamId != 0UL)
		{
			Debug.LogError("Overwriting player restriction! It should be cleared first.", this);
		}
		this._customerSteamId = player.userID;
		this._customerName = player.displayName;
		this._timeUntilCustomerExpiry = this.lockToCustomerDuration;
		base.SendNetworkUpdateImmediate(false);
		base.ClientRPC<ulong>(null, "Client_CloseMarketUI", this._customerSteamId);
		this.RemoveAnyLooters();
		if (base.IsOpen())
		{
			Debug.LogError("Market terminal's inventory is still open after removing looters!", this);
		}
	}

	// Token: 0x06000DF7 RID: 3575 RVA: 0x0007643E File Offset: 0x0007463E
	private void ClearRestriction()
	{
		if (this._customerSteamId == 0UL)
		{
			return;
		}
		this._customerSteamId = 0UL;
		this._customerName = null;
		this._timeUntilCustomerExpiry = 0f;
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000DF8 RID: 3576 RVA: 0x00076470 File Offset: 0x00074670
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(3UL)]
	public void Server_TryOpenMarket(global::BaseEntity.RPCMessage msg)
	{
		if (!this.CanPlayerInteract(msg.player))
		{
			return;
		}
		if (!this._marketplace.IsValid(true))
		{
			Debug.LogError("Marketplace is not set", this);
			return;
		}
		using (EntityIdList entityIdList = Facepunch.Pool.Get<EntityIdList>())
		{
			entityIdList.entityIds = Facepunch.Pool.GetList<NetworkableId>();
			this.GetDeliveryEligibleVendingMachines(entityIdList.entityIds);
			base.ClientRPCPlayer<EntityIdList>(null, msg.player, "Client_OpenMarket", entityIdList);
		}
	}

	// Token: 0x06000DF9 RID: 3577 RVA: 0x000764F4 File Offset: 0x000746F4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(10UL)]
	public void Server_Purchase(global::BaseEntity.RPCMessage msg)
	{
		if (!this.CanPlayerInteract(msg.player))
		{
			return;
		}
		if (!this._marketplace.IsValid(true))
		{
			Debug.LogError("Marketplace is not set", this);
			return;
		}
		NetworkableId networkableId = msg.read.EntityID();
		int num = msg.read.Int32();
		int num2 = msg.read.Int32();
		global::VendingMachine vendingMachine = global::BaseNetworkable.serverEntities.Find(networkableId) as global::VendingMachine;
		if (vendingMachine == null || !vendingMachine.IsValid() || num < 0 || num >= vendingMachine.sellOrders.sellOrders.Count || num2 <= 0 || base.inventory.IsFull())
		{
			return;
		}
		this.GetDeliveryEligibleVendingMachines(null);
		if (global::MarketTerminal._deliveryEligible == null || !global::MarketTerminal._deliveryEligible.Contains(networkableId))
		{
			return;
		}
		try
		{
			this._transactionActive = true;
			int num3 = this.deliveryFeeAmount;
			ProtoBuf.VendingMachine.SellOrder sellOrder = vendingMachine.sellOrders.sellOrders[num];
			if (this.CanPlayerAffordOrderAndDeliveryFee(msg.player, sellOrder, num2))
			{
				int num4 = msg.player.inventory.Take(null, this.deliveryFeeCurrency.itemid, num3);
				if (num4 != num3)
				{
					Debug.LogError(string.Format("Took an incorrect number of items for the delivery fee (took {0}, should have taken {1})", num4, num3));
				}
				base.ClientRPCPlayer<int, int, bool>(null, msg.player, "Client_ShowItemNotice", this.deliveryFeeCurrency.itemid, -num3, false);
				if (!vendingMachine.DoTransaction(msg.player, num, num2, base.inventory, this._onCurrencyRemovedCached, this._onItemPurchasedCached, this))
				{
					global::Item item = ItemManager.CreateByItemID(this.deliveryFeeCurrency.itemid, num3, 0UL);
					if (!msg.player.inventory.GiveItem(item, null, false))
					{
						item.Drop(msg.player.inventory.containerMain.dropPosition, msg.player.inventory.containerMain.dropVelocity, default(Quaternion));
					}
				}
				else
				{
					this.RestrictToPlayer(msg.player);
					this.RegisterOrder(msg.player, vendingMachine);
				}
			}
		}
		finally
		{
			this._transactionActive = false;
		}
	}

	// Token: 0x06000DFA RID: 3578 RVA: 0x00076720 File Offset: 0x00074920
	private void UpdateHasItems(bool sendNetworkUpdate = true)
	{
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		bool flag = base.inventory.itemList.Count > 0;
		bool flag2 = this.pendingOrders != null && this.pendingOrders.Count != 0;
		base.SetFlag(global::BaseEntity.Flags.Reserved1, flag && !flag2, false, sendNetworkUpdate);
		base.SetFlag(global::BaseEntity.Flags.Reserved2, base.inventory.IsFull(), false, sendNetworkUpdate);
		if (!flag && !flag2)
		{
			this.ClearRestriction();
		}
	}

	// Token: 0x06000DFB RID: 3579 RVA: 0x0007679E File Offset: 0x0007499E
	private void OnCurrencyRemoved(global::BasePlayer player, global::Item currencyItem)
	{
		if (player != null && currencyItem != null)
		{
			base.ClientRPCPlayer<int, int, bool>(null, player, "Client_ShowItemNotice", currencyItem.info.itemid, -currencyItem.amount, false);
		}
	}

	// Token: 0x06000DFC RID: 3580 RVA: 0x000767CC File Offset: 0x000749CC
	private void OnItemPurchased(global::BasePlayer player, global::Item purchasedItem)
	{
		if (player != null && purchasedItem != null)
		{
			base.ClientRPCPlayer<int, int, bool>(null, player, "Client_ShowItemNotice", purchasedItem.info.itemid, purchasedItem.amount, true);
		}
	}

	// Token: 0x06000DFD RID: 3581 RVA: 0x000767FC File Offset: 0x000749FC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.marketTerminal = Facepunch.Pool.Get<ProtoBuf.MarketTerminal>();
		info.msg.marketTerminal.customerSteamId = this._customerSteamId;
		info.msg.marketTerminal.customerName = this._customerName;
		info.msg.marketTerminal.timeUntilExpiry = this._timeUntilCustomerExpiry;
		info.msg.marketTerminal.marketplaceId = this._marketplace.uid;
		info.msg.marketTerminal.orders = Facepunch.Pool.GetList<ProtoBuf.MarketTerminal.PendingOrder>();
		if (this.pendingOrders != null)
		{
			foreach (ProtoBuf.MarketTerminal.PendingOrder pendingOrder in this.pendingOrders)
			{
				ProtoBuf.MarketTerminal.PendingOrder item = pendingOrder.Copy();
				info.msg.marketTerminal.orders.Add(item);
			}
		}
	}

	// Token: 0x06000DFE RID: 3582 RVA: 0x000768F4 File Offset: 0x00074AF4
	public override bool ItemFilter(global::Item item, int targetSlot)
	{
		return this._transactionActive || item.parent == null || item.parent == base.inventory;
	}

	// Token: 0x06000DFF RID: 3583 RVA: 0x0007691B File Offset: 0x00074B1B
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		this.UpdateHasItems(true);
	}

	// Token: 0x06000E00 RID: 3584 RVA: 0x00076924 File Offset: 0x00074B24
	public override bool CanOpenLootPanel(global::BasePlayer player, string panelName)
	{
		return this.CanPlayerInteract(player) && base.HasFlag(global::BaseEntity.Flags.Reserved1) && base.CanOpenLootPanel(player, panelName);
	}

	// Token: 0x06000E01 RID: 3585 RVA: 0x00076948 File Offset: 0x00074B48
	private void RemoveAnyLooters()
	{
		global::ItemContainer inventory = base.inventory;
		foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
		{
			if (!(basePlayer == null) && !(basePlayer.inventory == null) && !(basePlayer.inventory.loot == null) && basePlayer.inventory.loot.containers.Contains(inventory))
			{
				basePlayer.inventory.loot.Clear();
			}
		}
	}

	// Token: 0x06000E02 RID: 3586 RVA: 0x000769EC File Offset: 0x00074BEC
	public void GetDeliveryEligibleVendingMachines(List<NetworkableId> vendingMachineIds)
	{
		if (global::MarketTerminal._deliveryEligibleLastCalculated < 5f)
		{
			if (vendingMachineIds != null)
			{
				foreach (NetworkableId item in global::MarketTerminal._deliveryEligible)
				{
					vendingMachineIds.Add(item);
				}
			}
			return;
		}
		global::MarketTerminal._deliveryEligibleLastCalculated = 0f;
		global::MarketTerminal._deliveryEligible.Clear();
		using (List<MapMarker>.Enumerator enumerator2 = MapMarker.serverMapMarkers.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				VendingMachineMapMarker vendingMachineMapMarker;
				if ((vendingMachineMapMarker = (enumerator2.Current as VendingMachineMapMarker)) != null && !(vendingMachineMapMarker.server_vendingMachine == null))
				{
					global::VendingMachine server_vendingMachine = vendingMachineMapMarker.server_vendingMachine;
					if (!(server_vendingMachine == null) && (this.<GetDeliveryEligibleVendingMachines>g__IsEligible|24_0(server_vendingMachine, this.config.vendingMachineOffset, 1) || this.<GetDeliveryEligibleVendingMachines>g__IsEligible|24_0(server_vendingMachine, this.config.vendingMachineOffset + Vector3.forward * this.config.maxDistanceFromVendingMachine, 2)))
					{
						global::MarketTerminal._deliveryEligible.Add(server_vendingMachine.net.ID);
					}
				}
			}
		}
		if (vendingMachineIds != null)
		{
			foreach (NetworkableId item2 in global::MarketTerminal._deliveryEligible)
			{
				vendingMachineIds.Add(item2);
			}
		}
	}

	// Token: 0x06000E03 RID: 3587 RVA: 0x00076B78 File Offset: 0x00074D78
	public bool CanPlayerAffordOrderAndDeliveryFee(global::BasePlayer player, ProtoBuf.VendingMachine.SellOrder sellOrder, int numberOfTransactions)
	{
		int num = player.inventory.FindItemIDs(this.deliveryFeeCurrency.itemid).Sum((global::Item i) => i.amount);
		int num2 = this.deliveryFeeAmount;
		if (num < num2)
		{
			return false;
		}
		if (sellOrder != null)
		{
			int num3 = sellOrder.currencyAmountPerItem * numberOfTransactions;
			if (sellOrder.currencyID == this.deliveryFeeCurrency.itemid && !sellOrder.currencyIsBP && num < num2 + num3)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000E04 RID: 3588 RVA: 0x00076BFD File Offset: 0x00074DFD
	public bool HasPendingOrderFor(NetworkableId vendingMachineId)
	{
		List<ProtoBuf.MarketTerminal.PendingOrder> list = this.pendingOrders;
		object obj;
		if (list == null)
		{
			obj = null;
		}
		else
		{
			obj = list.FindWith((ProtoBuf.MarketTerminal.PendingOrder o) => o.vendingMachineId, vendingMachineId, null);
		}
		return obj != null;
	}

	// Token: 0x06000E05 RID: 3589 RVA: 0x00076C35 File Offset: 0x00074E35
	public bool CanPlayerInteract(global::BasePlayer player)
	{
		return !(player == null) && (this._customerSteamId == 0UL || this._timeUntilCustomerExpiry <= 0f || player.userID == this._customerSteamId);
	}

	// Token: 0x06000E06 RID: 3590 RVA: 0x00076C6C File Offset: 0x00074E6C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.marketTerminal != null)
		{
			this._customerSteamId = info.msg.marketTerminal.customerSteamId;
			this._customerName = info.msg.marketTerminal.customerName;
			this._timeUntilCustomerExpiry = info.msg.marketTerminal.timeUntilExpiry;
			this._marketplace = new EntityRef<Marketplace>(info.msg.marketTerminal.marketplaceId);
			if (this.pendingOrders == null)
			{
				this.pendingOrders = Facepunch.Pool.GetList<ProtoBuf.MarketTerminal.PendingOrder>();
			}
			if (this.pendingOrders.Count > 0)
			{
				foreach (ProtoBuf.MarketTerminal.PendingOrder pendingOrder in this.pendingOrders)
				{
					Facepunch.Pool.Free<ProtoBuf.MarketTerminal.PendingOrder>(ref pendingOrder);
				}
				this.pendingOrders.Clear();
			}
			foreach (ProtoBuf.MarketTerminal.PendingOrder pendingOrder2 in info.msg.marketTerminal.orders)
			{
				ProtoBuf.MarketTerminal.PendingOrder item = pendingOrder2.Copy();
				this.pendingOrders.Add(item);
			}
		}
	}

	// Token: 0x06000E09 RID: 3593 RVA: 0x00076DEC File Offset: 0x00074FEC
	[CompilerGenerated]
	private bool <GetDeliveryEligibleVendingMachines>g__IsEligible|24_0(global::VendingMachine vendingMachine, Vector3 offset, int n)
	{
		RaycastHit raycastHit;
		return vendingMachine is NPCVendingMachine || (vendingMachine.IsBroadcasting() && this.config.IsVendingMachineAccessible(vendingMachine, offset, out raycastHit));
	}
}
