using System;
using System.Collections.Generic;
using ConVar;
using Rust.AI;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000201 RID: 513
public class BaseNavigator : BaseMonoBehaviour
{
	// Token: 0x040012DA RID: 4826
	[ServerVar(Help = "The max step-up height difference for pet base navigation")]
	public static float maxStepUpDistance = 1.7f;

	// Token: 0x040012DB RID: 4827
	[ServerVar(Help = "How many frames between base navigation movement updates")]
	public static int baseNavMovementFrameInterval = 2;

	// Token: 0x040012DC RID: 4828
	[ServerVar(Help = "How long we are not moving for before trigger the stuck event")]
	public static float stuckTriggerDuration = 10f;

	// Token: 0x040012DD RID: 4829
	[ServerVar]
	public static float navTypeHeightOffset = 0.5f;

	// Token: 0x040012DE RID: 4830
	[ServerVar]
	public static float navTypeDistance = 1f;

	// Token: 0x040012DF RID: 4831
	[Header("General")]
	public bool CanNavigateMounted;

	// Token: 0x040012E0 RID: 4832
	public bool CanUseNavMesh = true;

	// Token: 0x040012E1 RID: 4833
	public bool CanUseAStar = true;

	// Token: 0x040012E2 RID: 4834
	public bool CanUseBaseNav;

	// Token: 0x040012E3 RID: 4835
	public bool CanUseCustomNav;

	// Token: 0x040012E4 RID: 4836
	public float StoppingDistance = 0.5f;

	// Token: 0x040012E5 RID: 4837
	public string DefaultArea = "Walkable";

	// Token: 0x040012E6 RID: 4838
	[Header("Stuck Detection")]
	public bool TriggerStuckEvent;

	// Token: 0x040012E7 RID: 4839
	public float StuckDistance = 1f;

	// Token: 0x040012E8 RID: 4840
	[Header("Speed")]
	public float Speed = 5f;

	// Token: 0x040012E9 RID: 4841
	public float Acceleration = 5f;

	// Token: 0x040012EA RID: 4842
	public float TurnSpeed = 10f;

	// Token: 0x040012EB RID: 4843
	public BaseNavigator.NavigationSpeed MoveTowardsSpeed = BaseNavigator.NavigationSpeed.Normal;

	// Token: 0x040012EC RID: 4844
	public bool FaceMoveTowardsTarget;

	// Token: 0x040012ED RID: 4845
	[Header("Speed Fractions")]
	public float SlowestSpeedFraction = 0.16f;

	// Token: 0x040012EE RID: 4846
	public float SlowSpeedFraction = 0.3f;

	// Token: 0x040012EF RID: 4847
	public float NormalSpeedFraction = 0.5f;

	// Token: 0x040012F0 RID: 4848
	public float FastSpeedFraction = 1f;

	// Token: 0x040012F1 RID: 4849
	public float LowHealthSpeedReductionTriggerFraction;

	// Token: 0x040012F2 RID: 4850
	public float LowHealthMaxSpeedFraction = 0.5f;

	// Token: 0x040012F3 RID: 4851
	public float SwimmingSpeedMultiplier = 0.25f;

	// Token: 0x040012F4 RID: 4852
	[Header("AIPoint Usage")]
	public float BestMovementPointMaxDistance = 10f;

	// Token: 0x040012F5 RID: 4853
	public float BestCoverPointMaxDistance = 20f;

	// Token: 0x040012F6 RID: 4854
	public float BestRoamPointMaxDistance = 20f;

	// Token: 0x040012F7 RID: 4855
	public float MaxRoamDistanceFromHome = -1f;

	// Token: 0x040012F8 RID: 4856
	[Header("Misc")]
	public float MaxWaterDepth = 0.75f;

	// Token: 0x040012F9 RID: 4857
	public bool SpeedBasedAvoidancePriority;

	// Token: 0x040012FA RID: 4858
	private NavMeshPath path;

	// Token: 0x040012FB RID: 4859
	private NavMeshQueryFilter navMeshQueryFilter;

	// Token: 0x040012FE RID: 4862
	private int defaultAreaMask;

	// Token: 0x040012FF RID: 4863
	[InspectorFlags]
	public TerrainBiome.Enum biomePreference = (TerrainBiome.Enum)12;

	// Token: 0x04001300 RID: 4864
	public bool UseBiomePreference;

	// Token: 0x04001301 RID: 4865
	[InspectorFlags]
	public TerrainTopology.Enum topologyPreference = (TerrainTopology.Enum)96;

	// Token: 0x04001302 RID: 4866
	[InspectorFlags]
	public TerrainTopology.Enum topologyPrevent;

	// Token: 0x04001303 RID: 4867
	[InspectorFlags]
	public TerrainBiome.Enum biomeRequirement;

	// Token: 0x04001309 RID: 4873
	private float stuckTimer;

	// Token: 0x0400130A RID: 4874
	private Vector3 stuckCheckPosition;

	// Token: 0x0400130C RID: 4876
	protected bool traversingNavMeshLink;

	// Token: 0x0400130D RID: 4877
	protected string currentNavMeshLinkName;

	// Token: 0x0400130E RID: 4878
	protected Vector3 currentNavMeshLinkEndPos;

	// Token: 0x0400130F RID: 4879
	protected Stack<IAIPathNode> currentAStarPath;

	// Token: 0x04001310 RID: 4880
	protected IAIPathNode targetNode;

	// Token: 0x04001311 RID: 4881
	protected float currentSpeedFraction = 1f;

	// Token: 0x04001312 RID: 4882
	private float lastSetDestinationTime;

	// Token: 0x04001313 RID: 4883
	protected BaseNavigator.OverrideFacingDirectionMode overrideFacingDirectionMode;

