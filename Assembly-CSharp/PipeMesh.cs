using System;
using UnityEngine;

// Token: 0x020001BD RID: 445
public class PipeMesh : MonoBehaviour
{
	// Token: 0x040011A3 RID: 4515
	public float PipeRadius = 0.04f;

	// Token: 0x040011A4 RID: 4516
	public Material PipeMaterial;

	// Token: 0x040011A5 RID: 4517
	public float StraightLength = 0.3f;

	// Token: 0x040011A6 RID: 4518
	public int PipeSubdivisions = 8;

	// Token: 0x040011A7 RID: 4519
	public int BendTesselation = 6;

	// Token: 0x040011A8 RID: 4520
	public float RidgeHeight = 0.05f;

	// Token: 0x040011A9 RID: 4521
	public float UvScaleMultiplier = 2f;

	// Token: 0x040011AA RID: 4522
	public float RidgeIncrements = 0.5f;

	// Token: 0x040011AB RID: 4523
	public float RidgeLength = 0.05f;

	// Token: 0x040011AC RID: 4524
	public Vector2 HorizontalUvRange = new Vector2(0f, 0.2f);
}
