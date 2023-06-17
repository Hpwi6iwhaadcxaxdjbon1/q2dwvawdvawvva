using System;

// Token: 0x02000175 RID: 373
public class CoverageQueryFlare : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x04001058 RID: 4184
	public bool isDynamic;

	// Token: 0x04001059 RID: 4185
	public bool timeShimmer;

	// Token: 0x0400105A RID: 4186
	public bool positionalShimmer;

	// Token: 0x0400105B RID: 4187
	public bool rotate;

	// Token: 0x0400105C RID: 4188
	public float maxVisibleDistance = 30f;

	// Token: 0x0400105D RID: 4189
	public bool lightScaled;

	// Token: 0x0400105E RID: 4190
	public float dotMin = -1f;

	// Token: 0x0400105F RID: 4191
	public float dotMax = -1f;

	// Token: 0x04001060 RID: 4192
	public CoverageQueries.RadiusSpace coverageRadiusSpace;

	// Token: 0x04001061 RID: 4193
	public float coverageRadius = 0.01f;

	// Token: 0x04001062 RID: 4194
	public LODDistanceMode DistanceMode;
}
