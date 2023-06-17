using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020004F7 RID: 1271
public class DeployVolumeEntityBoundsReverse : DeployVolume
{
	// Token: 0x04002109 RID: 8457
	private Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	// Token: 0x0400210A RID: 8458
	private int layer;

	// Token: 0x0600290B RID: 10507 RVA: 0x000FC78C File Offset: 0x000FA98C
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * this.bounds.center;
		OBB test = new OBB(position, this.bounds.size, rotation);
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(position, test.extents.magnitude, list, this.layers & mask, QueryTriggerInteraction.Collide);
		foreach (BaseEntity baseEntity in list)
		{
			DeployVolume[] volumes = PrefabAttribute.server.FindAll<DeployVolume>(baseEntity.prefabID);
			if (DeployVolume.Check(baseEntity.transform.position, baseEntity.transform.rotation, volumes, test, 1 << this.layer))
			{
				Pool.FreeList<BaseEntity>(ref list);
				return true;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return false;
	}

	// Token: 0x0600290C RID: 10508 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected override bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1)
	{
		return false;
	}

	// Token: 0x0600290D RID: 10509 RVA: 0x000FC87C File Offset: 0x000FAA7C
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.bounds = rootObj.GetComponent<BaseEntity>().bounds;
		this.layer = rootObj.layer;
	}
}
