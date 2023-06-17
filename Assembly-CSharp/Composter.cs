using System;
using ConVar;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000104 RID: 260
public class Composter : StorageContainer
{
	// Token: 0x04000DE5 RID: 3557
	[Header("Composter")]
	public ItemDefinition FertilizerDef;

	// Token: 0x04000DE6 RID: 3558
	[Tooltip("If enabled, entire item stacks will be composted each tick, instead of a single item of a stack.")]
	public bool CompostEntireStack;

	// Token: 0x04000DE7 RID: 3559
	private float fertilizerProductionProgress;

	// Token: 0x170001E6 RID: 486
	// (get) Token: 0x060015A2 RID: 5538 RVA: 0x000AAC07 File Offset: 0x000A8E07
	protected float UpdateInterval
	{
		get
		{
			return Server.composterUpdateInterval;
		}
	}

	// Token: 0x060015A3 RID: 5539 RVA: 0x000AAC10 File Offset: 0x000A8E10
	public override void ServerInit()
	{
		base.ServerInit();
		global::ItemContainer inventory = base.inventory;
		inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.InventoryItemFilter));
		base.InvokeRandomized(new Action(this.UpdateComposting), this.UpdateInterval, this.UpdateInterval, this.UpdateInterval * 0.1f);
	}

	// Token: 0x060015A4 RID: 5540 RVA: 0x000AAC74 File Offset: 0x000A8E74
	public bool InventoryItemFilter(global::Item item, int targetSlot)
	{
		return item != null && (item.info.GetComponent<ItemModCompostable>() != null || this.ItemIsFertilizer(item));
	}

	// Token: 0x060015A5 RID: 5541 RVA: 0x000AAC9A File Offset: 0x000A8E9A
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.composter = Facepunch.Pool.Get<ProtoBuf.Composter>();
		info.msg.composter.fertilizerProductionProgress = this.fertilizerProductionProgress;
	}

	// Token: 0x060015A6 RID: 5542 RVA: 0x000AACC9 File Offset: 0x000A8EC9
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.composter != null)
		{
			this.fertilizerProductionProgress = info.msg.composter.fertilizerProductionProgress;
		}
	}

	// Token: 0x060015A7 RID: 5543 RVA: 0x00083D57 File Offset: 0x00081F57
	private bool ItemIsFertilizer(global::Item item)
	{
		return item.info.shortname == "fertilizer";
	}

	// Token: 0x060015A8 RID: 5544 RVA: 0x000AACF8 File Offset: 0x000A8EF8
	public void UpdateComposting()
	{
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot != null)
			{
				this.CompostItem(slot);
			}
		}
	}

	// Token: 0x060015A9 RID: 5545 RVA: 0x000AAD34 File Offset: 0x000A8F34
	private void CompostItem(global::Item item)
	{
		if (this.ItemIsFertilizer(item))
		{
			return;
		}
		ItemModCompostable component = item.info.GetComponent<ItemModCompostable>();
		if (component == null)
		{
			return;
		}
		int num = this.CompostEntireStack ? item.amount : 1;
		item.UseItem(num);
		this.fertilizerProductionProgress += (float)num * component.TotalFertilizerProduced;
		this.ProduceFertilizer(Mathf.FloorToInt(this.fertilizerProductionProgress));
	}

	// Token: 0x060015AA RID: 5546 RVA: 0x000AADA4 File Offset: 0x000A8FA4
	private void ProduceFertilizer(int amount)
	{
		if (amount <= 0)
		{
			return;
		}
		global::Item item = ItemManager.Create(this.FertilizerDef, amount, 0UL);
		if (!item.MoveToContainer(base.inventory, -1, true, false, null, true))
		{
			item.Drop(base.inventory.dropPosition, base.inventory.dropVelocity, default(Quaternion));
		}
		this.fertilizerProductionProgress -= (float)amount;
	}
}
