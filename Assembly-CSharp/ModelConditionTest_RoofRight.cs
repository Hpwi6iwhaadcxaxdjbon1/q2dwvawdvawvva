using System;
using UnityEngine;

// Token: 0x02000264 RID: 612
public class ModelConditionTest_RoofRight : ModelConditionTest
{
	// Token: 0x04001533 RID: 5427
	public ModelConditionTest_RoofRight.AngleType angle = ModelConditionTest_RoofRight.AngleType.None;

	// Token: 0x04001534 RID: 5428
	public ModelConditionTest_RoofRight.ShapeType shape = ModelConditionTest_RoofRight.ShapeType.Any;

	// Token: 0x04001535 RID: 5429
	private const string roof_square = "roof/";

	// Token: 0x04001536 RID: 5430
	private const string roof_triangle = "roof.triangle/";

	// Token: 0x04001537 RID: 5431
	private const string socket_right = "sockets/neighbour/3";

	// Token: 0x04001538 RID: 5432
	private const string socket_left = "sockets/neighbour/4";

	// Token: 0x04001539 RID: 5433
	private static string[] sockets_right = new string[]
	{
		"roof/sockets/neighbour/3",
		"roof.triangle/sockets/neighbour/3"
	};

	// Token: 0x17000263 RID: 611
	// (get) Token: 0x06001C7C RID: 7292 RVA: 0x000C6297 File Offset: 0x000C4497
	private bool IsConvex
	{
		get
		{
			return this.angle > (ModelConditionTest_RoofRight.AngleType)10;
		}
	}

	// Token: 0x17000264 RID: 612
	// (get) Token: 0x06001C7D RID: 7293 RVA: 0x000C62A3 File Offset: 0x000C44A3
	private bool IsConcave
	{
		get
		{
			return this.angle < (ModelConditionTest_RoofRight.AngleType)(-10);
		}
	}

	// Token: 0x06001C7E RID: 7294 RVA: 0x000C62B0 File Offset: 0x000C44B0
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(-3f, 1.5f, 0f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06001C7F RID: 7295 RVA: 0x000C6304 File Offset: 0x000C4504
	public override bool DoTest(BaseEntity ent)
	{
		BuildingBlock buildingBlock = ent as BuildingBlock;
		if (buildingBlock == null)
		{
			return false;
		}
		EntityLink entityLink = ent.FindLink(ModelConditionTest_RoofRight.sockets_right);
		if (entityLink == null)
		{
			return false;
		}
		if (this.angle == ModelConditionTest_RoofRight.AngleType.None)
		{
			for (int i = 0; i < entityLink.connections.Count; i++)
			{
				if (entityLink.connections[i].name.EndsWith("sockets/neighbour/4"))
				{
					return false;
				}
			}
			return true;
		}
		if (entityLink.IsEmpty())
		{
			return false;
		}
		bool result = false;
		for (int j = 0; j < entityLink.connections.Count; j++)
		{
			EntityLink entityLink2 = entityLink.connections[j];
			if (entityLink2.name.EndsWith("sockets/neighbour/4") && (this.shape != ModelConditionTest_RoofRight.ShapeType.Square || entityLink2.name.StartsWith("roof/")) && (this.shape != ModelConditionTest_RoofRight.ShapeType.Triangle || entityLink2.name.StartsWith("roof.triangle/")))
			{
				BuildingBlock buildingBlock2 = entityLink2.owner as BuildingBlock;
				if (!(buildingBlock2 == null) && buildingBlock2.grade == buildingBlock.grade)
				{
					int num = (int)this.angle;
					float num2 = -Vector3.SignedAngle(ent.transform.forward, buildingBlock2.transform.forward, Vector3.up);
					if (num2 < (float)(num - 10))
					{
						if (this.IsConvex)
						{
							return false;
						}
					}
					else if (num2 > (float)(num + 10))
					{
						if (this.IsConvex)
						{
							return false;
						}
					}
					else
					{
						result = true;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x02000C88 RID: 3208
	public enum AngleType
	{
		// Token: 0x040043C0 RID: 17344
		None = -1,
		// Token: 0x040043C1 RID: 17345
		Straight,
		// Token: 0x040043C2 RID: 17346
		Convex60 = 60,
		// Token: 0x040043C3 RID: 17347
		Convex90 = 90,
		// Token: 0x040043C4 RID: 17348
		Convex120 = 120,
		// Token: 0x040043C5 RID: 17349
		Concave30 = -30,
		// Token: 0x040043C6 RID: 17350
		Concave60 = -60,
		// Token: 0x040043C7 RID: 17351
		Concave90 = -90,
		// Token: 0x040043C8 RID: 17352
		Concave120 = -120
	}

	// Token: 0x02000C89 RID: 3209
	public enum ShapeType
	{
		// Token: 0x040043CA RID: 17354
		Any = -1,
		// Token: 0x040043CB RID: 17355
		Square,
		// Token: 0x040043CC RID: 17356
		Triangle
	}
}
