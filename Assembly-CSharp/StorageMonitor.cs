using System;
using Facepunch;
using Network;
using ProtoBuf;

// Token: 0x020004C4 RID: 1220
public class StorageMonitor : AppIOEntity
{
	// Token: 0x04002030 RID: 8240
	private readonly Action<global::Item, bool> _onContainerChangedHandler;

	// Token: 0x04002031 RID: 8241
	private readonly Action _resetSwitchHandler;

	// Token: 0x04002032 RID: 8242
	private double _lastPowerOnUpdate;

	// Token: 0x17000363 RID: 867
	// (get) Token: 0x060027BF RID: 10175 RVA: 0x000037BE File Offset: 0x000019BE
	public override AppEntityType Type
	{
		get
		{
			return AppEntityType.StorageMonitor;
		}
	}

	// Token: 0x17000364 RID: 868
	// (get) Token: 0x060027C0 RID: 10176 RVA: 0x00007641 File Offset: 0x00005841
	// (set) Token: 0x060027C1 RID: 10177 RVA: 0x000063A5 File Offset: 0x000045A5
	public override bool Value
	{
		get
		{
			return base.IsOn();
		}
		set
		{
		}
	}

	// Token: 0x060027C2 RID: 10178 RVA: 0x000F79A9 File Offset: 0x000F5BA9
	public StorageMonitor()
	{
		this._onContainerChangedHandler = new Action<global::Item, bool>(this.OnContainerChanged);
		this._resetSwitchHandler = new Action(this.ResetSwitch);
	}

	// Token: 0x060027C3 RID: 10179 RVA: 0x000F79D8 File Offset: 0x000F5BD8
	internal override void FillEntityPayload(AppEntityPayload payload)
	{
		base.FillEntityPayload(payload);
		StorageContainer storageContainer = this.GetStorageContainer();
		if (storageContainer == null || !base.HasFlag(global::BaseEntity.Flags.Reserved8))
		{
			return;
		}
		payload.items = Pool.GetList<AppEntityPayload.Item>();
		foreach (global::Item item in storageContainer.inventory.itemList)
		{
			AppEntityPayload.Item item2 = Pool.Get<AppEntityPayload.Item>();
			item2.itemId = (item.IsBlueprint() ? item.blueprintTargetDef.itemid : item.info.itemid);
			item2.quantity = item.amount;
			item2.itemIsBlueprint = item.IsBlueprint();
			payload.items.Add(item2);
		}
		payload.capacity = storageContainer.inventory.capacity;
		BuildingPrivlidge buildingPrivlidge;
		if ((buildingPrivlidge = (storageContainer as BuildingPrivlidge)) != null)
		{
			payload.hasProtection = true;
			float protectedMinutes = buildingPrivlidge.GetProtectedMinutes(false);
			if (protectedMinutes > 0f)
			{
				payload.protectionExpiry = (uint)DateTimeOffset.UtcNow.AddMinutes((double)protectedMinutes).ToUnixTimeSeconds();
			}
		}
	}

	// Token: 0x060027C4 RID: 10180 RVA: 0x000F7B04 File Offset: 0x000F5D04
	public override void Init()
	{
		base.Init();
		StorageContainer storageContainer = this.GetStorageContainer();
		if (storageContainer != null && storageContainer.inventory != null)
		{
			global::ItemContainer inventory = storageContainer.inventory;
			inventory.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Combine(inventory.onItemAddedRemoved, this._onContainerChangedHandler);
		}
	}

	// Token: 0x060027C5 RID: 10181 RVA: 0x000F7B50 File Offset: 0x000F5D50
	public override void DestroyShared()
	{
		base.DestroyShared();
		StorageContainer storageContainer = this.GetStorageContainer();
		if (storageContainer != null && storageContainer.inventory != null)
		{
			global::ItemContainer inventory = storageContainer.inventory;
			inventory.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Remove(inventory.onItemAddedRemoved, this._onContainerChangedHandler);
		}
	}

	// Token: 0x060027C6 RID: 10182 RVA: 0x000F7B9C File Offset: 0x000F5D9C
	private StorageContainer GetStorageContainer()
	{
		return base.GetParentEntity() as StorageContainer;
	}

	// Token: 0x060027C7 RID: 10183 RVA: 0x00062775 File Offset: 0x00060975
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x060027C8 RID: 10184 RVA: 0x000F7BAC File Offset: 0x000F5DAC
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		bool flag = base.HasFlag(global::BaseEntity.Flags.Reserved8);
		base.UpdateHasPower(inputAmount, inputSlot);
		if (inputSlot == 0)
		{
			bool flag2 = inputAmount >= this.ConsumptionAmount();
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			if (flag2 && !flag && this._lastPowerOnUpdate < realtimeSinceStartup - 1.0)
			{
				this._lastPowerOnUpdate = realtimeSinceStartup;
				base.BroadcastValueChange();
			}
		}
	}

	// Token: 0x060027C9 RID: 10185 RVA: 0x000F7C08 File Offset: 0x000F5E08
	private void OnContainerChanged(global::Item item, bool added)
	{
		if (!base.HasFlag(global::BaseEntity.Flags.Reserved8))
		{
			return;
		}
		base.Invoke(this._resetSwitchHandler, 0.5f);
		if (base.IsOn())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
		base.BroadcastValueChange();
	}

	// Token: 0x060027CA RID: 10186 RVA: 0x000F7C5A File Offset: 0x000F5E5A
	private void ResetSwitch()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
		base.BroadcastValueChange();
	}
}