	// Token: 0x04001314 RID: 4884
	protected BaseEntity facingDirectionEntity;

	// Token: 0x04001315 RID: 4885
	protected bool overrideFacingDirection;

	// Token: 0x04001316 RID: 4886
	protected Vector3 facingDirectionOverride;

	// Token: 0x04001317 RID: 4887
	protected bool paused;

	// Token: 0x04001318 RID: 4888
	private int frameCount;

	// Token: 0x04001319 RID: 4889
	private float accumDelta;

	// Token: 0x1700023F RID: 575
	// (get) Token: 0x06001ACC RID: 6860 RVA: 0x000BF423 File Offset: 0x000BD623
	// (set) Token: 0x06001ACD RID: 6861 RVA: 0x000BF42B File Offset: 0x000BD62B
	public AIMovePointPath Path { get; set; }

	// Token: 0x17000240 RID: 576
	// (get) Token: 0x06001ACE RID: 6862 RVA: 0x000BF434 File Offset: 0x000BD634
	// (set) Token: 0x06001ACF RID: 6863 RVA: 0x000BF43C File Offset: 0x000BD63C
	public BasePath AStarGraph { get; set; }

	// Token: 0x06001AD0 RID: 6864 RVA: 0x000BF445 File Offset: 0x000BD645
	public int TopologyPreference()
	{
		return (int)this.topologyPreference;
	}

	// Token: 0x06001AD1 RID: 6865 RVA: 0x000BF44D File Offset: 0x000BD64D
	public int TopologyPrevent()
	{
		return (int)this.topologyPrevent;
	}

	// Token: 0x06001AD2 RID: 6866 RVA: 0x000BF455 File Offset: 0x000BD655
	public int BiomeRequirement()
	{
		return (int)this.biomeRequirement;
	}

	// Token: 0x17000241 RID: 577
	// (get) Token: 0x06001AD3 RID: 6867 RVA: 0x000BF45D File Offset: 0x000BD65D
	// (set) Token: 0x06001AD4 RID: 6868 RVA: 0x000BF465 File Offset: 0x000BD665
	public NavMeshAgent Agent { get; private set; }

	// Token: 0x17000242 RID: 578
	// (get) Token: 0x06001AD5 RID: 6869 RVA: 0x000BF46E File Offset: 0x000BD66E
	// (set) Token: 0x06001AD6 RID: 6870 RVA: 0x000BF476 File Offset: 0x000BD676
	public BaseCombatEntity BaseEntity { get; private set; }

	// Token: 0x17000243 RID: 579
	// (get) Token: 0x06001AD7 RID: 6871 RVA: 0x000BF47F File Offset: 0x000BD67F
	// (set) Token: 0x06001AD8 RID: 6872 RVA: 0x000BF487 File Offset: 0x000BD687
	public Vector3 Destination { get; protected set; }

	// Token: 0x17000244 RID: 580
	// (get) Token: 0x06001AD9 RID: 6873 RVA: 0x000BF490 File Offset: 0x000BD690
	public virtual bool IsOnNavMeshLink
	{
		get
		{
			return this.Agent.enabled && this.Agent.isOnOffMeshLink;
		}
	}

	// Token: 0x17000245 RID: 581
	// (get) Token: 0x06001ADA RID: 6874 RVA: 0x000BF4AC File Offset: 0x000BD6AC
	public bool Moving
	{
		get
		{
			return this.CurrentNavigationType > BaseNavigator.NavigationType.None;
		}
	}

	// Token: 0x17000246 RID: 582
	// (get) Token: 0x06001ADB RID: 6875 RVA: 0x000BF4B7 File Offset: 0x000BD6B7
	// (set) Token: 0x06001ADC RID: 6876 RVA: 0x000BF4BF File Offset: 0x000BD6BF
	public BaseNavigator.NavigationType CurrentNavigationType { get; private set; }

	// Token: 0x17000247 RID: 583
	// (get) Token: 0x06001ADD RID: 6877 RVA: 0x000BF4C8 File Offset: 0x000BD6C8
	// (set) Token: 0x06001ADE RID: 6878 RVA: 0x000BF4D0 File Offset: 0x000BD6D0
	public BaseNavigator.NavigationType LastUsedNavigationType { get; private set; }

	// Token: 0x17000248 RID: 584
	// (get) Token: 0x06001ADF RID: 6879 RVA: 0x000BF4D9 File Offset: 0x000BD6D9
	// (set) Token: 0x06001AE0 RID: 6880 RVA: 0x000BF4E1 File Offset: 0x000BD6E1
	[HideInInspector]
	public bool StuckOffNavmesh { get; private set; }

	// Token: 0x17000249 RID: 585
	// (get) Token: 0x06001AE1 RID: 6881 RVA: 0x000BF4EA File Offset: 0x000BD6EA
	public virtual bool HasPath
	{
		get
		{
			return !(this.Agent == null) && ((this.Agent.enabled && this.Agent.hasPath) || this.currentAStarPath != null);
		}
	}

	// Token: 0x06001AE2 RID: 6882 RVA: 0x000BF524 File Offset: 0x000BD724
	public virtual void Init(BaseCombatEntity entity, NavMeshAgent agent)
	{
		this.defaultAreaMask = 1 << NavMesh.GetAreaFromName(this.DefaultArea);
		this.BaseEntity = entity;
		this.Agent = agent;
		if (this.Agent != null)
		{
			this.Agent.acceleration = this.Acceleration;
			this.Agent.angularSpeed = this.TurnSpeed;
		}
		this.navMeshQueryFilter = default(NavMeshQueryFilter);
		this.navMeshQueryFilter.agentTypeID = this.Agent.agentTypeID;
		this.navMeshQueryFilter.areaMask = this.defaultAreaMask;
		this.path = new NavMeshPath();
		this.SetCurrentNavigationType(BaseNavigator.NavigationType.None);
	}

