using System;
using UnityEngine;

// Token: 0x02000575 RID: 1397
public class SpaceCheckingSpawnPoint : GenericSpawnPoint
{
	// Token: 0x040022C2 RID: 8898
	public bool useCustomBoundsCheckMask;

	// Token: 0x040022C3 RID: 8899
	public LayerMask customBoundsCheckMask;

	// Token: 0x040022C4 RID: 8900
	public float customBoundsCheckScale = 1f;

	// Token: 0x06002AC1 RID: 10945 RVA: 0x00104054 File Offset: 0x00102254
	public override bool IsAvailableTo(GameObjectRef prefabRef)
	{
		if (!base.IsAvailableTo(prefabRef))
		{
			return false;
		}
		if (this.useCustomBoundsCheckMask)
		{
			return SpawnHandler.CheckBounds(prefabRef.Get(), base.transform.position, base.transform.rotation, Vector3.one * this.customBoundsCheckScale, this.customBoundsCheckMask);
		}
		return SingletonComponent<SpawnHandler>.Instance.CheckBounds(prefabRef.Get(), base.transform.position, base.transform.rotation, Vector3.one * this.customBoundsCheckScale);
	}
}
