using System;
using UnityEngine;

// Token: 0x0200039B RID: 923
public class DiscoFloorMesh : MonoBehaviour, IClientComponent
{
	// Token: 0x04001982 RID: 6530
	public int GridRows = 5;

	// Token: 0x04001983 RID: 6531
	public int GridColumns = 5;

	// Token: 0x04001984 RID: 6532
	public float GridSize = 1f;

	// Token: 0x04001985 RID: 6533
	[Range(0f, 10f)]
	public float TestOffset;

	// Token: 0x04001986 RID: 6534
	public Color OffColor = Color.grey;

	// Token: 0x04001987 RID: 6535
	public MeshRenderer Renderer;

	// Token: 0x04001988 RID: 6536
	public bool DrawInEditor;

	// Token: 0x04001989 RID: 6537
	public MeshFilter Filter;

	// Token: 0x0400198A RID: 6538
	public AnimationCurve customCurveX = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400198B RID: 6539
	public AnimationCurve customCurveY = AnimationCurve.Linear(0f, 0f, 1f, 1f);
}
