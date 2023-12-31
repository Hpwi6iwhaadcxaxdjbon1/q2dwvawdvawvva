﻿using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000648 RID: 1608
public static class BoundsCheckEx
{
	// Token: 0x06002E84 RID: 11908 RVA: 0x00117930 File Offset: 0x00115B30
	public static bool ApplyBoundsChecks(this BaseEntity entity, BoundsCheck[] bounds, Vector3 pos, Quaternion rot, Vector3 scale, LayerMask rejectOnLayer)
	{
		if (bounds.Length == 0 && rejectOnLayer == 0)
		{
			return true;
		}
		OBB obb = new OBB(pos + rot * Vector3.Scale(entity.bounds.center, scale), Vector3.Scale(entity.bounds.extents, scale), rot);
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapOBB(obb, list, 262144 | rejectOnLayer, QueryTriggerInteraction.Collide);
		foreach (Collider collider in list)
		{
			if (!collider.isTrigger && (collider.gameObject.layer & rejectOnLayer) != 0)
			{
				return false;
			}
			SpawnableBoundsBlocker spawnableBoundsBlocker;
			if (collider.TryGetComponent<SpawnableBoundsBlocker>(out spawnableBoundsBlocker))
			{
				foreach (BoundsCheck boundsCheck in bounds)
				{
					if (spawnableBoundsBlocker.BlockType == boundsCheck.IsType)
					{
						Pool.FreeList<Collider>(ref list);
						return false;
					}
				}
			}
		}
		Pool.FreeList<Collider>(ref list);
		return true;
	}
}
