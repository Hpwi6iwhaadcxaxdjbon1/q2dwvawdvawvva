using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x020002A8 RID: 680
public class MeshColliderData
{
	// Token: 0x04001624 RID: 5668
	public List<int> triangles;

	// Token: 0x04001625 RID: 5669
	public List<Vector3> vertices;

	// Token: 0x04001626 RID: 5670
	public List<Vector3> normals;

	// Token: 0x06001D39 RID: 7481 RVA: 0x000C9635 File Offset: 0x000C7835
	public void Alloc()
	{
		if (this.triangles == null)
		{
			this.triangles = Facepunch.Pool.GetList<int>();
		}
		if (this.vertices == null)
		{
			this.vertices = Facepunch.Pool.GetList<Vector3>();
		}
		if (this.normals == null)
		{
			this.normals = Facepunch.Pool.GetList<Vector3>();
		}
	}

	// Token: 0x06001D3A RID: 7482 RVA: 0x000C9670 File Offset: 0x000C7870
	public void Free()
	{
		if (this.triangles != null)
		{
			Facepunch.Pool.FreeList<int>(ref this.triangles);
		}
		if (this.vertices != null)
		{
			Facepunch.Pool.FreeList<Vector3>(ref this.vertices);
		}
		if (this.normals != null)
		{
			Facepunch.Pool.FreeList<Vector3>(ref this.normals);
		}
	}

	// Token: 0x06001D3B RID: 7483 RVA: 0x000C96AB File Offset: 0x000C78AB
	public void Clear()
	{
		if (this.triangles != null)
		{
			this.triangles.Clear();
		}
		if (this.vertices != null)
		{
			this.vertices.Clear();
		}
		if (this.normals != null)
		{
			this.normals.Clear();
		}
	}

	// Token: 0x06001D3C RID: 7484 RVA: 0x000C96E8 File Offset: 0x000C78E8
	public void Apply(UnityEngine.Mesh mesh)
	{
		mesh.Clear();
		if (this.vertices != null)
		{
			mesh.SetVertices(this.vertices);
		}
		if (this.triangles != null)
		{
			mesh.SetTriangles(this.triangles, 0);
		}
		if (this.normals != null)
		{
			if (this.normals.Count == this.vertices.Count)
			{
				mesh.SetNormals(this.normals);
				return;
			}
			if (this.normals.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping collider normals because some meshes were missing them.");
			}
		}
	}

	// Token: 0x06001D3D RID: 7485 RVA: 0x000C9774 File Offset: 0x000C7974
	public void Combine(MeshColliderGroup meshGroup)
	{
		for (int i = 0; i < meshGroup.data.Count; i++)
		{
			MeshColliderInstance meshColliderInstance = meshGroup.data[i];
			Matrix4x4 matrix4x = Matrix4x4.TRS(meshColliderInstance.position, meshColliderInstance.rotation, meshColliderInstance.scale);
			int count = this.vertices.Count;
			for (int j = 0; j < meshColliderInstance.data.triangles.Length; j++)
			{
				this.triangles.Add(count + meshColliderInstance.data.triangles[j]);
			}
			for (int k = 0; k < meshColliderInstance.data.vertices.Length; k++)
			{
				this.vertices.Add(matrix4x.MultiplyPoint3x4(meshColliderInstance.data.vertices[k]));
			}
			for (int l = 0; l < meshColliderInstance.data.normals.Length; l++)
			{
				this.normals.Add(matrix4x.MultiplyVector(meshColliderInstance.data.normals[l]));
			}
		}
	}

	// Token: 0x06001D3E RID: 7486 RVA: 0x000C9888 File Offset: 0x000C7A88
	public void Combine(MeshColliderGroup meshGroup, MeshColliderLookup colliderLookup)
	{
		for (int i = 0; i < meshGroup.data.Count; i++)
		{
			MeshColliderInstance meshColliderInstance = meshGroup.data[i];
			Matrix4x4 matrix4x = Matrix4x4.TRS(meshColliderInstance.position, meshColliderInstance.rotation, meshColliderInstance.scale);
			int count = this.vertices.Count;
			for (int j = 0; j < meshColliderInstance.data.triangles.Length; j++)
			{
				this.triangles.Add(count + meshColliderInstance.data.triangles[j]);
			}
			for (int k = 0; k < meshColliderInstance.data.vertices.Length; k++)
			{
				this.vertices.Add(matrix4x.MultiplyPoint3x4(meshColliderInstance.data.vertices[k]));
			}
			for (int l = 0; l < meshColliderInstance.data.normals.Length; l++)
			{
				this.normals.Add(matrix4x.MultiplyVector(meshColliderInstance.data.normals[l]));
			}
			colliderLookup.Add(meshColliderInstance);
		}
	}
}
