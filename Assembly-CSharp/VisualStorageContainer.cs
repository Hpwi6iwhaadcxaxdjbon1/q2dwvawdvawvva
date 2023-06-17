using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000429 RID: 1065
public class VisualStorageContainer : LootContainer
{
	// Token: 0x04001C14 RID: 7188
	public VisualStorageContainerNode[] displayNodes;

	// Token: 0x04001C15 RID: 7189
	public VisualStorageContainer.DisplayModel[] displayModels;

	// Token: 0x04001C16 RID: 7190
	public Transform nodeParent;

	// Token: 0x04001C17 RID: 7191
	public GameObject defaultDisplayModel;

	// Token: 0x060023F2 RID: 9202 RVA: 0x000E59B8 File Offset: 0x000E3BB8
	public override void ServerInit()
	{
		base.ServerInit();
	}

	// Token: 0x060023F3 RID: 9203 RVA: 0x000A3CB9 File Offset: 0x000A1EB9
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
	}

	// Token: 0x060023F4 RID: 9204 RVA: 0x000E59C0 File Offset: 0x000E3BC0
	public override void PopulateLoot()
	{
		base.PopulateLoot();
		for (int i = 0; i < this.inventorySlots; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot != null)
			{
				DroppedItem component = slot.Drop(this.displayNodes[i].transform.position + new Vector3(0f, 0.25f, 0f), Vector3.zero, this.displayNodes[i].transform.rotation).GetComponent<DroppedItem>();
				if (component)
				{
					base.ReceiveCollisionMessages(false);
					base.CancelInvoke(new Action(component.IdleDestroy));
					Rigidbody componentInChildren = component.GetComponentInChildren<Rigidbody>();
					if (componentInChildren)
					{
						componentInChildren.constraints = (RigidbodyConstraints)10;
					}
				}
			}
		}
	}

	// Token: 0x060023F5 RID: 9205 RVA: 0x000E5A84 File Offset: 0x000E3C84
	public void ClearRigidBodies()
	{
		if (this.displayModels == null)
		{
			return;
		}
		foreach (VisualStorageContainer.DisplayModel displayModel in this.displayModels)
		{
			if (displayModel != null)
			{
				UnityEngine.Object.Destroy(displayModel.displayModel.GetComponentInChildren<Rigidbody>());
			}
		}
	}

	// Token: 0x060023F6 RID: 9206 RVA: 0x000E5AC8 File Offset: 0x000E3CC8
	public void SetItemsVisible(bool vis)
	{
		if (this.displayModels == null)
		{
			return;
		}
		foreach (VisualStorageContainer.DisplayModel displayModel in this.displayModels)
		{
			if (displayModel != null)
			{
				LODGroup componentInChildren = displayModel.displayModel.GetComponentInChildren<LODGroup>();
				if (componentInChildren)
				{
					componentInChildren.localReferencePoint = (vis ? Vector3.zero : new Vector3(10000f, 10000f, 10000f));
				}
				else
				{
					Debug.Log("VisualStorageContainer item missing LODGroup" + displayModel.displayModel.gameObject.name);
				}
			}
		}
	}

	// Token: 0x060023F7 RID: 9207 RVA: 0x000E5B53 File Offset: 0x000E3D53
	public void ItemUpdateComplete()
	{
		this.ClearRigidBodies();
		this.SetItemsVisible(true);
	}

	// Token: 0x060023F8 RID: 9208 RVA: 0x000E5B64 File Offset: 0x000E3D64
	public void UpdateVisibleItems(ProtoBuf.ItemContainer msg)
	{
		for (int i = 0; i < this.displayModels.Length; i++)
		{
			VisualStorageContainer.DisplayModel displayModel = this.displayModels[i];
			if (displayModel != null)
			{
				UnityEngine.Object.Destroy(displayModel.displayModel);
				this.displayModels[i] = null;
			}
		}
		if (msg == null)
		{
			return;
		}
		foreach (ProtoBuf.Item item in msg.contents)
		{
			ItemDefinition itemDefinition = ItemManager.FindItemDefinition(item.itemid);
			GameObject gameObject;
			if (itemDefinition.worldModelPrefab != null && itemDefinition.worldModelPrefab.isValid)
			{
				gameObject = itemDefinition.worldModelPrefab.Instantiate(null);
			}
			else
			{
				gameObject = UnityEngine.Object.Instantiate<GameObject>(this.defaultDisplayModel);
			}
			if (gameObject)
			{
				gameObject.transform.SetPositionAndRotation(this.displayNodes[item.slot].transform.position + new Vector3(0f, 0.25f, 0f), this.displayNodes[item.slot].transform.rotation);
				Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
				rigidbody.mass = 1f;
				rigidbody.drag = 0.1f;
				rigidbody.angularDrag = 0.1f;
				rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
				rigidbody.constraints = (RigidbodyConstraints)10;
				this.displayModels[item.slot].displayModel = gameObject;
				this.displayModels[item.slot].slot = item.slot;
				this.displayModels[item.slot].def = itemDefinition;
				gameObject.SetActive(true);
			}
		}
		this.SetItemsVisible(false);
		base.CancelInvoke(new Action(this.ItemUpdateComplete));
		base.Invoke(new Action(this.ItemUpdateComplete), 1f);
	}

	// Token: 0x02000CDD RID: 3293
	[Serializable]
	public class DisplayModel
	{
		// Token: 0x0400453A RID: 17722
		public GameObject displayModel;

		// Token: 0x0400453B RID: 17723
		public ItemDefinition def;

		// Token: 0x0400453C RID: 17724
		public int slot;
	}
}
