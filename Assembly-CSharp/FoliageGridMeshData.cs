using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000933 RID: 2355
public class FoliageGridMeshData
{
	// Token: 0x04003343 RID: 13123
	public List<FoliageGridMeshData.FoliageVertex> vertices;

	// Token: 0x04003344 RID: 13124
	public List<int> triangles;

	// Token: 0x04003345 RID: 13125
	public Bounds bounds;

	// Token: 0x06003887 RID: 14471 RVA: 0x00151393 File Offset: 0x0014F593
	public void Alloc()
	{
		if (this.triangles == null)
		{
			this.triangles = Pool.GetList<int>();
		}
		if (this.vertices == null)
		{
			this.vertices = Pool.GetList<FoliageGridMeshData.FoliageVertex>();
		}
	}

	// Token: 0x06003888 RID: 14472 RVA: 0x001513BB File Offset: 0x0014F5BB
	public void Free()
	{
		if (this.triangles != null)
		{
			Pool.FreeList<int>(ref this.triangles);
		}
		if (this.vertices != null)
		{
			Pool.FreeList<FoliageGridMeshData.FoliageVertex>(ref this.vertices);
		}
	}

	// Token: 0x06003889 RID: 14473 RVA: 0x001513E3 File Offset: 0x0014F5E3
	public void Clear()
	{
		List<int> list = this.triangles;
		if (list != null)
		{
			list.Clear();
		}
		List<FoliageGridMeshData.FoliageVertex> list2 = this.vertices;
		if (list2 == null)
		{
			return;
		}
		list2.Clear();
	}

	// Token: 0x0600388A RID: 14474 RVA: 0x00151408 File Offset: 0x0014F608
	public void Combine(MeshGroup meshGroup)
	{
		if (meshGroup.data.Count == 0)
		{
			return;
		}
		this.bounds = new Bounds(meshGroup.data[0].position, Vector3.one);
		for (int i = 0; i < meshGroup.data.Count; i++)
		{
			MeshInstance meshInstance = meshGroup.data[i];
			Matrix4x4 matrix4x = Matrix4x4.TRS(meshInstance.position, meshInstance.rotation, meshInstance.scale);
			int count = this.vertices.Count;
			for (int j = 0; j < meshInstance.data.triangles.Length; j++)
			{
				this.triangles.Add(count + meshInstance.data.triangles[j]);
			}
			for (int k = 0; k < meshInstance.data.vertices.Length; k++)
			{
				Vector4 vector = meshInstance.data.tangents[k];
				Vector3 vector2 = new Vector3(vector.x, vector.y, vector.z);
				Vector3 vector3 = matrix4x.MultiplyVector(vector2);
				FoliageGridMeshData.FoliageVertex item = default(FoliageGridMeshData.FoliageVertex);
				item.position = matrix4x.MultiplyPoint3x4(meshInstance.data.vertices[k]);
				item.normal = matrix4x.MultiplyVector(meshInstance.data.normals[k]);
				item.uv = meshInstance.data.uv[k];
				item.uv2 = meshInstance.position;
				item.tangent = new Vector4(vector3.x, vector3.y, vector3.z, vector.w);
				if (meshInstance.data.colors32.Length != 0)
				{
					item.color = meshInstance.data.colors32[k];
				}
				this.vertices.Add(item);
			}
			this.bounds.Encapsulate(meshInstance.position);
		}
		this.bounds.size = this.bounds.size + Vector3.one * 2f;
	}

	// Token: 0x0600388B RID: 14475 RVA: 0x0015162C File Offset: 0x0014F82C
	public void Apply(Mesh mesh)
	{
		mesh.SetVertexBufferParams(this.vertices.Count, FoliageGridMeshData.FoliageVertex.VertexLayout);
		mesh.SetVertexBufferData<FoliageGridMeshData.FoliageVertex>(this.vertices, 0, 0, this.vertices.Count, 0, MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontRecalculateBounds);
		mesh.SetIndices(this.triangles, MeshTopology.Triangles, 0, false, 0);
		mesh.bounds = this.bounds;
	}

	// Token: 0x02000EBC RID: 3772
	public struct FoliageVertex
	{
		// Token: 0x04004CC2 RID: 19650
		public Vector3 position;

		// Token: 0x04004CC3 RID: 19651
		public Vector3 normal;

		// Token: 0x04004CC4 RID: 19652
		public Vector4 tangent;

		// Token: 0x04004CC5 RID: 19653
		public Color32 color;

		// Token: 0x04004CC6 RID: 19654
		public Vector2 uv;

		// Token: 0x04004CC7 RID: 19655
		public Vector4 uv2;

		// Token: 0x04004CC8 RID: 19656
		public static readonly VertexAttributeDescriptor[] VertexLayout = new VertexAttributeDescriptor[]
		{
			new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0),
			new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, 0),
			new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.Float32, 4, 0),
			new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4, 0),
			new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, 0),
			new VertexAttributeDescriptor(VertexAttribute.TexCoord2, VertexAttributeFormat.Float32, 4, 0)
		};
	}
}
