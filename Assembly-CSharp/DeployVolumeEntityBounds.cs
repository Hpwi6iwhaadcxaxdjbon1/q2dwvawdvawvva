using System;
using UnityEngine;

// Token: 0x020004F6 RID: 1270
public class DeployVolumeEntityBounds : DeployVolume
{
	// Token: 0x04002108 RID: 8456
	private Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	// Token: 0x06002907 RID: 10503 RVA: 0x000FC70C File Offset: 0x000FA90C
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * this.bounds.center;
		return DeployVolume.CheckOBB(new OBB(position, this.bounds.size, rotation), this.layers & mask, this);
	}

	// Token: 0x06002908 RID: 10504 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected override bool Check(Vector3 position, Quaternion rotation, OBB obb, int mask = -1)
	{
		return false;
	}

	// Token: 0x06002909 RID: 10505 RVA: 0x000FC75C File Offset: 0x000FA95C
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.bounds = rootObj.GetComponent<BaseEntity>().bounds;
	}
}
