using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000EB RID: 235
public class VendingMachine : StorageContainer
{
	// Token: 0x04000CFD RID: 3325
	[Header("VendingMachine")]
	public static readonly Translate.Phrase WaitForVendingMessage = new Translate.Phrase("vendingmachine.wait", "Please wait...");

	// Token: 0x04000CFE RID: 3326
	public GameObjectRef adminMenuPrefab;

	// Token: 0x04000CFF RID: 3327
	public string customerPanel = "";

	// Token: 0x04000D00 RID: 3328
	public ProtoBuf.VendingMachine.SellOrderContainer sellOrders;

	// Token: 0x04000D01 RID: 3329
	public SoundPlayer buySound;

	// Token: 0x04000D02 RID: 3330
	public string shopName = "A Shop";

	// Token: 0x04000D03 RID: 3331
	public GameObjectRef mapMarkerPrefab;

	// Token: 0x04000D04 RID: 3332
	public ItemDefinition blueprintBaseDef;

	// Token: 0x04000D05 RID: 3333
	private Action fullUpdateCached;

	// Token: 0x04000D06 RID: 3334
	protected global::BasePlayer vend_Player;

	// Token: 0x04000D07 RID: 3335
	private int vend_sellOrderID;

	// Token: 0x04000D08 RID: 3336
	private int vend_numberOfTransactions;

	// Token: 0x04000D09 RID: 3337
	protected bool transactionActive;

	// Token: 0x04000D0A RID: 3338
	private VendingMachineMapMarker myMarker;

	// Token: 0x04000D0B RID: 3339
	private bool industrialItemIncoming;

