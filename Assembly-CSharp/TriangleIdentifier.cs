using System;
using UnityEngine;

// Token: 0x02000314 RID: 788
public class TriangleIdentifier : MonoBehaviour
{
	// Token: 0x040017C2 RID: 6082
	public int TriangleID;

	// Token: 0x040017C3 RID: 6083
	public int SubmeshID;

	// Token: 0x040017C4 RID: 6084
	public float LineLength = 1.5f;

	// Token: 0x06001EC2 RID: 7874 RVA: 0x000D18F4 File Offset: 0x000CFAF4
	private void OnDrawGizmosSelected()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		if (component == null || component.sharedMesh == null)
		{
			return;
		}
		int[] triangles = component.sharedMesh.GetTriangles(this.SubmeshID);
		if (this.TriangleID < 0 || this.TriangleID * 3 > triangles.Length)
		{
			return;
		}
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Vector3 a = component.sharedMesh.vertices[this.TriangleID * 3];
		Vector3 b = component.sharedMesh.vertices[this.TriangleID * 3 + 1];
		Vector3 b2 = component.sharedMesh.vertices[this.TriangleID * 3 + 2];
		Vector3 a2 = component.sharedMesh.normals[this.TriangleID * 3];
		Vector3 b3 = component.sharedMesh.normals[this.TriangleID * 3 + 1];
		Vector3 b4 = component.sharedMesh.normals[this.TriangleID * 3 + 2];
		Vector3 vector = (a + b + b2) / 3f;
		Vector3 a3 = (a2 + b3 + b4) / 3f;
		Gizmos.DrawLine(vector, vector + a3 * this.LineLength);
	}
}
