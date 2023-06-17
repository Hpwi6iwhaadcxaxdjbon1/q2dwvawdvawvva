using System;
using UnityEngine;

// Token: 0x020002DD RID: 733
public class QueryVis : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x0400171A RID: 5914
	public Collider checkCollider;

	// Token: 0x0400171B RID: 5915
	private CoverageQueries.Query query;

	// Token: 0x0400171C RID: 5916
	public CoverageQueries.RadiusSpace coverageRadiusSpace = CoverageQueries.RadiusSpace.World;

	// Token: 0x0400171D RID: 5917
	public float coverageRadius = 0.2f;
}
