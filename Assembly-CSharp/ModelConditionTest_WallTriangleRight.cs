using System;
using UnityEngine;

// Token: 0x0200026D RID: 621
public class ModelConditionTest_WallTriangleRight : ModelConditionTest
{
	// Token: 0x04001553 RID: 5459
	private const string socket_1 = "wall/sockets/wall-female";

	// Token: 0x04001554 RID: 5460
	private const string socket_2 = "wall/sockets/floor-female/1";

	// Token: 0x04001555 RID: 5461
	private const string socket_3 = "wall/sockets/floor-female/2";

	// Token: 0x04001556 RID: 5462
	private const string socket_4 = "wall/sockets/floor-female/3";

	// Token: 0x04001557 RID: 5463
	private const string socket_5 = "wall/sockets/floor-female/4";

	// Token: 0x04001558 RID: 5464
	private const string socket_6 = "wall/sockets/stability/2";

	// Token: 0x04001559 RID: 5465
	private const string socket = "wall/sockets/neighbour/1";

	// Token: 0x06001C9A RID: 7322 RVA: 0x000C6B18 File Offset: 0x000C4D18
	public static bool CheckCondition(BaseEntity ent)
	{
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/wall-female"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/floor-female/1"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/floor-female/2"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/floor-female/3"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/floor-female/4"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/stability/2"))
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
				if (buildingBlock.blockDefinition.info.name.token == "roof" && Vector3.Angle(ent.transform.forward, -buildingBlock.transform.forward) < 10f)
				{
					return true;
				}
				if (buildingBlock.blockDefinition.info.name.token == "roof_triangle" && Vector3.Angle(ent.transform.forward, -buildingBlock.transform.forward) < 40f)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001C9B RID: 7323 RVA: 0x000C6C64 File Offset: 0x000C4E64
	private static bool CheckSocketOccupied(BaseEntity ent, string socket)
	{
		EntityLink entityLink = ent.FindLink(socket);
		return entityLink != null && !entityLink.IsEmpty();
	}

	// Token: 0x06001C9C RID: 7324 RVA: 0x000C6C87 File Offset: 0x000C4E87
	public override bool DoTest(BaseEntity ent)
	{
		return ModelConditionTest_WallTriangleRight.CheckCondition(ent);
	}
}
