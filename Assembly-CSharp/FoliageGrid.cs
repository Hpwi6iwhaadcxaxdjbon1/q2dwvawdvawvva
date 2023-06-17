using System;
using UnityEngine.Rendering;

// Token: 0x02000509 RID: 1289
public class FoliageGrid : SingletonComponent<FoliageGrid>, IClientComponent
{
	// Token: 0x04002136 RID: 8502
	public static bool Paused;

	// Token: 0x04002137 RID: 8503
	public GameObjectRef BatchPrefab;

	// Token: 0x04002138 RID: 8504
	public float CellSize = 50f;

	// Token: 0x04002139 RID: 8505
	public LayerSelect FoliageLayer = 0;

	// Token: 0x0400213A RID: 8506
	public ShadowCastingMode FoliageShadows;
}
