using System;
using System.Linq;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000B27 RID: 2855
	public class EngineStorage : StorageContainer
	{
		// Token: 0x04003DB9 RID: 15801
		[Header("Engine Storage")]
		public Sprite engineIcon;

		// Token: 0x04003DBA RID: 15802
		public float internalDamageMultiplier = 0.5f;

		// Token: 0x04003DBB RID: 15803
		public EngineStorage.EngineItemTypes[] slotTypes;

		// Token: 0x04003DBC RID: 15804
		[SerializeField]
		private VehicleModuleEngineItems allEngineItems;

		// Token: 0x04003DBD RID: 15805
		[SerializeField]
		[ReadOnly]
		private int accelerationBoostSlots;

		// Token: 0x04003DBE RID: 15806
		[SerializeField]
		[ReadOnly]
		private int topSpeedBoostSlots;

		// Token: 0x04003DBF RID: 15807
		[SerializeField]
		[ReadOnly]
		private int fuelEconomyBoostSlots;

		// Token: 0x17000642 RID: 1602
		// (get) Token: 0x0600452D RID: 17709 RVA: 0x0019521E File Offset: 0x0019341E
		// (set) Token: 0x0600452E RID: 17710 RVA: 0x00195226 File Offset: 0x00193426
		public bool isUsable { get; private set; }

		// Token: 0x17000643 RID: 1603
		// (get) Token: 0x0600452F RID: 17711 RVA: 0x0019522F File Offset: 0x0019342F
		// (set) Token: 0x06004530 RID: 17712 RVA: 0x00195237 File Offset: 0x00193437
		public float accelerationBoostPercent { get; private set; }

		// Token: 0x17000644 RID: 1604
		// (get) Token: 0x06004531 RID: 17713 RVA: 0x00195240 File Offset: 0x00193440
		// (set) Token: 0x06004532 RID: 17714 RVA: 0x00195248 File Offset: 0x00193448
		public float topSpeedBoostPercent { get; private set; }

		// Token: 0x17000645 RID: 1605
		// (get) Token: 0x06004533 RID: 17715 RVA: 0x00195251 File Offset: 0x00193451
		// (set) Token: 0x06004534 RID: 17716 RVA: 0x00195259 File Offset: 0x00193459
		public float fuelEconomyBoostPercent { get; private set; }

		// Token: 0x06004535 RID: 17717 RVA: 0x00195264 File Offset: 0x00193464
		public VehicleModuleEngine GetEngineModule()
		{
			global::BaseEntity parentEntity = base.GetParentEntity();
			if (parentEntity != null)
			{
				return parentEntity.GetComponent<VehicleModuleEngine>();
			}
			return null;
		}

		// Token: 0x06004536 RID: 17718 RVA: 0x00195289 File Offset: 0x00193489
		public float GetAveragedLoadoutPercent()
		{
			return (this.accelerationBoostPercent + this.topSpeedBoostPercent + this.fuelEconomyBoostPercent) / 3f;
		}

		// Token: 0x06004537 RID: 17719 RVA: 0x001952A8 File Offset: 0x001934A8
		public override void Load(global::BaseNetworkable.LoadInfo info)
		{
			base.Load(info);
			if (info.msg.engineStorage != null)
			{
				this.isUsable = info.msg.engineStorage.isUsable;
				this.accelerationBoostPercent = info.msg.engineStorage.accelerationBoost;
				this.topSpeedBoostPercent = info.msg.engineStorage.topSpeedBoost;
				this.fuelEconomyBoostPercent = info.msg.engineStorage.fuelEconomyBoost;
			}
			VehicleModuleEngine engineModule = this.GetEngineModule();
			if (engineModule == null)
			{
				return;
			}
			engineModule.RefreshPerformanceStats(this);
		}

		// Token: 0x06004538 RID: 17720 RVA: 0x00195334 File Offset: 0x00193534
		public override bool CanBeLooted(global::BasePlayer player)
		{
			VehicleModuleEngine engineModule = this.GetEngineModule();
			return engineModule != null && engineModule.CanBeLooted(player);
		}

		// Token: 0x06004539 RID: 17721 RVA: 0x0019535A File Offset: 0x0019355A
		public override int GetIdealSlot(global::BasePlayer player, global::Item item)
		{
			return this.GetValidSlot(item);
		}

		// Token: 0x0600453A RID: 17722 RVA: 0x00195364 File Offset: 0x00193564
		private int GetValidSlot(global::Item item)
		{
			ItemModEngineItem component = item.info.GetComponent<ItemModEngineItem>();
			if (component == null)
			{
				return -1;
			}
			EngineStorage.EngineItemTypes engineItemType = component.engineItemType;
			for (int i = 0; i < this.inventorySlots; i++)
			{
				if (engineItemType == this.slotTypes[i] && !base.inventory.SlotTaken(item, i))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x0600453B RID: 17723 RVA: 0x001953BD File Offset: 0x001935BD
		public override void OnInventoryFirstCreated(global::ItemContainer container)
		{
			this.RefreshLoadoutData();
		}

		// Token: 0x0600453C RID: 17724 RVA: 0x000063A5 File Offset: 0x000045A5
		public void NonUserSpawn()
		{
		}

		// Token: 0x0600453D RID: 17725 RVA: 0x001953BD File Offset: 0x001935BD
		public override void OnItemAddedOrRemoved(global::Item item, bool added)
		{
			this.RefreshLoadoutData();
		}

		// Token: 0x0600453E RID: 17726 RVA: 0x001953C8 File Offset: 0x001935C8
		public override bool ItemFilter(global::Item item, int targetSlot)
		{
			if (!base.ItemFilter(item, targetSlot))
			{
				return false;
			}
			if (targetSlot < 0 || targetSlot >= this.slotTypes.Length)
			{
				return false;
			}
			ItemModEngineItem component = item.info.GetComponent<ItemModEngineItem>();
			return component != null && component.engineItemType == this.slotTypes[targetSlot];
		}

		// Token: 0x0600453F RID: 17727 RVA: 0x0019541C File Offset: 0x0019361C
		public void RefreshLoadoutData()
		{
			bool isUsable;
			if (base.inventory.IsFull())
			{
				isUsable = base.inventory.itemList.All((global::Item item) => !item.isBroken);
			}
			else
			{
				isUsable = false;
			}
			this.isUsable = isUsable;
			this.accelerationBoostPercent = this.GetContainerItemsValueFor(new Func<EngineStorage.EngineItemTypes, bool>(EngineItemTypeEx.BoostsAcceleration)) / (float)this.accelerationBoostSlots;
			this.topSpeedBoostPercent = this.GetContainerItemsValueFor(new Func<EngineStorage.EngineItemTypes, bool>(EngineItemTypeEx.BoostsTopSpeed)) / (float)this.topSpeedBoostSlots;
			this.fuelEconomyBoostPercent = this.GetContainerItemsValueFor(new Func<EngineStorage.EngineItemTypes, bool>(EngineItemTypeEx.BoostsFuelEconomy)) / (float)this.fuelEconomyBoostSlots;
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			VehicleModuleEngine engineModule = this.GetEngineModule();
			if (engineModule == null)
			{
				return;
			}
			engineModule.RefreshPerformanceStats(this);
		}

		// Token: 0x06004540 RID: 17728 RVA: 0x001954E8 File Offset: 0x001936E8
		public override void Save(global::BaseNetworkable.SaveInfo info)
		{
			base.Save(info);
			info.msg.engineStorage = Pool.Get<EngineStorage>();
			info.msg.engineStorage.isUsable = this.isUsable;
			info.msg.engineStorage.accelerationBoost = this.accelerationBoostPercent;
			info.msg.engineStorage.topSpeedBoost = this.topSpeedBoostPercent;
			info.msg.engineStorage.fuelEconomyBoost = this.fuelEconomyBoostPercent;
		}

		// Token: 0x06004541 RID: 17729 RVA: 0x00195564 File Offset: 0x00193764
		public void OnModuleDamaged(float damageTaken)
		{
			if (damageTaken <= 0f)
			{
				return;
			}
			damageTaken *= this.internalDamageMultiplier;
			float[] array = new float[base.inventory.capacity];
			float num = 0f;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = UnityEngine.Random.value;
				num += array[i];
			}
			float num2 = damageTaken / num;
			for (int j = 0; j < array.Length; j++)
			{
				global::Item slot = base.inventory.GetSlot(j);
				if (slot != null)
				{
					slot.condition -= array[j] * num2;
				}
			}
			this.RefreshLoadoutData();
		}

		// Token: 0x06004542 RID: 17730 RVA: 0x001955FC File Offset: 0x001937FC
		public void AdminAddParts(int tier)
		{
			if (base.inventory == null)
			{
				Debug.LogWarning(base.GetType().Name + ": Null inventory on " + base.name);
				return;
			}
			for (int i = 0; i < base.inventory.capacity; i++)
			{
				global::Item slot = base.inventory.GetSlot(i);
				if (slot != null)
				{
					slot.RemoveFromContainer();
					slot.Remove(0f);
				}
			}
			for (int j = 0; j < base.inventory.capacity; j++)
			{
				ItemModEngineItem itemModEngineItem;
				if (base.inventory.GetSlot(j) == null && this.allEngineItems.TryGetItem(tier, this.slotTypes[j], out itemModEngineItem))
				{
					ItemDefinition component = itemModEngineItem.GetComponent<ItemDefinition>();
					global::Item item = ItemManager.Create(component, 1, 0UL);
					if (item != null)
					{
						item.condition = component.condition.max;
						item.MoveToContainer(base.inventory, j, false, false, null, true);
					}
					else
					{
						Debug.LogError(base.GetType().Name + ": Failed to create engine storage item.");
					}
				}
			}
		}

		// Token: 0x06004543 RID: 17731 RVA: 0x00195708 File Offset: 0x00193908
		private float GetContainerItemsValueFor(Func<EngineStorage.EngineItemTypes, bool> boostConditional)
		{
			float num = 0f;
			foreach (global::Item item in base.inventory.itemList)
			{
				ItemModEngineItem component = item.info.GetComponent<ItemModEngineItem>();
				if (component != null && boostConditional(component.engineItemType) && !item.isBroken)
				{
					num += (float)item.amount * this.GetTierValue(component.tier);
				}
			}
			return num;
		}

		// Token: 0x06004544 RID: 17732 RVA: 0x001957A4 File Offset: 0x001939A4
		private float GetTierValue(int tier)
		{
			switch (tier)
			{
			case 1:
				return 0.6f;
			case 2:
				return 0.8f;
			case 3:
				return 1f;
			default:
				Debug.LogError(base.GetType().Name + ": Unrecognised item tier: " + tier);
				return 0f;
			}
		}

		// Token: 0x02000F94 RID: 3988
		public enum EngineItemTypes
		{
			// Token: 0x0400504B RID: 20555
			Crankshaft,
			// Token: 0x0400504C RID: 20556
			Carburetor,
			// Token: 0x0400504D RID: 20557
			SparkPlug,
			// Token: 0x0400504E RID: 20558
			Piston,
			// Token: 0x0400504F RID: 20559
			Valve
		}
	}
}
