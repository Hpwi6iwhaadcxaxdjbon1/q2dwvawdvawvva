using System;
using UnityEngine;

// Token: 0x020006CA RID: 1738
public class GenerateRoadMeshes : ProceduralComponent
{
	// Token: 0x04002851 RID: 10321
	public const float NormalSmoothing = 0f;

	// Token: 0x04002852 RID: 10322
	public const bool SnapToTerrain = true;

	// Token: 0x04002853 RID: 10323
	public Mesh RoadMesh;

	// Token: 0x04002854 RID: 10324
	public Mesh[] RoadMeshes;

	// Token: 0x04002855 RID: 10325
	public Material RoadMaterial;

	// Token: 0x04002856 RID: 10326
	public Material RoadRingMaterial;

	// Token: 0x04002857 RID: 10327
	public PhysicMaterial RoadPhysicMaterial;

	// Token: 0x060031B1 RID: 12721 RVA: 0x0012F3FC File Offset: 0x0012D5FC
	public override void Process(uint seed)
	{
		if (this.RoadMeshes == null || this.RoadMeshes.Length == 0)
		{
			this.RoadMeshes = new Mesh[]
			{
				this.RoadMesh
			};
		}
		foreach (PathList pathList in TerrainMeta.Path.Roads)
		{
			if (pathList.Hierarchy < 2)
			{
				foreach (PathList.MeshObject meshObject in pathList.CreateMesh(this.RoadMeshes, 0f, true, !pathList.Path.Circular, !pathList.Path.Circular))
				{
					GameObject gameObject = new GameObject("Road Mesh");
					gameObject.transform.position = meshObject.Position;
					gameObject.layer = 16;
					gameObject.SetHierarchyGroup(pathList.Name, true, false);
					gameObject.SetActive(false);
					MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
					meshCollider.sharedMaterial = this.RoadPhysicMaterial;
					meshCollider.sharedMesh = meshObject.Meshes[0];
					gameObject.AddComponent<AddToHeightMap>();
					gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x1700040E RID: 1038
	// (get) Token: 0x060031B2 RID: 12722 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
