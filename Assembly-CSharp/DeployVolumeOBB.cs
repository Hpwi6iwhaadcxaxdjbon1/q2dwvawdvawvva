using System;
using UnityEngine;

// Token: 0x020004F8 RID: 1272
public class DeployVolumeOBB : DeployVolume
{
	// Token: 0x0400210B RID: 8459
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	// Token: 0x0600290F RID: 10511 RVA: 0x000FC8B8 File Offset: 0x000FAAB8
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.bounds.center + this.worldPosition);
		return DeployVolume.CheckOBB(new OBB(position, this.bounds.size, rotation * this.worldRotation), this.layers & mask, this);
	}

	// Token: 0x06002910 RID: 10512 RVA: 0x000FC92C File Offset: 0x000FAB2C
	protected override bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.bounds.center + this.worldPosition);
		OBB obb = new OBB(position, this.bounds.size, rotation * this.worldRotation);
		return (this.layers & mask) != 0 && obb.Intersects(test);
	}
}
