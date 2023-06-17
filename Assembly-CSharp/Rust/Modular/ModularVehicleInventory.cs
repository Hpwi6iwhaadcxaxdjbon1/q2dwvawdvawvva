using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000B29 RID: 2857
	public class ModularVehicleInventory : IDisposable
	{
		// Token: 0x04003DDB RID: 15835
		private readonly BaseModularVehicle vehicle;

		// Token: 0x17000646 RID: 1606
		// (get) Token: 0x06004547 RID: 17735 RVA: 0x0019587F File Offset: 0x00193A7F
		public ItemContainer ModuleContainer { get; }

		// Token: 0x17000647 RID: 1607
		// (get) Token: 0x06004548 RID: 17736 RVA: 0x00195887 File Offset: 0x00193A87
		public ItemContainer ChassisContainer { get; }

		// Token: 0x17000648 RID: 1608
		// (get) Token: 0x06004549 RID: 17737 RVA: 0x0019588F File Offset: 0x00193A8F
		public ItemContainerId UID
		{
			get
			{
				return this.ModuleContainer.uid;
			}
		}

		// Token: 0x17000649 RID: 1609
		// (get) Token: 0x0600454A RID: 17738 RVA: 0x0019589C File Offset: 0x00193A9C
		private int TotalSockets
		{
			get
			{
				return this.vehicle.TotalSockets;
			}
		}

		// Token: 0x0600454B RID: 17739 RVA: 0x001958AC File Offset: 0x00193AAC
		public ModularVehicleInventory(BaseModularVehicle vehicle, ItemDefinition chassisItemDef, bool giveUID)
		{
			this.vehicle = vehicle;
			this.ModuleContainer = this.CreateModuleInventory(vehicle, giveUID);
			this.ChassisContainer = this.CreateChassisInventory(vehicle, giveUID);
			vehicle.AssociatedItemInstance = ItemManager.Create(chassisItemDef, 1, 0UL);
			if (!Application.isLoadingSave)
			{
				vehicle.AssociatedItemInstance.MoveToContainer(this.ChassisContainer, 0, false, false, null, true);
			}
		}

		// Token: 0x0600454C RID: 17740 RVA: 0x00195910 File Offset: 0x00193B10
		public void Dispose()
		{
			foreach (Item item in this.ModuleContainer.itemList)
			{
				item.OnDirty -= this.OnModuleItemChanged;
			}
		}

		// Token: 0x0600454D RID: 17741 RVA: 0x00195974 File Offset: 0x00193B74
		public void GiveUIDs()
		{
			this.ModuleContainer.GiveUID();
			this.ChassisContainer.GiveUID();
		}

		// Token: 0x0600454E RID: 17742 RVA: 0x0019598C File Offset: 0x00193B8C
		public bool SocketIsFree(int socketIndex, Item moduleItem = null)
		{
			Item item = null;
			int num = socketIndex;
			while (item == null && num >= 0)
			{
				item = this.ModuleContainer.GetSlot(num);
				if (item != null)
				{
					if (item == moduleItem)
					{
						return true;
					}
					ItemModVehicleModule component = item.info.GetComponent<ItemModVehicleModule>();
					return num + component.socketsTaken - 1 < socketIndex;
				}
				else
				{
					num--;
				}
			}
			return true;
		}

		// Token: 0x0600454F RID: 17743 RVA: 0x001959DB File Offset: 0x00193BDB
		public bool SocketIsTaken(int socketIndex)
		{
			return !this.SocketIsFree(socketIndex, null);
		}

		// Token: 0x06004550 RID: 17744 RVA: 0x001959E8 File Offset: 0x00193BE8
		public bool TryAddModuleItem(Item moduleItem, int socketIndex)
		{
			if (moduleItem == null)
			{
				Debug.LogError(base.GetType().Name + ": Can't add null item.");
				return false;
			}
			return moduleItem.MoveToContainer(this.ModuleContainer, socketIndex, false, false, null, true);
		}

		// Token: 0x06004551 RID: 17745 RVA: 0x00195A1A File Offset: 0x00193C1A
		public bool RemoveAndDestroy(Item itemToRemove)
		{
			bool result = this.ModuleContainer.Remove(itemToRemove);
			itemToRemove.Remove(0f);
			return result;
		}

		// Token: 0x06004552 RID: 17746 RVA: 0x00195A33 File Offset: 0x00193C33
		public int TryGetFreeSocket(int socketsTaken)
		{
			return this.TryGetFreeSocket(null, socketsTaken);
		}

		// Token: 0x06004553 RID: 17747 RVA: 0x00195A40 File Offset: 0x00193C40
		public int TryGetFreeSocket(Item moduleItem, int socketsTaken)
		{
			for (int i = 0; i <= this.TotalSockets - socketsTaken; i++)
			{
				if (this.SocketsAreFree(i, socketsTaken, moduleItem))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06004554 RID: 17748 RVA: 0x00195A70 File Offset: 0x00193C70
		public bool SocketsAreFree(int firstIndex, int socketsTaken, Item moduleItem = null)
		{
			if (firstIndex < 0 || firstIndex + socketsTaken > this.TotalSockets)
			{
				return false;
			}
			for (int i = firstIndex; i < firstIndex + socketsTaken; i++)
			{
				if (!this.SocketIsFree(i, moduleItem))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004555 RID: 17749 RVA: 0x00195AAC File Offset: 0x00193CAC
		public bool TrySyncModuleInventory(BaseVehicleModule moduleEntity, int firstSocketIndex)
		{
			if (firstSocketIndex < 0)
			{
				Debug.LogError(string.Format("{0}: Invalid socket index ({1}) for new module entity.", base.GetType().Name, firstSocketIndex), this.vehicle.gameObject);
				return false;
			}
			Item slot = this.ModuleContainer.GetSlot(firstSocketIndex);
			int numSocketsTaken = moduleEntity.GetNumSocketsTaken();
			if (!this.SocketsAreFree(firstSocketIndex, numSocketsTaken, null) && (slot == null || moduleEntity.AssociatedItemInstance != slot))
			{
				Debug.LogError(string.Format("{0}: Sockets are not free for new module entity. First: {1} Taken: {2}", base.GetType().Name, firstSocketIndex, numSocketsTaken), this.vehicle.gameObject);
				return false;
			}
			if (slot != null)
			{
				return true;
			}
			Item item = ItemManager.Create(moduleEntity.AssociatedItemDef, 1, 0UL);
			item.condition = moduleEntity.health;
			moduleEntity.AssociatedItemInstance = item;
			bool flag = this.TryAddModuleItem(item, firstSocketIndex);
			if (flag)
			{
				this.vehicle.SetUpModule(moduleEntity, item);
				return flag;
			}
			item.Remove(0f);
			return flag;
		}

		// Token: 0x06004556 RID: 17750 RVA: 0x00195B93 File Offset: 0x00193D93
		private bool SocketIsUsed(Item item, int slotIndex)
		{
			return !this.SocketIsFree(slotIndex, item);
		}

		// Token: 0x06004557 RID: 17751 RVA: 0x00195BA0 File Offset: 0x00193DA0
		private ItemContainer CreateModuleInventory(BaseModularVehicle vehicle, bool giveUID)
		{
			if (this.ModuleContainer != null)
			{
				return this.ModuleContainer;
			}
			ItemContainer itemContainer = new ItemContainer
			{
				entityOwner = vehicle,
				allowedContents = ItemContainer.ContentsType.Generic,
				maxStackSize = 1
			};
			itemContainer.ServerInitialize(null, this.TotalSockets);
			if (giveUID)
			{
				itemContainer.GiveUID();
			}
			itemContainer.onItemAddedRemoved = new Action<Item, bool>(this.OnSocketInventoryAddRemove);
			itemContainer.canAcceptItem = new Func<Item, int, bool>(this.ItemFilter);
			itemContainer.slotIsReserved = new Func<Item, int, bool>(this.SocketIsUsed);
			return itemContainer;
		}

		// Token: 0x06004558 RID: 17752 RVA: 0x00195C24 File Offset: 0x00193E24
		private ItemContainer CreateChassisInventory(BaseModularVehicle vehicle, bool giveUID)
		{
			if (this.ChassisContainer != null)
			{
				return this.ChassisContainer;
			}
			ItemContainer itemContainer = new ItemContainer
			{
				entityOwner = vehicle,
				allowedContents = ItemContainer.ContentsType.Generic,
				maxStackSize = 1
			};
			itemContainer.ServerInitialize(null, 1);
			if (giveUID)
			{
				itemContainer.GiveUID();
			}
			return itemContainer;
		}

		// Token: 0x06004559 RID: 17753 RVA: 0x00195C6D File Offset: 0x00193E6D
		private void OnSocketInventoryAddRemove(Item moduleItem, bool added)
		{
			if (added)
			{
				this.ModuleItemAdded(moduleItem, moduleItem.position);
				return;
			}
			this.ModuleItemRemoved(moduleItem);
		}

		// Token: 0x0600455A RID: 17754 RVA: 0x00195C88 File Offset: 0x00193E88
		private void ModuleItemAdded(Item moduleItem, int socketIndex)
		{
			ItemModVehicleModule component = moduleItem.info.GetComponent<ItemModVehicleModule>();
			if (!Application.isLoadingSave && this.vehicle.GetModuleForItem(moduleItem) == null)
			{
				this.vehicle.CreatePhysicalModuleEntity(moduleItem, component, socketIndex);
			}
			moduleItem.OnDirty += this.OnModuleItemChanged;
		}

		// Token: 0x0600455B RID: 17755 RVA: 0x00195CE0 File Offset: 0x00193EE0
		private void ModuleItemRemoved(Item moduleItem)
		{
			if (moduleItem == null)
			{
				Debug.LogError("Null module item removed.", this.vehicle.gameObject);
				return;
			}
			moduleItem.OnDirty -= this.OnModuleItemChanged;
			BaseVehicleModule moduleForItem = this.vehicle.GetModuleForItem(moduleItem);
			if (moduleForItem != null)
			{
				if (!moduleForItem.IsFullySpawned())
				{
					Debug.LogError("Module entity being removed before it's fully spawned. This could cause errors.", this.vehicle.gameObject);
				}
				moduleForItem.Kill(BaseNetworkable.DestroyMode.None);
				return;
			}
			Debug.Log("Couldn't find entity for this item.");
		}

		// Token: 0x0600455C RID: 17756 RVA: 0x00195D60 File Offset: 0x00193F60
		private void OnModuleItemChanged(Item moduleItem)
		{
			BaseVehicleModule moduleForItem = this.vehicle.GetModuleForItem(moduleItem);
			if (moduleForItem != null)
			{
				moduleForItem.SetHealth(moduleItem.condition);
				if (moduleForItem.FirstSocketIndex != moduleItem.position)
				{
					this.ModuleItemRemoved(moduleItem);
					this.ModuleItemAdded(moduleItem, moduleItem.position);
				}
			}
		}

		// Token: 0x0600455D RID: 17757 RVA: 0x00195DB4 File Offset: 0x00193FB4
		private bool ItemFilter(Item item, int targetSlot)
		{
			string text;
			return this.vehicle.ModuleCanBeAdded(item, targetSlot, out text);
		}
	}
}
