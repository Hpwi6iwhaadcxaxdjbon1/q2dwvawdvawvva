using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020004B8 RID: 1208
public class TriggerTrainCollisions : TriggerBase
{
	// Token: 0x04002004 RID: 8196
	public Collider triggerCollider;

	// Token: 0x04002005 RID: 8197
	public TriggerTrainCollisions.Location location;

	// Token: 0x04002006 RID: 8198
	public TrainCar owner;

	// Token: 0x04002007 RID: 8199
	[NonSerialized]
	public HashSet<GameObject> staticContents = new HashSet<GameObject>();

	// Token: 0x04002008 RID: 8200
	[NonSerialized]
	public HashSet<TrainCar> trainContents = new HashSet<TrainCar>();

	// Token: 0x04002009 RID: 8201
	[NonSerialized]
	public HashSet<Rigidbody> otherRigidbodyContents = new HashSet<Rigidbody>();

	// Token: 0x0400200A RID: 8202
	[NonSerialized]
	public HashSet<Collider> colliderContents = new HashSet<Collider>();

	// Token: 0x0400200B RID: 8203
	private const float TICK_RATE = 0.2f;

	// Token: 0x17000358 RID: 856
	// (get) Token: 0x0600277E RID: 10110 RVA: 0x000F692D File Offset: 0x000F4B2D
	public bool HasAnyStaticContents
	{
		get
		{
			return this.staticContents.Count > 0;
		}
	}

	// Token: 0x17000359 RID: 857
	// (get) Token: 0x0600277F RID: 10111 RVA: 0x000F693D File Offset: 0x000F4B3D
	public bool HasAnyTrainContents
	{
		get
		{
			return this.trainContents.Count > 0;
		}
	}

	// Token: 0x1700035A RID: 858
	// (get) Token: 0x06002780 RID: 10112 RVA: 0x000F694D File Offset: 0x000F4B4D
	public bool HasAnyOtherRigidbodyContents
	{
		get
		{
			return this.otherRigidbodyContents.Count > 0;
		}
	}

	// Token: 0x1700035B RID: 859
	// (get) Token: 0x06002781 RID: 10113 RVA: 0x000F695D File Offset: 0x000F4B5D
	public bool HasAnyNonStaticContents
	{
		get
		{
			return this.HasAnyTrainContents || this.HasAnyOtherRigidbodyContents;
		}
	}

	// Token: 0x06002782 RID: 10114 RVA: 0x000F6970 File Offset: 0x000F4B70
	internal override void OnObjectAdded(GameObject obj, Collider col)
	{
		if (!this.owner.isServer)
		{
			return;
		}
		base.OnObjectAdded(obj, col);
		if (obj != null)
		{
			BaseEntity baseEntity = obj.ToBaseEntity();
			if (baseEntity != null)
			{
				Vector3 a = baseEntity.transform.position + baseEntity.transform.rotation * Vector3.Scale(obj.transform.lossyScale, baseEntity.bounds.center);
				Vector3 center = this.triggerCollider.bounds.center;
				Vector3 rhs = a - center;
				bool flag = Vector3.Dot(this.owner.transform.forward, rhs) > 0f;
				if (this.location == TriggerTrainCollisions.Location.Front && !flag)
				{
					return;
				}
				if (this.location == TriggerTrainCollisions.Location.Rear && flag)
				{
					return;
				}
			}
		}
		if (obj != null)
		{
			Rigidbody componentInParent = obj.GetComponentInParent<Rigidbody>();
			if (componentInParent != null)
			{
				TrainCar componentInParent2 = obj.GetComponentInParent<TrainCar>();
				if (componentInParent2 != null)
				{
					this.trainContents.Add(componentInParent2);
					if (this.owner.coupling != null)
					{
						this.owner.coupling.TryCouple(componentInParent2, this.location);
					}
					base.InvokeRepeating(new Action(this.TrainContentsTick), 0.2f, 0.2f);
				}
				else
				{
					this.otherRigidbodyContents.Add(componentInParent);
				}
			}
			else
			{
				ITrainCollidable componentInParent3 = obj.GetComponentInParent<ITrainCollidable>();
				if (componentInParent3 == null)
				{
					if (!obj.CompareTag("Railway"))
					{
						this.staticContents.Add(obj);
					}
				}
				else if (!componentInParent3.EqualNetID(this.owner) && !componentInParent3.CustomCollision(this.owner, this))
				{
					this.staticContents.Add(obj);
				}
			}
		}
		if (col != null)
		{
			this.colliderContents.Add(col);
		}
	}

	// Token: 0x06002783 RID: 10115 RVA: 0x000F6B40 File Offset: 0x000F4D40
	internal override void OnObjectRemoved(GameObject obj)
	{
		if (!this.owner.isServer)
		{
			return;
		}
		if (obj == null)
		{
			return;
		}
		foreach (Collider item in obj.GetComponents<Collider>())
		{
			this.colliderContents.Remove(item);
		}
		if (!this.staticContents.Remove(obj))
		{
			TrainCar componentInParent = obj.GetComponentInParent<TrainCar>();
			if (componentInParent != null)
			{
				if (!this.<OnObjectRemoved>g__HasAnotherColliderFor|18_0<TrainCar>(componentInParent))
				{
					this.trainContents.Remove(componentInParent);
					if (this.trainContents == null || this.trainContents.Count == 0)
					{
						base.CancelInvoke(new Action(this.TrainContentsTick));
					}
				}
			}
			else
			{
				Rigidbody componentInParent2 = obj.GetComponentInParent<Rigidbody>();
				if (!this.<OnObjectRemoved>g__HasAnotherColliderFor|18_0<Rigidbody>(componentInParent2))
				{
					this.otherRigidbodyContents.Remove(componentInParent2);
				}
			}
		}
		base.OnObjectRemoved(obj);
	}

	// Token: 0x06002784 RID: 10116 RVA: 0x000F6C10 File Offset: 0x000F4E10
	private void TrainContentsTick()
	{
		if (this.trainContents == null)
		{
			return;
		}
		foreach (TrainCar trainCar in this.trainContents)
		{
			if (trainCar.IsValid() && !trainCar.IsDestroyed && this.owner.coupling != null)
			{
				this.owner.coupling.TryCouple(trainCar, this.location);
			}
		}
	}

	// Token: 0x06002786 RID: 10118 RVA: 0x000F6CD0 File Offset: 0x000F4ED0
	[CompilerGenerated]
	private bool <OnObjectRemoved>g__HasAnotherColliderFor|18_0<T>(T component) where T : Component
	{
		foreach (Collider collider in this.colliderContents)
		{
			if (collider != null && collider.GetComponentInParent<T>() == component)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x02000D16 RID: 3350
	public enum Location
	{
		// Token: 0x04004623 RID: 17955
		Front,
		// Token: 0x04004624 RID: 17956
		Rear
	}
}