	// Token: 0x060014A5 RID: 5285 RVA: 0x000A2DDC File Offset: 0x000A0FDC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("VendingMachine.OnRpcMessage", 0))
		{
			if (rpc == 3011053703U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - BuyItem ");
				}
				using (TimeWarning.New("BuyItem", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3011053703U, "BuyItem", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3011053703U, "BuyItem", this, player, 3f))
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
							this.BuyItem(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in BuyItem");
					}
				}
				return true;
			}
			if (rpc == 1626480840U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_AddSellOrder ");
				}
				using (TimeWarning.New("RPC_AddSellOrder", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1626480840U, "RPC_AddSellOrder", this, player, 3f))
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
							this.RPC_AddSellOrder(msg2);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_AddSellOrder");
					}
				}
				return true;
			}
			if (rpc == 169239598U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Broadcast ");
				}
				using (TimeWarning.New("RPC_Broadcast", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(169239598U, "RPC_Broadcast", this, player, 3f))
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
							this.RPC_Broadcast(msg3);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RPC_Broadcast");
					}
				}
				return true;
			}
			if (rpc == 3680901137U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_DeleteSellOrder ");
				}
				using (TimeWarning.New("RPC_DeleteSellOrder", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3680901137U, "RPC_DeleteSellOrder", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_DeleteSellOrder(msg4);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in RPC_DeleteSellOrder");
					}
				}
				return true;
			}
			if (rpc == 2555993359U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenAdmin ");
				}
				using (TimeWarning.New("RPC_OpenAdmin", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2555993359U, "RPC_OpenAdmin", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenAdmin(msg5);
						}
					}
					catch (Exception exception5)
					{
						Debug.LogException(exception5);
						player.Kick("RPC Error in RPC_OpenAdmin");
					}
				}
				return true;
			}
			if (rpc == 36164441U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenShop ");
				}
				using (TimeWarning.New("RPC_OpenShop", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(36164441U, "RPC_OpenShop", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg6 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenShop(msg6);
						}
					}
					catch (Exception exception6)
					{
						Debug.LogException(exception6);
						player.Kick("RPC Error in RPC_OpenShop");
					}
				}
				return true;
			}
			if (rpc == 3346513099U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RotateVM ");
				}
				using (TimeWarning.New("RPC_RotateVM", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3346513099U, "RPC_RotateVM", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg7 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RotateVM(msg7);
						}
					}
					catch (Exception exception7)
					{
						Debug.LogException(exception7);
						player.Kick("RPC Error in RPC_RotateVM");
					}
				}
				return true;
			}
			if (rpc == 1012779214U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_UpdateShopName ");
				}
				using (TimeWarning.New("RPC_UpdateShopName", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1012779214U, "RPC_UpdateShopName", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg8 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_UpdateShopName(msg8);
						}
					}
					catch (Exception exception8)
					{
						Debug.LogException(exception8);
						player.Kick("RPC Error in RPC_UpdateShopName");
					}
				}
				return true;
			}
			if (rpc == 3559014831U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - TransactionStart ");
				}
				using (TimeWarning.New("TransactionStart", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3559014831U, "TransactionStart", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.TransactionStart(rpc3);
						}
					}
					catch (Exception exception9)
					{
						Debug.LogException(exception9);
						player.Kick("RPC Error in TransactionStart");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060014A6 RID: 5286 RVA: 0x000A3A74 File Offset: 0x000A1C74
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.vendingMachine != null)
		{
			this.shopName = info.msg.vendingMachine.shopName;
			if (info.msg.vendingMachine.sellOrderContainer != null)
			{
				this.sellOrders = info.msg.vendingMachine.sellOrderContainer;
				this.sellOrders.ShouldPool = false;
			}
			if (info.fromDisk && base.isServer)
			{
				this.RefreshSellOrderStockLevel(null);
			}
		}
	}

	// Token: 0x060014A7 RID: 5287 RVA: 0x000A3AF8 File Offset: 0x000A1CF8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.vendingMachine = new ProtoBuf.VendingMachine();
		info.msg.vendingMachine.ShouldPool = false;
		info.msg.vendingMachine.shopName = this.shopName;
		if (this.sellOrders != null)
		{
			info.msg.vendingMachine.sellOrderContainer = new ProtoBuf.VendingMachine.SellOrderContainer();
			info.msg.vendingMachine.sellOrderContainer.ShouldPool = false;
			info.msg.vendingMachine.sellOrderContainer.sellOrders = new List<ProtoBuf.VendingMachine.SellOrder>();
			foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in this.sellOrders.sellOrders)
			{
				ProtoBuf.VendingMachine.SellOrder sellOrder2 = new ProtoBuf.VendingMachine.SellOrder();
				sellOrder2.ShouldPool = false;
				sellOrder.CopyTo(sellOrder2);
				info.msg.vendingMachine.sellOrderContainer.sellOrders.Add(sellOrder2);
			}
		}
	}

	// Token: 0x060014A8 RID: 5288 RVA: 0x000A3C04 File Offset: 0x000A1E04
	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isServer)
		{
			this.InstallDefaultSellOrders();
			base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
			base.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
			this.RefreshSellOrderStockLevel(null);
			global::ItemContainer inventory = base.inventory;
			inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.CanAcceptItem));
			this.UpdateMapMarker();
			this.fullUpdateCached = new Action(this.FullUpdate);
		}
	}

	// Token: 0x060014A9 RID: 5289 RVA: 0x000A3C91 File Offset: 0x000A1E91
	public override void DestroyShared()
	{
		if (this.myMarker)
		{
			this.myMarker.Kill(global::BaseNetworkable.DestroyMode.None);
			this.myMarker = null;
		}
		base.DestroyShared();
	}

	// Token: 0x060014AA RID: 5290 RVA: 0x000A3CB9 File Offset: 0x000A1EB9
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
	}

	// Token: 0x060014AB RID: 5291 RVA: 0x000A3CC3 File Offset: 0x000A1EC3
	public void FullUpdate()
	{
		this.RefreshSellOrderStockLevel(null);
		this.UpdateMapMarker();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060014AC RID: 5292 RVA: 0x000A3CD9 File Offset: 0x000A1ED9
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		base.CancelInvoke(this.fullUpdateCached);
		base.Invoke(this.fullUpdateCached, 0.2f);
	}

	// Token: 0x060014AD RID: 5293 RVA: 0x000A3D00 File Offset: 0x000A1F00
	public void RefreshSellOrderStockLevel(ItemDefinition itemDef = null)
	{
		foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in this.sellOrders.sellOrders)
		{
			if (itemDef == null || itemDef.itemid == sellOrder.itemToSellID)
			{
				List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
				this.GetItemsToSell(sellOrder, list);
				ProtoBuf.VendingMachine.SellOrder sellOrder2 = sellOrder;
				int inStock;
				if (list.Count < 0)
				{
					inStock = 0;
				}
				else
				{
					inStock = list.Sum((global::Item x) => x.amount) / sellOrder.itemToSellAmount;
				}
				sellOrder2.inStock = inStock;
				float itemCondition = 0f;
				float itemConditionMax = 0f;
				int instanceData = 0;
				if (list.Count > 0)
				{
					if (list[0].hasCondition)
					{
						itemCondition = list[0].condition;
						itemConditionMax = list[0].maxCondition;
					}
					if (list[0].info != null && list[0].info.amountType == ItemDefinition.AmountType.Genetics && list[0].instanceData != null)
					{
						instanceData = list[0].instanceData.dataInt;
						sellOrder.inStock = list[0].amount;
					}
				}
				sellOrder.itemCondition = itemCondition;
				sellOrder.itemConditionMax = itemConditionMax;
				sellOrder.instanceData = instanceData;
				Facepunch.Pool.FreeList<global::Item>(ref list);
			}
		}
	}

	// Token: 0x060014AE RID: 5294 RVA: 0x000A3E88 File Offset: 0x000A2088
	public bool OutOfStock()
	{
		using (List<ProtoBuf.VendingMachine.SellOrder>.Enumerator enumerator = this.sellOrders.sellOrders.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.inStock > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060014AF RID: 5295 RVA: 0x000A3EE8 File Offset: 0x000A20E8
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		this.RefreshSellOrderStockLevel(null);
		this.UpdateMapMarker();
	}

	// Token: 0x060014B0 RID: 5296 RVA: 0x000A3F0B File Offset: 0x000A210B
	public void UpdateEmptyFlag()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved1, base.inventory.itemList.Count == 0, false, true);
	}

	// Token: 0x060014B1 RID: 5297 RVA: 0x000A3F2D File Offset: 0x000A212D
	public override void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		this.UpdateEmptyFlag();
		if (this.vend_Player != null && this.vend_Player == player)
		{
			this.ClearPendingOrder();
		}
	}

	// Token: 0x060014B2 RID: 5298 RVA: 0x000A3F5E File Offset: 0x000A215E
	public virtual void InstallDefaultSellOrders()
	{
		this.sellOrders = new ProtoBuf.VendingMachine.SellOrderContainer();
		this.sellOrders.ShouldPool = false;
		this.sellOrders.sellOrders = new List<ProtoBuf.VendingMachine.SellOrder>();
	}

	// Token: 0x060014B3 RID: 5299 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool HasVendingSounds()
	{
		return true;
	}

	// Token: 0x060014B4 RID: 5300 RVA: 0x000A3F87 File Offset: 0x000A2187
	public virtual float GetBuyDuration()
	{
		return 2.5f;
	}

	// Token: 0x060014B5 RID: 5301 RVA: 0x000A3F8E File Offset: 0x000A218E
	public void SetPendingOrder(global::BasePlayer buyer, int sellOrderId, int numberOfTransactions)
	{
		this.ClearPendingOrder();
		this.vend_Player = buyer;
		this.vend_sellOrderID = sellOrderId;
		this.vend_numberOfTransactions = numberOfTransactions;
		base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
		if (this.HasVendingSounds())
		{
			base.ClientRPC<int>(null, "CLIENT_StartVendingSounds", sellOrderId);
		}
	}

	// Token: 0x060014B6 RID: 5302 RVA: 0x000A3FD0 File Offset: 0x000A21D0
	public void ClearPendingOrder()
	{
		base.CancelInvoke(new Action(this.CompletePendingOrder));
		this.vend_Player = null;
		this.vend_sellOrderID = -1;
		this.vend_numberOfTransactions = -1;
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		base.ClientRPC(null, "CLIENT_CancelVendingSounds");
	}

	// Token: 0x060014B7 RID: 5303 RVA: 0x000A4020 File Offset: 0x000A2220
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void BuyItem(global::BaseEntity.RPCMessage rpc)
	{
		if (!base.OccupiedCheck(rpc.player))
		{
			return;
		}
		int sellOrderId = rpc.read.Int32();
		int numberOfTransactions = rpc.read.Int32();
		if (this.IsVending())
		{
			rpc.player.ShowToast(GameTip.Styles.Red_Normal, global::VendingMachine.WaitForVendingMessage, Array.Empty<string>());
			return;
		}
		this.SetPendingOrder(rpc.player, sellOrderId, numberOfTransactions);
		base.Invoke(new Action(this.CompletePendingOrder), this.GetBuyDuration());
	}

	// Token: 0x060014B8 RID: 5304 RVA: 0x000A409A File Offset: 0x000A229A
	public virtual void CompletePendingOrder()
	{
		this.DoTransaction(this.vend_Player, this.vend_sellOrderID, this.vend_numberOfTransactions, null, null, null, null);
		this.ClearPendingOrder();
		global::Decay.RadialDecayTouch(base.transform.position, 40f, 2097408);
	}

	// Token: 0x060014B9 RID: 5305 RVA: 0x000063A5 File Offset: 0x000045A5
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void TransactionStart(global::BaseEntity.RPCMessage rpc)
	{
	}

	// Token: 0x060014BA RID: 5306 RVA: 0x000A40DC File Offset: 0x000A22DC
	private void GetItemsToSell(ProtoBuf.VendingMachine.SellOrder sellOrder, List<global::Item> items)
	{
		if (sellOrder.itemToSellIsBP)
		{
			using (List<global::Item>.Enumerator enumerator = base.inventory.itemList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::Item item = enumerator.Current;
					if (item.info.itemid == this.blueprintBaseDef.itemid && item.blueprintTarget == sellOrder.itemToSellID)
					{
						items.Add(item);
					}
				}
				return;
			}
		}
		foreach (global::Item item2 in base.inventory.itemList)
		{
			if (item2.info.itemid == sellOrder.itemToSellID)
			{
				items.Add(item2);
			}
		}
	}

	// Token: 0x060014BB RID: 5307 RVA: 0x000A41BC File Offset: 0x000A23BC
	public bool DoTransaction(global::BasePlayer buyer, int sellOrderId, int numberOfTransactions = 1, global::ItemContainer targetContainer = null, Action<global::BasePlayer, global::Item> onCurrencyRemoved = null, Action<global::BasePlayer, global::Item> onItemPurchased = null, global::MarketTerminal droneMarketTerminal = null)
	{
		if (sellOrderId < 0 || sellOrderId >= this.sellOrders.sellOrders.Count)
		{
			return false;
		}
		if (targetContainer == null && Vector3.Distance(buyer.transform.position, base.transform.position) > 4f)
		{
			return false;
		}
		ProtoBuf.VendingMachine.SellOrder sellOrder = this.sellOrders.sellOrders[sellOrderId];
		List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
		this.GetItemsToSell(sellOrder, list);
		if (list == null || list.Count == 0)
		{
			Facepunch.Pool.FreeList<global::Item>(ref list);
			return false;
		}
		numberOfTransactions = Mathf.Clamp(numberOfTransactions, 1, list[0].hasCondition ? 1 : 1000000);
		int num = sellOrder.itemToSellAmount * numberOfTransactions;
		int num2 = list.Sum((global::Item x) => x.amount);
		if (num > num2)
		{
			Facepunch.Pool.FreeList<global::Item>(ref list);
			return false;
		}
		List<global::Item> list2 = buyer.inventory.FindItemIDs(sellOrder.currencyID);
		if (sellOrder.currencyIsBP)
		{
			list2 = (from x in buyer.inventory.FindItemIDs(this.blueprintBaseDef.itemid)
			where x.blueprintTarget == sellOrder.currencyID
			select x).ToList<global::Item>();
		}
		list2 = (from x in list2
		where !x.hasCondition || (x.conditionNormalized >= 0.5f && x.maxConditionNormalized > 0.5f)
		select x).ToList<global::Item>();
		if (list2.Count == 0)
		{
			Facepunch.Pool.FreeList<global::Item>(ref list);
			return false;
		}
		int num3 = list2.Sum((global::Item x) => x.amount);
		int num4 = sellOrder.currencyAmountPerItem * numberOfTransactions;
		if (num3 < num4)
		{
			Facepunch.Pool.FreeList<global::Item>(ref list);
			return false;
		}
		this.transactionActive = true;
		int num5 = 0;
		foreach (global::Item item in list2)
		{
			int num6 = Mathf.Min(num4 - num5, item.amount);
			global::Item item2;
			if (item.amount <= num6)
			{
				item2 = item;
			}
			else
			{
				item2 = item.SplitItem(num6);
			}
			this.TakeCurrencyItem(item2);
			if (onCurrencyRemoved != null)
			{
				onCurrencyRemoved(buyer, item2);
			}
			num5 += num6;
			if (num5 >= num4)
			{
				break;
			}
		}
		Analytics.Azure.OnBuyFromVendingMachine(buyer, this, sellOrder.itemToSellID, sellOrder.itemToSellAmount * numberOfTransactions, sellOrder.itemToSellIsBP, sellOrder.currencyID, sellOrder.currencyAmountPerItem * numberOfTransactions, sellOrder.currencyIsBP, numberOfTransactions, droneMarketTerminal);
		int num7 = 0;
		foreach (global::Item item3 in list)
		{
			int num8 = num - num7;
			global::Item item4;
			if (item3.amount <= num8)
			{
				item4 = item3;
			}
			else
			{
				item4 = item3.SplitItem(num8);
			}
			if (item4 == null)
			{
				Debug.LogError("Vending machine error, contact developers!");
			}
			else
			{
				num7 += item4.amount;
				this.RecordSaleAnalytics(item4);
				if (targetContainer == null)
				{
					this.GiveSoldItem(item4, buyer);
				}
				else if (!item4.MoveToContainer(targetContainer, -1, true, false, null, true))
				{
					item4.Drop(targetContainer.dropPosition, targetContainer.dropVelocity, default(Quaternion));
				}
				if (onItemPurchased != null)
				{
					onItemPurchased(buyer, item4);
				}
			}
			if (num7 >= num)
			{
				break;
			}
		}
		Facepunch.Pool.FreeList<global::Item>(ref list);
		this.UpdateEmptyFlag();
		this.transactionActive = false;
		return true;
	}

	// Token: 0x060014BC RID: 5308 RVA: 0x000A4560 File Offset: 0x000A2760
	protected virtual void RecordSaleAnalytics(global::Item itemSold)
	{
		Analytics.Server.VendingMachineTransaction(null, itemSold.info, itemSold.amount);
	}

	// Token: 0x060014BD RID: 5309 RVA: 0x000A4574 File Offset: 0x000A2774
	public virtual void TakeCurrencyItem(global::Item takenCurrencyItem)
	{
		if (!takenCurrencyItem.MoveToContainer(base.inventory, -1, true, false, null, true))
		{
			takenCurrencyItem.Drop(base.inventory.dropPosition, Vector3.zero, default(Quaternion));
		}
	}

	// Token: 0x060014BE RID: 5310 RVA: 0x000A45B4 File Offset: 0x000A27B4
	public virtual void GiveSoldItem(global::Item soldItem, global::BasePlayer buyer)
	{
		while (soldItem.amount > soldItem.MaxStackable())
		{
			global::Item item = soldItem.SplitItem(soldItem.MaxStackable());
			buyer.GiveItem(item, global::BaseEntity.GiveItemReason.PickedUp);
		}
		buyer.GiveItem(soldItem, global::BaseEntity.GiveItemReason.PickedUp);
	}

	// Token: 0x060014BF RID: 5311 RVA: 0x000A45EE File Offset: 0x000A27EE
	public void SendSellOrders(global::BasePlayer player = null)
	{
		if (player)
		{
			base.ClientRPCPlayer<ProtoBuf.VendingMachine.SellOrderContainer>(null, player, "CLIENT_ReceiveSellOrders", this.sellOrders);
			return;
		}
		base.ClientRPC<ProtoBuf.VendingMachine.SellOrderContainer>(null, "CLIENT_ReceiveSellOrders", this.sellOrders);
	}

	// Token: 0x060014C0 RID: 5312 RVA: 0x000A4620 File Offset: 0x000A2820
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Broadcast(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		bool b = msg.read.Bit();
		if (this.CanPlayerAdmin(player))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved4, b, false, true);
			this.UpdateMapMarker();
		}
	}

	// Token: 0x060014C1 RID: 5313 RVA: 0x000A4660 File Offset: 0x000A2860
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_UpdateShopName(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		string text = msg.read.String(32);
		if (this.CanPlayerAdmin(player))
		{
			this.shopName = text;
			this.UpdateMapMarker();
		}
	}

	// Token: 0x060014C2 RID: 5314 RVA: 0x000A4698 File Offset: 0x000A2898
	public void UpdateMapMarker()
	{
		if (!this.IsBroadcasting())
		{
			if (this.myMarker)
			{
				this.myMarker.Kill(global::BaseNetworkable.DestroyMode.None);
				this.myMarker = null;
			}
			return;
		}
		bool flag = false;
		if (this.myMarker == null)
		{
			this.myMarker = (GameManager.server.CreateEntity(this.mapMarkerPrefab.resourcePath, base.transform.position, Quaternion.identity, true) as VendingMachineMapMarker);
			flag = true;
		}
		this.myMarker.SetFlag(global::BaseEntity.Flags.Busy, this.OutOfStock(), false, true);
		this.myMarker.markerShopName = this.shopName;
		this.myMarker.server_vendingMachine = this;
		if (flag)
		{
			this.myMarker.Spawn();
			return;
		}
		this.myMarker.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060014C3 RID: 5315 RVA: 0x000A4764 File Offset: 0x000A2964
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_OpenShop(global::BaseEntity.RPCMessage msg)
	{
		if (!base.OccupiedCheck(msg.player))
		{
			return;
		}
		this.SendSellOrders(msg.player);
		this.PlayerOpenLoot(msg.player, this.customerPanel, true);
	}

	// Token: 0x060014C4 RID: 5316 RVA: 0x000A4798 File Offset: 0x000A2998
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_OpenAdmin(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanPlayerAdmin(player))
		{
			return;
		}
		this.SendSellOrders(player);
		this.PlayerOpenLoot(player, "", true);
		base.ClientRPCPlayer(null, player, "CLIENT_OpenAdminMenu");
	}

	// Token: 0x060014C5 RID: 5317 RVA: 0x000A47D8 File Offset: 0x000A29D8
	public void OnIndustrialItemTransferBegins()
	{
		this.industrialItemIncoming = true;
	}

	// Token: 0x060014C6 RID: 5318 RVA: 0x000A47E1 File Offset: 0x000A29E1
	public void OnIndustrialItemTransferEnds()
	{
		this.industrialItemIncoming = false;
	}

	// Token: 0x060014C7 RID: 5319 RVA: 0x000A47EC File Offset: 0x000A29EC
	public bool CanAcceptItem(global::Item item, int targetSlot)
	{
		global::BasePlayer ownerPlayer = item.GetOwnerPlayer();
		return this.transactionActive || this.industrialItemIncoming || item.parent == null || base.inventory.itemList.Contains(item) || (!(ownerPlayer == null) && this.CanPlayerAdmin(ownerPlayer));
	}

	// Token: 0x060014C8 RID: 5320 RVA: 0x000A4843 File Offset: 0x000A2A43
	public override bool CanMoveFrom(global::BasePlayer player, global::Item item)
	{
		return this.CanPlayerAdmin(player);
	}

	// Token: 0x060014C9 RID: 5321 RVA: 0x000A484C File Offset: 0x000A2A4C
	public override bool CanOpenLootPanel(global::BasePlayer player, string panelName)
	{
		return panelName == this.customerPanel || (base.CanOpenLootPanel(player, panelName) && this.CanPlayerAdmin(player));
	}

	// Token: 0x060014CA RID: 5322 RVA: 0x000A4874 File Offset: 0x000A2A74
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_DeleteSellOrder(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanPlayerAdmin(player))
		{
			return;
		}
		int num = msg.read.Int32();
		if (num >= 0 && num < this.sellOrders.sellOrders.Count)
		{
			ProtoBuf.VendingMachine.SellOrder sellOrder = this.sellOrders.sellOrders[num];
			Analytics.Azure.OnVendingMachineOrderChanged(msg.player, this, sellOrder.itemToSellID, sellOrder.itemToSellAmount, sellOrder.itemToSellIsBP, sellOrder.currencyID, sellOrder.currencyAmountPerItem, sellOrder.currencyIsBP, false);
			this.sellOrders.sellOrders.RemoveAt(num);
		}
		this.RefreshSellOrderStockLevel(null);
		this.UpdateMapMarker();
		this.SendSellOrders(player);
	}

	// Token: 0x060014CB RID: 5323 RVA: 0x000A4920 File Offset: 0x000A2B20
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_RotateVM(global::BaseEntity.RPCMessage msg)
	{
		if (!this.CanRotate())
		{
			return;
		}
		this.UpdateEmptyFlag();
		if (msg.player.CanBuild() && this.IsInventoryEmpty())
		{
			base.transform.rotation = Quaternion.LookRotation(-base.transform.forward, base.transform.up);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060014CC RID: 5324 RVA: 0x000A4984 File Offset: 0x000A2B84
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_AddSellOrder(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanPlayerAdmin(player))
		{
			return;
		}
		if (this.sellOrders.sellOrders.Count >= 7)
		{
			player.ChatMessage("Too many sell orders - remove some");
			return;
		}
		int num = msg.read.Int32();
		int num2 = msg.read.Int32();
		int num3 = msg.read.Int32();
		int num4 = msg.read.Int32();
		byte b = msg.read.UInt8();
		this.AddSellOrder(num, num2, num3, num4, b);
		Analytics.Azure.OnVendingMachineOrderChanged(msg.player, this, num, num2, b == 2 || b == 3, num3, num4, b == 1 || b == 3, true);
	}

	// Token: 0x060014CD RID: 5325 RVA: 0x000A4A38 File Offset: 0x000A2C38
	public void AddSellOrder(int itemToSellID, int itemToSellAmount, int currencyToUseID, int currencyAmount, byte bpState)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemToSellID);
		ItemDefinition x = ItemManager.FindItemDefinition(currencyToUseID);
		if (itemDefinition == null || x == null)
		{
			return;
		}
		currencyAmount = Mathf.Clamp(currencyAmount, 1, 10000);
		itemToSellAmount = Mathf.Clamp(itemToSellAmount, 1, itemDefinition.stackable);
		ProtoBuf.VendingMachine.SellOrder sellOrder = new ProtoBuf.VendingMachine.SellOrder();
		sellOrder.ShouldPool = false;
		sellOrder.itemToSellID = itemToSellID;
		sellOrder.itemToSellAmount = itemToSellAmount;
		sellOrder.currencyID = currencyToUseID;
		sellOrder.currencyAmountPerItem = currencyAmount;
		sellOrder.currencyIsBP = (bpState == 3 || bpState == 2);
		sellOrder.itemToSellIsBP = (bpState == 3 || bpState == 1);
		this.sellOrders.sellOrders.Add(sellOrder);
		this.RefreshSellOrderStockLevel(itemDefinition);
		this.UpdateMapMarker();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060014CE RID: 5326 RVA: 0x000A4AF9 File Offset: 0x000A2CF9
	public void RefreshAndSendNetworkUpdate()
	{
		this.RefreshSellOrderStockLevel(null);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060014CF RID: 5327 RVA: 0x000A4B0C File Offset: 0x000A2D0C
	public void UpdateOrCreateSalesSheet()
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition("note");
		List<global::Item> list = base.inventory.FindItemsByItemID(itemDefinition.itemid);
		global::Item item = null;
		foreach (global::Item item2 in list)
		{
			if (item2.text.Length == 0)
			{
				item = item2;
				break;
			}
		}
		if (item == null)
		{
			ItemDefinition itemDefinition2 = ItemManager.FindItemDefinition("paper");
			global::Item item3 = base.inventory.FindItemByItemID(itemDefinition2.itemid);
			if (item3 != null)
			{
				item = ItemManager.CreateByItemID(itemDefinition.itemid, 1, 0UL);
				if (!item.MoveToContainer(base.inventory, -1, true, false, null, true))
				{
					item.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
				}
				item3.UseItem(1);
			}
		}
		if (item != null)
		{
			foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in this.sellOrders.sellOrders)
			{
				ItemDefinition itemDefinition3 = ItemManager.FindItemDefinition(sellOrder.itemToSellID);
				global::Item item4 = item;
				item4.text = item4.text + itemDefinition3.displayName.translated + "\n";
			}
			item.MarkDirty();
		}
	}

	// Token: 0x060014D0 RID: 5328 RVA: 0x000A4C68 File Offset: 0x000A2E68
	protected virtual bool CanRotate()
	{
		return !base.HasAttachedStorageAdaptor();
	}

	// Token: 0x060014D1 RID: 5329 RVA: 0x000238E0 File Offset: 0x00021AE0
	public bool IsBroadcasting()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved4);
	}

	// Token: 0x060014D2 RID: 5330 RVA: 0x000231B4 File Offset: 0x000213B4
	public bool IsInventoryEmpty()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved1);
	}

	// Token: 0x060014D3 RID: 5331 RVA: 0x0000564C File Offset: 0x0000384C
	public bool IsVending()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved2);
	}

	// Token: 0x060014D4 RID: 5332 RVA: 0x000A4C74 File Offset: 0x000A2E74
	public bool PlayerBehind(global::BasePlayer player)
	{
		return Vector3.Dot(base.transform.forward, (player.transform.position - base.transform.position).normalized) <= -0.7f;
	}

	// Token: 0x060014D5 RID: 5333 RVA: 0x000A4CC0 File Offset: 0x000A2EC0
	public bool PlayerInfront(global::BasePlayer player)
	{
		return Vector3.Dot(base.transform.forward, (player.transform.position - base.transform.position).normalized) >= 0.7f;
	}

	// Token: 0x060014D6 RID: 5334 RVA: 0x000A4D0A File Offset: 0x000A2F0A
	public virtual bool CanPlayerAdmin(global::BasePlayer player)
	{
		return this.PlayerBehind(player) && base.OccupiedCheck(player);
	}

	// Token: 0x060014D7 RID: 5335 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x02000C15 RID: 3093
	public static class VendingMachineFlags
	{
		// Token: 0x040041F4 RID: 16884
		public const global::BaseEntity.Flags EmptyInv = global::BaseEntity.Flags.Reserved1;

		// Token: 0x040041F5 RID: 16885
		public const global::BaseEntity.Flags IsVending = global::BaseEntity.Flags.Reserved2;

		// Token: 0x040041F6 RID: 16886
		public const global::BaseEntity.Flags Broadcasting = global::BaseEntity.Flags.Reserved4;

		// Token: 0x040041F7 RID: 16887
		public const global::BaseEntity.Flags OutOfStock = global::BaseEntity.Flags.Reserved5;

		// Token: 0x040041F8 RID: 16888
		public const global::BaseEntity.Flags NoDirectAccess = global::BaseEntity.Flags.Reserved6;
	}
}
