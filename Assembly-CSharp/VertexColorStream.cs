using System;
using UnityEngine;

// Token: 0x020009A5 RID: 2469
[ExecuteInEditMode]
public class VertexColorStream : MonoBehaviour
{
	// Token: 0x04003550 RID: 13648
	[HideInInspector]
	public Mesh originalMesh;

	// Token: 0x04003551 RID: 13649
	[HideInInspector]
	public Mesh paintedMesh;

	// Token: 0x04003552 RID: 13650
	[HideInInspector]
	public MeshHolder meshHold;

	// Token: 0x04003553 RID: 13651
	[HideInInspector]
	public Vector3[] _vertices;

	// Token: 0x04003554 RID: 13652
	[HideInInspector]
	public Vector3[] _normals;

	// Token: 0x04003555 RID: 13653
	[HideInInspector]
	public int[] _triangles;

	// Token: 0x04003556 RID: 13654
	[HideInInspector]
	public int[][] _Subtriangles;

	// Token: 0x04003557 RID: 13655
	[HideInInspector]
	public Matrix4x4[] _bindPoses;

	// Token: 0x04003558 RID: 13656
	[HideInInspector]
	public BoneWeight[] _boneWeights;

	// Token: 0x04003559 RID: 13657
	[HideInInspector]
	public Bounds _bounds;

	// Token: 0x0400355A RID: 13658
	[HideInInspector]
	public int _subMeshCount;

	// Token: 0x0400355B RID: 13659
	[HideInInspector]
	public Vector4[] _tangents;

	// Token: 0x0400355C RID: 13660
	[HideInInspector]
	public Vector2[] _uv;

	// Token: 0x0400355D RID: 13661
	[HideInInspector]
	public Vector2[] _uv2;

	// Token: 0x0400355E RID: 13662
	[HideInInspector]
	public Vector2[] _uv3;

	// Token: 0x0400355F RID: 13663
	[HideInInspector]
	public Color[] _colors;

	// Token: 0x04003560 RID: 13664
	[HideInInspector]
	public Vector2[] _uv4;

	// Token: 0x06003AE4 RID: 15076 RVA: 0x000063A5 File Offset: 0x000045A5
	private void OnDidApplyAnimationProperties()
	{
	}

	// Token: 0x06003AE5 RID: 15077 RVA: 0x0015D2EC File Offset: 0x0015B4EC
	public void init(Mesh origMesh, bool destroyOld)
	{
		this.originalMesh = origMesh;
		this.paintedMesh = UnityEngine.Object.Instantiate<Mesh>(origMesh);
		if (destroyOld)
		{
			UnityEngine.Object.DestroyImmediate(origMesh);
		}
		this.paintedMesh.hideFlags = HideFlags.None;
		this.paintedMesh.name = "vpp_" + base.gameObject.name;
		this.meshHold = new MeshHolder();
		this.meshHold._vertices = this.paintedMesh.vertices;
		this.meshHold._normals = this.paintedMesh.normals;
		this.meshHold._triangles = this.paintedMesh.triangles;
		this.meshHold._TrianglesOfSubs = new trisPerSubmesh[this.paintedMesh.subMeshCount];
		for (int i = 0; i < this.paintedMesh.subMeshCount; i++)
		{
			this.meshHold._TrianglesOfSubs[i] = new trisPerSubmesh();
			this.meshHold._TrianglesOfSubs[i].triangles = this.paintedMesh.GetTriangles(i);
		}
		this.meshHold._bindPoses = this.paintedMesh.bindposes;
		this.meshHold._boneWeights = this.paintedMesh.boneWeights;
		this.meshHold._bounds = this.paintedMesh.bounds;
		this.meshHold._subMeshCount = this.paintedMesh.subMeshCount;
		this.meshHold._tangents = this.paintedMesh.tangents;
		this.meshHold._uv = this.paintedMesh.uv;
		this.meshHold._uv2 = this.paintedMesh.uv2;
		this.meshHold._uv3 = this.paintedMesh.uv3;
		this.meshHold._colors = this.paintedMesh.colors;
		this.meshHold._uv4 = this.paintedMesh.uv4;
		base.GetComponent<MeshFilter>().sharedMesh = this.paintedMesh;
		if (base.GetComponent<MeshCollider>())
		{
			base.GetComponent<MeshCollider>().sharedMesh = this.paintedMesh;
		}
	}

