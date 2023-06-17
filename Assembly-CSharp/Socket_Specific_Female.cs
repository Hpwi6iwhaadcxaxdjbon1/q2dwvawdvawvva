using System;
using UnityEngine;

// Token: 0x02000281 RID: 641
public class Socket_Specific_Female : Socket_Base
{
	// Token: 0x0400159E RID: 5534
	public int rotationDegrees;

	// Token: 0x0400159F RID: 5535
	public int rotationOffset;

	// Token: 0x040015A0 RID: 5536
	public string[] allowedMaleSockets;

	// Token: 0x06001CE8 RID: 7400 RVA: 0x000C84B4 File Offset: 0x000C66B4
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

	// Token: 0x06001CE9 RID: 7401 RVA: 0x000C6C8F File Offset: 0x000C4E8F
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
	}

	// Token: 0x06001CEA RID: 7402 RVA: 0x000C8550 File Offset: 0x000C6750
	public bool CanAccept(Socket_Specific socket)
	{
		foreach (string b in this.allowedMaleSockets)
		{
			if (socket.targetSocketName == b)
			{
				return true;
			}
		}
		return false;
	}
}
