using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000544 RID: 1348
public class RendererLOD : LODComponent, IBatchingHandler
{
	// Token: 0x04002214 RID: 8724
	public RendererLOD.State[] States;

	// Token: 0x02000D42 RID: 3394
	[Serializable]
	public class State
	{
		// Token: 0x040046BE RID: 18110
		public float distance;

		// Token: 0x040046BF RID: 18111
		public Renderer renderer;

		// Token: 0x040046C0 RID: 18112
		[NonSerialized]
		public MeshFilter filter;

		// Token: 0x040046C1 RID: 18113
		[NonSerialized]
		public ShadowCastingMode shadowMode;

		// Token: 0x040046C2 RID: 18114
		[NonSerialized]
		public bool isImpostor;
	}
}
