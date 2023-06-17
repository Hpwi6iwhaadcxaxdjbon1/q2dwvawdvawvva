using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ConVar;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200020A RID: 522
public class MonumentNavMesh : FacepunchBehaviour, IServerComponent
{
	// Token: 0x04001349 RID: 4937
	public int NavMeshAgentTypeIndex;

	// Token: 0x0400134A RID: 4938
	[Tooltip("The default area associated with the NavMeshAgent index.")]
	public string DefaultAreaName = "HumanNPC";

	// Token: 0x0400134B RID: 4939
	[Tooltip("How many cells to use squared")]
	public int CellCount = 1;

	// Token: 0x0400134C RID: 4940
	[Tooltip("The size of each cell for async object gathering")]
	public int CellSize = 80;

	// Token: 0x0400134D RID: 4941
	public int Height = 100;

	// Token: 0x0400134E RID: 4942
	public float NavmeshResolutionModifier = 0.5f;

	// Token: 0x0400134F RID: 4943
	[Tooltip("Use the bounds specified in editor instead of generating it from cellsize * cellcount")]
	public bool overrideAutoBounds;

	// Token: 0x04001350 RID: 4944
	[Tooltip("Bounds which are auto calculated from CellSize * CellCount")]
	public Bounds Bounds;

	// Token: 0x04001351 RID: 4945
	public NavMeshData NavMeshData;

	// Token: 0x04001352 RID: 4946
	public NavMeshDataInstance NavMeshDataInstance;

	// Token: 0x04001353 RID: 4947
	public LayerMask LayerMask;

	// Token: 0x04001354 RID: 4948
	public NavMeshCollectGeometry NavMeshCollectGeometry;

	// Token: 0x04001355 RID: 4949
	public bool forceCollectTerrain;

	// Token: 0x04001356 RID: 4950
	public bool shouldNotifyAIZones = true;

	// Token: 0x04001357 RID: 4951
	public Transform CustomNavMeshRoot;

	// Token: 0x04001358 RID: 4952
	[ServerVar]
	public static bool use_baked_terrain_mesh = true;

	// Token: 0x04001359 RID: 4953
	private List<NavMeshBuildSource> sources;

	// Token: 0x0400135A RID: 4954
	private AsyncOperation BuildingOperation;

	// Token: 0x0400135B RID: 4955
	private bool HasBuildOperationStarted;

	// Token: 0x0400135C RID: 4956
	private Stopwatch BuildTimer = new Stopwatch();

	// Token: 0x0400135D RID: 4957
	private int defaultArea;

	// Token: 0x0400135E RID: 4958
	private int agentTypeId;

	// Token: 0x17000251 RID: 593
	// (get) Token: 0x06001B4E RID: 6990 RVA: 0x000C18EE File Offset: 0x000BFAEE
	public bool IsBuilding
	{
		get
		{
			return !this.HasBuildOperationStarted || this.BuildingOperation != null;
		}
	}

