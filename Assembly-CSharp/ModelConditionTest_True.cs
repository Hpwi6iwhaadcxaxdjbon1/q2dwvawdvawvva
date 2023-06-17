using System;

// Token: 0x02000268 RID: 616
public class ModelConditionTest_True : ModelConditionTest
{
	// Token: 0x04001547 RID: 5447
	public ConditionalModel reference;

	// Token: 0x06001C8C RID: 7308 RVA: 0x000C673D File Offset: 0x000C493D
	public override bool DoTest(BaseEntity ent)
	{
		return this.reference.RunTests(ent);
	}
}