	// Token: 0x06003AE6 RID: 15078 RVA: 0x0015D4FC File Offset: 0x0015B6FC
	public void setWholeMesh(Mesh tmpMesh)
	{
		this.paintedMesh.vertices = tmpMesh.vertices;
		this.paintedMesh.triangles = tmpMesh.triangles;
		this.paintedMesh.normals = tmpMesh.normals;
		this.paintedMesh.colors = tmpMesh.colors;
		this.paintedMesh.uv = tmpMesh.uv;
		this.paintedMesh.uv2 = tmpMesh.uv2;
		this.paintedMesh.uv3 = tmpMesh.uv3;
		this.meshHold._vertices = tmpMesh.vertices;
		this.meshHold._triangles = tmpMesh.triangles;
		this.meshHold._normals = tmpMesh.normals;
		this.meshHold._colors = tmpMesh.colors;
		this.meshHold._uv = tmpMesh.uv;
		this.meshHold._uv2 = tmpMesh.uv2;
		this.meshHold._uv3 = tmpMesh.uv3;
	}

	// Token: 0x06003AE7 RID: 15079 RVA: 0x0015D5F8 File Offset: 0x0015B7F8
	public Vector3[] setVertices(Vector3[] _deformedVertices)
	{
		this.paintedMesh.vertices = _deformedVertices;
		this.meshHold._vertices = _deformedVertices;
		this.paintedMesh.RecalculateNormals();
		this.paintedMesh.RecalculateBounds();
		this.meshHold._normals = this.paintedMesh.normals;
		this.meshHold._bounds = this.paintedMesh.bounds;
		base.GetComponent<MeshCollider>().sharedMesh = null;
		if (base.GetComponent<MeshCollider>())
		{
			base.GetComponent<MeshCollider>().sharedMesh = this.paintedMesh;
		}
		return this.meshHold._normals;
	}

	// Token: 0x06003AE8 RID: 15080 RVA: 0x0015D694 File Offset: 0x0015B894
	public Vector3[] getVertices()
	{
		return this.paintedMesh.vertices;
	}

	// Token: 0x06003AE9 RID: 15081 RVA: 0x0015D6A1 File Offset: 0x0015B8A1
	public Vector3[] getNormals()
	{
		return this.paintedMesh.normals;
	}

	// Token: 0x06003AEA RID: 15082 RVA: 0x0015D6AE File Offset: 0x0015B8AE
	public int[] getTriangles()
	{
		return this.paintedMesh.triangles;
	}

	// Token: 0x06003AEB RID: 15083 RVA: 0x0015D6BB File Offset: 0x0015B8BB
	public void setTangents(Vector4[] _meshTangents)
	{
		this.paintedMesh.tangents = _meshTangents;
		this.meshHold._tangents = _meshTangents;
	}

	// Token: 0x06003AEC RID: 15084 RVA: 0x0015D6D5 File Offset: 0x0015B8D5
	public Vector4[] getTangents()
	{
		return this.paintedMesh.tangents;
	}

	// Token: 0x06003AED RID: 15085 RVA: 0x0015D6E2 File Offset: 0x0015B8E2
	public void setColors(Color[] _vertexColors)
	{
		this.paintedMesh.colors = _vertexColors;
		this.meshHold._colors = _vertexColors;
	}

	// Token: 0x06003AEE RID: 15086 RVA: 0x0015D6FC File Offset: 0x0015B8FC
	public Color[] getColors()
	{
		return this.paintedMesh.colors;
	}

