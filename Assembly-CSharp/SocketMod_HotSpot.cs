using System;
using UnityEngine;

// Token: 0x02000277 RID: 631
public class SocketMod_HotSpot : SocketMod
{
	// Token: 0x04001576 RID: 5494
	public float spotSize = 0.1f;

	// Token: 0x06001CBF RID: 7359 RVA: 0x000C76C3 File Offset: 0x000C58C3
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
		Gizmos.DrawSphere(Vector3.zero, this.spotSize);
	}

	// Token: 0x06001CC0 RID: 7360 RVA: 0x000C7704 File Offset: 0x000C5904
	public override void ModifyPlacement(Construction.Placement place)
	{
		Vector3 position = place.position + place.rotation * this.worldPosition;
		place.position = position;
	}
}
