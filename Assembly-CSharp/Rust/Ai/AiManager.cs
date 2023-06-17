using System;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	// Token: 0x02000B3C RID: 2876
	[DefaultExecutionOrder(-103)]
	public class AiManager : SingletonComponent<AiManager>, IServerComponent
	{
		// Token: 0x04003E24 RID: 15908
		[Header("Cover System")]
		[SerializeField]
		public bool UseCover = true;

		// Token: 0x04003E25 RID: 15909
		public float CoverPointVolumeCellSize = 20f;

		// Token: 0x04003E26 RID: 15910
		public float CoverPointVolumeCellHeight = 8f;

		// Token: 0x04003E27 RID: 15911
		public float CoverPointRayLength = 1f;

		// Token: 0x04003E28 RID: 15912
		public CoverPointVolume cpvPrefab;

		// Token: 0x04003E29 RID: 15913
		[SerializeField]
		public LayerMask DynamicCoverPointVolumeLayerMask;

		// Token: 0x04003E2A RID: 15914
		private WorldSpaceGrid<CoverPointVolume> coverPointVolumeGrid;

		// Token: 0x04003E2B RID: 15915
		[ServerVar(Help = "If true we'll wait for the navmesh to generate before completely starting the server. This might cause your server to hitch and lag as it generates in the background.")]
		public static bool nav_wait = true;

		// Token: 0x04003E2C RID: 15916
		[ServerVar(Help = "If set to true the navmesh won't generate.. which means Ai that uses the navmesh won't be able to move")]
		public static bool nav_disable = false;

		// Token: 0x04003E2D RID: 15917
		[ServerVar(Help = "If set to true, npcs will attempt to place themselves on the navmesh if not on a navmesh when set destination is called.")]
		public static bool setdestination_navmesh_failsafe = false;

		// Token: 0x04003E2E RID: 15918
		[ServerVar(Help = "If ai_dormant is true, any npc outside the range of players will render itself dormant and take up less resources, but wildlife won't simulate as well.")]
		public static bool ai_dormant = true;

		// Token: 0x04003E2F RID: 15919
		[ServerVar(Help = "If an agent is beyond this distance to a player, it's flagged for becoming dormant.")]
		public static float ai_to_player_distance_wakeup_range = 160f;

		// Token: 0x04003E30 RID: 15920
		[ServerVar(Help = "nav_obstacles_carve_state defines which obstacles can carve the terrain. 0 - No carving, 1 - Only player construction carves, 2 - All obstacles carve.")]
		public static int nav_obstacles_carve_state = 2;

		// Token: 0x04003E31 RID: 15921
		[ServerVar(Help = "ai_dormant_max_wakeup_per_tick defines the maximum number of dormant agents we will wake up in a single tick. (default: 30)")]
		public static int ai_dormant_max_wakeup_per_tick = 30;

		// Token: 0x04003E32 RID: 15922
		[ServerVar(Help = "ai_htn_player_tick_budget defines the maximum amount of milliseconds ticking htn player agents are allowed to consume. (default: 4 ms)")]
		public static float ai_htn_player_tick_budget = 4f;

		// Token: 0x04003E33 RID: 15923
		[ServerVar(Help = "ai_htn_player_junkpile_tick_budget defines the maximum amount of milliseconds ticking htn player junkpile agents are allowed to consume. (default: 4 ms)")]
		public static float ai_htn_player_junkpile_tick_budget = 4f;

		// Token: 0x04003E34 RID: 15924
		[ServerVar(Help = "ai_htn_animal_tick_budget defines the maximum amount of milliseconds ticking htn animal agents are allowed to consume. (default: 4 ms)")]
		public static float ai_htn_animal_tick_budget = 4f;

		// Token: 0x04003E35 RID: 15925
		[ServerVar(Help = "If ai_htn_use_agency_tick is true, the ai manager's agency system will tick htn agents at the ms budgets defined in ai_htn_player_tick_budget and ai_htn_animal_tick_budget. If it's false, each agent registers with the invoke system individually, with no frame-budget restrictions. (default: true)")]
		public static bool ai_htn_use_agency_tick = true;

		// Token: 0x04003E36 RID: 15926
		private readonly BasePlayer[] playerVicinityQuery = new BasePlayer[1];

		// Token: 0x04003E37 RID: 15927
		private readonly Func<BasePlayer, bool> filter = new Func<BasePlayer, bool>(AiManager.InterestedInPlayersOnly);

		// Token: 0x060045BD RID: 17853 RVA: 0x000063A5 File Offset: 0x000045A5
		internal void OnEnableAgency()
		{
		}

		// Token: 0x060045BE RID: 17854 RVA: 0x000063A5 File Offset: 0x000045A5
		internal void OnDisableAgency()
		{
		}

		// Token: 0x060045BF RID: 17855 RVA: 0x000063A5 File Offset: 0x000045A5
		internal void UpdateAgency()
		{
		}

		// Token: 0x060045C0 RID: 17856 RVA: 0x00197324 File Offset: 0x00195524
		internal void OnEnableCover()
		{
			if (this.coverPointVolumeGrid == null)
			{
				Vector3 size = TerrainMeta.Size;
				this.coverPointVolumeGrid = new WorldSpaceGrid<CoverPointVolume>(size.x, this.CoverPointVolumeCellSize);
			}
		}

		// Token: 0x060045C1 RID: 17857 RVA: 0x00197358 File Offset: 0x00195558
		internal void OnDisableCover()
		{
			if (this.coverPointVolumeGrid == null || this.coverPointVolumeGrid.Cells == null)
			{
				return;
			}
			for (int i = 0; i < this.coverPointVolumeGrid.Cells.Length; i++)
			{
				UnityEngine.Object.Destroy(this.coverPointVolumeGrid.Cells[i]);
			}
		}

		// Token: 0x060045C2 RID: 17858 RVA: 0x001973A8 File Offset: 0x001955A8
		public static CoverPointVolume CreateNewCoverVolume(Vector3 point, Transform coverPointGroup)
		{
			if (SingletonComponent<AiManager>.Instance != null && SingletonComponent<AiManager>.Instance.enabled && SingletonComponent<AiManager>.Instance.UseCover)
			{
				CoverPointVolume coverPointVolume = SingletonComponent<AiManager>.Instance.GetCoverVolumeContaining(point);
				if (coverPointVolume == null)
				{
					Vector2i vector2i = SingletonComponent<AiManager>.Instance.coverPointVolumeGrid.WorldToGridCoords(point);
					if (SingletonComponent<AiManager>.Instance.cpvPrefab != null)
					{
						coverPointVolume = UnityEngine.Object.Instantiate<CoverPointVolume>(SingletonComponent<AiManager>.Instance.cpvPrefab);
					}
					else
					{
						coverPointVolume = new GameObject("CoverPointVolume").AddComponent<CoverPointVolume>();
					}
					coverPointVolume.transform.localPosition = default(Vector3);
					coverPointVolume.transform.position = SingletonComponent<AiManager>.Instance.coverPointVolumeGrid.GridToWorldCoords(vector2i) + Vector3.up * point.y;
					coverPointVolume.transform.localScale = new Vector3(SingletonComponent<AiManager>.Instance.CoverPointVolumeCellSize, SingletonComponent<AiManager>.Instance.CoverPointVolumeCellHeight, SingletonComponent<AiManager>.Instance.CoverPointVolumeCellSize);
					coverPointVolume.CoverLayerMask = SingletonComponent<AiManager>.Instance.DynamicCoverPointVolumeLayerMask;
					coverPointVolume.CoverPointRayLength = SingletonComponent<AiManager>.Instance.CoverPointRayLength;
					SingletonComponent<AiManager>.Instance.coverPointVolumeGrid[vector2i] = coverPointVolume;
					coverPointVolume.GenerateCoverPoints(coverPointGroup);
				}
				return coverPointVolume;
			}
			return null;
		}

		// Token: 0x060045C3 RID: 17859 RVA: 0x001974F0 File Offset: 0x001956F0
		public CoverPointVolume GetCoverVolumeContaining(Vector3 point)
		{
			if (this.coverPointVolumeGrid == null)
			{
				return null;
			}
			Vector2i cellCoords = this.coverPointVolumeGrid.WorldToGridCoords(point);
			return this.coverPointVolumeGrid[cellCoords];
		}

		// Token: 0x1700065F RID: 1631
		// (get) Token: 0x060045C5 RID: 17861 RVA: 0x00197528 File Offset: 0x00195728
		// (set) Token: 0x060045C4 RID: 17860 RVA: 0x00197520 File Offset: 0x00195720
		[ServerVar(Help = "The maximum amount of nodes processed each frame in the asynchronous pathfinding process. Increasing this value will cause the paths to be processed faster, but can cause some hiccups in frame rate. Default value is 100, a good range for tuning is between 50 and 500.")]
		public static int pathfindingIterationsPerFrame
		{
			get
			{
				return NavMesh.pathfindingIterationsPerFrame;
			}
			set
			{
				NavMesh.pathfindingIterationsPerFrame = value;
			}
		}

		// Token: 0x17000660 RID: 1632
		// (get) Token: 0x060045C6 RID: 17862 RVA: 0x0000441C File Offset: 0x0000261C
		public bool repeat
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060045C7 RID: 17863 RVA: 0x0019752F File Offset: 0x0019572F
		public void Initialize()
		{
			this.OnEnableAgency();
			if (this.UseCover)
			{
				this.OnEnableCover();
			}
		}

		// Token: 0x060045C8 RID: 17864 RVA: 0x00197545 File Offset: 0x00195745
		private void OnDisable()
		{
			if (Application.isQuitting)
			{
				return;
			}
			this.OnDisableAgency();
			if (this.UseCover)
			{
				this.OnDisableCover();
			}
		}

		// Token: 0x060045C9 RID: 17865 RVA: 0x00197563 File Offset: 0x00195763
		public float? ExecuteUpdate(float deltaTime, float nextInterval)
		{
			if (AiManager.nav_disable)
			{
				return new float?(nextInterval);
			}
			this.UpdateAgency();
			return new float?(UnityEngine.Random.value + 1f);
		}

		// Token: 0x060045CA RID: 17866 RVA: 0x0019758C File Offset: 0x0019578C
		private static bool InterestedInPlayersOnly(BaseEntity entity)
		{
			BasePlayer basePlayer = entity as BasePlayer;
			return !(basePlayer == null) && !basePlayer.IsSleeping() && basePlayer.IsConnected;
		}
	}
}
