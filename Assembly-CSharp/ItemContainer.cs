using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020005C7 RID: 1479
public sealed class ItemContainer
{
	// Token: 0x04002428 RID: 9256
	public global::ItemContainer.Flag flags;

	// Token: 0x04002429 RID: 9257
	public global::ItemContainer.ContentsType allowedContents;

	// Token: 0x0400242A RID: 9258
	public ItemDefinition[] onlyAllowedItems;

	// Token: 0x0400242B RID: 9259
	public List<ItemSlot> availableSlots = new List<ItemSlot>();

	// Token: 0x0400242C RID: 9260
	public int capacity = 2;

	// Token: 0x0400242D RID: 9261
	public ItemContainerId uid;

	// Token: 0x0400242E RID: 9262
	public bool dirty;

	// Token: 0x0400242F RID: 9263
	public List<global::Item> itemList = new List<global::Item>();

	// Token: 0x04002430 RID: 9264
	public float temperature = 15f;

	// Token: 0x04002431 RID: 9265
	public global::Item parent;

	// Token: 0x04002432 RID: 9266
	public global::BasePlayer playerOwner;

	// Token: 0x04002433 RID: 9267
	public global::BaseEntity entityOwner;

	// Token: 0x04002434 RID: 9268
	public bool isServer;

	// Token: 0x04002435 RID: 9269
	public int maxStackSize;

	// Token: 0x04002437 RID: 9271
	public Func<global::Item, int, bool> canAcceptItem;

	// Token: 0x04002438 RID: 9272
	public Func<global::Item, int, bool> slotIsReserved;

	// Token: 0x04002439 RID: 9273
	public Action<global::Item, bool> onItemAddedRemoved;

	// Token: 0x0400243A RID: 9274
	public Action<global::Item> onPreItemRemove;

