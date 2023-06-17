using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001AD RID: 429
public class RuntimePath : IAIPath
{
	// Token: 0x04001166 RID: 4454
	private List<IAIPathSpeedZone> speedZones = new List<IAIPathSpeedZone>();

	// Token: 0x04001167 RID: 4455
	private List<IAIPathInterestNode> interestNodes = new List<IAIPathInterestNode>();

	// Token: 0x17000217 RID: 535
	// (get) Token: 0x060018BE RID: 6334 RVA: 0x000B7AB7 File Offset: 0x000B5CB7
	// (set) Token: 0x060018BF RID: 6335 RVA: 0x000B7ABF File Offset: 0x000B5CBF
	public IAIPathNode[] Nodes { get; set; } = new IAIPathNode[0];

	// Token: 0x17000218 RID: 536
	// (get) Token: 0x060018C0 RID: 6336 RVA: 0x000B7AC8 File Offset: 0x000B5CC8
	public IEnumerable<IAIPathSpeedZone> SpeedZones
	{
		get
		{
			return this.speedZones;
		}
	}

	// Token: 0x17000219 RID: 537
	// (get) Token: 0x060018C1 RID: 6337 RVA: 0x000B7AD0 File Offset: 0x000B5CD0
	public IEnumerable<IAIPathInterestNode> InterestNodes
	{
		get
		{
			return this.interestNodes;
		}
	}

	// Token: 0x060018C2 RID: 6338 RVA: 0x000B7AD8 File Offset: 0x000B5CD8
	public IAIPathNode GetClosestToPoint(Vector3 point)
	{
		IAIPathNode result = this.Nodes[0];
		float num = float.PositiveInfinity;
		foreach (IAIPathNode iaipathNode in this.Nodes)
		{
			float sqrMagnitude = (point - iaipathNode.Position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = iaipathNode;
			}
		}
		return result;
	}

	// Token: 0x060018C3 RID: 6339 RVA: 0x000B7B34 File Offset: 0x000B5D34
	public void GetNodesNear(Vector3 point, ref List<IAIPathNode> nearNodes, float dist = 10f)
	{
		foreach (IAIPathNode iaipathNode in this.Nodes)
		{
			if ((Vector3Ex.XZ(point) - Vector3Ex.XZ(iaipathNode.Position)).sqrMagnitude <= dist * dist)
			{
				nearNodes.Add(iaipathNode);
			}
		}
	}

	// Token: 0x060018C4 RID: 6340 RVA: 0x000B7B88 File Offset: 0x000B5D88
	public IAIPathInterestNode GetRandomInterestNodeAwayFrom(Vector3 from, float dist = 10f)
	{
		IAIPathInterestNode iaipathInterestNode = null;
		int num = 0;
		while (iaipathInterestNode == null && num < 20)
		{
			iaipathInterestNode = this.interestNodes[UnityEngine.Random.Range(0, this.interestNodes.Count)];
			if ((iaipathInterestNode.Position - from).sqrMagnitude >= dist * dist)
			{
				break;
			}
			iaipathInterestNode = null;
			num++;
		}
		if (iaipathInterestNode == null)
		{
			Debug.LogError("Returning default interest zone");
			iaipathInterestNode = this.interestNodes[0];
		}
		return iaipathInterestNode;
	}

	// Token: 0x060018C5 RID: 6341 RVA: 0x000B7BFA File Offset: 0x000B5DFA
	public void AddInterestNode(IAIPathInterestNode interestNode)
	{
		if (this.interestNodes.Contains(interestNode))
		{
			return;
		}
		this.interestNodes.Add(interestNode);
	}

	// Token: 0x060018C6 RID: 6342 RVA: 0x000B7C17 File Offset: 0x000B5E17
	public void AddSpeedZone(IAIPathSpeedZone speedZone)
	{
		if (this.speedZones.Contains(speedZone))
		{
			return;
		}
		this.speedZones.Add(speedZone);
	}
}
