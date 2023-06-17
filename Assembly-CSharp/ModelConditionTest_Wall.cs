using System;

// Token: 0x02000269 RID: 617
public class ModelConditionTest_Wall : ModelConditionTest
{
	// Token: 0x06001C8E RID: 7310 RVA: 0x000C674B File Offset: 0x000C494B
	public override bool DoTest(BaseEntity ent)
	{
		return !ModelConditionTest_WallTriangleLeft.CheckCondition(ent) && !ModelConditionTest_WallTriangleRight.CheckCondition(ent);
	}
}
