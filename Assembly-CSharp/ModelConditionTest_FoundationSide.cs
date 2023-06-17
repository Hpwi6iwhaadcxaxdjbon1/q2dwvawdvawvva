using System;
using UnityEngine;

// Token: 0x0200025F RID: 607
public class ModelConditionTest_FoundationSide : ModelConditionTest
{
	// Token: 0x0400151A RID: 5402
	private const string square_south = "foundation/sockets/foundation-top/1";

	// Token: 0x0400151B RID: 5403
	private const string square_north = "foundation/sockets/foundation-top/3";

	// Token: 0x0400151C RID: 5404
	private const string square_west = "foundation/sockets/foundation-top/2";

	// Token: 0x0400151D RID: 5405
	private const string square_east = "foundation/sockets/foundation-top/4";

	// Token: 0x0400151E RID: 5406
	private const string triangle_south = "foundation.triangle/sockets/foundation-top/1";

	// Token: 0x0400151F RID: 5407
	private const string triangle_northwest = "foundation.triangle/sockets/foundation-top/2";

	// Token: 0x04001520 RID: 5408
	private const string triangle_northeast = "foundation.triangle/sockets/foundation-top/3";

	// Token: 0x04001521 RID: 5409
	private string socket = string.Empty;

	// Token: 0x06001C68 RID: 7272 RVA: 0x000C5C78 File Offset: 0x000C3E78
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(1.5f, 1.5f, 0f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06001C69 RID: 7273 RVA: 0x000C5CCC File Offset: 0x000C3ECC
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		Vector3 vector = this.worldRotation * Vector3.right;
		if (name.Contains("foundation.triangle"))
		{
			if (vector.z < -0.9f)
			{
				this.socket = "foundation.triangle/sockets/foundation-top/1";
			}
			if (vector.x < -0.1f)
			{
				this.socket = "foundation.triangle/sockets/foundation-top/2";
			}
			if (vector.x > 0.1f)
			{
				this.socket = "foundation.triangle/sockets/foundation-top/3";
				return;
			}
		}
		else
		{
			if (vector.z < -0.9f)
			{
				this.socket = "foundation/sockets/foundation-top/1";
			}
			if (vector.z > 0.9f)
			{
				this.socket = "foundation/sockets/foundation-top/3";
			}
			if (vector.x < -0.9f)
			{
				this.socket = "foundation/sockets/foundation-top/2";
			}
			if (vector.x > 0.9f)
			{
				this.socket = "foundation/sockets/foundation-top/4";
			}
		}
	}

	// Token: 0x06001C6A RID: 7274 RVA: 0x000C5DA0 File Offset: 0x000C3FA0
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink(this.socket);
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			BuildingBlock buildingBlock = entityLink.connections[i].owner as BuildingBlock;
			if (!(buildingBlock == null) && !(buildingBlock.blockDefinition.info.name.token == "foundation_steps"))
			{
				if (buildingBlock.grade == BuildingGrade.Enum.TopTier)
				{
					return false;
				}
				if (buildingBlock.grade == BuildingGrade.Enum.Metal)
				{
					return false;
				}
				if (buildingBlock.grade == BuildingGrade.Enum.Stone)
				{
					return false;
				}
			}
		}
		return true;
	}
}
