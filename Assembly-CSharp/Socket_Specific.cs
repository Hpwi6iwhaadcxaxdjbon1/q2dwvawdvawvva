using System;
using UnityEngine;

// Token: 0x02000280 RID: 640
public class Socket_Specific : Socket_Base
{
	// Token: 0x0400159C RID: 5532
	public bool useFemaleRotation = true;

	// Token: 0x0400159D RID: 5533
	public string targetSocketName;

	// Token: 0x06001CE4 RID: 7396 RVA: 0x000C8294 File Offset: 0x000C6494
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.2f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 0.1f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	// Token: 0x06001CE5 RID: 7397 RVA: 0x000C8330 File Offset: 0x000C6530
	public override bool TestTarget(Construction.Target target)
	{
		if (!base.TestTarget(target))
		{
			return false;
		}
		Socket_Specific_Female socket_Specific_Female = target.socket as Socket_Specific_Female;
		return !(socket_Specific_Female == null) && socket_Specific_Female.CanAccept(this);
	}

	// Token: 0x06001CE6 RID: 7398 RVA: 0x000C8368 File Offset: 0x000C6568
	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		Quaternion rhs = target.socket.rotation;
		if (target.socket.male && target.socket.female)
		{
			rhs = target.socket.rotation * Quaternion.Euler(180f, 0f, 180f);
		}
		Transform transform = target.entity.transform;
		Vector3 vector = transform.localToWorldMatrix.MultiplyPoint3x4(target.socket.position);
		Quaternion lhs;
		if (this.useFemaleRotation)
		{
			lhs = transform.rotation * rhs;
		}
		else
		{
			Vector3 a = new Vector3(vector.x, 0f, vector.z);
			Vector3 b = new Vector3(target.player.eyes.position.x, 0f, target.player.eyes.position.z);
			lhs = Quaternion.LookRotation((a - b).normalized) * rhs;
		}
		Construction.Placement placement = new Construction.Placement();
		Quaternion rotation = lhs * Quaternion.Inverse(this.rotation);
		Vector3 b2 = rotation * this.position;
		placement.position = vector - b2;
		placement.rotation = rotation;
		return placement;
	}
}