	// Token: 0x06001B4F RID: 6991 RVA: 0x000C1904 File Offset: 0x000BFB04
	private void OnEnable()
	{
		this.agentTypeId = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex).agentTypeID;
		this.NavMeshData = new NavMeshData(this.agentTypeId);
		this.sources = new List<NavMeshBuildSource>();
		this.defaultArea = NavMesh.GetAreaFromName(this.DefaultAreaName);
		base.InvokeRepeating(new Action(this.FinishBuildingNavmesh), 0f, 1f);
	}

	// Token: 0x06001B50 RID: 6992 RVA: 0x000C1973 File Offset: 0x000BFB73
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.CancelInvoke(new Action(this.FinishBuildingNavmesh));
		this.NavMeshDataInstance.Remove();
	}

	// Token: 0x06001B51 RID: 6993 RVA: 0x000C199C File Offset: 0x000BFB9C
	[ContextMenu("Update Monument Nav Mesh")]
	public void UpdateNavMeshAsync()
	{
		if (this.HasBuildOperationStarted)
		{
			return;
		}
		if (AiManager.nav_disable || !AI.npc_enable)
		{
			return;
		}
		float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
		UnityEngine.Debug.Log("Starting Monument Navmesh Build with " + this.sources.Count + " sources");
		NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex);
		settingsByIndex.overrideVoxelSize = true;
		settingsByIndex.voxelSize *= this.NavmeshResolutionModifier;
		this.BuildingOperation = NavMeshBuilder.UpdateNavMeshDataAsync(this.NavMeshData, settingsByIndex, this.sources, this.Bounds);
		this.BuildTimer.Reset();
		this.BuildTimer.Start();
		this.HasBuildOperationStarted = true;
		float num = UnityEngine.Time.realtimeSinceStartup - realtimeSinceStartup;
		if (num > 0.1f)
		{
			UnityEngine.Debug.LogWarning("Calling UpdateNavMesh took " + num);
		}
		if (this.shouldNotifyAIZones)
		{
			this.NotifyInformationZonesOfCompletion();
		}
	}

	// Token: 0x06001B52 RID: 6994 RVA: 0x000C1A82 File Offset: 0x000BFC82
	public IEnumerator UpdateNavMeshAndWait()
	{
		if (this.HasBuildOperationStarted)
		{
			yield break;
		}
		if (AiManager.nav_disable || !AI.npc_enable)
		{
			yield break;
		}
		this.HasBuildOperationStarted = false;
		this.Bounds.center = base.transform.position;
		if (!this.overrideAutoBounds)
		{
			this.Bounds.size = new Vector3((float)(this.CellSize * this.CellCount), (float)this.Height, (float)(this.CellSize * this.CellCount));
		}
		IEnumerator enumerator = NavMeshTools.CollectSourcesAsync(this.Bounds, this.LayerMask, this.NavMeshCollectGeometry, this.defaultArea, MonumentNavMesh.use_baked_terrain_mesh && !this.forceCollectTerrain, this.CellSize, this.sources, new Action<List<NavMeshBuildSource>>(this.AppendModifierVolumes), new Action(this.UpdateNavMeshAsync), this.CustomNavMeshRoot);
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

	// Token: 0x06001B53 RID: 6995 RVA: 0x000C1A94 File Offset: 0x000BFC94
	public void NotifyInformationZonesOfCompletion()
	{
		foreach (AIInformationZone aiinformationZone in AIInformationZone.zones)
		{
			aiinformationZone.NavmeshBuildingComplete();
		}
	}

	// Token: 0x06001B54 RID: 6996 RVA: 0x000C1AE4 File Offset: 0x000BFCE4
	private void AppendModifierVolumes(List<NavMeshBuildSource> sources)
	{
		foreach (NavMeshModifierVolume navMeshModifierVolume in NavMeshModifierVolume.activeModifiers)
		{
			if ((this.LayerMask & 1 << navMeshModifierVolume.gameObject.layer) != 0 && navMeshModifierVolume.AffectsAgentType(this.agentTypeId))
			{
				Vector3 vector = navMeshModifierVolume.transform.TransformPoint(navMeshModifierVolume.center);
				if (this.Bounds.Contains(vector))
				{
					Vector3 lossyScale = navMeshModifierVolume.transform.lossyScale;
					Vector3 size = new Vector3(navMeshModifierVolume.size.x * Mathf.Abs(lossyScale.x), navMeshModifierVolume.size.y * Mathf.Abs(lossyScale.y), navMeshModifierVolume.size.z * Mathf.Abs(lossyScale.z));
					sources.Add(new NavMeshBuildSource
					{
						shape = NavMeshBuildSourceShape.ModifierBox,
						transform = Matrix4x4.TRS(vector, navMeshModifierVolume.transform.rotation, Vector3.one),
						size = size,
						area = navMeshModifierVolume.area
					});
				}
			}
		}
	}

	// Token: 0x06001B55 RID: 6997 RVA: 0x000C1C3C File Offset: 0x000BFE3C
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
		UnityEngine.Debug.Log(string.Format("Monument Navmesh Build took {0:0.00} seconds", this.BuildTimer.Elapsed.TotalSeconds));
		this.BuildingOperation = null;
	}

	// Token: 0x06001B56 RID: 6998 RVA: 0x000C1CAC File Offset: 0x000BFEAC
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta * new Color(1f, 1f, 1f, 0.5f);
		Gizmos.DrawCube(base.transform.position + this.Bounds.center, this.Bounds.size);
	}
}
