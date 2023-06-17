using System;
using UnityEngine;

// Token: 0x02000538 RID: 1336
public class LODMasterMesh : LODComponent
{
	// Token: 0x040021FE RID: 8702
	public MeshRenderer ReplacementMesh;

	// Token: 0x040021FF RID: 8703
	public float Distance = 100f;

	// Token: 0x04002200 RID: 8704
	public LODComponent[] ChildComponents;

	// Token: 0x04002201 RID: 8705
	public bool Block;

	// Token: 0x04002202 RID: 8706
	public Bounds MeshBounds;
}
