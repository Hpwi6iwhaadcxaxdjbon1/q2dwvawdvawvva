using System;
using Rust;
using UnityEngine;

// Token: 0x02000924 RID: 2340
public static class RaycastHitEx
{
	// Token: 0x0600384D RID: 14413 RVA: 0x001500A9 File Offset: 0x0014E2A9
	public static Transform GetTransform(this RaycastHit hit)
	{
		return hit.transform;
	}

	// Token: 0x0600384E RID: 14414 RVA: 0x001500B2 File Offset: 0x0014E2B2
	public static Rigidbody GetRigidbody(this RaycastHit hit)
	{
		return hit.rigidbody;
	}

	// Token: 0x0600384F RID: 14415 RVA: 0x001500BB File Offset: 0x0014E2BB
	public static Collider GetCollider(this RaycastHit hit)
	{
		return hit.collider;
	}

	// Token: 0x06003850 RID: 14416 RVA: 0x001500C4 File Offset: 0x0014E2C4
	public static BaseEntity GetEntity(this RaycastHit hit)
	{
		return hit.collider.ToBaseEntity();
	}

	// Token: 0x06003851 RID: 14417 RVA: 0x001500D2 File Offset: 0x0014E2D2
	public static bool IsOnLayer(this RaycastHit hit, Layer rustLayer)
	{
		return hit.collider != null && hit.collider.gameObject.IsOnLayer(rustLayer);
	}

	// Token: 0x06003852 RID: 14418 RVA: 0x001500F7 File Offset: 0x0014E2F7
	public static bool IsOnLayer(this RaycastHit hit, int layer)
	{
		return hit.collider != null && hit.collider.gameObject.IsOnLayer(layer);
	}
}
