using System;
using UnityEngine;

// Token: 0x02000271 RID: 625
public class SocketMod_AngleCheck : SocketMod
{
	// Token: 0x0400155C RID: 5468
	public bool wantsAngle = true;

	// Token: 0x0400155D RID: 5469
	public Vector3 worldNormal = Vector3.up;

	// Token: 0x0400155E RID: 5470
	public float withinDegrees = 45f;

	// Token: 0x0400155F RID: 5471
	public static Translate.Phrase ErrorPhrase = new Translate.Phrase("error_anglecheck", "Invalid angle");

	// Token: 0x06001CA9 RID: 7337 RVA: 0x000C6D70 File Offset: 0x000C4F70
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.yellow;
		Gizmos.DrawFrustum(Vector3.zero, this.withinDegrees, 1f, 0f, 1f);
	}

	// Token: 0x06001CAA RID: 7338 RVA: 0x000C6DAB File Offset: 0x000C4FAB
	public override bool DoCheck(Construction.Placement place)
	{
		if (this.worldNormal.DotDegrees(place.rotation * Vector3.up) < this.withinDegrees)
		{
			return true;
		}
		Construction.lastPlacementError = SocketMod_AngleCheck.ErrorPhrase.translated;
		return false;
	}
}