	// Token: 0x06002C95 RID: 11413 RVA: 0x0010E223 File Offset: 0x0010C423
	public bool HasFlag(global::ItemContainer.Flag f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x06002C96 RID: 11414 RVA: 0x0010E230 File Offset: 0x0010C430
	public void SetFlag(global::ItemContainer.Flag f, bool b)
	{
		if (b)
		{
			this.flags |= f;
			return;
		}
		this.flags &= ~f;
	}

	// Token: 0x06002C97 RID: 11415 RVA: 0x0010E253 File Offset: 0x0010C453
	public bool IsLocked()
	{
		return this.HasFlag(global::ItemContainer.Flag.IsLocked);
	}

	// Token: 0x06002C98 RID: 11416 RVA: 0x0010E25D File Offset: 0x0010C45D
	public bool PlayerItemInputBlocked()
	{
		return this.HasFlag(global::ItemContainer.Flag.NoItemInput);
	}

	// Token: 0x170003AB RID: 939
	// (get) Token: 0x06002C99 RID: 11417 RVA: 0x0010E26A File Offset: 0x0010C46A
	public bool HasLimitedAllowedItems
	{
		get
		{
			return this.onlyAllowedItems != null && this.onlyAllowedItems.Length != 0;
		}
	}

	// Token: 0x14000004 RID: 4
	// (add) Token: 0x06002C9A RID: 11418 RVA: 0x0010E280 File Offset: 0x0010C480
	// (remove) Token: 0x06002C9B RID: 11419 RVA: 0x0010E2B8 File Offset: 0x0010C4B8
	public event Action onDirty;

	// Token: 0x06002C9C RID: 11420 RVA: 0x0010E2F0 File Offset: 0x0010C4F0
	public float GetTemperature(int slot)
	{
		global::BaseOven baseOven;
		if ((baseOven = (this.entityOwner as global::BaseOven)) != null)
		{
			return baseOven.GetTemperature(slot);
		}
		return this.temperature;
	}

	// Token: 0x06002C9D RID: 11421 RVA: 0x0010E31A File Offset: 0x0010C51A
	public void ServerInitialize(global::Item parentItem, int iMaxCapacity)
	{
		this.parent = parentItem;
		this.capacity = iMaxCapacity;
		this.uid = default(ItemContainerId);
		this.isServer = true;
		if (this.allowedContents == (global::ItemContainer.ContentsType)0)
		{
			this.allowedContents = global::ItemContainer.ContentsType.Generic;
		}
		this.MarkDirty();
	}

	// Token: 0x06002C9E RID: 11422 RVA: 0x0010E352 File Offset: 0x0010C552
	public void GiveUID()
	{
		Assert.IsTrue(!this.uid.IsValid, "Calling GiveUID - but already has a uid!");
		this.uid = new ItemContainerId(Net.sv.TakeUID());
	}

	// Token: 0x06002C9F RID: 11423 RVA: 0x0010E381 File Offset: 0x0010C581
	public void MarkDirty()
	{
		this.dirty = true;
		if (this.parent != null)
		{
			this.parent.MarkDirty();
		}
		if (this.onDirty != null)
		{
			this.onDirty();
		}
	}

	// Token: 0x06002CA0 RID: 11424 RVA: 0x0010E3B0 File Offset: 0x0010C5B0
	public DroppedItemContainer Drop(string prefab, Vector3 pos, Quaternion rot, float destroyPercent)
	{
		if (this.itemList == null || this.itemList.Count == 0)
		{
			return null;
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(prefab, pos, rot, true);
		if (baseEntity == null)
		{
			return null;
		}
		DroppedItemContainer droppedItemContainer = baseEntity as DroppedItemContainer;
		if (droppedItemContainer != null)
		{
			droppedItemContainer.TakeFrom(new global::ItemContainer[]
			{
				this
			}, destroyPercent);
		}
		droppedItemContainer.Spawn();
		return droppedItemContainer;
	}

	// Token: 0x06002CA1 RID: 11425 RVA: 0x0010E41C File Offset: 0x0010C61C
	public static DroppedItemContainer Drop(string prefab, Vector3 pos, Quaternion rot, params global::ItemContainer[] containers)
	{
		int num = 0;
		foreach (global::ItemContainer itemContainer in containers)
		{
			num += ((itemContainer.itemList != null) ? itemContainer.itemList.Count : 0);
		}
		if (num == 0)
		{
			return null;
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(prefab, pos, rot, true);
		if (baseEntity == null)
		{
			return null;
		}
		DroppedItemContainer droppedItemContainer = baseEntity as DroppedItemContainer;
		if (droppedItemContainer != null)
		{
			droppedItemContainer.TakeFrom(containers, 0f);
		}
		droppedItemContainer.Spawn();
		return droppedItemContainer;
	}

	// Token: 0x06002CA2 RID: 11426 RVA: 0x0010E49C File Offset: 0x0010C69C
	public global::BaseEntity GetEntityOwner(bool returnHeldEntity = false)
	{
		global::ItemContainer itemContainer = this;
		for (int i = 0; i < 10; i++)
		{
			if (itemContainer.entityOwner != null)
			{
				return itemContainer.entityOwner;
			}
			if (itemContainer.playerOwner != null)
			{
				return itemContainer.playerOwner;
			}
			if (returnHeldEntity)
			{
				global::Item item = itemContainer.parent;
				global::BaseEntity baseEntity = (item != null) ? item.GetHeldEntity() : null;
				if (baseEntity != null)
				{
					return baseEntity;
				}
			}
			global::Item item2 = itemContainer.parent;
			global::ItemContainer itemContainer2 = (item2 != null) ? item2.parent : null;
			if (itemContainer2 == null || itemContainer2 == itemContainer)
			{
				return null;
			}
			itemContainer = itemContainer2;
		}
		return null;
	}

	// Token: 0x06002CA3 RID: 11427 RVA: 0x0010E524 File Offset: 0x0010C724
	public void OnChanged()
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			this.itemList[i].OnChanged();
		}
	}

	// Token: 0x06002CA4 RID: 11428 RVA: 0x0010E558 File Offset: 0x0010C758
	public global::Item FindItemByUID(ItemId iUID)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			global::Item item = this.itemList[i];
			if (item.IsValid())
			{
				global::Item item2 = item.FindItem(iUID);
				if (item2 != null)
				{
					return item2;
				}
			}
		}
		return null;
	}

	// Token: 0x06002CA5 RID: 11429 RVA: 0x0010E59E File Offset: 0x0010C79E
	public bool IsFull()
	{
		return this.itemList.Count >= this.capacity;
	}

	// Token: 0x06002CA6 RID: 11430 RVA: 0x0010E5B6 File Offset: 0x0010C7B6
	public bool IsEmpty()
	{
		return this.itemList.Count == 0;
	}

	// Token: 0x06002CA7 RID: 11431 RVA: 0x0010E5C6 File Offset: 0x0010C7C6
	public bool CanAccept(global::Item item)
	{
		return !this.IsFull();
	}

	// Token: 0x06002CA8 RID: 11432 RVA: 0x0010E5D4 File Offset: 0x0010C7D4
	public int GetMaxTransferAmount(ItemDefinition def)
	{
		int num = this.ContainerMaxStackSize();
		foreach (global::Item item in this.itemList)
		{
			if (item.info == def)
			{
				num -= item.amount;
				if (num <= 0)
				{
					return 0;
				}
			}
		}
		return num;
	}

	// Token: 0x06002CA9 RID: 11433 RVA: 0x0010E64C File Offset: 0x0010C84C
	public void SetOnlyAllowedItem(ItemDefinition def)
	{
		this.SetOnlyAllowedItems(new ItemDefinition[]
		{
			def
		});
	}

	// Token: 0x06002CAA RID: 11434 RVA: 0x0010E660 File Offset: 0x0010C860
	public void SetOnlyAllowedItems(params ItemDefinition[] defs)
	{
		int num = 0;
		for (int i = 0; i < defs.Length; i++)
		{
			if (defs[i] != null)
			{
				num++;
			}
		}
		this.onlyAllowedItems = new ItemDefinition[num];
		int num2 = 0;
		foreach (ItemDefinition itemDefinition in defs)
		{
			if (itemDefinition != null)
			{
				this.onlyAllowedItems[num2] = itemDefinition;
				num2++;
			}
		}
	}

	// Token: 0x06002CAB RID: 11435 RVA: 0x0010E6CC File Offset: 0x0010C8CC
	internal bool Insert(global::Item item)
	{
		if (this.itemList.Contains(item))
		{
			return false;
		}
		if (this.IsFull())
		{
			return false;
		}
		this.itemList.Add(item);
		item.parent = this;
		if (!this.FindPosition(item))
		{
			return false;
		}
		this.MarkDirty();
		if (this.onItemAddedRemoved != null)
		{
			this.onItemAddedRemoved(item, true);
		}
		return true;
	}

	// Token: 0x06002CAC RID: 11436 RVA: 0x0010E72D File Offset: 0x0010C92D
	public bool SlotTaken(global::Item item, int i)
	{
		return (this.slotIsReserved != null && this.slotIsReserved(item, i)) || this.GetSlot(i) != null;
	}

	// Token: 0x06002CAD RID: 11437 RVA: 0x0010E754 File Offset: 0x0010C954
	public global::Item GetSlot(int slot)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].position == slot)
			{
				return this.itemList[i];
			}
		}
		return null;
	}

	// Token: 0x06002CAE RID: 11438 RVA: 0x0010E79C File Offset: 0x0010C99C
	public global::Item GetNonFullStackWithinRange(global::Item def, Vector2i range)
	{
		int count = this.itemList.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.itemList[i].amount < this.itemList[i].info.stackable && this.itemList[i].position >= range.x && this.itemList[i].position <= range.y)
			{
				if (def.IsBlueprint())
				{
					if (this.itemList[i].blueprintTarget != def.blueprintTarget)
					{
						goto IL_BF;
					}
				}
				else if (this.itemList[i].info != def.info)
				{
					goto IL_BF;
				}
				return this.itemList[i];
			}
			IL_BF:;
		}
		return null;
	}

	// Token: 0x06002CAF RID: 11439 RVA: 0x0010E874 File Offset: 0x0010CA74
	internal bool FindPosition(global::Item item)
	{
		int position = item.position;
		item.position = -1;
		if (position >= 0 && !this.SlotTaken(item, position))
		{
			item.position = position;
			return true;
		}
		for (int i = 0; i < this.capacity; i++)
		{
			if (!this.SlotTaken(item, i))
			{
				item.position = i;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002CB0 RID: 11440 RVA: 0x0010E8CB File Offset: 0x0010CACB
	public void SetLocked(bool isLocked)
	{
		this.SetFlag(global::ItemContainer.Flag.IsLocked, isLocked);
		this.MarkDirty();
	}

	// Token: 0x06002CB1 RID: 11441 RVA: 0x0010E8DC File Offset: 0x0010CADC
	internal bool Remove(global::Item item)
	{
		if (!this.itemList.Contains(item))
		{
			return false;
		}
		if (this.onPreItemRemove != null)
		{
			this.onPreItemRemove(item);
		}
		this.itemList.Remove(item);
		item.parent = null;
		this.MarkDirty();
		if (this.onItemAddedRemoved != null)
		{
			this.onItemAddedRemoved(item, false);
		}
		return true;
	}

	// Token: 0x06002CB2 RID: 11442 RVA: 0x0010E940 File Offset: 0x0010CB40
	internal void Clear()
	{
		global::Item[] array = this.itemList.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Remove(0f);
		}
	}

	// Token: 0x06002CB3 RID: 11443 RVA: 0x0010E974 File Offset: 0x0010CB74
	public void Kill()
	{
		this.onDirty = null;
		this.canAcceptItem = null;
		this.slotIsReserved = null;
		this.onItemAddedRemoved = null;
		if (Net.sv != null)
		{
			Net.sv.ReturnUID(this.uid.Value);
			this.uid = default(ItemContainerId);
		}
		List<global::Item> list = Pool.GetList<global::Item>();
		foreach (global::Item item in this.itemList)
		{
			list.Add(item);
		}
		foreach (global::Item item2 in list)
		{
			item2.Remove(0f);
		}
		Pool.FreeList<global::Item>(ref list);
		this.itemList.Clear();
	}

	// Token: 0x06002CB4 RID: 11444 RVA: 0x0010EA64 File Offset: 0x0010CC64
	public int GetAmount(int itemid, bool onlyUsableAmounts)
	{
		int num = 0;
		foreach (global::Item item in this.itemList)
		{
			if (item.info.itemid == itemid && (!onlyUsableAmounts || !item.IsBusy()))
			{
				num += item.amount;
			}
		}
		return num;
	}

	// Token: 0x06002CB5 RID: 11445 RVA: 0x0010EAD8 File Offset: 0x0010CCD8
	public global::Item FindItemByItemID(int itemid)
	{
		foreach (global::Item item in this.itemList)
		{
			if (item.info.itemid == itemid)
			{
				return item;
			}
		}
		return null;
	}

	// Token: 0x06002CB6 RID: 11446 RVA: 0x0010EB3C File Offset: 0x0010CD3C
	public global::Item FindItemsByItemName(string name)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(name);
		if (itemDefinition == null)
		{
			return null;
		}
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].info == itemDefinition)
			{
				return this.itemList[i];
			}
		}
		return null;
	}

	// Token: 0x06002CB7 RID: 11447 RVA: 0x0010EB98 File Offset: 0x0010CD98
	public global::Item FindBySubEntityID(NetworkableId subEntityID)
	{
		if (!subEntityID.IsValid)
		{
			return null;
		}
		foreach (global::Item item in this.itemList)
		{
			if (item.instanceData != null && item.instanceData.subEntity == subEntityID)
			{
				return item;
			}
		}
		return null;
	}

	// Token: 0x06002CB8 RID: 11448 RVA: 0x0010EC14 File Offset: 0x0010CE14
	public List<global::Item> FindItemsByItemID(int itemid)
	{
		return this.itemList.FindAll((global::Item x) => x.info.itemid == itemid);
	}

	// Token: 0x06002CB9 RID: 11449 RVA: 0x0010EC48 File Offset: 0x0010CE48
	public ProtoBuf.ItemContainer Save()
	{
		ProtoBuf.ItemContainer itemContainer = Pool.Get<ProtoBuf.ItemContainer>();
		itemContainer.contents = Pool.GetList<ProtoBuf.Item>();
		itemContainer.UID = this.uid;
		itemContainer.slots = this.capacity;
		itemContainer.temperature = this.temperature;
		itemContainer.allowedContents = (int)this.allowedContents;
		if (this.HasLimitedAllowedItems)
		{
			itemContainer.allowedItems = Pool.GetList<int>();
			for (int i = 0; i < this.onlyAllowedItems.Length; i++)
			{
				if (this.onlyAllowedItems[i] != null)
				{
					itemContainer.allowedItems.Add(this.onlyAllowedItems[i].itemid);
				}
			}
		}
		itemContainer.flags = (int)this.flags;
		itemContainer.maxStackSize = this.maxStackSize;
		if (this.availableSlots != null && this.availableSlots.Count > 0)
		{
			itemContainer.availableSlots = Pool.GetList<int>();
			for (int j = 0; j < this.availableSlots.Count; j++)
			{
				itemContainer.availableSlots.Add((int)this.availableSlots[j]);
			}
		}
		for (int k = 0; k < this.itemList.Count; k++)
		{
			global::Item item = this.itemList[k];
			if (item.IsValid())
			{
				itemContainer.contents.Add(item.Save(true, true));
			}
		}
		return itemContainer;
	}

	// Token: 0x06002CBA RID: 11450 RVA: 0x0010ED8C File Offset: 0x0010CF8C
	public void Load(ProtoBuf.ItemContainer container)
	{
		using (TimeWarning.New("ItemContainer.Load", 0))
		{
			this.uid = container.UID;
			this.capacity = container.slots;
			List<global::Item> list = this.itemList;
			this.itemList = Pool.GetList<global::Item>();
			this.temperature = container.temperature;
			this.flags = (global::ItemContainer.Flag)container.flags;
			this.allowedContents = (global::ItemContainer.ContentsType)((container.allowedContents == 0) ? 1 : container.allowedContents);
			if (container.allowedItems != null && container.allowedItems.Count > 0)
			{
				this.onlyAllowedItems = new ItemDefinition[container.allowedItems.Count];
				for (int i = 0; i < container.allowedItems.Count; i++)
				{
					this.onlyAllowedItems[i] = ItemManager.FindItemDefinition(container.allowedItems[i]);
				}
			}
			else
			{
				this.onlyAllowedItems = null;
			}
			this.maxStackSize = container.maxStackSize;
			this.availableSlots.Clear();
			for (int j = 0; j < container.availableSlots.Count; j++)
			{
				this.availableSlots.Add((ItemSlot)container.availableSlots[j]);
			}
			using (TimeWarning.New("container.contents", 0))
			{
				foreach (ProtoBuf.Item item in container.contents)
				{
					global::Item item2 = null;
					foreach (global::Item item3 in list)
					{
						if (item3.uid == item.UID)
						{
							item2 = item3;
							break;
						}
					}
					item2 = ItemManager.Load(item, item2, this.isServer);
					if (item2 != null)
					{
						item2.parent = this;
						item2.position = item.slot;
						this.Insert(item2);
					}
				}
			}
			using (TimeWarning.New("Delete old items", 0))
			{
				foreach (global::Item item4 in list)
				{
					if (!this.itemList.Contains(item4))
					{
						item4.Remove(0f);
					}
				}
			}
			this.dirty = true;
			Pool.FreeList<global::Item>(ref list);
		}
	}

	// Token: 0x06002CBB RID: 11451 RVA: 0x0010F084 File Offset: 0x0010D284
	public global::BasePlayer GetOwnerPlayer()
	{
		return this.playerOwner;
	}

	// Token: 0x06002CBC RID: 11452 RVA: 0x0010F08C File Offset: 0x0010D28C
	public int ContainerMaxStackSize()
	{
		if (this.maxStackSize <= 0)
		{
			return int.MaxValue;
		}
		return this.maxStackSize;
	}

	// Token: 0x06002CBD RID: 11453 RVA: 0x0010F0A4 File Offset: 0x0010D2A4
	public int Take(List<global::Item> collect, int itemid, int iAmount)
	{
		int num = 0;
		if (iAmount == 0)
		{
			return num;
		}
		List<global::Item> list = Pool.GetList<global::Item>();
		foreach (global::Item item in this.itemList)
		{
			if (item.info.itemid == itemid)
			{
				int num2 = iAmount - num;
				if (num2 > 0)
				{
					if (item.amount > num2)
					{
						item.MarkDirty();
						item.amount -= num2;
						num += num2;
						global::Item item2 = ItemManager.CreateByItemID(itemid, 1, 0UL);
						item2.amount = num2;
						item2.CollectedForCrafting(this.playerOwner);
						if (collect != null)
						{
							collect.Add(item2);
							break;
						}
						break;
					}
					else
					{
						if (item.amount <= num2)
						{
							num += item.amount;
							list.Add(item);
							if (collect != null)
							{
								collect.Add(item);
							}
						}
						if (num == iAmount)
						{
							break;
						}
					}
				}
			}
		}
		foreach (global::Item item3 in list)
		{
			item3.RemoveFromContainer();
		}
		Pool.FreeList<global::Item>(ref list);
		return num;
	}

	// Token: 0x170003AC RID: 940
	// (get) Token: 0x06002CBE RID: 11454 RVA: 0x0010F1DC File Offset: 0x0010D3DC
	public Vector3 dropPosition
	{
		get
		{
			if (this.playerOwner)
			{
				return this.playerOwner.GetDropPosition();
			}
			if (this.entityOwner)
			{
				return this.entityOwner.GetDropPosition();
			}
			if (this.parent != null)
			{
				global::BaseEntity worldEntity = this.parent.GetWorldEntity();
				if (worldEntity != null)
				{
					return worldEntity.GetDropPosition();
				}
			}
			Debug.LogWarning("ItemContainer.dropPosition dropped through");
			return Vector3.zero;
		}
	}

	// Token: 0x170003AD RID: 941
	// (get) Token: 0x06002CBF RID: 11455 RVA: 0x0010F250 File Offset: 0x0010D450
	public Vector3 dropVelocity
	{
		get
		{
			if (this.playerOwner)
			{
				return this.playerOwner.GetDropVelocity();
			}
			if (this.entityOwner)
			{
				return this.entityOwner.GetDropVelocity();
			}
			if (this.parent != null)
			{
				global::BaseEntity worldEntity = this.parent.GetWorldEntity();
				if (worldEntity != null)
				{
					return worldEntity.GetDropVelocity();
				}
			}
			Debug.LogWarning("ItemContainer.dropVelocity dropped through");
			return Vector3.zero;
		}
	}

	// Token: 0x06002CC0 RID: 11456 RVA: 0x0010F2C4 File Offset: 0x0010D4C4
	public void OnCycle(float delta)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].IsValid())
			{
				this.itemList[i].OnCycle(delta);
			}
		}
	}

	// Token: 0x06002CC1 RID: 11457 RVA: 0x0010F30C File Offset: 0x0010D50C
	public void FindAmmo(List<global::Item> list, AmmoTypes ammoType)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			this.itemList[i].FindAmmo(list, ammoType);
		}
	}

	// Token: 0x06002CC2 RID: 11458 RVA: 0x0010F344 File Offset: 0x0010D544
	public bool HasAmmo(AmmoTypes ammoType)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].HasAmmo(ammoType))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002CC3 RID: 11459 RVA: 0x0010F380 File Offset: 0x0010D580
	public int GetAmmoAmount(AmmoTypes ammoType)
	{
		int num = 0;
		for (int i = 0; i < this.itemList.Count; i++)
		{
			num += this.itemList[i].GetAmmoAmount(ammoType);
		}
		return num;
	}

	// Token: 0x06002CC4 RID: 11460 RVA: 0x0010F3BC File Offset: 0x0010D5BC
	public int TotalItemAmount()
	{
		int num = 0;
		for (int i = 0; i < this.itemList.Count; i++)
		{
			num += this.itemList[i].amount;
		}
		return num;
	}

	// Token: 0x06002CC5 RID: 11461 RVA: 0x0010F3F8 File Offset: 0x0010D5F8
	public int GetTotalItemAmount(global::Item item, int slotStartInclusive, int slotEndInclusive)
	{
		int num = 0;
		for (int i = slotStartInclusive; i <= slotEndInclusive; i++)
		{
			global::Item slot = this.GetSlot(i);
			if (slot != null)
			{
				if (item.IsBlueprint())
				{
					if (slot.IsBlueprint() && slot.blueprintTarget == item.blueprintTarget)
					{
						num += slot.amount;
					}
				}
				else if (slot.info == item.info || slot.info.isRedirectOf == item.info || item.info.isRedirectOf == slot.info)
				{
					num += slot.amount;
				}
			}
		}
		return num;
	}

	// Token: 0x06002CC6 RID: 11462 RVA: 0x0010F49C File Offset: 0x0010D69C
	public int GetTotalCategoryAmount(ItemCategory category, int slotStartInclusive, int slotEndInclusive)
	{
		int num = 0;
		for (int i = slotStartInclusive; i <= slotEndInclusive; i++)
		{
			global::Item slot = this.GetSlot(i);
			if (slot != null && slot.info.category == category)
			{
				num += slot.amount;
			}
		}
		return num;
	}

	// Token: 0x06002CC7 RID: 11463 RVA: 0x0010F4DC File Offset: 0x0010D6DC
	public void AddItem(ItemDefinition itemToCreate, int amount, ulong skin = 0UL, global::ItemContainer.LimitStack limitStack = global::ItemContainer.LimitStack.Existing)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (amount == 0)
			{
				return;
			}
			if (!(this.itemList[i].info != itemToCreate))
			{
				int num = this.itemList[i].MaxStackable();
				if (num > this.itemList[i].amount || limitStack == global::ItemContainer.LimitStack.None)
				{
					this.MarkDirty();
					this.itemList[i].amount += amount;
					amount -= amount;
					if (this.itemList[i].amount > num && limitStack != global::ItemContainer.LimitStack.None)
					{
						amount = this.itemList[i].amount - num;
						if (amount > 0)
						{
							this.itemList[i].amount -= amount;
						}
					}
				}
			}
		}
		if (amount == 0)
		{
			return;
		}
		int num2 = (limitStack == global::ItemContainer.LimitStack.All) ? Mathf.Min(itemToCreate.stackable, this.ContainerMaxStackSize()) : int.MaxValue;
		if (num2 > 0)
		{
			while (amount > 0)
			{
				int num3 = Mathf.Min(amount, num2);
				global::Item item = ItemManager.Create(itemToCreate, num3, skin);
				amount -= num3;
				if (!item.MoveToContainer(this, -1, true, false, null, true))
				{
					item.Remove(0f);
				}
			}
		}
	}

	// Token: 0x06002CC8 RID: 11464 RVA: 0x0010F61C File Offset: 0x0010D81C
	public void OnMovedToWorld()
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			this.itemList[i].OnMovedToWorld();
		}
	}

	// Token: 0x06002CC9 RID: 11465 RVA: 0x0010F650 File Offset: 0x0010D850
	public void OnRemovedFromWorld()
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			this.itemList[i].OnRemovedFromWorld();
		}
	}

	// Token: 0x06002CCA RID: 11466 RVA: 0x0010F684 File Offset: 0x0010D884
	public uint ContentsHash()
	{
		uint num = 0U;
		for (int i = 0; i < this.capacity; i++)
		{
			global::Item slot = this.GetSlot(i);
			if (slot != null)
			{
				num = CRC.Compute32(num, slot.info.itemid);
				num = CRC.Compute32(num, slot.skin);
			}
		}
		return num;
	}

	// Token: 0x06002CCB RID: 11467 RVA: 0x0010F6D0 File Offset: 0x0010D8D0
	internal global::ItemContainer FindContainer(ItemContainerId id)
	{
		if (id == this.uid)
		{
			return this;
		}
		for (int i = 0; i < this.itemList.Count; i++)
		{
			global::Item item = this.itemList[i];
			if (item.contents != null)
			{
				global::ItemContainer itemContainer = item.contents.FindContainer(id);
				if (itemContainer != null)
				{
					return itemContainer;
				}
			}
		}
		return null;
	}

	// Token: 0x06002CCC RID: 11468 RVA: 0x0010F72C File Offset: 0x0010D92C
	public global::ItemContainer.CanAcceptResult CanAcceptItem(global::Item item, int targetPos)
	{
		if (this.canAcceptItem != null && !this.canAcceptItem(item, targetPos))
		{
			return global::ItemContainer.CanAcceptResult.CannotAccept;
		}
		if (this.isServer && this.availableSlots != null && this.availableSlots.Count > 0)
		{
			if (item.info.occupySlots == (ItemSlot)0 || item.info.occupySlots == ItemSlot.None)
			{
				return global::ItemContainer.CanAcceptResult.CannotAccept;
			}
			if (item.isBroken)
			{
				return global::ItemContainer.CanAcceptResult.CannotAccept;
			}
			int num = 0;
			foreach (ItemSlot itemSlot in this.availableSlots)
			{
				num |= (int)itemSlot;
			}
			if ((num & (int)item.info.occupySlots) != (int)item.info.occupySlots)
			{
				return global::ItemContainer.CanAcceptResult.CannotAcceptRightNow;
			}
		}
		if ((this.allowedContents & item.info.itemType) != item.info.itemType)
		{
			return global::ItemContainer.CanAcceptResult.CannotAccept;
		}
		if (this.HasLimitedAllowedItems)
		{
			bool flag = false;
			for (int i = 0; i < this.onlyAllowedItems.Length; i++)
			{
				if (this.onlyAllowedItems[i] == item.info)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return global::ItemContainer.CanAcceptResult.CannotAccept;
			}
		}
		return global::ItemContainer.CanAcceptResult.CanAccept;
	}

	// Token: 0x02000D72 RID: 3442
	[Flags]
	public enum Flag
	{
		// Token: 0x0400477E RID: 18302
		IsPlayer = 1,
		// Token: 0x0400477F RID: 18303
		Clothing = 2,
		// Token: 0x04004780 RID: 18304
		Belt = 4,
		// Token: 0x04004781 RID: 18305
		SingleType = 8,
		// Token: 0x04004782 RID: 18306
		IsLocked = 16,
		// Token: 0x04004783 RID: 18307
		ShowSlotsOnIcon = 32,
		// Token: 0x04004784 RID: 18308
		NoBrokenItems = 64,
		// Token: 0x04004785 RID: 18309
		NoItemInput = 128,
		// Token: 0x04004786 RID: 18310
		ContentsHidden = 256
	}

	// Token: 0x02000D73 RID: 3443
	[Flags]
	public enum ContentsType
	{
		// Token: 0x04004788 RID: 18312
		Generic = 1,
		// Token: 0x04004789 RID: 18313
		Liquid = 2
	}

	// Token: 0x02000D74 RID: 3444
	public enum LimitStack
	{
		// Token: 0x0400478B RID: 18315
		None,
		// Token: 0x0400478C RID: 18316
		Existing,
		// Token: 0x0400478D RID: 18317
		All
	}

	// Token: 0x02000D75 RID: 3445
	public enum CanAcceptResult
	{
		// Token: 0x0400478F RID: 18319
		CanAccept,
		// Token: 0x04004790 RID: 18320
		CannotAccept,
		// Token: 0x04004791 RID: 18321
		CannotAcceptRightNow
	}
}
