using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;

// Token: 0x02000428 RID: 1064
public class VendingMachineMapMarker : MapMarker
{
	// Token: 0x04001C0F RID: 7183
	public string markerShopName;

	// Token: 0x04001C10 RID: 7184
	public global::VendingMachine server_vendingMachine;

	// Token: 0x04001C11 RID: 7185
	public ProtoBuf.VendingMachine client_vendingMachine;

	// Token: 0x04001C12 RID: 7186
	[NonSerialized]
	public NetworkableId client_vendingMachineNetworkID;

	// Token: 0x04001C13 RID: 7187
	public GameObjectRef clusterMarkerObj;

	// Token: 0x060023EF RID: 9199 RVA: 0x000E5768 File Offset: 0x000E3968
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.vendingMachine = new ProtoBuf.VendingMachine();
		info.msg.vendingMachine.shopName = this.markerShopName;
		if (this.server_vendingMachine != null)
		{
			info.msg.vendingMachine.networkID = this.server_vendingMachine.net.ID;
			info.msg.vendingMachine.sellOrderContainer = new ProtoBuf.VendingMachine.SellOrderContainer();
			info.msg.vendingMachine.sellOrderContainer.ShouldPool = false;
			info.msg.vendingMachine.sellOrderContainer.sellOrders = new List<ProtoBuf.VendingMachine.SellOrder>();
			foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in this.server_vendingMachine.sellOrders.sellOrders)
			{
				ProtoBuf.VendingMachine.SellOrder sellOrder2 = new ProtoBuf.VendingMachine.SellOrder();
				sellOrder2.ShouldPool = false;
				sellOrder.CopyTo(sellOrder2);
				info.msg.vendingMachine.sellOrderContainer.sellOrders.Add(sellOrder2);
			}
		}
	}

	// Token: 0x060023F0 RID: 9200 RVA: 0x000E5890 File Offset: 0x000E3A90
	public override AppMarker GetAppMarkerData()
	{
		AppMarker appMarkerData = base.GetAppMarkerData();
		appMarkerData.name = (this.markerShopName ?? "");
		appMarkerData.outOfStock = !base.HasFlag(global::BaseEntity.Flags.Busy);
		if (this.server_vendingMachine != null)
		{
			appMarkerData.sellOrders = Pool.GetList<AppMarker.SellOrder>();
			foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in this.server_vendingMachine.sellOrders.sellOrders)
			{
				AppMarker.SellOrder sellOrder2 = Pool.Get<AppMarker.SellOrder>();
				sellOrder2.itemId = sellOrder.itemToSellID;
				sellOrder2.quantity = sellOrder.itemToSellAmount;
				sellOrder2.currencyId = sellOrder.currencyID;
				sellOrder2.costPerItem = sellOrder.currencyAmountPerItem;
				sellOrder2.amountInStock = sellOrder.inStock;
				sellOrder2.itemIsBlueprint = sellOrder.itemToSellIsBP;
				sellOrder2.currencyIsBlueprint = sellOrder.currencyIsBP;
				sellOrder2.itemCondition = sellOrder.itemCondition;
				sellOrder2.itemConditionMax = sellOrder.itemConditionMax;
				appMarkerData.sellOrders.Add(sellOrder2);
			}
		}
		return appMarkerData;
	}
}
