using System;
using System.Collections.Generic;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x020003E9 RID: 1001
public class EntityFuelSystem
{
	// Token: 0x04001A72 RID: 6770
	private readonly bool isServer;

	// Token: 0x04001A73 RID: 6771
	private readonly bool editorGiveFreeFuel;

	// Token: 0x04001A74 RID: 6772
	private readonly uint fuelStorageID;

	// Token: 0x04001A75 RID: 6773
	public EntityRef<StorageContainer> fuelStorageInstance;

	// Token: 0x04001A76 RID: 6774
	private float nextFuelCheckTime;

	// Token: 0x04001A77 RID: 6775
	private bool cachedHasFuel;

	// Token: 0x04001A78 RID: 6776
	private float pendingFuel;

	// Token: 0x06002251 RID: 8785 RVA: 0x000DD970 File Offset: 0x000DBB70
	public EntityFuelSystem(bool isServer, GameObjectRef fuelStoragePrefab, List<BaseEntity> children, bool editorGiveFreeFuel = true)
	{
		this.isServer = isServer;
		this.editorGiveFreeFuel = editorGiveFreeFuel;
		this.fuelStorageID = fuelStoragePrefab.GetEntity().prefabID;
		if (isServer)
		{
			foreach (BaseEntity child in children)
			{
				this.CheckNewChild(child);
			}
		}
	}

	// Token: 0x06002252 RID: 8786 RVA: 0x000DD9E8 File Offset: 0x000DBBE8
	public bool IsInFuelInteractionRange(BasePlayer player)
	{
		StorageContainer fuelContainer = this.GetFuelContainer();
		if (fuelContainer != null)
		{
			float num = 0f;
			if (this.isServer)
			{
				num = 3f;
			}
			return fuelContainer.Distance(player.eyes.position) <= num;
		}
		return false;
	}

	// Token: 0x06002253 RID: 8787 RVA: 0x000DDA34 File Offset: 0x000DBC34
	private StorageContainer GetFuelContainer()
	{
		StorageContainer storageContainer = this.fuelStorageInstance.Get(this.isServer);
		if (storageContainer.IsValid())
		{
			return storageContainer;
		}
		return null;
	}

	// Token: 0x06002254 RID: 8788 RVA: 0x000DDA5E File Offset: 0x000DBC5E
	public void CheckNewChild(BaseEntity child)
	{
		if (child.prefabID == this.fuelStorageID)
		{
			this.fuelStorageInstance.Set((StorageContainer)child);
		}
	}

	// Token: 0x06002255 RID: 8789 RVA: 0x000DDA80 File Offset: 0x000DBC80
	public Item GetFuelItem()
	{
		StorageContainer fuelContainer = this.GetFuelContainer();
		if (fuelContainer == null)
		{
			return null;
		}
		return fuelContainer.inventory.GetSlot(0);
	}

	// Token: 0x06002256 RID: 8790 RVA: 0x000DDAAC File Offset: 0x000DBCAC
	public int GetFuelAmount()
	{
		Item fuelItem = this.GetFuelItem();
		if (fuelItem == null || fuelItem.amount < 1)
		{
			return 0;
		}
		return fuelItem.amount;
	}

	// Token: 0x06002257 RID: 8791 RVA: 0x000DDAD4 File Offset: 0x000DBCD4
	public float GetFuelFraction()
	{
		Item fuelItem = this.GetFuelItem();
		if (fuelItem == null || fuelItem.amount < 1)
		{
			return 0f;
		}
		return Mathf.Clamp01((float)fuelItem.amount / (float)fuelItem.MaxStackable());
	}

	// Token: 0x06002258 RID: 8792 RVA: 0x000DDB10 File Offset: 0x000DBD10
	public bool HasFuel(bool forceCheck = false)
	{
		if (Time.time > this.nextFuelCheckTime || forceCheck)
		{
			this.cachedHasFuel = ((float)this.GetFuelAmount() > 0f);
			this.nextFuelCheckTime = Time.time + UnityEngine.Random.Range(1f, 2f);
		}
		return this.cachedHasFuel;
	}

	// Token: 0x06002259 RID: 8793 RVA: 0x000DDB64 File Offset: 0x000DBD64
	public int TryUseFuel(float seconds, float fuelUsedPerSecond)
	{
		StorageContainer fuelContainer = this.GetFuelContainer();
		if (fuelContainer == null)
		{
			return 0;
		}
		Item slot = fuelContainer.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		this.pendingFuel += seconds * fuelUsedPerSecond;
		if (this.pendingFuel >= 1f)
		{
			int num = Mathf.FloorToInt(this.pendingFuel);
			slot.UseItem(num);
			Analytics.Azure.AddPendingItems(((fuelContainer != null) ? fuelContainer.GetParentEntity() : null) ?? fuelContainer, slot.info.shortname, num, "fuel_system", true, false);
			this.pendingFuel -= (float)num;
			return num;
		}
		return 0;
	}

	// Token: 0x0600225A RID: 8794 RVA: 0x000DDC09 File Offset: 0x000DBE09
	public void LootFuel(BasePlayer player)
	{
		if (this.IsInFuelInteractionRange(player))
		{
			this.GetFuelContainer().PlayerOpenLoot(player, "", true);
		}
	}

	// Token: 0x0600225B RID: 8795 RVA: 0x000DDC28 File Offset: 0x000DBE28
	public void AddStartingFuel(float amount = -1f)
	{
		amount = ((amount == -1f) ? ((float)this.GetFuelContainer().allowedItem.stackable * 0.2f) : amount);
		this.GetFuelContainer().inventory.AddItem(this.GetFuelContainer().allowedItem, Mathf.FloorToInt(amount), 0UL, ItemContainer.LimitStack.Existing);
	}

	// Token: 0x0600225C RID: 8796 RVA: 0x000DDC7D File Offset: 0x000DBE7D
	public void AdminAddFuel()
	{
		this.GetFuelContainer().inventory.AddItem(this.GetFuelContainer().allowedItem, this.GetFuelContainer().allowedItem.stackable, 0UL, ItemContainer.LimitStack.Existing);
	}
}
