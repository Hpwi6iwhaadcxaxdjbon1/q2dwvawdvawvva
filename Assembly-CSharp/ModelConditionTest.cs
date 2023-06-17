using System;

// Token: 0x0200025D RID: 605
public abstract class ModelConditionTest : PrefabAttribute
{
	// Token: 0x06001C63 RID: 7267
	public abstract bool DoTest(BaseEntity ent);

	// Token: 0x06001C64 RID: 7268 RVA: 0x000C5C51 File Offset: 0x000C3E51
	protected override Type GetIndexedType()
	{
		return typeof(ModelConditionTest);
	}
}
