using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200024B RID: 587
public class SoundSource : MonoBehaviour, IClientComponentEx, ILOD
{
	// Token: 0x040014D0 RID: 5328
	[Header("Occlusion")]
	public bool handleOcclusionChecks;

	// Token: 0x040014D1 RID: 5329
	public LayerMask occlusionLayerMask;

	// Token: 0x040014D2 RID: 5330
	public List<SoundSource.OcclusionPoint> occlusionPoints = new List<SoundSource.OcclusionPoint>();

	// Token: 0x040014D3 RID: 5331
	public bool isOccluded;

	// Token: 0x040014D4 RID: 5332
	public float occlusionAmount;

	// Token: 0x040014D5 RID: 5333
	public float lodDistance = 100f;

	// Token: 0x040014D6 RID: 5334
	public bool inRange;

	// Token: 0x06001C1F RID: 7199 RVA: 0x000C42D2 File Offset: 0x000C24D2
	public virtual void PreClientComponentCull(IPrefabProcessor p)
	{
		p.RemoveComponent(this);
	}

	// Token: 0x06001C20 RID: 7200 RVA: 0x00007A3C File Offset: 0x00005C3C
	public bool IsSyncedToParent()
	{
		return false;
	}

	// Token: 0x02000C80 RID: 3200
	[Serializable]
	public class OcclusionPoint
	{
		// Token: 0x04004388 RID: 17288
		public Vector3 offset = Vector3.zero;

		// Token: 0x04004389 RID: 17289
		public bool isOccluded;
	}
}
