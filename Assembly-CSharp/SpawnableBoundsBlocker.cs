using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000649 RID: 1609
public class SpawnableBoundsBlocker : MonoBehaviour
{
	// Token: 0x04002645 RID: 9797
	public BoundsCheck.BlockType BlockType;

	// Token: 0x04002646 RID: 9798
	public BoxCollider BoxCollider;

	// Token: 0x06002E85 RID: 11909 RVA: 0x00117A40 File Offset: 0x00115C40
	[Button("Clear Trees")]
	public void ClearTrees()
	{
		List<TreeEntity> list = Pool.GetList<TreeEntity>();
		if (this.BoxCollider != null)
		{
			GamePhysics.OverlapOBB<TreeEntity>(new OBB(base.transform.TransformPoint(this.BoxCollider.center), this.BoxCollider.size + Vector3.one, base.transform.rotation), list, 1073741824, QueryTriggerInteraction.Collide);
		}
		foreach (TreeEntity treeEntity in list)
		{
			BoundsCheck boundsCheck = PrefabAttribute.server.Find<BoundsCheck>(treeEntity.prefabID);
			if (boundsCheck != null && boundsCheck.IsType == this.BlockType)
			{
				treeEntity.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
		Pool.FreeList<TreeEntity>(ref list);
	}
}
