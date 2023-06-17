using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002A7 RID: 679
public static class MeshCache
{
	// Token: 0x04001623 RID: 5667
	public static Dictionary<Mesh, MeshCache.Data> dictionary = new Dictionary<Mesh, MeshCache.Data>();

	// Token: 0x06001D37 RID: 7479 RVA: 0x000C9584 File Offset: 0x000C7784
	public static MeshCache.Data Get(Mesh mesh)
	{
		MeshCache.Data data;
		if (!MeshCache.dictionary.TryGetValue(mesh, out data))
		{
			data = new MeshCache.Data();
			data.mesh = mesh;
			data.vertices = mesh.vertices;
			data.normals = mesh.normals;
			data.tangents = mesh.tangents;
			data.colors32 = mesh.colors32;
			data.triangles = mesh.triangles;
			data.uv = mesh.uv;
			data.uv2 = mesh.uv2;
			data.uv3 = mesh.uv3;
			data.uv4 = mesh.uv4;
			MeshCache.dictionary.Add(mesh, data);
		}
		return data;
	}

	// Token: 0x02000C8F RID: 3215
	[Serializable]
	public class Data
	{
		// Token: 0x040043E3 RID: 17379
		public Mesh mesh;

		// Token: 0x040043E4 RID: 17380
		public Vector3[] vertices;

		// Token: 0x040043E5 RID: 17381
		public Vector3[] normals;

		// Token: 0x040043E6 RID: 17382
		public Vector4[] tangents;

		// Token: 0x040043E7 RID: 17383
		public Color32[] colors32;

		// Token: 0x040043E8 RID: 17384
		public int[] triangles;

		// Token: 0x040043E9 RID: 17385
		public Vector2[] uv;

		// Token: 0x040043EA RID: 17386
		public Vector2[] uv2;

		// Token: 0x040043EB RID: 17387
		public Vector2[] uv3;

		// Token: 0x040043EC RID: 17388
		public Vector2[] uv4;
	}
}
