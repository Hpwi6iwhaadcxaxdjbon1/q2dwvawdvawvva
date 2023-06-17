using System;

// Token: 0x02000266 RID: 614
public class ModelConditionTest_RoofTriangle : ModelConditionTest
{
	// Token: 0x04001542 RID: 5442
	private const string socket = "roof/sockets/wall-female";

	// Token: 0x06001C86 RID: 7302 RVA: 0x000C65F0 File Offset: 0x000C47F0
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink("roof/sockets/wall-female");
		return entityLink == null || entityLink.IsEmpty();
	}
}