	// Token: 0x06003AEF RID: 15087 RVA: 0x0015D709 File Offset: 0x0015B909
	public Vector2[] getUVs()
	{
		return this.paintedMesh.uv;
	}

	// Token: 0x06003AF0 RID: 15088 RVA: 0x0015D716 File Offset: 0x0015B916
	public void setUV4s(Vector2[] _uv4s)
	{
		this.paintedMesh.uv4 = _uv4s;
		this.meshHold._uv4 = _uv4s;
	}

	// Token: 0x06003AF1 RID: 15089 RVA: 0x0015D730 File Offset: 0x0015B930
	public Vector2[] getUV4s()
	{
		return this.paintedMesh.uv4;
	}

	// Token: 0x06003AF2 RID: 15090 RVA: 0x0015D73D File Offset: 0x0015B93D
	public void unlink()
	{
		this.init(this.paintedMesh, false);
	}

	// Token: 0x06003AF3 RID: 15091 RVA: 0x0015D74C File Offset: 0x0015B94C
	public void rebuild()
	{
		if (!base.GetComponent<MeshFilter>())
		{
			return;
		}
		this.paintedMesh = new Mesh();
		this.paintedMesh.hideFlags = HideFlags.HideAndDontSave;
		this.paintedMesh.name = "vpp_" + base.gameObject.name;
		if (this.meshHold == null || this.meshHold._vertices.Length == 0 || this.meshHold._TrianglesOfSubs.Length == 0)
		{
			this.paintedMesh.subMeshCount = this._subMeshCount;
			this.paintedMesh.vertices = this._vertices;
			this.paintedMesh.normals = this._normals;
			this.paintedMesh.triangles = this._triangles;
			this.meshHold._TrianglesOfSubs = new trisPerSubmesh[this.paintedMesh.subMeshCount];
			for (int i = 0; i < this.paintedMesh.subMeshCount; i++)
			{
				this.meshHold._TrianglesOfSubs[i] = new trisPerSubmesh();
				this.meshHold._TrianglesOfSubs[i].triangles = this.paintedMesh.GetTriangles(i);
			}
			this.paintedMesh.bindposes = this._bindPoses;
			this.paintedMesh.boneWeights = this._boneWeights;
			this.paintedMesh.bounds = this._bounds;
			this.paintedMesh.tangents = this._tangents;
			this.paintedMesh.uv = this._uv;
			this.paintedMesh.uv2 = this._uv2;
			this.paintedMesh.uv3 = this._uv3;
			this.paintedMesh.colors = this._colors;
			this.paintedMesh.uv4 = this._uv4;
			this.init(this.paintedMesh, true);
			return;
		}
		this.paintedMesh.subMeshCount = this.meshHold._subMeshCount;
		this.paintedMesh.vertices = this.meshHold._vertices;
		this.paintedMesh.normals = this.meshHold._normals;
		for (int j = 0; j < this.meshHold._subMeshCount; j++)
		{
			this.paintedMesh.SetTriangles(this.meshHold._TrianglesOfSubs[j].triangles, j);
		}
		this.paintedMesh.bindposes = this.meshHold._bindPoses;
		this.paintedMesh.boneWeights = this.meshHold._boneWeights;
		this.paintedMesh.bounds = this.meshHold._bounds;
		this.paintedMesh.tangents = this.meshHold._tangents;
		this.paintedMesh.uv = this.meshHold._uv;
		this.paintedMesh.uv2 = this.meshHold._uv2;
		this.paintedMesh.uv3 = this.meshHold._uv3;
		this.paintedMesh.colors = this.meshHold._colors;
		this.paintedMesh.uv4 = this.meshHold._uv4;
		this.init(this.paintedMesh, true);
	}

	// Token: 0x06003AF4 RID: 15092 RVA: 0x0015DA5B File Offset: 0x0015BC5B
	private void Start()
	{
		if (!this.paintedMesh || this.meshHold == null)
		{
			this.rebuild();
		}
	}
}
