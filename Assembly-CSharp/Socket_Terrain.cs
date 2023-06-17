using System;
using UnityEngine;

// Token: 0x02000282 RID: 642
public class Socket_Terrain : Socket_Base
{
	// Token: 0x040015A1 RID: 5537
	public float placementHeight;

	// Token: 0x040015A2 RID: 5538
	public bool alignToNormal;

	// Token: 0x06001CEC RID: 7404 RVA: 0x000C8588 File Offset: 0x000C6788
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.2f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 0.1f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
		Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
		Gizmos.DrawCube(Vector3.zero, new Vector3(0.1f, 0.1f, this.placementHeight));
		Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.1f, 0.1f, this.placementHeight));
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	// Token: 0x06001CED RID: 7405 RVA: 0x000C814B File Offset: 0x000C634B
	public override bool TestTarget(Construction.Target target)
	{
		return target.onTerrain;
	}

	// Token: 0x06001CEE RID: 7406 RVA: 0x000C86A0 File Offset: 0x000C68A0
	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		Vector3 eulerAngles = this.rotation.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		Vector3 direction = target.ray.direction;
		direction.y = 0f;
		direction.Normalize();
		Vector3 upwards = Vector3.up;
		if (this.alignToNormal)
		{
			upwards = target.normal;
		}
		Quaternion rotation = Quaternion.LookRotation(direction, upwards) * Quaternion.Euler(0f, eulerAngles.y, 0f) * Quaternion.Euler(target.rotation);
		Vector3 vector = target.position;
		vector -= rotation * this.position;
		return new Construction.Placement
		{
			rotation = rotation,
			position = vector
		};
	}
}
