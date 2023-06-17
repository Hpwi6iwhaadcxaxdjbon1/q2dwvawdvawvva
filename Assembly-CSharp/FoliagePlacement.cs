using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200050A RID: 1290
[CreateAssetMenu(menuName = "Rust/Foliage Placement")]
public class FoliagePlacement : ScriptableObject
{
	// Token: 0x0400213B RID: 8507
	[Header("Placement")]
	public float Density = 2f;

	// Token: 0x0400213C RID: 8508
	[Header("Filter")]
	public SpawnFilter Filter;

	// Token: 0x0400213D RID: 8509
	[FormerlySerializedAs("Cutoff")]
	public float FilterCutoff = 0.5f;

	// Token: 0x0400213E RID: 8510
	public float FilterFade = 0.1f;

	// Token: 0x0400213F RID: 8511
	[FormerlySerializedAs("Scaling")]
	public float FilterScaling = 1f;

	// Token: 0x04002140 RID: 8512
	[Header("Randomization")]
	public float RandomScaling = 0.2f;

	// Token: 0x04002141 RID: 8513
	[Header("Placement Range")]
	[MinMax(0f, 1f)]
	public MinMax Range = new MinMax(0f, 1f);

	// Token: 0x04002142 RID: 8514
	public float RangeFade = 0.1f;

	// Token: 0x04002143 RID: 8515
	[Header("LOD")]
	[Range(0f, 1f)]
	public float DistanceDensity;

	// Token: 0x04002144 RID: 8516
	[Range(1f, 2f)]
	public float DistanceScaling = 2f;

	// Token: 0x04002145 RID: 8517
	[Header("Visuals")]
	public Material material;

	// Token: 0x04002146 RID: 8518
	[FormerlySerializedAs("mesh")]
	public Mesh mesh0;

	// Token: 0x04002147 RID: 8519
	[FormerlySerializedAs("mesh")]
	public Mesh mesh1;

	// Token: 0x04002148 RID: 8520
	[FormerlySerializedAs("mesh")]
	public Mesh mesh2;

	// Token: 0x04002149 RID: 8521
	public const int lods = 5;

	// Token: 0x0400214A RID: 8522
	public const int octaves = 1;

	// Token: 0x0400214B RID: 8523
	public const float frequency = 0.05f;

	// Token: 0x0400214C RID: 8524
	public const float amplitude = 0.5f;

	// Token: 0x0400214D RID: 8525
	public const float offset = 0.5f;
}
