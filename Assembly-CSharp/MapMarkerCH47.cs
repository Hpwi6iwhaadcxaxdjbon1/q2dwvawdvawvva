using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x020001B7 RID: 439
public class MapMarkerCH47 : MapMarker
{
	// Token: 0x0400119D RID: 4509
	private GameObject createdMarker;

	// Token: 0x060018EF RID: 6383 RVA: 0x000B86E8 File Offset: 0x000B68E8
	private float GetRotation()
	{
		global::BaseEntity parentEntity = base.GetParentEntity();
		if (!parentEntity)
		{
			return 0f;
		}
		Vector3 forward = parentEntity.transform.forward;
		forward.y = 0f;
		forward.Normalize();
		return Mathf.Atan2(forward.x, -forward.z) * 57.29578f + 180f;
	}

	// Token: 0x060018F0 RID: 6384 RVA: 0x000B8747 File Offset: 0x000B6947
	public override AppMarker GetAppMarkerData()
	{
		AppMarker appMarkerData = base.GetAppMarkerData();
		appMarkerData.rotation = this.GetRotation();
		return appMarkerData;
	}
}
