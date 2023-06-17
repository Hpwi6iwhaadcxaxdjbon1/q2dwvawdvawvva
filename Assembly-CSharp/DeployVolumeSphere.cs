using System;
using UnityEngine;

// Token: 0x020004F9 RID: 1273
public class DeployVolumeSphere : DeployVolume
{
	// Token: 0x0400210C RID: 8460
	public Vector3 center = Vector3.zero;

	// Token: 0x0400210D RID: 8461
	public float radius = 0.5f;

	// Token: 0x06002912 RID: 10514 RVA: 0x000FC9C4 File Offset: 0x000FABC4
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.center + this.worldPosition);
		return DeployVolume.CheckSphere(position, this.radius, this.layers & mask, this);
	}

	// Token: 0x06002913 RID: 10515 RVA: 0x000FCA1C File Offset: 0x000FAC1C
	protected override bool Check(Vector3 position, Quaternion rotation, OBB obb, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.center + this.worldPosition);
		return (this.layers & mask) != 0 && Vector3.Distance(position, obb.ClosestPoint(position)) <= this.radius;
	}
}
