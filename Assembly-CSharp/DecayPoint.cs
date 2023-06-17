using System;
using UnityEngine;

// Token: 0x0200025B RID: 603
public class DecayPoint : PrefabAttribute
{
	// Token: 0x04001514 RID: 5396
	[Tooltip("If this point is occupied this will take this % off the power of the decay")]
	public float protection = 0.25f;

	// Token: 0x04001515 RID: 5397
	public Socket_Base socket;

	// Token: 0x06001C5B RID: 7259 RVA: 0x000C59C5 File Offset: 0x000C3BC5
	public bool IsOccupied(BaseEntity entity)
	{
		return entity.IsOccupied(this.socket);
	}

	// Token: 0x06001C5C RID: 7260 RVA: 0x000C59D3 File Offset: 0x000C3BD3
	protected override Type GetIndexedType()
	{
		return typeof(DecayPoint);
	}
}
