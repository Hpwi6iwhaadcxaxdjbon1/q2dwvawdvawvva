using System;
using UnityEngine;

// Token: 0x0200026F RID: 623
public class SocketHandle : PrefabAttribute
{
	// Token: 0x06001CA2 RID: 7330 RVA: 0x000C6CF6 File Offset: 0x000C4EF6
	protected override Type GetIndexedType()
	{
		return typeof(SocketHandle);
	}

	// Token: 0x06001CA3 RID: 7331 RVA: 0x000C6D04 File Offset: 0x000C4F04
	internal void AdjustTarget(ref Construction.Target target, float maxplaceDistance)
	{
		Vector3 worldPosition = this.worldPosition;
		Vector3 a = target.ray.origin + target.ray.direction * maxplaceDistance - worldPosition;
		target.ray.direction = (a - target.ray.origin).normalized;
	}
}
