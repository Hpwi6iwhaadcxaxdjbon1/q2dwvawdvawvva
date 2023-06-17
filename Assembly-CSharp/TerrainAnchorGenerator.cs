using System;
using UnityEngine;

// Token: 0x0200068B RID: 1675
public class TerrainAnchorGenerator : MonoBehaviour, IEditorComponent
{
	// Token: 0x04002748 RID: 10056
	public float PlacementRadius = 32f;

	// Token: 0x04002749 RID: 10057
	public float PlacementPadding;

	// Token: 0x0400274A RID: 10058
	public float PlacementFade = 16f;

	// Token: 0x0400274B RID: 10059
	public float PlacementDistance = 8f;

	// Token: 0x0400274C RID: 10060
	public float AnchorExtentsMin = 8f;

	// Token: 0x0400274D RID: 10061
	public float AnchorExtentsMax = 16f;

	// Token: 0x0400274E RID: 10062
	public float AnchorOffsetMin;

	// Token: 0x0400274F RID: 10063
	public float AnchorOffsetMax;
}
