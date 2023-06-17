using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020001DD RID: 477
public class AIInformationZone : BaseMonoBehaviour, IServerComponent
{
	// Token: 0x0400122D RID: 4653
	public bool RenderBounds;

	// Token: 0x0400122E RID: 4654
	public bool ShouldSleepAI;

	// Token: 0x0400122F RID: 4655
	public bool Virtual;

	// Token: 0x04001230 RID: 4656
	public bool UseCalculatedCoverDistances = true;

	// Token: 0x04001231 RID: 4657
	public static List<AIInformationZone> zones = new List<AIInformationZone>();

	// Token: 0x04001232 RID: 4658
	public List<AICoverPoint> coverPoints = new List<AICoverPoint>();

	// Token: 0x04001233 RID: 4659
	public List<AIMovePoint> movePoints = new List<AIMovePoint>();

	// Token: 0x04001234 RID: 4660
	private AICoverPoint[] coverPointArray;

	// Token: 0x04001235 RID: 4661
	private AIMovePoint[] movePointArray;

	// Token: 0x04001236 RID: 4662
	public List<NavMeshLink> navMeshLinks = new List<NavMeshLink>();

	// Token: 0x04001237 RID: 4663
	public List<AIMovePointPath> paths = new List<AIMovePointPath>();

	// Token: 0x04001238 RID: 4664
	public Bounds bounds;

	// Token: 0x04001239 RID: 4665
	private AIInformationGrid grid;

	// Token: 0x0400123B RID: 4667
	private List<IAISleepable> sleepables = new List<IAISleepable>();

	// Token: 0x0400123C RID: 4668
	private OBB areaBox;

	// Token: 0x0400123D RID: 4669
	private bool isDirty = true;

	// Token: 0x0400123E RID: 4670
	private int processIndex;

	// Token: 0x0400123F RID: 4671
	private int halfPaths;

	// Token: 0x04001240 RID: 4672
	private int pathSuccesses;

	// Token: 0x04001241 RID: 4673
	private int pathFails;

	// Token: 0x04001242 RID: 4674
	private bool initd;

	// Token: 0x04001243 RID: 4675
	private static bool lastFrameAnyDirty = false;

	// Token: 0x04001244 RID: 4676
	private static float rebuildStartTime = 0f;

	// Token: 0x04001245 RID: 4677
	public static float buildTimeTest = 0f;

	// Token: 0x04001246 RID: 4678
	private static float lastNavmeshBuildTime = 0f;

	// Token: 0x06001949 RID: 6473 RVA: 0x000B9D38 File Offset: 0x000B7F38
	public static AIInformationZone Merge(List<AIInformationZone> zones, GameObject newRoot)
	{
		if (zones == null)
		{
			return null;
		}
		AIInformationZone aiinformationZone = newRoot.AddComponent<AIInformationZone>();
		aiinformationZone.UseCalculatedCoverDistances = false;
		foreach (AIInformationZone aiinformationZone2 in zones)
		{
			if (!(aiinformationZone2 == null))
			{
				foreach (AIMovePoint aimovePoint in aiinformationZone2.movePoints)
				{
					aiinformationZone.AddMovePoint(aimovePoint);
					aimovePoint.transform.SetParent(newRoot.transform);
				}
				foreach (AICoverPoint aicoverPoint in aiinformationZone2.coverPoints)
				{
					aiinformationZone.AddCoverPoint(aicoverPoint);
					aicoverPoint.transform.SetParent(newRoot.transform);
				}
			}
		}
		aiinformationZone.bounds = AIInformationZone.EncapsulateBounds(zones);
		AIInformationZone aiinformationZone3 = aiinformationZone;
		aiinformationZone3.bounds.extents = aiinformationZone3.bounds.extents + new Vector3(5f, 0f, 5f);
		AIInformationZone aiinformationZone4 = aiinformationZone;
		aiinformationZone4.bounds.center = aiinformationZone4.bounds.center - aiinformationZone.transform.position;
		for (int i = zones.Count - 1; i >= 0; i--)
		{
			AIInformationZone aiinformationZone5 = zones[i];
			if (!(aiinformationZone5 == null))
			{
				UnityEngine.Object.Destroy(aiinformationZone5);
			}
		}
		return aiinformationZone;
	}

