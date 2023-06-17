using System;

// Token: 0x02000647 RID: 1607
public class BoundsCheck : PrefabAttribute
{
	// Token: 0x04002644 RID: 9796
	public BoundsCheck.BlockType IsType;

	// Token: 0x06002E82 RID: 11906 RVA: 0x00117923 File Offset: 0x00115B23
	protected override Type GetIndexedType()
	{
		return typeof(BoundsCheck);
	}

	// Token: 0x02000D97 RID: 3479
	public enum BlockType
	{
		// Token: 0x0400481E RID: 18462
		Tree
	}
}
