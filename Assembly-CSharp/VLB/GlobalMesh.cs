using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020009B4 RID: 2484
	public static class GlobalMesh
	{
		// Token: 0x040035E4 RID: 13796
		private static Mesh ms_Mesh;

		// Token: 0x170004B4 RID: 1204
		// (get) Token: 0x06003B3C RID: 15164 RVA: 0x0015F264 File Offset: 0x0015D464
		public static Mesh mesh
		{
			get
			{
				if (GlobalMesh.ms_Mesh == null)
				{
					GlobalMesh.ms_Mesh = MeshGenerator.GenerateConeZ_Radius(1f, 1f, 1f, Config.Instance.sharedMeshSides, Config.Instance.sharedMeshSegments, true);
					GlobalMesh.ms_Mesh.hideFlags = Consts.ProceduralObjectsHideFlags;
				}
				return GlobalMesh.ms_Mesh;
			}
		}
	}
}