	// Token: 0x0600194A RID: 6474 RVA: 0x000B9EDC File Offset: 0x000B80DC
	public static Bounds EncapsulateBounds(List<AIInformationZone> zones)
	{
		Bounds result = default(Bounds);
		result.center = zones[0].transform.position;
		foreach (AIInformationZone aiinformationZone in zones)
		{
			if (!(aiinformationZone == null))
			{
				Vector3 center = aiinformationZone.bounds.center + aiinformationZone.transform.position;
				Bounds bounds = aiinformationZone.bounds;
				bounds.center = center;
				result.Encapsulate(bounds);
			}
		}
		return result;
	}

	// Token: 0x17000223 RID: 547
	// (get) Token: 0x0600194B RID: 6475 RVA: 0x000B9F84 File Offset: 0x000B8184
	// (set) Token: 0x0600194C RID: 6476 RVA: 0x000B9F8C File Offset: 0x000B818C
	public bool Sleeping { get; private set; }

	// Token: 0x17000224 RID: 548
	// (get) Token: 0x0600194D RID: 6477 RVA: 0x000B9F95 File Offset: 0x000B8195
	public int SleepingCount
	{
		get
		{
			if (!this.Sleeping)
			{
				return 0;
			}
			return this.sleepables.Count;
		}
	}

	// Token: 0x0600194E RID: 6478 RVA: 0x000B9FAC File Offset: 0x000B81AC
	public void Start()
	{
		this.Init();
	}

	// Token: 0x0600194F RID: 6479 RVA: 0x000B9FB4 File Offset: 0x000B81B4
	public void Init()
	{
		if (this.initd)
		{
			return;
		}
		this.initd = true;
		this.AddInitialPoints();
		this.areaBox = new OBB(base.transform.position, base.transform.lossyScale, base.transform.rotation, this.bounds);
		AIInformationZone.zones.Add(this);
		this.grid = base.GetComponent<AIInformationGrid>();
		if (this.grid != null)
		{
			this.grid.Init();
		}
	}

	// Token: 0x06001950 RID: 6480 RVA: 0x000BA039 File Offset: 0x000B8239
	public void RegisterSleepableEntity(IAISleepable sleepable)
	{
		if (sleepable == null)
		{
			return;
		}
		if (!sleepable.AllowedToSleep())
		{
			return;
		}
		if (this.sleepables.Contains(sleepable))
		{
			return;
		}
		this.sleepables.Add(sleepable);
		if (this.Sleeping && sleepable.AllowedToSleep())
		{
			sleepable.SleepAI();
		}
	}

	// Token: 0x06001951 RID: 6481 RVA: 0x000BA079 File Offset: 0x000B8279
	public void UnregisterSleepableEntity(IAISleepable sleepable)
	{
		if (sleepable == null)
		{
			return;
		}
		this.sleepables.Remove(sleepable);
	}

	// Token: 0x06001952 RID: 6482 RVA: 0x000BA08C File Offset: 0x000B828C
	public void SleepAI()
	{
		if (!AI.sleepwake)
		{
			return;
		}
		if (!this.ShouldSleepAI)
		{
			return;
		}
		foreach (IAISleepable iaisleepable in this.sleepables)
		{
			if (iaisleepable != null)
			{
				iaisleepable.SleepAI();
			}
		}
		this.Sleeping = true;
	}

	// Token: 0x06001953 RID: 6483 RVA: 0x000BA0FC File Offset: 0x000B82FC
	public void WakeAI()
	{
		foreach (IAISleepable iaisleepable in this.sleepables)
		{
			if (iaisleepable != null)
			{
				iaisleepable.WakeAI();
			}
		}
		this.Sleeping = false;
	}

	// Token: 0x06001954 RID: 6484 RVA: 0x000BA158 File Offset: 0x000B8358
	private void AddCoverPoint(AICoverPoint point)
	{
		if (this.coverPoints.Contains(point))
		{
			return;
		}
		this.coverPoints.Add(point);
		this.MarkDirty(false);
	}

