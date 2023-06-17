using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020005C4 RID: 1476
public class Item
{
	// Token: 0x0400240F RID: 9231
	private const string DefaultArmourBreakEffectPath = "assets/bundled/prefabs/fx/armor_break.prefab";

	// Token: 0x04002410 RID: 9232
	private float _condition;

	// Token: 0x04002411 RID: 9233
	private float _maxCondition = 100f;

	// Token: 0x04002412 RID: 9234
	public ItemDefinition info;

	// Token: 0x04002413 RID: 9235
	public ItemId uid;

	// Token: 0x04002414 RID: 9236
	public bool dirty;

	// Token: 0x04002415 RID: 9237
	public int amount = 1;

	// Token: 0x04002416 RID: 9238
	public int position;

	// Token: 0x04002417 RID: 9239
	public float busyTime;

	// Token: 0x04002418 RID: 9240
	public float removeTime;

	// Token: 0x04002419 RID: 9241
	public float fuel;

	// Token: 0x0400241A RID: 9242
	public bool isServer;

	// Token: 0x0400241B RID: 9243
	public ProtoBuf.Item.InstanceData instanceData;

	// Token: 0x0400241C RID: 9244
	public ulong skin;

	// Token: 0x0400241D RID: 9245
	public string name;

	// Token: 0x0400241E RID: 9246
	public string streamerName;

	// Token: 0x0400241F RID: 9247
	public string text;

	// Token: 0x04002420 RID: 9248
	public float cookTimeLeft;

	// Token: 0x04002422 RID: 9250
	public global::Item.Flag flags;

	// Token: 0x04002423 RID: 9251
	public global::ItemContainer contents;

	// Token: 0x04002424 RID: 9252
	public global::ItemContainer parent;

	// Token: 0x04002425 RID: 9253
	private EntityRef worldEnt;

	// Token: 0x04002426 RID: 9254
	private EntityRef heldEntity;

	// Token: 0x1700039E RID: 926
	// (get) Token: 0x06002C3F RID: 11327 RVA: 0x0010C3BB File Offset: 0x0010A5BB
	// (set) Token: 0x06002C3E RID: 11326 RVA: 0x0010C374 File Offset: 0x0010A574
	public float condition
	{
		get
		{
			return this._condition;
		}
		set
		{
			float condition = this._condition;
			this._condition = Mathf.Clamp(value, 0f, this.maxCondition);
			if (this.isServer && Mathf.Ceil(value) != Mathf.Ceil(condition))
			{
				this.MarkDirty();
			}
		}
	}

	// Token: 0x1700039F RID: 927
	// (get) Token: 0x06002C41 RID: 11329 RVA: 0x0010C3F4 File Offset: 0x0010A5F4
	// (set) Token: 0x06002C40 RID: 11328 RVA: 0x0010C3C3 File Offset: 0x0010A5C3
	public float maxCondition
	{
		get
		{
			return this._maxCondition;
		}
		set
		{
			this._maxCondition = Mathf.Clamp(value, 0f, this.info.condition.max);
			if (this.isServer)
			{
				this.MarkDirty();
			}
		}
	}

	// Token: 0x170003A0 RID: 928
	// (get) Token: 0x06002C42 RID: 11330 RVA: 0x0010C3FC File Offset: 0x0010A5FC
	public float maxConditionNormalized
	{
		get
		{
			return this._maxCondition / this.info.condition.max;
		}
	}

	// Token: 0x170003A1 RID: 929
	// (get) Token: 0x06002C43 RID: 11331 RVA: 0x0010C415 File Offset: 0x0010A615
	// (set) Token: 0x06002C44 RID: 11332 RVA: 0x0010C432 File Offset: 0x0010A632
	public float conditionNormalized
	{
		get
		{
			if (!this.hasCondition)
			{
				return 1f;
			}
			return this.condition / this.maxCondition;
		}
		set
		{
			if (!this.hasCondition)
			{
				return;
			}
			this.condition = value * this.maxCondition;
		}
	}

	// Token: 0x170003A2 RID: 930
	// (get) Token: 0x06002C45 RID: 11333 RVA: 0x0010C44B File Offset: 0x0010A64B
	public bool hasCondition
	{
		get
		{
			return this.info != null && this.info.condition.enabled && this.info.condition.max > 0f;
		}
	}

	// Token: 0x170003A3 RID: 931
	// (get) Token: 0x06002C46 RID: 11334 RVA: 0x0010C486 File Offset: 0x0010A686
	public bool isBroken
	{
		get
		{
			return this.hasCondition && this.condition <= 0f;
		}
	}