	// Token: 0x06001AE3 RID: 6883 RVA: 0x000BF5CC File Offset: 0x000BD7CC
	public void SetNavMeshEnabled(bool flag)
	{
		if (this.Agent == null)
		{
			return;
		}
		if (this.Agent.enabled == flag)
		{
			return;
		}
		if (AiManager.nav_disable)
		{
			this.Agent.enabled = false;
			return;
		}
		if (this.Agent.enabled)
		{
			if (flag)
			{
				if (this.Agent.isOnNavMesh)
				{
					this.Agent.isStopped = false;
				}
			}
			else if (this.Agent.isOnNavMesh)
			{
				this.Agent.isStopped = true;
			}
		}
		this.Agent.enabled = flag;
		if (flag)
		{
			if (!this.CanEnableNavMeshNavigation())
			{
				return;
			}
			this.PlaceOnNavMesh();
		}
	}

	// Token: 0x06001AE4 RID: 6884 RVA: 0x000BF66E File Offset: 0x000BD86E
	protected virtual bool CanEnableNavMeshNavigation()
	{
		return this.CanUseNavMesh;
	}

	// Token: 0x06001AE5 RID: 6885 RVA: 0x000BF67B File Offset: 0x000BD87B
	protected virtual bool CanUpdateMovement()
	{
		return !(this.BaseEntity != null) || this.BaseEntity.IsAlive();
	}

	// Token: 0x06001AE6 RID: 6886 RVA: 0x000BF69B File Offset: 0x000BD89B
	public void ForceToGround()
	{
		base.CancelInvoke(new Action(this.DelayedForceToGround));
		base.Invoke(new Action(this.DelayedForceToGround), 0.5f);
	}

