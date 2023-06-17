using System;
using UnityEngine;

// Token: 0x02000262 RID: 610
public class ModelConditionTest_RoofBottom : ModelConditionTest
{
	// Token: 0x04001524 RID: 5412
	private const string roof_square = "roof/";

	// Token: 0x04001525 RID: 5413
	private const string roof_triangle = "roof.triangle/";

	// Token: 0x04001526 RID: 5414
	private const string socket_bot_right = "sockets/neighbour/3";

	// Token: 0x04001527 RID: 5415
	private const string socket_bot_left = "sockets/neighbour/4";

	// Token: 0x04001528 RID: 5416
	private const string socket_top_right = "sockets/neighbour/5";

	// Token: 0x04001529 RID: 5417
	private const string socket_top_left = "sockets/neighbour/6";

	// Token: 0x0400152A RID: 5418
	private static string[] sockets_bot_right = new string[]
	{
		"roof/sockets/neighbour/3",
		"roof.triangle/sockets/neighbour/3"
	};

	// Token: 0x0400152B RID: 5419
	private static string[] sockets_bot_left = new string[]
	{
		"roof/sockets/neighbour/4",
		"roof.triangle/sockets/neighbour/4"
	};

	// Token: 0x06001C72 RID: 7282 RVA: 0x000C5F40 File Offset: 0x000C4140
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(0f, -1.5f, 3f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06001C73 RID: 7283 RVA: 0x000C5F94 File Offset: 0x000C4194
	public override bool DoTest(BaseEntity ent)
	{
		bool flag = false;
		bool flag2 = false;
		EntityLink entityLink = ent.FindLink(ModelConditionTest_RoofBottom.sockets_bot_right);
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			if (entityLink.connections[i].name.EndsWith("sockets/neighbour/5"))
			{
				flag = true;
				break;
			}
		}
		EntityLink entityLink2 = ent.FindLink(ModelConditionTest_RoofBottom.sockets_bot_left);
		if (entityLink2 == null)
		{
			return false;
		}
		for (int j = 0; j < entityLink2.connections.Count; j++)
		{
			if (entityLink2.connections[j].name.EndsWith("sockets/neighbour/6"))
			{
				flag2 = true;
				break;
			}
		}
		return !flag || !flag2;
	}
}
