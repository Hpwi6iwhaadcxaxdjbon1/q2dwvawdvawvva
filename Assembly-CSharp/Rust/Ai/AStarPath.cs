using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.AI
{
	// Token: 0x02000B47 RID: 2887
	public static class AStarPath
	{
		// Token: 0x060045FE RID: 17918 RVA: 0x00198A19 File Offset: 0x00196C19
		private static float Heuristic(IAIPathNode from, IAIPathNode to)
		{
			return Vector3.Distance(from.Position, to.Position);
		}

		// Token: 0x060045FF RID: 17919 RVA: 0x00198A2C File Offset: 0x00196C2C
		public static bool FindPath(IAIPathNode start, IAIPathNode goal, out Stack<IAIPathNode> path, out float pathCost)
		{
			path = null;
			pathCost = -1f;
			bool result = false;
			if (start == goal)
			{
				return false;
			}
			AStarNodeList astarNodeList = new AStarNodeList();
			HashSet<IAIPathNode> hashSet = new HashSet<IAIPathNode>();
			AStarNode item = new AStarNode(0f, AStarPath.Heuristic(start, goal), null, start);
			astarNodeList.Add(item);
			while (astarNodeList.Count > 0)
			{
				AStarNode astarNode = astarNodeList[0];
				astarNodeList.RemoveAt(0);
				hashSet.Add(astarNode.Node);
				if (astarNode.Satisfies(goal))
				{
					path = new Stack<IAIPathNode>();
					pathCost = 0f;
					while (astarNode.Parent != null)
					{
						pathCost += astarNode.F;
						path.Push(astarNode.Node);
						astarNode = astarNode.Parent;
					}
					if (astarNode != null)
					{
						path.Push(astarNode.Node);
					}
					result = true;
					break;
				}
				foreach (IAIPathNode iaipathNode in astarNode.Node.Linked)
				{
					if (!hashSet.Contains(iaipathNode))
					{
						float num = astarNode.G + AStarPath.Heuristic(astarNode.Node, iaipathNode);
						AStarNode astarNode2 = astarNodeList.GetAStarNodeOf(iaipathNode);
						if (astarNode2 == null)
						{
							astarNode2 = new AStarNode(num, AStarPath.Heuristic(iaipathNode, goal), astarNode, iaipathNode);
							astarNodeList.Add(astarNode2);
							astarNodeList.AStarNodeSort();
						}
						else if (num < astarNode2.G)
						{
							astarNode2.Update(num, astarNode2.H, astarNode, iaipathNode);
							astarNodeList.AStarNodeSort();
						}
					}
				}
			}
			return result;
		}
	}
}
