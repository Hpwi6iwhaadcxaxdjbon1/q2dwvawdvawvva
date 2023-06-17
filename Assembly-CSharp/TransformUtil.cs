using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

// Token: 0x02000953 RID: 2387
public static class TransformUtil
{
	// Token: 0x06003959 RID: 14681 RVA: 0x0015501D File Offset: 0x0015321D
	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hit, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out hit, 100f, -1, ignoreTransform);
	}

	// Token: 0x0600395A RID: 14682 RVA: 0x00155032 File Offset: 0x00153232
	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hit, float range, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out hit, range, -1, ignoreTransform);
	}

	// Token: 0x0600395B RID: 14683 RVA: 0x00155044 File Offset: 0x00153244
	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hitOut, float range, LayerMask mask, Transform ignoreTransform = null)
	{
		startPos.y += 0.25f;
		range += 0.25f;
		hitOut = default(RaycastHit);
		RaycastHit raycastHit;
		if (!Physics.Raycast(new Ray(startPos, Vector3.down), out raycastHit, range, mask))
		{
			return false;
		}
		if (ignoreTransform != null && (raycastHit.collider.transform == ignoreTransform || raycastHit.collider.transform.IsChildOf(ignoreTransform)))
		{
			return TransformUtil.GetGroundInfo(startPos - new Vector3(0f, 0.01f, 0f), out hitOut, range, mask, ignoreTransform);
		}
		hitOut = raycastHit;
		return true;
	}

	// Token: 0x0600395C RID: 14684 RVA: 0x001550EF File Offset: 0x001532EF
	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out pos, out normal, 100f, -1, ignoreTransform);
	}

	// Token: 0x0600395D RID: 14685 RVA: 0x00155105 File Offset: 0x00153305
	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, Transform ignoreTransform = null)
	{
		return TransformUtil.GetGroundInfo(startPos, out pos, out normal, range, -1, ignoreTransform);
	}

	// Token: 0x0600395E RID: 14686 RVA: 0x00155118 File Offset: 0x00153318
	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, LayerMask mask, Transform ignoreTransform = null)
	{
		startPos.y += 0.25f;
		range += 0.25f;
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(startPos, Vector3.down), 0f, list, range, mask, QueryTriggerInteraction.Ignore, null);
		foreach (RaycastHit raycastHit in list)
		{
			if (!(ignoreTransform != null) || (!(raycastHit.collider.transform == ignoreTransform) && !raycastHit.collider.transform.IsChildOf(ignoreTransform)))
			{
				pos = raycastHit.point;
				normal = raycastHit.normal;
				Pool.FreeList<RaycastHit>(ref list);
				return true;
			}
		}
		pos = startPos;
		normal = Vector3.up;
		Pool.FreeList<RaycastHit>(ref list);
		return false;
	}

	// Token: 0x0600395F RID: 14687 RVA: 0x00155214 File Offset: 0x00153414
	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal)
	{
		return TransformUtil.GetGroundInfoTerrainOnly(startPos, out pos, out normal, 100f, -1);
	}

	// Token: 0x06003960 RID: 14688 RVA: 0x00155229 File Offset: 0x00153429
	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range)
	{
		return TransformUtil.GetGroundInfoTerrainOnly(startPos, out pos, out normal, range, -1);
	}

	// Token: 0x06003961 RID: 14689 RVA: 0x0015523C File Offset: 0x0015343C
	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, LayerMask mask)
	{
		startPos.y += 0.25f;
		range += 0.25f;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(startPos, Vector3.down), out raycastHit, range, mask) && raycastHit.collider is TerrainCollider)
		{
			pos = raycastHit.point;
			normal = raycastHit.normal;
			return true;
		}
		pos = startPos;
		normal = Vector3.up;
		return false;
	}

	// Token: 0x06003962 RID: 14690 RVA: 0x001552BB File Offset: 0x001534BB
	public static Transform[] GetRootObjects()
	{
		return (from x in UnityEngine.Object.FindObjectsOfType<Transform>()
		where x.transform == x.transform.root
		select x).ToArray<Transform>();
	}
}
