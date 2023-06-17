using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000209 RID: 521
public class DynamicNavMesh : SingletonComponent<DynamicNavMesh>, IServerComponent
{
	// Token: 0x04001339 RID: 4921
	public int NavMeshAgentTypeIndex;

	// Token: 0x0400133A RID: 4922
	[Tooltip("The default area associated with the NavMeshAgent index.")]
	public string DefaultAreaName = "Walkable";

	// Token: 0x0400133B RID: 4923
	public int AsyncTerrainNavMeshBakeCellSize = 80;

	// Token: 0x0400133C RID: 4924
	public int AsyncTerrainNavMeshBakeCellHeight = 100;

	// Token: 0x0400133D RID: 4925
	public Bounds Bounds;

	// Token: 0x0400133E RID: 4926
	public NavMeshData NavMeshData;

	// Token: 0x0400133F RID: 4927
	public NavMeshDataInstance NavMeshDataInstance;

	// Token: 0x04001340 RID: 4928
	public LayerMask LayerMask;

	// Token: 0x04001341 RID: 4929
	public NavMeshCollectGeometry NavMeshCollectGeometry;

	// Token: 0x04001342 RID: 4930
	[ServerVar]
	public static bool use_baked_terrain_mesh;

	// Token: 0x04001343 RID: 4931
	private List<NavMeshBuildSource> sources;

	// Token: 0x04001344 RID: 4932
	private AsyncOperation BuildingOperation;

	// Token: 0x04001345 RID: 4933
	private bool HasBuildOperationStarted;

	// Token: 0x04001346 RID: 4934
	private Stopwatch BuildTimer = new Stopwatch();

	// Token: 0x04001347 RID: 4935
	private int defaultArea;

	// Token: 0x04001348 RID: 4936
	private int agentTypeId;

	// Token: 0x17000250 RID: 592
	// (get) Token: 0x06001B45 RID: 6981 RVA: 0x000C157F File Offset: 0x000BF77F
	public bool IsBuilding
	{
		get
		{
			return !this.HasBuildOperationStarted || this.BuildingOperation != null;
		}
	}

