using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x020005D4 RID: 1492
public abstract class ItemModAssociatedEntity<T> : ItemMod where T : global::BaseEntity
{
	// Token: 0x040024A1 RID: 9377
	public GameObjectRef entityPrefab;

	// Token: 0x170003B9 RID: 953
	// (get) Token: 0x06002CFF RID: 11519 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected virtual bool AllowNullParenting
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170003BA RID: 954
	// (get) Token: 0x06002D00 RID: 11520 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected virtual bool AllowHeldEntityParenting
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170003BB RID: 955
	// (get) Token: 0x06002D01 RID: 11521 RVA: 0x0000441C File Offset: 0x0000261C
	protected virtual bool ShouldAutoCreateEntity
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170003BC RID: 956
	// (get) Token: 0x06002D02 RID: 11522 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected virtual bool OwnedByParentPlayer
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002D03 RID: 11523 RVA: 0x0010FCF2 File Offset: 0x0010DEF2
	public override void OnItemCreated(global::Item item)
	{
		base.OnItemCreated(item);
		if (this.ShouldAutoCreateEntity)
		{
			this.CreateAssociatedEntity(item);
		}
	}

	// Token: 0x06002D04 RID: 11524 RVA: 0x0010FD0C File Offset: 0x0010DF0C
	public T CreateAssociatedEntity(global::Item item)
	{
		if (item.instanceData != null)
		{
			return default(T);
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.entityPrefab.resourcePath, Vector3.zero, default(Quaternion), true);
		T component = baseEntity.GetComponent<T>();
		this.OnAssociatedItemCreated(component);
		baseEntity.Spawn();
		item.instanceData = new ProtoBuf.Item.InstanceData();
		item.instanceData.ShouldPool = false;
		item.instanceData.subEntity = baseEntity.net.ID;
		item.MarkDirty();
		return component;
	}

	// Token: 0x06002D05 RID: 11525 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnAssociatedItemCreated(T ent)
	{
	}

	// Token: 0x06002D06 RID: 11526 RVA: 0x0010FD98 File Offset: 0x0010DF98
	public override void OnRemove(global::Item item)
	{
		base.OnRemove(item);
		T associatedEntity = ItemModAssociatedEntity<T>.GetAssociatedEntity(item, true);
		if (associatedEntity)
		{
			associatedEntity.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06002D07 RID: 11527 RVA: 0x0010FDCD File Offset: 0x0010DFCD
	public override void OnMovedToWorld(global::Item item)
	{
		this.UpdateParent(item);
		base.OnMovedToWorld(item);
	}

	// Token: 0x06002D08 RID: 11528 RVA: 0x0010FDDD File Offset: 0x0010DFDD
	public override void OnRemovedFromWorld(global::Item item)
	{
		this.UpdateParent(item);
		base.OnRemovedFromWorld(item);
	}

	// Token: 0x06002D09 RID: 11529 RVA: 0x0010FDF0 File Offset: 0x0010DFF0
	public void UpdateParent(global::Item item)
	{
		T associatedEntity = ItemModAssociatedEntity<T>.GetAssociatedEntity(item, true);
		if (associatedEntity == null)
		{
			return;
		}
		global::BaseEntity entityForParenting = this.GetEntityForParenting(item);
		if (entityForParenting == null)
		{
			if (this.AllowNullParenting)
			{
				associatedEntity.SetParent(null, false, true);
			}
			if (this.OwnedByParentPlayer)
			{
				associatedEntity.OwnerID = 0UL;
			}
			return;
		}
		if (!entityForParenting.isServer)
		{
			return;
		}
		if (!entityForParenting.IsFullySpawned())
		{
			return;
		}
		associatedEntity.SetParent(entityForParenting, false, true);
		global::BasePlayer basePlayer;
		if (this.OwnedByParentPlayer && (basePlayer = (entityForParenting as global::BasePlayer)) != null)
		{
			associatedEntity.OwnerID = basePlayer.userID;
		}
	}

	// Token: 0x06002D0A RID: 11530 RVA: 0x0010FE94 File Offset: 0x0010E094
	public override void OnParentChanged(global::Item item)
	{
		base.OnParentChanged(item);
		this.UpdateParent(item);
	}

	// Token: 0x06002D0B RID: 11531 RVA: 0x0010FEA4 File Offset: 0x0010E0A4
	public global::BaseEntity GetEntityForParenting(global::Item item = null)
	{
		if (item == null)
		{
			return null;
		}
		global::BasePlayer ownerPlayer = item.GetOwnerPlayer();
		if (ownerPlayer)
		{
			return ownerPlayer;
		}
		global::BaseEntity baseEntity = (item.parent == null) ? null : item.parent.entityOwner;
		if (baseEntity != null)
		{
			return baseEntity;
		}
		global::BaseEntity worldEntity = item.GetWorldEntity();
		if (worldEntity)
		{
			return worldEntity;
		}
		if (this.AllowHeldEntityParenting && item.parentItem != null && item.parentItem.GetHeldEntity() != null)
		{
			return item.parentItem.GetHeldEntity();
		}
		return null;
	}

	// Token: 0x06002D0C RID: 11532 RVA: 0x0010FF2C File Offset: 0x0010E12C
	public static T GetAssociatedEntity(global::Item item, bool isServer = true)
	{
		if (((item != null) ? item.instanceData : null) == null)
		{
			return default(T);
		}
		global::BaseNetworkable baseNetworkable = null;
		if (isServer)
		{
			baseNetworkable = global::BaseNetworkable.serverEntities.Find(item.instanceData.subEntity);
		}
		if (baseNetworkable)
		{
			return baseNetworkable.GetComponent<T>();
		}
		return default(T);
	}
}
