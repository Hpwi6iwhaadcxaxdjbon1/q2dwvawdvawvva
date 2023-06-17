using System;
using UnityEngine;

// Token: 0x02000495 RID: 1173
public class MagnetSnap
{
	// Token: 0x04001EDF RID: 7903
	private Transform snapLocation;

	// Token: 0x04001EE0 RID: 7904
	private Vector3 prevSnapLocation;

	// Token: 0x06002693 RID: 9875 RVA: 0x000F20ED File Offset: 0x000F02ED
	public MagnetSnap(Transform snapLocation)
	{
		this.snapLocation = snapLocation;
		this.prevSnapLocation = snapLocation.position;
	}

	// Token: 0x06002694 RID: 9876 RVA: 0x000F2108 File Offset: 0x000F0308
	public void FixedUpdate(Transform target)
	{
		this.PositionTarget(target);
		if (this.snapLocation.hasChanged)
		{
			this.prevSnapLocation = this.snapLocation.position;
			this.snapLocation.hasChanged = false;
		}
	}

	// Token: 0x06002695 RID: 9877 RVA: 0x000F213C File Offset: 0x000F033C
	public void PositionTarget(Transform target)
	{
		if (target == null)
		{
			return;
		}
		Transform transform = target.transform;
		Quaternion quaternion = this.snapLocation.rotation;
		if (Vector3.Angle(transform.forward, this.snapLocation.forward) > 90f)
		{
			quaternion *= Quaternion.Euler(0f, 180f, 0f);
		}
		if (transform.position != this.snapLocation.position)
		{
			transform.position += this.snapLocation.position - this.prevSnapLocation;
			transform.position = Vector3.MoveTowards(transform.position, this.snapLocation.position, 1f * Time.fixedDeltaTime);
		}
		if (transform.rotation != quaternion)
		{
			transform.rotation = Quaternion.RotateTowards(transform.rotation, quaternion, 40f * Time.fixedDeltaTime);
		}
	}
}
