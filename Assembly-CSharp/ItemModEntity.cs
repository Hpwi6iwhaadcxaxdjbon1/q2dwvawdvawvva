using System;
using UnityEngine;

// Token: 0x020005EA RID: 1514
public class ItemModEntity : ItemMod
{
	// Token: 0x040024DB RID: 9435
	public GameObjectRef entityPrefab = new GameObjectRef();

	// Token: 0x040024DC RID: 9436
	public string defaultBone;

	// Token: 0x06002D4C RID: 11596 RVA: 0x00110EE4 File Offset: 0x0010F0E4
	public override void OnChanged(Item item)
	{
		HeldEntity heldEntity = item.GetHeldEntity() as HeldEntity;
		if (heldEntity != null)
		{
			heldEntity.OnItemChanged(item);
		}
		base.OnChanged(item);
	}

	// Token: 0x06002D4D RID: 11597 RVA: 0x00110F14 File Offset: 0x0010F114
	public override void OnItemCreated(Item item)
	{
		if (item.GetHeldEntity() == null)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.entityPrefab.resourcePath, default(Vector3), default(Quaternion), true);
			if (baseEntity == null)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"Couldn't create item entity ",
					item.info.displayName.english,
					" (",
					this.entityPrefab.resourcePath,
					")"
				}));
				return;
			}
			baseEntity.skinID = item.skin;
			baseEntity.Spawn();
			item.SetHeldEntity(baseEntity);
		}
	}

	// Token: 0x06002D4E RID: 11598 RVA: 0x00110FC8 File Offset: 0x0010F1C8
	public override void OnRemove(Item item)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		heldEntity.Kill(BaseNetworkable.DestroyMode.None);
		item.SetHeldEntity(null);
	}

	// Token: 0x06002D4F RID: 11599 RVA: 0x00110FF4 File Offset: 0x0010F1F4
	private bool ParentToParent(Item item, BaseEntity ourEntity)
	{
		if (item.parentItem == null)
		{
			return false;
		}
		BaseEntity baseEntity = item.parentItem.GetWorldEntity();
		if (baseEntity == null)
		{
			baseEntity = item.parentItem.GetHeldEntity();
		}
		ourEntity.SetFlag(BaseEntity.Flags.Disabled, false, false, true);
		ourEntity.limitNetworking = false;
		ourEntity.SetParent(baseEntity, this.defaultBone, false, false);
		return true;
	}

	// Token: 0x06002D50 RID: 11600 RVA: 0x00111050 File Offset: 0x0010F250
	private bool ParentToPlayer(Item item, BaseEntity ourEntity)
	{
		HeldEntity heldEntity = ourEntity as HeldEntity;
		if (heldEntity == null)
		{
			return false;
		}
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		if (ownerPlayer)
		{
			heldEntity.SetOwnerPlayer(ownerPlayer);
			return true;
		}
		heldEntity.ClearOwnerPlayer();
		return true;
	}

	// Token: 0x06002D51 RID: 11601 RVA: 0x00111090 File Offset: 0x0010F290
	public override void OnParentChanged(Item item)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		if (this.ParentToParent(item, heldEntity))
		{
			return;
		}
		if (this.ParentToPlayer(item, heldEntity))
		{
			return;
		}
		heldEntity.SetParent(null, false, false);
		heldEntity.limitNetworking = true;
		heldEntity.SetFlag(BaseEntity.Flags.Disabled, true, false, true);
	}

	// Token: 0x06002D52 RID: 11602 RVA: 0x001110E0 File Offset: 0x0010F2E0
	public override void CollectedForCrafting(Item item, BasePlayer crafter)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		HeldEntity heldEntity2 = heldEntity as HeldEntity;
		if (heldEntity2 == null)
		{
			return;
		}
		heldEntity2.CollectedForCrafting(item, crafter);
	}

	// Token: 0x06002D53 RID: 11603 RVA: 0x00111118 File Offset: 0x0010F318
	public override void ReturnedFromCancelledCraft(Item item, BasePlayer crafter)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		HeldEntity heldEntity2 = heldEntity as HeldEntity;
		if (heldEntity2 == null)
		{
			return;
		}
		heldEntity2.ReturnedFromCancelledCraft(item, crafter);
	}
}
