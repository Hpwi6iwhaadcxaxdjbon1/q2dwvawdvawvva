using System;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x02000129 RID: 297
public class NPCVendingMachine : VendingMachine
{
	// Token: 0x04000ECB RID: 3787
	public NPCVendingOrder vendingOrders;

	// Token: 0x04000ECC RID: 3788
	private float[] refillTimes;

	// Token: 0x060016A3 RID: 5795 RVA: 0x000AE940 File Offset: 0x000ACB40
	public byte GetBPState(bool sellItemAsBP, bool currencyItemAsBP)
	{
		byte result = 0;
		if (sellItemAsBP)
		{
			result = 1;
		}
		if (currencyItemAsBP)
		{
			result = 2;
		}
		if (sellItemAsBP && currencyItemAsBP)
		{
			result = 3;
		}
		return result;
	}

	// Token: 0x060016A4 RID: 5796 RVA: 0x000AE961 File Offset: 0x000ACB61
	public override void TakeCurrencyItem(Item takenCurrencyItem)
	{
		takenCurrencyItem.MoveToContainer(base.inventory, -1, true, false, null, true);
		takenCurrencyItem.RemoveFromContainer();
		takenCurrencyItem.Remove(0f);
	}

	// Token: 0x060016A5 RID: 5797 RVA: 0x000AE986 File Offset: 0x000ACB86
	public override void GiveSoldItem(Item soldItem, BasePlayer buyer)
	{
		base.GiveSoldItem(soldItem, buyer);
	}

	// Token: 0x060016A6 RID: 5798 RVA: 0x000AE990 File Offset: 0x000ACB90
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.Invoke(new Action(this.InstallFromVendingOrders), 1f);
	}

	// Token: 0x060016A7 RID: 5799 RVA: 0x000AE9B0 File Offset: 0x000ACBB0
	public override void ServerInit()
	{
		base.ServerInit();
		this.skinID = 861142659UL;
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		base.Invoke(new Action(this.InstallFromVendingOrders), 1f);
		base.InvokeRandomized(new Action(this.Refill), 1f, 1f, 0.1f);
	}

	// Token: 0x060016A8 RID: 5800 RVA: 0x000AEA10 File Offset: 0x000ACC10
	public virtual void InstallFromVendingOrders()
	{
		if (this.vendingOrders == null)
		{
			Debug.LogError("No vending orders!");
			return;
		}
		this.ClearSellOrders();
		base.inventory.Clear();
		ItemManager.DoRemoves();
		foreach (NPCVendingOrder.Entry entry in this.vendingOrders.orders)
		{
			this.AddItemForSale(entry.sellItem.itemid, entry.sellItemAmount, entry.currencyItem.itemid, entry.currencyAmount, this.GetBPState(entry.sellItemAsBP, entry.currencyAsBP));
		}
	}

	// Token: 0x060016A9 RID: 5801 RVA: 0x000AEAA4 File Offset: 0x000ACCA4
	public override void InstallDefaultSellOrders()
	{
		base.InstallDefaultSellOrders();
	}

	// Token: 0x060016AA RID: 5802 RVA: 0x000AEAAC File Offset: 0x000ACCAC
	public void Refill()
	{
		if (this.vendingOrders == null || this.vendingOrders.orders == null)
		{
			return;
		}
		if (base.inventory == null)
		{
			return;
		}
		if (this.refillTimes == null)
		{
			this.refillTimes = new float[this.vendingOrders.orders.Length];
		}
		for (int i = 0; i < this.vendingOrders.orders.Length; i++)
		{
			NPCVendingOrder.Entry entry = this.vendingOrders.orders[i];
			if (Time.realtimeSinceStartup > this.refillTimes[i])
			{
				int num = Mathf.FloorToInt((float)(base.inventory.GetAmount(entry.sellItem.itemid, false) / entry.sellItemAmount));
				int num2 = Mathf.Min(10 - num, entry.refillAmount) * entry.sellItemAmount;
				if (num2 > 0)
				{
					this.transactionActive = true;
					Item item;
					if (entry.sellItemAsBP)
					{
						item = ItemManager.Create(this.blueprintBaseDef, num2, 0UL);
						item.blueprintTarget = entry.sellItem.itemid;
					}
					else
					{
						item = ItemManager.Create(entry.sellItem, num2, 0UL);
					}
					item.MoveToContainer(base.inventory, -1, true, false, null, true);
					this.transactionActive = false;
				}
				this.refillTimes[i] = Time.realtimeSinceStartup + entry.refillDelay;
			}
		}
	}

	// Token: 0x060016AB RID: 5803 RVA: 0x000AEBF2 File Offset: 0x000ACDF2
	public void ClearSellOrders()
	{
		this.sellOrders.sellOrders.Clear();
	}

	// Token: 0x060016AC RID: 5804 RVA: 0x000AEC04 File Offset: 0x000ACE04
	public void AddItemForSale(int itemID, int amountToSell, int currencyID, int currencyPerTransaction, byte bpState)
	{
		base.AddSellOrder(itemID, amountToSell, currencyID, currencyPerTransaction, bpState);
		this.transactionActive = true;
		int num = 10;
		if (bpState == 1 || bpState == 3)
		{
			for (int i = 0; i < num; i++)
			{
				Item item = ItemManager.CreateByItemID(this.blueprintBaseDef.itemid, 1, 0UL);
				item.blueprintTarget = itemID;
				base.inventory.Insert(item);
			}
		}
		else
		{
			base.inventory.AddItem(ItemManager.FindItemDefinition(itemID), amountToSell * num, 0UL, ItemContainer.LimitStack.Existing);
		}
		this.transactionActive = false;
		base.RefreshSellOrderStockLevel(null);
	}

	// Token: 0x060016AD RID: 5805 RVA: 0x000063A5 File Offset: 0x000045A5
	public void RefreshStock()
	{
	}

	// Token: 0x060016AE RID: 5806 RVA: 0x000AEC8D File Offset: 0x000ACE8D
	protected override void RecordSaleAnalytics(Item itemSold)
	{
		Analytics.Server.VendingMachineTransaction(this.vendingOrders, itemSold.info, itemSold.amount);
	}

	// Token: 0x060016AF RID: 5807 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected override bool CanRotate()
	{
		return false;
	}

	// Token: 0x060016B0 RID: 5808 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool CanPlayerAdmin(BasePlayer player)
	{
		return false;
	}
}
