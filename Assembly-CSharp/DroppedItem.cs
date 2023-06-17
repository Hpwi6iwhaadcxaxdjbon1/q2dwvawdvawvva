using System;
using ConVar;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x020004C0 RID: 1216
public class DroppedItem : WorldItem
{
	// Token: 0x04002027 RID: 8231
	[Header("DroppedItem")]
	public GameObject itemModel;

	// Token: 0x04002028 RID: 8232
	private Collider childCollider;

	// Token: 0x04002029 RID: 8233
	[NonSerialized]
	public DroppedItem.DropReasonEnum DropReason;

	// Token: 0x0400202A RID: 8234
	[NonSerialized]
	public ulong DroppedBy;

	// Token: 0x060027A9 RID: 10153 RVA: 0x0000627E File Offset: 0x0000447E
	public override float GetNetworkTime()
	{
		return UnityEngine.Time.fixedTime;
	}

	// Token: 0x060027AA RID: 10154 RVA: 0x000F726F File Offset: 0x000F546F
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.GetDespawnDuration() < float.PositiveInfinity)
		{
			base.Invoke(new Action(this.IdleDestroy), this.GetDespawnDuration());
		}
		base.ReceiveCollisionMessages(true);
	}

	// Token: 0x060027AB RID: 10155 RVA: 0x000F72A3 File Offset: 0x000F54A3
	public virtual float GetDespawnDuration()
	{
		Item item = this.item;
		if (item == null)
		{
			return Server.itemdespawn;
		}
		return item.GetDespawnDuration();
	}

	// Token: 0x060027AC RID: 10156 RVA: 0x000F72BA File Offset: 0x000F54BA
	public void IdleDestroy()
	{
		Analytics.Azure.OnItemDespawn(this, this.item, (int)this.DropReason, this.DroppedBy);
		base.DestroyItem();
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060027AD RID: 10157 RVA: 0x000F72E4 File Offset: 0x000F54E4
	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		if (this.item == null)
		{
			return;
		}
		if (this.item.MaxStackable() <= 1)
		{
			return;
		}
		DroppedItem droppedItem = hitEntity as DroppedItem;
		if (droppedItem == null)
		{
			return;
		}
		if (droppedItem.item == null)
		{
			return;
		}
		if (droppedItem.item.info != this.item.info)
		{
			return;
		}
		droppedItem.OnDroppedOn(this);
	}

	// Token: 0x060027AE RID: 10158 RVA: 0x000F7348 File Offset: 0x000F5548
	public void OnDroppedOn(DroppedItem di)
	{
		if (this.item == null)
		{
			return;
		}
		if (di.item == null)
		{
			return;
		}
		if (di.item.info != this.item.info)
		{
			return;
		}
		if (di.item.IsBlueprint() && di.item.blueprintTarget != this.item.blueprintTarget)
		{
			return;
		}
		if ((di.item.hasCondition && di.item.condition != di.item.maxCondition) || (this.item.hasCondition && this.item.condition != this.item.maxCondition))
		{
			return;
		}
		if (di.item.info != null)
		{
			if (di.item.info.amountType == ItemDefinition.AmountType.Genetics)
			{
				int num = (di.item.instanceData != null) ? di.item.instanceData.dataInt : -1;
				int num2 = (this.item.instanceData != null) ? this.item.instanceData.dataInt : -1;
				if (num != num2)
				{
					return;
				}
			}
			if (di.item.info.GetComponent<ItemModSign>() != null && ItemModAssociatedEntity<SignContent>.GetAssociatedEntity(di.item, true) != null)
			{
				return;
			}
			if (this.item.info != null && this.item.info.GetComponent<ItemModSign>() != null && ItemModAssociatedEntity<SignContent>.GetAssociatedEntity(this.item, true) != null)
			{
				return;
			}
		}
		int num3 = di.item.amount + this.item.amount;
		if (num3 > this.item.MaxStackable())
		{
			return;
		}
		if (num3 == 0)
		{
			return;
		}
		if (di.DropReason == DroppedItem.DropReasonEnum.Player)
		{
			this.DropReason = DroppedItem.DropReasonEnum.Player;
		}
		di.DestroyItem();
		di.Kill(BaseNetworkable.DestroyMode.None);
		this.item.amount = num3;
		this.item.MarkDirty();
		if (this.GetDespawnDuration() < float.PositiveInfinity)
		{
			base.Invoke(new Action(this.IdleDestroy), this.GetDespawnDuration());
		}
		Effect.server.Run("assets/bundled/prefabs/fx/notice/stack.world.fx.prefab", this, 0U, Vector3.zero, Vector3.zero, null, false);
	}

	// Token: 0x060027AF RID: 10159 RVA: 0x000F7570 File Offset: 0x000F5770
	internal override void OnParentRemoved()
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component == null)
		{
			base.OnParentRemoved();
			return;
		}
		Vector3 vector = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		base.SetParent(null, false, false);
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(vector + Vector3.up * 2f, Vector3.down, out raycastHit, 2f, 27328512) && vector.y < raycastHit.point.y)
		{
			vector += Vector3.up * 1.5f;
		}
		base.transform.position = vector;
		base.transform.rotation = rotation;
		ConVar.Physics.ApplyDropped(component);
		component.isKinematic = false;
		component.useGravity = true;
		component.WakeUp();
		if (this.GetDespawnDuration() < float.PositiveInfinity)
		{
			base.Invoke(new Action(this.IdleDestroy), this.GetDespawnDuration());
		}
	}

	// Token: 0x060027B0 RID: 10160 RVA: 0x000F7664 File Offset: 0x000F5864
	public override void PostInitShared()
	{
		base.PostInitShared();
		GameObject gameObject;
		if (this.item != null && this.item.info.worldModelPrefab.isValid)
		{
			gameObject = this.item.info.worldModelPrefab.Instantiate(null);
		}
		else
		{
			gameObject = UnityEngine.Object.Instantiate<GameObject>(this.itemModel);
		}
		gameObject.transform.SetParent(base.transform, false);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.SetLayerRecursive(base.gameObject.layer);
		this.childCollider = gameObject.GetComponent<Collider>();
		if (this.childCollider)
		{
			this.childCollider.enabled = false;
			this.childCollider.enabled = true;
		}
		if (base.isServer)
		{
			WorldModel component = gameObject.GetComponent<WorldModel>();
			float mass = component ? component.mass : 1f;
			float drag = 0.1f;
			float angularDrag = 0.1f;
			Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
			rigidbody.mass = mass;
			rigidbody.drag = drag;
			rigidbody.angularDrag = angularDrag;
			rigidbody.interpolation = RigidbodyInterpolation.None;
			ConVar.Physics.ApplyDropped(rigidbody);
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}
		if (this.item != null)
		{
			PhysicsEffects component2 = base.gameObject.GetComponent<PhysicsEffects>();
			if (component2 != null)
			{
				component2.entity = this;
				if (this.item.info.physImpactSoundDef != null)
				{
					component2.physImpactSoundDef = this.item.info.physImpactSoundDef;
				}
			}
		}
		gameObject.SetActive(true);
	}

	// Token: 0x060027B1 RID: 10161 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool ShouldInheritNetworkGroup()
	{
		return false;
	}

	// Token: 0x02000D1A RID: 3354
	public enum DropReasonEnum
	{
		// Token: 0x04004631 RID: 17969
		Unknown,
		// Token: 0x04004632 RID: 17970
		Player,
		// Token: 0x04004633 RID: 17971
		Death,
		// Token: 0x04004634 RID: 17972
		Loot
	}
}
