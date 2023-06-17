using System;
using UnityEngine;

// Token: 0x0200026A RID: 618
public class ModelConditionTest_WallCornerLeft : ModelConditionTest
{
	// Token: 0x04001548 RID: 5448
	private const string socket = "sockets/stability/2";

	// Token: 0x04001549 RID: 5449
	private static string[] sockets = new string[]
	{
		"wall/sockets/stability/2",
		"wall.half/sockets/stability/2",
		"wall.low/sockets/stability/2",
		"wall.doorway/sockets/stability/2",
		"wall.window/sockets/stability/2"
	};

	// Token: 0x06001C90 RID: 7312 RVA: 0x000C6760 File Offset: 0x000C4960
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink(ModelConditionTest_WallCornerLeft.sockets);
		if (entityLink == null)
		{
			return false;
		}
		BuildingBlock buildingBlock = ent as BuildingBlock;
		if (buildingBlock == null)
		{
			return false;
		}
		bool result = false;
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			EntityLink entityLink2 = entityLink.connections[i];
			BuildingBlock buildingBlock2 = entityLink2.owner as BuildingBlock;
			if (!(buildingBlock2 == null))
			{
				float num = Vector3.SignedAngle(ent.transform.forward, buildingBlock2.transform.forward, Vector3.up);
				if (entityLink2.name.EndsWith("sockets/stability/2"))
				{
					if (num > -10f || num < -100f)
					{
						return false;
					}
				}
				else
				{
					if (num < 10f && num > -10f)
					{
						return false;
					}
					if (num < -10f)
					{
						return false;
					}
					if (buildingBlock2.grade == buildingBlock.grade)
					{
						result = true;
					}
				}
			}
		}
		return result;
	}
}
