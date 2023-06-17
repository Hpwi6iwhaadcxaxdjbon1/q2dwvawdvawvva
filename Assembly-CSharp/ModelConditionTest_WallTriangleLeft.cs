using System;
using UnityEngine;

// Token: 0x0200026C RID: 620
public class ModelConditionTest_WallTriangleLeft : ModelConditionTest
{
	// Token: 0x0400154C RID: 5452
	private const string socket_1 = "wall/sockets/wall-female";

	// Token: 0x0400154D RID: 5453
	private const string socket_2 = "wall/sockets/floor-female/1";

	// Token: 0x0400154E RID: 5454
	private const string socket_3 = "wall/sockets/floor-female/2";

	// Token: 0x0400154F RID: 5455
	private const string socket_4 = "wall/sockets/floor-female/3";

	// Token: 0x04001550 RID: 5456
	private const string socket_5 = "wall/sockets/floor-female/4";

	// Token: 0x04001551 RID: 5457
	private const string socket_6 = "wall/sockets/stability/1";

	// Token: 0x04001552 RID: 5458
	private const string socket = "wall/sockets/neighbour/1";

	// Token: 0x06001C96 RID: 7318 RVA: 0x000C69A8 File Offset: 0x000C4BA8
	public static bool CheckCondition(BaseEntity ent)
	{
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/wall-female"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/floor-female/1"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/floor-female/2"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/floor-female/3"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/floor-female/4"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/stability/1"))
		{
			return false;
		}
		EntityLink entityLink = ent.FindLink("wall/sockets/neighbour/1");
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			BuildingBlock buildingBlock = entityLink.connections[i].owner as BuildingBlock;
			if (!(buildingBlock == null))
			{
				if (buildingBlock.blockDefinition.info.name.token == "roof" && Vector3.Angle(ent.transform.forward, buildingBlock.transform.forward) < 10f)
				{
					return true;
				}
				if (buildingBlock.blockDefinition.info.name.token == "roof_triangle" && Vector3.Angle(ent.transform.forward, buildingBlock.transform.forward) < 40f)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001C97 RID: 7319 RVA: 0x000C6AEC File Offset: 0x000C4CEC
	private static bool CheckSocketOccupied(BaseEntity ent, string socket)
	{
		EntityLink entityLink = ent.FindLink(socket);
		return entityLink != null && !entityLink.IsEmpty();
	}

	// Token: 0x06001C98 RID: 7320 RVA: 0x000C6B0F File Offset: 0x000C4D0F
	public override bool DoTest(BaseEntity ent)
	{
		return ModelConditionTest_WallTriangleLeft.CheckCondition(ent);
	}
}