	// Token: 0x06001AE7 RID: 6887 RVA: 0x000BF6C8 File Offset: 0x000BD8C8
	private void DelayedForceToGround()
	{
		int layerMask = 10551296;
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(base.transform.position + Vector3.up * 0.5f, Vector3.down, out raycastHit, 1000f, layerMask))
		{
			this.BaseEntity.ServerPosition = raycastHit.point;
		}
	}

	// Token: 0x06001AE8 RID: 6888 RVA: 0x000BF720 File Offset: 0x000BD920
	public bool PlaceOnNavMesh()
	{
		if (this.Agent.isOnNavMesh)
		{
			return true;
		}
		float maxRange = this.IsSwimming() ? 30f : 6f;
		Vector3 position;
		bool result;
		if (this.GetNearestNavmeshPosition(base.transform.position + Vector3.one * 2f, out position, maxRange))
		{
			result = this.Warp(position);
		}
		else
		{
			result = false;
			this.StuckOffNavmesh = true;
			Debug.LogWarning(string.Concat(new object[]
			{
				base.gameObject.name,
				" failed to sample navmesh at position ",
				base.transform.position,
				" on area: ",
				this.DefaultArea
			}), base.gameObject);
		}
		return result;
	}

	// Token: 0x06001AE9 RID: 6889 RVA: 0x000BF7E4 File Offset: 0x000BD9E4
	private bool Warp(Vector3 position)
	{
		this.Agent.Warp(position);
		this.Agent.enabled = true;
		base.transform.position = position;
		if (!this.Agent.isOnNavMesh)
		{
			Debug.LogWarning("Agent still not on navmesh after a warp. No navmesh areas matching agent type? Agent type: " + this.Agent.agentTypeID, base.gameObject);
			this.StuckOffNavmesh = true;
			return false;
		}
		this.StuckOffNavmesh = false;
		return true;
	}

	// Token: 0x06001AEA RID: 6890 RVA: 0x000BF85C File Offset: 0x000BDA5C
	public bool GetNearestNavmeshPosition(Vector3 target, out Vector3 position, float maxRange)
	{
		position = base.transform.position;
		bool result = true;
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(target, out navMeshHit, maxRange, this.defaultAreaMask))
		{
			position = navMeshHit.position;
		}
		else
		{
			result = false;
		}
		return result;
	}

	// Token: 0x06001AEB RID: 6891 RVA: 0x000BF89F File Offset: 0x000BDA9F
	public bool SetBaseDestination(Vector3 pos, float speedFraction)
	{
		if (!AI.move)
		{
			return false;
		}
		if (!AI.navthink)
		{
			return false;
		}
		this.paused = false;
		this.currentSpeedFraction = speedFraction;
		if (this.ReachedPosition(pos))
		{
			return true;
		}
		this.Destination = pos;
		this.SetCurrentNavigationType(BaseNavigator.NavigationType.Base);
		return true;
	}

	// Token: 0x06001AEC RID: 6892 RVA: 0x000BF8DC File Offset: 0x000BDADC
	public bool SetDestination(BasePath path, IAIPathNode newTargetNode, float speedFraction)
	{
		if (!AI.move)
		{
			return false;
		}
		if (!AI.navthink)
		{
			return false;
		}
		this.paused = false;
		if (!this.CanUseAStar)
		{
			return false;
		}
		if (newTargetNode == this.targetNode && this.HasPath)
		{
			return true;
		}
		if (this.ReachedPosition(newTargetNode.Position))
		{
			return true;
		}
		IAIPathNode closestToPoint = path.GetClosestToPoint(base.transform.position);
		if (closestToPoint == null || !closestToPoint.IsValid())
		{
			return false;
		}
		float num;
		if (AStarPath.FindPath(closestToPoint, newTargetNode, out this.currentAStarPath, out num))
		{
			this.currentSpeedFraction = speedFraction;
			this.targetNode = newTargetNode;
			this.SetCurrentNavigationType(BaseNavigator.NavigationType.AStar);
			this.Destination = newTargetNode.Position;
			return true;
		}
		return false;
	}

	// Token: 0x06001AED RID: 6893 RVA: 0x000BF983 File Offset: 0x000BDB83
	public bool SetDestination(Vector3 pos, BaseNavigator.NavigationSpeed speed, float updateInterval = 0f, float navmeshSampleDistance = 0f)
	{
		return this.SetDestination(pos, this.GetSpeedFraction(speed), updateInterval, navmeshSampleDistance);
	}

	// Token: 0x06001AEE RID: 6894 RVA: 0x000BF996 File Offset: 0x000BDB96
	protected virtual bool SetCustomDestination(Vector3 pos, float speedFraction = 1f, float updateInterval = 0f)
	{
		if (!AI.move)
		{
			return false;
		}
		if (!AI.navthink)
		{
			return false;
		}
		if (!this.CanUseCustomNav)
		{
			return false;
		}
		this.paused = false;
		if (this.ReachedPosition(pos))
		{
			return true;
		}
		this.currentSpeedFraction = speedFraction;
		this.SetCurrentNavigationType(BaseNavigator.NavigationType.Custom);
		return true;
	}

	// Token: 0x06001AEF RID: 6895 RVA: 0x000BF9D8 File Offset: 0x000BDBD8
	public bool SetDestination(Vector3 pos, float speedFraction = 1f, float updateInterval = 0f, float navmeshSampleDistance = 0f)
	{
		if (!AI.move)
		{
			return false;
		}
		if (!AI.navthink)
		{
			return false;
		}
		if (updateInterval > 0f && !this.UpdateIntervalElapsed(updateInterval))
		{
			return true;
		}
		this.lastSetDestinationTime = UnityEngine.Time.time;
		this.paused = false;
		this.currentSpeedFraction = speedFraction;
		if (this.ReachedPosition(pos))
		{
			return true;
		}
		BaseNavigator.NavigationType navigationType = BaseNavigator.NavigationType.NavMesh;
		if (this.CanUseBaseNav && this.CanUseNavMesh)
		{
			Vector3 position;
			BaseNavigator.NavigationType navigationType2 = this.DetermineNavigationType(base.transform.position, out position);
			Vector3 vector;
			BaseNavigator.NavigationType navigationType3 = this.DetermineNavigationType(pos, out vector);
			if (navigationType3 == BaseNavigator.NavigationType.NavMesh && navigationType2 == BaseNavigator.NavigationType.NavMesh && (this.CurrentNavigationType == BaseNavigator.NavigationType.None || this.CurrentNavigationType == BaseNavigator.NavigationType.Base))
			{
				this.Warp(position);
			}
			if (navigationType3 == BaseNavigator.NavigationType.Base && navigationType2 != BaseNavigator.NavigationType.Base)
			{
				BasePet basePet = this.BaseEntity as BasePet;
				if (basePet != null)
				{
					BasePlayer basePlayer = basePet.Brain.Events.Memory.Entity.Get(5) as BasePlayer;
					if (basePlayer != null)
					{
						BuildingPrivlidge buildingPrivilege = basePlayer.GetBuildingPrivilege(new OBB(pos, base.transform.rotation, this.BaseEntity.bounds));
						if (buildingPrivilege != null && !buildingPrivilege.IsAuthed(basePlayer) && buildingPrivilege.AnyAuthed())
						{
							return false;
						}
					}
				}
			}
			if (navigationType3 == BaseNavigator.NavigationType.Base)
			{
				if (navigationType2 != BaseNavigator.NavigationType.Base)
				{
					if (Vector3.Distance(this.BaseEntity.ServerPosition, pos) <= 10f && Mathf.Abs(this.BaseEntity.ServerPosition.y - pos.y) <= 3f)
					{
						navigationType = BaseNavigator.NavigationType.Base;
					}
					else
					{
						navigationType = BaseNavigator.NavigationType.NavMesh;
					}
				}
				else
				{
					navigationType = BaseNavigator.NavigationType.Base;
				}
			}
			else if (navigationType3 == BaseNavigator.NavigationType.NavMesh)
			{
				if (navigationType2 != BaseNavigator.NavigationType.NavMesh)
				{
					navigationType = BaseNavigator.NavigationType.Base;
				}
				else
				{
					navigationType = BaseNavigator.NavigationType.NavMesh;
				}
			}
		}
		else
		{
			navigationType = (this.CanUseNavMesh ? BaseNavigator.NavigationType.NavMesh : BaseNavigator.NavigationType.AStar);
		}
		if (navigationType == BaseNavigator.NavigationType.Base)
		{
			return this.SetBaseDestination(pos, speedFraction);
		}
		if (navigationType == BaseNavigator.NavigationType.AStar)
		{
			if (this.AStarGraph != null)
			{
				return this.SetDestination(this.AStarGraph, this.AStarGraph.GetClosestToPoint(pos), speedFraction);
			}
			return this.CanUseCustomNav && this.SetCustomDestination(pos, speedFraction, updateInterval);
		}
		else
		{
			if (AiManager.nav_disable)
			{
				return false;
			}
			if (navmeshSampleDistance > 0f && AI.setdestinationsamplenavmesh)
			{
				NavMeshHit navMeshHit;
				if (!NavMesh.SamplePosition(pos, out navMeshHit, navmeshSampleDistance, this.defaultAreaMask))
				{
					return false;
				}
				pos = navMeshHit.position;
			}
			this.SetCurrentNavigationType(BaseNavigator.NavigationType.NavMesh);
			if (!this.Agent.isOnNavMesh)
			{
				return false;
			}
			if (!this.Agent.isActiveAndEnabled)
			{
				return false;
			}
			this.Destination = pos;
			bool flag;
			if (AI.usecalculatepath)
			{
				flag = NavMesh.CalculatePath(base.transform.position, this.Destination, this.navMeshQueryFilter, this.path);
				if (flag)
				{
					this.Agent.SetPath(this.path);
				}
				else if (AI.usesetdestinationfallback)
				{
					flag = this.Agent.SetDestination(this.Destination);
				}
			}
			else
			{
				flag = this.Agent.SetDestination(this.Destination);
			}
			if (flag && this.SpeedBasedAvoidancePriority)
			{
				this.Agent.avoidancePriority = UnityEngine.Random.Range(0, 21) + Mathf.FloorToInt(speedFraction * 80f);
			}
			return flag;
		}
	}

	// Token: 0x06001AF0 RID: 6896 RVA: 0x000BFCD4 File Offset: 0x000BDED4
	private BaseNavigator.NavigationType DetermineNavigationType(Vector3 location, out Vector3 navMeshPos)
	{
		navMeshPos = location;
		int layerMask = 2097152;
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(location + Vector3.up * BaseNavigator.navTypeHeightOffset, Vector3.down, out raycastHit, BaseNavigator.navTypeDistance, layerMask))
		{
			return BaseNavigator.NavigationType.Base;
		}
		Vector3 vector;
		BaseNavigator.NavigationType result = this.GetNearestNavmeshPosition(location + Vector3.up * BaseNavigator.navTypeHeightOffset, out vector, BaseNavigator.navTypeDistance) ? BaseNavigator.NavigationType.NavMesh : BaseNavigator.NavigationType.Base;
		navMeshPos = vector;
		return result;
	}

	// Token: 0x06001AF1 RID: 6897 RVA: 0x000BFD48 File Offset: 0x000BDF48
	public void SetCurrentSpeed(BaseNavigator.NavigationSpeed speed)
	{
		this.currentSpeedFraction = this.GetSpeedFraction(speed);
	}

	// Token: 0x06001AF2 RID: 6898 RVA: 0x000BFD57 File Offset: 0x000BDF57
	public bool UpdateIntervalElapsed(float updateInterval)
	{
		return updateInterval <= 0f || UnityEngine.Time.time - this.lastSetDestinationTime >= updateInterval;
	}

	// Token: 0x06001AF3 RID: 6899 RVA: 0x000BFD75 File Offset: 0x000BDF75
	public float GetSpeedFraction(BaseNavigator.NavigationSpeed speed)
	{
		switch (speed)
		{
		case BaseNavigator.NavigationSpeed.Slowest:
			return this.SlowestSpeedFraction;
		case BaseNavigator.NavigationSpeed.Slow:
			return this.SlowSpeedFraction;
		case BaseNavigator.NavigationSpeed.Normal:
			return this.NormalSpeedFraction;
		case BaseNavigator.NavigationSpeed.Fast:
			return this.FastSpeedFraction;
		default:
			return 1f;
		}
	}

	// Token: 0x06001AF4 RID: 6900 RVA: 0x000BFDB0 File Offset: 0x000BDFB0
	protected void SetCurrentNavigationType(BaseNavigator.NavigationType navType)
	{
		if (this.CurrentNavigationType == BaseNavigator.NavigationType.None)
		{
			this.stuckCheckPosition = base.transform.position;
			this.stuckTimer = 0f;
		}
		this.CurrentNavigationType = navType;
		if (this.CurrentNavigationType != BaseNavigator.NavigationType.None)
		{
			this.LastUsedNavigationType = this.CurrentNavigationType;
		}
		if (navType == BaseNavigator.NavigationType.None)
		{
			this.stuckTimer = 0f;
			return;
		}
		if (navType != BaseNavigator.NavigationType.NavMesh)
		{
			return;
		}
		this.SetNavMeshEnabled(true);
	}

	// Token: 0x06001AF5 RID: 6901 RVA: 0x000BFE17 File Offset: 0x000BE017
	public void Pause()
	{
		if (this.CurrentNavigationType == BaseNavigator.NavigationType.None)
		{
			return;
		}
		this.Stop();
		this.paused = true;
	}

	// Token: 0x06001AF6 RID: 6902 RVA: 0x000BFE2F File Offset: 0x000BE02F
	public void Resume()
	{
		if (!this.paused)
		{
			return;
		}
		this.SetDestination(this.Destination, this.currentSpeedFraction, 0f, 0f);
		this.paused = false;
	}

	// Token: 0x06001AF7 RID: 6903 RVA: 0x000BFE60 File Offset: 0x000BE060
	public void Stop()
	{
		switch (this.CurrentNavigationType)
		{
		case BaseNavigator.NavigationType.NavMesh:
			this.StopNavMesh();
			break;
		case BaseNavigator.NavigationType.AStar:
			this.StopAStar();
			break;
		case BaseNavigator.NavigationType.Custom:
			this.StopCustom();
			break;
		}
		this.SetCurrentNavigationType(BaseNavigator.NavigationType.None);
		this.paused = false;
	}

	// Token: 0x06001AF8 RID: 6904 RVA: 0x000BFEAE File Offset: 0x000BE0AE
	private void StopNavMesh()
	{
		this.SetNavMeshEnabled(false);
	}

	// Token: 0x06001AF9 RID: 6905 RVA: 0x000BFEB7 File Offset: 0x000BE0B7
	private void StopAStar()
	{
		this.currentAStarPath = null;
		this.targetNode = null;
	}

	// Token: 0x06001AFA RID: 6906 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void StopCustom()
	{
	}

	// Token: 0x06001AFB RID: 6907 RVA: 0x000BFEC7 File Offset: 0x000BE0C7
	public void Think(float delta)
	{
		if (!AI.move)
		{
			return;
		}
		if (!AI.navthink)
		{
			return;
		}
		if (this.BaseEntity == null)
		{
			return;
		}
		this.UpdateNavigation(delta);
	}

	// Token: 0x06001AFC RID: 6908 RVA: 0x000BFEEF File Offset: 0x000BE0EF
	public void UpdateNavigation(float delta)
	{
		this.UpdateMovement(delta);
	}

	// Token: 0x06001AFD RID: 6909 RVA: 0x000BFEF8 File Offset: 0x000BE0F8
	private void UpdateMovement(float delta)
	{
		if (!AI.move)
		{
			return;
		}
		if (!this.CanUpdateMovement())
		{
			return;
		}
		Vector3 moveToPosition = base.transform.position;
		if (this.TriggerStuckEvent)
		{
			this.stuckTimer += delta;
			if (this.CurrentNavigationType != BaseNavigator.NavigationType.None && this.stuckTimer >= BaseNavigator.stuckTriggerDuration)
			{
				if (Vector3.Distance(base.transform.position, this.stuckCheckPosition) <= this.StuckDistance)
				{
					this.OnStuck();
				}
				this.stuckTimer = 0f;
				this.stuckCheckPosition = base.transform.position;
			}
		}
		if (this.CurrentNavigationType == BaseNavigator.NavigationType.Base)
		{
			moveToPosition = this.Destination;
		}
		else if (this.IsOnNavMeshLink)
		{
			this.HandleNavMeshLinkTraversal(delta, ref moveToPosition);
		}
		else if (this.HasPath)
		{
			moveToPosition = this.GetNextPathPosition();
		}
		else if (this.CurrentNavigationType == BaseNavigator.NavigationType.Custom)
		{
			moveToPosition = this.Destination;
		}
		if (!this.ValidateNextPosition(ref moveToPosition))
		{
			return;
		}
		bool swimming = this.IsSwimming();
		this.UpdateSpeed(delta, swimming);
		this.UpdatePositionAndRotation(moveToPosition, delta);
	}

	// Token: 0x06001AFE RID: 6910 RVA: 0x000BFFF8 File Offset: 0x000BE1F8
	public virtual void OnStuck()
	{
		BasePet basePet = this.BaseEntity as BasePet;
		if (basePet != null && basePet.Brain != null)
		{
			basePet.Brain.LoadDefaultAIDesign();
		}
	}

	// Token: 0x06001AFF RID: 6911 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool IsSwimming()
	{
		return false;
	}

	// Token: 0x06001B00 RID: 6912 RVA: 0x000C0034 File Offset: 0x000BE234
	private Vector3 GetNextPathPosition()
	{
		if (this.currentAStarPath != null && this.currentAStarPath.Count > 0)
		{
			return this.currentAStarPath.Peek().Position;
		}
		return this.Agent.nextPosition;
	}

	// Token: 0x06001B01 RID: 6913 RVA: 0x000C0068 File Offset: 0x000BE268
	private bool ValidateNextPosition(ref Vector3 moveToPosition)
	{
		bool flag = ValidBounds.Test(moveToPosition);
		if (this.BaseEntity != null && !flag && base.transform != null && !this.BaseEntity.IsDestroyed)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Invalid NavAgent Position: ",
				this,
				" ",
				moveToPosition.ToString(),
				" (destroying)"
			}));
			this.BaseEntity.Kill(BaseNetworkable.DestroyMode.None);
			return false;
		}
		return true;
	}

	// Token: 0x06001B02 RID: 6914 RVA: 0x000C00F8 File Offset: 0x000BE2F8
	private void UpdateSpeed(float delta, bool swimming)
	{
		float num = this.GetTargetSpeed();
		if (this.LowHealthSpeedReductionTriggerFraction > 0f && this.BaseEntity.healthFraction <= this.LowHealthSpeedReductionTriggerFraction)
		{
			num = Mathf.Min(num, this.Speed * this.LowHealthMaxSpeedFraction);
		}
		this.Agent.speed = num * (swimming ? this.SwimmingSpeedMultiplier : 1f);
	}

	// Token: 0x06001B03 RID: 6915 RVA: 0x000C015D File Offset: 0x000BE35D
	protected virtual float GetTargetSpeed()
	{
		return this.Speed * this.currentSpeedFraction;
	}

	// Token: 0x06001B04 RID: 6916 RVA: 0x000C016C File Offset: 0x000BE36C
	protected virtual void UpdatePositionAndRotation(Vector3 moveToPosition, float delta)
	{
		if (this.CurrentNavigationType == BaseNavigator.NavigationType.AStar && this.currentAStarPath != null && this.currentAStarPath.Count > 0)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, moveToPosition, this.Agent.speed * delta);
			this.BaseEntity.ServerPosition = base.transform.localPosition;
			if (this.ReachedPosition(moveToPosition))
			{
				this.currentAStarPath.Pop();
				if (this.currentAStarPath.Count == 0)
				{
					this.Stop();
					return;
				}
				moveToPosition = this.currentAStarPath.Peek().Position;
			}
		}
		if (this.CurrentNavigationType == BaseNavigator.NavigationType.NavMesh)
		{
			if (this.ReachedPosition(this.Agent.destination))
			{
				this.Stop();
			}
			if (this.BaseEntity != null)
			{
				this.BaseEntity.ServerPosition = moveToPosition;
			}
		}
		if (this.CurrentNavigationType == BaseNavigator.NavigationType.Base)
		{
			this.frameCount++;
			this.accumDelta += delta;
			if (this.frameCount < BaseNavigator.baseNavMovementFrameInterval)
			{
				return;
			}
			this.frameCount = 0;
			delta = this.accumDelta;
			this.accumDelta = 0f;
			int layerMask = 10551552;
			Vector3 a = Vector3Ex.Direction2D(this.Destination, this.BaseEntity.ServerPosition);
			Vector3 a2 = this.BaseEntity.ServerPosition + a * delta * this.Agent.speed;
			Vector3 vector = this.BaseEntity.ServerPosition + Vector3.up * BaseNavigator.maxStepUpDistance;
			Vector3 direction = Vector3Ex.Direction(a2 + Vector3.up * BaseNavigator.maxStepUpDistance, this.BaseEntity.ServerPosition + Vector3.up * BaseNavigator.maxStepUpDistance);
			float maxDistance = Vector3.Distance(vector, a2 + Vector3.up * BaseNavigator.maxStepUpDistance) + 0.25f;
			RaycastHit raycastHit;
			if (UnityEngine.Physics.Raycast(vector, direction, out raycastHit, maxDistance, layerMask))
			{
				return;
			}
			if (!UnityEngine.Physics.SphereCast(a2 + Vector3.up * (BaseNavigator.maxStepUpDistance + 0.3f), 0.25f, Vector3.down, out raycastHit, 10f, layerMask))
			{
				return;
			}
			Vector3 point = raycastHit.point;
			if (point.y - this.BaseEntity.ServerPosition.y > BaseNavigator.maxStepUpDistance)
			{
				return;
			}
			this.BaseEntity.ServerPosition = point;
			if (this.ReachedPosition(moveToPosition))
			{
				this.Stop();
			}
		}
		if (this.overrideFacingDirectionMode != BaseNavigator.OverrideFacingDirectionMode.None)
		{
			this.ApplyFacingDirectionOverride();
		}
	}

	// Token: 0x06001B05 RID: 6917 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ApplyFacingDirectionOverride()
	{
	}

	// Token: 0x06001B06 RID: 6918 RVA: 0x000C0400 File Offset: 0x000BE600
	public void SetFacingDirectionEntity(BaseEntity entity)
	{
		this.overrideFacingDirectionMode = BaseNavigator.OverrideFacingDirectionMode.Entity;
		this.facingDirectionEntity = entity;
	}

	// Token: 0x06001B07 RID: 6919 RVA: 0x000C0410 File Offset: 0x000BE610
	public void SetFacingDirectionOverride(Vector3 direction)
	{
		this.overrideFacingDirectionMode = BaseNavigator.OverrideFacingDirectionMode.Direction;
		this.overrideFacingDirection = true;
		this.facingDirectionOverride = direction;
	}

	// Token: 0x06001B08 RID: 6920 RVA: 0x000C0427 File Offset: 0x000BE627
	public void ClearFacingDirectionOverride()
	{
		this.overrideFacingDirectionMode = BaseNavigator.OverrideFacingDirectionMode.None;
		this.overrideFacingDirection = false;
		this.facingDirectionEntity = null;
	}

	// Token: 0x1700024A RID: 586
	// (get) Token: 0x06001B09 RID: 6921 RVA: 0x000C043E File Offset: 0x000BE63E
	public bool IsOverridingFacingDirection
	{
		get
		{
			return this.overrideFacingDirectionMode > BaseNavigator.OverrideFacingDirectionMode.None;
		}
	}

	// Token: 0x1700024B RID: 587
	// (get) Token: 0x06001B0A RID: 6922 RVA: 0x000C0449 File Offset: 0x000BE649
	public Vector3 FacingDirectionOverride
	{
		get
		{
			return this.facingDirectionOverride;
		}
	}

	// Token: 0x06001B0B RID: 6923 RVA: 0x000C0451 File Offset: 0x000BE651
	protected bool ReachedPosition(Vector3 position)
	{
		return Vector3.Distance(position, base.transform.position) <= this.StoppingDistance;
	}

	// Token: 0x06001B0C RID: 6924 RVA: 0x000C046F File Offset: 0x000BE66F
	private void HandleNavMeshLinkTraversal(float delta, ref Vector3 moveToPosition)
	{
		if (!this.traversingNavMeshLink)
		{
			this.HandleNavMeshLinkTraversalStart(delta);
		}
		this.HandleNavMeshLinkTraversalTick(delta, ref moveToPosition);
		if (this.IsNavMeshLinkTraversalComplete(delta, ref moveToPosition))
		{
			this.CompleteNavMeshLink();
		}
	}

	// Token: 0x06001B0D RID: 6925 RVA: 0x000C049C File Offset: 0x000BE69C
	private bool HandleNavMeshLinkTraversalStart(float delta)
	{
		OffMeshLinkData currentOffMeshLinkData = this.Agent.currentOffMeshLinkData;
		if (!currentOffMeshLinkData.valid || !currentOffMeshLinkData.activated)
		{
			return false;
		}
		Vector3 normalized = (currentOffMeshLinkData.endPos - currentOffMeshLinkData.startPos).normalized;
		normalized.y = 0f;
		Vector3 desiredVelocity = this.Agent.desiredVelocity;
		desiredVelocity.y = 0f;
		if (Vector3.Dot(desiredVelocity, normalized) < 0.1f)
		{
			this.CompleteNavMeshLink();
			return false;
		}
		this.currentNavMeshLinkName = currentOffMeshLinkData.linkType.ToString();
		Vector3 a = (this.BaseEntity != null) ? this.BaseEntity.ServerPosition : base.transform.position;
		if ((a - currentOffMeshLinkData.startPos).sqrMagnitude > (a - currentOffMeshLinkData.endPos).sqrMagnitude)
		{
			this.currentNavMeshLinkEndPos = currentOffMeshLinkData.startPos;
		}
		else
		{
			this.currentNavMeshLinkEndPos = currentOffMeshLinkData.endPos;
		}
		this.traversingNavMeshLink = true;
		this.Agent.ActivateCurrentOffMeshLink(false);
		this.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
		if (!(this.currentNavMeshLinkName == "OpenDoorLink") && !(this.currentNavMeshLinkName == "JumpRockLink"))
		{
			this.currentNavMeshLinkName == "JumpFoundationLink";
		}
		return true;
	}

	// Token: 0x06001B0E RID: 6926 RVA: 0x000C0604 File Offset: 0x000BE804
	private void HandleNavMeshLinkTraversalTick(float delta, ref Vector3 moveToPosition)
	{
		if (this.currentNavMeshLinkName == "OpenDoorLink")
		{
			moveToPosition = Vector3.MoveTowards(moveToPosition, this.currentNavMeshLinkEndPos, this.Agent.speed * delta);
			return;
		}
		if (this.currentNavMeshLinkName == "JumpRockLink")
		{
			moveToPosition = Vector3.MoveTowards(moveToPosition, this.currentNavMeshLinkEndPos, this.Agent.speed * delta);
			return;
		}
		if (this.currentNavMeshLinkName == "JumpFoundationLink")
		{
			moveToPosition = Vector3.MoveTowards(moveToPosition, this.currentNavMeshLinkEndPos, this.Agent.speed * delta);
			return;
		}
		moveToPosition = Vector3.MoveTowards(moveToPosition, this.currentNavMeshLinkEndPos, this.Agent.speed * delta);
	}

	// Token: 0x06001B0F RID: 6927 RVA: 0x000C06DC File Offset: 0x000BE8DC
	private bool IsNavMeshLinkTraversalComplete(float delta, ref Vector3 moveToPosition)
	{
		if ((moveToPosition - this.currentNavMeshLinkEndPos).sqrMagnitude < 0.01f)
		{
			moveToPosition = this.currentNavMeshLinkEndPos;
			this.traversingNavMeshLink = false;
			this.currentNavMeshLinkName = string.Empty;
			this.CompleteNavMeshLink();
			return true;
		}
		return false;
	}

	// Token: 0x06001B10 RID: 6928 RVA: 0x000C0730 File Offset: 0x000BE930
	private void CompleteNavMeshLink()
	{
		this.Agent.ActivateCurrentOffMeshLink(true);
		this.Agent.CompleteOffMeshLink();
		this.Agent.isStopped = false;
		this.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
	}

	// Token: 0x06001B11 RID: 6929 RVA: 0x000C0764 File Offset: 0x000BE964
	public bool IsPositionATopologyPreference(Vector3 position)
	{
		if (TerrainMeta.TopologyMap != null)
		{
			int topology = TerrainMeta.TopologyMap.GetTopology(position);
			if ((this.TopologyPreference() & topology) != 0)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001B12 RID: 6930 RVA: 0x000C0798 File Offset: 0x000BE998
	public bool IsPositionPreventTopology(Vector3 position)
	{
		if (TerrainMeta.TopologyMap != null)
		{
			int topology = TerrainMeta.TopologyMap.GetTopology(position);
			if ((this.TopologyPrevent() & topology) != 0)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001B13 RID: 6931 RVA: 0x000C07CC File Offset: 0x000BE9CC
	public bool IsPositionABiomePreference(Vector3 position)
	{
		if (!this.UseBiomePreference)
		{
			return true;
		}
		if (TerrainMeta.BiomeMap != null)
		{
			int num = (int)this.biomePreference;
			if ((TerrainMeta.BiomeMap.GetBiomeMaxType(position, -1) & num) != 0)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001B14 RID: 6932 RVA: 0x000C080C File Offset: 0x000BEA0C
	public bool IsPositionABiomeRequirement(Vector3 position)
	{
		if (this.biomeRequirement == (TerrainBiome.Enum)0)
		{
			return true;
		}
		if (TerrainMeta.BiomeMap != null)
		{
			int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(position, -1);
			if ((this.BiomeRequirement() & biomeMaxType) != 0)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001B15 RID: 6933 RVA: 0x000C084A File Offset: 0x000BEA4A
	public bool IsAcceptableWaterDepth(Vector3 pos)
	{
		return WaterLevel.GetOverallWaterDepth(pos, true, null, false) <= this.MaxWaterDepth;
	}

	// Token: 0x06001B16 RID: 6934 RVA: 0x000C0860 File Offset: 0x000BEA60
	public void SetBrakingEnabled(bool flag)
	{
		this.Agent.autoBraking = flag;
	}

	// Token: 0x02000C5F RID: 3167
	public enum NavigationType
	{
		// Token: 0x040042D5 RID: 17109
		None,
		// Token: 0x040042D6 RID: 17110
		NavMesh,
		// Token: 0x040042D7 RID: 17111
		AStar,
		// Token: 0x040042D8 RID: 17112
		Custom,
		// Token: 0x040042D9 RID: 17113
		Base
	}

	// Token: 0x02000C60 RID: 3168
	public enum NavigationSpeed
	{
		// Token: 0x040042DB RID: 17115
		Slowest,
		// Token: 0x040042DC RID: 17116
		Slow,
		// Token: 0x040042DD RID: 17117
		Normal,
		// Token: 0x040042DE RID: 17118
		Fast
	}

	// Token: 0x02000C61 RID: 3169
	protected enum OverrideFacingDirectionMode
	{
		// Token: 0x040042E0 RID: 17120
		None,
		// Token: 0x040042E1 RID: 17121
		Direction,
		// Token: 0x040042E2 RID: 17122
		Entity
	}
}
