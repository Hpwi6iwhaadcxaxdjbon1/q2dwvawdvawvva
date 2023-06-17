using System;
using UnityEngine;

// Token: 0x020004F5 RID: 1269
public class DeployVolumeCapsule : DeployVolume
{
	// Token: 0x04002105 RID: 8453
	public Vector3 center = Vector3.zero;

	// Token: 0x04002106 RID: 8454
	public float radius = 0.5f;

	// Token: 0x04002107 RID: 8455
	public float height = 1f;

	// Token: 0x06002904 RID: 10500 RVA: 0x000FC628 File Offset: 0x000FA828
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.center + this.worldPosition);
		Vector3 start = position + rotation * this.worldRotation * Vector3.up * this.height * 0.5f;
		Vector3 end = position + rotation * this.worldRotation * Vector3.down * this.height * 0.5f;
		return DeployVolume.CheckCapsule(start, end, this.radius, this.layers & mask, this);
	}

	// Token: 0x06002905 RID: 10501 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected override bool Check(Vector3 position, Quaternion rotation, OBB obb, int mask = -1)
	{
		return false;
	}
}