	// Token: 0x06002C47 RID: 11335 RVA: 0x0010C4A4 File Offset: 0x0010A6A4
	public void LoseCondition(float amount)
	{
		if (!this.hasCondition)
		{
			return;
		}
		if (Debugging.disablecondition)
		{
			return;
		}
		float condition = this.condition;
		this.condition -= amount;
		if (ConVar.Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[]
			{
				this.info.shortname,
				" was damaged by: ",
				amount,
				"cond is: ",
				this.condition,
				"/",
				this.maxCondition
			}));
		}
		if (this.condition <= 0f && this.condition < condition)
		{
			this.OnBroken();
		}
	}

	// Token: 0x06002C48 RID: 11336 RVA: 0x0010C556 File Offset: 0x0010A756
	public void RepairCondition(float amount)
	{
		if (!this.hasCondition)
		{
			return;
		}
		this.condition += amount;
	}

	// Token: 0x06002C49 RID: 11337 RVA: 0x0010C570 File Offset: 0x0010A770
	public void DoRepair(float maxLossFraction)
	{
		if (!this.hasCondition)
		{
			return;
		}
		if (this.info.condition.maintainMaxCondition)
		{
			maxLossFraction = 0f;
		}
		float num = 1f - this.condition / this.maxCondition;
		maxLossFraction = Mathf.Clamp(maxLossFraction, 0f, this.info.condition.max);
		this.maxCondition *= 1f - maxLossFraction * num;
		this.condition = this.maxCondition;
		global::BaseEntity baseEntity = this.GetHeldEntity();
		if (baseEntity != null)
		{
			baseEntity.SetFlag(global::BaseEntity.Flags.Broken, false, false, true);
		}
		if (ConVar.Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[]
			{
				this.info.shortname,
				" was repaired! new cond is: ",
				this.condition,
				"/",
				this.maxCondition
			}));
		}
	}

	// Token: 0x06002C4A RID: 11338 RVA: 0x0010C664 File Offset: 0x0010A864
	public global::ItemContainer GetRootContainer()
	{
		global::ItemContainer itemContainer = this.parent;
		int num = 0;
		while (itemContainer != null && num <= 8 && itemContainer.parent != null && itemContainer.parent.parent != null)
		{
			itemContainer = itemContainer.parent.parent;
			num++;
		}
		if (num == 8)
		{
			Debug.LogWarning("GetRootContainer failed with 8 iterations");
		}
		return itemContainer;
	}

	// Token: 0x06002C4B RID: 11339 RVA: 0x0010C6B8 File Offset: 0x0010A8B8
	public virtual void OnBroken()
	{
		if (!this.hasCondition)
		{
			return;
		}
		global::BaseEntity baseEntity = this.GetHeldEntity();
		if (baseEntity != null)
		{
			baseEntity.SetFlag(global::BaseEntity.Flags.Broken, true, false, true);
		}
		global::BasePlayer ownerPlayer = this.GetOwnerPlayer();
		if (ownerPlayer)
		{
			if (ownerPlayer.GetActiveItem() == this)
			{
				Effect.server.Run("assets/bundled/prefabs/fx/item_break.prefab", ownerPlayer, 0U, Vector3.zero, Vector3.zero, null, false);
				ownerPlayer.ChatMessage("Your active item was broken!");
			}
			ItemModWearable itemModWearable;
			if (this.info.TryGetComponent<ItemModWearable>(out itemModWearable) && ownerPlayer.inventory.containerWear.itemList.Contains(this))
			{
				if (itemModWearable.breakEffect.isValid)
				{
					Effect.server.Run(itemModWearable.breakEffect.resourcePath, ownerPlayer, 0U, Vector3.zero, Vector3.zero, null, false);
				}
				else
				{
					Effect.server.Run("assets/bundled/prefabs/fx/armor_break.prefab", ownerPlayer, 0U, Vector3.zero, Vector3.zero, null, false);
				}
			}
		}
		if ((!this.info.condition.repairable && !this.info.GetComponent<ItemModRepair>()) || this.maxCondition <= 5f)
		{
			this.Remove(0f);
		}
		else if (this.parent != null && this.parent.HasFlag(global::ItemContainer.Flag.NoBrokenItems))
		{
			global::ItemContainer rootContainer = this.GetRootContainer();
			if (rootContainer.HasFlag(global::ItemContainer.Flag.NoBrokenItems))
			{
				this.Remove(0f);
			}
			else
			{
				global::BasePlayer playerOwner = rootContainer.playerOwner;
				if (playerOwner != null && !this.MoveToContainer(playerOwner.inventory.containerMain, -1, true, false, null, true))
				{
					this.Drop(playerOwner.transform.position, playerOwner.eyes.BodyForward() * 1.5f, default(Quaternion));
				}
			}
		}
		this.MarkDirty();
	}

	// Token: 0x06002C4C RID: 11340 RVA: 0x0010C873 File Offset: 0x0010AA73
	public string GetName(bool? streamerModeOverride = null)
	{
		if (streamerModeOverride == null)
		{
			return this.name;
		}
		if (!streamerModeOverride.Value)
		{
			return this.name;
		}
		return this.streamerName ?? this.name;
	}

	// Token: 0x170003A4 RID: 932
	// (get) Token: 0x06002C4D RID: 11341 RVA: 0x0010C8A8 File Offset: 0x0010AAA8
	public int despawnMultiplier
	{
		get
		{
			Rarity rarity = this.info.despawnRarity;
			if (rarity == Rarity.None)
			{
				rarity = this.info.rarity;
			}
			if (!(this.info != null))
			{
				return 1;
			}
			return Mathf.Clamp((rarity - Rarity.Common) * 4, 1, 100);
		}
	}

	// Token: 0x170003A5 RID: 933
	// (get) Token: 0x06002C4E RID: 11342 RVA: 0x0010C8ED File Offset: 0x0010AAED
	public ItemDefinition blueprintTargetDef
	{
		get
		{
			if (!this.IsBlueprint())
			{
				return null;
			}
			return ItemManager.FindItemDefinition(this.blueprintTarget);
		}
	}

	// Token: 0x170003A6 RID: 934
	// (get) Token: 0x06002C4F RID: 11343 RVA: 0x0010C904 File Offset: 0x0010AB04
	// (set) Token: 0x06002C50 RID: 11344 RVA: 0x0010C91B File Offset: 0x0010AB1B
	public int blueprintTarget
	{
		get
		{
			if (this.instanceData == null)
			{
				return 0;
			}
			return this.instanceData.blueprintTarget;
		}
		set
		{
			if (this.instanceData == null)
			{
				this.instanceData = new ProtoBuf.Item.InstanceData();
			}
			this.instanceData.ShouldPool = false;
			this.instanceData.blueprintTarget = value;
		}
	}

	// Token: 0x170003A7 RID: 935
	// (get) Token: 0x06002C51 RID: 11345 RVA: 0x0010C948 File Offset: 0x0010AB48
	// (set) Token: 0x06002C52 RID: 11346 RVA: 0x0010C950 File Offset: 0x0010AB50
	public int blueprintAmount
	{
		get
		{
			return this.amount;
		}
		set
		{
			this.amount = value;
		}
	}

	// Token: 0x06002C53 RID: 11347 RVA: 0x0010C959 File Offset: 0x0010AB59
	public bool IsBlueprint()
	{
		return this.blueprintTarget != 0;
	}

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x06002C54 RID: 11348 RVA: 0x0010C964 File Offset: 0x0010AB64
	// (remove) Token: 0x06002C55 RID: 11349 RVA: 0x0010C99C File Offset: 0x0010AB9C
	public event Action<global::Item> OnDirty;

	// Token: 0x06002C56 RID: 11350 RVA: 0x0010C9D1 File Offset: 0x0010ABD1
	public bool HasFlag(global::Item.Flag f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x06002C57 RID: 11351 RVA: 0x0010C9DE File Offset: 0x0010ABDE
	public void SetFlag(global::Item.Flag f, bool b)
	{
		if (b)
		{
			this.flags |= f;
			return;
		}
		this.flags &= ~f;
	}

	// Token: 0x06002C58 RID: 11352 RVA: 0x0010CA01 File Offset: 0x0010AC01
	public bool IsOn()
	{
		return this.HasFlag(global::Item.Flag.IsOn);
	}

	// Token: 0x06002C59 RID: 11353 RVA: 0x0010CA0A File Offset: 0x0010AC0A
	public bool IsOnFire()
	{
		return this.HasFlag(global::Item.Flag.OnFire);
	}

	// Token: 0x06002C5A RID: 11354 RVA: 0x0010CA13 File Offset: 0x0010AC13
	public bool IsCooking()
	{
		return this.HasFlag(global::Item.Flag.Cooking);
	}

	// Token: 0x06002C5B RID: 11355 RVA: 0x0010CA1D File Offset: 0x0010AC1D
	public bool IsLocked()
	{
		return this.HasFlag(global::Item.Flag.IsLocked) || (this.parent != null && this.parent.IsLocked());
	}

	// Token: 0x170003A8 RID: 936
	// (get) Token: 0x06002C5C RID: 11356 RVA: 0x0010CA3F File Offset: 0x0010AC3F
	public global::Item parentItem
	{
		get
		{
			if (this.parent == null)
			{
				return null;
			}
			return this.parent.parent;
		}
	}

	// Token: 0x06002C5D RID: 11357 RVA: 0x0010CA56 File Offset: 0x0010AC56
	public void MarkDirty()
	{
		this.OnChanged();
		this.dirty = true;
		if (this.parent != null)
		{
			this.parent.MarkDirty();
		}
		if (this.OnDirty != null)
		{
			this.OnDirty(this);
		}
	}

	// Token: 0x06002C5E RID: 11358 RVA: 0x0010CA8C File Offset: 0x0010AC8C
	public void OnChanged()
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnChanged(this);
		}
		if (this.contents != null)
		{
			this.contents.OnChanged();
		}
	}

	// Token: 0x06002C5F RID: 11359 RVA: 0x0010CAD0 File Offset: 0x0010ACD0
	public void CollectedForCrafting(global::BasePlayer crafter)
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].CollectedForCrafting(this, crafter);
		}
	}

	// Token: 0x06002C60 RID: 11360 RVA: 0x0010CB04 File Offset: 0x0010AD04
	public void ReturnedFromCancelledCraft(global::BasePlayer crafter)
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].ReturnedFromCancelledCraft(this, crafter);
		}
	}

	// Token: 0x06002C61 RID: 11361 RVA: 0x0010CB38 File Offset: 0x0010AD38
	public void Initialize(ItemDefinition template)
	{
		this.uid = new ItemId(Network.Net.sv.TakeUID());
		this.condition = (this.maxCondition = this.info.condition.max);
		this.OnItemCreated();
	}

	// Token: 0x06002C62 RID: 11362 RVA: 0x0010CB80 File Offset: 0x0010AD80
	public void OnItemCreated()
	{
		this.onCycle = null;
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnItemCreated(this);
		}
	}

	// Token: 0x06002C63 RID: 11363 RVA: 0x0010CBB8 File Offset: 0x0010ADB8
	public void OnVirginSpawn()
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnVirginItem(this);
		}
	}

	// Token: 0x06002C64 RID: 11364 RVA: 0x0010CBE8 File Offset: 0x0010ADE8
	public float GetDespawnDuration()
	{
		if (this.info.quickDespawn)
		{
			return ConVar.Server.itemdespawn_quick;
		}
		return ConVar.Server.itemdespawn * (float)this.despawnMultiplier;
	}

	// Token: 0x06002C65 RID: 11365 RVA: 0x0010CC0C File Offset: 0x0010AE0C
	protected void RemoveFromWorld()
	{
		global::BaseEntity worldEntity = this.GetWorldEntity();
		if (worldEntity == null)
		{
			return;
		}
		this.SetWorldEntity(null);
		this.OnRemovedFromWorld();
		if (this.contents != null)
		{
			this.contents.OnRemovedFromWorld();
		}
		if (!worldEntity.IsValid())
		{
			return;
		}
		global::WorldItem worldItem;
		if ((worldItem = (worldEntity as global::WorldItem)) != null)
		{
			worldItem.RemoveItem();
		}
		worldEntity.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06002C66 RID: 11366 RVA: 0x0010CC6C File Offset: 0x0010AE6C
	public void OnRemovedFromWorld()
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnRemovedFromWorld(this);
		}
	}

	// Token: 0x06002C67 RID: 11367 RVA: 0x0010CC9C File Offset: 0x0010AE9C
	public void RemoveFromContainer()
	{
		if (this.parent == null)
		{
			return;
		}
		this.SetParent(null);
	}

	// Token: 0x06002C68 RID: 11368 RVA: 0x0010CCAE File Offset: 0x0010AEAE
	public bool DoItemSlotsConflict(global::Item other)
	{
		return (this.info.occupySlots & other.info.occupySlots) > (ItemSlot)0;
	}

	// Token: 0x06002C69 RID: 11369 RVA: 0x0010CCCC File Offset: 0x0010AECC
	public void SetParent(global::ItemContainer target)
	{
		if (target == this.parent)
		{
			return;
		}
		if (this.parent != null)
		{
			this.parent.Remove(this);
			this.parent = null;
		}
		if (target == null)
		{
			this.position = 0;
		}
		else
		{
			this.parent = target;
			if (!this.parent.Insert(this))
			{
				this.Remove(0f);
				Debug.LogError("Item.SetParent caused remove - this shouldn't ever happen");
			}
		}
		this.MarkDirty();
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnParentChanged(this);
		}
	}

	// Token: 0x06002C6A RID: 11370 RVA: 0x0010CD60 File Offset: 0x0010AF60
	public void OnAttacked(HitInfo hitInfo)
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnAttacked(this, hitInfo);
		}
	}

	// Token: 0x06002C6B RID: 11371 RVA: 0x0010CD91 File Offset: 0x0010AF91
	public global::BaseEntity GetEntityOwner()
	{
		global::ItemContainer itemContainer = this.parent;
		if (itemContainer == null)
		{
			return null;
		}
		return itemContainer.GetEntityOwner(false);
	}

	// Token: 0x06002C6C RID: 11372 RVA: 0x0010CDA8 File Offset: 0x0010AFA8
	public bool IsChildContainer(global::ItemContainer c)
	{
		if (this.contents == null)
		{
			return false;
		}
		if (this.contents == c)
		{
			return true;
		}
		using (List<global::Item>.Enumerator enumerator = this.contents.itemList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsChildContainer(c))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002C6D RID: 11373 RVA: 0x0010CE1C File Offset: 0x0010B01C
	public bool CanMoveTo(global::ItemContainer newcontainer, int iTargetPos = -1)
	{
		return !this.IsChildContainer(newcontainer) && newcontainer.CanAcceptItem(this, iTargetPos) == global::ItemContainer.CanAcceptResult.CanAccept && iTargetPos < newcontainer.capacity && (this.parent == null || newcontainer != this.parent || iTargetPos != this.position);
	}

	// Token: 0x06002C6E RID: 11374 RVA: 0x0010CE68 File Offset: 0x0010B068
	public bool MoveToContainer(global::ItemContainer newcontainer, int iTargetPos = -1, bool allowStack = true, bool ignoreStackLimit = false, global::BasePlayer sourcePlayer = null, bool allowSwap = true)
	{
		bool result;
		using (TimeWarning.New("MoveToContainer", 0))
		{
			bool flag = iTargetPos == -1;
			global::ItemContainer itemContainer = this.parent;
			if (iTargetPos == -1)
			{
				if (allowStack && this.info.stackable > 1)
				{
					foreach (global::Item item in from x in newcontainer.FindItemsByItemID(this.info.itemid)
					orderby x.position
					select x)
					{
						if (item.CanStack(this) && (ignoreStackLimit || item.amount < item.MaxStackable()))
						{
							iTargetPos = item.position;
						}
					}
				}
				if (iTargetPos == -1)
				{
					IItemContainerEntity itemContainerEntity = newcontainer.GetEntityOwner(true) as IItemContainerEntity;
					if (itemContainerEntity != null)
					{
						iTargetPos = itemContainerEntity.GetIdealSlot(sourcePlayer, this);
						if (iTargetPos == -2147483648)
						{
							return false;
						}
					}
				}
				if (iTargetPos == -1)
				{
					if (newcontainer == this.parent)
					{
						return false;
					}
					bool flag2 = newcontainer.HasFlag(global::ItemContainer.Flag.Clothing) && this.info.isWearable;
					ItemModWearable itemModWearable = this.info.ItemModWearable;
					for (int i = 0; i < newcontainer.capacity; i++)
					{
						global::Item slot = newcontainer.GetSlot(i);
						if (slot == null)
						{
							if (this.CanMoveTo(newcontainer, i))
							{
								iTargetPos = i;
								break;
							}
						}
						else
						{
							if (flag2 && slot != null && !slot.info.ItemModWearable.CanExistWith(itemModWearable))
							{
								iTargetPos = i;
								break;
							}
							if (newcontainer.availableSlots != null && newcontainer.availableSlots.Count > 0 && this.DoItemSlotsConflict(slot))
							{
								iTargetPos = i;
								break;
							}
						}
					}
					if (flag2 && iTargetPos == -1)
					{
						iTargetPos = newcontainer.capacity - 1;
					}
				}
			}
			if (iTargetPos == -1)
			{
				result = false;
			}
			else if (!this.CanMoveTo(newcontainer, iTargetPos))
			{
				result = false;
			}
			else if (iTargetPos >= 0 && newcontainer.SlotTaken(this, iTargetPos))
			{
				global::Item slot2 = newcontainer.GetSlot(iTargetPos);
				if (slot2 == this)
				{
					result = false;
				}
				else
				{
					if (allowStack && slot2 != null)
					{
						int num = slot2.MaxStackable();
						if (slot2.CanStack(this))
						{
							if (ignoreStackLimit)
							{
								num = int.MaxValue;
							}
							if (slot2.amount >= num)
							{
								return false;
							}
							int num2 = Mathf.Min(num - slot2.amount, this.amount);
							slot2.amount += num2;
							this.amount -= num2;
							slot2.MarkDirty();
							this.MarkDirty();
							if (this.amount <= 0)
							{
								this.RemoveFromWorld();
								this.RemoveFromContainer();
								this.Remove(0f);
								return true;
							}
							if (flag)
							{
								return this.MoveToContainer(newcontainer, -1, allowStack, ignoreStackLimit, sourcePlayer, true);
							}
							return false;
						}
					}
					if (this.parent != null && allowSwap && slot2 != null)
					{
						global::ItemContainer itemContainer2 = this.parent;
						int iTargetPos2 = this.position;
						global::ItemContainer itemContainer3 = slot2.parent;
						int num3 = slot2.position;
						if (!slot2.CanMoveTo(itemContainer2, iTargetPos2))
						{
							result = false;
						}
						else
						{
							global::BaseEntity entityOwner = this.GetEntityOwner();
							global::BaseEntity entityOwner2 = slot2.GetEntityOwner();
							this.RemoveFromContainer();
							slot2.RemoveFromContainer();
							this.RemoveConflictingSlots(newcontainer, entityOwner, sourcePlayer);
							slot2.RemoveConflictingSlots(itemContainer2, entityOwner2, sourcePlayer);
							if (!slot2.MoveToContainer(itemContainer2, iTargetPos2, true, false, sourcePlayer, true) || !this.MoveToContainer(newcontainer, iTargetPos, true, false, sourcePlayer, true))
							{
								this.RemoveFromContainer();
								slot2.RemoveFromContainer();
								this.SetParent(itemContainer2);
								this.position = iTargetPos2;
								slot2.SetParent(itemContainer3);
								slot2.position = num3;
								result = true;
							}
							else
							{
								result = true;
							}
						}
					}
					else
					{
						result = false;
					}
				}
			}
			else if (this.parent == newcontainer)
			{
				if (iTargetPos >= 0 && iTargetPos != this.position && !this.parent.SlotTaken(this, iTargetPos))
				{
					this.position = iTargetPos;
					this.MarkDirty();
					result = true;
				}
				else
				{
					result = false;
				}
			}
			else if (newcontainer.maxStackSize > 0 && newcontainer.maxStackSize < this.amount)
			{
				global::Item item2 = this.SplitItem(newcontainer.maxStackSize);
				if (item2 != null && !item2.MoveToContainer(newcontainer, iTargetPos, false, false, sourcePlayer, true) && (itemContainer == null || !item2.MoveToContainer(itemContainer, -1, true, false, sourcePlayer, true)))
				{
					item2.Drop(newcontainer.dropPosition, newcontainer.dropVelocity, default(Quaternion));
				}
				result = true;
			}
			else if (!newcontainer.CanAccept(this))
			{
				result = false;
			}
			else
			{
				global::BaseEntity entityOwner3 = this.GetEntityOwner();
				this.RemoveFromContainer();
				this.RemoveFromWorld();
				this.RemoveConflictingSlots(newcontainer, entityOwner3, sourcePlayer);
				this.position = iTargetPos;
				this.SetParent(newcontainer);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06002C6F RID: 11375 RVA: 0x0010D35C File Offset: 0x0010B55C
	private void RemoveConflictingSlots(global::ItemContainer container, global::BaseEntity entityOwner, global::BasePlayer sourcePlayer)
	{
		if (this.isServer && container.availableSlots != null && container.availableSlots.Count > 0)
		{
			List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
			list.AddRange(container.itemList);
			foreach (global::Item item in list)
			{
				if (item.DoItemSlotsConflict(this))
				{
					item.RemoveFromContainer();
					global::BasePlayer basePlayer;
					IItemContainerEntity itemContainerEntity;
					if ((basePlayer = (entityOwner as global::BasePlayer)) != null)
					{
						basePlayer.GiveItem(item, global::BaseEntity.GiveItemReason.Generic);
					}
					else if ((itemContainerEntity = (entityOwner as IItemContainerEntity)) != null)
					{
						item.MoveToContainer(itemContainerEntity.inventory, -1, true, false, sourcePlayer, true);
					}
				}
			}
			Facepunch.Pool.FreeList<global::Item>(ref list);
		}
	}

	// Token: 0x06002C70 RID: 11376 RVA: 0x0010D424 File Offset: 0x0010B624
	public global::BaseEntity CreateWorldObject(Vector3 pos, Quaternion rotation = default(Quaternion), global::BaseEntity parentEnt = null, uint parentBone = 0U)
	{
		global::BaseEntity baseEntity = this.GetWorldEntity();
		if (baseEntity != null)
		{
			return baseEntity;
		}
		baseEntity = GameManager.server.CreateEntity("assets/prefabs/misc/burlap sack/generic_world.prefab", pos, rotation, true);
		if (baseEntity == null)
		{
			Debug.LogWarning("Couldn't create world object for prefab: items/generic_world");
			return null;
		}
		global::WorldItem worldItem = baseEntity as global::WorldItem;
		if (worldItem != null)
		{
			worldItem.InitializeItem(this);
		}
		if (parentEnt != null)
		{
			baseEntity.SetParent(parentEnt, parentBone, false, false);
		}
		baseEntity.Spawn();
		this.SetWorldEntity(baseEntity);
		return this.GetWorldEntity();
	}

	// Token: 0x06002C71 RID: 11377 RVA: 0x0010D4AC File Offset: 0x0010B6AC
	public global::BaseEntity Drop(Vector3 vPos, Vector3 vVelocity, Quaternion rotation = default(Quaternion))
	{
		this.RemoveFromWorld();
		global::BaseEntity baseEntity = null;
		if (vPos != Vector3.zero && !this.info.HasFlag(ItemDefinition.Flag.NoDropping))
		{
			baseEntity = this.CreateWorldObject(vPos, rotation, null, 0U);
			if (baseEntity)
			{
				baseEntity.SetVelocity(vVelocity);
			}
		}
		else
		{
			this.Remove(0f);
		}
		this.RemoveFromContainer();
		return baseEntity;
	}

	// Token: 0x06002C72 RID: 11378 RVA: 0x0010D50C File Offset: 0x0010B70C
	public global::BaseEntity DropAndTossUpwards(Vector3 vPos, float force = 2f)
	{
		float f = UnityEngine.Random.value * 3.1415927f * 2f;
		Vector3 a = new Vector3(Mathf.Sin(f), 1f, Mathf.Cos(f));
		return this.Drop(vPos + Vector3.up * 0.1f, a * force, default(Quaternion));
	}

	// Token: 0x06002C73 RID: 11379 RVA: 0x0010D56E File Offset: 0x0010B76E
	public bool IsBusy()
	{
		return this.busyTime > UnityEngine.Time.time;
	}

	// Token: 0x06002C74 RID: 11380 RVA: 0x0010D580 File Offset: 0x0010B780
	public void BusyFor(float fTime)
	{
		this.busyTime = UnityEngine.Time.time + fTime;
	}

	// Token: 0x06002C75 RID: 11381 RVA: 0x0010D590 File Offset: 0x0010B790
	public void Remove(float fTime = 0f)
	{
		if (this.removeTime > 0f)
		{
			return;
		}
		if (this.isServer)
		{
			ItemMod[] itemMods = this.info.itemMods;
			for (int i = 0; i < itemMods.Length; i++)
			{
				itemMods[i].OnRemove(this);
			}
		}
		this.onCycle = null;
		this.removeTime = UnityEngine.Time.time + fTime;
		this.OnDirty = null;
		this.position = -1;
		if (this.isServer)
		{
			ItemManager.RemoveItem(this, fTime);
		}
	}

	// Token: 0x06002C76 RID: 11382 RVA: 0x0010D608 File Offset: 0x0010B808
	internal void DoRemove()
	{
		this.OnDirty = null;
		this.onCycle = null;
		if (this.isServer && this.uid.IsValid && Network.Net.sv != null)
		{
			Network.Net.sv.ReturnUID(this.uid.Value);
			this.uid = default(ItemId);
		}
		if (this.contents != null)
		{
			this.contents.Kill();
			this.contents = null;
		}
		if (this.isServer)
		{
			this.RemoveFromWorld();
			this.RemoveFromContainer();
		}
		global::BaseEntity baseEntity = this.GetHeldEntity();
		if (baseEntity.IsValid())
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"Item's Held Entity not removed!",
				this.info.displayName.english,
				" -> ",
				baseEntity
			}), baseEntity);
		}
	}

	// Token: 0x06002C77 RID: 11383 RVA: 0x0010D6D5 File Offset: 0x0010B8D5
	public void SwitchOnOff(bool bNewState)
	{
		if (this.HasFlag(global::Item.Flag.IsOn) == bNewState)
		{
			return;
		}
		this.SetFlag(global::Item.Flag.IsOn, bNewState);
		this.MarkDirty();
	}

	// Token: 0x06002C78 RID: 11384 RVA: 0x0010D6F0 File Offset: 0x0010B8F0
	public void LockUnlock(bool bNewState)
	{
		if (this.HasFlag(global::Item.Flag.IsLocked) == bNewState)
		{
			return;
		}
		this.SetFlag(global::Item.Flag.IsLocked, bNewState);
		this.MarkDirty();
	}

	// Token: 0x170003A9 RID: 937
	// (get) Token: 0x06002C79 RID: 11385 RVA: 0x0010D70B File Offset: 0x0010B90B
	public float temperature
	{
		get
		{
			if (this.parent != null)
			{
				return this.parent.GetTemperature(this.position);
			}
			return 15f;
		}
	}

	// Token: 0x06002C7A RID: 11386 RVA: 0x0010D72C File Offset: 0x0010B92C
	public global::BasePlayer GetOwnerPlayer()
	{
		if (this.parent == null)
		{
			return null;
		}
		return this.parent.GetOwnerPlayer();
	}

	// Token: 0x06002C7B RID: 11387 RVA: 0x0010D744 File Offset: 0x0010B944
	public global::Item SplitItem(int split_Amount)
	{
		Assert.IsTrue(split_Amount > 0, "split_Amount <= 0");
		if (split_Amount <= 0)
		{
			return null;
		}
		if (split_Amount >= this.amount)
		{
			return null;
		}
		this.amount -= split_Amount;
		global::Item item = ItemManager.CreateByItemID(this.info.itemid, 1, 0UL);
		item.amount = split_Amount;
		item.skin = this.skin;
		if (this.IsBlueprint())
		{
			item.blueprintTarget = this.blueprintTarget;
		}
		if (this.info.amountType == ItemDefinition.AmountType.Genetics && this.instanceData != null && this.instanceData.dataInt != 0)
		{
			item.instanceData = new ProtoBuf.Item.InstanceData();
			item.instanceData.dataInt = this.instanceData.dataInt;
			item.instanceData.ShouldPool = false;
		}
		if (this.instanceData != null && this.instanceData.dataInt > 0 && this.info != null && this.info.Blueprint != null && this.info.Blueprint.workbenchLevelRequired == 3)
		{
			item.instanceData = new ProtoBuf.Item.InstanceData();
			item.instanceData.dataInt = this.instanceData.dataInt;
			item.instanceData.ShouldPool = false;
			item.SetFlag(global::Item.Flag.IsOn, this.IsOn());
		}
		this.MarkDirty();
		return item;
	}

	// Token: 0x06002C7C RID: 11388 RVA: 0x0010D894 File Offset: 0x0010BA94
	public bool CanBeHeld()
	{
		return !this.isBroken;
	}

	// Token: 0x06002C7D RID: 11389 RVA: 0x0010D8A4 File Offset: 0x0010BAA4
	public bool CanStack(global::Item item)
	{
		if (item == this)
		{
			return false;
		}
		if (this.MaxStackable() <= 1)
		{
			return false;
		}
		if (item.MaxStackable() <= 1)
		{
			return false;
		}
		if (item.info.itemid != this.info.itemid)
		{
			return false;
		}
		if (this.hasCondition && this.condition != item.info.condition.max)
		{
			return false;
		}
		if (item.hasCondition && item.condition != item.info.condition.max)
		{
			return false;
		}
		if (!this.IsValid())
		{
			return false;
		}
		if (this.IsBlueprint() && this.blueprintTarget != item.blueprintTarget)
		{
			return false;
		}
		if (item.skin != this.skin)
		{
			return false;
		}
		if (item.info.amountType == ItemDefinition.AmountType.Genetics || this.info.amountType == ItemDefinition.AmountType.Genetics)
		{
			int num = (item.instanceData != null) ? item.instanceData.dataInt : -1;
			int num2 = (this.instanceData != null) ? this.instanceData.dataInt : -1;
			if (num != num2)
			{
				return false;
			}
		}
		return (item.instanceData == null || this.instanceData == null || (item.IsOn() == this.IsOn() && (item.instanceData.dataInt == this.instanceData.dataInt || !(item.info.Blueprint != null) || item.info.Blueprint.workbenchLevelRequired != 3))) && (this.instanceData == null || !this.instanceData.subEntity.IsValid || !this.info.GetComponent<ItemModSign>()) && (item.instanceData == null || !item.instanceData.subEntity.IsValid || !item.info.GetComponent<ItemModSign>());
	}

	// Token: 0x06002C7E RID: 11390 RVA: 0x0010DA66 File Offset: 0x0010BC66
	public bool IsValid()
	{
		return this.removeTime <= 0f;
	}

	// Token: 0x06002C7F RID: 11391 RVA: 0x0010DA78 File Offset: 0x0010BC78
	public void SetWorldEntity(global::BaseEntity ent)
	{
		if (!ent.IsValid())
		{
			this.worldEnt.Set(null);
			this.MarkDirty();
			return;
		}
		if (this.worldEnt.uid == ent.net.ID)
		{
			return;
		}
		this.worldEnt.Set(ent);
		this.MarkDirty();
		this.OnMovedToWorld();
		if (this.contents != null)
		{
			this.contents.OnMovedToWorld();
		}
	}

	// Token: 0x06002C80 RID: 11392 RVA: 0x0010DAEC File Offset: 0x0010BCEC
	public void OnMovedToWorld()
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnMovedToWorld(this);
		}
	}

	// Token: 0x06002C81 RID: 11393 RVA: 0x0010DB1C File Offset: 0x0010BD1C
	public global::BaseEntity GetWorldEntity()
	{
		return this.worldEnt.Get(this.isServer);
	}

	// Token: 0x06002C82 RID: 11394 RVA: 0x0010DB30 File Offset: 0x0010BD30
	public void SetHeldEntity(global::BaseEntity ent)
	{
		if (!ent.IsValid())
		{
			this.heldEntity.Set(null);
			this.MarkDirty();
			return;
		}
		if (this.heldEntity.uid == ent.net.ID)
		{
			return;
		}
		this.heldEntity.Set(ent);
		this.MarkDirty();
		if (ent.IsValid())
		{
			global::HeldEntity heldEntity = ent as global::HeldEntity;
			if (heldEntity != null)
			{
				heldEntity.SetupHeldEntity(this);
			}
		}
	}

	// Token: 0x06002C83 RID: 11395 RVA: 0x0010DBA7 File Offset: 0x0010BDA7
	public global::BaseEntity GetHeldEntity()
	{
		return this.heldEntity.Get(this.isServer);
	}

	// Token: 0x14000003 RID: 3
	// (add) Token: 0x06002C84 RID: 11396 RVA: 0x0010DBBC File Offset: 0x0010BDBC
	// (remove) Token: 0x06002C85 RID: 11397 RVA: 0x0010DBF4 File Offset: 0x0010BDF4
	public event Action<global::Item, float> onCycle;

	// Token: 0x06002C86 RID: 11398 RVA: 0x0010DC29 File Offset: 0x0010BE29
	public void OnCycle(float delta)
	{
		if (this.onCycle != null)
		{
			this.onCycle(this, delta);
		}
	}

	// Token: 0x06002C87 RID: 11399 RVA: 0x0010DC40 File Offset: 0x0010BE40
	public void ServerCommand(string command, global::BasePlayer player)
	{
		global::HeldEntity heldEntity = this.GetHeldEntity() as global::HeldEntity;
		if (heldEntity != null)
		{
			heldEntity.ServerCommand(this, command, player);
		}
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].ServerCommand(this, command, player);
		}
	}

	// Token: 0x06002C88 RID: 11400 RVA: 0x0010DC90 File Offset: 0x0010BE90
	public void UseItem(int amountToConsume = 1)
	{
		if (amountToConsume <= 0)
		{
			return;
		}
		this.amount -= amountToConsume;
		if (this.amount <= 0)
		{
			this.amount = 0;
			this.Remove(0f);
			return;
		}
		this.MarkDirty();
	}

	// Token: 0x06002C89 RID: 11401 RVA: 0x0010DCC8 File Offset: 0x0010BEC8
	public bool HasAmmo(AmmoTypes ammoType)
	{
		ItemModProjectile itemModProjectile;
		return (this.info.TryGetComponent<ItemModProjectile>(out itemModProjectile) && itemModProjectile.IsAmmo(ammoType)) || (this.contents != null && this.contents.HasAmmo(ammoType));
	}

	// Token: 0x06002C8A RID: 11402 RVA: 0x0010DD08 File Offset: 0x0010BF08
	public void FindAmmo(List<global::Item> list, AmmoTypes ammoType)
	{
		ItemModProjectile itemModProjectile;
		if (this.info.TryGetComponent<ItemModProjectile>(out itemModProjectile) && itemModProjectile.IsAmmo(ammoType))
		{
			list.Add(this);
			return;
		}
		if (this.contents != null)
		{
			this.contents.FindAmmo(list, ammoType);
		}
	}

	// Token: 0x06002C8B RID: 11403 RVA: 0x0010DD4C File Offset: 0x0010BF4C
	public int GetAmmoAmount(AmmoTypes ammoType)
	{
		int num = 0;
		ItemModProjectile itemModProjectile;
		if (this.info.TryGetComponent<ItemModProjectile>(out itemModProjectile) && itemModProjectile.IsAmmo(ammoType))
		{
			num += this.amount;
		}
		if (this.contents != null)
		{
			num += this.contents.GetAmmoAmount(ammoType);
		}
		return num;
	}

	// Token: 0x06002C8C RID: 11404 RVA: 0x0010DD94 File Offset: 0x0010BF94
	public override string ToString()
	{
		return string.Concat(new object[]
		{
			"Item.",
			this.info.shortname,
			"x",
			this.amount,
			".",
			this.uid
		});
	}

	// Token: 0x06002C8D RID: 11405 RVA: 0x0010DDEE File Offset: 0x0010BFEE
	public global::Item FindItem(ItemId iUID)
	{
		if (this.uid == iUID)
		{
			return this;
		}
		if (this.contents == null)
		{
			return null;
		}
		return this.contents.FindItemByUID(iUID);
	}

	// Token: 0x06002C8E RID: 11406 RVA: 0x0010DE18 File Offset: 0x0010C018
	public int MaxStackable()
	{
		int num = this.info.stackable;
		if (this.parent != null && this.parent.maxStackSize > 0)
		{
			num = Mathf.Min(this.parent.maxStackSize, num);
		}
		return num;
	}

	// Token: 0x170003AA RID: 938
	// (get) Token: 0x06002C8F RID: 11407 RVA: 0x0010DE5A File Offset: 0x0010C05A
	public global::BaseEntity.TraitFlag Traits
	{
		get
		{
			return this.info.Traits;
		}
	}

	// Token: 0x06002C90 RID: 11408 RVA: 0x0010DE68 File Offset: 0x0010C068
	public virtual ProtoBuf.Item Save(bool bIncludeContainer = false, bool bIncludeOwners = true)
	{
		this.dirty = false;
		ProtoBuf.Item item = Facepunch.Pool.Get<ProtoBuf.Item>();
		item.UID = this.uid;
		item.itemid = this.info.itemid;
		item.slot = this.position;
		item.amount = this.amount;
		item.flags = (int)this.flags;
		item.removetime = this.removeTime;
		item.locktime = this.busyTime;
		item.instanceData = this.instanceData;
		item.worldEntity = this.worldEnt.uid;
		item.heldEntity = this.heldEntity.uid;
		item.skinid = this.skin;
		item.name = this.name;
		item.streamerName = this.streamerName;
		item.text = this.text;
		item.cooktime = this.cookTimeLeft;
		if (this.hasCondition)
		{
			item.conditionData = Facepunch.Pool.Get<ProtoBuf.Item.ConditionData>();
			item.conditionData.maxCondition = this._maxCondition;
			item.conditionData.condition = this._condition;
		}
		if (this.contents != null && bIncludeContainer)
		{
			item.contents = this.contents.Save();
		}
		return item;
	}

	// Token: 0x06002C91 RID: 11409 RVA: 0x0010DF9C File Offset: 0x0010C19C
	public virtual void Load(ProtoBuf.Item load)
	{
		if (this.info == null || this.info.itemid != load.itemid)
		{
			this.info = ItemManager.FindItemDefinition(load.itemid);
		}
		this.uid = load.UID;
		this.name = load.name;
		this.streamerName = load.streamerName;
		this.text = load.text;
		this.cookTimeLeft = load.cooktime;
		this.amount = load.amount;
		this.position = load.slot;
		this.busyTime = load.locktime;
		this.removeTime = load.removetime;
		this.flags = (global::Item.Flag)load.flags;
		this.worldEnt.uid = load.worldEntity;
		this.heldEntity.uid = load.heldEntity;
		if (this.isServer)
		{
			Network.Net.sv.RegisterUID(this.uid.Value);
		}
		if (this.instanceData != null)
		{
			this.instanceData.ShouldPool = true;
			this.instanceData.ResetToPool();
			this.instanceData = null;
		}
		this.instanceData = load.instanceData;
		if (this.instanceData != null)
		{
			this.instanceData.ShouldPool = false;
		}
		this.skin = load.skinid;
		if (this.info == null || this.info.itemid != load.itemid)
		{
			this.info = ItemManager.FindItemDefinition(load.itemid);
		}
		if (this.info == null)
		{
			return;
		}
		this._condition = 0f;
		this._maxCondition = 0f;
		if (load.conditionData != null)
		{
			this._condition = load.conditionData.condition;
			this._maxCondition = load.conditionData.maxCondition;
		}
		else if (this.info.condition.enabled)
		{
			this._condition = this.info.condition.max;
			this._maxCondition = this.info.condition.max;
		}
		if (load.contents != null)
		{
			if (this.contents == null)
			{
				this.contents = new global::ItemContainer();
				if (this.isServer)
				{
					this.contents.ServerInitialize(this, load.contents.slots);
				}
			}
			this.contents.Load(load.contents);
		}
		if (this.isServer)
		{
			this.removeTime = 0f;
			this.OnItemCreated();
		}
	}

	// Token: 0x02000D70 RID: 3440
	[Flags]
	public enum Flag
	{
		// Token: 0x04004775 RID: 18293
		None = 0,
		// Token: 0x04004776 RID: 18294
		Placeholder = 1,
		// Token: 0x04004777 RID: 18295
		IsOn = 2,
		// Token: 0x04004778 RID: 18296
		OnFire = 4,
		// Token: 0x04004779 RID: 18297
		IsLocked = 8,
		// Token: 0x0400477A RID: 18298
		Cooking = 16
	}
}