	// Token: 0x06001955 RID: 6485 RVA: 0x000BA17C File Offset: 0x000B837C
	private void RemoveCoverPoint(AICoverPoint point, bool markDirty = true)
	{
		this.coverPoints.Remove(point);
		if (markDirty)
		{
			this.MarkDirty(false);
		}
	}

	// Token: 0x06001956 RID: 6486 RVA: 0x000BA195 File Offset: 0x000B8395
	private void AddMovePoint(AIMovePoint point)
	{
		if (this.movePoints.Contains(point))
		{
			return;
		}
		this.movePoints.Add(point);
		this.MarkDirty(false);
	}

	// Token: 0x06001957 RID: 6487 RVA: 0x000BA1B9 File Offset: 0x000B83B9
	private void RemoveMovePoint(AIMovePoint point, bool markDirty = true)
	{
		this.movePoints.Remove(point);
		if (markDirty)
		{
			this.MarkDirty(false);
		}
	}

	// Token: 0x06001958 RID: 6488 RVA: 0x000BA1D4 File Offset: 0x000B83D4
	public void MarkDirty(bool completeRefresh = false)
	{
		this.isDirty = true;
		this.processIndex = 0;
		this.halfPaths = 0;
		this.pathSuccesses = 0;
		this.pathFails = 0;
		if (completeRefresh)
		{
			Debug.Log("AIInformationZone performing complete refresh, please wait...");
			foreach (AIMovePoint aimovePoint in this.movePoints)
			{
				aimovePoint.distances.Clear();
				aimovePoint.distancesToCover.Clear();
			}
		}
	}

	// Token: 0x06001959 RID: 6489 RVA: 0x000BA264 File Offset: 0x000B8464
	private bool PassesBudget(float startTime, float budgetSeconds)
	{
		return UnityEngine.Time.realtimeSinceStartup - startTime <= budgetSeconds;
	}

	// Token: 0x0600195A RID: 6490 RVA: 0x0000441C File Offset: 0x0000261C
	public bool ProcessDistancesAttempt()
	{
		return true;
	}