	// Token: 0x06001B46 RID: 6982 RVA: 0x000C1594 File Offset: 0x000BF794
	private void OnEnable()
	{
		this.agentTypeId = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex).agentTypeID;
		this.NavMeshData = new NavMeshData(this.agentTypeId);
		this.sources = new List<NavMeshBuildSource>();
		this.defaultArea = NavMesh.GetAreaFromName(this.DefaultAreaName);
		base.InvokeRepeating(new Action(this.FinishBuildingNavmesh), 0f, 1f);
	}

	// Token: 0x06001B47 RID: 6983 RVA: 0x000C1603 File Offset: 0x000BF803
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.CancelInvoke(new Action(this.FinishBuildingNavmesh));
		this.NavMeshDataInstance.Remove();
	}

	// Token: 0x06001B48 RID: 6984 RVA: 0x000C162C File Offset: 0x000BF82C
	[ContextMenu("Update Nav Mesh")]
	public void UpdateNavMeshAsync()
	{
		if (this.HasBuildOperationStarted)
		{
			return;
		}
		if (AiManager.nav_disable)
		{
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		UnityEngine.Debug.Log("Starting Navmesh Build with " + this.sources.Count + " sources");
		NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex);
		settingsByIndex.overrideVoxelSize = true;
		settingsByIndex.voxelSize *= 2f;
		this.BuildingOperation = NavMeshBuilder.UpdateNavMeshDataAsync(this.NavMeshData, settingsByIndex, this.sources, this.Bounds);
		this.BuildTimer.Reset();
		this.BuildTimer.Start();
		this.HasBuildOperationStarted = true;
		float num = Time.realtimeSinceStartup - realtimeSinceStartup;
		if (num > 0.1f)
		{
			UnityEngine.Debug.LogWarning("Calling UpdateNavMesh took " + num);
		}
	}

	// Token: 0x06001B49 RID: 6985 RVA: 0x000C16FC File Offset: 0x000BF8FC
	public IEnumerator UpdateNavMeshAndWait()
	{
		if (this.HasBuildOperationStarted)
		{
			yield break;
		}
		if (AiManager.nav_disable)
		{
			yield break;
		}
		this.HasBuildOperationStarted = false;
		this.Bounds.size = TerrainMeta.Size;
		NavMesh.pathfindingIterationsPerFrame = AiManager.pathfindingIterationsPerFrame;
		IEnumerator enumerator = NavMeshTools.CollectSourcesAsync(this.Bounds, this.LayerMask, this.NavMeshCollectGeometry, this.defaultArea, DynamicNavMesh.use_baked_terrain_mesh, this.AsyncTerrainNavMeshBakeCellSize, this.sources, new Action<List<NavMeshBuildSource>>(this.AppendModifierVolumes), new Action(this.UpdateNavMeshAsync), null);
		if (AiManager.nav_wait)
		{
			yield return enumerator;
		}
		else
		{
			base.StartCoroutine(enumerator);
		}
		if (!AiManager.nav_wait)
		{
			UnityEngine.Debug.Log("nav_wait is false, so we're not waiting for the navmesh to finish generating. This might cause your server to sputter while it's generating.");
			yield break;
		}
		int lastPct = 0;
		while (!this.HasBuildOperationStarted)
		{
			yield return CoroutineEx.waitForSecondsRealtime(0.25f);
		}
		while (this.BuildingOperation != null)
		{
			int num = (int)(this.BuildingOperation.progress * 100f);
			if (lastPct != num)
			{
				UnityEngine.Debug.LogFormat("{0}%", new object[]
				{
					num
				});
				lastPct = num;
			}
			yield return CoroutineEx.waitForSecondsRealtime(0.25f);
			this.FinishBuildingNavmesh();
		}
		yield break;
	}

	// Token: 0x06001B4A RID: 6986 RVA: 0x000C170C File Offset: 0x000BF90C
	private void AppendModifierVolumes(List<NavMeshBuildSource> sources)
	{
		foreach (NavMeshModifierVolume navMeshModifierVolume in NavMeshModifierVolume.activeModifiers)
		{
			if ((this.LayerMask & 1 << navMeshModifierVolume.gameObject.layer) != 0 && navMeshModifierVolume.AffectsAgentType(this.agentTypeId))
			{
				Vector3 pos = navMeshModifierVolume.transform.TransformPoint(navMeshModifierVolume.center);
				Vector3 lossyScale = navMeshModifierVolume.transform.lossyScale;
				Vector3 size = new Vector3(navMeshModifierVolume.size.x * Mathf.Abs(lossyScale.x), navMeshModifierVolume.size.y * Mathf.Abs(lossyScale.y), navMeshModifierVolume.size.z * Mathf.Abs(lossyScale.z));
				sources.Add(new NavMeshBuildSource
				{
					shape = NavMeshBuildSourceShape.ModifierBox,
					transform = Matrix4x4.TRS(pos, navMeshModifierVolume.transform.rotation, Vector3.one),
					size = size,
					area = navMeshModifierVolume.area
				});
			}
		}
	}

	// Token: 0x06001B4B RID: 6987 RVA: 0x000C1850 File Offset: 0x000BFA50
	public void FinishBuildingNavmesh()
	{
		if (this.BuildingOperation == null)
		{
			return;
		}
		if (!this.BuildingOperation.isDone)
		{
			return;
		}
		if (!this.NavMeshDataInstance.valid)
		{
			this.NavMeshDataInstance = NavMesh.AddNavMeshData(this.NavMeshData);
		}
		UnityEngine.Debug.Log(string.Format("Navmesh Build took {0:0.00} seconds", this.BuildTimer.Elapsed.TotalSeconds));
		this.BuildingOperation = null;
	}
}
