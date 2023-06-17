using System;
using UnityEngine;

// Token: 0x0200053E RID: 1342
public class MeshLOD : LODComponent, IBatchingHandler
{
	// Token: 0x0400220D RID: 8717
	[Horizontal(1, 0)]
	public MeshLOD.State[] States;

	// Token: 0x02000D3E RID: 3390
	[Serializable]
	public class State
	{
		// Token: 0x040046B3 RID: 18099
		public float distance;

		// Token: 0x040046B4 RID: 18100
		public Mesh mesh;
	}
}