	// Token: 0x0600195B RID: 6491 RVA: 0x000BA274 File Offset: 0x000B8474
	private bool ProcessDistances()
	{
		if (!this.UseCalculatedCoverDistances)
		{
			return true;
		}
		float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
		float budgetSeconds = AIThinkManager.framebudgetms / 1000f * 0.25f;
		if (realtimeSinceStartup < AIInformationZone.lastNavmeshBuildTime + 60f)
		{
			budgetSeconds = 0.1f;
		}
		int areaMask = 1 << NavMesh.GetAreaFromName("HumanNPC");
		NavMeshPath navMeshPath = new NavMeshPath();
		while (this.PassesBudget(realtimeSinceStartup, budgetSeconds))
		{
			AIMovePoint aimovePoint = this.movePoints[this.processIndex];
			bool flag = true;
			int num = 0;
			for (int i = aimovePoint.distances.Keys.Count - 1; i >= 0; i--)
			{
				AIMovePoint aimovePoint2 = aimovePoint.distances.Keys[i];
				if (!this.movePoints.Contains(aimovePoint2))
				{
					aimovePoint.distances.Remove(aimovePoint2);
				}
			}
			for (int j = aimovePoint.distancesToCover.Keys.Count - 1; j >= 0; j--)
			{
				AICoverPoint aicoverPoint = aimovePoint.distancesToCover.Keys[j];
				if (!this.coverPoints.Contains(aicoverPoint))
				{
					num++;
					aimovePoint.distancesToCover.Remove(aicoverPoint);
				}
			}
			foreach (AICoverPoint aicoverPoint2 in this.coverPoints)
			{
				if (!(aicoverPoint2 == null) && !aimovePoint.distancesToCover.Contains(aicoverPoint2))
				{
					float val;
					if (Vector3.Distance(aimovePoint.transform.position, aicoverPoint2.transform.position) > 40f)
					{
						val = -2f;
					}
					else if (NavMesh.CalculatePath(aimovePoint.transform.position, aicoverPoint2.transform.position, areaMask, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
					{
						int num2 = navMeshPath.corners.Length;
						if (num2 > 1)
						{
							Vector3 a = navMeshPath.corners[0];
							float num3 = 0f;
							for (int k = 0; k < num2; k++)
							{
								Vector3 vector = navMeshPath.corners[k];
								num3 += Vector3.Distance(a, vector);
								a = vector;
							}
							val = num3;
							this.pathSuccesses++;
						}
						else
						{
							val = Vector3.Distance(aimovePoint.transform.position, aicoverPoint2.transform.position);
							this.halfPaths++;
						}
					}
					else
					{
						this.pathFails++;
						val = -2f;
					}
					aimovePoint.distancesToCover.Add(aicoverPoint2, val);
					if (!this.PassesBudget(realtimeSinceStartup, budgetSeconds))
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				this.processIndex++;
			}
			if (this.processIndex >= this.movePoints.Count - 1)
			{
				break;
			}
		}
		return this.processIndex >= this.movePoints.Count - 1;
	}

	// Token: 0x0600195C RID: 6492 RVA: 0x000BA594 File Offset: 0x000B8794
	public static void BudgetedTick()
	{
		if (!AI.move)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup < AIInformationZone.buildTimeTest)
		{
			return;
		}
		bool flag = false;
		foreach (AIInformationZone aiinformationZone in AIInformationZone.zones)
		{
			if (aiinformationZone.isDirty)
			{
				flag = true;
				bool flag2 = aiinformationZone.isDirty;
				aiinformationZone.isDirty = !aiinformationZone.ProcessDistancesAttempt();
				break;
			}
		}
		if (Global.developer > 0)
		{
			if (flag && !AIInformationZone.lastFrameAnyDirty)
			{
				Debug.Log("AIInformationZones rebuilding...");
				AIInformationZone.rebuildStartTime = UnityEngine.Time.realtimeSinceStartup;
			}
			if (AIInformationZone.lastFrameAnyDirty && !flag)
			{
				Debug.Log("AIInformationZone rebuild complete! Duration : " + (UnityEngine.Time.realtimeSinceStartup - AIInformationZone.rebuildStartTime) + " seconds.");
			}
		}
		AIInformationZone.lastFrameAnyDirty = flag;
	}

	// Token: 0x0600195D RID: 6493 RVA: 0x000BA674 File Offset: 0x000B8874
	public void NavmeshBuildingComplete()
	{
		AIInformationZone.lastNavmeshBuildTime = UnityEngine.Time.realtimeSinceStartup;
		AIInformationZone.buildTimeTest = UnityEngine.Time.realtimeSinceStartup + 15f;
		this.MarkDirty(true);
	}

	// Token: 0x0600195E RID: 6494 RVA: 0x000BA697 File Offset: 0x000B8897
	public Vector3 ClosestPointTo(Vector3 target)
	{
		return this.areaBox.ClosestPoint(target);
	}

	// Token: 0x0600195F RID: 6495 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnDrawGizmos()
	{
	}

	// Token: 0x06001960 RID: 6496 RVA: 0x000BA6A5 File Offset: 0x000B88A5
	public void OnDrawGizmosSelected()
	{
		this.DrawBounds();
	}

	// Token: 0x06001961 RID: 6497 RVA: 0x000BA6B0 File Offset: 0x000B88B0
	private void DrawBounds()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		Gizmos.DrawCube(this.bounds.center, this.bounds.size);
	}

	// Token: 0x06001962 RID: 6498 RVA: 0x000BA708 File Offset: 0x000B8908
	public void AddInitialPoints()
	{
		foreach (AICoverPoint point in base.transform.GetComponentsInChildren<AICoverPoint>())
		{
			this.AddCoverPoint(point);
		}
		foreach (AIMovePoint point2 in base.transform.GetComponentsInChildren<AIMovePoint>(true))
		{
			this.AddMovePoint(point2);
		}
		this.RefreshPointArrays();
		NavMeshLink[] componentsInChildren3 = base.transform.GetComponentsInChildren<NavMeshLink>(true);
		this.navMeshLinks.AddRange(componentsInChildren3);
		AIMovePointPath[] componentsInChildren4 = base.transform.GetComponentsInChildren<AIMovePointPath>();
		this.paths.AddRange(componentsInChildren4);
	}

	// Token: 0x06001963 RID: 6499 RVA: 0x000BA79E File Offset: 0x000B899E
	private void RefreshPointArrays()
	{
		List<AIMovePoint> list = this.movePoints;
		this.movePointArray = ((list != null) ? list.ToArray() : null);
		List<AICoverPoint> list2 = this.coverPoints;
		this.coverPointArray = ((list2 != null) ? list2.ToArray() : null);
	}

	// Token: 0x06001964 RID: 6500 RVA: 0x000BA7D0 File Offset: 0x000B89D0
	public void AddDynamicAIPoints(AIMovePoint[] movePoints, AICoverPoint[] coverPoints, Func<Vector3, bool> validatePoint = null)
	{
		if (movePoints != null)
		{
			foreach (AIMovePoint aimovePoint in movePoints)
			{
				if (!(aimovePoint == null) && (validatePoint == null || (validatePoint != null && validatePoint(aimovePoint.transform.position))))
				{
					this.AddMovePoint(aimovePoint);
				}
			}
		}
		if (coverPoints != null)
		{
			foreach (AICoverPoint aicoverPoint in coverPoints)
			{
				if (!(aicoverPoint == null) && (validatePoint == null || (validatePoint != null && validatePoint(aicoverPoint.transform.position))))
				{
					this.AddCoverPoint(aicoverPoint);
				}
			}
		}
		this.RefreshPointArrays();
	}

	// Token: 0x06001965 RID: 6501 RVA: 0x000BA868 File Offset: 0x000B8A68
	public void RemoveDynamicAIPoints(AIMovePoint[] movePoints, AICoverPoint[] coverPoints)
	{
		if (movePoints != null)
		{
			foreach (AIMovePoint aimovePoint in movePoints)
			{
				if (!(aimovePoint == null))
				{
					this.RemoveMovePoint(aimovePoint, false);
				}
			}
		}
		if (coverPoints != null)
		{
			foreach (AICoverPoint aicoverPoint in coverPoints)
			{
				if (!(aicoverPoint == null))
				{
					this.RemoveCoverPoint(aicoverPoint, false);
				}
			}
		}
		this.MarkDirty(false);
		this.RefreshPointArrays();
	}

	// Token: 0x06001966 RID: 6502 RVA: 0x000BA8D8 File Offset: 0x000B8AD8
	public AIMovePointPath GetNearestPath(Vector3 position)
	{
		if (this.paths == null || this.paths.Count == 0)
		{
			return null;
		}
		float num = float.MaxValue;
		AIMovePointPath result = null;
		foreach (AIMovePointPath aimovePointPath in this.paths)
		{
			foreach (AIMovePoint aimovePoint in aimovePointPath.Points)
			{
				float num2 = Vector3.SqrMagnitude(aimovePoint.transform.position - position);
				if (num2 < num)
				{
					num = num2;
					result = aimovePointPath;
				}
			}
		}
		return result;
	}

	// Token: 0x06001967 RID: 6503 RVA: 0x000BA9A0 File Offset: 0x000B8BA0
	public static AIInformationZone GetForPoint(Vector3 point, bool fallBackToNearest = true)
	{
		if (AIInformationZone.zones == null || AIInformationZone.zones.Count == 0)
		{
			return null;
		}
		foreach (AIInformationZone aiinformationZone in AIInformationZone.zones)
		{
			if (!(aiinformationZone == null) && !aiinformationZone.Virtual && aiinformationZone.areaBox.Contains(point))
			{
				return aiinformationZone;
			}
		}
		if (!fallBackToNearest)
		{
			return null;
		}
		float num = float.PositiveInfinity;
		AIInformationZone aiinformationZone2 = AIInformationZone.zones[0];
		foreach (AIInformationZone aiinformationZone3 in AIInformationZone.zones)
		{
			if (!(aiinformationZone3 == null) && !(aiinformationZone3.transform == null) && !aiinformationZone3.Virtual)
			{
				float num2 = Vector3.Distance(aiinformationZone3.transform.position, point);
				if (num2 < num)
				{
					num = num2;
					aiinformationZone2 = aiinformationZone3;
				}
			}
		}
		if (aiinformationZone2.Virtual)
		{
			aiinformationZone2 = null;
		}
		return aiinformationZone2;
	}

	// Token: 0x06001968 RID: 6504 RVA: 0x000BAAC8 File Offset: 0x000B8CC8
	public bool PointInside(Vector3 point)
	{
		return this.areaBox.Contains(point);
	}

	// Token: 0x06001969 RID: 6505 RVA: 0x000BAAD8 File Offset: 0x000B8CD8
	public AIMovePoint GetBestMovePointNear(Vector3 targetPosition, Vector3 fromPosition, float minRange, float maxRange, bool checkLOS = false, BaseEntity forObject = null, bool returnClosest = false)
	{
		AIPoint aipoint = null;
		AIPoint aipoint2 = null;
		float num = -1f;
		float num2 = float.PositiveInfinity;
		int num3;
		AIPoint[] movePointsInRange = this.GetMovePointsInRange(targetPosition, maxRange, out num3);
		if (movePointsInRange == null || num3 <= 0)
		{
			return null;
		}
		for (int i = 0; i < num3; i++)
		{
			AIPoint aipoint3 = movePointsInRange[i];
			if (aipoint3.transform.parent.gameObject.activeSelf && (fromPosition.y < WaterSystem.OceanLevel || aipoint3.transform.position.y >= WaterSystem.OceanLevel))
			{
				float num4 = 0f;
				Vector3 position = aipoint3.transform.position;
				float num5 = Vector3.Distance(targetPosition, position);
				if (num5 < num2)
				{
					aipoint2 = aipoint3;
					num2 = num5;
				}
				if (num5 <= maxRange)
				{
					num4 += (aipoint3.CanBeUsedBy(forObject) ? 100f : 0f);
					num4 += (1f - Mathf.InverseLerp(minRange, maxRange, num5)) * 100f;
					if (num4 >= num && (!checkLOS || !UnityEngine.Physics.Linecast(targetPosition + Vector3.up * 1f, position + Vector3.up * 1f, 1218519297, QueryTriggerInteraction.Ignore)) && num4 > num)
					{
						aipoint = aipoint3;
						num = num4;
					}
				}
			}
		}
		if (aipoint == null && returnClosest)
		{
			return aipoint2 as AIMovePoint;
		}
		return aipoint as AIMovePoint;
	}

	// Token: 0x0600196A RID: 6506 RVA: 0x000BAC44 File Offset: 0x000B8E44
	public AIPoint[] GetMovePointsInRange(Vector3 currentPos, float maxRange, out int pointCount)
	{
		pointCount = 0;
		AIMovePoint[] movePointsInRange;
		if (this.grid != null && AI.usegrid)
		{
			movePointsInRange = this.grid.GetMovePointsInRange(currentPos, maxRange, out pointCount);
		}
		else
		{
			movePointsInRange = this.movePointArray;
			if (movePointsInRange != null)
			{
				pointCount = movePointsInRange.Length;
			}
		}
		return movePointsInRange;
	}

	// Token: 0x0600196B RID: 6507 RVA: 0x000BAC8C File Offset: 0x000B8E8C
	private AIMovePoint GetClosestRaw(Vector3 pos, bool onlyIncludeWithCover = false)
	{
		AIMovePoint result = null;
		float num = float.PositiveInfinity;
		foreach (AIMovePoint aimovePoint in this.movePoints)
		{
			if (!onlyIncludeWithCover || aimovePoint.distancesToCover.Count != 0)
			{
				float num2 = Vector3.Distance(aimovePoint.transform.position, pos);
				if (num2 < num)
				{
					num = num2;
					result = aimovePoint;
				}
			}
		}
		return result;
	}

	// Token: 0x0600196C RID: 6508 RVA: 0x000BAD10 File Offset: 0x000B8F10
	public AICoverPoint GetBestCoverPoint(Vector3 currentPosition, Vector3 hideFromPosition, float minRange = 0f, float maxRange = 20f, BaseEntity forObject = null, bool allowObjectToReuse = true)
	{
		AICoverPoint aicoverPoint = null;
		float num = 0f;
		AIMovePoint closestRaw = this.GetClosestRaw(currentPosition, true);
		int num2;
		AICoverPoint[] coverPointsInRange = this.GetCoverPointsInRange(currentPosition, maxRange, out num2);
		if (coverPointsInRange == null || num2 <= 0)
		{
			return null;
		}
		for (int i = 0; i < num2; i++)
		{
			AICoverPoint aicoverPoint2 = coverPointsInRange[i];
			Vector3 position = aicoverPoint2.transform.position;
			Vector3 normalized = (hideFromPosition - position).normalized;
			float num3 = Vector3.Dot(aicoverPoint2.transform.forward, normalized);
			if (num3 >= 1f - aicoverPoint2.coverDot)
			{
				float num4;
				if (this.UseCalculatedCoverDistances && closestRaw != null && closestRaw.distancesToCover.Contains(aicoverPoint2) && !this.isDirty)
				{
					num4 = closestRaw.distancesToCover[aicoverPoint2];
					if (num4 == -2f)
					{
						goto IL_20D;
					}
				}
				else
				{
					num4 = Vector3.Distance(currentPosition, position);
				}
				float num5 = 0f;
				if (aicoverPoint2.InUse())
				{
					bool flag = aicoverPoint2.IsUsedBy(forObject);
					if (!allowObjectToReuse || !flag)
					{
						num5 -= 1000f;
					}
				}
				if (minRange > 0f)
				{
					num5 -= (1f - Mathf.InverseLerp(0f, minRange, num4)) * 100f;
				}
				float value = Mathf.Abs(position.y - currentPosition.y);
				num5 += (1f - Mathf.InverseLerp(1f, 5f, value)) * 500f;
				num5 += Mathf.InverseLerp(1f - aicoverPoint2.coverDot, 1f, num3) * 50f;
				num5 += (1f - Mathf.InverseLerp(2f, maxRange, num4)) * 100f;
				float num6 = 1f - Mathf.InverseLerp(4f, 10f, Vector3.Distance(currentPosition, hideFromPosition));
				float value2 = Vector3.Dot((aicoverPoint2.transform.position - currentPosition).normalized, normalized);
				num5 -= Mathf.InverseLerp(-1f, 0.25f, value2) * 50f * num6;
				if (num5 > num)
				{
					aicoverPoint = aicoverPoint2;
					num = num5;
				}
			}
			IL_20D:;
		}
		if (aicoverPoint)
		{
			return aicoverPoint;
		}
		return null;
	}

	// Token: 0x0600196D RID: 6509 RVA: 0x000BAF44 File Offset: 0x000B9144
	private AICoverPoint[] GetCoverPointsInRange(Vector3 position, float maxRange, out int pointCount)
	{
		pointCount = 0;
		AICoverPoint[] coverPointsInRange;
		if (this.grid != null && AI.usegrid)
		{
			coverPointsInRange = this.grid.GetCoverPointsInRange(position, maxRange, out pointCount);
		}
		else
		{
			coverPointsInRange = this.coverPointArray;
			if (coverPointsInRange != null)
			{
				pointCount = coverPointsInRange.Length;
			}
		}
		return coverPointsInRange;
	}

	// Token: 0x0600196E RID: 6510 RVA: 0x000BAF8C File Offset: 0x000B918C
	public NavMeshLink GetClosestNavMeshLink(Vector3 pos)
	{
		NavMeshLink result = null;
		float num = float.PositiveInfinity;
		foreach (NavMeshLink navMeshLink in this.navMeshLinks)
		{
			float num2 = Vector3.Distance(navMeshLink.gameObject.transform.position, pos);
			if (num2 < num)
			{
				result = navMeshLink;
				num = num2;
				if (num2 < 0.25f)
				{
					break;
				}
			}
		}
		return result;
	}
}
