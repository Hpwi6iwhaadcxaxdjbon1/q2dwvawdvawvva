using System;

// Token: 0x0200025E RID: 606
public class ModelConditionTest_False : ModelConditionTest
{
	// Token: 0x04001519 RID: 5401
	public ConditionalModel reference;

	// Token: 0x06001C66 RID: 7270 RVA: 0x000C5C5D File Offset: 0x000C3E5D
	public override bool DoTest(BaseEntity ent)
	{
		return !this.reference.RunTests(ent);
	}
}
