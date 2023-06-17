using System;
using UnityEngine;

// Token: 0x0200048D RID: 1165
public class BaseMagnet : MonoBehaviour
{
	// Token: 0x04001E95 RID: 7829
	public BaseEntity entityOwner;

	// Token: 0x04001E96 RID: 7830
	public BaseEntity.Flags magnetFlag = BaseEntity.Flags.Reserved6;

	// Token: 0x04001E97 RID: 7831
	public TriggerMagnet magnetTrigger;

	// Token: 0x04001E98 RID: 7832
	public FixedJoint fixedJoint;

	// Token: 0x04001E99 RID: 7833
	public Rigidbody kinematicAttachmentBody;

	// Token: 0x04001E9A RID: 7834
	public float magnetForce;

	// Token: 0x04001E9B RID: 7835
	public Transform attachDepthPoint;

	// Token: 0x04001E9C RID: 7836
	public GameObjectRef attachEffect;

	// Token: 0x04001E9D RID: 7837
	public bool isMagnetOn;

	// Token: 0x04001E9E RID: 7838
	public GameObject colliderSource;

	// Token: 0x04001E9F RID: 7839
	private BasePlayer associatedPlayer;

	// Token: 0x06002644 RID: 9796 RVA: 0x000F1024 File Offset: 0x000EF224
	public bool HasConnectedObject()
	{
		return this.fixedJoint.connectedBody != null && this.isMagnetOn;
	}

	// Token: 0x06002645 RID: 9797 RVA: 0x000F1044 File Offset: 0x000EF244
	public OBB GetConnectedOBB(float scale = 1f)
	{
		if (this.fixedJoint.connectedBody == null)
		{
			Debug.LogError("BaseMagnet returning fake OBB because no connected body!");
			return new OBB(Vector3.zero, Vector3.one, Quaternion.identity);
		}
		BaseEntity component = this.fixedJoint.connectedBody.gameObject.GetComponent<BaseEntity>();
		Bounds bounds = component.bounds;
		bounds.extents *= scale;
		return new OBB(component.transform.position, component.transform.rotation, bounds);
	}

	// Token: 0x06002646 RID: 9798 RVA: 0x000F10D0 File Offset: 0x000EF2D0
	public void SetCollisionsEnabled(GameObject other, bool wants)
	{
		Collider[] componentsInChildren = other.GetComponentsInChildren<Collider>();
		Collider[] componentsInChildren2 = this.colliderSource.GetComponentsInChildren<Collider>();
		foreach (Collider collider in componentsInChildren)
		{
			foreach (Collider collider2 in componentsInChildren2)
			{
				Physics.IgnoreCollision(collider, collider2, !wants);
			}
		}
	}

	// Token: 0x06002647 RID: 9799 RVA: 0x000F112C File Offset: 0x000EF32C
	public virtual void SetMagnetEnabled(bool wantsOn, BasePlayer forPlayer)
	{
		if (this.isMagnetOn == wantsOn)
		{
			return;
		}
		this.associatedPlayer = forPlayer;
		this.isMagnetOn = wantsOn;
		if (this.isMagnetOn)
		{
			this.OnMagnetEnabled();
		}
		else
		{
			this.OnMagnetDisabled();
		}
		if (this.entityOwner != null)
		{
			this.entityOwner.SetFlag(this.magnetFlag, this.isMagnetOn, false, true);
		}
	}

	// Token: 0x06002648 RID: 9800 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnMagnetEnabled()
	{
	}

	// Token: 0x06002649 RID: 9801 RVA: 0x000F1190 File Offset: 0x000EF390
	public virtual void OnMagnetDisabled()
	{
		if (this.fixedJoint.connectedBody)
		{
			this.SetCollisionsEnabled(this.fixedJoint.connectedBody.gameObject, true);
			Rigidbody connectedBody = this.fixedJoint.connectedBody;
			this.fixedJoint.connectedBody = null;
			connectedBody.WakeUp();
		}
	}

	// Token: 0x0600264A RID: 9802 RVA: 0x000F11E2 File Offset: 0x000EF3E2
	public bool IsMagnetOn()
	{
		return this.isMagnetOn;
	}

	// Token: 0x0600264B RID: 9803 RVA: 0x000F11EC File Offset: 0x000EF3EC
	public void MagnetThink(float delta)
	{
		if (!this.isMagnetOn)
		{
			return;
		}
		Vector3 position = this.magnetTrigger.transform.position;
		if (this.magnetTrigger.entityContents != null)
		{
			foreach (BaseEntity baseEntity in this.magnetTrigger.entityContents)
			{
				if (baseEntity.syncPosition)
				{
					Rigidbody component = baseEntity.GetComponent<Rigidbody>();
					if (!(component == null) && !component.isKinematic && !baseEntity.isClient)
					{
						OBB obb = new OBB(baseEntity.transform.position, baseEntity.transform.rotation, baseEntity.bounds);
						if (obb.Contains(this.attachDepthPoint.position))
						{
							baseEntity.GetComponent<MagnetLiftable>().SetMagnetized(true, this, this.associatedPlayer);
							if (this.fixedJoint.connectedBody == null)
							{
								Effect.server.Run(this.attachEffect.resourcePath, this.attachDepthPoint.position, -this.attachDepthPoint.up, null, false);
								this.fixedJoint.connectedBody = component;
								this.SetCollisionsEnabled(component.gameObject, false);
								continue;
							}
						}
						if (this.fixedJoint.connectedBody == null)
						{
							Vector3 position2 = baseEntity.transform.position;
							float b = Vector3.Distance(position2, position);
							Vector3 a = Vector3Ex.Direction(position, position2);
							float d = 1f / Mathf.Max(1f, b);
							component.AddForce(a * this.magnetForce * d, ForceMode.Acceleration);
						}
					}
				}
			}
		}
	}
}
