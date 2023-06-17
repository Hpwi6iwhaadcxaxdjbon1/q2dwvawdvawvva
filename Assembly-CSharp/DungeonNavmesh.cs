using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ConVar;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000208 RID: 520
public class DungeonNavmesh : FacepunchBehaviour, IServerComponent
{
	// Token: 0x04001329 RID: 4905
	public int NavMeshAgentTypeIndex;

	// Token: 0x0400132A RID: 4906
	[Tooltip("The default area associated with the NavMeshAgent index.")]
	public string DefaultAreaName = "HumanNPC";

	// Token: 0x0400132B RID: 4907
	public float NavmeshResolutionModifier = 1.25f;

	// Token: 0x0400132C RID: 4908
	[Tooltip("Bounds which are auto calculated from CellSize * CellCount")]
	public Bounds Bounds;

	// Token: 0x0400132D RID: 4909
	public NavMeshData NavMeshData;

	// Token: 0x0400132E RID: 4910
	public NavMeshDataInstance NavMeshDataInstance;

	// Token: 0x0400132F RID: 4911
	public LayerMask LayerMask;

	// Token: 0x04001330 RID: 4912
	public NavMeshCollectGeometry NavMeshCollectGeometry;

	// Token: 0x04001331 RID: 4913
	public static List<DungeonNavmesh> Instances = new List<DungeonNavmesh>();

	// Token: 0x04001332 RID: 4914
	[ServerVar]
	public static bool use_baked_terrain_mesh = true;

	// Token: 0x04001333 RID: 4915
	private List<NavMeshBuildSource> sources;

	// Token: 0x04001334 RID: 4916
	private AsyncOperation BuildingOperation;

	// Token: 0x04001335 RID: 4917
	private bool HasBuildOperationStarted;

	// Token: 0x04001336 RID: 4918
	private Stopwatch BuildTimer = new Stopwatch();

	// Token: 0x04001337 RID: 4919
	private int defaultArea;

	// Token: 0x04001338 RID: 4920
	private int agentTypeId;

	// Token: 0x06001B38 RID: 6968 RVA: 0x000C0F74 File Offset: 0x000BF174
	public static bool NavReady()
	{
		if (DungeonNavmesh.Instances == null || DungeonNavmesh.Instances.Count == 0)
		{
			return true;
		}
		using (List<DungeonNavmesh>.Enumerator enumerator = DungeonNavmesh.Instances.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsBuilding)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x1700024F RID: 591
	// (get) Token: 0x06001B39 RID: 6969 RVA: 0x000C0FE4 File Offset: 0x000BF1E4
	public bool IsBuilding
	{
		get
		{
			return !this.HasBuildOperationStarted || this.BuildingOperation != null;
		}
	}

	// Token: 0x06001B3A RID: 6970 RVA: 0x000C0FFC File Offset: 0x000BF1FC
	private void OnEnable()
	{
		this.agentTypeId = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex).agentTypeID;
		this.NavMeshData = new NavMeshData(this.agentTypeId);
		this.sources = new List<NavMeshBuildSource>();
		this.defaultArea = NavMesh.GetAreaFromName(this.DefaultAreaName);
		base.InvokeRepeating(new Action(this.FinishBuildingNavmesh), 0f, 1f);
		DungeonNavmesh.Instances.Add(this);
	}

	// Token: 0x06001B3B RID: 6971 RVA: 0x000C1076 File Offset: 0x000BF276
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.CancelInvoke(new Action(this.FinishBuildingNavmesh));
		this.NavMeshDataInstance.Remove();
		DungeonNavmesh.Instances.Remove(this);
	}

	// Token: 0x06001B3C RID: 6972 RVA: 0x000C10AC File Offset: 0x000BF2AC
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
		UnityEngine.Debug.Log("Starting Dungeon Navmesh Build with " + this.sources.Count + " sources");
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
		this.NotifyInformationZonesOfCompletion();
	}

	// Token: 0x06001B3D RID: 6973 RVA: 0x000C118C File Offset: 0x000BF38C
	public void NotifyInformationZonesOfCompletion()
	{
		foreach (AIInformationZone aiinformationZone in AIInformationZone.zones)
		{
			aiinformationZone.NavmeshBuildingComplete();
		}
	}

	// Token: 0x06001B3E RID: 6974 RVA: 0x000C11DC File Offset: 0x000BF3DC
	public void SourcesCollected()
	{
		int count = this.sources.Count;
		UnityEngine.Debug.Log("Source count Pre cull : " + this.sources.Count);
		for (int i = this.sources.Count - 1; i >= 0; i--)
		{
			NavMeshBuildSource item = this.sources[i];
			Matrix4x4 transform = item.transform;
			Vector3 vector = new Vector3(transform[0, 3], transform[1, 3], transform[2, 3]);
			bool flag = false;
			using (List<AIInformationZone>.Enumerator enumerator = AIInformationZone.zones.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (Vector3Ex.Distance2D(enumerator.Current.ClosestPointTo(vector), vector) <= 50f)
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				this.sources.Remove(item);
			}
		}
		UnityEngine.Debug.Log(string.Concat(new object[]
		{
			"Source count post cull : ",
			this.sources.Count,
			" total removed : ",
			count - this.sources.Count
		}));
	}

	// Token: 0x06001B3F RID: 6975 RVA: 0x000C131C File Offset: 0x000BF51C
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
		this.Bounds.size = new Vector3(1000000f, 100000f, 100000f);
		IEnumerator enumerator = NavMeshTools.CollectSourcesAsync(base.transform, this.LayerMask.value, this.NavMeshCollectGeometry, this.defaultArea, this.sources, new Action<List<NavMeshBuildSource>>(this.AppendModifierVolumes), new Action(this.UpdateNavMeshAsync));
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

	// Token: 0x06001B40 RID: 6976 RVA: 0x000C132C File Offset: 0x000BF52C
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

	// Token: 0x06001B41 RID: 6977 RVA: 0x000C1484 File Offset: 0x000BF684
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

	// Token: 0x06001B42 RID: 6978 RVA: 0x000C14F4 File Offset: 0x000BF6F4
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta * new Color(1f, 1f, 1f, 0.5f);
		Gizmos.DrawCube(base.transform.position, this.Bounds.size);
	}
}
