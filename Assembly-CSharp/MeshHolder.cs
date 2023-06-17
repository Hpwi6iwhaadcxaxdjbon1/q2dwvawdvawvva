using System;
using UnityEngine;

// Token: 0x020009A2 RID: 2466
[Serializable]
public class MeshHolder
{
	// Token: 0x0400353C RID: 13628
	[HideInInspector]
	public Vector3[] _vertices;

	// Token: 0x0400353D RID: 13629
	[HideInInspector]
	public Vector3[] _normals;

	// Token: 0x0400353E RID: 13630
	[HideInInspector]
	public int[] _triangles;

	// Token: 0x0400353F RID: 13631
	[HideInInspector]
	public trisPerSubmesh[] _TrianglesOfSubs;

	// Token: 0x04003540 RID: 13632
	[HideInInspector]
	public Matrix4x4[] _bindPoses;

	// Token: 0x04003541 RID: 13633
	[HideInInspector]
	public BoneWeight[] _boneWeights;

	// Token: 0x04003542 RID: 13634
	[HideInInspector]
	public Bounds _bounds;

	// Token: 0x04003543 RID: 13635
	[HideInInspector]
	public int _subMeshCount;

	// Token: 0x04003544 RID: 13636
	[HideInInspector]
	public Vector4[] _tangents;

	// Token: 0x04003545 RID: 13637
	[HideInInspector]
	public Vector2[] _uv;

	// Token: 0x04003546 RID: 13638
	[HideInInspector]
	public Vector2[] _uv2;

	// Token: 0x04003547 RID: 13639
	[HideInInspector]
	public Vector2[] _uv3;

	// Token: 0x04003548 RID: 13640
	[HideInInspector]
	public Color[] _colors;

	// Token: 0x04003549 RID: 13641
	[HideInInspector]
	public Vector2[] _uv4;

	// Token: 0x06003AD9 RID: 15065 RVA: 0x0015CF5E File Offset: 0x0015B15E
	public void setAnimationData(Mesh mesh)
	{
		this._colors = mesh.colors;
	}
}
