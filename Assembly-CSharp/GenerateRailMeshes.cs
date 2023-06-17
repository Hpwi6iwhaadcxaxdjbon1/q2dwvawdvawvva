using System;
using UnityEngine;

// Token: 0x020006BE RID: 1726
public class GenerateRailMeshes : ProceduralComponent
{
	// Token: 0x04002814 RID: 10260
	public const float NormalSmoothing = 0f;

	// Token: 0x04002815 RID: 10261
	public const bool SnapToTerrain = false;

	// Token: 0x04002816 RID: 10262
	public Mesh RailMesh;

	// Token: 0x04002817 RID: 10263
	public Mesh[] RailMeshes;

	// Token: 0x04002818 RID: 10264
	public Material RailMaterial;

	// Token: 0x04002819 RID: 10265
	public PhysicMaterial RailPhysicMaterial;

	// Token: 0x06003190 RID: 12688 RVA: 0x0012CACC File Offset: 0x0012ACCC
	public override void Process(uint seed)
	{
		if (this.RailMeshes == null || this.RailMeshes.Length == 0)
		{
			this.RailMeshes = new Mesh[]
			{
				this.RailMesh
			};
		}
		foreach (PathList pathList in TerrainMeta.Path.Rails)
		{
			foreach (PathList.MeshObject meshObject in pathList.CreateMesh(this.RailMeshes, 0f, false, !pathList.Path.Circular && !pathList.Start, !pathList.Path.Circular && !pathList.End))
			{
				GameObject gameObject = new GameObject("Rail Mesh");
				gameObject.transform.position = meshObject.Position;
				gameObject.tag = "Railway";
				gameObject.layer = 16;
				gameObject.SetHierarchyGroup(pathList.Name, true, false);
				gameObject.SetActive(false);
				MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
				meshCollider.sharedMaterial = this.RailPhysicMaterial;
				meshCollider.sharedMesh = meshObject.Meshes[0];
				gameObject.AddComponent<AddToHeightMap>();
				gameObject.SetActive(true);
			}
			this.AddTrackSpline(pathList);
		}
	}

	// Token: 0x1700040C RID: 1036
	// (get) Token: 0x06003191 RID: 12689 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06003192 RID: 12690 RVA: 0x0012CC38 File Offset: 0x0012AE38
	private void AddTrackSpline(PathList rail)
	{
		TrainTrackSpline trainTrackSpline = HierarchyUtil.GetRoot(rail.Name, true, false).AddComponent<TrainTrackSpline>();
		trainTrackSpline.aboveGroundSpawn = (rail.Hierarchy == 2);
		trainTrackSpline.hierarchy = rail.Hierarchy;
		if (trainTrackSpline.aboveGroundSpawn)
		{
			TrainTrackSpline.SidingSplines.Add(trainTrackSpline);
		}
		Vector3[] array = new Vector3[rail.Path.Points.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = rail.Path.Points[i];
			Vector3[] array2 = array;
			int num = i;
			array2[num].y = array2[num].y + 0.41f;
		}
		Vector3[] array3 = new Vector3[rail.Path.Tangents.Length];
		for (int j = 0; j < array.Length; j++)
		{
			array3[j] = rail.Path.Tangents[j];
		}
		trainTrackSpline.SetAll(array, array3, 0.25f);
	}
}
