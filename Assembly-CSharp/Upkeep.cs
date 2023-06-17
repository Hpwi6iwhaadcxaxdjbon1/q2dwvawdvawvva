using System;

// Token: 0x020003E7 RID: 999
public class Upkeep : PrefabAttribute
{
	// Token: 0x04001A6E RID: 6766
	public float upkeepMultiplier = 1f;

	// Token: 0x06002248 RID: 8776 RVA: 0x000DD809 File Offset: 0x000DBA09
	protected override Type GetIndexedType()
	{
		return typeof(Upkeep);
	}
}
