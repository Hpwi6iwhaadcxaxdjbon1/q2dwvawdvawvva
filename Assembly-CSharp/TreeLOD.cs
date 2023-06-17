using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000547 RID: 1351
public class TreeLOD : LODComponent
{
	// Token: 0x04002218 RID: 8728
	[Horizontal(1, 0)]
	public TreeLOD.State[] States;

	// Token: 0x02000D43 RID: 3395
	[Serializable]
	public class State
	{
		// Token: 0x040046C3 RID: 18115
		public float distance;

		// Token: 0x040046C4 RID: 18116
		public Renderer renderer;

		// Token: 0x040046C5 RID: 18117
		[NonSerialized]
		public MeshFilter filter;

		// Token: 0x040046C6 RID: 18118
		[NonSerialized]
		public ShadowCastingMode shadowMode;

		// Token: 0x040046C7 RID: 18119
		[NonSerialized]
		public bool isImpostor;
	}
}
