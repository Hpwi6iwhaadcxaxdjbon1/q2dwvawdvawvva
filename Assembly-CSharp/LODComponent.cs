using System;
using UnityEngine;

// Token: 0x02000534 RID: 1332
public abstract class LODComponent : BaseMonoBehaviour, IClientComponent, ILOD
{
	// Token: 0x040021F4 RID: 8692
	public LODDistanceMode DistanceMode;

	// Token: 0x040021F5 RID: 8693
	public LODComponent.OccludeeParameters OccludeeParams = new LODComponent.OccludeeParameters
	{
		isDynamic = false,
		dynamicUpdateInterval = 0.2f,
		shadowRangeScale = 3f,
		showBounds = false
	};

	// Token: 0x02000D3D RID: 3389
	[Serializable]
	public struct OccludeeParameters
	{
		// Token: 0x040046AF RID: 18095
		[Tooltip("Is Occludee dynamic or static?")]
		public bool isDynamic;

		// Token: 0x040046B0 RID: 18096
		[Tooltip("Dynamic occludee update interval in seconds; 0 = every frame")]
		public float dynamicUpdateInterval;

		// Token: 0x040046B1 RID: 18097
		[Tooltip("Distance scale combined with occludee max bounds size at which culled occludee shadows are still visible")]
		public float shadowRangeScale;

		// Token: 0x040046B2 RID: 18098
		[Tooltip("Show culling bounds via gizmos; editor only")]
		public bool showBounds;
	}
}
