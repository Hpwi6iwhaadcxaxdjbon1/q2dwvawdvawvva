using System;
using UnityEngine;

// Token: 0x020006C5 RID: 1733
public class GenerateRiverMeshes : ProceduralComponent
{
	// Token: 0x0400283C RID: 10300
	public const float NormalSmoothing = 0.1f;

	// Token: 0x0400283D RID: 10301
	public const bool SnapToTerrain = true;

	// Token: 0x0400283E RID: 10302
	public Mesh RiverMesh;

	// Token: 0x0400283F RID: 10303
	public Mesh[] RiverMeshes;

	// Token: 0x04002840 RID: 10304
	public Material RiverMaterial;

	// Token: 0x04002841 RID: 10305
	public PhysicMaterial RiverPhysicMaterial;

	// Token: 0x060031A4 RID: 12708 RVA: 0x0012E688 File Offset: 0x0012C888
	public override void Process(uint seed)
	{
		this.RiverMeshes = new Mesh[]
		{
			this.RiverMesh
		};
		foreach (PathList pathList in TerrainMeta.Path.Rivers)
		{
			foreach (PathList.MeshObject meshObject in pathList.CreateMesh(this.RiverMeshes, 0.1f, true, !pathList.Path.Circular, !pathList.Path.Circular))
			{
				GameObject gameObject = new GameObject("River Mesh");
				gameObject.transform.position = meshObject.Position;
				gameObject.tag = "River";
				gameObject.layer = 4;
				gameObject.SetHierarchyGroup(pathList.Name, true, false);
				gameObject.SetActive(false);
				MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
				meshCollider.sharedMaterial = this.RiverPhysicMaterial;
				meshCollider.sharedMesh = meshObject.Meshes[0];
				gameObject.AddComponent<RiverInfo>();
				gameObject.AddComponent<WaterBody>().FishingType = WaterBody.FishingTag.River;
				gameObject.AddComponent<AddToWaterMap>();
				gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x1700040D RID: 1037
	// (get) Token: 0x060031A5 RID: 12709 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
