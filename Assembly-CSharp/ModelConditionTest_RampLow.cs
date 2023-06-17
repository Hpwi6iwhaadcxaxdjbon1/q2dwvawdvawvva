using System;
using UnityEngine;

// Token: 0x02000261 RID: 609
public class ModelConditionTest_RampLow : ModelConditionTest
{
	// Token: 0x04001523 RID: 5411
	private const string socket = "ramp/sockets/block-male/1";

	// Token: 0x06001C6F RID: 7279 RVA: 0x000C5EC4 File Offset: 0x000C40C4
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(0f, 0.375f, 0f), new Vector3(3f, 0.75f, 3f));
	}

	// Token: 0x06001C70 RID: 7280 RVA: 0x000C5F18 File Offset: 0x000C4118
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink("ramp/sockets/block-male/1");
		return entityLink != null && !entityLink.IsEmpty();
	}
}
