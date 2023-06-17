using System;
using UnityEngine;

// Token: 0x02000283 RID: 643
public class StabilitySocket : Socket_Base
{
	// Token: 0x040015A3 RID: 5539
	[Range(0f, 1f)]
	public float support = 1f;

	// Token: 0x06001CF0 RID: 7408 RVA: 0x000C6C8F File Offset: 0x000C4E8F
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
	}

	// Token: 0x06001CF1 RID: 7409 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool TestTarget(Construction.Target target)
	{
		return false;
	}

	// Token: 0x06001CF2 RID: 7410 RVA: 0x000C876C File Offset: 0x000C696C
	public override bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		if (!base.CanConnect(position, rotation, socket, socketPosition, socketRotation))
		{
			return false;
		}
		OBB selectBounds = base.GetSelectBounds(position, rotation);
		OBB selectBounds2 = socket.GetSelectBounds(socketPosition, socketRotation);
		return selectBounds.Intersects(selectBounds2);
	}
}
