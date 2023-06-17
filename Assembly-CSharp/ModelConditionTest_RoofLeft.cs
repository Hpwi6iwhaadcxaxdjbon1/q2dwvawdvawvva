using System;
using UnityEngine;

// Token: 0x02000263 RID: 611
public class ModelConditionTest_RoofLeft : ModelConditionTest
{
	// Token: 0x0400152C RID: 5420
	public ModelConditionTest_RoofLeft.AngleType angle = ModelConditionTest_RoofLeft.AngleType.None;

	// Token: 0x0400152D RID: 5421
	public ModelConditionTest_RoofLeft.ShapeType shape = ModelConditionTest_RoofLeft.ShapeType.Any;

	// Token: 0x0400152E RID: 5422
	private const string roof_square = "roof/";

	// Token: 0x0400152F RID: 5423
	private const string roof_triangle = "roof.triangle/";

	// Token: 0x04001530 RID: 5424
	private const string socket_right = "sockets/neighbour/3";

	// Token: 0x04001531 RID: 5425
	private const string socket_left = "sockets/neighbour/4";

	// Token: 0x04001532 RID: 5426
	private static string[] sockets_left = new string[]
	{
		"roof/sockets/neighbour/4",
		"roof.triangle/sockets/neighbour/4"
	};

	// Token: 0x17000261 RID: 609
	// (get) Token: 0x06001C76 RID: 7286 RVA: 0x000C607F File Offset: 0x000C427F
	private bool IsConvex
	{
		get
		{
			return this.angle > (ModelConditionTest_RoofLeft.AngleType)10;
		}
	}

	// Token: 0x17000262 RID: 610
	// (get) Token: 0x06001C77 RID: 7287 RVA: 0x000C608B File Offset: 0x000C428B
	private bool IsConcave
	{
		get
		{
			return this.angle < (ModelConditionTest_RoofLeft.AngleType)(-10);
		}
	}

	// Token: 0x06001C78 RID: 7288 RVA: 0x000C6098 File Offset: 0x000C4298
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(3f, 1.5f, 0f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06001C79 RID: 7289 RVA: 0x000C60EC File Offset: 0x000C42EC
	public override bool DoTest(BaseEntity ent)
	{
		BuildingBlock buildingBlock = ent as BuildingBlock;
		if (buildingBlock == null)
		{
			return false;
		}
		EntityLink entityLink = ent.FindLink(ModelConditionTest_RoofLeft.sockets_left);
		if (entityLink == null)
		{
			return false;
		}
		if (this.angle == ModelConditionTest_RoofLeft.AngleType.None)
		{
			for (int i = 0; i < entityLink.connections.Count; i++)
			{
				if (entityLink.connections[i].name.EndsWith("sockets/neighbour/3"))
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
			if (entityLink2.name.EndsWith("sockets/neighbour/3") && (this.shape != ModelConditionTest_RoofLeft.ShapeType.Square || entityLink2.name.StartsWith("roof/")) && (this.shape != ModelConditionTest_RoofLeft.ShapeType.Triangle || entityLink2.name.StartsWith("roof.triangle/")))
			{
				BuildingBlock buildingBlock2 = entityLink2.owner as BuildingBlock;
				if (!(buildingBlock2 == null) && buildingBlock2.grade == buildingBlock.grade)
				{
					int num = (int)this.angle;
					float num2 = Vector3.SignedAngle(ent.transform.forward, buildingBlock2.transform.forward, Vector3.up);
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

	// Token: 0x02000C86 RID: 3206
	public enum AngleType
	{
		// Token: 0x040043B2 RID: 17330
		None = -1,
		// Token: 0x040043B3 RID: 17331
		Straight,
		// Token: 0x040043B4 RID: 17332
		Convex60 = 60,
		// Token: 0x040043B5 RID: 17333
		Convex90 = 90,
		// Token: 0x040043B6 RID: 17334
		Convex120 = 120,
		// Token: 0x040043B7 RID: 17335
		Concave30 = -30,
		// Token: 0x040043B8 RID: 17336
		Concave60 = -60,
		// Token: 0x040043B9 RID: 17337
		Concave90 = -90,
		// Token: 0x040043BA RID: 17338
		Concave120 = -120
	}

	// Token: 0x02000C87 RID: 3207
	public enum ShapeType
	{
		// Token: 0x040043BC RID: 17340
		Any = -1,
		// Token: 0x040043BD RID: 17341
		Square,
		// Token: 0x040043BE RID: 17342
		Triangle
	}
}
