using System;
using UnityEngine;

// Token: 0x0200026E RID: 622
public class NeighbourSocket : Socket_Base
{
	// Token: 0x06001C9E RID: 7326 RVA: 0x000C6C8F File Offset: 0x000C4E8F
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
	}

	// Token: 0x06001C9F RID: 7327 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool TestTarget(Construction.Target target)
	{
		return false;
	}

	// Token: 0x06001CA0 RID: 7328 RVA: 0x000C6CB4 File Offset: 0x000C4EB4
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
