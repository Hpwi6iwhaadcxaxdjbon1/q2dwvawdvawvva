using System;
using UnityEngine;

// Token: 0x02000265 RID: 613
public class ModelConditionTest_RoofTop : ModelConditionTest
{
	// Token: 0x0400153A RID: 5434
	private const string roof_square = "roof/";

	// Token: 0x0400153B RID: 5435
	private const string roof_triangle = "roof.triangle/";

	// Token: 0x0400153C RID: 5436
	private const string socket_bot_right = "sockets/neighbour/3";

	// Token: 0x0400153D RID: 5437
	private const string socket_bot_left = "sockets/neighbour/4";

	// Token: 0x0400153E RID: 5438
	private const string socket_top_right = "sockets/neighbour/5";

	// Token: 0x0400153F RID: 5439
	private const string socket_top_left = "sockets/neighbour/6";

	// Token: 0x04001540 RID: 5440
	private static string[] sockets_top_right = new string[]
	{
		"roof/sockets/neighbour/5",
		"roof.triangle/sockets/neighbour/5"
	};

	// Token: 0x04001541 RID: 5441
	private static string[] sockets_top_left = new string[]
	{
		"roof/sockets/neighbour/6",
		"roof.triangle/sockets/neighbour/6"
	};

	// Token: 0x06001C82 RID: 7298 RVA: 0x000C64B0 File Offset: 0x000C46B0
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(0f, -1.5f, 3f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06001C83 RID: 7299 RVA: 0x000C6504 File Offset: 0x000C4704
	public override bool DoTest(BaseEntity ent)
	{
		bool flag = false;
		bool flag2 = false;
		EntityLink entityLink = ent.FindLink(ModelConditionTest_RoofTop.sockets_top_right);
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			if (entityLink.connections[i].name.EndsWith("sockets/neighbour/3"))
			{
				flag = true;
				break;
			}
		}
		EntityLink entityLink2 = ent.FindLink(ModelConditionTest_RoofTop.sockets_top_left);
		if (entityLink2 == null)
		{
			return false;
		}
		for (int j = 0; j < entityLink2.connections.Count; j++)
		{
			if (entityLink2.connections[j].name.EndsWith("sockets/neighbour/4"))
			{
				flag2 = true;
				break;
			}
		}
		return !flag || !flag2;
	}
}
