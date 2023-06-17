using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A2 RID: 418
public class BasePath : MonoBehaviour, IAIPath
{
	// Token: 0x04001141 RID: 4417
	public List<BasePathNode> nodes;

	// Token: 0x04001142 RID: 4418
	public List<PathInterestNode> interestZones;

	// Token: 0x04001143 RID: 4419
	public List<PathSpeedZone> speedZones;

	// Token: 0x17000207 RID: 519
	// (get) Token: 0x0600187C RID: 6268 RVA: 0x000B7015 File Offset: 0x000B5215
	public IEnumerable<IAIPathInterestNode> InterestNodes
	{
		get
		{
			return this.interestZones;
		}
	}

	// Token: 0x17000208 RID: 520
	// (get) Token: 0x0600187D RID: 6269 RVA: 0x000B701D File Offset: 0x000B521D
	public IEnumerable<IAIPathSpeedZone> SpeedZones
	{
		get
		{
			return this.speedZones;
		}
	}

	// Token: 0x0600187E RID: 6270 RVA: 0x000B7028 File Offset: 0x000B5228
	private void AddChildren()
	{
		if (this.nodes != null)
		{
			this.nodes.Clear();
			this.nodes.AddRange(base.GetComponentsInChildren<BasePathNode>());
			foreach (BasePathNode basePathNode in this.nodes)
			{
				basePathNode.Path = this;
			}
		}
		if (this.interestZones != null)
		{
			this.interestZones.Clear();
			this.interestZones.AddRange(base.GetComponentsInChildren<PathInterestNode>());
		}
		if (this.speedZones != null)
		{
			this.speedZones.Clear();
			this.speedZones.AddRange(base.GetComponentsInChildren<PathSpeedZone>());
		}
	}

	// Token: 0x0600187F RID: 6271 RVA: 0x000B70E8 File Offset: 0x000B52E8
	private void ClearChildren()
	{
		if (this.nodes != null)
		{
			foreach (BasePathNode basePathNode in this.nodes)
			{
				basePathNode.linked.Clear();
			}
		}
		this.nodes.Clear();
	}

	// Token: 0x06001880 RID: 6272 RVA: 0x000B7150 File Offset: 0x000B5350
	public static void AutoGenerateLinks(BasePath path, float maxRange = -1f)
	{
		path.AddChildren();
		foreach (BasePathNode basePathNode in path.nodes)
		{
			if (basePathNode.linked == null)
			{
				basePathNode.linked = new List<BasePathNode>();
			}
			else
			{
				basePathNode.linked.Clear();
			}
			foreach (BasePathNode basePathNode2 in path.nodes)
			{
				if (!(basePathNode == basePathNode2) && (maxRange == -1f || Vector3.Distance(basePathNode.Position, basePathNode2.Position) <= maxRange) && GamePhysics.LineOfSight(basePathNode.Position, basePathNode2.Position, 429990145, null) && GamePhysics.LineOfSight(basePathNode2.Position, basePathNode.Position, 429990145, null))
				{
					basePathNode.linked.Add(basePathNode2);
				}
			}
		}
	}

	// Token: 0x06001881 RID: 6273 RVA: 0x000B7268 File Offset: 0x000B5468
	public void GetNodesNear(Vector3 point, ref List<IAIPathNode> nearNodes, float dist = 10f)
	{
		foreach (BasePathNode basePathNode in this.nodes)
		{
			if ((Vector3Ex.XZ(point) - Vector3Ex.XZ(basePathNode.Position)).sqrMagnitude <= dist * dist)
			{
				nearNodes.Add(basePathNode);
			}
		}
	}

	// Token: 0x06001882 RID: 6274 RVA: 0x000B72E0 File Offset: 0x000B54E0
	public IAIPathNode GetClosestToPoint(Vector3 point)
	{
		IAIPathNode result = this.nodes[0];
		float num = float.PositiveInfinity;
		foreach (BasePathNode basePathNode in this.nodes)
		{
			if (!(basePathNode == null) && !(basePathNode.transform == null))
			{
				float sqrMagnitude = (point - basePathNode.Position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = basePathNode;
				}
			}
		}
		return result;
	}

	// Token: 0x06001883 RID: 6275 RVA: 0x000B7378 File Offset: 0x000B5578
	public IAIPathInterestNode GetRandomInterestNodeAwayFrom(Vector3 from, float dist = 10f)
	{
		PathInterestNode pathInterestNode = null;
		int num = 0;
		while (pathInterestNode == null && num < 20)
		{
			pathInterestNode = this.interestZones[UnityEngine.Random.Range(0, this.interestZones.Count)];
			if ((pathInterestNode.transform.position - from).sqrMagnitude >= dist * dist)
			{
				break;
			}
			pathInterestNode = null;
			num++;
		}
		if (pathInterestNode == null)
		{
			Debug.LogError("REturning default interest zone");
			pathInterestNode = this.interestZones[0];
		}
		return pathInterestNode;
	}

	// Token: 0x06001884 RID: 6276 RVA: 0x0002A2F3 File Offset: 0x000284F3
	public void AddInterestNode(IAIPathInterestNode interestZone)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001885 RID: 6277 RVA: 0x0002A2F3 File Offset: 0x000284F3
	public void AddSpeedZone(IAIPathSpeedZone speedZone)
	{
		throw new NotImplementedException();
	}
}
