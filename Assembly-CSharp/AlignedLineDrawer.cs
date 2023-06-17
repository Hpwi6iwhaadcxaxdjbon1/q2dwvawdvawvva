using System;
using UnityEngine;

// Token: 0x020001BF RID: 447
public class AlignedLineDrawer : MonoBehaviour, IClientComponent
{
	// Token: 0x040011B2 RID: 4530
	public MeshFilter Filter;

	// Token: 0x040011B3 RID: 4531
	public MeshRenderer Renderer;

	// Token: 0x040011B4 RID: 4532
	public float LineWidth = 1f;

	// Token: 0x040011B5 RID: 4533
	public float SurfaceOffset = 0.001f;

	// Token: 0x040011B6 RID: 4534
	public float SprayThickness = 0.4f;

	// Token: 0x040011B7 RID: 4535
	public float uvTilingFactor = 1f;

	// Token: 0x040011B8 RID: 4536
	public bool DrawEndCaps;

	// Token: 0x040011B9 RID: 4537
	public bool DrawSideMesh;

	// Token: 0x040011BA RID: 4538
	public bool DrawBackMesh;

	// Token: 0x040011BB RID: 4539
	public SprayCanSpray_Freehand Spray;

	// Token: 0x02000C39 RID: 3129
	[Serializable]
	public struct LinePoint
	{
		// Token: 0x0400428F RID: 17039
		public Vector3 LocalPosition;

		// Token: 0x04004290 RID: 17040
		public Vector3 WorldNormal;
	}
}
