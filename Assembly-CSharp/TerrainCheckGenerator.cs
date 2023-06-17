using System;
using UnityEngine;

// Token: 0x0200068E RID: 1678
public class TerrainCheckGenerator : MonoBehaviour, IEditorComponent
{
	// Token: 0x04002752 RID: 10066
	public float PlacementRadius = 32f;

	// Token: 0x04002753 RID: 10067
	public float PlacementPadding;

	// Token: 0x04002754 RID: 10068
	public float PlacementFade = 16f;

	// Token: 0x04002755 RID: 10069
	public float PlacementDistance = 8f;

	// Token: 0x04002756 RID: 10070
	public float CheckExtentsMin = 8f;

	// Token: 0x04002757 RID: 10071
	public float CheckExtentsMax = 16f;

	// Token: 0x04002758 RID: 10072
	public bool CheckRotate = true;
}
