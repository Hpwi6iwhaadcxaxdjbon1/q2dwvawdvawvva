using System;
using UnityEngine;

// Token: 0x020004D8 RID: 1240
public class IndustrialStorageAdaptor : IndustrialEntity, IIndustrialStorage
{
	// Token: 0x0400206A RID: 8298
	public GameObject GreenLight;

	// Token: 0x0400206B RID: 8299
	public GameObject RedLight;

	// Token: 0x0400206C RID: 8300
	private BaseEntity _cachedParent;

	// Token: 0x0400206D RID: 8301
	private ItemContainer cachedContainer;

	// Token: 0x1700036C RID: 876
	// (get) Token: 0x0600282A RID: 10282 RVA: 0x000F896E File Offset: 0x000F6B6E
	public BaseEntity cachedParent
	{
		get
		{
			if (this._cachedParent == null)
			{
				this._cachedParent = base.GetParentEntity();
			}
			return this._cachedParent;
		}
	}

	// Token: 0x1700036D RID: 877
	// (get) Token: 0x0600282B RID: 10283 RVA: 0x000F8990 File Offset: 0x000F6B90
	public ItemContainer Container
	{
		get
		{
			if (this.cachedContainer == null)
			{
				StorageContainer storageContainer = this.cachedParent as StorageContainer;
				this.cachedContainer = ((storageContainer != null) ? storageContainer.inventory : null);
			}
			return this.cachedContainer;
		}
	}

	// Token: 0x0600282C RID: 10284 RVA: 0x000F89BD File Offset: 0x000F6BBD
	public override void ServerInit()
	{
		base.ServerInit();
		this._cachedParent = null;
		this.cachedContainer = null;
	}

	// Token: 0x0600282D RID: 10285 RVA: 0x000F89D4 File Offset: 0x000F6BD4
	public Vector2i InputSlotRange(int slotIndex)
	{
		if (this.cachedParent != null)
		{
			IIndustrialStorage industrialStorage;
			if ((industrialStorage = (this.cachedParent as IIndustrialStorage)) != null)
			{
				return industrialStorage.InputSlotRange(slotIndex);
			}
			Locker locker;
			if ((locker = (this.cachedParent as Locker)) != null)
			{
				Vector3 localPosition = base.transform.localPosition;
				return locker.GetIndustrialSlotRange(localPosition);
			}
		}
		if (this.Container != null)
		{
			return new Vector2i(0, this.Container.capacity - 1);
		}
		return new Vector2i(0, 0);
	}

	// Token: 0x0600282E RID: 10286 RVA: 0x000F8A4C File Offset: 0x000F6C4C
	public Vector2i OutputSlotRange(int slotIndex)
	{
		if (this.cachedParent != null)
		{
			if (this.cachedParent is DropBox && this.Container != null)
			{
				return new Vector2i(0, this.Container.capacity - 2);
			}
			IIndustrialStorage industrialStorage;
			if ((industrialStorage = (this.cachedParent as IIndustrialStorage)) != null)
			{
				return industrialStorage.OutputSlotRange(slotIndex);
			}
			Locker locker;
			if ((locker = (this.cachedParent as Locker)) != null)
			{
				Vector3 localPosition = base.transform.localPosition;
				return locker.GetIndustrialSlotRange(localPosition);
			}
		}
		if (this.Container != null)
		{
			return new Vector2i(0, this.Container.capacity - 1);
		}
		return new Vector2i(0, 0);
	}

	// Token: 0x0600282F RID: 10287 RVA: 0x000F8AF0 File Offset: 0x000F6CF0
	public void OnStorageItemTransferBegin()
	{
		VendingMachine vendingMachine;
		if (this.cachedParent != null && (vendingMachine = (this.cachedParent as VendingMachine)) != null)
		{
			vendingMachine.OnIndustrialItemTransferBegins();
		}
	}

	// Token: 0x06002830 RID: 10288 RVA: 0x000F8B20 File Offset: 0x000F6D20
	public void OnStorageItemTransferEnd()
	{
		VendingMachine vendingMachine;
		if (this.cachedParent != null && (vendingMachine = (this.cachedParent as VendingMachine)) != null)
		{
			vendingMachine.OnIndustrialItemTransferEnds();
		}
	}

	// Token: 0x1700036E RID: 878
	// (get) Token: 0x06002831 RID: 10289 RVA: 0x000037E7 File Offset: 0x000019E7
	public BaseEntity IndustrialEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06002832 RID: 10290 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x06002833 RID: 10291 RVA: 0x000F8B50 File Offset: 0x000F6D50
	public void ClientNotifyItemAddRemoved(bool add)
	{
		if (add)
		{
			this.GreenLight.SetActive(false);
			this.GreenLight.SetActive(true);
			return;
		}
		this.RedLight.SetActive(false);
		this.RedLight.SetActive(true);
	}
}
