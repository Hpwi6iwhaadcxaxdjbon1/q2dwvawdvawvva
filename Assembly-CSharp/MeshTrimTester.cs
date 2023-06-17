using System;
using UnityEngine;

// Token: 0x0200030A RID: 778
[ExecuteInEditMode]
public class MeshTrimTester : MonoBehaviour
{
	// Token: 0x040017A8 RID: 6056
	public MeshTrimSettings Settings = MeshTrimSettings.Default;

	// Token: 0x040017A9 RID: 6057
	public Mesh SourceMesh;

	// Token: 0x040017AA RID: 6058
	public MeshFilter TargetMeshFilter;

	// Token: 0x040017AB RID: 6059
	public int SubtractIndex;
}
