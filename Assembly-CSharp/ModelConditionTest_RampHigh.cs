using System;
using UnityEngine;

// Token: 0x02000260 RID: 608
public class ModelConditionTest_RampHigh : ModelConditionTest
{
	// Token: 0x04001522 RID: 5410
	private const string socket = "ramp/sockets/block-male/1";

	// Token: 0x06001C6C RID: 7276 RVA: 0x000C5E4C File Offset: 0x000C404C
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(0f, 0.75f, 0f), new Vector3(3f, 1.5f, 3f));
	}

	// Token: 0x06001C6D RID: 7277 RVA: 0x000C5EA0 File Offset: 0x000C40A0
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink("ramp/sockets/block-male/1");
		return entityLink != null && entityLink.IsEmpty();
	}
}
