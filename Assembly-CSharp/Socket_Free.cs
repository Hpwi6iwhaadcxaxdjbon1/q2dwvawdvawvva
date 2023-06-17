using System;
using UnityEngine;

// Token: 0x0200027F RID: 639
public class Socket_Free : Socket_Base
{
	// Token: 0x04001599 RID: 5529
	public Vector3 idealPlacementNormal = Vector3.up;

	// Token: 0x0400159A RID: 5530
	public bool useTargetNormal = true;

	// Token: 0x0400159B RID: 5531
	public bool blendAimAngle = true;

	// Token: 0x06001CE0 RID: 7392 RVA: 0x000C80DC File Offset: 0x000C62DC
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 1f);
		GizmosUtil.DrawWireCircleZ(Vector3.forward * 0f, 0.2f);
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	// Token: 0x06001CE1 RID: 7393 RVA: 0x000C814B File Offset: 0x000C634B
	public override bool TestTarget(Construction.Target target)
	{
		return target.onTerrain;
	}

	// Token: 0x06001CE2 RID: 7394 RVA: 0x000C8154 File Offset: 0x000C6354
	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		Quaternion rotation = Quaternion.identity;
		if (this.useTargetNormal)
		{
			if (this.blendAimAngle)
			{
				Vector3 vector = (target.position - target.ray.origin).normalized;
				float t = Mathf.Abs(Vector3.Dot(vector, target.normal));
				vector = Vector3.Lerp(vector, this.idealPlacementNormal, t);
				rotation = Quaternion.LookRotation(target.normal, vector) * Quaternion.Inverse(this.rotation) * Quaternion.Euler(target.rotation);
			}
			else
			{
				rotation = Quaternion.LookRotation(target.normal);
			}
		}
		else
		{
			Vector3 normalized = (target.position - target.ray.origin).normalized;
			normalized.y = 0f;
			rotation = Quaternion.LookRotation(normalized, this.idealPlacementNormal) * Quaternion.Euler(target.rotation);
		}
		Vector3 vector2 = target.position;
		vector2 -= rotation * this.position;
		return new Construction.Placement
		{
			rotation = rotation,
			position = vector2
		};
	}
}
